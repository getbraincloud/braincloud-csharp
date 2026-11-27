// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BrainCloudTests
{
    /// <summary>
    /// Direct connect attempts with no server: two players on localhost, given the lobby events
    /// brainCloud would send, with their calls to brainCloud captured.
    /// </summary>
    [TestFixture]
    public class TestDirectConnector
    {
        private const string LOBBY = "24223:directLobby:1";
        private const string CX_A = "24223:a:cx", CX_B = "24223:b:cx", CX_C = "24223:c:cx";
        private static readonly string CIPHER = DirectPacket.SupportedCiphers[0];
        private static readonly byte[] KEY_AB = Enumerable.Range(1, 16).Select(i => (byte)i).ToArray();

        private sealed class Player
        {
            public readonly List<ServerCall> Sent = new List<ServerCall>();
            public NatProbe Probe;
            public DirectConnector Connector;
            public string CxId;
            public int CandidatesDelivered;

            public IEnumerable<ServerCall> Calls(ServiceOperation in_operation)
            {
                return Sent.Where(c => c.GetOperation() == in_operation.Value);
            }
        }

        private readonly List<Player> m_players = new List<Player>();

        [TearDown]
        public void CloseSockets()
        {
            foreach (Player player in m_players)
            {
                DirectConnection taken = player.Connector.TakeConnection(LOBBY);
                if (taken != null) taken.Socket.Close();
                player.Connector.OnRttDisconnected();
            }
            m_players.Clear();
        }

        private Player NewPlayer(string in_cxId, bool in_enabled = true, int in_budgetMs = 3000)
        {
            var player = new Player { CxId = in_cxId };
            player.Probe = new NatProbe(null, call => { });
            player.Probe.Enabled = in_enabled;
            player.Connector = new DirectConnector(null, player.Probe, call => player.Sent.Add(call));
            // No STUN service here: localhost is the address the peer can reach
            player.Connector.LocalAddressesForTests = () => new List<IPAddress> { IPAddress.Loopback };
            player.Connector.PunchBudgetMsForTests = in_budgetMs;
            m_players.Add(player);
            return player;
        }

        private static Dictionary<string, object> DirectConnect(int in_netId, string in_owner, string in_cipher, params object[] in_peers)
        {
            var peers = new List<object>();
            for (int i = 0; i < in_peers.Length; i += 3)
            {
                var peer = new Dictionary<string, object> { { "cxId", in_peers[i] }, { "netId", in_peers[i + 1] } };
                if (in_peers[i + 2] != null) peer["pairKey"] = Convert.ToBase64String((byte[])in_peers[i + 2]);
                peers.Add(peer);
            }
            return new Dictionary<string, object>
            {
                { "lobbyId", LOBBY }, { "timeoutSecs", 6 }, { "netId", in_netId }, { "ownerCxId", in_owner },
                { "peers", peers.ToArray() }, { "cipher", in_cipher }
            };
        }

        /// A direct ROOM_READY's data, as brainCloud sends it once every member reached every other.
        private static Dictionary<string, object> RoomReady(string in_lobbyId, int in_keepAliveRateSecs = 0)
        {
            var connectData = new Dictionary<string, object> { { "type", "direct" }, { "peers", new object[0] } };
            if (in_keepAliveRateSecs > 0) connectData["keepAliveRateSecs"] = in_keepAliveRateSecs;
            return new Dictionary<string, object> { { "lobbyId", in_lobbyId }, { "connectData", connectData } };
        }

        /// The candidates player in_from sent to brainCloud, as brainCloud passes them on.
        private static Dictionary<string, object> CandidatesFrom(Player in_from, ServerCall in_call)
        {
            IDictionary data = in_call.GetJsonData();
            return new Dictionary<string, object> { { "lobbyId", data["lobbyId"] }, { "cxId", in_from.CxId }, { "candidates", data["candidates"] } };
        }

        /// Runs both players, passing each one's candidates to the other, until both are done or in_ms pass.
        private void Run(Player a, Player b, int in_ms = 5000)
        {
            RunAll(new[] { a, b }.Distinct().ToList(), in_ms, (from, to) => true);
        }

        /// <summary>
        /// Runs the players, passing candidates between them when in_deliver allows, until all are done or in_ms pass.
        /// </summary>
        private void RunAll(List<Player> in_players, int in_ms, Func<Player, Player, bool> in_deliver)
        {
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (clock.ElapsedMilliseconds < in_ms)
            {
                foreach (Player player in in_players)
                {
                    player.Connector.Update();
                    List<ServerCall> sent = player.Calls(ServiceOperation.SendDirectConnectCandidates).ToList();
                    while (player.CandidatesDelivered < sent.Count)
                    {
                        ServerCall call = sent[player.CandidatesDelivered++];
                        foreach (Player other in in_players)
                        {
                            if (other != player && in_deliver(player, other)) other.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", CandidatesFrom(player, call));
                        }
                    }
                }
                if (in_players.All(Done)) return;
                Thread.Sleep(5);
            }
        }

        private static bool Done(Player in_player)
        {
            DirectConnector.AttemptState state = in_player.Connector.State;
            return state == DirectConnector.AttemptState.Connected || state == DirectConnector.AttemptState.Failed;
        }

        /// What the player reported with DIRECT_CONNECT_RESULT, by peer, or null when it did not report.
        private static Dictionary<string, object> Reported(Player in_player)
        {
            ServerCall call = in_player.Calls(ServiceOperation.DirectConnectResult).SingleOrDefault();
            if (call == null) return null;
            Assert.That(call.GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
            return (Dictionary<string, object>)call.GetJsonData()["peers"];
        }

        [Test]
        public void TwoPlayers_ReachEachOther_AndReportIt()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB)), Is.False, "passed on to the game");
            Assert.That(b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB)), Is.False);

            Run(a, b);

            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));
            Assert.That(b.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, true } }));
            Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, true } }));

            // Each knows where to send to the other: the address the other's datagrams came from
            DirectPeer bSeenByA = a.Connector.Peers.Single();
            Assert.That(bSeenByA.Reached, Is.True);
            Assert.That(bSeenByA.Address, Is.EqualTo(new IPEndPoint(IPAddress.Loopback, b.Connector.Port)));
            Assert.That(bSeenByA.RoundTripMs, Is.GreaterThanOrEqualTo(0));

            var sent = (IEnumerable)a.Calls(ServiceOperation.SendDirectConnectCandidates).Single().GetJsonData()["candidates"];
            var first = sent.Cast<Dictionary<string, object>>().Single();
            Assert.That(first["type"], Is.EqualTo("host"));
            Assert.That(first["ip"], Is.EqualTo("127.0.0.1"));
            Assert.That(first["port"], Is.EqualTo(a.Connector.Port));

            // Not before the direct ROOM_READY: until then another member may still fail to reach someone
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null, "before ROOM_READY");

            // The connection takes the socket and keys over, and the attempt is then over
            a.Connector.OnRoomReady(RoomReady(LOBBY, 30));
            DirectConnection connection = a.Connector.TakeConnection(LOBBY);
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.KeepAliveRateSecs, Is.EqualTo(30));
            Assert.That(connection.Socket, Is.Not.Null);
            Assert.That(connection.Session.NetId, Is.EqualTo(0));
            Assert.That(connection.OwnerCxId, Is.EqualTo(CX_A));
            Assert.That(connection.Peers.Single().CxId, Is.EqualTo(CX_B));
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.None));
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null, "taken once");
            connection.Socket.Close();
        }

        [Test]
        public void ARoomReadyForAnotherLobby_OrNamingAServer_DoesNotHandOver()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            Run(a, b);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));

            a.Connector.OnRoomReady(RoomReady("24223:otherLobby:2"));
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null, "another lobby's ROOM_READY");
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));

            // The lobby went to a server after all (another member gave up, say): the socket is closed
            var relay = new Dictionary<string, object>
            {
                { "lobbyId", LOBBY },
                { "connectData", new Dictionary<string, object> { { "address", "relay.example" }, { "ports", new Dictionary<string, object> { { "udp", 9000 } } } } }
            };
            a.Connector.OnRoomReady(relay);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Failed));
            Assert.That(a.Connector.SocketForTests, Is.Null);
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null);
        }

        [Test]
        public void RoomReady_ReachesTheConnectorThroughFilterEvent_AndIsPassedOn()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            Run(a, b);
            string json = BrainCloud.JsonFx.Json.JsonWriter.Serialize(new Dictionary<string, object>
            {
                { "service", "lobby" }, { "operation", "ROOM_READY" }, { "data", RoomReady(LOBBY, 15) }
            });
            string passedOn = json;
            Assert.That(a.Connector.FilterEvent("lobby", "room_ready", ref passedOn), Is.False, "the game connects from it");
            Assert.That(passedOn, Is.EqualTo(json), "unchanged");
            DirectConnection connection = a.Connector.TakeConnection(LOBBY);
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.KeepAliveRateSecs, Is.EqualTo(15));
            connection.Socket.Close();
        }

        [Test]
        public void TheLobbyOwner_IsFollowedFromTheLobbysEvents_AfterTheHandOver()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            Run(a, b);
            a.Connector.OnRoomReady(RoomReady(LOBBY, 30));
            DirectConnection connection = a.Connector.TakeConnection(LOBBY);
            Assert.That(connection.LobbyOwnerCxId, Is.EqualTo(CX_A), "as DIRECT_CONNECT named");

            string left = BrainCloud.JsonFx.Json.JsonWriter.Serialize(new Dictionary<string, object>
            {
                { "service", "lobby" }, { "operation", "MEMBER_LEFT" },
                { "data", new Dictionary<string, object> { { "lobbyId", LOBBY }, { "lobby", new Dictionary<string, object> { { "ownerCxId", CX_B } } } } }
            });
            string passedOn = left;
            Assert.That(a.Connector.FilterEvent("lobby", "member_left", ref passedOn), Is.False);
            Assert.That(passedOn, Is.EqualTo(left), "passed on unchanged");
            Assert.That(connection.LobbyOwnerCxId, Is.EqualTo(CX_B));

            // Another lobby's events change nothing
            string other = left.Replace(LOBBY, "24223:otherLobby:2").Replace(CX_B, CX_C);
            a.Connector.FilterEvent("lobby", "member_left", ref other);
            Assert.That(connection.LobbyOwnerCxId, Is.EqualTo(CX_B));
            connection.Socket.Close();
        }

        [Test]
        public void CandidatesThatArriveBeforeDirectConnect_AreKept()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            // B starts first, and its addresses reach A before A's own DIRECT_CONNECT
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (!b.Calls(ServiceOperation.SendDirectConnectCandidates).Any() && clock.ElapsedMilliseconds < 3000)
            {
                b.Connector.Update();
                System.Threading.Thread.Sleep(5);
            }
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", CandidatesFrom(b, b.Calls(ServiceOperation.SendDirectConnectCandidates).Single())), Is.True);
            b.CandidatesDelivered = 1;

            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            Run(a, b);

            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, true } }));
            Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, true } }));
        }

        [Test]
        public void CandidatesAreTheLibrarys_AndOnlyFromAPeerInTheAttempt()
        {
            Player a = NewPlayer(CX_A, in_budgetMs: 600);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            var candidates = new Dictionary<string, object>
            {
                { "lobbyId", LOBBY }, { "cxId", CX_C },
                { "candidates", new object[] { new Dictionary<string, object> { { "type", "host" }, { "ip", "127.0.0.1" }, { "port", 9 } } } }
            };
            // It carries another player's address: never passed on to the game, whoever it is from
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", candidates), Is.True);
            Run(a, a, 2000);
            Assert.That(a.Connector.Peers.Single().Candidates, Is.Empty, "C is not a peer in this attempt");
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
        }

        [Test]
        public void APeerNeverHeardFrom_IsReportedNotReached_BeforeTheServersTimer()
        {
            Player a = NewPlayer(CX_A, in_budgetMs: 500);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            Run(a, a, 3000);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Failed));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null);
        }

        [Test]
        public void APlayerWithAnotherKey_IsNeverReached()
        {
            // B was given another key for the pair: neither can open the other's punches
            Player a = NewPlayer(CX_A, in_budgetMs: 800), b = NewPlayer(CX_B, in_budgetMs: 800);
            byte[] otherKey = Enumerable.Range(100, 16).Select(i => (byte)i).ToArray();
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, otherKey));
            Run(a, b, 3000);
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
            Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, false } }));
            Assert.That(a.Connector.Peers.Single().Address, Is.Null, "nothing that did not open moved the peer's address");
        }

        [Test]
        public void DirectConnectFailed_StopsTheAttempt_WithNothingToReport()
        {
            Player a = NewPlayer(CX_A);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            a.Connector.Update();
            var failed = new Dictionary<string, object> { { "lobbyId", LOBBY }, { "reason", "MEMBER_LEFT" } };
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT_FAILED", failed), Is.False, "passed on to the game");
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Failed));
            Run(a, a, 300);
            Assert.That(Reported(a), Is.Null);
        }

        [Test]
        public void TurnedOff_ReportsEveryPeerNotReached_AtOnce()
        {
            Player a = NewPlayer(CX_A, in_enabled: false);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Failed));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
            Assert.That(a.Calls(ServiceOperation.SendDirectConnectCandidates), Is.Empty, "no address is shared");
        }

        [TestCase("aes-512-future")]
        [TestCase(null)]
        public void ACipherThisLibraryDoesNotSupport_IsRefused(string in_cipher)
        {
            Player a = NewPlayer(CX_A);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, in_cipher, CX_B, 1, KEY_AB));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
            Assert.That(a.Calls(ServiceOperation.SendDirectConnectCandidates), Is.Empty);
        }

        [Test]
        public void ANetIdTheRelayProtocolCannotAddress_IsRefused_AndReported()
        {
            // The relay protocol has net ids 0 to 39
            Player a = NewPlayer(CX_A);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 40, KEY_AB));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }), "reported, so brainCloud is not left waiting");
            Assert.That(a.Calls(ServiceOperation.SendDirectConnectCandidates), Is.Empty);

            Player c = NewPlayer(CX_C);
            c.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(40, CX_A, CIPHER, CX_A, 0, KEY_AB));
            Assert.That(Reported(c), Is.EqualTo(new Dictionary<string, object> { { CX_A, false } }), "this player's own");
        }

        [Test]
        public void WhileWaitingToBeTakenOver_EachPeerIsPunched_ToKeepThePathOpen()
        {
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            Run(a, b);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));

            a.Connector.KeepMappingMsForTests = 50;
            int openedByB = b.Connector.DatagramsOpened;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (clock.ElapsedMilliseconds < 400)
            {
                a.Connector.Update();
                b.Connector.Update();
                Thread.Sleep(5);
            }
            Assert.That(b.Connector.DatagramsOpened - openedByB, Is.GreaterThanOrEqualTo(3), "A kept punching B");
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Connected));
        }

        [Test]
        public void APeersAddress_OnlyMovesToALaterDatagram()
        {
            var peer = new DirectPeer(CX_B, 1).Clear();
            var first = new IPEndPoint(IPAddress.Loopback, 1000);
            var second = new IPEndPoint(IPAddress.Loopback, 2000);
            byte[] epoch1 = DirectPacket.MakeEpoch(new byte[4], 1), epoch2 = DirectPacket.MakeEpoch(new byte[4], 2);
            peer.MoveAddress(first, epoch1, 5);
            peer.MoveAddress(second, epoch1, 4);
            Assert.That(peer.Address, Is.EqualTo(first), "an earlier sequence number");
            peer.MoveAddress(second, epoch1, 5);
            Assert.That(peer.Address, Is.EqualTo(first), "the same datagram again");
            peer.MoveAddress(second, epoch2, 0);
            Assert.That(peer.Address, Is.EqualTo(second), "a later epoch");
            peer.MoveAddress(first, epoch1, 99);
            Assert.That(peer.Address, Is.EqualTo(second), "an earlier epoch");
        }

        [Test]
        public void APeerWithoutAPairKey_IsRefused()
        {
            // As from a server that sends no pair keys
            Player a = NewPlayer(CX_A);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, null));
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
            Assert.That(a.Calls(ServiceOperation.SendDirectConnectCandidates), Is.Empty);
        }

        [Test]
        public void TheGamesCopyOfDirectConnect_HasNoPairKeys()
        {
            string json = "{\"service\":\"lobby\",\"operation\":\"DIRECT_CONNECT\",\"data\":{\"lobbyId\":\"" + LOBBY + "\",\"netId\":0," +
                "\"peers\":[{\"cxId\":\"" + CX_B + "\",\"netId\":1,\"pairKey\":\"AQIDBAUGBwgJCgsMDQ4PEA==\"}],\"cipher\":\"hmac-sha256\"}}";
            string forGame = DirectConnector.WithoutPairKeys(json);
            Assert.That(forGame, Does.Not.Contain("pairKey"));
            Assert.That(forGame, Does.Not.Contain("AQIDBAUGBwgJCgsMDQ4PEA=="));
            Dictionary<string, object> data = DirectConnector.DataOf(forGame);
            Assert.That(data["lobbyId"], Is.EqualTo(LOBBY));
            Assert.That(data["cipher"], Is.EqualTo("hmac-sha256"));
            var peer = ((IEnumerable)data["peers"]).Cast<Dictionary<string, object>>().Single();
            Assert.That(peer["cxId"], Is.EqualTo(CX_B));
            Assert.That(peer["netId"], Is.EqualTo(1));
            // A message with no pair key is passed on as it came
            string other = "{\"service\":\"lobby\",\"operation\":\"STARTING\",\"data\":{\"lobbyId\":\"x\"}}";
            Assert.That(DirectConnector.WithoutPairKeys(other), Is.SameAs(other));
        }

        [Test]
        public void StandingAside_TheConnectorTakesNoPartAndConsumesNothing()
        {
            // For tools such as stun-probe that play the attempt themselves
            Player a = NewPlayer(CX_A, in_enabled: false);
            a.Connector.StandAside = true;
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB)), Is.False);
            var candidates = new Dictionary<string, object> { { "lobbyId", LOBBY }, { "cxId", CX_B }, { "candidates", new object[0] } };
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", candidates), Is.False, "the tool needs them");
            a.Connector.Update();
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.None));
            Assert.That(a.Sent, Is.Empty, "nothing reported, although direct connect is off in the library");
        }

        [Test]
        public void ThreePlayers_EachReachesBothOthers()
        {
            byte[] keyAC = Enumerable.Range(40, 16).Select(i => (byte)i).ToArray(), keyBC = Enumerable.Range(70, 16).Select(i => (byte)i).ToArray();
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B), c = NewPlayer(CX_C);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB, CX_C, 2, keyAC));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB, CX_C, 2, keyBC));
            c.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(2, CX_A, CIPHER, CX_A, 0, keyAC, CX_B, 1, keyBC));
            RunAll(new List<Player> { a, b, c }, 5000, (from, to) => true);
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, true }, { CX_C, true } }));
            Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, true }, { CX_C, true } }));
            Assert.That(Reported(c), Is.EqualTo(new Dictionary<string, object> { { CX_A, true }, { CX_B, true } }));
        }

        [Test]
        public void APeerIsAlsoPunchedWhereItsPunchesComeFrom()
        {
            // A's addresses never reach B (lost, or a router mapping A could not know to offer). B
            // only learns where A is from A's authenticated punches, and must punch that address too.
            Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { a, b }, 5000, (from, to) => from == b);
            Assert.That(b.Connector.Peers.Single().Candidates, Is.Empty, "A's candidates never arrived");
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, true } }));
            Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, true } }));
        }

        [Test]
        public void Gathering_OffersThePublicAddressSTUNSees()
        {
            using (var stun = new LoopbackStun())
            {
                Player a = NewPlayer(CX_A), b = NewPlayer(CX_B);
                foreach (Player player in new[] { a, b })
                {
                    player.Probe.SetStunSection(new Dictionary<string, object> { { "host", "127.0.0.1" }, { "port", stun.Port } });
                    player.Connector.LocalAddressesForTests = () => new List<IPAddress>();   // STUN's answer alone
                }
                a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
                b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
                Run(a, b);

                var sent = ((IEnumerable)a.Calls(ServiceOperation.SendDirectConnectCandidates).Single().GetJsonData()["candidates"]).Cast<Dictionary<string, object>>().Single();
                Assert.That(sent["type"], Is.EqualTo("srflx"));
                Assert.That(sent["port"], Is.EqualTo(a.Connector.Port), "asked from the game's own socket");
                Assert.That(stun.Requests, Is.GreaterThanOrEqualTo(2));
                Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, true } }));
                Assert.That(Reported(b), Is.EqualTo(new Dictionary<string, object> { { CX_A, true } }));
            }
        }

        [Test]
        public void AnAcknowledgementOfAPunchNeverSent_DoesNotReachThePeer()
        {
            Player a = NewPlayer(CX_A, in_budgetMs: 800);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            RunAll(new List<Player> { a }, 200, (from, to) => false);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Punching));

            // A genuine PUNCH_ACK from B, for a punch id A never sent
            var bSession = new DirectSession(CIPHER, 1, new Dictionary<int, byte[]> { { 0, KEY_AB } });
            byte[] ack = bSession.SenderFor(0).Seal(new byte[] { DirectConnector.PUNCH_ACK, 0x7F, 0, 0, 1 }, 0, 5);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTo(ack, new IPEndPoint(IPAddress.Loopback, a.Connector.Port));
                RunAll(new List<Player> { a }, 2000, (from, to) => false);
            }
            Assert.That(a.Connector.Peers.Single().Reached, Is.False);
            Assert.That(Reported(a), Is.EqualTo(new Dictionary<string, object> { { CX_B, false } }));
        }

        [Test]
        public void CandidatesFromANonPeer_WhilePunching_AreIgnored()
        {
            Player a = NewPlayer(CX_A, in_budgetMs: 800);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            RunAll(new List<Player> { a }, 200, (from, to) => false);
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Punching));
            var fromC = new Dictionary<string, object>
            {
                { "lobbyId", LOBBY }, { "cxId", CX_C },
                { "candidates", new object[] { new Dictionary<string, object> { { "type", "host" }, { "ip", "127.0.0.1" }, { "port", 9 } } } }
            };
            Assert.That(a.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", fromC), Is.True);
            Assert.That(a.Connector.Peers.Single().Candidates, Is.Empty);
        }

        [Test]
        public void ANewAttemptWhileStillGathering_DoesNotWaitForTheOldOne()
        {
            // A STUN service that never answers keeps the first attempt gathering for most of a second
            using (var silent = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                silent.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                Player a = NewPlayer(CX_A);
                a.Probe.SetStunSection(new Dictionary<string, object> { { "host", "127.0.0.1" }, { "port", ((IPEndPoint)silent.LocalEndPoint).Port } });
                a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
                Thread.Sleep(100);
                Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Gathering));

                Socket first = a.Connector.SocketForTests;
                var clock = System.Diagnostics.Stopwatch.StartNew();
                a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
                Assert.That(clock.ElapsedMilliseconds, Is.LessThan(200));
                // Closing a socket another thread waits on can block for that long, so the first
                // attempt's socket is left to its own thread, which closes it when STUN gives up
                Assert.That(() => { int unused = first.Available; }, Throws.Nothing, "not closed from under the gathering thread");
                var waited = System.Diagnostics.Stopwatch.StartNew();
                bool closed = false;
                while (!closed && waited.ElapsedMilliseconds < 3000)
                {
                    try { int unused = first.Available; Thread.Sleep(20); }
                    catch (ObjectDisposedException) { closed = true; }
                }
                Assert.That(closed, Is.True, "closed by the gathering thread once it finished");

                // And the new attempt gets its own result, not the abandoned one's
                RunAll(new List<Player> { a }, 3000, (from, to) => false);
                Assert.That(a.Calls(ServiceOperation.SendDirectConnectCandidates).Count(), Is.EqualTo(1));
            }
        }

        [Test]
        public void OnlyPunchSizedDatagramsAreOpened()
        {
            // Anything else is dropped before any key is derived or anything decrypted
            Player a = NewPlayer(CX_A, in_budgetMs: 800);
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            RunAll(new List<Player> { a }, 200, (from, to) => false);
            var bSession = new DirectSession(CIPHER, 1, new Dictionary<int, byte[]> { { 0, KEY_AB } });
            byte[] big = bSession.SenderFor(0).Seal(new byte[1000], 0, 1000);
            byte[] punch = bSession.SenderFor(0).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 1 }, 0, 5);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                var to = new IPEndPoint(IPAddress.Loopback, a.Connector.Port);
                for (int i = 0; i < 20; ++i) socket.SendTo(big, to);
                RunAll(new List<Player> { a }, 300, (from, x) => false);
                Assert.That(a.Connector.DatagramsOpened, Is.EqualTo(0));
                socket.SendTo(punch, to);
                RunAll(new List<Player> { a }, 300, (from, x) => false);
                Assert.That(a.Connector.DatagramsOpened, Is.EqualTo(1), "a punch is");
            }
        }

        [Test]
        public void OldPunchesAreForgotten()
        {
            Player a = NewPlayer(CX_A, in_budgetMs: 1500);
            a.Connector.PunchRecordMsForTests = 300;
            a.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(0, CX_A, CIPHER, CX_B, 1, KEY_AB));
            var deadAddress = new Dictionary<string, object> { { "type", "host" }, { "ip", "127.0.0.1" }, { "port", 9 } };
            a.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", new Dictionary<string, object> { { "lobbyId", LOBBY }, { "cxId", CX_B }, { "candidates", new object[] { deadAddress } } });
            RunAll(new List<Player> { a }, 1200, (from, to) => false);
            // One address punched every 100 ms, each remembered for 300 ms
            Assert.That(a.Connector.PunchesRemembered, Is.InRange(1, 6));
        }

        [Test]
        public void TheAttemptsEventsAreRecognised_WhateverThePlayersLanguage()
        {
            // In Turkish, "I".ToLower() is a dotless i, which would hide DIRECT_CONNECT_CANDIDATES from the library
            System.Globalization.CultureInfo original = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
                Player a = NewPlayer(CX_A);
                string json = "{\"service\":\"lobby\",\"operation\":\"DIRECT_CONNECT_CANDIDATES\",\"data\":{\"lobbyId\":\"" + LOBBY + "\",\"cxId\":\"" + CX_B +
                    "\",\"candidates\":[{\"type\":\"host\",\"ip\":\"10.1.2.3\",\"port\":4000}]}}";
                Assert.That(a.Connector.FilterEvent(RTTComms.EventName("LOBBY"), RTTComms.EventName("DIRECT_CONNECT_CANDIDATES"), ref json), Is.True, "kept from the game");

                string start = "{\"service\":\"lobby\",\"operation\":\"DIRECT_CONNECT\",\"data\":" +
                    "{\"lobbyId\":\"" + LOBBY + "\",\"netId\":0,\"peers\":[{\"cxId\":\"" + CX_B + "\",\"netId\":1,\"pairKey\":\"" + Convert.ToBase64String(KEY_AB) + "\"}],\"cipher\":\"" + CIPHER + "\",\"timeoutSecs\":6}}";
                Assert.That(a.Connector.FilterEvent(RTTComms.EventName("lobby"), RTTComms.EventName("DIRECT_CONNECT"), ref start), Is.False, "passed on");
                Assert.That(start, Does.Not.Contain("pairKey"));
                Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Gathering), "and the library took part");
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentCulture = original;
            }
        }

        [Test]
        public void ThroughRTTComms_TheAttemptsEventsAreTheLibrarys_WhateverThePlayersLanguage()
        {
            // The real receive path: the event server's message, lowercased and queued by RTTComms,
            // then dispatched by its Update to the game's lobby callback
            System.Globalization.CultureInfo original = System.Globalization.CultureInfo.CurrentCulture;
            try
            {
                System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
                var rtt = new RTTComms(new BrainCloudClient());
                var toGame = new List<string>();
                rtt.RegisterRTTCallback(ServiceName.Lobby, json => toGame.Add(json));
                rtt.ReceiveForTests("{\"service\":\"lobby\",\"operation\":\"DIRECT_CONNECT_CANDIDATES\",\"data\":{\"lobbyId\":\"" + LOBBY + "\",\"cxId\":\"" + CX_B +
                    "\",\"candidates\":[{\"type\":\"host\",\"ip\":\"10.1.2.3\",\"port\":4000}]}}");
                rtt.ReceiveForTests("{\"service\":\"lobby\",\"operation\":\"STARTING\",\"data\":{\"lobbyId\":\"" + LOBBY + "\"}}");
                rtt.Update();
                Assert.That(toGame.Count, Is.EqualTo(1), "only STARTING reaches the game");
                Assert.That(toGame[0], Does.Contain("STARTING"));
                Assert.That(string.Join("", toGame), Does.Not.Contain("10.1.2.3"));
            }
            finally
            {
                System.Globalization.CultureInfo.CurrentCulture = original;
            }
        }

        [Test]
        public void TheFailureCallback_NeverGetsADirectConnectMessage()
        {
            Assert.That(RTTComms.ForFailureCallback("direct_connect_candidates", "{\"candidates\":[{\"ip\":\"10.1.2.3\"}]}"), Is.EqualTo("RTT connection closed"));
            Assert.That(RTTComms.ForFailureCallback("direct_connect", "{\"pairKey\":\"AQID\"}"), Is.EqualTo("RTT connection closed"));
            Assert.That(RTTComms.ForFailureCallback("starting", "{\"x\":1}"), Is.EqualTo("{\"x\":1}"), "anything else as before");
        }

        [Test]
        public void APunchFromAnUnknownAddress_EarnsAFewPunchesBack_NotAStream()
        {
            // Anyone on the path can replay a genuine punch from an address of their choosing
            Player b = NewPlayer(CX_B, in_budgetMs: 1500);
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { b }, 200, (from, to) => false);
            Assert.That(b.Connector.State, Is.EqualTo(DirectConnector.AttemptState.Punching));

            var aSession = new DirectSession(CIPHER, 0, new Dictionary<int, byte[]> { { 1, KEY_AB } });
            byte[] punch = aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 9 }, 0, 5);
            using (var elsewhere = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                elsewhere.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                elsewhere.SendTo(punch, new IPEndPoint(IPAddress.Loopback, b.Connector.Port));
                RunAll(new List<Player> { b }, 1000, (from, to) => false);
                Assert.That(Drain(elsewhere), Is.EqualTo(1 + DirectConnector.TRIGGERED_PUNCHES), "its PUNCH_ACK, and TRIGGERED_PUNCHES punches back");
                RunAll(new List<Player> { b }, 400, (from, to) => false);
                Assert.That(Drain(elsewhere), Is.EqualTo(0), "and then nothing more");
            }
        }

        private static int Drain(Socket in_socket)
        {
            int received = 0;
            var buffer = new byte[2000];
            while (in_socket.Available > 0)
            {
                EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                in_socket.ReceiveFrom(buffer, ref from);
                ++received;
            }
            return received;
        }

        [Test]
        public void ALostPunchBack_IsRetried()
        {
            // A has reached B and stopped punching: B's first punch back is lost, so only a retry reaches A
            Player b = NewPlayer(CX_B, in_budgetMs: 2000);
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { b }, 200, (from, to) => false);

            var aSession = new DirectSession(CIPHER, 0, new Dictionary<int, byte[]> { { 1, KEY_AB } });
            using (var a = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                a.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                var toB = new IPEndPoint(IPAddress.Loopback, b.Connector.Port);
                a.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 1 }, 0, 5), toB);

                int punchesSeen = 0;
                var buffer = new byte[2000];
                var clock = System.Diagnostics.Stopwatch.StartNew();
                while (!b.Connector.Peers.Single().Reached && clock.ElapsedMilliseconds < 1500)
                {
                    b.Connector.Update();
                    while (a.Available > 0)
                    {
                        EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                        int n = a.ReceiveFrom(buffer, ref from);
                        byte[] packet = aSession.ReceiverFor(1).Open(buffer, n);
                        if (packet == null || packet[0] != DirectConnector.PUNCH) continue;
                        // The first is lost; the second is answered
                        if (++punchesSeen == 2) a.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH_ACK, packet[1], packet[2], packet[3], packet[4] }, 0, 5), toB);
                    }
                    Thread.Sleep(5);
                }
                Assert.That(punchesSeen, Is.GreaterThanOrEqualTo(2));
                Assert.That(b.Connector.Peers.Single().Reached, Is.True);
            }
        }

        [Test]
        public void ARepeatPunch_RenewsTheCount_WithoutAnExtraPunchNow()
        {
            Player b = NewPlayer(CX_B, in_budgetMs: 1500);
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { b }, 200, (from, to) => false);
            var aSession = new DirectSession(CIPHER, 0, new Dictionary<int, byte[]> { { 1, KEY_AB } });
            using (var a = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                a.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                var toB = new IPEndPoint(IPAddress.Loopback, b.Connector.Port);
                // Two punches arriving together: two acknowledgements, and the punches of one, not two
                a.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 1 }, 0, 5), toB);
                a.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 2 }, 0, 5), toB);
                Thread.Sleep(20);
                RunAll(new List<Player> { b }, 1000, (from, to) => false);
                Assert.That(Drain(a), Is.EqualTo(2 + DirectConnector.TRIGGERED_PUNCHES));
            }
        }

        [Test]
        public void AnAddressThatIsPaidUp_GivesItsSlotBack()
        {
            // Spoofed punches from MAX_CANDIDATES addresses fill every slot; once they have had their
            // punches, a punch from yet another address must still be punched back
            Player b = NewPlayer(CX_B, in_budgetMs: 2500);
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { b }, 200, (from, to) => false);
            var aSession = new DirectSession(CIPHER, 0, new Dictionary<int, byte[]> { { 1, KEY_AB } });
            var toB = new IPEndPoint(IPAddress.Loopback, b.Connector.Port);
            var fillers = new List<Socket>();
            try
            {
                for (int i = 0; i < DirectConnector.MAX_CANDIDATES; ++i)
                {
                    var filler = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    filler.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    fillers.Add(filler);
                    filler.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 1, (byte)i }, 0, 5), toB);
                }
                RunAll(new List<Player> { b }, 800, (from, to) => false);
                using (var late = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    late.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    late.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 2, 0 }, 0, 5), toB);
                    RunAll(new List<Player> { b }, 300, (from, to) => false);
                    Assert.That(Drain(late), Is.GreaterThan(1), "its acknowledgement and at least one punch back");
                }
            }
            finally
            {
                foreach (Socket filler in fillers) filler.Close();
            }
        }

        [Test]
        public void TwoUnknownAddressesInOneBurst_AreBothPunchedBack()
        {
            // A peer on two networks: its punches arrive from two addresses it did not list
            Player b = NewPlayer(CX_B, in_budgetMs: 1500);
            b.Connector.OnLobbyEvent("DIRECT_CONNECT", DirectConnect(1, CX_A, CIPHER, CX_A, 0, KEY_AB));
            RunAll(new List<Player> { b }, 200, (from, to) => false);
            var aSession = new DirectSession(CIPHER, 0, new Dictionary<int, byte[]> { { 1, KEY_AB } });
            using (var first = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            using (var second = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                first.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                second.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                var toB = new IPEndPoint(IPAddress.Loopback, b.Connector.Port);
                first.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 1 }, 0, 5), toB);
                second.SendTo(aSession.SenderFor(1).Seal(new byte[] { DirectConnector.PUNCH, 0, 0, 0, 2 }, 0, 5), toB);
                RunAll(new List<Player> { b }, 1000, (from, to) => false);
                Assert.That(Drain(first), Is.EqualTo(1 + DirectConnector.TRIGGERED_PUNCHES));
                Assert.That(Drain(second), Is.EqualTo(1 + DirectConnector.TRIGGERED_PUNCHES), "neither starves the other");
            }
        }

        [Test]
        public void OtherEvents_PassThroughUnchanged()
        {
            Player a = NewPlayer(CX_A);
            string json = "{\"service\":\"lobby\",\"operation\":\"STARTING\",\"data\":{\"lobbyId\":\"" + LOBBY + "\"}}";
            string passed = json;
            Assert.That(a.Connector.FilterEvent("lobby", "starting", ref passed), Is.False);
            Assert.That(passed, Is.SameAs(json));
            Assert.That(a.Connector.FilterEvent("chat", "direct_connect", ref passed), Is.False, "only the lobby service's");
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.None));
        }

        [Test]
        public void TheLog_ShowsNeitherPairKeysNorOtherPlayersAddresses()
        {
            string start = "{\"operation\":\"DIRECT_CONNECT\",\"data\":{\"peers\":[{\"cxId\":\"x\",\"netId\":1,\"pairKey\" : \"AQID+/BAUGBw==\"}]}}";
            string candidates = "{\"operation\":\"DIRECT_CONNECT_CANDIDATES\",\"data\":{\"cxId\":\"x\",\"candidates\":[{\"type\":\"host\",\"ip\":\"10.1.2.3\",\"port\":4000},{\"type\":\"srflx\",\"ip\":\"203.0.113.9\",\"port\":4001}]}}";
            Assert.That(DirectConnector.RedactForLog(start), Does.Not.Contain("AQID"));
            Assert.That(DirectConnector.RedactForLog(start), Does.Contain("\"cxId\":\"x\""));
            Assert.That(DirectConnector.RedactForLog(candidates), Does.Not.Contain("10.1.2.3").And.Not.Contain("203.0.113.9"));
            Assert.That(DirectConnector.RedactForLog(candidates), Does.Contain("DIRECT_CONNECT_CANDIDATES"));
            // A message that cannot be read never comes back with its keys
            Assert.That(DirectConnector.WithoutPairKeys("{\"pairKey\":\"AQID\", not json"), Does.Not.Contain("AQID"));
        }

        /// A STUN service on localhost that answers every Binding request with the address it came from.
        private sealed class LoopbackStun : IDisposable
        {
            private readonly Socket m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            private readonly Thread m_thread;
            private volatile bool m_stop;
            private int m_requests;

            public LoopbackStun()
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                // Closing a socket another thread is blocked on can hang (.NET on macOS), so the
                // thread wakes regularly and is stopped before the socket is closed
                m_socket.ReceiveTimeout = 100;
                m_thread = new Thread(run) { IsBackground = true };
                m_thread.Start();
            }

            public int Port { get { return ((IPEndPoint)m_socket.LocalEndPoint).Port; } }
            public int Requests { get { return Volatile.Read(ref m_requests); } }

            public void Dispose()
            {
                m_stop = true;
                m_thread.Join(2000);
                m_socket.Close();
            }

            private void run()
            {
                byte[] buffer = new byte[1500];
                while (!m_stop)
                {
                    EndPoint from = new IPEndPoint(IPAddress.Any, 0);
                    int n;
                    try
                    {
                        n = m_socket.ReceiveFrom(buffer, ref from);
                    }
                    catch (SocketException)
                    {
                        continue;
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                    if (n < 20 || buffer[0] != 0 || buffer[1] != 1) continue;
                    Interlocked.Increment(ref m_requests);
                    var source = (IPEndPoint)from;
                    // Binding success: the transaction id, XOR-MAPPED-ADDRESS, FINGERPRINT
                    var reply = new byte[40];
                    reply[0] = 1; reply[1] = 1; reply[3] = 20;
                    Buffer.BlockCopy(buffer, 4, reply, 4, 16);
                    reply[21] = 0x20; reply[23] = 8; reply[25] = 1;
                    int port = source.Port ^ 0x2112;
                    reply[26] = (byte)(port >> 8); reply[27] = (byte)port;
                    byte[] ip = source.Address.GetAddressBytes();
                    for (int i = 0; i < 4; ++i) reply[28 + i] = (byte)(ip[i] ^ reply[4 + i]);
                    reply[32] = 0x80; reply[33] = 0x28; reply[35] = 4;
                    uint crc = StunClient.Fingerprint(reply, 32);
                    reply[36] = (byte)(crc >> 24); reply[37] = (byte)(crc >> 16); reply[38] = (byte)(crc >> 8); reply[39] = (byte)crc;
                    try
                    {
                        m_socket.SendTo(reply, source);
                    }
                    catch (ObjectDisposedException)
                    {
                        return;
                    }
                }
            }
        }

        [Test]
        public void TheNetworkReport_ListsTheCiphersThisBuildSupports()
        {
            Dictionary<string, object> natInfo = new NatReport(StunClient.MAPPING_EIM, true, null, null).ToNatInfo();
            Assert.That((string[])natInfo["crypto"], Is.EqualTo(DirectPacket.SupportedCiphers.ToArray()));
            Assert.That((string[])natInfo["crypto"], Does.Contain(DirectPacket.CIPHER_HMAC_SHA256));
        }
    }
}
