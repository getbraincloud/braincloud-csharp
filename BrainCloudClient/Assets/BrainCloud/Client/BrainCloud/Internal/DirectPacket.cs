// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Text;

    /// <summary>
    /// The datagram players send each other when connected directly. Every client library must
    /// produce it byte for byte. chacha20-poly1305 encrypts the packet; hmac-sha256 only
    /// authenticates it, leaving it readable as it is through a relay server.
    ///
    /// <code>
    /// offset  size  field
    ///      0     1  FORMAT (0xB1): brainCloud direct connect, format 1
    ///      1     1  sender's net id
    ///      2     1  receiver's net id
    ///      3     8  sender's epoch: 4 random bytes, fixed for this player's attempt, then a
    ///               4-byte big-endian count that goes up with every new sender
    ///     11     8  sequence number, big-endian, from 0 within the epoch
    ///     19     n  packet: encrypted with chacha20-poly1305, unchanged with hmac-sha256
    ///   19+n    16  authentication tag
    /// </code>
    ///
    /// Each pair of players has its own secret from brainCloud, so no member can make or read a
    /// datagram between two others. Each epoch of each direction has its own key (DeriveKey),
    /// bound to the cipher name. chacha20-poly1305: nonce = 4 zero bytes then the sequence number;
    /// associated data = the header. hmac-sha256: tag = first 16 bytes of HMAC-SHA256 of header
    /// then packet. A key never reuses a nonce: the sequence only goes up within an epoch, and a
    /// restarted sender starts a new epoch, so gets a new key. Receivers only move forward through
    /// epochs (DirectReceiver). DirectSession enforces these rules; the static methods are the format.
    /// </summary>
    internal static class DirectPacket
    {
        public const byte FORMAT = 0xB1;
        public const int EPOCH_LENGTH = 8;
        public const int HEADER_LENGTH = 19;
        public const int KEY_LENGTH = 32;
        public const int NONCE_LENGTH = 12;
        public const int TAG_LENGTH = 16;
        public const int OVERHEAD = HEADER_LENGTH + TAG_LENGTH;
        /// <summary>
        /// Largest datagram sent or accepted: the UDP payload that fits IPv6's 1,280-byte minimum MTU.
        /// A full game packet is 1,068 bytes. Larger datagrams are dropped before any decrypting.
        /// </summary>
        public const int MAX_DATAGRAM_LENGTH = 1232;
        /// Length of the secret brainCloud gives each pair of players (DIRECT_CONNECT peers[].pairKey).
        public const int PAIR_KEY_LENGTH = 16;

        /// Encrypts and authenticates. Available only when an implementation is included (DirectCipher).
        public const string CIPHER_CHACHA20_POLY1305 = "chacha20-poly1305";
        /// Authenticates only: packets cannot be forged, changed or replayed, but are readable on the path.
        public const string CIPHER_HMAC_SHA256 = "hmac-sha256";

        /// <summary>Ciphers this player can use, most preferred first, reported with its network test.</summary>
        public static System.Collections.ObjectModel.ReadOnlyCollection<string> SupportedCiphers
        {
            get
            {
                var names = new System.Collections.Generic.List<string>();
                foreach (string name in new[] { CIPHER_CHACHA20_POLY1305, CIPHER_HMAC_SHA256 })
                {
                    if (IsSupported(name)) names.Add(name);
                }
                return names.AsReadOnly();
            }
        }

        /// Whether this player can use in_cipher. Never throws.
        public static bool IsSupported(string in_cipher)
        {
            return DirectCipher.For(in_cipher) != null;
        }

        private static readonly byte[] KEY_SALT = Encoding.ASCII.GetBytes("brainCloud direct connect");

        /// <summary>
        /// Key for in_sender to in_receiver in the sender's epoch: HKDF-SHA256 with the 16-byte pair key
        /// as input key material, salt "brainCloud direct connect", and info = cipher name, a 0 byte,
        /// sender net id, receiver net id, the 8 epoch bytes.
        /// </summary>
        public static byte[] DeriveKey(byte[] in_pairKey, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            return DeriveKeyFromExtracted(ExtractKey(in_pairKey), in_cipher, in_sender, in_receiver, in_epoch);
        }

        /// The epoch-independent first step of DeriveKey, which a receiver can do once.
        public static byte[] ExtractKey(byte[] in_pairKey)
        {
            if (in_pairKey == null || in_pairKey.Length != PAIR_KEY_LENGTH) throw new ArgumentException("a pair key must be 16 bytes");
            return HmacSha256.HkdfExtract(KEY_SALT, in_pairKey);
        }

        /// DeriveKey from the result of ExtractKey.
        public static byte[] DeriveKeyFromExtracted(byte[] in_extracted, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            if (!IsSupported(in_cipher)) throw new ArgumentException("unsupported cipher: " + in_cipher);
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
            return HmacSha256.HkdfExpand(in_extracted, info, KEY_LENGTH);
        }

        /// <summary>The datagram carrying in_length bytes of in_packet from in_offset.</summary>
        public static byte[] Seal(string in_cipher, byte[] in_key, int in_sender, int in_receiver, byte[] in_epoch, ulong in_sequence, byte[] in_packet, int in_offset, int in_length)
        {
            checkNetId(in_sender);
            checkNetId(in_receiver);
            if (in_epoch == null || in_epoch.Length != EPOCH_LENGTH) throw new ArgumentException("the epoch must be 8 bytes");
            if (in_length > MAX_DATAGRAM_LENGTH - OVERHEAD) throw new ArgumentException("too long for one datagram: " + in_length + " bytes");
            byte[] header = new byte[HEADER_LENGTH];
            header[0] = FORMAT;
            header[1] = (byte)in_sender;
            header[2] = (byte)in_receiver;
            Buffer.BlockCopy(in_epoch, 0, header, 3, EPOCH_LENGTH);
            for (int i = 0; i < 8; ++i) header[11 + i] = (byte)(in_sequence >> (56 - 8 * i));

            byte[] sealedPacket = implementation(in_cipher).Seal(in_key, nonceFor(header), header, in_packet, in_offset, in_length);
            byte[] datagram = new byte[HEADER_LENGTH + sealedPacket.Length];
            Buffer.BlockCopy(header, 0, datagram, 0, HEADER_LENGTH);
            Buffer.BlockCopy(sealedPacket, 0, datagram, HEADER_LENGTH, sealedPacket.Length);
            return datagram;
        }

        /// <summary>Reads the header of a received datagram's first in_length bytes; false if not this format.</summary>
        public static bool TryReadHeader(byte[] in_datagram, int in_length, out int out_sender, out int out_receiver, out byte[] out_epoch, out ulong out_sequence)
        {
            out_sender = out_receiver = -1;
            out_epoch = null;
            out_sequence = 0;
            if (in_datagram == null || in_length < OVERHEAD || in_length > MAX_DATAGRAM_LENGTH || in_length > in_datagram.Length || in_datagram[0] != FORMAT) return false;
            out_sender = in_datagram[1];
            out_receiver = in_datagram[2];
            out_epoch = new byte[EPOCH_LENGTH];
            Buffer.BlockCopy(in_datagram, 3, out_epoch, 0, EPOCH_LENGTH);
            for (int i = 0; i < 8; ++i) out_sequence = (out_sequence << 8) | in_datagram[11 + i];
            return true;
        }

        /// <summary>
        /// The packet in a received datagram, or null if it is not from in_expectedSender to
        /// in_expectedReceiver or does not open. Take the expected ids from the key, never the header,
        /// or a datagram could be bounced back to its sender or passed on to another player.
        /// </summary>
        public static byte[] Open(string in_cipher, byte[] in_key, int in_expectedSender, int in_expectedReceiver, byte[] in_datagram, int in_length)
        {
            byte[] datagram = CopyReceived(in_datagram, in_length);
            return datagram == null ? null : OpenCopy(in_cipher, in_key, in_expectedSender, in_expectedReceiver, datagram);
        }

        /// <summary>
        /// A copy of a received datagram's first in_length bytes, or null for a bad length. Everything
        /// after reads only the copy, so a changing receive buffer cannot split checked from used bytes.
        /// </summary>
        internal static byte[] CopyReceived(byte[] in_datagram, int in_length)
        {
            if (in_datagram == null || in_length < OVERHEAD || in_length > MAX_DATAGRAM_LENGTH || in_length > in_datagram.Length) return null;
            byte[] datagram = new byte[in_length];
            Buffer.BlockCopy(in_datagram, 0, datagram, 0, in_length);
            return datagram;
        }

        /// Open, for a datagram already copied with CopyReceived.
        internal static byte[] OpenCopy(string in_cipher, byte[] in_key, int in_expectedSender, int in_expectedReceiver, byte[] in_datagram)
        {
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!TryReadHeader(in_datagram, in_datagram.Length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != in_expectedSender || receiver != in_expectedReceiver) return null;
            byte[] header = new byte[HEADER_LENGTH];
            Buffer.BlockCopy(in_datagram, 0, header, 0, HEADER_LENGTH);
            return implementation(in_cipher).Open(in_key, nonceFor(header), header, in_datagram, HEADER_LENGTH, in_datagram.Length - HEADER_LENGTH);
        }

        // The datagram does not name its cipher: the whole attempt uses the one the server chose.
        private static IDirectAead implementation(string in_cipher)
        {
            IDirectAead aead = DirectCipher.For(in_cipher);
            if (aead == null) throw new ArgumentException("unsupported cipher: " + in_cipher);
            return aead;
        }

        /// <summary>A new random epoch prefix, per attempt rather than per process, so sessions cannot be linked.</summary>
        public static byte[] RandomPrefix()
        {
            // A cryptographic generator, not a Guid (some platforms make Guids from the time): a
            // guessable prefix would link a player's attempts.
            byte[] prefix = new byte[4];
            using (var random = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                random.GetBytes(prefix);
            }
            return prefix;
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

        private static byte[] nonceFor(byte[] in_header)
        {
            byte[] nonce = new byte[NONCE_LENGTH];
            Buffer.BlockCopy(in_header, 11, nonce, 4, 8);
            return nonce;
        }

        internal static void checkNetId(int in_netId)
        {
            if (in_netId < 0 || in_netId > 255) throw new ArgumentOutOfRangeException("net id " + in_netId);
        }
    }

    /// <summary>
    /// A cipher as direct connect uses it: 32-byte key, 12-byte nonce, header as associated data,
    /// 16-byte tag after the packet. Implemented here or by the operating system (DirectCipher).
    /// </summary>
    internal interface IDirectAead
    {
        /// The protected form of in_length bytes of in_plaintext from in_offset, then the tag.
        byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length);

        /// The packet from in_length bytes of in_sealed at in_offset (packet and tag), or null if the tag does not match.
        byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length);
    }

    /// <summary>
    /// hmac-sha256: the packet in the clear, then the first 16 bytes of HMAC-SHA256 of header then
    /// packet. Always available. The nonce is unused: the header already carries the sequence number.
    /// </summary>
    internal sealed class HmacSha256Authentication : IDirectAead
    {
        public byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length)
        {
            if (in_plaintext == null || in_offset < 0 || in_length < 0 || in_length > in_plaintext.Length - in_offset) throw new ArgumentException("bad packet range");
            byte[] message = macInput(in_aad, in_plaintext, in_offset, in_length);
            byte[] result = new byte[in_length + DirectPacket.TAG_LENGTH];
            Buffer.BlockCopy(message, in_aad.Length, result, 0, in_length);
            Buffer.BlockCopy(mac(in_key, message), 0, result, in_length, DirectPacket.TAG_LENGTH);
            return result;
        }

        public byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length)
        {
            if (in_sealed == null || in_offset < 0 || in_length < DirectPacket.TAG_LENGTH || in_length > in_sealed.Length - in_offset) return null;
            int packetLength = in_length - DirectPacket.TAG_LENGTH;
            // Check and return the same copy, so bytes changed in the caller's buffer after the
            // check are never returned as authenticated
            byte[] message = macInput(in_aad, in_sealed, in_offset, packetLength);
            byte[] expected = mac(in_key, message);
            // Constant-time compare: the timing reveals nothing about the tag
            int difference = 0;
            for (int i = 0; i < DirectPacket.TAG_LENGTH; ++i) difference |= expected[i] ^ in_sealed[in_offset + packetLength + i];
            if (difference != 0) return null;
            byte[] packet = new byte[packetLength];
            Buffer.BlockCopy(message, in_aad.Length, packet, 0, packetLength);
            return packet;
        }

        // The header followed by the packet: what the tag covers
        private static byte[] macInput(byte[] in_aad, byte[] in_packet, int in_offset, int in_length)
        {
            byte[] message = new byte[in_aad.Length + in_length];
            Buffer.BlockCopy(in_aad, 0, message, 0, in_aad.Length);
            Buffer.BlockCopy(in_packet, in_offset, message, in_aad.Length, in_length);
            return message;
        }

        // HMAC-SHA256 of the message; its first TAG_LENGTH bytes are the tag
        private static byte[] mac(byte[] in_key, byte[] in_message)
        {
            if (in_key == null || in_key.Length != DirectPacket.KEY_LENGTH) throw new ArgumentException("the key must be 32 bytes");
            return HmacSha256.Compute(in_key, in_message);
        }
    }

