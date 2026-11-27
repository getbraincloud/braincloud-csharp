// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BrainCloudTests
{
    /// <summary>
    /// The relay's reliability layer and transports, with no brainCloud server: the channel is
    /// driven directly, and the transports talk to sockets opened here.
    /// </summary>
    [TestFixture]
    public class TestRelayInternals
    {
        #region reliable channel, incoming

        /// A RELAY packet as the relay server sends it: length prefix, control byte, 8-byte header, payload.
        private static byte[] Packet(bool reliable, bool ordered, int channel, int packetId, int netId, string payload = "x")
        {
            byte[] body = System.Text.Encoding.ASCII.GetBytes(payload);
            byte[] data = new byte[11 + body.Length];
            data[0] = (byte)(data.Length >> 8);
            data[1] = (byte)data.Length;
            data[2] = RelayComms.RS2CL_RELAY;
            int rh = (reliable ? RelayComms.RELIABLE_BIT : 0) | (ordered ? RelayComms.ORDERED_BIT : 0) | ((channel & 0x3) << 12) | (packetId & 0xFFF);
            data[3] = (byte)(rh >> 8);
            data[4] = (byte)rh;
            data[10] = (byte)netId; // the low byte of the player mask is the sender
            Buffer.BlockCopy(body, 0, data, 11, body.Length);
            return data;
        }

        private static int IdOf(byte[] data)
        {
            return ((data[3] << 8) | data[4]) & 0xFFF;
        }

        private RelayReliableChannel _channel;
        private List<int> _delivered;

        [SetUp]
        public void CreateChannel()
        {
            _channel = new RelayReliableChannel(null, null);
            _delivered = new List<int>();
        }

        private void Receive(byte[] data)
        {
            Assert.That(_channel.Receive(data, (netId, packet) => _delivered.Add(IdOf(packet))),
                Is.EqualTo(RelayReliableChannel.ReceiveOutcome.Accepted));
        }

        [Test]
        public void OrderedReliable_IsHeldUntilTheEarlierPacketsArrive()
        {
            Receive(Packet(true, true, 0, 2, 1));
            Receive(Packet(true, true, 0, 1, 1));
            Assert.That(_delivered, Is.Empty, "nothing before packet 0");

            Receive(Packet(true, true, 0, 0, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void OrderedReliable_DuplicateIsDropped()
        {
            Receive(Packet(true, true, 0, 0, 1));
            Receive(Packet(true, true, 0, 0, 1));
            Receive(Packet(true, true, 0, 2, 1));
            Receive(Packet(true, true, 0, 2, 1)); // held, and held again
            Receive(Packet(true, true, 0, 1, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void OrderedReliable_EachSenderAndChannelIsItsOwnStream()
        {
            Receive(Packet(true, true, 0, 0, 1));
            Receive(Packet(true, true, 0, 0, 2));
            Receive(Packet(true, true, 1, 0, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 0, 0, 0 }));
        }

        [Test]
        public void OrderedReliable_TooManyWaitingDropsTheLink()
        {
            // Packet 0 never comes
            RelayReliableChannel.ReceiveOutcome outcome = RelayReliableChannel.ReceiveOutcome.Accepted;
            for (int id = 1; id < 1000 && outcome == RelayReliableChannel.ReceiveOutcome.Accepted; ++id)
            {
                outcome = _channel.Receive(Packet(true, true, 0, id, 1), (netId, packet) => _delivered.Add(IdOf(packet)));
            }
            Assert.That(outcome, Is.EqualTo(RelayReliableChannel.ReceiveOutcome.TooManyOutOfOrder));
            Assert.That(_delivered, Is.Empty);
        }

        [Test]
        public void OrderedUnreliable_AnOlderPacketIsDropped()
        {
            Receive(Packet(false, true, 0, 0, 1));
            Receive(Packet(false, true, 0, 3, 1));
            Receive(Packet(false, true, 0, 2, 1));
            Receive(Packet(false, true, 0, 3, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 0, 3 }));
        }

        [Test]
        public void UnorderedReliable_AResendIsDeliveredOnce_BetweenPlayersDirectly()
        {
            _channel.PeerToPeer = true;
            // A resend whose first ACK was lost: the same header twice. Another sender, channel or
            // packet id is another packet.
            Receive(Packet(true, false, 0, 5, 1));
            Receive(Packet(true, false, 0, 5, 1));
            Receive(Packet(true, false, 0, 5, 2));
            Receive(Packet(true, false, 1, 5, 1));
            Receive(Packet(true, false, 0, 6, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 5, 5, 5, 6 }));
        }

        [Test]
        public void UnorderedReliable_IdsAreForgottenLongBeforeAStreamWrapsRound()
        {
            _channel.PeerToPeer = true;
            // A stream's ids wrap after 4096 packets; by then its earlier ones are forgotten, so the
            // wrapped ids are delivered
            for (int i = 0; i <= RelayReliableChannel.MAX_PACKET_ID + 1; ++i) Receive(Packet(true, false, 0, i & RelayReliableChannel.MAX_PACKET_ID, 1));
            Assert.That(_delivered.Count, Is.EqualTo(RelayReliableChannel.MAX_PACKET_ID + 2));
        }

        [Test]
        public void UnorderedReliable_ThroughARelayServer_IsLeftAsItWas()
        {
            // The relay drops repeats itself; a member rejoining with a recycled net id restarts its ids
            Receive(Packet(true, false, 0, 5, 1));
            Receive(Packet(true, false, 0, 5, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 5, 5 }));
        }

        [Test]
        public void BetweenPlayersDirectly_APeerCannotMakeThisPlayerHoldWithoutLimit()
        {
            // Ordered reliable packets, each in a stream of its own (another player mask): every
            // stream is remembered, so there can only be so many
            _channel.PeerToPeer = true;
            RelayReliableChannel.ReceiveOutcome outcome = RelayReliableChannel.ReceiveOutcome.Accepted;
            int sent = 0;
            for (int mask = 0; mask < 5000 && outcome == RelayReliableChannel.ReceiveOutcome.Accepted; ++mask, ++sent)
            {
                byte[] packet = Packet(true, true, 0, 0, 1);
                packet[7] = (byte)(mask >> 8);
                packet[8] = (byte)mask;
                outcome = _channel.Receive(packet, (netId, data) => _delivered.Add(IdOf(data)));
            }
            Assert.That(outcome, Is.EqualTo(RelayReliableChannel.ReceiveOutcome.TooManyOutOfOrder));
            Assert.That(sent, Is.GreaterThan(4000).And.LessThan(5000));

            // A peer sending in order to many player masks (teams, players in range) is legitimate:
            // each stream is counted once, and there is room for thousands
            var legitimate = new RelayReliableChannel(null, null) { PeerToPeer = true };
            for (int mask = 0; mask < 3000; ++mask)
            {
                byte[] packet = Packet(true, true, 0, 0, 1);
                packet[7] = (byte)(mask >> 8);
                packet[8] = (byte)mask;
                Assert.That(legitimate.Receive(packet, (netId, data) => { }), Is.EqualTo(RelayReliableChannel.ReceiveOutcome.Accepted), "mask " + mask);
            }

            // A few streams, each below its own limit, but holding too many between them
            var held = new RelayReliableChannel(null, null) { PeerToPeer = true };
            outcome = RelayReliableChannel.ReceiveOutcome.Accepted;
            sent = 0;
            for (int stream = 0; stream < 3 && outcome == RelayReliableChannel.ReceiveOutcome.Accepted; ++stream)
            {
                for (int id = 1; id <= 500 && outcome == RelayReliableChannel.ReceiveOutcome.Accepted; ++id, ++sent)
                {
                    byte[] packet = Packet(true, true, stream, id, 1);
                    outcome = held.Receive(packet, (netId, data) => { });
                }
            }
            Assert.That(outcome, Is.EqualTo(RelayReliableChannel.ReceiveOutcome.TooManyOutOfOrder));
            Assert.That(sent, Is.LessThan(1500));

            // Through a relay server the same packets are held as before
            var relayed = new RelayReliableChannel(null, null);
            for (int mask = 0; mask < 1000; ++mask)
            {
                byte[] packet = Packet(true, true, 0, 1, 1);
                packet[7] = (byte)(mask >> 8);
                packet[8] = (byte)mask;
                Assert.That(relayed.Receive(packet, (netId, data) => { }), Is.EqualTo(RelayReliableChannel.ReceiveOutcome.Accepted));
            }
        }

        [Test]
        public void MeshEvents_ReachTheGameInTheOrderTheyArrived_ConnectBeforeData()
        {
            var comms = new RelayComms(new BrainCloudClient());
            var seen = new List<string>();
            comms.RegisterSystemCallback(json => seen.Add("system"));
            comms.RegisterRelayCallback((netId, data) => seen.Add("relay " + netId));
            IDirectMeshListener mesh = comms;
            mesh.OnMeshSystem("{\"op\":\"CONNECT\",\"cxId\":\"1:b:cx\",\"ownerCxId\":\"1:a:cx\",\"netId\":1}");
            mesh.OnMeshRelay(1, Packet(false, false, 0, 0, 1));
            comms.Update();
            comms.Update();
            Assert.That(seen, Is.EqualTo(new[] { "system", "relay 1" }));
        }

        /// <summary>
        /// Two players' RelayComms connected directly, as a game drives them: Connect(DIRECT), then
        /// Send, EndMatch and Disconnect, with the mesh underneath. in_lost drops the owner's
        /// datagrams it returns true for.
        /// </summary>
        private sealed class MeshPair : IDisposable
        {
            public readonly RelayComms A, B;
            public readonly List<string> SystemA = new List<string>(), SystemB = new List<string>();
            public readonly List<string> DataB = new List<string>();
            public readonly List<ServerCall> CallsA = new List<ServerCall>(), CallsB = new List<ServerCall>();
            private readonly int m_keepAliveRateSecs;
            public int Takes;
            private readonly System.Net.Sockets.Socket m_a, m_b;

            public MeshPair(int in_keepAliveRateSecs = 0)
            {
                m_keepAliveRateSecs = in_keepAliveRateSecs;
                m_a = Bound();
                m_b = Bound();
                byte[] key = new byte[16];
                for (int i = 0; i < key.Length; ++i) key[i] = (byte)(i + 1);
                A = Comms("1:a:cx", 0, 1, "1:b:cx", m_a, m_b, key);
                B = Comms("1:b:cx", 1, 0, "1:a:cx", m_b, m_a, key);
                A.LobbyCallsForTests = call => CallsA.Add(call);
                B.LobbyCallsForTests = call => CallsB.Add(call);
                A.RegisterSystemCallback(json => SystemA.Add((string)((Dictionary<string, object>)JsonReader.Deserialize(json))["op"]));
                B.RegisterSystemCallback(json => SystemB.Add((string)((Dictionary<string, object>)JsonReader.Deserialize(json))["op"]));
                B.RegisterRelayCallback((netId, data) => DataB.Add(netId + ":" + System.Text.Encoding.UTF8.GetString(data)));
            }

            private static System.Net.Sockets.Socket Bound()
            {
                var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);
                socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0));
                return socket;
            }

            private RelayComms Comms(string in_cx, int in_netId, int in_peerNetId, string in_peerCx, System.Net.Sockets.Socket in_socket, System.Net.Sockets.Socket in_peerSocket, byte[] in_key)
            {
                var comms = new RelayComms(new BrainCloudClient()) { AssumeAuthenticatedForTests = true, CxIdForTests = in_cx };
                var peer = new DirectPeer(in_peerCx, in_peerNetId).Clear();
                peer.Address = (System.Net.IPEndPoint)in_peerSocket.LocalEndPoint;
                var session = new DirectSession(DirectPacket.SupportedCiphers[0], in_netId, new Dictionary<int, byte[]> { { in_peerNetId, in_key } });
                var connection = new DirectConnection("1:lobby:1", in_socket, session, "1:a:cx", new List<DirectPeer> { peer }, m_keepAliveRateSecs);
                bool taken = false;
                comms.TakeDirectConnectionForTests = lobbyId =>
                {
                    ++Takes;
                    if (taken || lobbyId != "1:lobby:1") return null;
                    taken = true;
                    return connection;
                };
                return comms;
            }

            public void Connect(RelayComms in_comms)
            {
                in_comms.Connect(RelayConnectionType.DIRECT, new RelayConnectOptions(false, null, 0, null, "1:lobby:1"));
            }

            public void Pump(int in_rounds)
            {
                for (int i = 0; i < in_rounds; ++i)
                {
                    A.Update();
                    B.Update();
                    Thread.Sleep(5);
                }
            }

            public void Dispose()
            {
                m_a.Close();
                m_b.Close();
            }
        }

        [Test]
        public void Direct_ThroughRelayComms_ConnectsSendsAndEndsTheMatch_EvenWhenTheGameLeavesAtOnce()
        {
            using (var pair = new MeshPair())
            {
                pair.Connect(pair.A);
                pair.Connect(pair.A);   // again, before connecting: the same connection carries on
                pair.Connect(pair.B);
                pair.Pump(20);
                Assert.That(pair.A.IsConnected() && pair.B.IsConnected(), Is.True);
                Assert.That(pair.A.ConnectionType, Is.EqualTo(RelayConnectionType.DIRECT));
                Assert.That(pair.A.MeshForTests, Is.Not.Null);
                Assert.That(pair.SystemB, Is.EqualTo(new[] { "CONNECT", "CONNECT" }));

                pair.A.Send(System.Text.Encoding.UTF8.GetBytes("hello"), 1UL << 1, true, true, 0);
                pair.Pump(10);
                Assert.That(pair.DataB, Is.EqualTo(new[] { "0:hello" }));

                // The owner ends the match and leaves in the same frame; its first END_MATCH is lost
                int lost = 0;
                pair.A.MeshForTests.DropForTests = (to, packet) => packet[0] == DirectMeshTransport.END_MATCH && lost++ < 1;
                pair.A.EndMatch(new Dictionary<string, object> { { "winner", "a" } });
                pair.A.Disconnect();
                pair.Pump(80);
                Assert.That(pair.SystemA.Last(), Is.EqualTo("END_MATCH"));
                Assert.That(pair.SystemB, Is.EqualTo(new[] { "CONNECT", "CONNECT", "END_MATCH" }), "no DISCONNECT or MIGRATE_OWNER");
                Assert.That(pair.A.IsConnected() || pair.B.IsConnected(), Is.False);
                Assert.That(pair.A.ConnectionType, Is.EqualTo(RelayConnectionType.INVALID));
            }
        }

        [Test]
        public void Direct_ThroughRelayComms_PlayersLeavingTogether_StopTheLobbyOnce()
        {
            using (var pair = new MeshPair(30))
            {
                pair.Connect(pair.A);
                pair.Connect(pair.B);
                pair.Pump(20);
                Assert.That(pair.A.IsConnected() && pair.B.IsConnected(), Is.True);

                // Both games leave in the same frame
                pair.A.Disconnect();
                pair.B.Disconnect();
                pair.Pump(80);
                Assert.That(pair.A.IsConnected() || pair.B.IsConnected(), Is.False);
                int stopsA = pair.CallsA.Count(c => c.GetOperation() == ServiceOperation.RoomStopped.Value);
                int stopsB = pair.CallsB.Count(c => c.GetOperation() == ServiceOperation.RoomStopped.Value);
                Assert.That(stopsA + stopsB, Is.EqualTo(1), "brainCloud gets it once");
                Assert.That(stopsA, Is.EqualTo(1), "from the lowest net id");
            }
        }

        [Test]
        public void UnreliableUnordered_IsAlwaysDelivered()
        {
            Receive(Packet(false, false, 0, 1, 1));
            Receive(Packet(false, false, 0, 1, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 1, 1 }));
        }

        [Test]
        public void TrackedPacketId_SetsWhereAnOrderedStreamStarts()
        {
            // We joined after this sender had sent packets 0 to 6 on channel 2
            _channel.SetTrackedPacketId(2, 1, 6);
            _channel.SetTrackedPacketId(2, 1, 6); // a repeated NET_ID message must not throw

            Receive(Packet(true, true, 2, 7, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 7 }));
        }

        [Test]
        public void TrackedPacketIds_AreForgottenOnReset()
        {
            _channel.SetTrackedPacketId(0, 1, 6);
            _channel.Reset();

            Receive(Packet(true, true, 0, 7, 1));
            Assert.That(_delivered, Is.Empty, "a new connection starts at packet 0");
        }

        [Test]
        public void PacketIds_CompareAcrossTheWrap()
        {
            Assert.That(RelayReliableChannel.packetLE(1, 2), Is.True);
            Assert.That(RelayReliableChannel.packetLE(2, 1), Is.False);
            Assert.That(RelayReliableChannel.packetLE(RelayReliableChannel.MAX_PACKET_ID, 0), Is.True, "4095 comes before 0");
            Assert.That(RelayReliableChannel.packetLE(0, RelayReliableChannel.MAX_PACKET_ID), Is.False);
        }

        #endregion

        #region reliable channel, outgoing

        [Test]
        public void NextPacketId_CountsUpPerStreamAndWraps()
        {
            ulong a = 0x8000000000000100UL, b = 0x8000000000000200UL;
            Assert.That(_channel.NextPacketId(a), Is.EqualTo(0));
            Assert.That(_channel.NextPacketId(a), Is.EqualTo(1));
            Assert.That(_channel.NextPacketId(b), Is.EqualTo(0));

            for (int i = 2; i <= RelayReliableChannel.MAX_PACKET_ID; ++i) _channel.NextPacketId(a);
            Assert.That(_channel.NextPacketId(a), Is.EqualTo(0));
        }

        [Test]
        public void Reliables_AreResentUntilAcknowledged()
        {
            var sent = new List<byte[]>();
            byte[] packet = { RelayComms.CL2RS_RELAY, 1, 2, 3 };
            _channel.TrackReliable(42UL, packet, 0, 0, 1000);

            Assert.That(_channel.ProcessResends(1040, p => { sent.Add(p); return true; }), Is.True);
            Assert.That(sent, Is.Empty, "not yet: channel 0 waits 50 ms");

            Assert.That(_channel.ProcessResends(1051, p => { sent.Add(p); return true; }), Is.True);
            Assert.That(sent.Count, Is.EqualTo(1));
            Assert.That(sent[0], Is.SameAs(packet));

            _channel.Acknowledge(42UL);
            Assert.That(_channel.UnacknowledgedCount, Is.EqualTo(0));
            Assert.That(_channel.ProcessResends(5000, p => { sent.Add(p); return true; }), Is.True);
            Assert.That(sent.Count, Is.EqualTo(1));
        }

        [Test]
        public void Reliables_GiveUpAfterTenSeconds()
        {
            _channel.TrackReliable(42UL, new byte[] { RelayComms.CL2RS_RELAY }, 0, 0, 1000);
            Assert.That(_channel.ProcessResends(1000 + RelayReliableChannel.RELIABLE_GIVE_UP_MS, p => true), Is.True);
            Assert.That(_channel.ProcessResends(1001 + RelayReliableChannel.RELIABLE_GIVE_UP_MS, p => true), Is.False);
        }

        #endregion

        #region transports

        private sealed class RecordingListener : IRelayTransportListener
        {
            private readonly object _lock = new object();
            private readonly List<byte[]> _data = new List<byte[]>();
            private readonly List<string> _errors = new List<string>();
            private readonly BrainCloudClient _client = new BrainCloudClient();
            public volatile bool Connected;

            public BrainCloudClient Client { get { return _client; } }
            public bool IsRelayConnected { get { return true; } }
            public bool EndMatchRequested { get { return false; } }

            public void OnTransportConnected() { Connected = true; }

            public void OnTransportData(byte[] data, int length)
            {
                byte[] copy = new byte[length];
                Buffer.BlockCopy(data, 0, copy, 0, length);
                lock (_lock) _data.Add(copy);
            }

            public void OnTransportError(string message)
            {
                lock (_lock) _errors.Add(message);
            }

            public List<byte[]> Data { get { lock (_lock) return _data.ToList(); } }
            public List<string> Errors { get { lock (_lock) return _errors.ToList(); } }
        }

        private static void WaitFor(Func<bool> condition, string what, int timeoutMs = 5000)
        {
            var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (!condition())
            {
                if (DateTime.UtcNow > deadline) Assert.Fail("Timed out waiting for " + what);
                Thread.Sleep(10);
            }
        }

        private static byte[] Framed(int totalLength, byte fill)
        {
            byte[] data = Enumerable.Repeat(fill, totalLength).ToArray();
            data[0] = (byte)(totalLength >> 8);
            data[1] = (byte)totalLength;
            return data;
        }

        [Test]
        public void Tcp_ReceivesALargePacketThatArrivesInPieces()
        {
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            var listener = new RecordingListener();
            var transport = new RelayTcpTransport(listener);
            try
            {
                transport.Connect("127.0.0.1", ((IPEndPoint)server.LocalEndpoint).Port, false);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    WaitFor(() => listener.Connected, "the transport to connect");
                    NetworkStream stream = peer.GetStream();

                    // Bigger than the 1024-byte limit on a payload: a full payload plus its header
                    byte[] packet = Framed(1024 + 13, 0x5A);
                    stream.Write(packet, 0, 600);
                    stream.Flush();
                    Thread.Sleep(200);
                    stream.Write(packet, 600, packet.Length - 600);

                    WaitFor(() => listener.Data.Count == 1 || listener.Errors.Count > 0, "the packet");
                    Assert.That(listener.Errors, Is.Empty);
                    byte[] received = listener.Data[0];
                    Assert.That(received.Length, Is.EqualTo(packet.Length));
                    Assert.That(received.Skip(2), Is.EqualTo(packet.Skip(2)), "the bytes after the length prefix");
                }
            }
            finally
            {
                transport.Close();
                server.Stop();
            }
        }

        [Test]
        public void Tcp_ReceivesALengthPrefixThatArrivesInPieces()
        {
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            var listener = new RecordingListener();
            var transport = new RelayTcpTransport(listener);
            try
            {
                transport.Connect("127.0.0.1", ((IPEndPoint)server.LocalEndpoint).Port, false);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    WaitFor(() => listener.Connected, "the transport to connect");
                    peer.NoDelay = true;
                    NetworkStream stream = peer.GetStream();

                    byte[] first = Framed(40, 0x31);
                    byte[] second = Framed(50, 0x32);
                    stream.Write(first, 0, 1);       // half of the length prefix
                    stream.Flush();
                    Thread.Sleep(300);
                    stream.Write(first, 1, first.Length - 1);
                    stream.Write(second, 0, second.Length);

                    WaitFor(() => listener.Data.Count == 2 || listener.Errors.Count > 0, "both packets");
                    Assert.That(listener.Errors, Is.Empty);
                    Assert.That(listener.Data[0].Length, Is.EqualTo(40));
                    Assert.That(listener.Data[0][2], Is.EqualTo(0x31));
                    Assert.That(listener.Data[1].Length, Is.EqualTo(50));
                    Assert.That(listener.Data[1][2], Is.EqualTo(0x32));
                }
            }
            finally
            {
                transport.Close();
                server.Stop();
            }
        }

        [Test]
        public void Tcp_SendsEachPacketOnceAndInOrder()
        {
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            var listener = new RecordingListener();
            var transport = new RelayTcpTransport(listener);
            try
            {
                transport.Connect("127.0.0.1", ((IPEndPoint)server.LocalEndpoint).Port, false);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    WaitFor(() => listener.Connected, "the transport to connect");

                    // Far more than the socket buffers hold, sent before anything is read, so
                    // that writes are still in progress when the next ones are sent
                    const int count = 4000;
                    const int size = 1000;
                    for (int i = 0; i < count; ++i)
                    {
                        byte[] packet = Framed(size, (byte)(i & 0xFF));
                        transport.Send(packet);
                    }

                    var seen = new List<int>();
                    NetworkStream stream = peer.GetStream();
                    stream.ReadTimeout = 5000;
                    byte[] buffer = new byte[size];
                    try
                    {
                        while (seen.Count < count + 1)
                        {
                            int read = 0;
                            while (read < buffer.Length)
                            {
                                int n = stream.Read(buffer, read, buffer.Length - read);
                                if (n == 0) throw new System.IO.IOException("closed");
                                read += n;
                            }
                            seen.Add(buffer[2]);
                            if (seen.Count == count) stream.ReadTimeout = 500; // anything more would be a duplicate
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        // timed out waiting for more
                    }

                    Assert.That(listener.Errors, Is.Empty);
                    Assert.That(seen.Count, Is.EqualTo(count), "each packet once");
                    Assert.That(seen, Is.EqualTo(Enumerable.Range(0, count).Select(i => i & 0xFF)), "in the order sent");
                }
            }
            finally
            {
                transport.Close();
                server.Stop();
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Tcp_ALengthWithNoPacketIsAnError(int length)
        {
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            var listener = new RecordingListener();
            var transport = new RelayTcpTransport(listener);
            try
            {
                transport.Connect("127.0.0.1", ((IPEndPoint)server.LocalEndpoint).Port, false);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    WaitFor(() => listener.Connected, "the transport to connect");
                    // The length counts the 2-byte prefix itself, so 2 or less leaves no room for a control byte
                    peer.GetStream().Write(new byte[] { 0, (byte)length }, 0, 2);

                    WaitFor(() => listener.Errors.Count > 0, "an error");
                    Assert.That(listener.Errors[0], Does.Contain("Bad packet length " + length));
                    Assert.That(listener.Data, Is.Empty);
                }
            }
            finally
            {
                transport.Close();
                server.Stop();
            }
        }

        [Test]
        public void WebSocket_ClosesWhileStillConnecting()
        {
            // Accepts the TCP connection but never answers the WebSocket handshake
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            var listener = new RecordingListener();
            var transport = new RelayWebSocketTransport(listener);
            try
            {
                transport.Connect("127.0.0.1", ((IPEndPoint)server.LocalEndpoint).Port, false);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    // What RelayComms does when Connect() is called again before the relay answers
                    Assert.That(() => transport.Close(), Throws.Nothing);
                    Assert.That(transport.CanSend, Is.False);
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void WebSocket_ClosesAfterItsConnectFailed()
        {
            // A port with nothing listening on it
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            int port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();

            var listener = new RecordingListener();
            var transport = new RelayWebSocketTransport(listener);
            transport.Connect("127.0.0.1", port, false);
            WaitFor(() => listener.Errors.Count > 0, "the connect to fail");

            // What RelayComms does when the game calls Connect() again to retry
            Assert.That(() => transport.Close(), Throws.Nothing);
            Assert.That(transport.CanSend, Is.False);
        }

        // BrainCloudWebSocket itself, which RTT uses too: Close() must not throw in any state

        [Test]
        public void WebSocketWrapper_ClosesWhileStillConnecting()
        {
            // Accepts the TCP connection but never answers the WebSocket handshake
            var server = new TcpListener(IPAddress.Loopback, 0);
            server.Start();
            try
            {
                var webSocket = new BrainCloudWebSocket("ws://127.0.0.1:" + ((IPEndPoint)server.LocalEndpoint).Port);
                using (TcpClient peer = server.AcceptTcpClient())
                {
                    Assert.That(() => webSocket.Close(), Throws.Nothing);
                    Assert.That(() => webSocket.Close(), Throws.Nothing, "a second Close()");
                }
            }
            finally
            {
                server.Stop();
            }
        }

        [Test]
        public void WebSocketWrapper_ClosesAfterItsConnectFailed()
        {
            // A port with nothing listening on it
            var probe = new TcpListener(IPAddress.Loopback, 0);
            probe.Start();
            int port = ((IPEndPoint)probe.LocalEndpoint).Port;
            probe.Stop();

            var webSocket = new BrainCloudWebSocket("ws://127.0.0.1:" + port);
            bool failed = false;
            webSocket.OnError += (sender, message) => failed = true;
            // The refusal can come before the handler is added, so wait for it either way
            Thread.Sleep(500);
            Assert.That(() => webSocket.Close(), Throws.Nothing, "after the connect " + (failed ? "failed" : "had time to fail"));
        }

        [Test]
        public void Udp_ReceivesFromARelayGivenByName()
        {
            using (var server = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
            {
                var listener = new RecordingListener();
                var transport = new RelayUdpTransport(listener);
                try
                {
                    transport.Connect("localhost", ((IPEndPoint)server.Client.LocalEndPoint).Port, false);
                    WaitFor(() => listener.Connected, "the transport to connect");

                    transport.Send(Framed(20, 0x11));
                    IPEndPoint from = new IPEndPoint(IPAddress.Any, 0);
                    server.Client.ReceiveTimeout = 5000;
                    byte[] got = server.Receive(ref from);
                    Assert.That(got.Length, Is.EqualTo(20));

                    byte[] reply = Framed(30, 0x22);
                    server.Send(reply, reply.Length, from);

                    WaitFor(() => listener.Data.Count == 1 || listener.Errors.Count > 0, "the reply");
                    Assert.That(listener.Errors, Is.Empty);
                    Assert.That(listener.Data[0], Is.EqualTo(reply));
                }
                finally
                {
                    transport.Close();
                }
            }
        }

        #endregion
    }
}
