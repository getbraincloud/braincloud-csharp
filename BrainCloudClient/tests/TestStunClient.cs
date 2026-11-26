// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BrainCloudTests
{
    /// <summary>
    /// The STUN client and the direct connect network test, with no brainCloud server: the codec
    /// against the RFC 5769 test vectors (the server's STUN module uses the same ones), and the
    /// probe against small STUN responders on loopback.
    /// </summary>
    [TestFixture]
    public class TestStunClient
    {
        private static readonly byte[] TX = Hex("b7 e7 a7 01 bc 34 d6 86 fa 87 df ae");

        private readonly List<FakeStun> m_servers = new List<FakeStun>();

        [TearDown]
        public void StopServers()
        {
            foreach (FakeStun server in m_servers) server.Dispose();
            m_servers.Clear();
        }

        #region codec

        [Test]
        public void Fingerprint_MatchesRfc5769SampleRequest()
        {
            byte[] request = Hex("00 01 00 58 21 12 a4 42 b7 e7 a7 01 bc 34 d6 86 fa 87 df ae 80 22 00 10 53 54 55 4e 20 74 65 73 74 20 63 6c 69 65 6e 74 " +
                "00 24 00 04 6e 00 01 ff 80 29 00 08 93 2f f9 b1 51 26 3b 36 00 06 00 09 65 76 74 6a 3a 68 36 76 59 20 20 20 " +
                "00 08 00 14 9a ea a7 0c bf d8 cb 56 78 1e f2 b5 b2 d3 f2 49 c1 b5 71 a2 80 28 00 04 e5 7a 3b cf");
            Assert.That(StunClient.Fingerprint(request, request.Length - 8), Is.EqualTo(0xe57a3bcfu));
        }

        [Test]
        public void ParseResponse_Rfc5769Ipv4Address()
        {
            byte[] msg = Response(Hex("00 20 00 08 00 01 a1 47 e1 12 a6 43"));
            string error;
            IPEndPoint mapped = StunClient.ParseResponse(msg, msg.Length, TX, out error);
            Assert.That(error, Is.Null);
            Assert.That(mapped, Is.EqualTo(new IPEndPoint(IPAddress.Parse("192.0.2.1"), 32853)));
        }

        [Test]
        public void ParseResponse_Rfc5769Ipv6Address()
        {
            byte[] msg = Response(Hex("00 20 00 14 00 02 a1 47 01 13 a9 fa a5 d3 f1 79 bc 25 f4 b5 be d2 b9 d9"));
            string error;
            IPEndPoint mapped = StunClient.ParseResponse(msg, msg.Length, TX, out error);
            Assert.That(error, Is.Null);
            Assert.That(mapped, Is.EqualTo(new IPEndPoint(IPAddress.Parse("2001:db8:1234:5678:11:2233:4455:6677"), 32853)));
        }

        [Test]
        public void ParseResponse_ChecksTheFingerprint()
        {
            byte[] msg = Response(Hex("00 20 00 08 00 01 a1 47 e1 12 a6 43"), true);
            string error;
            Assert.That(StunClient.ParseResponse(msg, msg.Length, TX, out error), Is.Not.Null, error);

            msg[msg.Length - 1] ^= 1;
            Assert.That(StunClient.ParseResponse(msg, msg.Length, TX, out error), Is.Null);
            Assert.That(error, Does.Contain("FINGERPRINT"));
        }

        [Test]
        public void ParseResponse_RejectsWhatIsNotOurAnswer()
        {
            byte[] good = Response(Hex("00 20 00 08 00 01 a1 47 e1 12 a6 43"));
            string error;

            byte[] otherTx = (byte[])TX.Clone();
            otherTx[11] ^= 1;
            Assert.That(StunClient.ParseResponse(good, good.Length, otherTx, out error), Is.Null);
            Assert.That(error, Does.Contain("transaction id"));

            byte[] errorResponse = (byte[])good.Clone();
            errorResponse[1] = 0x11; // 0x0111, Binding error response
            Assert.That(StunClient.ParseResponse(errorResponse, errorResponse.Length, TX, out error), Is.Null);
            Assert.That(error, Does.Contain("Binding success"));

            Assert.That(StunClient.ParseResponse(good, good.Length - 4, TX, out error), Is.Null);
            Assert.That(error, Does.Contain("length"));

            Assert.That(StunClient.ParseResponse(good, 12, TX, out error), Is.Null);
            Assert.That(error, Does.Contain("header"));

            byte[] noAddress = Response(new byte[0]);
            Assert.That(StunClient.ParseResponse(noAddress, noAddress.Length, TX, out error), Is.Null);
            Assert.That(error, Does.Contain("XOR-MAPPED-ADDRESS"));
        }

        [Test]
        public void BuildRequest_PutsTheTokenFirst()
        {
            byte[] token = Enumerable.Range(1, 48).Select(i => (byte)i).ToArray();
            byte[] msg = StunClient.BuildRequest(TX, token);

            Assert.That(msg.Length, Is.EqualTo(20 + 4 + 48));
            Assert.That(msg.Take(8).ToArray(), Is.EqualTo(Hex("00 01 00 34 21 12 a4 42")));
            Assert.That(msg.Skip(8).Take(12).ToArray(), Is.EqualTo(TX));
            Assert.That(msg.Skip(20).Take(4).ToArray(), Is.EqualTo(Hex("bc 01 00 30")));
            Assert.That(msg.Skip(24).ToArray(), Is.EqualTo(token));
        }

        [Test]
        public void BuildRequest_WithoutTokenIsABareHeader()
        {
            byte[] msg = StunClient.BuildRequest(TX, null);
            Assert.That(msg.Take(8).ToArray(), Is.EqualTo(Hex("00 01 00 00 21 12 a4 42")));
            Assert.That(msg.Length, Is.EqualTo(20));
        }

        #endregion

        #region probe

        [Test]
        public void Probe_SamePublicAddressFromBothServers_IsEim()
        {
            // Loopback has no NAT, so both servers see the socket's own address
            List<StunResult> results = Probe(Start(), Start());
            Assert.That(results.All(r => r.Mapped != null), Is.True, Errors(results));
            Assert.That(StunClient.ClassifyMapping(results), Is.EqualTo(StunClient.MAPPING_EIM));
        }

        [Test]
        public void Probe_DifferentPublicPorts_IsApdm()
        {
            FakeStun symmetric = Start();
            symmetric.MapTo = from => new IPEndPoint(from.Address, from.Port == 65535 ? 1 : from.Port + 1);
            List<StunResult> results = Probe(Start(), symmetric);
            Assert.That(results.All(r => r.Mapped != null), Is.True, Errors(results));
            Assert.That(StunClient.ClassifyMapping(results), Is.EqualTo(StunClient.MAPPING_APDM));
        }

        [Test]
        public void Probe_OneAnswer_IsUnknown()
        {
            FakeStun silent = Start();
            silent.Silent = true;
            List<StunResult> results = Probe(Start(), silent);
            Assert.That(results[0].Mapped, Is.Not.Null, results[0].Error);
            Assert.That(results[1].Mapped, Is.Null);
            Assert.That(results[1].Error, Does.Contain("no reply"));
            Assert.That(StunClient.ClassifyMapping(results), Is.EqualTo(StunClient.MAPPING_UNKNOWN));
        }

        [Test]
        public void Probe_ResendsAfterALostRequest()
        {
            FakeStun lossy = Start();
            lossy.DropFirst = 1;
            List<StunResult> results = Probe(lossy);
            Assert.That(results[0].Mapped, Is.Not.Null, results[0].Error);
            // At least one resend; a slow machine may send another before the answer arrives
            Assert.That(lossy.Requests, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Probe_SendsTheToken()
        {
            byte[] token = Enumerable.Range(0, 48).Select(i => (byte)(i * 7)).ToArray();
            FakeStun server = Start();
            server.RequiredToken = token;

            List<StunResult> results = ProbeWithToken(token, server);
            Assert.That(results[0].Mapped, Is.Not.Null, results[0].Error);

            byte[] wrong = (byte[])token.Clone();
            wrong[0] ^= 1;
            Assert.That(ProbeWithToken(wrong, server)[0].Mapped, Is.Null, "a server that checks tokens does not answer a bad one");
            Assert.That(ProbeWithToken(null, server)[0].Mapped, Is.Null, "nor a request without one");
        }

        [Test]
        public void Measure_ReportsNoAddressToTheServer()
        {
            FakeStun a = Start();
            FakeStun b = Start();
            NatReport report = NatProbe.Measure(new List<IPEndPoint> { a.EndPoint, b.EndPoint }, null);

            Assert.That(report.Mapping, Is.EqualTo(StunClient.MAPPING_EIM), report.Detail);
            Assert.That(report.Udp, Is.True);
            Assert.That(report.PublicAddress, Is.Not.Null);
            Assert.That(report.Detail, Is.EqualTo("2 of 2 STUN addresses answered"));

            Dictionary<string, object> natInfo = report.ToNatInfo();
            Assert.That(natInfo.Keys, Is.EquivalentTo(new[] { "v", "mapping", "udp" }));
            Assert.That(natInfo["v"], Is.EqualTo(1));
            Assert.That(natInfo["mapping"], Is.EqualTo("eim"));
            Assert.That(natInfo["udp"], Is.EqualTo(true));
        }

        [Test]
        public void Measure_NoServerAddresses_IsUnknownWithoutUdp()
        {
            NatReport report = NatProbe.Measure(new List<IPEndPoint>(), null);
            Assert.That(report.Mapping, Is.EqualTo(StunClient.MAPPING_UNKNOWN));
            Assert.That(report.Udp, Is.False);
            Assert.That(report.PublicAddress, Is.Null);
        }

        #endregion

        #region network test and report

        // Each test drives a NatProbe the way RTTComms does (stun section, connected, disconnected,
        // Update every frame) and captures the SET_NAT_INFO calls instead of sending them. The STUN
        // host is 127.0.0.1 alone, so the mapping is always "unknown"; what matters here is when,
        // and whether, a report goes out.

        [Test]
        public void NatProbe_ReportsOncePerConnection_OnlyAfterConnected()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            Pump(probe, () => false, 300);
            Assert.That(sent, Is.Empty, "nothing before RTT connects");
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.None));

            probe.OnRttConnected();
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Testing));
            Assert.That(Pump(probe, () => sent.Count == 1), Is.True, "the report was not sent");
            Assert.That(sent[0].GetService(), Is.EqualTo("rttRegistration"));
            Assert.That(sent[0].GetOperation(), Is.EqualTo("SET_NAT_INFO"));
            Assert.That(NatInfo(sent[0])["mapping"], Is.EqualTo("unknown"));
            Assert.That(NatInfo(sent[0])["udp"], Is.EqualTo(true));
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Sent));

            sent[0].GetCallback().OnSuccessCallback("{}");
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Accepted));

            Pump(probe, () => false, 300);
            Assert.That(sent.Count, Is.EqualTo(1), "one report per connection");
        }

        [Test]
        public void NatProbe_NotEnabledOrNoStunSection_SendsNothing()
        {
            var sent = new List<ServerCall>();
            NatProbe off = NewProbe(sent);
            off.OnRttConnected();
            Pump(off, () => false, 500);
            Assert.That(off.State, Is.EqualTo(NatProbe.ReportState.None));

            NatProbe noStun = NewProbe(sent);
            noStun.SetStunSection(null);
            noStun.Enabled = true;
            noStun.OnRttConnected();
            Pump(noStun, () => false, 500);
            Assert.That(noStun.State, Is.EqualTo(NatProbe.ReportState.None));

            Assert.That(sent, Is.Empty);
        }

        [Test]
        public void NatProbe_ResultFromAnEarlierConnection_IsDropped()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;

            // Disconnected before the result: nothing is sent
            probe.OnRttConnected();
            probe.OnRttDisconnected();
            Assert.That(Pump(probe, () => probe.ResultsHandled == 1), Is.True, "the test did not finish");
            Assert.That(sent, Is.Empty);

            // Reconnected while the first test runs: only the new connection's result is sent
            probe.OnRttConnected();
            probe.OnRttDisconnected();
            probe.OnRttConnected();
            Assert.That(Pump(probe, () => probe.ResultsHandled == 3), Is.True, "the tests did not finish");
            Assert.That(sent.Count, Is.EqualTo(1));
        }

        [Test]
        public void NatProbe_TurnedOffBeforeTheResult_SendsNothing()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            probe.OnRttConnected();
            probe.Enabled = false;
            Assert.That(Pump(probe, () => probe.ResultsHandled == 1), Is.True, "the test did not finish");
            Assert.That(sent, Is.Empty, "nothing was reported, so there is nothing to withdraw");
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.None));
        }

        [Test]
        public void NatProbe_TurnedOffAfterAReport_Withdraws_AndOnAgainTestsAgain()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            probe.OnRttConnected();
            Assert.That(Pump(probe, () => sent.Count == 1), Is.True, "the report was not sent");
            sent[0].GetCallback().OnSuccessCallback("{}");

            probe.Enabled = false;
            Assert.That(sent.Count, Is.EqualTo(2), "the withdrawal is sent at once");
            Assert.That(NatInfo(sent[1])["mapping"], Is.EqualTo("unknown"));
            Assert.That(NatInfo(sent[1])["udp"], Is.EqualTo(false));
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Sent));

            probe.Enabled = true;
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Testing));
            sent[1].GetCallback().OnSuccessCallback("{}");
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Testing), "the withdrawal's answer does not settle the new test");
            Assert.That(Pump(probe, () => sent.Count == 3), Is.True, "no new report after turning it back on");
            Assert.That(NatInfo(sent[2])["udp"], Is.EqualTo(true));
        }

        [Test]
        public void NatProbe_TurnedOffAgainBeforeANewReport_SendsNoSecondWithdrawal()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            probe.OnRttConnected();
            Assert.That(Pump(probe, () => sent.Count == 1), Is.True, "the report was not sent");
            sent[0].GetCallback().OnSuccessCallback("{}");

            probe.Enabled = false;
            Assert.That(sent.Count, Is.EqualTo(2), "the withdrawal");

            // On and off again before the new test reports: the server still holds the withdrawal
            probe.Enabled = true;
            probe.Enabled = false;
            Assert.That(Pump(probe, () => probe.ResultsHandled == 2), Is.True, "the second test did not finish");
            Assert.That(sent.Count, Is.EqualTo(2));
        }

        [Test]
        public void NatProbe_TurnedOffWhileDisconnected_SendsNothing()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            probe.OnRttConnected();
            Assert.That(Pump(probe, () => sent.Count == 1), Is.True, "the report was not sent");

            // The server dropped the report with the connection
            probe.OnRttDisconnected();
            probe.Enabled = false;
            Assert.That(sent.Count, Is.EqualTo(1));
        }

        [Test]
        public void NatProbe_AnswerForAnEarlierConnection_DoesNotChangeTheState()
        {
            var sent = new List<ServerCall>();
            NatProbe probe = NewProbe(sent);
            probe.Enabled = true;
            probe.OnRttConnected();
            Assert.That(Pump(probe, () => sent.Count == 1), Is.True, "the report was not sent");

            probe.OnRttDisconnected();
            probe.OnRttConnected();
            Assert.That(probe.LastReport, Is.Null);
            sent[0].GetCallback().OnSuccessCallback("{}");
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Testing));
        }

        #endregion

        #region helpers

        /// A probe whose STUN host is a loopback responder, sending its reports to in_sent.
        private NatProbe NewProbe(List<ServerCall> in_sent)
        {
            var probe = new NatProbe(null, call => in_sent.Add(call));
            var stun = new Dictionary<string, object>();
            stun["host"] = "127.0.0.1";
            stun["port"] = Start().EndPoint.Port;
            probe.SetStunSection(stun);
            return probe;
        }

        /// Calls Update every 10 ms, as RTTComms does each frame, until in_done is true or the time is up.
        private static bool Pump(NatProbe in_probe, Func<bool> in_done, int in_timeoutMs = 3000)
        {
            Stopwatch clock = Stopwatch.StartNew();
            while (clock.ElapsedMilliseconds < in_timeoutMs)
            {
                in_probe.Update();
                if (in_done()) return true;
                Thread.Sleep(10);
            }
            return false;
        }

        private static Dictionary<string, object> NatInfo(ServerCall in_call)
        {
            return (Dictionary<string, object>)in_call.GetJsonData()["natInfo"];
        }

        private FakeStun Start()
        {
            var server = new FakeStun();
            m_servers.Add(server);
            return server;
        }

        private static List<StunResult> Probe(params FakeStun[] in_servers)
        {
            return ProbeWithToken(null, in_servers);
        }

        /// Shorter timeouts than the library's (a silent server costs 1.4 s rather than 2.8 s), with room for a slow machine
        private static List<StunResult> ProbeWithToken(byte[] in_token, params FakeStun[] in_servers)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                return StunClient.Probe(socket, in_servers.Select(s => s.EndPoint).ToList(), in_token, 3, 200);
            }
        }

        private static string Errors(IEnumerable<StunResult> in_results)
        {
            return string.Join("; ", in_results.Select(r => r.Server + ": " + (r.Error ?? "ok")).ToArray());
        }

        /// A Binding success response to TX carrying the given attributes, optionally with a FINGERPRINT.
        private static byte[] Response(byte[] in_attributes, bool in_fingerprint = false)
        {
            int body = in_attributes.Length + (in_fingerprint ? 8 : 0);
            byte[] msg = new byte[20 + body];
            Hex("01 01").CopyTo(msg, 0);
            msg[2] = (byte)(body >> 8);
            msg[3] = (byte)body;
            Hex("21 12 a4 42").CopyTo(msg, 4);
            TX.CopyTo(msg, 8);
            in_attributes.CopyTo(msg, 20);
            if (in_fingerprint)
            {
                int at = 20 + in_attributes.Length;
                Hex("80 28 00 04").CopyTo(msg, at);
                uint crc = StunClient.Fingerprint(msg, at);
                msg[at + 4] = (byte)(crc >> 24);
                msg[at + 5] = (byte)(crc >> 16);
                msg[at + 6] = (byte)(crc >> 8);
                msg[at + 7] = (byte)crc;
            }
            return msg;
        }

        private static byte[] Hex(string in_hex)
        {
            string[] parts = in_hex.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Select(p => Convert.ToByte(p, 16)).ToArray();
        }

        /// <summary>
        /// A STUN server on loopback that answers Binding requests the way the brainCloud STUN service
        /// does: XOR-MAPPED-ADDRESS then FINGERPRINT, and no reply at all to a request it will not serve.
        /// </summary>
        private sealed class FakeStun : IDisposable
        {
            private readonly Socket m_socket;
            private readonly Thread m_thread;
            private int m_requests;

            public FakeStun()
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                EndPoint = (IPEndPoint)m_socket.LocalEndPoint;
                m_thread = new Thread(run) { IsBackground = true };
                m_thread.Start();
            }

            public IPEndPoint EndPoint { get; private set; }

            /// The address to report for a request from the given one; by default, that address.
            public Func<IPEndPoint, IPEndPoint> MapTo { get; set; }

            /// When set, only requests whose first attribute is this token are answered.
            public byte[] RequiredToken { get; set; }

            public bool Silent { get; set; }

            /// How many requests to ignore before answering, as if they were lost.
            public int DropFirst { get; set; }

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
                    int count = Interlocked.Increment(ref m_requests);
                    if (Silent || count <= DropFirst || !hasToken(buffer, n)) continue;

                    IPEndPoint source = (IPEndPoint)from;
                    IPEndPoint mapped = MapTo != null ? MapTo(source) : source;
                    byte[] reply = answer(buffer, mapped);
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

            private bool hasToken(byte[] in_msg, int in_length)
            {
                byte[] token = RequiredToken;
                if (token == null) return true;
                if (in_length < 24 + token.Length) return false;
                if (in_msg[20] != 0xBC || in_msg[21] != 0x01 || ((in_msg[22] << 8) | in_msg[23]) != token.Length) return false;
                for (int i = 0; i < token.Length; ++i)
                {
                    if (in_msg[24 + i] != token[i]) return false;
                }
                return true;
            }

            private static byte[] answer(byte[] in_request, IPEndPoint in_mapped)
            {
                byte[] msg = new byte[20 + 12 + 8];
                msg[0] = 0x01;
                msg[1] = 0x01;
                msg[3] = 20;
                Buffer.BlockCopy(in_request, 4, msg, 4, 16); // cookie and transaction id

                msg[20] = 0x00;
                msg[21] = 0x20;
                msg[23] = 8;
                msg[25] = 0x01;
                int port = in_mapped.Port ^ 0x2112;
                msg[26] = (byte)(port >> 8);
                msg[27] = (byte)port;
                byte[] ip = in_mapped.Address.GetAddressBytes();
                for (int i = 0; i < 4; ++i) msg[28 + i] = (byte)(ip[i] ^ msg[4 + i]);

                msg[32] = 0x80;
                msg[33] = 0x28;
                msg[35] = 4;
                uint crc = StunClient.Fingerprint(msg, 32);
                msg[36] = (byte)(crc >> 24);
                msg[37] = (byte)(crc >> 16);
                msg[38] = (byte)(crc >> 8);
                msg[39] = (byte)crc;
                return msg;
            }
        }

        #endregion
    }

    /// <summary>
    /// The network test against a brainCloud server. It needs a deployment with a STUN service and
    /// an app whose plan offers direct connect; anywhere else the RTT connection request carries no
    /// stun section, and the test is ignored.
    /// </summary>
    [TestFixture]
    public class TestDirectConnectReport : TestFixtureBase
    {
        [Test]
        public void TestReportAfterRttConnectsAndWithdrawWhenTurnedOff()
        {
            NatProbe probe = _bc.Client.DirectConnectProbe;
            _bc.RelayService.EnableDirectConnect(true);
            Assert.That(_bc.RelayService.IsDirectConnectEnabled(), Is.True);

            TestResult tr = new TestResult(_bc);
            _bc.RTTService.EnableRTT(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            if (!probe.StunSectionReceived)
            {
                Assert.Ignore("The server sent no stun section: no STUN service here, or the app's plan has no direct connect");
            }
            // A stun section starts the test as RTT connects; if not, the client failed to read it
            Assert.That(probe.State, Is.Not.EqualTo(NatProbe.ReportState.None), "the server sent a stun section but no test started");

            Assert.That(pumpUntilSettled(probe, 10000), Is.True, "the report was not settled; state " + probe.State);
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Accepted));
            Assert.That(probe.LastReport, Is.Not.Null);
            Console.WriteLine("Network test: " + probe.LastReport.Mapping + ", UDP " + probe.LastReport.Udp + " (" + probe.LastReport.Detail + ")");

            _bc.RelayService.EnableDirectConnect(false);
            Assert.That(_bc.RelayService.IsDirectConnectEnabled(), Is.False);
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Sent), "turning it off sends a withdrawal");
            Assert.That(probe.LastReport.Mapping, Is.EqualTo(StunClient.MAPPING_UNKNOWN));
            Assert.That(probe.LastReport.Udp, Is.False);
            Assert.That(pumpUntilSettled(probe, 5000), Is.True);
            Assert.That(probe.State, Is.EqualTo(NatProbe.ReportState.Accepted));

            _bc.RTTService.DisableRTT();
        }

        private bool pumpUntilSettled(NatProbe in_probe, int in_timeoutMs)
        {
            DateTime until = DateTime.UtcNow.AddMilliseconds(in_timeoutMs);
            while (DateTime.UtcNow < until)
            {
                _bc.Update();
                if (in_probe.State == NatProbe.ReportState.Accepted || in_probe.State == NatProbe.ReportState.Failed) return true;
                Thread.Sleep(20);
            }
            return false;
        }
    }
}
