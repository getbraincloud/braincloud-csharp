// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BrainCloudTests
{
    /// <summary>
    /// The direct mesh, with no server: two or three players in this process, on localhost, each
    /// with the socket, keys and peer addresses a successful attempt would hand over, and a clock
    /// the test moves on.
    /// </summary>
    [TestFixture]
    public class TestDirectMesh
    {
        private const string LOBBY = "24223:directLobby:1";
        private static readonly string[] CX = { "24223:a:cx", "24223:b:cx", "24223:c:cx", "24223:d:cx" };
        private static readonly string CIPHER = DirectPacket.SupportedCiphers[0];

        private sealed class Listener : IDirectMeshListener
        {
            public readonly List<Dictionary<string, object>> System = new List<Dictionary<string, object>>();
            public readonly List<KeyValuePair<short, byte[]>> Relay = new List<KeyValuePair<short, byte[]>>();
            public readonly List<string> Errors = new List<string>();

            public void OnMeshSystem(string in_json)
            {
                System.Add((Dictionary<string, object>)JsonReader.Deserialize(in_json));
            }

            public void OnMeshRelay(short in_netId, byte[] in_packet)
            {
                // What RelayComms passes to the game: without the length prefix, RELAY byte and header
                byte[] data = new byte[in_packet.Length - 11];
                Buffer.BlockCopy(in_packet, 11, data, 0, data.Length);
                Relay.Add(new KeyValuePair<short, byte[]>(in_netId, data));
            }

            public void OnMeshError(string in_message)
            {
                Errors.Add(in_message);
            }

            public IEnumerable<Dictionary<string, object>> Ops(string in_op)
            {
                return System.Where(m => (string)m["op"] == in_op);
            }

            public IEnumerable<string> Texts(int in_from)
            {
                return Relay.Where(r => r.Key == in_from).Select(r => Encoding.UTF8.GetString(r.Value));
            }
        }

        private sealed class Node
        {
            public int NetId;
            public Socket Socket;
            public DirectSession Session;
            public DirectMeshTransport Mesh;
            public DirectConnection Connection;
            public readonly Listener Listener = new Listener();
            public readonly List<ServerCall> Sent = new List<ServerCall>();
            public bool Paused;

            public IEnumerable<ServerCall> Calls(ServiceOperation in_operation)
            {
                return Sent.Where(c => c.GetOperation() == in_operation.Value);
            }
        }

        private readonly List<Node> m_nodes = new List<Node>();
        private readonly List<Socket> m_extraSockets = new List<Socket>();
        private long m_now = 1000000;

        [TearDown]
        public void CloseSockets()
        {
            foreach (Node node in m_nodes) node.Mesh.CloseNow();
            foreach (Socket socket in m_extraSockets) socket.Close();
            m_nodes.Clear();
            m_extraSockets.Clear();
        }

        private static byte[] PairKey(int a, int b)
        {
            int low = Math.Min(a, b), high = Math.Max(a, b);
            return Enumerable.Range(0, 16).Select(i => (byte)(low * 40 + high * 7 + i + 1)).ToArray();
        }

        private static Socket NewSocket()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return socket;
        }

        /// <summary>
        /// in_count players, net ids 0 up, owner net id 0, each with what a successful attempt hands
        /// over. Not started.
        /// </summary>
        private List<Node> Lobby(int in_count, int in_keepAliveRateSecs = 0)
        {
            var nodes = new List<Node>();
            for (int i = 0; i < in_count; ++i) nodes.Add(new Node { NetId = i, Socket = NewSocket() });
            foreach (Node node in nodes)
            {
                var keys = new Dictionary<int, byte[]>();
                var peers = new List<DirectPeer>();
                foreach (Node other in nodes)
                {
                    if (other == node) continue;
                    keys[other.NetId] = PairKey(node.NetId, other.NetId);
                    var peer = new DirectPeer(CX[other.NetId], other.NetId).Clear();
                    peer.Address = (IPEndPoint)other.Socket.LocalEndPoint;
                    peer.Reached = true;
                    peers.Add(peer);
                }
                node.Session = new DirectSession(CIPHER, node.NetId, keys);
                var connection = new DirectConnection(LOBBY, node.Socket, node.Session, CX[0], peers, in_keepAliveRateSecs);
                node.Connection = connection;
                Node self = node;
                node.Mesh = new DirectMeshTransport(connection, CX[node.NetId], node.Listener, null, call => self.Sent.Add(call));
            }
            m_nodes.AddRange(nodes);
            return nodes;
        }

        private void StartAll(IEnumerable<Node> in_nodes)
        {
            foreach (Node node in in_nodes) node.Mesh.Start(m_now);
        }

        /// Updates every node not paused, in_rounds times, moving the clock on in_stepMs each round.
        private void Pump(int in_rounds = 20, int in_stepMs = 10)
        {
            for (int i = 0; i < in_rounds; ++i)
            {
                Thread.Sleep(2);
                m_now += in_stepMs;
                foreach (Node node in m_nodes)
                {
                    if (!node.Paused) node.Mesh.Update(m_now);
                }
            }
        }

        private static byte[] Text(string in_text)
        {
            return Encoding.UTF8.GetBytes(in_text);
        }

        private static ulong Bit(int in_netId)
        {
            return 1UL << in_netId;
        }

        [Test]
        public void EachPlayer_RaisesConnectForItselfFirst_ThenForEachPeerAsItConnects()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            foreach (Node node in nodes)
            {
                List<Dictionary<string, object>> connects = node.Listener.Ops("CONNECT").ToList();
                Assert.That(connects.Count, Is.EqualTo(2), "net id " + node.NetId);
                Assert.That(connects[0]["cxId"], Is.EqualTo(CX[node.NetId]), "itself first, as the relay sends it");
                Assert.That(connects[0]["netId"], Is.EqualTo(node.NetId));
                Assert.That(connects[0]["ownerCxId"], Is.EqualTo(CX[0]));
                Assert.That(connects[1]["cxId"], Is.EqualTo(CX[1 - node.NetId]));
                Assert.That(connects[1]["netId"], Is.EqualTo(1 - node.NetId));
                Assert.That(connects[1]["ownerCxId"], Is.EqualTo(CX[0]));
                Assert.That(node.Mesh.RoundTripMs, Is.GreaterThanOrEqualTo(0), "the pings measured it");
            }
        }

        [Test]
        public void GameData_ReachesOnlyThePlayersAddressed_WithTheSendersNetId()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            nodes[0].Mesh.SendRelay(Text("to b"), Bit(1), false, false, 0, m_now);
            nodes[0].Mesh.SendRelay(Text("to all"), BrainCloudRelay.TO_ALL_PLAYERS & ~Bit(0), true, true, 1, m_now);
            Pump();

            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "to b", "to all" }));
            Assert.That(nodes[2].Listener.Texts(0), Is.EqualTo(new[] { "to all" }));
            Assert.That(nodes[0].Listener.Relay, Is.Empty, "not addressed to itself");

            // Its own bit set: it gets its own copy, as the relay echoes it
            nodes[2].Mesh.SendRelay(Text("echo"), Bit(2) | Bit(0), false, false, 0, m_now);
            Pump();
            Assert.That(nodes[2].Listener.Texts(2), Is.EqualTo(new[] { "echo" }));
            Assert.That(nodes[0].Listener.Texts(2), Is.EqualTo(new[] { "echo" }));
            Assert.That(nodes[1].Listener.Texts(2), Is.Empty);
        }

        [Test]
        public void ALostReliablePacket_IsResent_AndDeliveredOnce()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            int dropped = 0;
            nodes[0].Mesh.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.RELAY && dropped++ < 2;
            nodes[0].Mesh.SendRelay(Text("important"), Bit(1), true, false, 0, m_now);
            Pump(1);
            Assert.That(nodes[1].Listener.Relay, Is.Empty, "the first copy was lost");

            Pump(40, 50);
            Assert.That(dropped, Is.GreaterThanOrEqualTo(2));
            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "important" }));
        }

        [Test]
        public void ALostAck_MeansAResend_WhichIsNotDeliveredAgain()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            int acksDropped = 0;
            nodes[1].Mesh.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.ACK && acksDropped++ < 2;
            nodes[0].Mesh.SendRelay(Text("once"), Bit(1), true, false, 0, m_now);
            Pump(40, 50);

            Assert.That(acksDropped, Is.GreaterThanOrEqualTo(2), "resent until an ACK got through");
            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "once" }));
        }

        [Test]
        public void OrderedReliablePackets_AreDeliveredInOrder_WhenOneIsLost()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            bool droppedFirst = false;
            nodes[0].Mesh.DropForTests = (to, packet) =>
            {
                if (droppedFirst || packet[0] != DirectMeshTransport.RELAY) return false;
                droppedFirst = true;
                return true;
            };
            for (int i = 0; i < 5; ++i) nodes[0].Mesh.SendRelay(Text("m" + i), Bit(1), true, true, 2, m_now);
            Pump(1);
            Assert.That(nodes[1].Listener.Relay, Is.Empty, "held until the first arrives");
            Pump(40, 50);
            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "m0", "m1", "m2", "m3", "m4" }));
        }

        [Test]
        public void ReliablePackets_ArePerPeer_SoOneLosingThemDoesNotHoldUpAnother()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            // Nothing gets through to C, but B is acknowledged
            nodes[0].Mesh.DropForTests = (to, packet) => to == 2 && packet[0] == DirectMeshTransport.RELAY;
            nodes[0].Mesh.SendRelay(Text("both"), Bit(1) | Bit(2), true, false, 0, m_now);
            Pump(5, 50);
            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "both" }));
            Assert.That(nodes[2].Listener.Relay, Is.Empty);

            // C is given up on after the reliable timeout, alone: B stays
            Pump(25, 500);
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Select(m => m["cxId"]), Is.EqualTo(new[] { CX[2] }));
            nodes[0].Mesh.SendRelay(Text("after"), Bit(1), true, false, 0, m_now);
            Pump();
            Assert.That(nodes[1].Listener.Texts(0), Is.EqualTo(new[] { "both", "after" }));
        }

        [Test]
        public void APeerNotHeardFromFor10Seconds_IsDisconnected()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            nodes[1].Paused = true;
            Pump(9, 1000);
            Assert.That(nodes[0].Listener.Ops("DISCONNECT"), Is.Empty);
            Pump(3, 1000);
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Single()["cxId"], Is.EqualTo(CX[1]));
            // As with a relay server when a member's connection drops: the others stay
            Assert.That(nodes[0].Listener.Errors, Is.Empty);
        }

        [Test]
        public void OnePeerOfTwoGoingQuiet_IsADisconnect_NotAFailure()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            nodes[2].Paused = true;
            Pump(12, 1000);
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Single()["cxId"], Is.EqualTo(CX[2]));
            Assert.That(nodes[0].Listener.Errors, Is.Empty, "B is still here");
        }

        [Test]
        public void WhenTheLastPeerWentQuiet_TheLobbyIsNeverStopped_ThisPlayerMayBeTheOneCutOff()
        {
            foreach (bool endMatch in new[] { false, true })
            {
                CloseSockets();
                List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 2);
                StartAll(nodes);
                Pump();
                nodes[1].Paused = true;
                Pump(12, 1000);
                Assert.That(nodes[0].Listener.Ops("DISCONNECT").Count(), Is.EqualTo(1));
                if (endMatch) Assert.That(nodes[0].Mesh.EndMatch(null, m_now), Is.Null);
                nodes[0].Mesh.Close(m_now);
                Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped), Is.Empty, endMatch ? "on EndMatch" : "on leaving");
            }
        }

        [Test]
        public void APeerThatWentQuietLongAgo_DoesNotStopTheOwnerLastToLeaveFromStoppingTheLobby()
        {
            List<Node> nodes = Lobby(3, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            // C goes quiet; much later B leaves properly
            nodes[2].Paused = true;
            Pump(12, 1000);
            Pump(30, 1000);
            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            nodes[1].Paused = true;
            Pump();
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Count(), Is.EqualTo(2));
            Assert.That(nodes[0].Listener.Errors, Is.Empty);
            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1));
        }

        [Test]
        public void DisconnectIsSentThreeTimes_SoOneLostCopyIsNotTakenForGoingQuiet()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();
            int lost = 0;
            nodes[1].Mesh.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.DISCONNECT && lost++ < DirectMeshTransport.DISCONNECT_COPIES - 1;
            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            nodes[1].Paused = true;
            Pump();
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Count(), Is.EqualTo(1), "the last copy got through");
        }

        [Test]
        public void EndMatchFromTheNewOwner_ReachesAPlayerThatHasNotLostTheOldOneYet()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();
            // A, the owner, leaves; its DISCONNECT reaches B but not C
            nodes[0].Mesh.DropForTests = (to, packet) => to == 2 && packet[0] == DirectMeshTransport.DISCONNECT;
            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            nodes[0].Paused = true;
            Pump();
            Assert.That(nodes[1].Mesh.OwnerCxId, Is.EqualTo(CX[1]));
            Assert.That(nodes[2].Mesh.OwnerCxId, Is.EqualTo(CX[0]), "C has not lost A yet");

            // B, owner in its own view, ends the match at once
            Assert.That(nodes[1].Mesh.EndMatch(new Dictionary<string, object> { { "winner", "b" } }, m_now), Is.Null);
            Pump();
            Assert.That(nodes[2].Listener.Ops("END_MATCH"), Is.Empty, "held while A is still the owner here");

            // C loses A, makes B owner too, and the match ends there as well
            Pump(12, 1000);
            Assert.That(nodes[2].Listener.Ops("MIGRATE_OWNER").Single()["cxId"], Is.EqualTo(CX[1]));
            Assert.That(((Dictionary<string, object>)nodes[2].Listener.Ops("END_MATCH").Single()["data"])["winner"], Is.EqualTo("b"));
        }

        [Test]
        public void EndMatchFromAMemberThatWouldNotBeOwner_IsNeverHeld()
        {
            List<Node> nodes = Lobby(4);
            StartAll(nodes);
            Pump();
            Node a = nodes[0], b = nodes[1], c = nodes[2], d = nodes[3];

            // At D, C ends the match while A owns it and B is A's successor: not held
            byte[] endMatch = { DirectMeshTransport.END_MATCH, 0, 60, (byte)'{', (byte)'}' };
            c.Socket.SendTo(c.Session.SenderFor(3).Seal(endMatch, 0, endMatch.Length), d.Socket.LocalEndPoint);
            Pump();

            // B leaves, then A goes quiet: D makes C owner, and must not end the match on C's old END_MATCH
            b.Mesh.SendDisconnect();
            b.Mesh.Close(m_now);
            b.Paused = true;
            Pump();
            a.Paused = true;
            Pump(12, 1000);
            Assert.That(d.Mesh.OwnerCxId, Is.EqualTo(CX[2]));
            Assert.That(d.Listener.Ops("END_MATCH"), Is.Empty);
        }

        [Test]
        public void AHeldEndMatch_IsDropped_WhenItsSenderIsLostBeforeTheOwner()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();
            Node a = nodes[0], b = nodes[1], c = nodes[2];

            // B, A's successor, ends the match towards C (held there), then goes quiet
            byte[] endMatch = { DirectMeshTransport.END_MATCH, 0, 61, (byte)'{', (byte)'}' };
            b.Socket.SendTo(b.Session.SenderFor(2).Seal(endMatch, 0, endMatch.Length), c.Socket.LocalEndPoint);
            Pump();
            b.Paused = true;
            Pump(12, 1000);
            Assert.That(c.Listener.Ops("DISCONNECT").Single()["cxId"], Is.EqualTo(CX[1]));

            // Then A leaves: C becomes owner, and B's END_MATCH is not applied
            a.Mesh.SendDisconnect();
            a.Mesh.Close(m_now);
            a.Paused = true;
            Pump();
            Assert.That(c.Mesh.OwnerCxId, Is.EqualTo(CX[2]));
            Assert.That(c.Listener.Ops("END_MATCH"), Is.Empty);
        }

        [Test]
        public void TheOwner_LastToLeave_TellsBrainCloudTheMatchStopped()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            nodes[1].Paused = true;
            Pump();
            Assert.That(nodes[0].Listener.Errors, Is.Empty, "B left: A stays, as with a relay server");
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped), Is.Empty, "not while A is still playing");
            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Single().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped), Is.Empty, "B left while A was still playing");
        }

        [Test]
        public void AMemberCannotMakeItselfOwner()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            // C announces itself owner while A, the owner, is still here, then ends the match
            byte[] owner = { DirectMeshTransport.OWNER, 0, 50, 2 };
            byte[] endMatch = { DirectMeshTransport.END_MATCH, 0, 51, (byte)'{', (byte)'}' };
            foreach (Node target in new[] { nodes[0], nodes[1] })
            {
                foreach (byte[] packet in new[] { owner, endMatch })
                {
                    nodes[2].Socket.SendTo(nodes[2].Session.SenderFor(target.NetId).Seal(packet, 0, packet.Length), target.Socket.LocalEndPoint);
                }
            }
            Pump();
            foreach (Node node in new[] { nodes[0], nodes[1] })
            {
                Assert.That(node.Listener.Ops("MIGRATE_OWNER"), Is.Empty, "net id " + node.NetId);
                Assert.That(node.Listener.Ops("END_MATCH"), Is.Empty, "net id " + node.NetId);
                Assert.That(node.Mesh.OwnerCxId, Is.EqualTo(CX[0]));
            }
        }

        [Test]
        public void SystemMessages_CarryTheProfileIds_AsTheRelayAddsThem()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();
            var connects = nodes[0].Listener.Ops("CONNECT").ToList();
            Assert.That(connects[0]["profileId"], Is.EqualTo("a"));
            Assert.That(connects[1]["profileId"], Is.EqualTo("b"));
            Assert.That(connects[1]["ownerId"], Is.EqualTo("a"));
        }

        [Test]
        public void APeerThatNeverConnects_IsGivenUp_WithoutADisconnect()
        {
            List<Node> nodes = Lobby(2);
            nodes[0].Mesh.Start(m_now);
            nodes[1].Paused = true;
            Pump(DirectMeshTransport.CONNECT_TIMEOUT_MS / 1000 + 5, 1000);
            Assert.That(nodes[0].Listener.Ops("CONNECT").Count(), Is.EqualTo(1), "only itself");
            Assert.That(nodes[0].Listener.Ops("DISCONNECT"), Is.Empty, "never connected, so never disconnected");
            // Nothing is sent to it any more, not even pings
            byte[] buffer = new byte[2048];
            while (nodes[1].Socket.Available > 0) nodes[1].Socket.Receive(buffer);
            nodes[0].Mesh.SendRelay(Text("x"), Bit(1), true, false, 0, m_now);
            Pump(3, 1000);
            Assert.That(nodes[1].Socket.Available, Is.EqualTo(0));
        }

        [Test]
        public void APlayerLeaving_IsADisconnectForTheOthers()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            Pump();
            Assert.That(nodes[0].Listener.Ops("DISCONNECT").Single()["cxId"], Is.EqualTo(CX[1]));
            Assert.That(nodes[0].Listener.Errors, Is.Empty, "B left: A stays connected, as with a relay server");
        }

        [Test]
        public void TheOwnerEndingTheMatch_ReachesEveryone_AndIsDeliveredBeforeItsSocketCloses()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            // Only the owner may, as with the relay
            Assert.That(nodes[1].Mesh.EndMatch(new Dictionary<string, object> { { "x", 1 } }, m_now), Does.Contain("owner"));

            int endsDropped = 0;
            nodes[0].Mesh.DropForTests = (to, packet) => to == 2 && packet[0] == DirectMeshTransport.END_MATCH && endsDropped++ < 2;
            Assert.That(nodes[0].Mesh.EndMatch(new Dictionary<string, object> { { "winner", "b" } }, m_now), Is.Null);
            // The owner gets END_MATCH too, as the relay sends it back
            Assert.That(((Dictionary<string, object>)nodes[0].Listener.Ops("END_MATCH").Single()["data"])["winner"], Is.EqualTo("b"));
            // RelayComms closes the connection on END_MATCH: it lingers to deliver it
            Assert.That(nodes[0].Mesh.Close(m_now), Is.True);

            for (int i = 0; i < 40 && !nodes[0].Mesh.IsClosed; ++i)
            {
                Thread.Sleep(2);
                m_now += 50;
                nodes[0].Mesh.UpdateClosing(m_now);
                nodes[1].Mesh.Update(m_now);
                nodes[2].Mesh.Update(m_now);
            }
            Assert.That(nodes[0].Mesh.IsClosed, Is.True, "closed once both acknowledged");
            Assert.That(endsDropped, Is.EqualTo(3), "two copies to C lost, the third delivered");
            foreach (Node node in new[] { nodes[1], nodes[2] })
            {
                var endMatch = node.Listener.Ops("END_MATCH").Single();
                Assert.That(((Dictionary<string, object>)endMatch["data"])["winner"], Is.EqualTo("b"), "net id " + node.NetId);
            }
        }

        [Test]
        public void EndMatch_GivesUpAfter5Seconds_WhenAPeerNeverAcknowledges()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();
            nodes[0].Mesh.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.END_MATCH;
            nodes[0].Mesh.EndMatch(null, m_now);
            Assert.That(nodes[0].Mesh.Close(m_now), Is.True);
            m_now += 4000;
            Assert.That(nodes[0].Mesh.UpdateClosing(m_now), Is.True);
            m_now += 1500;
            Assert.That(nodes[0].Mesh.UpdateClosing(m_now), Is.False);
            Assert.That(nodes[0].Mesh.IsClosed, Is.True);
        }

        [Test]
        public void EndMatchFromAPeerThatIsNotTheOwner_IsIgnored()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            // A forged END_MATCH from B, sealed with B's own key: genuine, but B is not the owner
            byte[] packet = { DirectMeshTransport.END_MATCH, 0, 9, (byte)'{', (byte)'}' };
            byte[] datagram = nodes[1].Session.SenderFor(0).Seal(packet, 0, packet.Length);
            nodes[1].Socket.SendTo(datagram, nodes[0].Socket.LocalEndPoint);
            Pump();
            Assert.That(nodes[0].Listener.Ops("END_MATCH"), Is.Empty);
        }

        [Test]
        public void WhenTheOwnerLeaves_TheLowestNetIdLeftTakesOver_AndEveryoneAgrees()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            nodes[0].Paused = true;
            Pump();

            foreach (Node node in new[] { nodes[1], nodes[2] })
            {
                Assert.That(node.Listener.Ops("DISCONNECT").Single()["cxId"], Is.EqualTo(CX[0]), "net id " + node.NetId);
                Assert.That(node.Listener.Ops("MIGRATE_OWNER").Single()["cxId"], Is.EqualTo(CX[1]), "net id " + node.NetId);
                Assert.That(node.Mesh.OwnerCxId, Is.EqualTo(CX[1]));
            }
            // The new owner may end the match
            Assert.That(nodes[1].Mesh.EndMatch(null, m_now), Is.Null);
            Pump();
            Assert.That(nodes[2].Listener.Ops("END_MATCH").Count(), Is.EqualTo(1));
        }

        [Test]
        public void ALobbyThatOutlivesItsMatch_IsKeptAliveByTheOwner_AndToldWhenTheMatchStops()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 2);
            StartAll(nodes);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomKeepAlive).Count(), Is.EqualTo(1), "at once");
            Pump(10, 210);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomKeepAlive).Count(), Is.EqualTo(2), "then every 2 seconds");
            Assert.That(nodes[0].Calls(ServiceOperation.RoomKeepAlive).First().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
            Assert.That(nodes[1].Calls(ServiceOperation.RoomKeepAlive), Is.Empty, "the owner's job alone");

            nodes[0].Mesh.EndMatch(null, m_now);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Single().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
        }

        [Test]
        public void ALobbyThatEndsWithItsMatch_SendsNeitherKeepAliveNorStopped()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump(10, 500);
            nodes[0].Mesh.EndMatch(null, m_now);
            Assert.That(nodes[0].Sent, Is.Empty);
        }

        [Test]
        public void APacketClaimingAnotherSender_IsDropped()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            // B, with its own key to A, claims in the header that the data is C's
            byte[] packet = { DirectMeshTransport.RELAY, 0, 0, 0x80, 0, 0, 0, 0, 2, (byte)'x' };
            byte[] datagram = nodes[1].Session.SenderFor(0).Seal(packet, 0, packet.Length);
            nodes[1].Socket.SendTo(datagram, nodes[0].Socket.LocalEndPoint);
            Pump();
            Assert.That(nodes[0].Listener.Relay, Is.Empty);

            // The same naming B itself is delivered, so it was the claim that was dropped
            packet[8] = 1;
            datagram = nodes[1].Session.SenderFor(0).Seal(packet, 0, packet.Length);
            nodes[1].Socket.SendTo(datagram, nodes[0].Socket.LocalEndPoint);
            Pump();
            Assert.That(nodes[0].Listener.Texts(1), Is.EqualTo(new[] { "x" }));
        }

        [Test]
        public void ADatagramThatDoesNotOpen_ChangesNothing()
        {
            List<Node> nodes = Lobby(2);
            nodes[0].Mesh.Start(m_now);

            // Sealed with a key B does not share with A
            var stranger = new DirectSession(CIPHER, 1, new Dictionary<int, byte[]> { { 0, Enumerable.Repeat((byte)7, 16).ToArray() } });
            byte[] ping = { DirectMeshTransport.PING, 0, 0, 0, 1 };
            Socket elsewhere = NewSocket();
            m_extraSockets.Add(elsewhere);
            elsewhere.SendTo(stranger.SenderFor(0).Seal(ping, 0, ping.Length), nodes[0].Socket.LocalEndPoint);
            nodes[1].Paused = true;
            Pump();
            Assert.That(nodes[0].Listener.Ops("CONNECT").Count(), Is.EqualTo(1), "B did not connect");
            Assert.That(elsewhere.Available, Is.EqualTo(0), "nothing was answered");
        }

        [Test]
        public void TrafficFollowsThePeer_ToTheAddressItsLatestGenuineDatagramCameFrom()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();

            // B's router gives it a new mapping: its datagrams now come from another address
            Socket moved = NewSocket();
            m_extraSockets.Add(moved);
            byte[] ping = { DirectMeshTransport.PING, 0, 0, 0, 1 };
            moved.SendTo(nodes[1].Session.SenderFor(0).Seal(ping, 0, ping.Length), nodes[0].Socket.LocalEndPoint);
            nodes[1].Paused = true;
            Pump(5);

            Assert.That(moved.Available, Is.GreaterThan(0), "A answered at the new address");
            byte[] buffer = new byte[2048];
            int length = moved.Receive(buffer);
            byte[] pong = nodes[1].Session.ReceiverFor(0).Open(buffer, length);
            Assert.That(pong, Is.Not.Null);
            Assert.That(pong[0], Is.EqualTo(DirectMeshTransport.PONG));
        }

        [Test]
        public void APunchFromAPeerStillInItsAttempt_IsAnswered_ButDoesNotConnectIt()
        {
            List<Node> nodes = Lobby(2);
            nodes[0].Mesh.Start(m_now);
            nodes[1].Paused = true;

            byte[] punch = { DirectConnector.PUNCH, 0, 0, 0, 42 };
            nodes[1].Socket.SendTo(nodes[1].Session.SenderFor(0).Seal(punch, 0, punch.Length), nodes[0].Socket.LocalEndPoint);
            Pump(3);
            Assert.That(nodes[0].Listener.Ops("CONNECT").Count(), Is.EqualTo(1), "B has not taken the connection over");

            // B's socket got A's first ping and the PUNCH_ACK
            var types = new List<byte>();
            byte[] buffer = new byte[2048];
            while (nodes[1].Socket.Available > 0)
            {
                int length = nodes[1].Socket.Receive(buffer);
                byte[] opened = nodes[1].Session.ReceiverFor(0).Open(buffer, length);
                if (opened != null) types.Add(opened[0]);
            }
            Assert.That(types, Does.Contain(DirectConnector.PUNCH_ACK));
        }

        [Test]
        public void EndMatchThenDisconnect_InOneFrame_StillEndsTheMatchForEveryone()
        {
            List<Node> nodes = Lobby(3);
            StartAll(nodes);
            Pump();

            // The first END_MATCH copies are lost; the game leaves at once, as games do
            int lost = 0;
            nodes[0].Mesh.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.END_MATCH && lost++ < 2;
            nodes[0].Mesh.EndMatch(new Dictionary<string, object> { { "winner", "a" } }, m_now);
            nodes[0].Mesh.SendDisconnect();
            Assert.That(nodes[0].Mesh.Close(m_now), Is.True);
            for (int i = 0; i < 40 && !nodes[0].Mesh.IsClosed; ++i)
            {
                Thread.Sleep(2);
                m_now += 50;
                nodes[0].Mesh.UpdateClosing(m_now);
                nodes[1].Mesh.Update(m_now);
                nodes[2].Mesh.Update(m_now);
            }
            foreach (Node node in new[] { nodes[1], nodes[2] })
            {
                Assert.That(node.Listener.Ops("END_MATCH").Count(), Is.EqualTo(1), "net id " + node.NetId);
                Assert.That(node.Listener.Ops("DISCONNECT"), Is.Empty, "no DISCONNECT after END_MATCH, as with the relay");
                Assert.That(node.Listener.Ops("MIGRATE_OWNER"), Is.Empty);
            }
        }

        [Test]
        public void WhenBrainCloudsLobbyOwnerIsNotThePlayersOwner_BothTellBrainCloudTheMatchStopped()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 2);
            // A left the lobby and came back, say: brainCloud's owner is now B, while A still owns the match
            foreach (Node node in nodes) node.Connection.LobbyOwnerCxId = CX[1];
            StartAll(nodes);
            Pump();
            Assert.That(nodes[0].Calls(ServiceOperation.RoomKeepAlive).Count(), Is.GreaterThan(0), "the players' owner");
            Assert.That(nodes[1].Calls(ServiceOperation.RoomKeepAlive).Count(), Is.GreaterThan(0), "brainCloud's owner");

            // A, ending the match, tells brainCloud; B, its lobby owner, does too, in case brainCloud
            // no longer counts A a member. The first one wins, and the repeat changes nothing.
            nodes[0].Mesh.EndMatch(null, m_now);
            Pump();
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Single().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped).Single().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
        }

        [Test]
        public void TheLastToLeave_StopsTheLobby_WhetherOrNotItIsAnOwner()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            // A, both owners, leaves the match and stays in the lobby; B plays on, then leaves
            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            nodes[0].Paused = true;
            Pump();
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped), Is.Empty, "B is still playing");
            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped).Single().GetJsonData()["lobbyId"], Is.EqualTo(LOBBY));
        }

        /// Runs the closing connections of in_nodes until they finish, with the others updating.
        private void PumpClosing(List<Node> in_nodes, int in_rounds = 40)
        {
            for (int i = 0; i < in_rounds; ++i)
            {
                Thread.Sleep(2);
                m_now += 50;
                foreach (Node node in in_nodes)
                {
                    if (!node.Mesh.IsClosed) node.Mesh.UpdateClosing(m_now);
                }
            }
        }

        [Test]
        public void PlayersLeavingTogether_TellBrainCloudTheMatchStopped_Once()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            // Both leave in the same moment: neither has heard the other leave
            foreach (Node node in nodes) node.Mesh.SendDisconnect();
            foreach (Node node in nodes) Assert.That(node.Mesh.Close(m_now), Is.True, "listening for the others leaving");
            PumpClosing(nodes);
            Assert.That(nodes.All(n => n.Mesh.IsClosed), Is.True);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1), "the lowest net id among them");
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped), Is.Empty, "brainCloud gets it once");
        }

        [Test]
        public void TheLastToLeave_StopsTheLobbyInTheSameCallAsDisconnect()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            nodes[1].Mesh.SendDisconnect();
            nodes[1].Mesh.Close(m_now);
            nodes[1].Paused = true;
            Pump();
            // Sent before Close, so it reaches brainCloud ahead of a LeaveLobby the game makes next
            nodes[0].Mesh.SendDisconnect();
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1));
            nodes[0].Mesh.Close(m_now);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1), "once");
        }

        [Test]
        public void PlayersLeavingTogether_HearingEachOtherBeforeClosing_StillStopTheLobbyOnce()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            // Both leave; each hears the other in the Update that runs before its Close, as through RelayComms
            foreach (Node node in nodes) node.Mesh.SendDisconnect();
            Thread.Sleep(20);
            m_now += 10;
            foreach (Node node in nodes) node.Mesh.Update(m_now);
            Assert.That(nodes.All(n => n.Listener.Ops("DISCONNECT").Count() == 1), Is.True, "each heard the other leave");
            foreach (Node node in nodes) node.Mesh.Close(m_now);
            PumpClosing(nodes);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1));
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped), Is.Empty, "brainCloud gets it once");
        }

        [Test]
        public void APlayerLeavingAlone_DoesNotStopTheLobby_WhileItListensForOthersLeaving()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            nodes[0].Mesh.SendDisconnect();
            Assert.That(nodes[0].Mesh.Close(m_now), Is.True);
            PumpClosing(new List<Node> { nodes[0] });
            Assert.That(nodes[0].Mesh.IsClosed, Is.True, "stops listening after a while");
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped), Is.Empty, "B is still playing");
        }

        [Test]
        public void DisconnectThenEndMatch_InOneFrame_EndsNothing()
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            nodes[0].Mesh.SendDisconnect();
            Assert.That(nodes[0].Mesh.EndMatch(null, m_now), Does.Contain("closing"));
            Assert.That(nodes[0].Listener.Ops("END_MATCH"), Is.Empty);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomStopped), Is.Empty, "B plays on, as with the relay");
        }

        [Test]
        public void EndMatchTwice_EndsTheMatchOnce()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();
            Assert.That(nodes[0].Mesh.EndMatch(null, m_now), Is.Null);
            Assert.That(nodes[0].Mesh.EndMatch(null, m_now), Is.Null);
            Pump();
            Assert.That(nodes[0].Listener.Ops("END_MATCH").Count(), Is.EqualTo(1));
            Assert.That(nodes[1].Listener.Ops("END_MATCH").Count(), Is.EqualTo(1));
        }

        [Test]
        public void APlayerCutOffFromTheOwnerAlone_DoesNotStopTheLobbyOnTakingOver()
        {
            List<Node> nodes = Lobby(3, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            // The path between A (owner) and B fails; C hears both
            nodes[0].Mesh.DropForTests = (to, packet) => to == 1;
            nodes[1].Mesh.DropForTests = (to, packet) => to == 0;
            Pump(12, 1000);
            Assert.That(nodes[1].Mesh.OwnerCxId, Is.EqualTo(CX[1]), "B took over from the owner it lost");
            Assert.That(nodes[1].Mesh.EndMatch(null, m_now), Is.Null);
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped), Is.Empty, "A and C may still be playing");
        }

        [Test]
        public void TakingOverFromAnOwnerThatLeft_StopsTheLobbyOnEndMatch()
        {
            List<Node> nodes = Lobby(3, in_keepAliveRateSecs: 30);
            StartAll(nodes);
            Pump();
            nodes[0].Mesh.SendDisconnect();
            nodes[0].Mesh.Close(m_now);
            nodes[0].Paused = true;
            Pump();
            Assert.That(nodes[1].Mesh.OwnerCxId, Is.EqualTo(CX[1]));
            Assert.That(nodes[1].Mesh.EndMatch(null, m_now), Is.Null);
            Assert.That(nodes[1].Calls(ServiceOperation.RoomStopped).Count(), Is.EqualTo(1));
        }

        // The match is over (another member stopped it), the lobby is gone, or this player has left it
        [TestCase(ReasonCodes.INVALID_REQUEST)]
        [TestCase(ReasonCodes.LOBBY_NOT_FOUND)]
        [TestCase(ReasonCodes.LOBBY_MEMBER_NOT_FOUND)]
        public void KeepAlivesStop_OnceBrainCloudSaysTheMatchIsOver(int in_reasonCode)
        {
            List<Node> nodes = Lobby(2, in_keepAliveRateSecs: 2);
            StartAll(nodes);
            ServerCall first = nodes[0].Calls(ServiceOperation.RoomKeepAlive).Single();
            first.GetCallback().OnErrorCallback(400, in_reasonCode, "refused");
            Pump(10, 500);
            Assert.That(nodes[0].Calls(ServiceOperation.RoomKeepAlive).Count(), Is.EqualTo(1), "no more, each one billed");
        }

        [Test]
        public void AnEndMatchPayloadTooBigForOneDatagram_IsRefused()
        {
            List<Node> nodes = Lobby(2);
            StartAll(nodes);
            Pump();
            var payload = new Dictionary<string, object> { { "big", new string('x', DirectMeshTransport.MAX_PACKET_LENGTH) } };
            Assert.That(nodes[0].Mesh.EndMatch(payload, m_now), Does.Contain("too big"));
            Assert.That(nodes[0].Listener.Ops("END_MATCH"), Is.Empty);
        }
    }
}
