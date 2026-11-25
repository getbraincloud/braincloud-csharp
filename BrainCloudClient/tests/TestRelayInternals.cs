// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
            _channel = new RelayReliableChannel(null);
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
        public void ReliableUnordered_IsDeliveredOnceEvenWhenResent()
        {
            // The sender resends when our acknowledgement is lost
            Receive(Packet(true, false, 0, 5, 1));
            Receive(Packet(true, false, 0, 4, 1));
            Receive(Packet(true, false, 0, 5, 1));
            Receive(Packet(true, false, 0, 4, 1));
            Assert.That(_delivered, Is.EqualTo(new[] { 5, 4 }), "in the order received, each once");
        }

        [Test]
        public void ReliableUnordered_AnIdIsNewAgainOnceItHasWrappedRound()
        {
            for (int id = 0; id <= RelayReliableChannel.MAX_PACKET_ID; ++id)
            {
                Receive(Packet(true, false, 0, id, 1));
            }
            Receive(Packet(true, false, 0, 0, 1));
            Assert.That(_delivered.Count, Is.EqualTo(RelayReliableChannel.MAX_PACKET_ID + 2));
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

                    // Read the frames back off the stream
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
