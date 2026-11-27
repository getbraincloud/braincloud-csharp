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
    /// A lobby's direct connect attempt, with no server: two players in this process, on
    /// localhost, each given the lobby events brainCloud would send, and each one's calls to
    /// brainCloud captured, so the test can pass one player's addresses to the other as
    /// brainCloud does.
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
        /// Runs the players, passing each one's candidates to every other as brainCloud does, when
        /// in_deliver says so, until all are done or in_ms pass.
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

            // The candidates went to brainCloud as it expects them
            var sent = (IEnumerable)a.Calls(ServiceOperation.SendDirectConnectCandidates).Single().GetJsonData()["candidates"];
            var first = sent.Cast<Dictionary<string, object>>().Single();
            Assert.That(first["type"], Is.EqualTo("host"));
            Assert.That(first["ip"], Is.EqualTo("127.0.0.1"));
            Assert.That(first["port"], Is.EqualTo(a.Connector.Port));

            // The connection takes the socket and keys over, and the attempt is then over
            DirectConnection connection = a.Connector.TakeConnection(LOBBY);
            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.Socket, Is.Not.Null);
            Assert.That(connection.Session.NetId, Is.EqualTo(0));
            Assert.That(connection.OwnerCxId, Is.EqualTo(CX_A));
            Assert.That(connection.Peers.Single().CxId, Is.EqualTo(CX_B));
            Assert.That(a.Connector.State, Is.EqualTo(DirectConnector.AttemptState.None));
            Assert.That(a.Connector.TakeConnection(LOBBY), Is.Null, "taken once");
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
        public void APeerWithoutAPairKey_IsRefused()
        {
            // As from a server from before pair keys
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
            // Everything else is still there
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
            private int m_requests;

            public LoopbackStun()
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                new Thread(run) { IsBackground = true }.Start();
            }

            public int Port { get { return ((IPEndPoint)m_socket.LocalEndPoint).Port; } }
            public int Requests { get { return Volatile.Read(ref m_requests); } }

            public void Dispose()
            {
                m_socket.Close();
            }

            private void run()
            {
                byte[] buffer = new byte[1500];
                while (true)
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
