// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using BrainCloud.JsonFx.Json;

    /// <summary>One peer in a direct connect attempt.</summary>
    internal sealed class DirectPeer
    {
        public DirectPeer(string in_cxId, int in_netId)
        {
            CxId = in_cxId;
            NetId = in_netId;
        }

        public string CxId { get; private set; }
        public int NetId { get; private set; }

        /// Where the peer may be reached, from its DIRECT_CONNECT_CANDIDATES.
        public readonly List<IPEndPoint> Candidates = new List<IPEndPoint>();

        /// <summary>
        /// Where to send to the peer: the source of its latest authenticated datagram, so nothing
        /// forged can redirect the traffic.
        /// </summary>
        public IPEndPoint Address { get; internal set; }

        // The epoch count and sequence of the datagram Address came from
        internal bool AddressFromDatagram;
        internal uint AddressEpochCount;
        internal ulong AddressSequence;

        /// <summary>
        /// Moves Address to in_from only if this datagram is newer than the one Address came from, so
        /// a delayed datagram from an old address cannot move it back.
        /// </summary>
        internal void MoveAddress(IPEndPoint in_from, byte[] in_epoch, ulong in_sequence)
        {
            uint count = DirectPacket.EpochCount(in_epoch);
            if (AddressFromDatagram && (count < AddressEpochCount || (count == AddressEpochCount && in_sequence <= AddressSequence))) return;
            Address = in_from;
            AddressFromDatagram = true;
            AddressEpochCount = count;
            AddressSequence = in_sequence;
        }

        /// True once a PUNCH_ACK from the peer has opened: a full round trip.
        public bool Reached { get; internal set; }

        /// The round trip of the punch that reached the peer, in milliseconds, or -1.
        public int RoundTripMs { get; internal set; }

        // Non-candidate addresses the peer's punches came from, and the punches each is still owed
        internal readonly Dictionary<IPEndPoint, int> Triggered = new Dictionary<IPEndPoint, int>();

        internal DirectPeer Clear()
        {
            Candidates.Clear();
            Address = null;
            AddressFromDatagram = false;
            Reached = false;
            RoundTripMs = -1;
            Triggered.Clear();
            return this;
        }
    }

    /// <summary>A successful attempt's socket, keys and peers, for the connection that takes it over.</summary>
    internal sealed class DirectConnection
    {
        internal DirectConnection(string in_lobbyId, Socket in_socket, DirectSession in_session, string in_ownerCxId, List<DirectPeer> in_peers, int in_keepAliveRateSecs)
        {
            LobbyId = in_lobbyId;
            Socket = in_socket;
            Session = in_session;
            OwnerCxId = in_ownerCxId;
            Peers = in_peers;
            KeepAliveRateSecs = in_keepAliveRateSecs;
            LobbyOwner = new DirectLobbyOwner { CxId = in_ownerCxId };
        }

        public string LobbyId { get; private set; }
        public Socket Socket { get; private set; }
        public DirectSession Session { get; private set; }
        public string OwnerCxId { get; private set; }
        public List<DirectPeer> Peers { get; private set; }

        /// From the direct ROOM_READY: how often the owner keeps the lobby alive, or 0 for a lobby that does not outlive its match.
        public int KeepAliveRateSecs { get; private set; }

        /// <summary>
        /// brainCloud's lobby owner, tracked from lobby events. It changes only when the owner leaves the
        /// lobby, so it can differ from the players' owner (who changes on leaving the match). It keeps
        /// the lobby alive, and tells brainCloud the match stopped on the players' owner's END_MATCH.
        /// </summary>
        public string LobbyOwnerCxId
        {
            get { return LobbyOwner.CxId; }
            internal set { LobbyOwner.CxId = value; }
        }

        /// The connector keeps only this after handover, to follow the lobby owner.
        internal DirectLobbyOwner LobbyOwner { get; private set; }
    }

    /// brainCloud's owner of a lobby whose connection has been taken over, followed from its events.
    internal sealed class DirectLobbyOwner
    {
        public string CxId;
    }

    /// <summary>
    /// This player's part in a lobby's direct connect attempt: gather addresses, exchange them, punch
    /// every peer, report which were reached. Main thread only, except DNS and STUN.
    /// </summary>
    internal sealed class DirectConnector
    {
        public enum AttemptState
        {
            /// No attempt, or the last one was handed over.
            None,
            /// Opening the socket, asking STUN, sending this player's addresses.
            Gathering,
            /// Punching the peers.
            Punching,
            /// Every peer was reached and reported; waiting for the connection to take over.
            Connected,
            /// Not every peer was reached, the attempt was refused, or brainCloud gave it up.
            Failed
        }

        public const byte PUNCH = 16;
        public const byte PUNCH_ACK = 17;
        public const string CANDIDATE_HOST = "host";
        public const string CANDIDATE_SRFLX = "srflx";

        /// How often each address of a peer not yet reached is punched.
        public const int PUNCH_INTERVAL_MS = 100;
        /// Report this long before brainCloud's own timeout, so the fallback starts on the report.
        public const int REPORT_MARGIN_MS = 1000;
        public const int MIN_PUNCH_BUDGET_MS = 1000;
        /// brainCloud takes at most this many candidates from a player.
        public const int MAX_CANDIDATES = 16;
        /// How long a successful attempt's socket stays open with nothing taking it over.
        public const int CONNECTED_IDLE_MS = 60000;
        /// A PUNCH_ACK is only matched to a punch sent this recently; older records are dropped.
        public const int PUNCH_RECORD_MS = 5000;
        /// <summary>
        /// Punches earned by a non-candidate address a peer's punch came from, one per PUNCH_INTERVAL_MS
        /// (like ICE triggered checks): the peer may stop punching once it reaches us, so one is too few.
        /// </summary>
        public const int TRIGGERED_PUNCHES = 5;
        /// <summary>
        /// Punch interval while awaiting takeover: peers stop punching once reached, and a router can
        /// drop an idle mapping in 30 seconds.
        /// </summary>
        public const int KEEP_MAPPING_MS = 5000;
        // The only datagram exchanged: a sealed 5-byte PUNCH or PUNCH_ACK
        private const int PUNCH_DATAGRAM_LENGTH = DirectPacket.OVERHEAD + 5;
        // Most peers whose candidates are kept when they arrive before DIRECT_CONNECT
        private const int MAX_EARLY_PEERS = 64;
        // Most datagrams read per Update, so a flood cannot stall a frame
        private const int MAX_RECEIVES_PER_UPDATE = 256;

        private readonly BrainCloudClient m_clientRef;
        private readonly NatProbe m_probe;
        private readonly Action<ServerCall> m_send;

        private AttemptState m_state = AttemptState.None;
        private int m_attempt = 0;
        private string m_lobbyId;
        private string m_ownerCxId;
        private DirectSession m_session;
        private Socket m_socket;
        private readonly Dictionary<string, DirectPeer> m_peers = new Dictionary<string, DirectPeer>();       // by cxId
        private readonly Dictionary<int, DirectPeer> m_peersByNetId = new Dictionary<int, DirectPeer>();
        // Candidates that came before this player's DIRECT_CONNECT, by sender: lobby and addresses
        private readonly Dictionary<string, KeyValuePair<string, List<IPEndPoint>>> m_early = new Dictionary<string, KeyValuePair<string, List<IPEndPoint>>>();
        private readonly Dictionary<uint, Punch> m_punches = new Dictionary<uint, Punch>();
        private uint m_nextPunchId = 0;
        private int m_deadlineMs;
        private int m_lastPunchMs;
        private int m_connectedAtMs;
        private bool m_reported;
        private bool m_roomReady;
        private int m_keepAliveRateSecs;
        // brainCloud's lobby owner from events during the attempt (null: as DIRECT_CONNECT named)
        private string m_lobbyOwnerCxId;
        // Peers DIRECT_CONNECT named with an unusable net id; reported as not reached
        private readonly List<string> m_unusablePeers = new List<string>();
        // The last connection taken over: its lobby and owner, kept up to date
        private string m_handedOverLobbyId;
        private DirectLobbyOwner m_handedOverOwner;
        private int m_lastKeepMappingMs;
        private readonly byte[] m_buffer = new byte[DirectPacket.MAX_DATAGRAM_LENGTH];

        // The gathering thread's job and result. The job owns the socket while it runs: closing a socket
        // another thread waits on can block for that wait, so an abandoned job closes it when it ends.
        private readonly object m_gatheredLock = new object();
        private GatherJob m_job;
        private Gathered m_gathered;

        private struct Punch
        {
            public DirectPeer Peer;
            public int SentAtMs;
        }

        private sealed class GatherJob
        {
            public Socket Socket;
            public bool Abandoned;
            public bool Done;
        }

        private sealed class Gathered
        {
            public int Attempt;
            public IPEndPoint Public;
            public List<IPAddress> Local;
            public string Detail;
        }

        public DirectConnector(BrainCloudClient in_client, NatProbe in_probe)
            : this(in_client, in_probe, in_client.SendRequest)
        {
        }

        /// For the unit tests: in_send receives each call instead of the client, and in_client may be null.
        internal DirectConnector(BrainCloudClient in_client, NatProbe in_probe, Action<ServerCall> in_send)
        {
            m_clientRef = in_client;
            m_probe = in_probe;
            m_send = in_send;
#if UNITY_IOS && !UNITY_EDITOR
            // Sending to a local address may trigger iOS's local network permission prompt
            IncludeLocalAddresses = false;
#else
            IncludeLocalAddresses = true;
#endif
        }

        /// <summary>
        /// Offer local-network addresses, so players behind the same router can connect. On by default
        /// except on iOS, where it may trigger the local network permission prompt.
        /// </summary>
        public bool IncludeLocalAddresses { get; set; }

        public AttemptState State { get { return m_state; } }

        /// <summary>
        /// For tools that run the attempt themselves, such as stun-probe: the connector stays out and
        /// passes every event to the game's lobby callback.
        /// </summary>
        internal bool StandAside { get; set; }

        /// The peers of the current or last attempt.
        public ICollection<DirectPeer> Peers { get { return m_peers.Values; } }

        /// For the tests: the local addresses to offer, in place of the machine's own.
        internal Func<List<IPAddress>> LocalAddressesForTests { get; set; }

        /// For the tests: the punching time allowed, in place of the one DIRECT_CONNECT gives.
        internal int PunchBudgetMsForTests { get; set; }

        /// For the tests: how often a connected attempt punches its peers, in place of KEEP_MAPPING_MS.
        internal int KeepMappingMsForTests { get; set; }

        /// For the tests: how long a punch is remembered, in place of PUNCH_RECORD_MS.
        internal int PunchRecordMsForTests { get; set; }

        /// For the tests: the socket's port, once it is open.
        internal int Port { get { return m_socket != null ? ((IPEndPoint)m_socket.LocalEndPoint).Port : 0; } }

        private bool loggingEnabled { get { return m_clientRef != null && m_clientRef.LoggingEnabled; } }

        private void log(string in_message)
        {
            if (loggingEnabled) m_clientRef.Log("Direct connect: " + in_message);
        }

        private static int nowMs { get { return Environment.TickCount; } }

        /// <summary>
        /// Takes an RTT event before the game's callbacks; true means it is the library's alone. A
        /// DIRECT_CONNECT passed on loses its pair keys. Names arrive lowercased (ToLowerInvariant).
        /// </summary>
        internal bool FilterEvent(string in_service, string in_operation, ref string io_json)
        {
            if (!string.Equals(in_service, "lobby", StringComparison.Ordinal) || in_operation == null) return false;
            if (string.Equals(in_operation, "room_ready", StringComparison.Ordinal))
            {
                // Noted, and passed on: the game connects from it
                if (!StandAside) OnRoomReady(DataOf(io_json));
                return false;
            }
            if (!StandAside && (m_handedOverOwner != null || m_state == AttemptState.Connected) && !in_operation.StartsWith("direct_connect", StringComparison.Ordinal))
            {
                // brainCloud's lobby owner, for the connection taken over or about to be
                noteLobbyOwner(DataOf(io_json));
                return false;
            }
            if (!in_operation.StartsWith("direct_connect", StringComparison.Ordinal)) return false;
            if (OnLobbyEvent(in_operation.ToUpperInvariant(), DataOf(io_json))) return true;
            // The game sees DIRECT_CONNECT without pair keys, unless a tool is playing the attempt
            if (string.Equals(in_operation, "direct_connect", StringComparison.Ordinal) && !StandAside)
            {
                io_json = WithoutPairKeys(io_json);
            }
            return false;
        }

        /// <summary>
        /// Takes a direct connect lobby event; true means the game must not see it
        /// (DIRECT_CONNECT_CANDIDATES carries other players' IP addresses).
        /// </summary>
        public bool OnLobbyEvent(string in_operation, Dictionary<string, object> in_data)
        {
            if (in_data == null || StandAside) return false;
            switch (in_operation)
            {
                case "DIRECT_CONNECT":
                    onDirectConnect(in_data);
                    return false;
                case "DIRECT_CONNECT_CANDIDATES":
                    onCandidates(in_data);
                    return true;
                case "DIRECT_CONNECT_FAILED":
                    if (string.Equals(str(in_data, "lobbyId"), m_lobbyId) && m_state != AttemptState.None)
                    {
                        log("brainCloud gave the attempt up (" + str(in_data, "reason") + "); the lobby will use its server");
                        // Nothing more to report or to punch for
                        m_reported = true;
                        fail();
                    }
                    return false;
                default:
                    return false;
            }
        }

        /// The RTT connection was lost: an attempt in progress cannot report, so it ends.
        public void OnRttDisconnected()
        {
            if (m_state == AttemptState.Gathering || m_state == AttemptState.Punching || m_state == AttemptState.Connected)
            {
                fail();
            }
            m_early.Clear();
        }

        /// <summary>
        /// A direct ROOM_READY for the connected attempt's lobby allows takeover; one naming a server
        /// closes the attempt's socket.
        /// </summary>
        public void OnRoomReady(Dictionary<string, object> in_data)
        {
            if (in_data == null || m_state != AttemptState.Connected || !string.Equals(str(in_data, "lobbyId"), m_lobbyId)) return;
            object value;
            var connectData = in_data.TryGetValue("connectData", out value) ? value as Dictionary<string, object> : null;
            if (connectData != null && string.Equals(str(connectData, "type"), "direct", StringComparison.Ordinal))
            {
                m_roomReady = true;
                m_keepAliveRateSecs = Math.Max(0, integer(connectData, "keepAliveRateSecs", 0));
                // Waiting for the game to connect from it
                m_connectedAtMs = nowMs;
            }
            else
            {
                log("the lobby is using a server after all; closing the attempt's socket");
                fail();
            }
        }

        /// <summary>
        /// Hands over a successful attempt for in_lobbyId and ends it; null until its direct ROOM_READY
        /// arrives, since a member may not yet have reached every other.
        /// </summary>
        public DirectConnection TakeConnection(string in_lobbyId)
        {
            if (m_state != AttemptState.Connected || !m_roomReady || !string.Equals(in_lobbyId, m_lobbyId)) return null;
            var connection = new DirectConnection(m_lobbyId, m_socket, m_session, m_ownerCxId, new List<DirectPeer>(m_peers.Values), m_keepAliveRateSecs);
            if (m_lobbyOwnerCxId != null) connection.LobbyOwnerCxId = m_lobbyOwnerCxId;
            m_handedOverLobbyId = m_lobbyId;
            m_handedOverOwner = connection.LobbyOwner;
            m_socket = null;
            m_session = null;
            m_punches.Clear();
            m_state = AttemptState.None;
            ++m_attempt;
            return connection;
        }

        // A lobby event's lobby.ownerCxId, for the lobby of the attempt or of the connection taken over
        private void noteLobbyOwner(Dictionary<string, object> in_data)
        {
            string lobbyId = str(in_data, "lobbyId");
            object value;
            var lobby = in_data != null && in_data.TryGetValue("lobby", out value) ? value as Dictionary<string, object> : null;
            string owner = str(lobby, "ownerCxId");
            if (lobbyId == null || owner == null) return;
            if (m_handedOverOwner != null && lobbyId == m_handedOverLobbyId) m_handedOverOwner.CxId = owner;
            if (m_state == AttemptState.Connected && lobbyId == m_lobbyId) m_lobbyOwnerCxId = owner;
        }

        #region DIRECT_CONNECT

        private void onDirectConnect(Dictionary<string, object> in_data)
        {
            // A new attempt replaces any earlier one
            close();
            ++m_attempt;
            m_peers.Clear();
            m_peersByNetId.Clear();
            m_punches.Clear();
            m_reported = false;
            m_roomReady = false;
            m_keepAliveRateSecs = 0;
            m_lobbyId = str(in_data, "lobbyId");
            m_ownerCxId = str(in_data, "ownerCxId");
            m_lobbyOwnerCxId = null;
            string cipher = str(in_data, "cipher");
            int timeoutSecs = integer(in_data, "timeoutSecs", 0);
            int myNetId = integer(in_data, "netId", -1);

            var pairKeys = new Dictionary<int, byte[]>();
            bool everyPeerHasAKey = true;
            m_unusablePeers.Clear();
            foreach (Dictionary<string, object> entry in objects(in_data, "peers"))
            {
                string cxId = str(entry, "cxId");
                int netId = integer(entry, "netId", -1);
                // A net id the relay protocol cannot address (it has 40), or one already taken
                if (cxId == null || netId < 0 || netId >= RelayComms.MAX_PLAYERS || m_peers.ContainsKey(cxId) || m_peersByNetId.ContainsKey(netId))
                {
                    // Reported as not reached with the others, so brainCloud is not left waiting on it
                    if (cxId != null && !m_peers.ContainsKey(cxId) && !m_unusablePeers.Contains(cxId)) m_unusablePeers.Add(cxId);
                    continue;
                }
                var peer = new DirectPeer(cxId, netId).Clear();
                m_peers[cxId] = peer;
                m_peersByNetId[netId] = peer;
                byte[] key = base64(str(entry, "pairKey"));
                if (key != null && key.Length == DirectPacket.PAIR_KEY_LENGTH) pairKeys[netId] = key;
                else everyPeerHasAKey = false;
            }
            m_state = AttemptState.Gathering;

            string refusal = null;
#if UNITY_WEBGL
            refusal = "WebGL has no UDP sockets";
#endif
            if (refusal == null && (m_probe == null || !m_probe.Enabled)) refusal = "direct connect is turned off";
            if (refusal == null && m_lobbyId == null) refusal = "DIRECT_CONNECT named no lobby";
            if (refusal == null && m_peers.Count == 0) refusal = "DIRECT_CONNECT named no peers";
            if (refusal == null && !DirectPacket.IsSupported(cipher)) refusal = "this library does not support the cipher " + (cipher ?? "(none)");
            if (refusal == null && !everyPeerHasAKey) refusal = "a peer came without a usable pairKey";
            if (refusal == null && (myNetId < 0 || myNetId >= RelayComms.MAX_PLAYERS)) refusal = "this player's net id is not usable: " + myNetId;
            if (refusal == null && m_unusablePeers.Count > 0) refusal = "a peer's net id is not usable";
            if (refusal == null)
            {
                try
                {
                    m_session = new DirectSession(cipher, myNetId, pairKeys);
                    // The session keeps its own copies
                    foreach (byte[] key in pairKeys.Values) Array.Clear(key, 0, key.Length);
                }
                catch (ArgumentException e)
                {
                    refusal = "the attempt's details are not usable: " + e.Message;
                }
            }
            if (refusal != null)
            {
                log(refusal + ", so every peer is reported as not reached");
                report();
                fail();
                return;
            }

            int budgetMs = PunchBudgetMsForTests > 0 ? PunchBudgetMsForTests : Math.Max(MIN_PUNCH_BUDGET_MS, timeoutSecs * 1000 - REPORT_MARGIN_MS);
            m_deadlineMs = nowMs + budgetMs;
            log("lobby " + m_lobbyId + ": net id " + myNetId + ", " + m_peers.Count + " peer(s), cipher " + cipher + ", " + budgetMs + " ms to connect");

            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            }
            catch (SocketException e)
            {
                log("could not open a UDP socket (" + e.SocketErrorCode + "), so every peer is reported as not reached");
                report();
                fail();
                return;
            }
            gather(m_attempt);
        }

        /// <summary>
        /// Asks STUN for this socket's public address on another thread (the network test's was for
        /// another socket). Nothing else reads the socket until this finishes.
        /// </summary>
        private void gather(int in_attempt)
        {
            Socket socket = m_socket;
            var job = new GatherJob { Socket = socket };
            lock (m_gatheredLock)
            {
                m_job = job;
            }
            string host = m_probe.StunHost;
            int port = m_probe.StunPort;
            byte[] token = m_probe.StunToken;
            bool includeLocal = IncludeLocalAddresses;
            Func<List<IPAddress>> localForTests = LocalAddressesForTests;

            Task.Run(() =>
            {
                var gathered = new Gathered { Attempt = in_attempt, Local = new List<IPAddress>() };
                List<IPEndPoint> servers = null;
                try
                {
                    if (!string.IsNullOrEmpty(host))
                    {
                        bool ipv6Only;
                        servers = NatProbe.Resolve(host, port, out ipv6Only);
                        if (servers.Count > 0)
                        {
                            // One address is enough here: the mapping was classified by the network test
                            foreach (StunResult result in StunClient.Probe(socket, servers.GetRange(0, 1), token, 2, 300))
                            {
                                if (result.Mapped != null) gathered.Public = result.Mapped;
                                else gathered.Detail = result.Error;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    gathered.Detail = e.Message;
                }
                if (includeLocal)
                {
                    try
                    {
                        gathered.Local = localForTests != null ? localForTests() : localAddresses(servers);
                    }
                    catch (Exception e)
                    {
                        gathered.Detail = (gathered.Detail != null ? gathered.Detail + "; " : "") + "local addresses: " + e.Message;
                    }
                }
                bool closeIt;
                lock (m_gatheredLock)
                {
                    job.Done = true;
                    closeIt = job.Abandoned;
                    // Only the current job's result: an earlier attempt's must not replace it
                    if (!closeIt && job == m_job) m_gathered = gathered;
                }
                if (closeIt) closeQuietly(socket);
            });
        }

        /// <summary>
        /// This machine's LAN-usable IPv4 addresses: from its interfaces or, where they cannot be
        /// listed, the route to STUN.
        /// </summary>
        private static List<IPAddress> localAddresses(List<IPEndPoint> in_stunServers)
        {
            var addresses = new List<IPAddress>();
            try
            {
                foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up) continue;
                    foreach (System.Net.NetworkInformation.UnicastIPAddressInformation unicast in nic.GetIPProperties().UnicastAddresses)
                    {
                        if (isUsableLocal(unicast.Address) && !addresses.Contains(unicast.Address)) addresses.Add(unicast.Address);
                    }
                }
            }
            catch (Exception)
            {
                // Not every platform can list its network interfaces; the route to STUN still gives one
            }
            if (addresses.Count == 0 && in_stunServers != null && in_stunServers.Count > 0)
            {
                // Connecting a UDP socket sends nothing: it only picks the local address the route uses
                using (var probe = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    probe.Connect(in_stunServers[0]);
                    IPAddress address = ((IPEndPoint)probe.LocalEndPoint).Address;
                    if (isUsableLocal(address)) addresses.Add(address);
                }
            }
            return addresses;
        }

        private static bool isUsableLocal(IPAddress in_address)
        {
            if (in_address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(in_address)) return false;
            byte[] b = in_address.GetAddressBytes();
            return !(b[0] == 169 && b[1] == 254) && !(b[0] == 0);   // link-local, unspecified
        }

        /// Sends this player's addresses once gathering is done, and starts punching.
        private void finishGathering()
        {
            Gathered gathered;
            lock (m_gatheredLock)
            {
                gathered = m_gathered;
                if (gathered != null) m_gathered = null;
            }
            if (gathered == null || gathered.Attempt != m_attempt) return;

            int port = Port;
            var mine = new List<Dictionary<string, object>>();
            if (gathered.Public != null) mine.Add(candidate(CANDIDATE_SRFLX, gathered.Public.Address, gathered.Public.Port));
            foreach (IPAddress local in gathered.Local)
            {
                if (mine.Count >= MAX_CANDIDATES) break;
                if (gathered.Public != null && local.Equals(gathered.Public.Address) && port == gathered.Public.Port) continue;
                mine.Add(candidate(CANDIDATE_HOST, local, port));
            }
            if (gathered.Public == null)
            {
                log("STUN gave no public address for the socket" + (gathered.Detail != null ? " (" + gathered.Detail + ")" : "") + "; offering local addresses only");
            }
            if (mine.Count == 0)
            {
                log("no address to offer, so every peer is reported as not reached");
                report();
                fail();
                return;
            }

            var data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = m_lobbyId;
            data[OperationParam.LobbyCandidates.Value] = mine.ToArray();
            post(ServiceOperation.SendDirectConnectCandidates, data, "sending this player's addresses failed: ");
            log("sent " + mine.Count + " address(es); punching");

            // Candidates that came before DIRECT_CONNECT, for this lobby
            foreach (KeyValuePair<string, KeyValuePair<string, List<IPEndPoint>>> early in m_early)
            {
                DirectPeer peer;
                if (early.Value.Key == m_lobbyId && m_peers.TryGetValue(early.Key, out peer)) addCandidates(peer, early.Value.Value);
            }
            m_early.Clear();
            m_state = AttemptState.Punching;
            m_lastPunchMs = nowMs - PUNCH_INTERVAL_MS;
        }

        private static Dictionary<string, object> candidate(string in_type, IPAddress in_address, int in_port)
        {
            var c = new Dictionary<string, object>();
            c["type"] = in_type;
            c["ip"] = in_address.ToString();
            c["port"] = in_port;
            return c;
        }

        #endregion

        #region DIRECT_CONNECT_CANDIDATES

        private void onCandidates(Dictionary<string, object> in_data)
        {
            string from = str(in_data, "cxId");
            if (from == null) return;
            var addresses = new List<IPEndPoint>();
            foreach (Dictionary<string, object> entry in objects(in_data, "candidates"))
            {
                IPAddress ip;
                int port = integer(entry, "port", 0);
                // IPv4 only, for now; the socket is IPv4
                if (IPAddress.TryParse(str(entry, "ip") ?? "", out ip) && ip.AddressFamily == AddressFamily.InterNetwork && port > 0 && port <= 65535)
                {
                    addresses.Add(new IPEndPoint(ip, port));
                }
                if (addresses.Count >= MAX_CANDIDATES) break;
            }

            string lobbyId = str(in_data, "lobbyId");
            bool thisAttempt = lobbyId != null && lobbyId == m_lobbyId;
            if (thisAttempt && m_state == AttemptState.Punching)
            {
                // Only from a peer brainCloud named in DIRECT_CONNECT; brainCloud sets cxId
                DirectPeer peer;
                if (m_peers.TryGetValue(from, out peer)) addCandidates(peer, addresses);
            }
            else if (thisAttempt && m_state == AttemptState.Connected)
            {
                // Every peer is reached already
            }
            else if (lobbyId != null && (m_early.Count < MAX_EARLY_PEERS || m_early.ContainsKey(from)))
            {
                // Sent only once, and can overtake DIRECT_CONNECT or arrive while gathering: kept until ours are sent
                m_early[from] = new KeyValuePair<string, List<IPEndPoint>>(lobbyId, addresses);
            }
        }

        private static void addCandidates(DirectPeer in_peer, List<IPEndPoint> in_addresses)
        {
            foreach (IPEndPoint address in in_addresses)
            {
                if (in_peer.Candidates.Count >= MAX_CANDIDATES) break;
                if (!in_peer.Candidates.Contains(address)) in_peer.Candidates.Add(address);
            }
        }

        #endregion

        #region Update

        /// <summary>Call on the main thread, every frame.</summary>
        public void Update()
        {
            if (StandAside) return;
            switch (m_state)
            {
                case AttemptState.Gathering:
                    if (m_socket != null) finishGathering();
                    if (m_state == AttemptState.Gathering && pastDeadline())
                    {
                        log("still gathering addresses at the deadline");
                        report();
                        fail();
                    }
                    break;

                case AttemptState.Punching:
                    if (m_probe != null && !m_probe.Enabled)
                    {
                        log("direct connect was turned off during the attempt");
                        report();
                        fail();
                        break;
                    }
                    receive();
                    if (m_state != AttemptState.Punching) break;
                    if (allReached() || pastDeadline())
                    {
                        report();
                        if (allReached())
                        {
                            m_state = AttemptState.Connected;
                            m_connectedAtMs = nowMs;
                            m_lastKeepMappingMs = m_connectedAtMs;
                        }
                        else
                        {
                            fail();
                        }
                        break;
                    }
                    punch();
                    break;

                case AttemptState.Connected:
                    // Peers may still be punching: keep answering until the connection takes over
                    receive();
                    if (m_state == AttemptState.Connected && nowMs - m_connectedAtMs > CONNECTED_IDLE_MS)
                    {
                        log("nothing took the connection over; closing it");
                        fail();
                        break;
                    }
                    keepMappings();
                    break;
            }
        }

        // Punches each peer at its address now and then while waiting, so the routers keep the path open
        private void keepMappings()
        {
            int now = nowMs;
            if (now - m_lastKeepMappingMs < (KeepMappingMsForTests > 0 ? KeepMappingMsForTests : KEEP_MAPPING_MS)) return;
            m_lastKeepMappingMs = now;
            prunePunches(now);
            foreach (DirectPeer peer in m_peers.Values)
            {
                if (peer.Address != null) punchAt(peer, peer.Address, now);
            }
        }

        private bool pastDeadline()
        {
            return nowMs - m_deadlineMs >= 0;
        }

        private bool allReached()
        {
            foreach (DirectPeer peer in m_peers.Values)
            {
                if (!peer.Reached) return false;
            }
            return true;
        }

        private void punch()
        {
            int now = nowMs;
            if (now - m_lastPunchMs < PUNCH_INTERVAL_MS) return;
            m_lastPunchMs = now;
            prunePunches(now);
            foreach (DirectPeer peer in m_peers.Values)
            {
                if (peer.Reached) continue;
                foreach (IPEndPoint address in peer.Candidates)
                {
                    punchAt(peer, address, now);
                }
                // Addresses still owed punches; paid-up ones, and ones now candidates, are dropped
                if (peer.Triggered.Count == 0) continue;
                foreach (IPEndPoint address in new List<IPEndPoint>(peer.Triggered.Keys))
                {
                    int owed = peer.Triggered[address];
                    if (owed <= 1 || peer.Candidates.Contains(address)) peer.Triggered.Remove(address);
                    else peer.Triggered[address] = owed - 1;
                    if (owed >= 1 && !peer.Candidates.Contains(address)) punchAt(peer, address, now);
                }
            }
        }

        private void punchAt(DirectPeer in_peer, IPEndPoint in_address, int in_now)
        {
            uint id = ++m_nextPunchId;
            m_punches[id] = new Punch { Peer = in_peer, SentAtMs = in_now };
            sendTo(in_peer, PUNCH, id, in_address);
        }

        private void prunePunches(int in_now)
        {
            List<uint> old = null;
            foreach (KeyValuePair<uint, Punch> punch in m_punches)
            {
                if (in_now - punch.Value.SentAtMs > (PunchRecordMsForTests > 0 ? PunchRecordMsForTests : PUNCH_RECORD_MS)) (old ?? (old = new List<uint>())).Add(punch.Key);
            }
            if (old == null) return;
            foreach (uint id in old) m_punches.Remove(id);
        }

        /// For the tests: how many received datagrams were opened (decrypted or authenticated).
        internal int DatagramsOpened { get; private set; }

        /// For the tests: the attempt's socket, while it has one.
        internal Socket SocketForTests { get { return m_socket; } }

        /// For the tests: how many punches are remembered.
        internal int PunchesRemembered { get { return m_punches.Count; } }

        private void sendTo(DirectPeer in_peer, byte in_type, uint in_id, IPEndPoint in_address)
        {
            byte[] packet = { in_type, (byte)(in_id >> 24), (byte)(in_id >> 16), (byte)(in_id >> 8), (byte)in_id };
            try
            {
                byte[] datagram = m_session.SenderFor(in_peer.NetId).Seal(packet, 0, packet.Length);
                m_socket.SendTo(datagram, in_address);
            }
            catch (SocketException)
            {
                // An address this network cannot route to, such as another network's local address
            }
        }

        private void receive()
        {
            for (int i = 0; i < MAX_RECEIVES_PER_UPDATE && m_socket != null; ++i)
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
                    // An ICMP "unreachable" for an earlier send to an address that does not answer
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    return;
                }
                handle(m_buffer, length, (IPEndPoint)from);
            }
        }

        private void handle(byte[] in_datagram, int in_length, IPEndPoint in_from)
        {
            // Only punches are exchanged here: anything else is dropped before any key is derived or anything decrypted
            if (in_length != PUNCH_DATAGRAM_LENGTH) return;
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(in_datagram, in_length, out sender, out receiver, out epoch, out sequence)) return;
            DirectPeer peer;
            if (!m_peersByNetId.TryGetValue(sender, out peer)) return;
            ++DatagramsOpened;
            byte[] packet = m_session.ReceiverFor(sender).Open(in_datagram, in_length);
            if (packet == null || packet.Length != 5 || (packet[0] != PUNCH && packet[0] != PUNCH_ACK)) return;

            // Authenticated, so from the peer: send there from now on, unless a newer datagram moved it
            peer.MoveAddress(in_from, epoch, sequence);
            uint id = ((uint)packet[1] << 24) | ((uint)packet[2] << 16) | ((uint)packet[3] << 8) | packet[4];
            if (packet[0] == PUNCH)
            {
                // Answered even once the peer is reached: its own punches may still be getting through
                sendTo(peer, PUNCH_ACK, id, in_from);
                // A punch from a non-candidate address (a mapping the peer could not know) earns it
                // TRIGGERED_PUNCHES punches, the first now. Bounded, since anyone on the path can replay a
                // punch from any address; a repeat only renews the count.
                if (!peer.Reached && !peer.Candidates.Contains(in_from))
                {
                    if (peer.Triggered.ContainsKey(in_from))
                    {
                        peer.Triggered[in_from] = TRIGGERED_PUNCHES - 1;
                    }
                    else if (peer.Triggered.Count < MAX_CANDIDATES)
                    {
                        peer.Triggered[in_from] = TRIGGERED_PUNCHES - 1;
                        punchAt(peer, in_from, nowMs);
                    }
                }
            }
            else if (packet[0] == PUNCH_ACK && !peer.Reached)
            {
                Punch sent;
                if (!m_punches.TryGetValue(id, out sent) || sent.Peer != peer) return;
                peer.Reached = true;
                peer.RoundTripMs = nowMs - sent.SentAtMs;
                log("reached net id " + peer.NetId + ", round trip " + peer.RoundTripMs + " ms");
            }
        }

        #endregion

        /// Reports which peers were reached, once per attempt.
        private void report()
        {
            if (m_reported || m_lobbyId == null || (m_peers.Count == 0 && m_unusablePeers.Count == 0)) return;
            m_reported = true;
            var peers = new Dictionary<string, object>();
            foreach (DirectPeer peer in m_peers.Values) peers[peer.CxId] = peer.Reached;
            foreach (string cxId in m_unusablePeers) peers[cxId] = false;
            var data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = m_lobbyId;
            data[OperationParam.LobbyPeers.Value] = peers;
            post(ServiceOperation.DirectConnectResult, data, "reporting the result failed: ");
        }

        private void post(ServiceOperation in_operation, Dictionary<string, object> in_data, string in_failureLog)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(
                (response, cbObject) => { },
                (status, reasonCode, error, cbObject) => log(in_failureLog + status + " " + reasonCode + " " + error));
            m_send(new ServerCall(ServiceName.Lobby, in_operation, in_data, callback));
        }

        private void fail()
        {
            close();
            m_punches.Clear();
            m_state = AttemptState.Failed;
        }

        private void close()
        {
            if (m_session != null)
            {
                m_session.Clear();
                m_session = null;
            }
            Socket socket = m_socket;
            m_socket = null;
            bool gathererClosesIt = false;
            lock (m_gatheredLock)
            {
                if (m_job != null && !m_job.Done && m_job.Socket == socket)
                {
                    // The gathering thread may be waiting on the socket: it closes it when it ends
                    m_job.Abandoned = true;
                    gathererClosesIt = true;
                }
                m_job = null;
                m_gathered = null;
            }
            if (socket != null && !gathererClosesIt) closeQuietly(socket);
        }

        private static void closeQuietly(Socket in_socket)
        {
            try
            {
                in_socket.Close();
            }
            catch (Exception)
            {
                // Closing is best effort
            }
        }

        #region JSON

        private static string str(Dictionary<string, object> in_data, string in_key)
        {
            object value;
            return in_data != null && in_data.TryGetValue(in_key, out value) ? value as string : null;
        }

        private static int integer(Dictionary<string, object> in_data, string in_key, int in_default)
        {
            object value;
            if (in_data == null || !in_data.TryGetValue(in_key, out value) || value == null) return in_default;
            if (value is int) return (int)value;
            if (value is long && (long)value >= int.MinValue && (long)value <= int.MaxValue) return (int)(long)value;
            if (value is double && (double)value == Math.Floor((double)value) && Math.Abs((double)value) <= int.MaxValue) return (int)(double)value;
            return in_default;
        }

        private static IEnumerable<Dictionary<string, object>> objects(Dictionary<string, object> in_data, string in_key)
        {
            object value;
            if (in_data == null || !in_data.TryGetValue(in_key, out value) || value is string) yield break;
            var list = value as IEnumerable;
            if (list == null) yield break;
            foreach (object item in list)
            {
                var entry = item as Dictionary<string, object>;
                if (entry != null) yield return entry;
            }
        }

        private static byte[] base64(string in_value)
        {
            if (in_value == null) return null;
            try
            {
                return Convert.FromBase64String(in_value);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        /// <summary>A lobby message without its pairKeys, for the game's lobby callback.</summary>
        internal static string WithoutPairKeys(string in_json)
        {
            try
            {
                var message = JsonReader.Deserialize(in_json) as Dictionary<string, object>;
                Dictionary<string, object> data = message != null && message.ContainsKey("data") ? message["data"] as Dictionary<string, object> : null;
                bool removed = false;
                foreach (Dictionary<string, object> peer in objects(data, "peers"))
                {
                    removed |= peer.Remove("pairKey");
                }
                return removed ? JsonWriter.Serialize(message) : in_json;
            }
            catch (Exception)
            {
                // Never the keys, even when the message cannot be read
                return RedactForLog(in_json);
            }
        }

        private static readonly System.Text.RegularExpressions.Regex PAIR_KEY_JSON =
            new System.Text.RegularExpressions.Regex("\"pairKey\"\\s*:\\s*\"[^\"]*\"");
        private static readonly System.Text.RegularExpressions.Regex CANDIDATES_JSON =
            new System.Text.RegularExpressions.Regex("\"candidates\"\\s*:\\s*\\[[^\\]]*\\]");

        /// <summary>An RTT message safe to log: pair keys and other players' addresses redacted.</summary>
        internal static string RedactForLog(string in_message)
        {
            if (in_message == null) return null;
            string redacted = in_message;
            if (redacted.IndexOf("\"pairKey\"", StringComparison.Ordinal) >= 0)
            {
                redacted = PAIR_KEY_JSON.Replace(redacted, "\"pairKey\":\"(redacted)\"");
            }
            if (redacted.IndexOf("\"candidates\"", StringComparison.Ordinal) >= 0)
            {
                redacted = CANDIDATES_JSON.Replace(redacted, "\"candidates\":\"(redacted)\"");
            }
            return redacted;
        }

        /// Parses an RTT lobby message's data, or null.
        internal static Dictionary<string, object> DataOf(string in_json)
        {
            try
            {
                var message = JsonReader.Deserialize(in_json) as Dictionary<string, object>;
                object data;
                return message != null && message.TryGetValue("data", out data) ? data as Dictionary<string, object> : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
