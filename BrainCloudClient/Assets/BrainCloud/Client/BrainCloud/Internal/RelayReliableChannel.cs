// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

// Uncomment this to have packet log
#define BC_DEBUG_RELAY_LOGS_ENABLED

using System.Runtime.CompilerServices;

// The unit tests use the relay's internal classes directly.
[assembly: InternalsVisibleTo("UnitTest")]

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The reliability layer of one relay link. Going out: packet ids, and resending reliable
    /// packets until they are acknowledged. Coming in: dropping duplicates and putting ordered
    /// packets back in order.
    ///
    /// Packets are in the relay wire format: a 2-byte length prefix, a control byte, then an
    /// 8-byte header (2 bytes of flags, channel and packet id, then a 6-byte player mask whose
    /// low byte is the sender's net id), then the payload.
    /// </summary>
    internal sealed class RelayReliableChannel
    {
        public const int MAX_PACKET_ID = 0xFFF;
        public const int MAX_CHANNELS = 4;

        /// A reliable packet not acknowledged for this long means the link is lost.
        public const int RELIABLE_GIVE_UP_MS = 10000;

        private const int MAX_PACKET_ID_HISTORY = 60 * 10; // So we last 10 seconds at 60 fps
        private const int MAX_RELIABLE_RESEND_INTERVAL = 500;
        private const int PACKET_LOWER_THRESHOLD = MAX_PACKET_ID * 25 / 100;
        private const int PACKET_HIGHER_THRESHOLD = MAX_PACKET_ID * 75 / 100;

        public enum ReceiveOutcome
        {
            Accepted,

            /// Too many packets are waiting for an earlier one that never came: the link should be dropped.
            TooManyOutOfOrder
        }

        private readonly Func<bool> m_loggingEnabled;
        private readonly Action<string> m_log;

        private Dictionary<ulong, int> m_sendPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, int> m_recvPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, UDPPacket> m_reliables = new Dictionary<ulong, UDPPacket>();
        private Dictionary<ulong, List<UDPPacket>> m_orderedReliablePackets = new Dictionary<ulong, List<UDPPacket>>();
        // Recent packet ids of each reliable, unordered stream, to drop resends already received
        private Dictionary<ulong, RecentPacketIds> m_recentUnordered = new Dictionary<ulong, RecentPacketIds>();
        // Which player (connection id) each net id belongs to, as far as we have been told
        private Dictionary<int, string> m_peerCxIds = new Dictionary<int, string>();
        /*
        Tracking packet IDs for each player
        index of array represents channel ids 0 to 3
        Dictionary value has the player netId as the key and the tracked packetId as the value
        Data is structured this way because once it is used to update the client it will then be discarded
        */
        private readonly Dictionary<int, int>[] m_trackedPacketIds = new Dictionary<int, int>[]
        {
            new Dictionary<int, int>(),
            new Dictionary<int, int>(),
            new Dictionary<int, int>(),
            new Dictionary<int, int>()
        };

        /// <param name="in_loggingEnabled">Whether to build and log diagnostic messages; checked each time.</param>
        /// <param name="in_log">Logs a diagnostic message.</param>
        public RelayReliableChannel(Func<bool> in_loggingEnabled, Action<string> in_log)
        {
            m_loggingEnabled = in_loggingEnabled ?? (() => false);
            m_log = in_log ?? (message => { });
        }

        /// Reliable packets sent and not yet acknowledged.
        public int UnacknowledgedCount
        {
            get { return m_reliables.Count; }
        }

        /// Forgets everything about the link, for a new connection.
        public void Reset()
        {
            m_sendPacketId.Clear();
            m_recvPacketId.Clear();
            m_reliables.Clear();
            m_orderedReliablePackets.Clear();
            m_recentUnordered.Clear();
            m_peerCxIds.Clear();
            ClearTrackedPacketIds();
        }

        /// <summary>
        /// Records which player a net id belongs to. When the relay has given the net id to a
        /// different player (it reuses the ids of players who left), what was received from the
        /// old player is forgotten: the new player's packet ids start again at 0, and the old
        /// player's would make them look like duplicates. A repeat for the same player changes
        /// nothing. Call before any tracked packet id for this player is set.
        /// </summary>
        /// <remarks>
        /// This is done when the net id is given out again, not when its player leaves: the relay
        /// keeps resending a leaver's last reliable packets until they are acknowledged, and those
        /// late resends must still be recognised as duplicates.
        /// </remarks>
        public void PeerConnected(int in_netId, string in_cxId)
        {
            string previous;
            if (m_peerCxIds.TryGetValue(in_netId, out previous) && previous != in_cxId)
            {
                ForgetPeer(in_netId);
            }
            m_peerCxIds[in_netId] = in_cxId;
        }

        /// <summary>
        /// Forgets what was received from a net id.
        /// </summary>
        public void ForgetPeer(int in_netId)
        {
            removeStreamsFrom(m_recvPacketId, in_netId);
            removeStreamsFrom(m_orderedReliablePackets, in_netId);
            removeStreamsFrom(m_recentUnordered, in_netId);
            foreach (var tracked in m_trackedPacketIds)
            {
                tracked.Remove(in_netId);
            }
        }

        // A received stream's key ends with the sender's net id
        private static void removeStreamsFrom<T>(Dictionary<ulong, T> in_streams, int in_netId)
        {
            List<ulong> keys = null;
            foreach (ulong key in in_streams.Keys)
            {
                if ((int)(key & 0xFF) == in_netId)
                {
                    if (keys == null) keys = new List<ulong>();
                    keys.Add(key);
                }
            }
            if (keys != null)
            {
                foreach (ulong key in keys)
                {
                    in_streams.Remove(key);
                }
            }
        }

        public void ClearTrackedPacketIds()
        {
            foreach (var tracked in m_trackedPacketIds)
            {
                tracked.Clear();
            }
        }

        #region outgoing

        /// <summary>
        /// The packet id for the next packet of a stream, where the stream is the header without
        /// its packet id (flags, channel and player mask).
        /// </summary>
        public int NextPacketId(ulong in_ackIdWithoutPacketId)
        {
            int packetId = 0;
            if (m_sendPacketId.ContainsKey(in_ackIdWithoutPacketId))
            {
                packetId = m_sendPacketId[in_ackIdWithoutPacketId];
            }
            m_sendPacketId[in_ackIdWithoutPacketId] = (packetId + 1) & MAX_PACKET_ID;
            return packetId;
        }

        /// <summary>
        /// Keeps a reliable packet to resend until it is acknowledged.
        /// </summary>
        /// <param name="in_ackId">The 8 header bytes as sent, read as a native ulong (what an ACK carries back).</param>
        /// <param name="in_packetData">The packet as sent, without the length prefix.</param>
        public void TrackReliable(ulong in_ackId, byte[] in_packetData, int in_channel, int in_packetId, long in_nowMs)
        {
            m_reliables[in_ackId] = new UDPPacket(in_packetData, in_channel, in_packetId, 0, in_nowMs);
        }

        public void Acknowledge(ulong in_ackId)
        {
            m_reliables.Remove(in_ackId);
        }

        /// <summary>
        /// Resends reliable packets whose resend time has come.
        /// </summary>
        /// <returns>False when a packet has gone unacknowledged too long: the link should be dropped.</returns>
        public bool ProcessResends(long in_nowMs, Func<byte[], bool> in_send)
        {
            foreach (var kv in m_reliables)
            {
                UDPPacket packet = kv.Value;
                if (in_nowMs - packet.TimeSinceFirstSend > RELIABLE_GIVE_UP_MS)
                {
                    return false;
                }
                if (in_nowMs - packet.LastTimeSent > packet.TimeInterval)
                {
                    packet.UpdateTimeIntervalSent(in_nowMs);
                    in_send(packet.RawData);
                }
            }
            return true;
        }

        #endregion

        #region incoming

        /// <summary>
        /// The packet id a peer's ordered stream on this channel had reached when we joined, so the
        /// next packet from it is taken as in order.
        /// </summary>
        public void SetTrackedPacketId(int in_channel, int in_netId, int in_packetId)
        {
            m_trackedPacketIds[in_channel][in_netId] = in_packetId;
        }

        /// <summary>
        /// Takes a received RELAY packet and hands the packets that are now ready to the game, in
        /// order. A duplicate is dropped; an ordered packet that arrives early is held until the
        /// ones before it have arrived.
        /// </summary>
        /// <param name="in_data">The packet as received, with its length prefix.</param>
        /// <param name="in_deliver">Called for each packet ready for the game, with the sender's net id.</param>
        public ReceiveOutcome Receive(byte[] in_data, Action<short, byte[]> in_deliver)
        {
            ushort rh = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[4] : in_data[3],
                BitConverter.IsLittleEndian ? in_data[3] : in_data[4]}, 0);
            ushort playerMask0 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[6] : in_data[5],
                BitConverter.IsLittleEndian ? in_data[5] : in_data[6]}, 0);
            ushort playerMask1 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[8] : in_data[7],
                BitConverter.IsLittleEndian ? in_data[7] : in_data[8]}, 0);
            ushort playerMask2 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[10] : in_data[9],
                BitConverter.IsLittleEndian ? in_data[9] : in_data[10]}, 0);

            ulong ackId =
                (((ulong)rh << 48) & 0xFFFF000000000000) |
                (((ulong)playerMask0 << 32) & 0x0000FFFF00000000) |
                (((ulong)playerMask1 << 16) & 0x00000000FFFF0000) |
                (((ulong)playerMask2) & 0x000000000000FFFF);
            ulong ackIdWithoutPacketId = ackId & 0xF000FFFFFFFFFFFF;
            bool reliable = ((rh & RelayComms.RELIABLE_BIT) != 0) ? true : false;
            bool ordered = ((rh & RelayComms.ORDERED_BIT) != 0) ? true : false;
            int channel = (rh >> 12) & 0x3;
            int packetId = rh & 0xFFF;
            byte netId = (byte)(playerMask2 & 0x00FF);

            if (ordered)
            {
                int prevPacketId = MAX_PACKET_ID;
                if (m_recvPacketId.ContainsKey(ackIdWithoutPacketId))
                {
                    prevPacketId = m_recvPacketId[ackIdWithoutPacketId];
                }

                if (m_trackedPacketIds[channel].ContainsKey(netId))
                {
                    prevPacketId = m_trackedPacketIds[channel][netId];
                    m_trackedPacketIds[channel].Remove(netId);
                    if (m_loggingEnabled()) m_log($"Found tracked packetId for channel: {channel} netId: {netId} which was {prevPacketId}");
                }

                if (reliable)
                {
                    if (packetLE(packetId, prevPacketId))
                    {
                        // We already received that packet if it's lower than the last confirmed
                        // packetId. This must be a duplicate
#if BC_DEBUG_RELAY_LOGS_ENABLED
                        if (m_loggingEnabled()) m_log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
#endif
                        return ReceiveOutcome.Accepted;
                    }

                    // Check if it's out of order, then save it for later
                    if (!m_orderedReliablePackets.ContainsKey(ackIdWithoutPacketId))
                    {
                        m_orderedReliablePackets[ackIdWithoutPacketId] = new List<UDPPacket>();
                    }
                    List<UDPPacket> orderedReliablePackets = m_orderedReliablePackets[ackIdWithoutPacketId];
                    if (packetId != ((prevPacketId + 1) & MAX_PACKET_ID))
                    {
                        if ((int)orderedReliablePackets.Count > MAX_PACKET_ID_HISTORY)
                        {
                            return ReceiveOutcome.TooManyOutOfOrder;
                        }

                        int insertIdx = 0;
                        for (; insertIdx < (int)orderedReliablePackets.Count; ++insertIdx)
                        {
                            var packet = orderedReliablePackets[insertIdx];
                            if (packet.Id == packetId)
                            {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                                if (m_loggingEnabled()) m_log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
#endif
                                return ReceiveOutcome.Accepted;
                            }
                            if (packetLE(packetId, packet.Id)) break;
                        }
                        var newPacket = new UDPPacket(in_data, channel, packetId, netId, 0);
                        orderedReliablePackets.Insert(insertIdx, newPacket);
#if BC_DEBUG_RELAY_LOGS_ENABLED
                        if (m_loggingEnabled()) m_log("Queuing out of order reliable from " + netId.ToString() + ". got " + packetId.ToString());
#endif
                        return ReceiveOutcome.Accepted;
                    }

                    // It's in order, queue event
                    m_recvPacketId[ackIdWithoutPacketId] = packetId;
                    in_deliver(netId, in_data);

                    // Empty previously queued packets if they follow this one
                    while (orderedReliablePackets.Count > 0)
                    {
                        var packet = orderedReliablePackets[0];
                        if (packet.Id == ((packetId + 1) & MAX_PACKET_ID))
                        {
                            in_deliver(packet.NetId, packet.RawData);
                            orderedReliablePackets.RemoveAt(0);
                            packetId = packet.Id;
                            m_recvPacketId[ackIdWithoutPacketId] = packetId;
                            continue;
                        }
                        break; // Out of order
                    }
                    return ReceiveOutcome.Accepted;
                }
                else
                {
                    if (packetLE(packetId, prevPacketId))
                    {
                        // Just drop out of order packets for unreliables
#if BC_DEBUG_RELAY_LOGS_ENABLED
                        if (m_loggingEnabled()) m_log("Out of order packet from " + netId.ToString() + ". Expecting " + ((prevPacketId + 1) & MAX_PACKET_ID).ToString() + ", got " + packetId.ToString());
#endif
                        return ReceiveOutcome.Accepted;
                    }
                    m_recvPacketId[ackIdWithoutPacketId] = packetId;
                }
            }
            else if (reliable)
            {
                // A resend whose acknowledgement was lost arrives again: deliver it once
                RecentPacketIds recent;
                if (!m_recentUnordered.TryGetValue(ackIdWithoutPacketId, out recent))
                {
                    recent = new RecentPacketIds(MAX_PACKET_ID_HISTORY);
                    m_recentUnordered[ackIdWithoutPacketId] = recent;
                }
                if (!recent.Add(packetId))
                {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                    if (m_loggingEnabled()) m_log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
#endif
                    return ReceiveOutcome.Accepted;
                }
            }

            in_deliver(netId, in_data);
            return ReceiveOutcome.Accepted;
        }

        #endregion

        /// True when packet id a comes at or before b, allowing for the ids wrapping round.
        public static bool packetLE(int a, int b)
        {
            if (a > PACKET_HIGHER_THRESHOLD && b <= PACKET_LOWER_THRESHOLD)
            {
                return true;
            }
            if (b > PACKET_HIGHER_THRESHOLD && a <= PACKET_LOWER_THRESHOLD)
            {
                return false;
            }
            return a <= b;
        }

        /// The last few packet ids of a stream. Ids wrap at MAX_PACKET_ID, so only recent ones mean anything.
        private class RecentPacketIds
        {
            private readonly int m_capacity;
            private readonly HashSet<int> m_ids = new HashSet<int>();
            private readonly Queue<int> m_order = new Queue<int>();

            public RecentPacketIds(int in_capacity)
            {
                m_capacity = in_capacity;
            }

            /// False when the id is already among the recent ones.
            public bool Add(int in_packetId)
            {
                if (!m_ids.Add(in_packetId))
                {
                    return false;
                }
                m_order.Enqueue(in_packetId);
                if (m_order.Count > m_capacity)
                {
                    m_ids.Remove(m_order.Dequeue());
                }
                return true;
            }
        }

        private class UDPPacket
        {
            public UDPPacket(byte[] in_data, int in_channel, int in_packetId, byte in_netId, long in_nowMs)
            {
                LastTimeSent = in_nowMs;
                TimeSinceFirstSend = in_nowMs;
                TimeInterval = in_channel <= 1 ? 50 : in_channel == 2 ? 150 : 250;// ms
                RawData = in_data;
                Id = in_packetId;
                NetId = in_netId;
            }

            public void UpdateTimeIntervalSent(long in_nowMs)
            {
                LastTimeSent = in_nowMs;
                TimeInterval = Math.Min((int)(TimeInterval * 1.25f), MAX_RELIABLE_RESEND_INTERVAL);
            }
            public long TimeSinceFirstSend { get; private set; }
            public long LastTimeSent { get; private set; }
            public int TimeInterval { get; private set; }
            public byte[] RawData { get; private set; }
            public int Id { get; private set; }
            public byte NetId { get; private set; }
        }
    }
}
