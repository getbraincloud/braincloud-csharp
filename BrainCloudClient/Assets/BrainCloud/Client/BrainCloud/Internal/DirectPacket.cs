// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
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
    ///      3     8  the sender's epoch: random, chosen whenever it starts sending to this peer
    ///     11     8  sequence number, big-endian, from 0 within the epoch
    ///     19     n  the packet, encrypted
    ///   19+n    16  authentication tag
    /// </code>
    ///
    /// Each epoch of each direction between two players has its own key (DeriveKey). The nonce is
    /// 4 zero bytes then the sequence number, and the header is the associated data. A key is never
    /// used with the same nonce twice: the sequence only goes up within an epoch, and a sender that
    /// starts again, for whatever reason, starts a new epoch and so gets a new key.
    ///
    /// Use DirectSender and DirectReceiver, which keep those rules; the static methods here are the
    /// format itself.
    /// </summary>
    internal static class DirectPacket
    {
        public const byte FORMAT = 0xB1;
        public const int EPOCH_LENGTH = 8;
        public const int HEADER_LENGTH = 19;
        public const int OVERHEAD = HEADER_LENGTH + ChaCha20Poly1305.TAG_LENGTH;
        public const int SESSION_TOKEN_LENGTH = 16;

        /// The cipher names this library supports, most preferred first, as reported to brainCloud.
        public const string CIPHER_CHACHA20_POLY1305 = "chacha20-poly1305";
        public static readonly string[] SUPPORTED_CIPHERS = { CIPHER_CHACHA20_POLY1305 };

        private static readonly byte[] KEY_SALT = Encoding.ASCII.GetBytes("brainCloud direct connect");

        /// <summary>
        /// The key for packets from in_sender to in_receiver in the sender's epoch in_epoch:
        /// HKDF-SHA256 with the attempt's 16-byte session token as the input key material,
        /// "brainCloud direct connect" as the salt, and as the info the cipher name, a 0 byte, the
        /// sender's net id, the receiver's net id and the 8 epoch bytes.
        /// </summary>
        public static byte[] DeriveKey(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            if (in_sessionToken == null || in_sessionToken.Length != SESSION_TOKEN_LENGTH) throw new ArgumentException("the session token must be 16 bytes");
            if (in_cipher != CIPHER_CHACHA20_POLY1305) throw new ArgumentException("unsupported cipher: " + in_cipher);
            if (in_epoch == null || in_epoch.Length != EPOCH_LENGTH) throw new ArgumentException("the epoch must be 8 bytes");
            checkNetId(in_sender);
            checkNetId(in_receiver);
            byte[] name = Encoding.ASCII.GetBytes(in_cipher);
            byte[] info = new byte[name.Length + 3 + EPOCH_LENGTH];
            Buffer.BlockCopy(name, 0, info, 0, name.Length);
            info[name.Length] = 0;
            info[name.Length + 1] = (byte)in_sender;
            info[name.Length + 2] = (byte)in_receiver;
            Buffer.BlockCopy(in_epoch, 0, info, name.Length + 3, EPOCH_LENGTH);
            return HmacSha256.Hkdf(KEY_SALT, in_sessionToken, info, ChaCha20Poly1305.KEY_LENGTH);
        }

        /// <summary>The datagram carrying in_length bytes of in_packet from in_offset.</summary>
        public static byte[] Seal(byte[] in_key, int in_sender, int in_receiver, byte[] in_epoch, ulong in_sequence, byte[] in_packet, int in_offset, int in_length)
        {
            checkNetId(in_sender);
            checkNetId(in_receiver);
            if (in_epoch == null || in_epoch.Length != EPOCH_LENGTH) throw new ArgumentException("the epoch must be 8 bytes");
            byte[] header = new byte[HEADER_LENGTH];
            header[0] = FORMAT;
            header[1] = (byte)in_sender;
            header[2] = (byte)in_receiver;
            Buffer.BlockCopy(in_epoch, 0, header, 3, EPOCH_LENGTH);
            for (int i = 0; i < 8; ++i) header[11 + i] = (byte)(in_sequence >> (56 - 8 * i));

            byte[] sealedPacket = ChaCha20Poly1305.Seal(in_key, nonceFor(header), header, in_packet, in_offset, in_length);
            byte[] datagram = new byte[HEADER_LENGTH + sealedPacket.Length];
            Buffer.BlockCopy(header, 0, datagram, 0, HEADER_LENGTH);
            Buffer.BlockCopy(sealedPacket, 0, datagram, HEADER_LENGTH, sealedPacket.Length);
            return datagram;
        }

        /// <summary>
        /// Reads the header of the first in_length bytes of a received datagram. False when they are
        /// not a datagram of this format.
        /// </summary>
        public static bool TryReadHeader(byte[] in_datagram, int in_length, out int out_sender, out int out_receiver, out byte[] out_epoch, out ulong out_sequence)
        {
            out_sender = out_receiver = -1;
            out_epoch = null;
            out_sequence = 0;
            if (in_datagram == null || in_length < OVERHEAD || in_length > in_datagram.Length || in_datagram[0] != FORMAT) return false;
            out_sender = in_datagram[1];
            out_receiver = in_datagram[2];
            out_epoch = new byte[EPOCH_LENGTH];
            Buffer.BlockCopy(in_datagram, 3, out_epoch, 0, EPOCH_LENGTH);
            for (int i = 0; i < 8; ++i) out_sequence = (out_sequence << 8) | in_datagram[11 + i];
            return true;
        }

        /// <summary>
        /// The packet in the first in_length bytes of a received datagram, or null when it is not
        /// from in_expectedSender to in_expectedReceiver or does not decrypt with in_key. The
        /// expected ids are the ones the key belongs to; never take them from the header, or a
        /// datagram could be bounced back to its sender or passed on to another player.
        /// </summary>
        public static byte[] Open(byte[] in_key, int in_expectedSender, int in_expectedReceiver, byte[] in_datagram, int in_length)
        {
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!TryReadHeader(in_datagram, in_length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != in_expectedSender || receiver != in_expectedReceiver) return null;
            byte[] header = new byte[HEADER_LENGTH];
            Buffer.BlockCopy(in_datagram, 0, header, 0, HEADER_LENGTH);
            return ChaCha20Poly1305.Open(in_key, nonceFor(header), header, in_datagram, HEADER_LENGTH, in_length - HEADER_LENGTH);
        }

        /// A new random epoch. It only has to differ from this sender's earlier ones, so a version 4 GUID's randomness is plenty.
        public static byte[] NewEpoch()
        {
            byte[] epoch = new byte[EPOCH_LENGTH];
            Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, epoch, 0, EPOCH_LENGTH);
            return epoch;
        }

        private static byte[] nonceFor(byte[] in_header)
        {
            byte[] nonce = new byte[ChaCha20Poly1305.NONCE_LENGTH];
            Buffer.BlockCopy(in_header, 11, nonce, 4, 8);
            return nonce;
        }

        internal static void checkNetId(int in_netId)
        {
            if (in_netId < 0 || in_netId > 255) throw new ArgumentOutOfRangeException("net id " + in_netId);
        }
    }

    /// <summary>
    /// Everything this player sends to one peer: the key for a new random epoch, and the sequence
    /// number, which only goes up. Create one per peer per connection; a new one is a new epoch.
    /// </summary>
    internal sealed class DirectSender
    {
        private readonly int m_sender;
        private readonly int m_receiver;
        private readonly byte[] m_epoch;
        private readonly byte[] m_key;
        private ulong m_next = 0;

        public DirectSender(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver)
            : this(in_sessionToken, in_cipher, in_sender, in_receiver, DirectPacket.NewEpoch())
        {
        }

        /// For the tests: a fixed epoch.
        internal DirectSender(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            m_sender = in_sender;
            m_receiver = in_receiver;
            m_epoch = (byte[])in_epoch.Clone();
            m_key = DirectPacket.DeriveKey(in_sessionToken, in_cipher, in_sender, in_receiver, m_epoch);
        }

        /// The sequence number the next datagram will carry.
        public ulong NextSequence { get { return m_next; } }

        /// The datagram carrying in_length bytes of in_packet from in_offset.
        public byte[] Seal(byte[] in_packet, int in_offset, int in_length)
        {
            if (m_next == ulong.MaxValue) throw new InvalidOperationException("sequence numbers used up; start a new sender");
            return DirectPacket.Seal(m_key, m_sender, m_receiver, m_epoch, m_next++, in_packet, in_offset, in_length);
        }
    }

    /// <summary>
    /// Everything this player receives from one peer. It accepts only datagrams from that peer to
    /// this player that decrypt, each sequence number once. When the peer starts a new epoch (it
    /// started sending again), the first datagram of it that decrypts moves this receiver to it;
    /// the old epoch is retired and never accepted again, so a recording of it cannot be replayed.
    /// </summary>
    internal sealed class DirectReceiver
    {
        private const int MAX_RETIRED_EPOCHS = 32;

        private readonly byte[] m_sessionToken;
        private readonly string m_cipher;
        private readonly int m_sender;
        private readonly int m_receiver;
        private byte[] m_epoch = null;
        private byte[] m_key = null;
        private ReplayWindow m_window = null;
        private readonly List<ulong> m_retired = new List<ulong>();

        /// in_sender is the peer's net id, in_receiver this player's.
        public DirectReceiver(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver)
        {
            // Checks the token, cipher and ids now rather than on the first datagram
            DirectPacket.DeriveKey(in_sessionToken, in_cipher, in_sender, in_receiver, new byte[DirectPacket.EPOCH_LENGTH]);
            m_sessionToken = (byte[])in_sessionToken.Clone();
            m_cipher = in_cipher;
            m_sender = in_sender;
            m_receiver = in_receiver;
        }

        /// <summary>
        /// The packet in the first in_length bytes of a received datagram, or null when it is to be
        /// dropped: not from this peer to this player, a repeat, from a retired epoch, or not
        /// decrypting.
        /// </summary>
        public byte[] Open(byte[] in_datagram, int in_length)
        {
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(in_datagram, in_length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != m_sender || receiver != m_receiver) return null;

            if (m_epoch != null && sameEpoch(epoch, m_epoch))
            {
                if (!m_window.IsNew(sequence)) return null;
                byte[] packet = DirectPacket.Open(m_key, m_sender, m_receiver, in_datagram, in_length);
                if (packet != null) m_window.Accept(sequence);
                return packet;
            }

            if (m_retired.Contains(epochValue(epoch))) return null;

            // A new epoch: only a datagram that decrypts with its key may move this receiver to it
            byte[] key = DirectPacket.DeriveKey(m_sessionToken, m_cipher, m_sender, m_receiver, epoch);
            byte[] opened = DirectPacket.Open(key, m_sender, m_receiver, in_datagram, in_length);
            if (opened == null) return null;
            if (m_epoch != null)
            {
                m_retired.Add(epochValue(m_epoch));
                if (m_retired.Count > MAX_RETIRED_EPOCHS) m_retired.RemoveAt(0);
            }
            m_epoch = epoch;
            m_key = key;
            m_window = new ReplayWindow();
            m_window.Accept(sequence);
            return opened;
        }

        private static bool sameEpoch(byte[] a, byte[] b)
        {
            for (int i = 0; i < DirectPacket.EPOCH_LENGTH; ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static ulong epochValue(byte[] in_epoch)
        {
            ulong v = 0;
            for (int i = 0; i < DirectPacket.EPOCH_LENGTH; ++i) v = (v << 8) | in_epoch[i];
            return v;
        }
    }

    /// <summary>
    /// Which sequence numbers have been received in one epoch from one peer, so a repeated or
    /// replayed datagram is dropped: the highest seen and a bit for each of the 63 before it, as
    /// DTLS does. Anything older than that is dropped too. Check before decrypting, and Accept only
    /// after a datagram has decrypted, so a forged one cannot move the window. DirectReceiver keeps
    /// one per epoch, created with the epoch's key.
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
                // A shift by 64 or more is masked to the low 6 bits in C#, so it must not be shifted at all
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
