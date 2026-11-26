// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Text;

    /// <summary>
    /// The datagram players send each other when connected directly: a short header, then the
    /// packet encrypted and authenticated with the header. Every client library must produce these
    /// byte for byte; the format is section 7 of the client design (Confluence BCSD 4389666817).
    ///
    /// <code>
    /// offset  size  field
    ///      0     1  FORMAT (0xB1): brainCloud direct connect, format 1
    ///      1     1  sender's net id
    ///      2     1  receiver's net id
    ///      3     8  sequence number, big-endian
    ///     11     n  the packet, encrypted
    ///   11+n    16  authentication tag
    /// </code>
    ///
    /// The nonce is 4 zero bytes then the sequence number, and the header is the associated data.
    /// Each direction between each pair of players has its own key (DeriveKey), so a counter that
    /// never repeats within a direction keeps every nonce unique.
    /// </summary>
    internal static class DirectPacket
    {
        public const byte FORMAT = 0xB1;
        public const int HEADER_LENGTH = 11;
        public const int OVERHEAD = HEADER_LENGTH + ChaCha20Poly1305.TAG_LENGTH;

        /// The cipher names this library supports, most preferred first, as reported to brainCloud.
        public const string CIPHER_CHACHA20_POLY1305 = "chacha20-poly1305";
        public static readonly string[] SUPPORTED_CIPHERS = { CIPHER_CHACHA20_POLY1305 };

        private static readonly byte[] KEY_SALT = Encoding.ASCII.GetBytes("brainCloud direct connect");

        /// <summary>
        /// The key for packets from in_sender to in_receiver: HKDF-SHA256 with the attempt's session
        /// token (16 bytes) as the input key material, "brainCloud direct connect" as the salt, and
        /// the cipher name, a 0 byte, the sender's net id and the receiver's net id as the info.
        /// </summary>
        public static byte[] DeriveKey(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver)
        {
            if (in_sessionToken == null || in_sessionToken.Length == 0) throw new ArgumentException("a session token is needed");
            if (in_cipher != CIPHER_CHACHA20_POLY1305) throw new ArgumentException("unsupported cipher: " + in_cipher);
            checkNetId(in_sender);
            checkNetId(in_receiver);
            byte[] name = Encoding.ASCII.GetBytes(in_cipher);
            byte[] info = new byte[name.Length + 3];
            Buffer.BlockCopy(name, 0, info, 0, name.Length);
            info[name.Length] = 0;
            info[name.Length + 1] = (byte)in_sender;
            info[name.Length + 2] = (byte)in_receiver;
            return HmacSha256.Hkdf(KEY_SALT, in_sessionToken, info, ChaCha20Poly1305.KEY_LENGTH);
        }

        /// <summary>The datagram carrying in_length bytes of in_packet from in_offset.</summary>
        public static byte[] Seal(byte[] in_key, int in_sender, int in_receiver, ulong in_sequence, byte[] in_packet, int in_offset, int in_length)
        {
            checkNetId(in_sender);
            checkNetId(in_receiver);
            byte[] header = new byte[HEADER_LENGTH];
            header[0] = FORMAT;
            header[1] = (byte)in_sender;
            header[2] = (byte)in_receiver;
            for (int i = 0; i < 8; ++i) header[3 + i] = (byte)(in_sequence >> (56 - 8 * i));

            byte[] sealedPacket = ChaCha20Poly1305.Seal(in_key, nonceFor(header), header, in_packet, in_offset, in_length);
            byte[] datagram = new byte[HEADER_LENGTH + sealedPacket.Length];
            Buffer.BlockCopy(header, 0, datagram, 0, HEADER_LENGTH);
            Buffer.BlockCopy(sealedPacket, 0, datagram, HEADER_LENGTH, sealedPacket.Length);
            return datagram;
        }

        /// <summary>
        /// Reads the header of a received datagram, so the receiver can pick the key and check the
        /// sequence number before decrypting. False when it is not a datagram of this format.
        /// </summary>
        public static bool TryReadHeader(byte[] in_datagram, int in_length, out int out_sender, out int out_receiver, out ulong out_sequence)
        {
            out_sender = out_receiver = -1;
            out_sequence = 0;
            if (in_datagram == null || in_length < OVERHEAD || in_datagram[0] != FORMAT) return false;
            out_sender = in_datagram[1];
            out_receiver = in_datagram[2];
            for (int i = 0; i < 8; ++i) out_sequence = (out_sequence << 8) | in_datagram[3 + i];
            return true;
        }

        /// <summary>
        /// The packet in a received datagram, or null when it does not decrypt with in_key: it was
        /// not sealed with that key, or the header or anything after it was changed.
        /// </summary>
        public static byte[] Open(byte[] in_key, byte[] in_datagram, int in_length)
        {
            int sender, receiver;
            ulong sequence;
            if (!TryReadHeader(in_datagram, in_length, out sender, out receiver, out sequence)) return null;
            byte[] header = new byte[HEADER_LENGTH];
            Buffer.BlockCopy(in_datagram, 0, header, 0, HEADER_LENGTH);
            return ChaCha20Poly1305.Open(in_key, nonceFor(header), header, in_datagram, HEADER_LENGTH, in_length - HEADER_LENGTH);
        }

        private static byte[] nonceFor(byte[] in_header)
        {
            byte[] nonce = new byte[ChaCha20Poly1305.NONCE_LENGTH];
            Buffer.BlockCopy(in_header, 3, nonce, 4, 8);
            return nonce;
        }

        private static void checkNetId(int in_netId)
        {
            if (in_netId < 0 || in_netId > 255) throw new ArgumentOutOfRangeException("net id " + in_netId);
        }
    }

    /// <summary>
    /// Which sequence numbers have been received from one peer, so a repeated or replayed datagram
    /// is dropped: the highest seen and a bit for each of the 63 before it, as DTLS does. Anything
    /// older than that is dropped too. Check before decrypting, and Accept only after a datagram
    /// has decrypted, so a forged one cannot move the window.
    /// </summary>
    internal sealed class ReplayWindow
    {
        public const int SIZE = 64;

        private ulong m_highest = 0;
        private ulong m_seen = 0;   // bit i: m_highest - i has been received
        private bool m_any = false;

        /// True when in_sequence has not been received and is not too old.
        public bool IsNew(ulong in_sequence)
        {
            if (!m_any || in_sequence > m_highest) return true;
            ulong behind = m_highest - in_sequence;
            if (behind >= SIZE) return false;
            return (m_seen & (1UL << (int)behind)) == 0;
        }

        /// Records in_sequence as received.
        public void Accept(ulong in_sequence)
        {
            if (!m_any)
            {
                m_any = true;
                m_highest = in_sequence;
                m_seen = 1;
                return;
            }
            if (in_sequence > m_highest)
            {
                ulong ahead = in_sequence - m_highest;
                m_seen = ahead >= SIZE ? 0 : m_seen << (int)ahead;
                m_seen |= 1;
                m_highest = in_sequence;
                return;
            }
            ulong behind = m_highest - in_sequence;
            if (behind < SIZE) m_seen |= 1UL << (int)behind;
        }
    }
}
