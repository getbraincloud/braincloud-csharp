// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using BrainCloud.JsonFx.Json;

    /// <summary>
    /// What a direct mesh reports back to RelayComms, on the main thread, from inside its own
    /// calls: RelayComms queues each one and handles it after the mesh has returned.
    /// </summary>
    internal interface IDirectMeshListener
    {
        /// <summary>
        /// A system message in the relay server's own form: CONNECT (this player first, then each
        /// peer as it connects), DISCONNECT, MIGRATE_OWNER and END_MATCH.
        /// </summary>
        void OnMeshSystem(string in_json);

        /// <summary>
        /// Game data from in_netId, in the layout RelayComms receives from a relay server: a 2-byte
        /// length prefix, the RELAY byte, the 8-byte header, then the data.
        /// </summary>
        void OnMeshRelay(short in_netId, byte[] in_packet);

        /// The connection cannot go on.
        void OnMeshError(string in_message);
    }

    /// <summary>
    /// The connection that carries a direct-connected lobby's game traffic (client design,
    /// Confluence BCSD 4389666817, sections 6 and 7): one UDP socket, the one the attempt reached
    /// every peer on, shared by all of them. It takes the place of the relay server:
    /// <list type="bullet">
    /// <item>a send to several players writes one copy to each, and stamps this player's net id
    /// in it, which the relay does otherwise;</item>
    /// <item>reliable packets are resent, acknowledged and given up on for each peer separately,
    /// with one RelayReliableChannel per peer, so the RELAY and ACK packets stay byte for byte the
    /// relay's;</item>
    /// <item>it raises the relay's system messages (CONNECT, DISCONNECT, MIGRATE_OWNER, END_MATCH)
    /// itself, so the game's callbacks need no change;</item>
    /// <item>for a lobby that outlives its match, the owner keeps the lobby alive with brainCloud
    /// and tells it when the match ends, which the relay server does otherwise.</item>
    /// </list>
    /// Every datagram is sealed with the attempt's DirectSession, and one that does not open is
    /// dropped. Packets go to the address of the latest datagram from the peer that opened, so a
    /// datagram that does not open can never redirect the traffic.
    ///
    /// Driven from RelayComms on the main thread; the socket is read without blocking in Update.
    /// </summary>
    internal sealed class DirectMeshTransport
    {
        public const byte RELAY = 2;
        public const byte ACK = 3;
        public const byte PING = 20;
        public const byte PONG = 21;
        public const byte DISCONNECT = 22;
        public const byte END_MATCH = 23;
        public const byte OWNER = 24;
        public const byte CONTROL_ACK = 25;

        /// A connected peer not heard from for this long is lost, as with the relay.
        public const int PEER_TIMEOUT_MS = 10000;
        /// <summary>
        /// A peer that has not connected this long after this player did is given up on: as long as
        /// its attempt's socket waits to be taken over, so a peer still able to connect never is.
        /// </summary>
        public const int CONNECT_TIMEOUT_MS = DirectConnector.CONNECTED_IDLE_MS;
        /// END_MATCH and OWNER are resent this often until acknowledged...
        public const int CONTROL_RESEND_MS = 200;
        /// ...for this long at most.
        public const int CONTROL_GIVE_UP_MS = 5000;
        /// After leaving, peers leaving at the same moment are listened for this long, so one of them tells brainCloud the match stopped.
        public const int LEAVE_LINGER_MS = 1000;
        /// After the match ends, repeats of END_MATCH are still acknowledged for this long.
        public const int END_MATCH_LINGER_MS = 1000;
        /// How many copies of DISCONNECT are sent; the first that arrives counts.
        public const int DISCONNECT_COPIES = 3;
        /// How EndMatch's refusal of a payload too big for one datagram begins.
        public const string END_MATCH_TOO_BIG = "the END_MATCH payload is too big: ";
        /// The most packet bytes one datagram can carry.
        public const int MAX_PACKET_LENGTH = DirectPacket.MAX_DATAGRAM_LENGTH - DirectPacket.OVERHEAD;

        // RELAY byte, then the relay's 8-byte header
        private const int RELAY_HEADER_LENGTH = 9;
        // The relay's own layout puts a 2-byte length prefix before the RELAY byte
        private const int LENGTH_PREFIX = 2;
        private const int MAX_RECEIVES_PER_UPDATE = 256;
        // Control ids remembered per peer, so a repeat is acknowledged but not acted on again
        private const int MAX_CONTROLS_SEEN = 32;

        private sealed class MeshPeer
        {
            public string CxId;
            public int NetId;
            public IPEndPoint Address;
            // The epoch count and sequence number of the datagram Address came from (see moveAddress)
            public bool AddressFromDatagram;
            public uint AddressEpochCount;
            public ulong AddressSequence;
            public bool Connected;
            public bool Lost;
            // Said it was leaving after this player had said so too: the two left together
            public bool LeftWithThisPlayer;
            public long LastHeardMs;
            public int RoundTripMs = -1;
            public RelayReliableChannel Channel;
            // Seals a packet the channel keeps and sends it to this peer
            public Func<byte[], bool> Resend;
            public readonly List<ushort> ControlsSeen = new List<ushort>();
        }

        private sealed class Control
        {
            public byte[] Packet;
            public readonly List<MeshPeer> Awaiting = new List<MeshPeer>();
            public long FirstSentMs;
            public long LastSentMs;
        }

        private readonly BrainCloudClient m_clientRef;
        private readonly DirectLobbyOwner m_lobbyOwner;
        private readonly Action<ServerCall> m_send;
        private readonly IDirectMeshListener m_listener;
        private readonly Socket m_socket;
        private readonly DirectSession m_session;
        private readonly string m_lobbyId;
        private readonly string m_myCxId;
        private readonly int m_myNetId;
        private readonly int m_keepAliveMs;
        private readonly string m_version;
        private readonly List<MeshPeer> m_peers = new List<MeshPeer>();
        private readonly Dictionary<int, MeshPeer> m_peersByNetId = new Dictionary<int, MeshPeer>();
        private readonly List<Control> m_controls = new List<Control>();
        private readonly byte[] m_buffer = new byte[DirectPacket.MAX_DATAGRAM_LENGTH + 1];
        private readonly Action<short, byte[]> m_deliver;
        private string m_ownerCxId;
        private ushort m_nextControlId = 0;
        private long m_startMs;
        private long m_lastPingMs;
        private long m_lastKeepAliveMs;
        private long m_lingerUntilMs;
        private bool m_started;
        private bool m_closing;
        private bool m_closed;
        private bool m_stoppedSent;
        // The owner is one this player elected, not the one DIRECT_CONNECT named
        private bool m_ownerElected;
        // This player ended the match, or the owner did: nothing more is sent, or taken in, but
        // what finishes END_MATCH
        private bool m_endMatchSent;
        private bool m_matchEnded;
        // This player said it is leaving (SendDisconnect)
        private bool m_leaving;
        // After Close, still listening for peers leaving at the same moment
        private bool m_lingerForLeavers;
        // This player owns the match because it was given it, or its owner left: not because it
        // stopped hearing from the owner, which may be its own path failing while the owner plays on
        private bool m_ownershipConfirmed = true;
        // brainCloud refused a keep-alive: the lobby is no longer running this match
        private bool m_keepAliveRefused;
        // Whether the last peer lost said it was leaving. When every peer is lost and the last one
        // went quiet instead, this player may be the one cut off, while the others play on.
        private bool m_lastLossGraceful;
        // END_MATCH from the member this player's election will make owner once it loses the
        // current one, which has not happened yet here: applied when it does
        private string m_pendingEndMatchFrom;
        private object m_pendingEndMatch;

        /// <param name="in_connection">What the attempt handed over (DirectConnector.TakeConnection).</param>
        /// <param name="in_myCxId">This player's RTT connection id.</param>
        /// <param name="in_listener">Where system messages, game data and errors go.</param>
        /// <param name="in_client">For logging, the library's version and the lobby calls; may be null in the tests.</param>
        /// <param name="in_send">Sends the lobby calls; null to send them through in_client.</param>
        internal DirectMeshTransport(DirectConnection in_connection, string in_myCxId, IDirectMeshListener in_listener, BrainCloudClient in_client, Action<ServerCall> in_send = null)
        {
            m_clientRef = in_client;
            m_lobbyOwner = in_connection.LobbyOwner;
            m_send = in_send ?? (in_client != null ? (Action<ServerCall>)in_client.SendRequest : null);
            m_listener = in_listener;
            m_socket = in_connection.Socket;
            m_session = in_connection.Session;
            m_lobbyId = in_connection.LobbyId;
            m_ownerCxId = in_connection.OwnerCxId;
            m_keepAliveMs = Math.Max(0, in_connection.KeepAliveRateSecs) * 1000;
            m_myCxId = in_myCxId;
            m_myNetId = m_session.NetId;
            m_version = in_client != null ? in_client.BrainCloudClientVersion : null;
            m_deliver = deliver;
            var peers = new List<DirectPeer>(in_connection.Peers);
            peers.Sort((a, b) => a.NetId.CompareTo(b.NetId));
            foreach (DirectPeer source in peers)
            {
                var peer = new MeshPeer
                {
                    CxId = source.CxId,
                    NetId = source.NetId,
                    // With where it came from, so only a later datagram moves it, across the handover too
                    Address = source.Address,
                    AddressFromDatagram = source.AddressFromDatagram,
                    AddressEpochCount = source.AddressEpochCount,
                    AddressSequence = source.AddressSequence,
                    Channel = new RelayReliableChannel(() => loggingEnabled, log) { PeerToPeer = true }
                };
                peer.Resend = packet => sendTo(peer, packet);
                m_peers.Add(peer);
                m_peersByNetId[peer.NetId] = peer;
            }
            PingIntervalMs = 1000;
        }

        /// How often each peer is pinged, to keep the path open and measure the round trip.
        public int PingIntervalMs { get; set; }

        /// The longest round trip to a connected peer, in milliseconds, or -1 before any is measured.
        public int RoundTripMs
        {
            get
            {
                int longest = -1;
                foreach (MeshPeer peer in m_peers)
                {
                    if (peer.Connected && !peer.Lost && peer.RoundTripMs > longest) longest = peer.RoundTripMs;
                }
                return longest;
            }
        }

        /// The current owner's RTT connection id.
        public string OwnerCxId { get { return m_ownerCxId; } }

        /// True once Close has finished and the socket is closed.
        public bool IsClosed { get { return m_closed; } }

        /// For the tests: drops a datagram this would send to a peer's net id when it returns true.
        internal Func<int, byte[], bool> DropForTests { get; set; }

        private bool isOwner { get { return string.Equals(m_ownerCxId, m_myCxId, StringComparison.Ordinal); } }

        // brainCloud's lobby owner, which changes only when a member leaves the lobby
        private bool isLobbyOwner { get { return string.Equals(m_lobbyOwner.CxId, m_myCxId, StringComparison.Ordinal); } }

        /// The lobby this connects.
        public string LobbyId { get { return m_lobbyId; } }

        private bool loggingEnabled { get { return m_clientRef != null && m_clientRef.LoggingEnabled; } }

        private void log(string in_message)
        {
            if (loggingEnabled) m_clientRef.Log("Direct mesh: " + in_message);
        }

        /// <summary>
        /// Starts the connection: raises this player's CONNECT, and pings every peer at once, so each
        /// one connects as soon as it hears from this player.
        /// </summary>
        public void Start(long in_nowMs)
        {
            if (m_started) return;
            m_started = true;
            m_startMs = in_nowMs;
            m_lastPingMs = in_nowMs;
            log("lobby " + m_lobbyId + ": net id " + m_myNetId + ", " + m_peers.Count + " peer(s)");

            var connect = new Dictionary<string, object>();
            connect["op"] = "CONNECT";
            connect["cxId"] = m_myCxId;
            connect["ownerCxId"] = m_ownerCxId;
            connect["netId"] = m_myNetId;
            if (m_version != null) connect["version"] = m_version;
            addProfileId(connect, "profileId", m_myCxId);
            addProfileId(connect, "ownerId", m_ownerCxId);
            m_listener.OnMeshSystem(JsonWriter.Serialize(connect));

            ping(in_nowMs);
            if (m_keepAliveMs > 0 && (isOwner || isLobbyOwner)) keepAlive(in_nowMs);
        }

        #region sending

        /// <summary>
        /// Sends game data as the relay would: one copy to each connected peer whose bit is set in
        /// in_playerMask, each with its own packet id, and to this player itself when its own bit is
        /// set, as the relay echoes it. A peer that has not connected yet is skipped, as the relay
        /// skips a member that has not connected.
        /// </summary>
        public void SendRelay(byte[] in_data, ulong in_playerMask, bool in_reliable, bool in_ordered, int in_channel, long in_nowMs)
        {
            if (m_closing) return;
            ushort flags = 0;
            if (in_reliable) flags |= RelayComms.RELIABLE_BIT;
            if (in_ordered) flags |= RelayComms.ORDERED_BIT;
            flags |= (ushort)((in_channel << 12) & 0x3000);
            ulong mask = RelayComms.EncodePlayerMask(in_playerMask);
            ulong stream = ((ulong)flags << 48) | mask;

            foreach (MeshPeer peer in m_peers)
            {
                if (((in_playerMask >> peer.NetId) & 1) == 0 || !peer.Connected || peer.Lost) continue;
                int packetId = peer.Channel.NextPacketId(stream);
                byte[] packet = relayPacket((ushort)(flags | packetId), mask, in_data);
                sendTo(peer, packet);
                if (in_reliable)
                {
                    // The header as sent, net id stamped: what the peer's ACK echoes
                    peer.Channel.TrackReliable(BitConverter.ToUInt64(packet, 1), packet, in_channel, packetId, in_nowMs);
                }
            }

            if (((in_playerMask >> m_myNetId) & 1) != 0)
            {
                m_listener.OnMeshRelay((short)m_myNetId, withLengthPrefix(relayPacket(flags, mask, in_data)));
            }
        }

        // The RELAY byte, the 8-byte header with this player's net id in the low byte of the mask, then the data
        private byte[] relayPacket(ushort in_flagsAndId, ulong in_mask, byte[] in_data)
        {
            byte[] packet = new byte[RELAY_HEADER_LENGTH + in_data.Length];
            packet[0] = RELAY;
            packet[1] = (byte)(in_flagsAndId >> 8);
            packet[2] = (byte)in_flagsAndId;
            ulong mask = (in_mask & 0x0000FFFFFFFFFF00UL) | (uint)m_myNetId;
            for (int i = 0; i < 6; ++i)
            {
                packet[3 + i] = (byte)(mask >> (40 - 8 * i));
            }
            Buffer.BlockCopy(in_data, 0, packet, RELAY_HEADER_LENGTH, in_data.Length);
            return packet;
        }

        /// <summary>
        /// Ends the match, for the owner: sends END_MATCH to every connected peer until each
        /// acknowledges it, tells brainCloud the match has stopped (for a lobby that outlives its
        /// match), and raises END_MATCH for this player, as the relay does. Returns why it cannot,
        /// or null: only the owner may end the match, as with the relay, which ignores anyone else.
        /// </summary>
        public string EndMatch(Dictionary<string, object> in_payload, long in_nowMs)
        {
            // After Disconnect, as with the relay, which takes no END_MATCH once its sender has left
            if (m_closing || m_leaving) return "the connection is closing";
            // Once: a second call in the same frame would raise END_MATCH again
            if (m_endMatchSent || m_matchEnded) return null;
            if (!isOwner) return "only the owner may end the match";
            Dictionary<string, object> payload = in_payload ?? new Dictionary<string, object>();
            byte[] json = Encoding.UTF8.GetBytes(JsonWriter.Serialize(payload));
            if (3 + json.Length > MAX_PACKET_LENGTH)
            {
                return END_MATCH_TOO_BIG + json.Length + " bytes, at most " + (MAX_PACKET_LENGTH - 3);
            }
            sendControl(END_MATCH, json, in_nowMs);
            m_endMatchSent = true;
            // Not when this player may be the one cut off, from everyone or from the owner it took
            // over from: brainCloud would start the lobby's next round while the others play on
            if (m_ownershipConfirmed && (!everyPeerLost() || m_lastLossGraceful)) roomStopped();
            raiseEndMatch(payload, in_nowMs, false);
            return null;
        }

        /// <summary>
        /// Tells brainCloud the match stopped, for a lobby that outlives its match, so it returns to
        /// setup: from the owner who ends it, or the last player to leave. brainCloud takes it from
        /// any member, once; a repeat from another member is refused and changes nothing.
        /// </summary>
        private void roomStopped()
        {
            if (m_keepAliveMs <= 0 || m_stoppedSent) return;
            m_stoppedSent = true;
            postLobby(ServiceOperation.RoomStopped, "telling brainCloud the match stopped failed: ");
        }

        /// <summary>
        /// Tells every peer this player is leaving. Close follows. Sent three times: a peer that
        /// misses it takes this player to have gone quiet, which it treats less kindly than leaving.
        /// </summary>
        public void SendDisconnect()
        {
            // After END_MATCH, as the relay: the match is over for everyone, and a DISCONNECT that
            // overtook a lost END_MATCH would have the others elect a new owner and drop the END_MATCH
            if (m_closing || m_endMatchSent || m_matchEnded) return;
            m_leaving = true;
            foreach (MeshPeer peer in m_peers)
            {
                if (!peer.Lost) sendDisconnect(peer);
            }
            // Already the last one playing: told now, in the frame the game leaves, so the call goes
            // to brainCloud ahead of a LeaveLobby the game makes next, after which it would be refused
            if (m_started && everyPeerLost()) stopIfLastToLeave();
        }

        private void sendDisconnect(MeshPeer in_peer)
        {
            for (int i = 0; i < DISCONNECT_COPIES; ++i) sendTo(in_peer, new byte[] { DISCONNECT });
        }

        private void ping(long in_nowMs)
        {
            uint time = (uint)in_nowMs;
            byte[] packet = { PING, (byte)(time >> 24), (byte)(time >> 16), (byte)(time >> 8), (byte)time };
            foreach (MeshPeer peer in m_peers)
            {
                // Also a peer that has not connected yet: it is how it learns this player has, and
                // keeps the path open meanwhile
                if (!peer.Lost) sendTo(peer, packet);
            }
        }

        private void sendControl(byte in_type, byte[] in_body, long in_nowMs)
        {
            ushort id = ++m_nextControlId;
            var control = new Control { FirstSentMs = in_nowMs, LastSentMs = in_nowMs };
            control.Packet = new byte[3 + in_body.Length];
            control.Packet[0] = in_type;
            control.Packet[1] = (byte)(id >> 8);
            control.Packet[2] = (byte)id;
            Buffer.BlockCopy(in_body, 0, control.Packet, 3, in_body.Length);
            foreach (MeshPeer peer in m_peers)
            {
                if (!peer.Connected || peer.Lost) continue;
                control.Awaiting.Add(peer);
                sendTo(peer, control.Packet);
            }
            if (control.Awaiting.Count > 0) m_controls.Add(control);
        }

        private void processControls(long in_nowMs)
        {
            for (int i = m_controls.Count - 1; i >= 0; --i)
            {
                Control control = m_controls[i];
                control.Awaiting.RemoveAll(peer => peer.Lost);
                if (control.Awaiting.Count == 0 || in_nowMs - control.FirstSentMs > CONTROL_GIVE_UP_MS)
                {
                    if (control.Awaiting.Count > 0) log("gave up waiting for " + control.Awaiting.Count + " peer(s) to acknowledge packet type " + control.Packet[0]);
                    m_controls.RemoveAt(i);
                    continue;
                }
                if (in_nowMs - control.LastSentMs < CONTROL_RESEND_MS) continue;
                control.LastSentMs = in_nowMs;
                foreach (MeshPeer peer in control.Awaiting) sendTo(peer, control.Packet);
            }
        }

        private bool sendTo(MeshPeer in_peer, byte[] in_packet)
        {
            if (m_closed || in_peer.Address == null) return false;
            if (in_packet.Length > MAX_PACKET_LENGTH) return false;
            try
            {
                byte[] datagram = m_session.SenderFor(in_peer.NetId).Seal(in_packet, 0, in_packet.Length);
                if (DropForTests != null && DropForTests(in_peer.NetId, in_packet)) return true;
                m_socket.SendTo(datagram, in_peer.Address);
                return true;
            }
            catch (SocketException e)
            {
                // Lost by UDP anyway; a peer that stays unreachable times out
                log("send to net id " + in_peer.NetId + " failed: " + e.SocketErrorCode);
                return false;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }

        private void keepAlive(long in_nowMs)
        {
            m_lastKeepAliveMs = in_nowMs;
            postLobby(ServiceOperation.RoomKeepAlive, "keeping the lobby alive failed: ", reasonCode =>
            {
                // The lobby is no longer running this match (another member stopped it), is gone, or
                // this player has left it: every further keep-alive would be refused, and each is a
                // call the app pays for
                if (reasonCode == ReasonCodes.INVALID_REQUEST || reasonCode == ReasonCodes.LOBBY_NOT_FOUND ||
                    reasonCode == ReasonCodes.LOBBY_MEMBER_NOT_FOUND)
                {
                    m_keepAliveRefused = true;
                }
            });
        }

        private void postLobby(ServiceOperation in_operation, string in_failureLog, Action<int> in_onFailure = null)
        {
            if (m_send == null || m_lobbyId == null) return;
            var data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = m_lobbyId;
            ServerCallback callback = BrainCloudClient.CreateServerCallback(
                (response, cbObject) => { },
                (status, reasonCode, error, cbObject) =>
                {
                    log(in_failureLog + status + " " + reasonCode + " " + error);
                    if (in_onFailure != null) in_onFailure(reasonCode);
                });
            m_send(new ServerCall(ServiceName.Lobby, in_operation, data, callback));
        }

        #endregion

        #region Update

        /// <summary>Call on the main thread, every frame, until Close.</summary>
        public void Update(long in_nowMs)
        {
            if (!m_started || m_closing) return;
            receive(in_nowMs);
            if (m_closing) return;

            foreach (MeshPeer peer in m_peers)
            {
                if (peer.Lost) continue;
                if (peer.Connected && in_nowMs - peer.LastHeardMs > PEER_TIMEOUT_MS)
                {
                    lose(peer, "nothing heard for " + (PEER_TIMEOUT_MS / 1000) + " seconds", false, in_nowMs);
                }
                else if (!peer.Connected && in_nowMs - m_startMs > CONNECT_TIMEOUT_MS)
                {
                    lose(peer, "it never connected", false, in_nowMs);
                }
                else if (peer.Connected && !peer.Channel.ProcessResends(in_nowMs, peer.Resend))
                {
                    lose(peer, "a reliable packet went unacknowledged for " + (RelayReliableChannel.RELIABLE_GIVE_UP_MS / 1000) + " seconds", false, in_nowMs);
                }
            }

            if (in_nowMs - m_lastPingMs >= PingIntervalMs)
            {
                m_lastPingMs = in_nowMs;
                ping(in_nowMs);
            }
            processControls(in_nowMs);
            // brainCloud's lobby owner too, when it is not the players' owner: brainCloud changes its owner
            // only when the owner leaves the lobby, and takes keep-alives from any member
            // None once this player has left or stopped the match: brainCloud would refuse it, billed
            if (m_keepAliveMs > 0 && (isOwner || isLobbyOwner) && !m_matchEnded && !m_keepAliveRefused && !m_leaving && !m_stoppedSent &&
                in_nowMs - m_lastKeepAliveMs >= m_keepAliveMs)
            {
                keepAlive(in_nowMs);
            }
        }

        private bool everyPeerLost()
        {
            foreach (MeshPeer peer in m_peers)
            {
                if (!peer.Lost) return false;
            }
            return true;
        }

        /// <summary>
        /// Stops the connection. Returns true when it must still be updated with UpdateClosing, to
        /// finish delivering END_MATCH; otherwise the socket is closed already.
        /// </summary>
        public bool Close(long in_nowMs)
        {
            if (m_closed) return false;
            // The last one playing, leaving without ending the match: the relay server tells brainCloud
            // the room stopped when its last player goes, so the lobby returns to setup. Only when the
            // last of the others said it was leaving: when it went quiet, this player may be the one
            // cut off, and the others still playing. Then brainCloud's own timeout ends the lobby.
            if (!m_closing && m_started)
            {
                if (everyPeerLost()) stopIfLastToLeave();
                else if (m_leaving && m_keepAliveMs > 0 && !m_stoppedSent)
                {
                    // Others may be leaving at this same moment, their DISCONNECT still on its way:
                    // listened for a little while, so the last of them all tells brainCloud
                    m_lingerForLeavers = true;
                    if (in_nowMs + LEAVE_LINGER_MS - m_lingerUntilMs > 0) m_lingerUntilMs = in_nowMs + LEAVE_LINGER_MS;
                }
            }
            m_closing = true;
            if (m_controls.Count == 0 && in_nowMs - m_lingerUntilMs >= 0)
            {
                closeSocket();
                return false;
            }
            return true;
        }

        /// <summary>
        /// After Close returned true: resends END_MATCH until every peer has acknowledged it, and
        /// acknowledges repeats of one received. Returns false once finished and closed.
        /// </summary>
        public bool UpdateClosing(long in_nowMs)
        {
            if (m_closed) return false;
            receive(in_nowMs);
            processControls(in_nowMs);
            if (m_controls.Count == 0 && in_nowMs - m_lingerUntilMs >= 0)
            {
                closeSocket();
                return false;
            }
            return true;
        }

        /// Closes the socket at once, whatever is still being delivered.
        public void CloseNow()
        {
            m_closing = true;
            closeSocket();
        }

        private void closeSocket()
        {
            if (m_closed) return;
            m_closed = true;
            m_controls.Clear();
            // The keys are no use any more, and are not kept
            m_session.Clear();
            try
            {
                m_socket.Close();
            }
            catch (Exception)
            {
                // Closing is best effort
            }
        }

        #endregion

        #region receiving

        private void receive(long in_nowMs)
        {
            for (int i = 0; i < MAX_RECEIVES_PER_UPDATE && !m_closed; ++i)
            {
                int length;
                EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    if (m_socket.Available <= 0) return;
                    length = m_socket.ReceiveFrom(m_buffer, ref from);
                }
                catch (SocketException)
                {
                    // An ICMP "unreachable" for an earlier send, or a datagram too big for the buffer
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                handle(m_buffer, length, (IPEndPoint)from, in_nowMs);
            }
        }

        private void handle(byte[] in_datagram, int in_length, IPEndPoint in_from, long in_nowMs)
        {
            // Bigger than any datagram a peer sends, or too small to carry a packet: dropped before any key is used
            if (in_length > DirectPacket.MAX_DATAGRAM_LENGTH || in_length <= DirectPacket.OVERHEAD) return;
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(in_datagram, in_length, out sender, out receiver, out epoch, out sequence)) return;
            if (receiver != m_myNetId) return;
            MeshPeer peer;
            if (!m_peersByNetId.TryGetValue(sender, out peer) || peer.Lost) return;
            byte[] packet = m_session.ReceiverFor(sender).Open(in_datagram, in_length);
            if (packet == null || packet.Length == 0) return;

            // Only a packet of a known type and the right length changes anything
            byte type = packet[0];
            if (!wellFormed(type, packet.Length)) return;

            // It opened, and is not a repeat: it is from the peer, and this is where to send to it now,
            // unless a later datagram from it came from elsewhere
            moveAddress(peer, in_from, epoch, sequence);
            peer.LastHeardMs = in_nowMs;

            if (m_closing || m_matchEnded)
            {
                // Only finishing END_MATCH: acknowledgements of it, and repeats of one received
                if (type == CONTROL_ACK) onControlAck(peer, packet);
                else if (type == END_MATCH && packet.Length >= 3) acknowledge(peer, packet);
                else if (type == DISCONNECT && m_lingerForLeavers && !m_stoppedSent) onLeftTogether(peer, in_nowMs);
                return;
            }

            switch (type)
            {
                case DirectConnector.PUNCH:
                    // The peer's own punches may still be getting through: answered as the attempt did
                    if (packet.Length == 5)
                    {
                        packet[0] = DirectConnector.PUNCH_ACK;
                        sendTo(peer, packet);
                    }
                    return;
                case DirectConnector.PUNCH_ACK:
                    return;
                case DISCONNECT:
                    // No DISCONNECT event for a peer that never connected; lose() knows
                    lose(peer, peer.Connected ? "it left" : "it left before connecting", true, in_nowMs);
                    return;
                case RELAY:
                case ACK:
                case PING:
                case PONG:
                case END_MATCH:
                case OWNER:
                case CONTROL_ACK:
                    break;
                default:
                    // A later format's packet: not understood, so ignored
                    return;
            }

            // Anything else from it means it has taken the connection over too
            if (!peer.Connected) connect(peer, in_nowMs);

            switch (type)
            {
                case RELAY:
                    onRelay(peer, packet, in_nowMs);
                    break;
                case ACK:
                    if (packet.Length == RELAY_HEADER_LENGTH) peer.Channel.Acknowledge(BitConverter.ToUInt64(packet, 1));
                    break;
                case PING:
                    if (packet.Length == 5)
                    {
                        packet[0] = PONG;
                        sendTo(peer, packet);
                    }
                    break;
                case PONG:
                    if (packet.Length == 5)
                    {
                        uint sent = ((uint)packet[1] << 24) | ((uint)packet[2] << 16) | ((uint)packet[3] << 8) | packet[4];
                        uint roundTrip = (uint)in_nowMs - sent;
                        if (roundTrip <= PEER_TIMEOUT_MS) peer.RoundTripMs = (int)roundTrip;
                    }
                    break;
                case END_MATCH:
                    onEndMatch(peer, packet);
                    break;
                case OWNER:
                    onOwner(peer, packet);
                    break;
                case CONTROL_ACK:
                    onControlAck(peer, packet);
                    break;
            }
        }

        /// <summary>
        /// Whether a packet that opened is one this player understands, at the length its type has:
        /// checked before anything about the peer changes.
        /// </summary>
        private static bool wellFormed(byte in_type, int in_length)
        {
            switch (in_type)
            {
                case DirectConnector.PUNCH:
                case DirectConnector.PUNCH_ACK:
                case PING:
                case PONG:
                    return in_length == 5;
                case RELAY:
                    return in_length >= RELAY_HEADER_LENGTH;
                case ACK:
                    return in_length == RELAY_HEADER_LENGTH;
                case DISCONNECT:
                    return in_length == 1;
                case END_MATCH:
                    return in_length >= 3;
                case OWNER:
                    return in_length == 4;
                case CONTROL_ACK:
                    return in_length == 3;
                default:
                    // A later format's packet: not understood, so ignored
                    return false;
            }
        }

        /// <summary>
        /// Moves a peer to where a datagram from it came from, only when that datagram is later (by
        /// epoch, then sequence number) than the one the current address came from: one held up on
        /// the way, from where the peer was before, must not move it back.
        /// </summary>
        private static void moveAddress(MeshPeer in_peer, IPEndPoint in_from, byte[] in_epoch, ulong in_sequence)
        {
            uint count = DirectPacket.EpochCount(in_epoch);
            if (in_peer.AddressFromDatagram &&
                (count < in_peer.AddressEpochCount || (count == in_peer.AddressEpochCount && in_sequence <= in_peer.AddressSequence)))
            {
                return;
            }
            in_peer.Address = in_from;
            in_peer.AddressFromDatagram = true;
            in_peer.AddressEpochCount = count;
            in_peer.AddressSequence = in_sequence;
        }

        /// For the tests: where this player sends to in_netId.
        internal IPEndPoint PeerAddressForTests(int in_netId)
        {
            MeshPeer peer;
            return m_peersByNetId.TryGetValue(in_netId, out peer) ? peer.Address : null;
        }

        private void connect(MeshPeer in_peer, long in_nowMs)
        {
            in_peer.Connected = true;
            in_peer.LastHeardMs = in_nowMs;
            log("net id " + in_peer.NetId + " connected");
            var connect = new Dictionary<string, object>();
            connect["op"] = "CONNECT";
            connect["cxId"] = in_peer.CxId;
            connect["ownerCxId"] = m_ownerCxId;
            connect["netId"] = in_peer.NetId;
            addProfileId(connect, "profileId", in_peer.CxId);
            addProfileId(connect, "ownerId", m_ownerCxId);
            m_listener.OnMeshSystem(JsonWriter.Serialize(connect));

            // An elected owner is the lowest net id among the peers connected at the time. A peer
            // connecting later with a lower one (slow to take its connection over) would be elected
            // by players it reached first: elected again here, every player ends up with the same owner.
            if (m_ownerElected && !string.Equals(electedCxId(), m_ownerCxId, StringComparison.Ordinal)) electOwner(in_nowMs);
        }

        private void onRelay(MeshPeer in_peer, byte[] in_packet, long in_nowMs)
        {
            if (in_packet.Length < RELAY_HEADER_LENGTH) return;
            // The low byte of the mask names the sender, and must be the peer whose key opened it
            if (in_packet[8] != in_peer.NetId) return;
            // Only what was addressed to this player, as the relay delivers
            ulong mask = 0;
            for (int i = 3; i < RELAY_HEADER_LENGTH; ++i) mask = (mask << 8) | in_packet[i];
            if (((mask >> (47 - m_myNetId)) & 1) == 0) return;

            ushort flags = (ushort)((in_packet[1] << 8) | in_packet[2]);
            if ((flags & RelayComms.RELIABLE_BIT) != 0)
            {
                // Always, even for a repeat: an earlier ACK may have been lost
                byte[] ack = new byte[RELAY_HEADER_LENGTH];
                Buffer.BlockCopy(in_packet, 0, ack, 0, RELAY_HEADER_LENGTH);
                ack[0] = ACK;
                sendTo(in_peer, ack);
            }
            if (in_peer.Channel.Receive(withLengthPrefix(in_packet), m_deliver) == RelayReliableChannel.ReceiveOutcome.TooManyOutOfOrder)
            {
                // Told, so it does not go on playing towards this player
                sendDisconnect(in_peer);
                lose(in_peer, "too many packets arrived out of order", false, in_nowMs);
            }
        }

        private void deliver(short in_netId, byte[] in_packet)
        {
            m_listener.OnMeshRelay(in_netId, in_packet);
        }

        private void onEndMatch(MeshPeer in_peer, byte[] in_packet)
        {
            if (in_packet.Length < 3 || !acknowledge(in_peer, in_packet)) return;
            object payload;
            try
            {
                payload = JsonReader.Deserialize(Encoding.UTF8.GetString(in_packet, 3, in_packet.Length - 3));
            }
            catch (Exception)
            {
                // The relay passes the match on as ended whatever the data, without the data
                payload = new Dictionary<string, object>();
            }
            if (!string.Equals(in_peer.CxId, m_ownerCxId, StringComparison.Ordinal))
            {
                // As the relay does: only the owner may end the match. Except that the owner's
                // successor may have lost the owner before this player has: its END_MATCH is held
                // until this player's own election makes it owner too.
                if (string.Equals(in_peer.CxId, successorCxId(), StringComparison.Ordinal))
                {
                    log("holding END_MATCH from net id " + in_peer.NetId + " until it is the owner here too");
                    m_pendingEndMatchFrom = in_peer.CxId;
                    m_pendingEndMatch = payload;
                }
                else
                {
                    log("ignored END_MATCH from net id " + in_peer.NetId + ", which is not the owner");
                }
                return;
            }
            raiseEndMatch(payload, in_peer.LastHeardMs, true);
        }

        /// in_fromPeer: the owner's END_MATCH, received.
        private void raiseEndMatch(object in_payload, long in_nowMs, bool in_fromPeer)
        {
            // Nothing after it is taken in: RelayComms closes the connection on END_MATCH
            m_matchEnded = true;
            // brainCloud's lobby owner, when that is not the owner who ended the match, tells it too,
            // in case brainCloud no longer counts that owner a member (its RTT connection dropped)
            if (in_fromPeer && isLobbyOwner) roomStopped();
            var endMatch = new Dictionary<string, object>();
            endMatch["op"] = "END_MATCH";
            endMatch["data"] = in_payload;
            // Repeats are still acknowledged for a while after the match ends, in case this one's acknowledgement was lost
            m_lingerUntilMs = in_nowMs + END_MATCH_LINGER_MS;
            m_listener.OnMeshSystem(JsonWriter.Serialize(endMatch));
        }

        private void onOwner(MeshPeer in_peer, byte[] in_packet)
        {
            if (in_packet.Length != 4 || !acknowledge(in_peer, in_packet)) return;
            // A peer can only name itself, and only as the member this player's own election would pick
            // once the owner is gone: otherwise any member could make itself owner and end the match.
            // A player that has not lost the owner yet runs the same election when it does.
            if (in_packet[3] != in_peer.NetId) return;
            MeshPeer owner = peerFor(m_ownerCxId);
            if (owner == null || !owner.Lost || !string.Equals(electedCxId(), in_peer.CxId, StringComparison.Ordinal))
            {
                log("ignored OWNER from net id " + in_peer.NetId);
                return;
            }
            setOwner(in_peer.CxId);
        }

        private void onControlAck(MeshPeer in_peer, byte[] in_packet)
        {
            if (in_packet.Length != 3) return;
            foreach (Control control in m_controls)
            {
                if (control.Packet[1] == in_packet[1] && control.Packet[2] == in_packet[2]) control.Awaiting.Remove(in_peer);
            }
        }

        /// Acknowledges a control packet; true the first time it is seen, false for a repeat.
        private bool acknowledge(MeshPeer in_peer, byte[] in_packet)
        {
            sendTo(in_peer, new byte[] { CONTROL_ACK, in_packet[1], in_packet[2] });
            ushort id = (ushort)((in_packet[1] << 8) | in_packet[2]);
            if (in_peer.ControlsSeen.Contains(id)) return false;
            in_peer.ControlsSeen.Add(id);
            if (in_peer.ControlsSeen.Count > MAX_CONTROLS_SEEN) in_peer.ControlsSeen.RemoveAt(0);
            return true;
        }

        #endregion

        #region peers and owner

        /// <summary>
        /// Tells brainCloud the match stopped when every peer is gone and the last of them said it
        /// was leaving. When several left together, each having said so before hearing the others,
        /// only the one with the lowest net id among them sends it, so brainCloud gets it once.
        /// </summary>
        private void stopIfLastToLeave()
        {
            if (!everyPeerLost() || !m_lastLossGraceful) return;
            foreach (MeshPeer peer in m_peers)
            {
                if (peer.LeftWithThisPlayer && peer.NetId < m_myNetId) return;
            }
            roomStopped();
        }

        // A peer's DISCONNECT while this player, having left too, waits for peers leaving with it
        private void onLeftTogether(MeshPeer in_peer, long in_nowMs)
        {
            // No events: the game has disconnected
            in_peer.Lost = true;
            in_peer.LeftWithThisPlayer = true;
            m_lastLossGraceful = true;
            if (!everyPeerLost()) return;
            stopIfLastToLeave();
            m_lingerUntilMs = in_nowMs;
        }

        /// in_graceful: the peer said it was leaving, rather than going quiet.
        private void lose(MeshPeer in_peer, string in_reason, bool in_graceful, long in_nowMs)
        {
            if (in_peer.Lost) return;
            in_peer.Lost = true;
            if (in_graceful && m_leaving) in_peer.LeftWithThisPlayer = true;
            m_lastLossGraceful = in_graceful;
            log("lost net id " + in_peer.NetId + ": " + in_reason);
            in_peer.Channel.Reset();
            if (in_peer.Connected)
            {
                var disconnect = new Dictionary<string, object>();
                disconnect["op"] = "DISCONNECT";
                disconnect["cxId"] = in_peer.CxId;
                addProfileId(disconnect, "profileId", in_peer.CxId);
                m_listener.OnMeshSystem(JsonWriter.Serialize(disconnect));
            }
            if (string.Equals(in_peer.CxId, m_ownerCxId, StringComparison.Ordinal))
            {
                // Taking over from an owner that left is certain; from one gone quiet, it may be this
                // player's own path that failed
                m_ownershipConfirmed = in_graceful;
                electOwner(in_nowMs);
            }
        }

        /// <summary>
        /// The owner was lost: the new one is the member connected and still here with the lowest net
        /// id. Every player works it out alone, from the net ids brainCloud gave them all, and the new
        /// owner also announces itself. brainCloud picks the lowest net id among the lobby's members
        /// when the owner leaves the lobby; it cannot see who is connected, so the two can differ, and
        /// keep-alives and ROOM_STOPPED are accepted from any member for that reason.
        /// </summary>
        private void electOwner(long in_nowMs)
        {
            m_ownerElected = true;
            setOwner(electedCxId());
            if (m_pendingEndMatchFrom != null)
            {
                // The successor ended the match before this player had lost the old owner
                if (string.Equals(m_pendingEndMatchFrom, m_ownerCxId, StringComparison.Ordinal))
                {
                    raiseEndMatch(m_pendingEndMatch, in_nowMs, true);
                    m_pendingEndMatchFrom = null;
                    m_pendingEndMatch = null;
                    return;
                }
                m_pendingEndMatchFrom = null;
                m_pendingEndMatch = null;
            }
            if (isOwner)
            {
                sendControl(OWNER, new byte[] { (byte)m_myNetId }, in_nowMs);
                // From the next Update, on the regular schedule
                if (m_keepAliveMs > 0) m_lastKeepAliveMs = in_nowMs - m_keepAliveMs;
            }
        }

        // The member connected and still here with the lowest net id, this player included
        private string electedCxId()
        {
            return lowestCxId(null);
        }

        // Who electedCxId would pick if the current owner were lost now
        private string successorCxId()
        {
            return lowestCxId(m_ownerCxId);
        }

        private string lowestCxId(string in_except)
        {
            int lowest = string.Equals(m_myCxId, in_except, StringComparison.Ordinal) ? int.MaxValue : m_myNetId;
            string owner = lowest == int.MaxValue ? null : m_myCxId;
            foreach (MeshPeer peer in m_peers)
            {
                // Only a peer that has connected and is still here: one that has not taken the
                // connection over yet cannot act as owner
                if (peer.Connected && !peer.Lost && peer.NetId < lowest && !string.Equals(peer.CxId, in_except, StringComparison.Ordinal))
                {
                    lowest = peer.NetId;
                    owner = peer.CxId;
                }
            }
            return owner;
        }

        private MeshPeer peerFor(string in_cxId)
        {
            foreach (MeshPeer peer in m_peers)
            {
                if (string.Equals(peer.CxId, in_cxId, StringComparison.Ordinal)) return peer;
            }
            return null;
        }

        // The profile id inside an RTT connection id (appId:profileId:connection), as the relay adds it
        private static string profileIdOf(string in_cxId)
        {
            string[] parts = in_cxId != null ? in_cxId.Split(':') : new string[0];
            return parts.Length == 3 ? parts[1] : null;
        }

        private static void addProfileId(Dictionary<string, object> io_message, string in_key, string in_cxId)
        {
            string profileId = profileIdOf(in_cxId);
            if (profileId != null) io_message[in_key] = profileId;
        }

        private void setOwner(string in_cxId)
        {
            if (string.Equals(in_cxId, m_ownerCxId, StringComparison.Ordinal)) return;
            m_ownerCxId = in_cxId;
            log("the owner is now " + in_cxId);
            var migrate = new Dictionary<string, object>();
            migrate["op"] = "MIGRATE_OWNER";
            migrate["cxId"] = in_cxId;
            addProfileId(migrate, "profileId", in_cxId);
            m_listener.OnMeshSystem(JsonWriter.Serialize(migrate));
        }

        #endregion

        private static byte[] withLengthPrefix(byte[] in_packet)
        {
            byte[] prefixed = new byte[LENGTH_PREFIX + in_packet.Length];
            int length = prefixed.Length;
            prefixed[0] = (byte)(length >> 8);
            prefixed[1] = (byte)length;
            Buffer.BlockCopy(in_packet, 0, prefixed, LENGTH_PREFIX, in_packet.Length);
            return prefixed;
        }
    }
}
