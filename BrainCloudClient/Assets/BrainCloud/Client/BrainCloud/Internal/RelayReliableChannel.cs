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
    /// Reliability for one relay link: packet ids and resends going out; duplicate dropping and
    /// reordering coming in. Packets use relay framing: 2-byte length, control byte, 8-byte header
    /// (2 bytes flags/channel/packet id, 6-byte player mask with the sender's net id in the low byte), payload.
    /// </summary>
    internal sealed class RelayReliableChannel
    {
        public const int MAX_PACKET_ID = 0xFFF;
        public const int MAX_CHANNELS = 4;

        /// A reliable packet unacknowledged this long means the link is lost.
        public const int RELIABLE_GIVE_UP_MS = 10000;

        private const int MAX_PACKET_ID_HISTORY = 60 * 10; // So we last 10 seconds at 60 fps
        // PeerToPeer limits on what one peer can make this player hold
        private const int MAX_PEER_STREAMS = 4096;
        private const int MAX_PEER_HELD_PACKETS = 2 * MAX_PACKET_ID_HISTORY;
        private const int MAX_RELIABLE_RESEND_INTERVAL = 500;
        private const int PACKET_LOWER_THRESHOLD = MAX_PACKET_ID * 25 / 100;
        private const int PACKET_HIGHER_THRESHOLD = MAX_PACKET_ID * 75 / 100;

        public enum ReceiveOutcome
        {
            Accepted,

            /// Too many packets are held waiting for a missing one: drop the link.
            TooManyOutOfOrder
        }

        private readonly Func<bool> m_loggingEnabled;
        private readonly Action<string> m_log;

        private Dictionary<ulong, int> m_sendPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, int> m_recvPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, UDPPacket> m_reliables = new Dictionary<ulong, UDPPacket>();
        private Dictionary<ulong, List<UDPPacket>> m_orderedReliablePackets = new Dictionary<ulong, List<UDPPacket>>();
        // Recent reliable unordered headers, so a resend is not delivered twice (PeerToPeer only)
        private readonly HashSet<ulong> m_unorderedSeen = new HashSet<ulong>();
        private readonly Queue<ulong> m_unorderedSeenOrder = new Queue<ulong>();
        // Ordered packets held across all streams
        private int m_heldCount = 0;
        // Ordered streams received from (PeerToPeer only)
        private readonly HashSet<ulong> m_peerStreams = new HashSet<ulong>();
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

        /// <param name="in_loggingEnabled">Checked before each log message is built.</param>
        public RelayReliableChannel(Func<bool> in_loggingEnabled, Action<string> in_log)
        {
            m_loggingEnabled = in_loggingEnabled ?? (() => false);
            m_log = in_log ?? (message => { });
        }

        /// <summary>
        /// For a direct link with no relay server: drops repeated reliable unordered packets (a relay
        /// does this itself) and caps streams and held packets (exceeding returns TooManyOutOfOrder).
        /// </summary>
        public bool PeerToPeer { get; set; }

        public int UnacknowledgedCount
        {
            get { return m_reliables.Count; }
        }

        /// Clears all link state, for a new connection.
        public void Reset()
        {
            m_sendPacketId.Clear();
            m_recvPacketId.Clear();
            m_reliables.Clear();
            m_orderedReliablePackets.Clear();
            m_unorderedSeen.Clear();
            m_unorderedSeenOrder.Clear();
            m_heldCount = 0;
            m_peerStreams.Clear();
            ClearTrackedPacketIds();
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
        /// Next packet id for a stream: the header without its packet id (flags, channel, player mask).
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
        /// <param name="in_ackId">The 8 header bytes as sent, read as a native ulong; an ACK echoes them.</param>
        /// <param name="in_packetData">Without the length prefix.</param>
        public void TrackReliable(ulong in_ackId, byte[] in_packetData, int in_channel, int in_packetId, long in_nowMs)
        {
            m_reliables[in_ackId] = new UDPPacket(in_packetData, in_channel, in_packetId, 0, in_nowMs);
        }

        public void Acknowledge(ulong in_ackId)
        {
            m_reliables.Remove(in_ackId);
        }

        /// <summary>
        /// Resends reliable packets that are due.
        /// </summary>
        /// <returns>False if a packet has gone unacknowledged too long: drop the link.</returns>
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
        /// The packet id a peer's ordered stream had reached when we joined, so its next packet counts as in order.
        /// </summary>
        public void SetTrackedPacketId(int in_channel, int in_netId, int in_packetId)
        {
            m_trackedPacketIds[in_channel][in_netId] = in_packetId;
        }

        /// <summary>
        /// Delivers a received RELAY packet and any it releases, in order. Duplicates are dropped;
        /// early ordered packets are held until the gap fills.
        /// </summary>
        /// <param name="in_data">With its length prefix.</param>
        /// <param name="in_deliver">Called per ready packet, with the sender's net id.</param>
        public ReceiveOutcome Receive(byte[] in_data, Action<short, byte[]> in_deliver)
        {
            // Big-endian on the wire
            ushort rh = (ushort)((in_data[3] << 8) | in_data[4]);
            ushort playerMask0 = (ushort)((in_data[5] << 8) | in_data[6]);
            ushort playerMask1 = (ushort)((in_data[7] << 8) | in_data[8]);
            ushort playerMask2 = (ushort)((in_data[9] << 8) | in_data[10]);

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
                if (PeerToPeer && !m_peerStreams.Contains(ackIdWithoutPacketId))
                {
                    if (m_peerStreams.Count >= MAX_PEER_STREAMS) return ReceiveOutcome.TooManyOutOfOrder;
                    m_peerStreams.Add(ackIdWithoutPacketId);
                }

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
                        if ((int)orderedReliablePackets.Count > MAX_PACKET_ID_HISTORY ||
                            (PeerToPeer && m_heldCount >= MAX_PEER_HELD_PACKETS))
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
                        ++m_heldCount;
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
                            --m_heldCount;
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
            else if (reliable && PeerToPeer)
            {
                // A repeated header is a resend whose ACK was lost. The history is far shorter than a
                // stream's id wrap.
                if (!m_unorderedSeen.Add(ackId))
                {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                    if (m_loggingEnabled()) m_log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
#endif
                    return ReceiveOutcome.Accepted;
                }
                m_unorderedSeenOrder.Enqueue(ackId);
                if (m_unorderedSeenOrder.Count > MAX_PACKET_ID_HISTORY)
                {
                    m_unorderedSeen.Remove(m_unorderedSeenOrder.Dequeue());
                }
            }

            in_deliver(netId, in_data);
            return ReceiveOutcome.Accepted;
        }

        #endregion

        /// True when packet id a is at or before b, allowing for wraparound.
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
