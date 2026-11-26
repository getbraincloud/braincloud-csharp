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
    ///      3     8  the sender's epoch: 4 random bytes, the same for every sender in this
    ///               process, then a 4-byte big-endian count that goes up with every new sender
    ///     11     8  sequence number, big-endian, from 0 within the epoch
    ///     19     n  the packet, encrypted
    ///   19+n    16  authentication tag
    /// </code>
    ///
    /// Each epoch of each direction between two players has its own key (DeriveKey). The nonce is
    /// 4 zero bytes then the sequence number, and the header is the associated data. A key is never
    /// used with the same nonce twice: the sequence only goes up within an epoch, and a sender that
    /// starts again, for whatever reason, starts a new epoch and so gets a new key. Epochs are
    /// ordered, so a receiver only ever moves forward (see DirectReceiver).
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
            return DeriveKeyFromExtracted(ExtractKey(in_sessionToken), in_cipher, in_sender, in_receiver, in_epoch);
        }

        /// The first, epoch-independent step of DeriveKey, which a receiver can do once.
        public static byte[] ExtractKey(byte[] in_sessionToken)
        {
            if (in_sessionToken == null || in_sessionToken.Length != SESSION_TOKEN_LENGTH) throw new ArgumentException("the session token must be 16 bytes");
            return HmacSha256.HkdfExtract(KEY_SALT, in_sessionToken);
        }

        /// DeriveKey from the result of ExtractKey.
        public static byte[] DeriveKeyFromExtracted(byte[] in_extracted, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
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
            return HmacSha256.HkdfExpand(in_extracted, info, ChaCha20Poly1305.KEY_LENGTH);
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

        // This process's epoch prefix, and how many senders it has made
        private static readonly byte[] s_epochPrefix = newPrefix();
        private static int s_epochCount = 0;

        /// <summary>
        /// The next epoch of this process: its random prefix, then a count that goes up every time.
        /// The prefix only has to differ from other processes' and the count makes every epoch in
        /// this one unique, whatever the platform's random source does.
        /// </summary>
        public static byte[] NewEpoch()
        {
            uint count = (uint)System.Threading.Interlocked.Increment(ref s_epochCount);
            if (count == 0) throw new InvalidOperationException("epochs used up");
            return MakeEpoch(s_epochPrefix, count);
        }

        /// An epoch from its two parts.
        public static byte[] MakeEpoch(byte[] in_prefix, uint in_count)
        {
            byte[] epoch = new byte[EPOCH_LENGTH];
            Buffer.BlockCopy(in_prefix, 0, epoch, 0, 4);
            epoch[4] = (byte)(in_count >> 24);
            epoch[5] = (byte)(in_count >> 16);
            epoch[6] = (byte)(in_count >> 8);
            epoch[7] = (byte)in_count;
            return epoch;
        }

        internal static bool SamePrefix(byte[] a, byte[] b)
        {
            return a[0] == b[0] && a[1] == b[1] && a[2] == b[2] && a[3] == b[3];
        }

        internal static uint EpochCount(byte[] in_epoch)
        {
            return ((uint)in_epoch[4] << 24) | ((uint)in_epoch[5] << 16) | ((uint)in_epoch[6] << 8) | in_epoch[7];
        }

        private static byte[] newPrefix()
        {
            byte[] prefix = new byte[4];
            Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, prefix, 0, 4);
            return prefix;
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
    /// Everything this player sends to one peer: the key for a new epoch, and the sequence number,
    /// which only goes up. Create one per peer per connection; a new one is a new epoch. Safe to
    /// use from more than one thread.
    /// </summary>
    internal sealed class DirectSender
    {
        private readonly int m_sender;
        private readonly int m_receiver;
        private readonly byte[] m_epoch;
        private readonly byte[] m_key;
        private readonly object m_lock = new object();
        private ulong m_next = 0;

        public DirectSender(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver)
            : this(in_sessionToken, in_cipher, in_sender, in_receiver, DirectPacket.NewEpoch())
        {
        }

        /// <summary>
        /// For the tests only: a chosen epoch. Anything else must use the other constructor, since
        /// reusing an epoch with the same token reuses nonces.
        /// </summary>
        internal DirectSender(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            m_sender = in_sender;
            m_receiver = in_receiver;
            m_epoch = (byte[])in_epoch.Clone();
            m_key = DirectPacket.DeriveKey(in_sessionToken, in_cipher, in_sender, in_receiver, m_epoch);
        }

        /// The sequence number the next datagram will carry.
        public ulong NextSequence
        {
            get { lock (m_lock) return m_next; }
        }

        /// The datagram carrying in_length bytes of in_packet from in_offset.
        public byte[] Seal(byte[] in_packet, int in_offset, int in_length)
        {
            ulong sequence;
            lock (m_lock)
            {
                // Two threads must never get the same sequence number: that would be the same nonce
                if (m_next == ulong.MaxValue) throw new InvalidOperationException("sequence numbers used up; start a new sender");
                sequence = m_next++;
            }
            return DirectPacket.Seal(m_key, m_sender, m_receiver, m_epoch, sequence, in_packet, in_offset, in_length);
        }
    }

    /// <summary>
    /// Everything this player receives from one peer. It accepts only datagrams from that peer to
    /// this player that decrypt, each sequence number once, and only ever moves forward:
    /// <list type="bullet">
    /// <item>The first datagram that decrypts fixes the peer's epoch prefix. A peer keeps its
    /// prefix for the life of its process, and a process that starts again cannot rejoin the same
    /// attempt (a new attempt has a new token), so another prefix is never genuine and is dropped
    /// without deriving any key.</item>
    /// <item>A higher count than the current epoch's means the peer started sending again: the
    /// first datagram of it that decrypts moves this receiver to it. A lower or equal count is an
    /// old epoch, and is dropped without deriving any key.</item>
    /// </list>
    /// Not thread-safe: one thread receives from a peer.
    /// </summary>
    internal sealed class DirectReceiver
    {
        private readonly byte[] m_extracted;
        private readonly string m_cipher;
        private readonly int m_sender;
        private readonly int m_receiver;
        private byte[] m_epoch = null;
        private byte[] m_key = null;
        private ReplayWindow m_window = null;

        /// in_sender is the peer's net id, in_receiver this player's.
        public DirectReceiver(byte[] in_sessionToken, string in_cipher, int in_sender, int in_receiver)
        {
            m_extracted = DirectPacket.ExtractKey(in_sessionToken);
            // Checks the cipher and ids now rather than on the first datagram
            DirectPacket.DeriveKeyFromExtracted(m_extracted, in_cipher, in_sender, in_receiver, new byte[DirectPacket.EPOCH_LENGTH]);
            m_cipher = in_cipher;
            m_sender = in_sender;
            m_receiver = in_receiver;
        }

        /// <summary>
        /// The packet in the first in_length bytes of a received datagram, or null when it is to be
        /// dropped: not from this peer to this player, a repeat, from an earlier epoch or another
        /// prefix, or not decrypting.
        /// </summary>
        public byte[] Open(byte[] in_datagram, int in_length)
        {
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(in_datagram, in_length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != m_sender || receiver != m_receiver) return null;

            if (m_epoch != null)
            {
                if (!DirectPacket.SamePrefix(epoch, m_epoch)) return null;
                uint count = DirectPacket.EpochCount(epoch), current = DirectPacket.EpochCount(m_epoch);
                if (count < current) return null;
                if (count == current)
                {
                    if (!m_window.IsNew(sequence)) return null;
                    byte[] packet = DirectPacket.Open(m_key, m_sender, m_receiver, in_datagram, in_length);
                    if (packet != null) m_window.Accept(sequence);
                    return packet;
                }
            }

            // The first epoch, or a later one: only a datagram that decrypts with its key moves this receiver to it
            byte[] key = DirectPacket.DeriveKeyFromExtracted(m_extracted, m_cipher, m_sender, m_receiver, epoch);
            byte[] opened = DirectPacket.Open(key, m_sender, m_receiver, in_datagram, in_length);
            if (opened == null) return null;
            m_epoch = epoch;
            m_key = key;
            m_window = new ReplayWindow();
            m_window.Accept(sequence);
            return opened;
        }
    }

    /// <summary>
    /// Which sequence numbers have been received in one epoch from one peer, so a repeated or
    /// replayed datagram is dropped: the highest seen and a bit for each of the 63 before it, as
    /// DTLS does. Anything older than that is dropped too. Check before decrypting, and Accept only
    /// after a datagram has decrypted, so a forged one cannot move the window. DirectReceiver keeps
    /// one, for its current epoch, and starts a new one when it moves to a later epoch.
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
