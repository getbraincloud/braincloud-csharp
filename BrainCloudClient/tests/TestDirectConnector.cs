// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (clock.ElapsedMilliseconds < in_ms)
            {
                foreach (Player player in new[] { a, b })
                {
                    player.Connector.Update();
                    Player other = player == a ? b : a;
                    List<ServerCall> sent = player.Calls(ServiceOperation.SendDirectConnectCandidates).ToList();
                    while (player.CandidatesDelivered < sent.Count)
                    {
                        other.Connector.OnLobbyEvent("DIRECT_CONNECT_CANDIDATES", CandidatesFrom(player, sent[player.CandidatesDelivered++]));
                    }
                }
                if (Done(a) && Done(b)) return;
                System.Threading.Thread.Sleep(5);
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
        public void TheNetworkReport_ListsTheCiphersThisBuildSupports()
        {
            Dictionary<string, object> natInfo = new NatReport(StunClient.MAPPING_EIM, true, null, null).ToNatInfo();
            Assert.That((string[])natInfo["crypto"], Is.EqualTo(DirectPacket.SupportedCiphers.ToArray()));
            Assert.That((string[])natInfo["crypto"], Does.Contain(DirectPacket.CIPHER_HMAC_SHA256));
        }
    }
}
