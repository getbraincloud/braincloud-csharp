// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Runtime.CompilerServices;

// The unit tests use the STUN client and network test directly.
[assembly: InternalsVisibleTo("UnitTest")]

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>What one STUN server address said about us, or why it said nothing.</summary>
    internal sealed class StunResult
    {
        public StunResult(IPEndPoint in_server, IPEndPoint in_mapped, string in_error)
        {
            Server = in_server;
            Mapped = in_mapped;
            Error = in_error;
        }

        public IPEndPoint Server { get; private set; }

        /// The public address the server saw for our socket, or null.
        public IPEndPoint Mapped { get; private set; }

        public string Error { get; private set; }
    }

    /// <summary>
    /// A minimal STUN (RFC 8489) Binding client: asks "what public address do you see for this
    /// socket?". The brainCloud STUN token, when there is one, is sent as the first and only
    /// attribute, which is where the STUN service looks for it.
    /// </summary>
    internal static class StunClient
    {
        public const int HEADER_LENGTH = 20;
        public const uint MAGIC_COOKIE = 0x2112A442;
        public const int BINDING_REQUEST = 0x0001;
        public const int BINDING_SUCCESS = 0x0101;
        public const int ATTR_XOR_MAPPED_ADDRESS = 0x0020;
        public const int ATTR_FINGERPRINT = 0x8028;
        public const int ATTR_TOKEN = 0xBC01;

        /// Every address saw the same public port: a peer can reach us there.
        public const string MAPPING_EIM = "eim";
        /// The addresses saw different ports (a symmetric NAT): a peer cannot use what STUN reports.
        public const string MAPPING_APDM = "apdm";
        /// Fewer than two addresses answered, so there was nothing to compare.
        public const string MAPPING_UNKNOWN = "unknown";

        private static readonly Random s_random = new Random();

        /// <summary>A Binding request; with a token, the token attribute comes first and is the only one.</summary>
        public static byte[] BuildRequest(byte[] in_transactionId, byte[] in_token)
        {
            int body = in_token == null ? 0 : 4 + in_token.Length;
            byte[] msg = new byte[HEADER_LENGTH + body];
            writeU16(msg, 0, BINDING_REQUEST);
            writeU16(msg, 2, body);
            writeU32(msg, 4, MAGIC_COOKIE);
            Buffer.BlockCopy(in_transactionId, 0, msg, 8, 12);
            if (in_token != null)
            {
                writeU16(msg, 20, ATTR_TOKEN);
                writeU16(msg, 22, in_token.Length);
                Buffer.BlockCopy(in_token, 0, msg, 24, in_token.Length);
            }
            return msg;
        }

        /// <summary>
        /// The address in a Binding success response, or null with the reason. Checks the type, the
        /// cookie, the length, the transaction id and, when present, the FINGERPRINT.
        /// </summary>
        public static IPEndPoint ParseResponse(byte[] in_msg, int in_length, byte[] in_transactionId, out string out_error)
        {
            out_error = null;
            if (in_length < HEADER_LENGTH) { out_error = "shorter than a STUN header"; return null; }
            if (readU16(in_msg, 0) != BINDING_SUCCESS) { out_error = "not a Binding success response"; return null; }
            if (readU32(in_msg, 4) != MAGIC_COOKIE) { out_error = "magic cookie missing"; return null; }
            if (readU16(in_msg, 2) != in_length - HEADER_LENGTH) { out_error = "length field does not match the datagram"; return null; }
            for (int i = 0; i < 12; ++i)
            {
                if (in_msg[8 + i] != in_transactionId[i]) { out_error = "transaction id does not match the request"; return null; }
            }

            IPEndPoint mapped = null;
            int p = HEADER_LENGTH;
            while (p + 4 <= in_length)
            {
                int type = readU16(in_msg, p);
                int len = readU16(in_msg, p + 2);
                int value = p + 4;
                if (value + len > in_length) { out_error = "attribute runs past the end of the message"; return null; }
                if (type == ATTR_XOR_MAPPED_ADDRESS && len >= 8)
                {
                    int family = in_msg[value + 1];
                    int port = readU16(in_msg, value + 2) ^ (int)(MAGIC_COOKIE >> 16);
                    int ipLen = family == 0x01 ? 4 : 16;
                    if (len < 4 + ipLen) { out_error = "XOR-MAPPED-ADDRESS is too short"; return null; }
                    byte[] ip = new byte[ipLen];
                    // The address is XORed with the cookie and then the transaction id, which follow it in the header
                    for (int i = 0; i < ipLen; ++i) ip[i] = (byte)(in_msg[value + 4 + i] ^ in_msg[4 + i]);
                    mapped = new IPEndPoint(new IPAddress(ip), port);
                }
                else if (type == ATTR_FINGERPRINT && len == 4)
                {
                    if (readU32(in_msg, value) != Fingerprint(in_msg, value - 4))
                    {
                        out_error = "FINGERPRINT does not match";
                        return null;
                    }
                }
                p = value + ((len + 3) & ~3);
            }
            if (mapped == null) out_error = "no XOR-MAPPED-ADDRESS in the response";
            return mapped;
        }

        /// <summary>The FINGERPRINT value for the first in_length bytes of a message.</summary>
        public static uint Fingerprint(byte[] in_msg, int in_length)
        {
            return crc32(in_msg, in_length) ^ 0x5354554E;
        }

        /// <summary>
        /// Sends a Binding request to each server address from the same socket and waits for the
        /// answers, resending to those that have not answered. Using one socket is the point:
        /// comparing the public port each address reports tells a punchable NAT from a symmetric one.
        /// Blocks for at most about (in_attempts doublings of in_firstTimeoutMs).
        /// </summary>
        public static List<StunResult> Probe(Socket in_socket, IList<IPEndPoint> in_servers, byte[] in_token,
            int in_attempts = 3, int in_firstTimeoutMs = 400)
        {
            var pending = new Dictionary<string, Pending>();
            var results = new Dictionary<IPEndPoint, StunResult>();
            Stopwatch clock = Stopwatch.StartNew();
            byte[] buffer = new byte[1500];
            int timeout = in_firstTimeoutMs;

            for (int attempt = 0; attempt < in_attempts && results.Count < in_servers.Count; ++attempt)
            {
                foreach (IPEndPoint server in in_servers)
                {
                    if (results.ContainsKey(server)) continue;
                    byte[] tx = new byte[12];
                    lock (s_random)
                    {
                        s_random.NextBytes(tx);
                    }
                    pending[BitConverter.ToString(tx)] = new Pending(server, tx);
                    try
                    {
                        in_socket.SendTo(BuildRequest(tx, in_token), server);
                    }
                    catch (SocketException)
                    {
                        // No route to this address; it simply will not answer
                    }
                }

                long deadline = clock.ElapsedMilliseconds + timeout;
                while (results.Count < in_servers.Count)
                {
                    int wait = (int)(deadline - clock.ElapsedMilliseconds);
                    if (wait <= 0 || !in_socket.Poll(wait * 1000, SelectMode.SelectRead)) break;
                    EndPoint from = new IPEndPoint(in_socket.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6Any : IPAddress.Any, 0);
                    int n;
                    try
                    {
                        n = in_socket.ReceiveFrom(buffer, ref from);
                    }
                    catch (SocketException)
                    {
                        continue; // e.g. an ICMP port-unreachable for an earlier send
                    }
                    if (n < HEADER_LENGTH) continue;
                    Pending sent;
                    if (!pending.TryGetValue(BitConverter.ToString(buffer, 8, 12), out sent)) continue;
                    string error;
                    IPEndPoint mapped = ParseResponse(buffer, n, sent.TransactionId, out error);
                    results[sent.Server] = new StunResult(sent.Server, mapped, error);
                }
                timeout *= 2;
            }

            var ordered = new List<StunResult>();
            foreach (IPEndPoint server in in_servers)
            {
                StunResult result;
                ordered.Add(results.TryGetValue(server, out result) ? result
                    : new StunResult(server, null, "no reply after " + in_attempts + " attempts"));
            }
            return ordered;
        }

        /// <summary>
        /// What kind of NAT the results show: eim, apdm, or unknown when fewer than two addresses
        /// gave us a public address.
        /// </summary>
        public static string ClassifyMapping(IList<StunResult> in_results)
        {
            IPEndPoint first = null;
            int answered = 0;
            bool allSame = true;
            foreach (StunResult result in in_results)
            {
                if (result.Mapped == null) continue;
                ++answered;
                if (first == null) first = result.Mapped;
                else if (!first.Equals(result.Mapped)) allSame = false;
            }
            if (answered < 2) return MAPPING_UNKNOWN;
            return allSame ? MAPPING_EIM : MAPPING_APDM;
        }

        private sealed class Pending
        {
            public Pending(IPEndPoint in_server, byte[] in_transactionId)
            {
                Server = in_server;
                TransactionId = in_transactionId;
            }

            public IPEndPoint Server { get; private set; }
            public byte[] TransactionId { get; private set; }
        }

        private static int readU16(byte[] b, int o) { return (b[o] << 8) | b[o + 1]; }
        private static uint readU32(byte[] b, int o) { return ((uint)b[o] << 24) | ((uint)b[o + 1] << 16) | ((uint)b[o + 2] << 8) | b[o + 3]; }
        private static void writeU16(byte[] b, int o, int v) { b[o] = (byte)(v >> 8); b[o + 1] = (byte)v; }
        private static void writeU32(byte[] b, int o, uint v) { b[o] = (byte)(v >> 24); b[o + 1] = (byte)(v >> 16); b[o + 2] = (byte)(v >> 8); b[o + 3] = (byte)v; }

        private static uint crc32(byte[] in_data, int in_length)
        {
            uint crc = 0xFFFFFFFF;
            for (int i = 0; i < in_length; ++i)
            {
                crc ^= in_data[i];
                for (int k = 0; k < 8; ++k) crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xEDB88320 : crc >> 1;
            }
            return ~crc;
        }
    }
}
