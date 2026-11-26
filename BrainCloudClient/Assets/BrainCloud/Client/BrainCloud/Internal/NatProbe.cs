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
    using System.Threading.Tasks;

    /// <summary>What a network test found, in the form reported to brainCloud.</summary>
    internal sealed class NatReport
    {
        public const int VERSION = 1;

        public NatReport(string in_mapping, bool in_udp, IPEndPoint in_publicAddress, string in_detail)
        {
            Mapping = in_mapping;
            Udp = in_udp;
            PublicAddress = in_publicAddress;
            Detail = in_detail;
        }

        /// StunClient.MAPPING_EIM, MAPPING_APDM or MAPPING_UNKNOWN.
        public string Mapping { get; private set; }

        /// True when at least one STUN address answered over UDP.
        public bool Udp { get; private set; }

        /// The public address STUN saw for the test socket, or null. Never sent to brainCloud.
        public IPEndPoint PublicAddress { get; private set; }

        /// Why the test found what it did, for the log.
        public string Detail { get; private set; }

        internal int Generation { get; set; }

        /// The natInfo object for rttRegistration.SET_NAT_INFO. It holds no address.
        public Dictionary<string, object> ToNatInfo()
        {
            var natInfo = new Dictionary<string, object>();
            natInfo["v"] = VERSION;
            natInfo["mapping"] = Mapping;
            natInfo["udp"] = Udp;
            return natInfo;
        }
    }

    /// <summary>
    /// Tests the player's network for direct connect and reports the result to brainCloud, once
    /// per RTT connection. brainCloud only lets a lobby's members connect to each other directly
    /// when every member has reported a network a peer can reach.
    ///
    /// The STUN address and token come with the RTT connection request. The test starts once
    /// the RTT connection is up, because requesting a connection clears any earlier report. It
    /// runs on a background thread with a socket of its own, and the result is sent from
    /// Update(), on the main thread. Nothing happens unless direct connect is enabled.
    /// </summary>
    internal sealed class NatProbe
    {
        public enum ReportState
        {
            None,
            Testing,
            Sent,
            Accepted,
            Failed
        }

        private readonly BrainCloudClient m_clientRef;
        private readonly Action<ServerCall> m_send;
        private bool m_enabled = false;
        private bool m_rttConnected = false;
        // On WebGL start() compiles empty, so some of these are set but never read (CS0414)
#pragma warning disable 0414
        private bool m_testedThisConnection = false;
        // A test's report is on the server for this connection; only then does turning off withdraw anything
        private bool m_testReported = false;
        private int m_generation = 0;

        private string m_stunHost = null;
        private int m_stunPort = 0;
        private byte[] m_stunToken = null;
#pragma warning restore 0414

        // Finished tests waiting for Update, added by the test threads. A list rather than a single
        // slot: a test from an earlier connection can finish next to the current one, and must not
        // overwrite it.
        private readonly object m_finishedLock = new object();
        private readonly List<NatReport> m_finished = new List<NatReport>();

        public NatProbe(BrainCloudClient in_client)
            : this(in_client, in_client.SendRequest)
        {
        }

        /// For the unit tests: in_send receives each report instead of the client, and in_client may be null.
        internal NatProbe(BrainCloudClient in_client, Action<ServerCall> in_send)
        {
            m_clientRef = in_client;
            m_send = in_send;
        }

        private bool loggingEnabled
        {
            get { return m_clientRef != null && m_clientRef.LoggingEnabled; }
        }

        /// <summary>
        /// Whether the player takes part in direct connect. Off by default. Switching it on while
        /// RTT is connected tests now rather than on the next connection. Switching it off withdraws
        /// a report already sent, so the player's lobbies stop qualifying straight away.
        /// </summary>
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                if (value == m_enabled) return;
                m_enabled = value;
                if (m_enabled)
                {
                    // Answers to anything sent before, such as a withdrawal, no longer change State
                    ++m_generation;
                    start();
                    return;
                }

                // Forget any test still running, and let a later switch back on test again
                ++m_generation;
                m_testedThisConnection = false;
                bool reported = m_testReported;
                m_testReported = false;
                LastReport = null;
                State = ReportState.None;
                // Not after logout: the server is dropping the connection, and the report with it
                if (m_rttConnected && reported && (m_clientRef == null || m_clientRef.Authenticated))
                {
                    send(new NatReport(StunClient.MAPPING_UNKNOWN, false, null, "direct connect was turned off"));
                }
            }
        }

        /// The last report sent for the current connection, or null.
        public NatReport LastReport { get; private set; }

        public ReportState State { get; private set; }

        /// For the unit tests: how many finished tests Update has taken, sent or dropped.
        internal int ResultsHandled { get; private set; }

        /// True when the last connection request's response had a stun section, usable or not.
        internal bool StunSectionReceived { get; private set; }

        /// <summary>
        /// Takes the stun section of an rttRegistration.REQUEST_CLIENT_CONNECTION response, or null
        /// when the response had none. Called for every connection request; replaces what was there.
        /// </summary>
        public void SetStunSection(Dictionary<string, object> in_stun)
        {
            m_stunHost = null;
            m_stunPort = 0;
            m_stunToken = null;
            StunSectionReceived = in_stun != null;
            if (in_stun == null) return;

            object host, port, token;
            if (in_stun.TryGetValue("host", out host) && host is string && in_stun.TryGetValue("port", out port) && port is int)
            {
                m_stunHost = (string)host;
                m_stunPort = (int)port;
            }
            if (in_stun.TryGetValue("token", out token) && token is string)
            {
                try
                {
                    m_stunToken = Convert.FromBase64String((string)token);
                }
                catch (FormatException)
                {
                    // Without a usable token the service will not answer; the test reports that
                    m_stunToken = null;
                }
            }
        }

        public void OnRttConnected()
        {
            ++m_generation;
            m_rttConnected = true;
            m_testedThisConnection = false;
            m_testReported = false;
            LastReport = null;
            State = ReportState.None;
            start();
        }

        public void OnRttDisconnected()
        {
            ++m_generation;
            m_rttConnected = false;
            m_testedThisConnection = false;
            // The server drops the report with the connection
            m_testReported = false;
        }

        /// <summary>Sends a finished test's report. Call on the main thread.</summary>
        public void Update()
        {
            // Results from an earlier connection, or from before direct connect was turned off, are dropped
            NatReport current = null;
            lock (m_finishedLock)
            {
                if (m_finished.Count == 0) return;
                ResultsHandled += m_finished.Count;
                foreach (NatReport finished in m_finished)
                {
                    if (finished.Generation == m_generation) current = finished;
                }
                m_finished.Clear();
            }
            if (current == null || !m_rttConnected || !m_enabled) return;

            m_testReported = true;
            send(current);
        }

        private void send(NatReport in_report)
        {
            if (loggingEnabled)
            {
                m_clientRef.Log("Direct connect network report: mapping " + in_report.Mapping + ", UDP " + (in_report.Udp ? "works" : "got no answer") +
                    (string.IsNullOrEmpty(in_report.Detail) ? "" : " (" + in_report.Detail + ")"));
            }

            LastReport = in_report;
            State = ReportState.Sent;
            int generation = m_generation;

            var data = new Dictionary<string, object>();
            data[OperationParam.RTTNatInfo.Value] = in_report.ToNatInfo();
            ServerCallback callback = BrainCloudClient.CreateServerCallback(
                (response, cbObject) =>
                {
                    if (generation == m_generation) State = ReportState.Accepted;
                },
                (status, reasonCode, error, cbObject) =>
                {
                    if (generation == m_generation) State = ReportState.Failed;
                    if (loggingEnabled)
                    {
                        m_clientRef.Log("Direct connect network report failed: " + status + " " + reasonCode + " " + error);
                    }
                });
            m_send(new ServerCall(ServiceName.RTTRegistration, ServiceOperation.SetNatInfo, data, callback));
        }

        private void start()
        {
            // No UDP in a browser: a WebGL player never tests or reports, and always uses the relay
#if !UNITY_WEBGL
            if (!m_enabled || !m_rttConnected || m_testedThisConnection || string.IsNullOrEmpty(m_stunHost)) return;
            m_testedThisConnection = true;
            State = ReportState.Testing;

            int generation = m_generation;
            string host = m_stunHost;
            int port = m_stunPort;
            byte[] token = m_stunToken;

            Task.Run(() =>
            {
                NatReport report;
                try
                {
                    report = Measure(resolve(host, port), token);
                }
                catch (Exception e)
                {
                    report = new NatReport(StunClient.MAPPING_UNKNOWN, false, null, e.Message);
                }
                report.Generation = generation;
                lock (m_finishedLock)
                {
                    m_finished.Add(report);
                }
            });
#endif
        }

        /// <summary>
        /// Up to two IPv4 addresses for the STUN host: classifying the NAT takes two, and the
        /// STUN service publishes one per zone under the same name.
        /// </summary>
        private static List<IPEndPoint> resolve(string in_host, int in_port)
        {
            var servers = new List<IPEndPoint>();
            var addresses = new List<IPAddress>();
            foreach (IPAddress address in Dns.GetHostAddresses(in_host))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork && !addresses.Contains(address))
                {
                    addresses.Add(address);
                    servers.Add(new IPEndPoint(address, in_port));
                    if (servers.Count == 2) break;
                }
            }
            return servers;
        }

        /// <summary>
        /// Asks each STUN address, from one throwaway socket, what public address it sees, and
        /// classifies the NAT from the answers. Blocks for up to a few seconds.
        /// </summary>
        public static NatReport Measure(IList<IPEndPoint> in_servers, byte[] in_token)
        {
            if (in_servers.Count == 0)
            {
                return new NatReport(StunClient.MAPPING_UNKNOWN, false, null, "the STUN host has no IPv4 address");
            }

            List<StunResult> results;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                results = StunClient.Probe(socket, in_servers, in_token);
            }

            IPEndPoint publicAddress = null;
            int answered = 0;
            string firstError = null;
            foreach (StunResult result in results)
            {
                if (result.Mapped != null)
                {
                    ++answered;
                    if (publicAddress == null) publicAddress = result.Mapped;
                }
                else if (firstError == null)
                {
                    firstError = result.Error;
                }
            }

            string detail = answered + " of " + results.Count + " STUN addresses answered";
            if (answered < results.Count && firstError != null) detail += "; " + firstError;
            return new NatReport(StunClient.ClassifyMapping(results), answered > 0, publicAddress, detail);
        }
    }
}