#if BRAINCLOUD_DIRECT_CONNECT_ENCRYPTION
    /// ChaCha20-Poly1305 as implemented in this library.
    internal sealed class ManagedChaCha20Poly1305 : IDirectAead
    {
        public byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length)
        {
            return ChaCha20Poly1305.Seal(in_key, in_nonce, in_aad, in_plaintext, in_offset, in_length);
        }

        public byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length)
        {
            return ChaCha20Poly1305.Open(in_key, in_nonce, in_aad, in_sealed, in_offset, in_length);
        }
    }
#endif

    /// <summary>
    /// Which implementation protects direct-connect traffic with each cipher. hmac-sha256 (and HKDF
    /// key derivation, which does not encrypt) is always available. chacha20-poly1305 needs an
    /// implementation: this library's is compiled in only when the game defines the scripting symbol
    /// BRAINCLOUD_DIRECT_CONNECT_ENCRYPTION. Without it the library has no encryption code, so a game
    /// need not declare any to app stores (as Apple asks) unless it chose to. Define it for each
    /// platform to use encrypted direct connect, which some lobby types require.
    /// </summary>
    internal static class DirectCipher
    {
        private static readonly IDirectAead s_hmacSha256 = new HmacSha256Authentication();
#if BRAINCLOUD_DIRECT_CONNECT_ENCRYPTION
        private static volatile IDirectAead s_chaCha20Poly1305 = new ManagedChaCha20Poly1305();
#else
        private static volatile IDirectAead s_chaCha20Poly1305 = null;
#endif

        /// The implementation of in_cipher, or null when there is none.
        public static IDirectAead For(string in_cipher)
        {
            if (in_cipher == DirectPacket.CIPHER_CHACHA20_POLY1305) return s_chaCha20Poly1305;
            if (in_cipher == DirectPacket.CIPHER_HMAC_SHA256) return s_hmacSha256;
            return null;
        }

        /// <summary>
        /// Replaces the ChaCha20-Poly1305 implementation (null removes it), e.g. with an OS-backed one
        /// or for tests. Set it at startup, before any attempt; datagrams are identical either way.
        /// </summary>
        internal static IDirectAead ChaCha20Poly1305Implementation
        {
            get { return s_chaCha20Poly1305; }
            set { s_chaCha20Poly1305 = value; }
        }
    }

    /// <summary>
    /// What this player sends to one peer in one epoch: the key and a rising sequence number. Get one
    /// from DirectSession, which gives each a new epoch and retires the one it replaces. Thread-safe.
    /// </summary>
    internal sealed class DirectSender
    {
        private readonly string m_cipher;
        private readonly int m_sender;
        private readonly int m_receiver;
        private readonly byte[] m_epoch;
        private readonly byte[] m_key;
        private readonly object m_lock = new object();
        private ulong m_next = 0;
        private bool m_retired = false;

        /// <summary>For DirectSession and tests only: reusing an epoch with the same pair key reuses nonces.</summary>
        internal DirectSender(byte[] in_pairKey, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            m_cipher = in_cipher;
            m_sender = in_sender;
            m_receiver = in_receiver;
            m_epoch = (byte[])in_epoch.Clone();
            m_key = DirectPacket.DeriveKey(in_pairKey, in_cipher, in_sender, in_receiver, m_epoch);
        }

        public int Receiver { get { return m_receiver; } }

        /// The sequence number the next datagram will carry.
        public ulong NextSequence
        {
            get { lock (m_lock) return m_next; }
        }

        /// <summary>
        /// The datagram carrying in_length bytes of in_packet from in_offset. Throws once this sender
        /// is replaced: the peer only moves forward, so would drop it anyway.
        /// </summary>
        public byte[] Seal(byte[] in_packet, int in_offset, int in_length)
        {
            ulong sequence;
            lock (m_lock)
            {
                if (m_retired) throw new InvalidOperationException("this sender was replaced by a newer one");
                // A sequence number used twice would reuse a nonce
                if (m_next == ulong.MaxValue) throw new InvalidOperationException("sequence numbers used up; start a new sender");
                sequence = m_next++;
            }
            return DirectPacket.Seal(m_cipher, m_key, m_sender, m_receiver, m_epoch, sequence, in_packet, in_offset, in_length);
        }

        /// Stops this sender for good, and overwrites its key.
        internal void Retire()
        {
            lock (m_lock)
            {
                m_retired = true;
                Array.Clear(m_key, 0, m_key.Length);
            }
        }
    }

    /// <summary>
    /// What this player receives from one peer. Accepts only datagrams from that peer to this player
    /// that decrypt, each sequence number once, and only moves forward:
    /// <list type="bullet">
    /// <item>The first datagram that decrypts fixes the peer's epoch prefix. A peer keeps it for the
    /// whole attempt and cannot rejoin one, so any other prefix is dropped without deriving a key.</item>
    /// <item>A higher epoch count means the peer restarted sending; its first datagram that decrypts
    /// moves this receiver to it. A lower count is dropped without deriving a key.</item>
    /// </list>
    /// Exactly one per peer per attempt (DirectSession ensures it): a new one would accept replays.
    /// Thread-safe. Never move a peer to a new address because a datagram from there decrypted:
    /// anyone can resend a genuine one from anywhere.
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
        private bool m_wiped = false;
        private readonly object m_lock = new object();

        /// in_sender is the peer's net id, in_receiver this player's.
        internal DirectReceiver(byte[] in_pairKey, string in_cipher, int in_sender, int in_receiver)
        {
            m_extracted = DirectPacket.ExtractKey(in_pairKey);
            // Fails now, not on the first datagram, for a bad cipher or id
            DirectPacket.DeriveKeyFromExtracted(m_extracted, in_cipher, in_sender, in_receiver, new byte[DirectPacket.EPOCH_LENGTH]);
            m_cipher = in_cipher;
            m_sender = in_sender;
            m_receiver = in_receiver;
        }

        /// <summary>
        /// The packet in a received datagram, or null to drop it: wrong sender or receiver, a repeat,
        /// an earlier epoch or other prefix, or not decrypting.
        /// </summary>
        public byte[] Open(byte[] in_datagram, int in_length)
        {
            // Copy first, so the epoch and sequence the replay window records are the ones the tag covered
            byte[] datagram = DirectPacket.CopyReceived(in_datagram, in_length);
            if (datagram == null) return null;
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(datagram, datagram.Length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != m_sender || receiver != m_receiver) return null;
            lock (m_lock)
            {
                if (m_wiped) return null;
                return open(datagram, epoch, sequence);
            }
        }

        /// Overwrites this receiver's keys; it opens nothing afterwards.
        internal void Wipe()
        {
            lock (m_lock)
            {
                m_wiped = true;
                Array.Clear(m_extracted, 0, m_extracted.Length);
                if (m_key != null) Array.Clear(m_key, 0, m_key.Length);
            }
        }

        private byte[] open(byte[] datagram, byte[] epoch, ulong sequence)
        {
            if (m_epoch != null)
            {
                if (!DirectPacket.SamePrefix(epoch, m_epoch)) return null;
                uint count = DirectPacket.EpochCount(epoch), current = DirectPacket.EpochCount(m_epoch);
                if (count < current) return null;
                if (count == current)
                {
                    if (!m_window.IsNew(sequence)) return null;
                    byte[] packet = DirectPacket.OpenCopy(m_cipher, m_key, m_sender, m_receiver, datagram);
                    if (packet != null) m_window.Accept(sequence);
                    return packet;
                }
            }

            // First or later epoch: move to it only if a datagram decrypts with its key
            byte[] key = DirectPacket.DeriveKeyFromExtracted(m_extracted, m_cipher, m_sender, m_receiver, epoch);
            byte[] opened = DirectPacket.OpenCopy(m_cipher, key, m_sender, m_receiver, datagram);
            if (opened == null) return null;
            m_epoch = epoch;
            m_key = key;
            m_window = new ReplayWindow();
            m_window.Accept(sequence);
            return opened;
        }
    }

    /// <summary>
    /// This player's side of one direct connect attempt: the cipher, net ids and pair keys from
    /// DIRECT_CONNECT, one receiver per peer for the whole attempt, and the current sender to each.
    /// Create it when DIRECT_CONNECT arrives; Clear it when the attempt or match ends. It enforces:
    /// <list type="bullet">
    /// <item>only DIRECT_CONNECT's peers, each with its own pair key;</item>
    /// <item>exactly one receiver per peer, so nothing received can be replayed;</item>
    /// <item>a new epoch for every sender (the attempt's prefix, a rising count), so no nonce is reused;</item>
    /// <item>a new sender retires the one it replaces.</item>
    /// </list>
    /// Thread-safe.
    /// </summary>
    internal sealed class DirectSession
    {
        private readonly string m_cipher;
        private readonly int m_netId;
        private readonly byte[] m_prefix;
        private uint m_count = 0;
        private readonly System.Collections.Generic.Dictionary<int, byte[]> m_pairKeys = new System.Collections.Generic.Dictionary<int, byte[]>();
        private readonly System.Collections.Generic.Dictionary<int, DirectSender> m_senders = new System.Collections.Generic.Dictionary<int, DirectSender>();
        private readonly System.Collections.Generic.Dictionary<int, DirectReceiver> m_receivers = new System.Collections.Generic.Dictionary<int, DirectReceiver>();
        private readonly object m_lock = new object();

        /// <summary>in_pairKeys maps each peer's net id to its 16-byte pairKey from DIRECT_CONNECT.</summary>
        public DirectSession(string in_cipher, int in_netId, System.Collections.Generic.IDictionary<int, byte[]> in_pairKeys)
            : this(in_cipher, in_netId, in_pairKeys, DirectPacket.RandomPrefix())
        {
        }

        /// For the tests: a chosen prefix.
        internal DirectSession(string in_cipher, int in_netId, System.Collections.Generic.IDictionary<int, byte[]> in_pairKeys, byte[] in_prefix)
        {
            if (!DirectPacket.IsSupported(in_cipher)) throw new ArgumentException("unsupported cipher: " + in_cipher);
            DirectPacket.checkNetId(in_netId);
            if (in_pairKeys == null || in_pairKeys.Count == 0) throw new ArgumentException("an attempt needs at least one peer");
            foreach (var peer in in_pairKeys)
            {
                DirectPacket.checkNetId(peer.Key);
                if (peer.Key == in_netId) throw new ArgumentException("this player is not its own peer: net id " + in_netId);
                DirectPacket.ExtractKey(peer.Value);   // checks the key now
                foreach (byte[] other in m_pairKeys.Values)
                {
                    // A key given for two peers would be shared beyond a pair
                    if (sameBytes(other, peer.Value)) throw new ArgumentException("two peers were given the same pair key");
                }
                m_pairKeys[peer.Key] = (byte[])peer.Value.Clone();
            }
            m_cipher = in_cipher;
            m_netId = in_netId;
            m_prefix = (byte[])in_prefix.Clone();
        }

        public int NetId { get { return m_netId; } }

        /// <summary>
        /// When the attempt or match ends: overwrites the pair keys and all keys derived from them and
        /// forgets every peer. Copies held elsewhere (e.g. the RTT message) are not reached.
        /// </summary>
        public void Clear()
        {
            lock (m_lock)
            {
                foreach (byte[] key in m_pairKeys.Values) Array.Clear(key, 0, key.Length);
                foreach (DirectSender sender in m_senders.Values) sender.Retire();
                foreach (DirectReceiver receiver in m_receivers.Values) receiver.Wipe();
                m_pairKeys.Clear();
                m_senders.Clear();
                m_receivers.Clear();
            }
        }

        /// Whether in_netId is one of this attempt's peers.
        public bool IsPeer(int in_netId)
        {
            return m_pairKeys.ContainsKey(in_netId);
        }

        /// The receiver for in_peerNetId, the same one for the whole attempt. Throws if not a peer.
        public DirectReceiver ReceiverFor(int in_peerNetId)
        {
            lock (m_lock)
            {
                DirectReceiver receiver;
                if (!m_receivers.TryGetValue(in_peerNetId, out receiver))
                {
                    receiver = new DirectReceiver(pairKey(in_peerNetId), m_cipher, in_peerNetId, m_netId);
                    m_receivers[in_peerNetId] = receiver;
                }
                return receiver;
            }
        }

        /// The current sender to in_peerNetId, made on first use. Throws if not a peer.
        public DirectSender SenderFor(int in_peerNetId)
        {
            lock (m_lock)
            {
                DirectSender sender;
                if (!m_senders.TryGetValue(in_peerNetId, out sender))
                {
                    sender = newSender(in_peerNetId);
                    m_senders[in_peerNetId] = sender;
                }
                return sender;
            }
        }

        /// <summary>A new sender to in_peerNetId in a new epoch, e.g. after a rebuilt socket. Retires the old one.</summary>
        public DirectSender RestartSender(int in_peerNetId)
        {
            lock (m_lock)
            {
                DirectSender replacement = newSender(in_peerNetId);
                DirectSender old;
                if (m_senders.TryGetValue(in_peerNetId, out old)) old.Retire();
                m_senders[in_peerNetId] = replacement;
                return replacement;
            }
        }

        // A sender in the next epoch; the caller records it. Call with m_lock held.
        private DirectSender newSender(int in_peerNetId)
        {
            byte[] key = pairKey(in_peerNetId);
            if (m_count == uint.MaxValue) throw new InvalidOperationException("epochs used up for this attempt");
            ++m_count;
            return new DirectSender(key, m_cipher, m_netId, in_peerNetId, DirectPacket.MakeEpoch(m_prefix, m_count));
        }

        private static bool sameBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private byte[] pairKey(int in_peerNetId)
        {
            byte[] key;
            if (!m_pairKeys.TryGetValue(in_peerNetId, out key)) throw new ArgumentException("net id " + in_peerNetId + " is not a peer in this attempt");
            return key;
        }
    }

    /// <summary>
    /// Replay protection for one epoch from one peer, as in DTLS: the highest sequence seen plus a bit
    /// for each of the 63 before it; anything older is dropped. Check IsNew before decrypting and
    /// Accept only after, so a forged datagram cannot move the window.
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
                // C# masks shift counts to 6 bits, so a shift of 64 or more must be avoided
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
