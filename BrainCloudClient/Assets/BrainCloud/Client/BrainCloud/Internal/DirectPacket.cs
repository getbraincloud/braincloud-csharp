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
    /// packet and a tag authenticating it with the header. With chacha20-poly1305 the packet is
    /// encrypted; with hmac-sha256 it is only authenticated, and travels readable, as it does
    /// through a relay server. Every client library must produce these byte for byte; the format is
    /// section 7 of the client design (Confluence BCSD 4389666817).
    ///
    /// <code>
    /// offset  size  field
    ///      0     1  FORMAT (0xB1): brainCloud direct connect, format 1
    ///      1     1  sender's net id
    ///      2     1  receiver's net id
    ///      3     8  the sender's epoch: 4 random bytes, the same for every sender in this
    ///               attempt, then a 4-byte big-endian count that goes up with every new sender
    ///     11     8  sequence number, big-endian, from 0 within the epoch
    ///     19     n  the packet: encrypted with chacha20-poly1305, unchanged with hmac-sha256
    ///   19+n    16  authentication tag
    /// </code>
    ///
    /// Each pair of players in an attempt has its own secret, from brainCloud, which only those two
    /// players have: so no member of an attempt can make or read a datagram between two others.
    /// Each epoch of each direction between two players has its own key (DeriveKey), and the
    /// cipher's name is part of it. With chacha20-poly1305 the nonce is 4 zero bytes then the
    /// sequence number, and the header is the associated data; with hmac-sha256 the tag is the
    /// first 16 bytes of HMAC-SHA256 of the header and the packet. A key is never
    /// used with the same nonce twice: the sequence only goes up within an epoch, and a sender that
    /// starts again, for whatever reason, starts a new epoch and so gets a new key. Epochs are
    /// ordered, so a receiver only ever moves forward (see DirectReceiver).
    ///
    /// Use DirectSession, which keeps those rules for one attempt; the static methods here are the
    /// format itself.
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
        /// The largest datagram sent or accepted: the most UDP payload that fits the 1,280 bytes
        /// every IPv6 link carries without fragmenting. A full game packet is 1,068 bytes. Anything
        /// larger is dropped before any decrypting, so it cannot be used to make a receiver work harder.
        /// </summary>
        public const int MAX_DATAGRAM_LENGTH = 1232;
        /// The length of the secret brainCloud gives each pair of players (DIRECT_CONNECT's peers[].pairKey).
        public const int PAIR_KEY_LENGTH = 16;

        /// Encrypts and authenticates. Only when an implementation is included: see DirectCipher.
        public const string CIPHER_CHACHA20_POLY1305 = "chacha20-poly1305";
        /// Authenticates only: no one can make, change or replay a packet, but anyone on the path can read it.
        public const string CIPHER_HMAC_SHA256 = "hmac-sha256";

        /// <summary>
        /// The ciphers this player can use, most preferred first, to report with its network test.
        /// hmac-sha256 is always there; chacha20-poly1305 only when an implementation is included.
        /// </summary>
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

        /// Whether this player can use in_cipher, which may be one DIRECT_CONNECT names. Never throws.
        public static bool IsSupported(string in_cipher)
        {
            return DirectCipher.For(in_cipher) != null;
        }

        private static readonly byte[] KEY_SALT = Encoding.ASCII.GetBytes("brainCloud direct connect");

        /// <summary>
        /// The key for packets from in_sender to in_receiver in the sender's epoch in_epoch:
        /// HKDF-SHA256 with the pair's 16-byte pair key as the input key material,
        /// "brainCloud direct connect" as the salt, and as the info the cipher name, a 0 byte, the
        /// sender's net id, the receiver's net id and the 8 epoch bytes.
        /// </summary>
        public static byte[] DeriveKey(byte[] in_pairKey, string in_cipher, int in_sender, int in_receiver, byte[] in_epoch)
        {
            return DeriveKeyFromExtracted(ExtractKey(in_pairKey), in_cipher, in_sender, in_receiver, in_epoch);
        }

        /// The first, epoch-independent step of DeriveKey, which a receiver can do once.
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

        /// <summary>The datagram carrying in_length bytes of in_packet from in_offset, with in_cipher and in_key.</summary>
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

        /// <summary>
        /// Reads the header of the first in_length bytes of a received datagram. False when they are
        /// not a datagram of this format.
        /// </summary>
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
        /// The packet in the first in_length bytes of a received datagram, or null when it is not
        /// from in_expectedSender to in_expectedReceiver or does not open with in_cipher and in_key. The
        /// expected ids are the ones the key belongs to; never take them from the header, or a
        /// datagram could be bounced back to its sender or passed on to another player.
        /// </summary>
        public static byte[] Open(string in_cipher, byte[] in_key, int in_expectedSender, int in_expectedReceiver, byte[] in_datagram, int in_length)
        {
            byte[] datagram = CopyReceived(in_datagram, in_length);
            return datagram == null ? null : OpenCopy(in_cipher, in_key, in_expectedSender, in_expectedReceiver, datagram);
        }

        /// <summary>
        /// The first in_length bytes of a received datagram, copied, or null when they cannot be a
        /// datagram of this format. Everything after is read from the copy alone, so a receive buffer
        /// that changes while a datagram is being opened cannot make the bytes that were checked
        /// differ from the bytes that are used.
        /// </summary>
        internal static byte[] CopyReceived(byte[] in_datagram, int in_length)
        {
            if (in_datagram == null || in_length < OVERHEAD || in_length > MAX_DATAGRAM_LENGTH || in_length > in_datagram.Length) return null;
            byte[] datagram = new byte[in_length];
            Buffer.BlockCopy(in_datagram, 0, datagram, 0, in_length);
            return datagram;
        }

        /// Open, for a datagram already copied with CopyReceived: in_datagram is not shared with anyone.
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

        // The datagram does not name its cipher: every member of an attempt uses the one the server chose.
        private static IDirectAead implementation(string in_cipher)
        {
            IDirectAead aead = DirectCipher.For(in_cipher);
            if (aead == null) throw new ArgumentException("unsupported cipher: " + in_cipher);
            return aead;
        }

        /// <summary>
        /// A new random epoch prefix, for one attempt. It is random per attempt, not per process, so
        /// it does not link a player's sessions together. A version 4 GUID's randomness is plenty:
        /// it only has to differ from other players' in the same attempt.
        /// </summary>
        public static byte[] RandomPrefix()
        {
            byte[] prefix = new byte[4];
            Buffer.BlockCopy(Guid.NewGuid().ToByteArray(), 0, prefix, 0, 4);
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
    /// How one cipher protects a packet as direct connect uses it: a 32-byte key, a 12-byte nonce,
    /// the header as associated data, and a 16-byte tag after the packet. Its implementation can come
    /// from this library, or from the operating system (DirectCipher).
    /// </summary>
    internal interface IDirectAead
    {
        /// The protected form of in_length bytes of in_plaintext from in_offset, followed by the tag.
        byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length);

        /// The packet in in_length bytes of in_sealed from in_offset (protected packet and tag), or null when the tag does not match.
        byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length);
    }

    /// <summary>
    /// hmac-sha256: the packet is sent as it is, followed by the first 16 bytes of HMAC-SHA256 of the
    /// header and the packet. Nothing is encrypted, so this needs no encryption implementation and
    /// is always available. The header carries the sequence number, so the nonce adds nothing.
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
            // The packet is copied once, and the copy is both checked and returned: bytes changed in
            // the caller's buffer after the check can never be handed back as authenticated
            byte[] message = macInput(in_aad, in_sealed, in_offset, packetLength);
            byte[] expected = mac(in_key, message);
            // Every byte is compared, whatever the first difference, so the time taken says nothing about the tag
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
    /// Which implementation protects direct-connect traffic with each cipher.
    ///
    /// hmac-sha256 authenticates without encrypting, and is always available.
    ///
    /// chacha20-poly1305 encrypts, and is available only when a game includes an implementation
    /// of it. This library's own is compiled in only when the game defines the scripting symbol
    /// BRAINCLOUD_DIRECT_CONNECT_ENCRYPTION; without it, the library contains no encryption code
    /// at all. That is deliberate: an app store may ask whether a game implements encryption of
    /// its own (Apple asks every app whether it uses standard encryption "instead of, or in
    /// addition to" the operating system's), and a game should only have to answer yes when it
    /// chose to. A game that wants direct connect encrypted, and lobby types that require it,
    /// defines the symbol for each platform it builds.
    ///
    /// Key derivation (HKDF-SHA256) is always in this library: it hashes and authenticates, and
    /// does not encrypt.
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
        /// Replaces the ChaCha20-Poly1305 implementation, or with null removes it: for one backed
        /// by the operating system, and for the tests. Install it at startup, before any attempt.
        /// Every implementation is the same cipher, so datagrams are the same either way.
        /// </summary>
        internal static IDirectAead ChaCha20Poly1305Implementation
        {
            get { return s_chaCha20Poly1305; }
            set { s_chaCha20Poly1305 = value; }
        }
    }

    /// <summary>
    /// Everything this player sends to one peer in one epoch: the key, and the sequence number,
    /// which only goes up. Get one from DirectSession, which gives each a new epoch and retires the
    /// one it replaces. Safe to use from more than one thread.
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

        /// <summary>
        /// Made by DirectSession, or by the tests with a chosen epoch. Reusing an epoch with the
        /// same pair key reuses nonces, so nothing else may call this.
        /// </summary>
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
        /// has been replaced: the peer would drop its datagrams anyway, since it only moves forward.
        /// </summary>
        public byte[] Seal(byte[] in_packet, int in_offset, int in_length)
        {
            ulong sequence;
            lock (m_lock)
            {
                if (m_retired) throw new InvalidOperationException("this sender was replaced by a newer one");
                // Two threads must never get the same sequence number: that would be the same nonce
                if (m_next == ulong.MaxValue) throw new InvalidOperationException("sequence numbers used up; start a new sender");
                sequence = m_next++;
            }
            return DirectPacket.Seal(m_cipher, m_key, m_sender, m_receiver, m_epoch, sequence, in_packet, in_offset, in_length);
        }

        internal void Retire()
        {
            lock (m_lock) m_retired = true;
        }
    }

    /// <summary>
    /// Everything this player receives from one peer. It accepts only datagrams from that peer to
    /// this player that decrypt, each sequence number once, and only ever moves forward:
    /// <list type="bullet">
    /// <item>The first datagram that decrypts fixes the peer's epoch prefix. A peer keeps its
    /// prefix for the whole attempt, and a player cannot rejoin a running attempt (a new attempt
    /// has new pair keys), so another prefix is never genuine and is dropped without deriving any
    /// key.</item>
    /// <item>A higher count than the current epoch's means the peer started sending again: the
    /// first datagram of it that decrypts moves this receiver to it. A lower or equal count is an
    /// old epoch, and is dropped without deriving any key.</item>
    /// </list>
    /// There must be exactly one for each peer for the whole attempt, which DirectSession ensures: a
    /// new one would accept a replay of everything the old one had received. Safe to use from more
    /// than one thread.
    ///
    /// It does not say where a datagram came from. Never move a peer to a new address just because
    /// a datagram from there decrypted: an attacker can resend a genuine one from anywhere.
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
        private readonly object m_lock = new object();

        /// in_sender is the peer's net id, in_receiver this player's. Made by DirectSession, or by the tests.
        internal DirectReceiver(byte[] in_pairKey, string in_cipher, int in_sender, int in_receiver)
        {
            m_extracted = DirectPacket.ExtractKey(in_pairKey);
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
            // One copy, made first: the epoch and sequence the replay window records are then always
            // the ones the tag was checked over, whatever happens to the caller's buffer meanwhile
            byte[] datagram = DirectPacket.CopyReceived(in_datagram, in_length);
            if (datagram == null) return null;
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            if (!DirectPacket.TryReadHeader(datagram, datagram.Length, out sender, out receiver, out epoch, out sequence)) return null;
            if (sender != m_sender || receiver != m_receiver) return null;
            lock (m_lock)
            {
                return open(datagram, epoch, sequence);
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

            // The first epoch, or a later one: only a datagram that decrypts with its key moves this receiver to it
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
    /// This player's side of one direct connect attempt: what DIRECT_CONNECT gave it (the cipher,
    /// its net id, and each peer's net id with the key this player shares with that peer alone),
    /// one receiver for each peer for the whole attempt, and the current sender to each. Create one
    /// when DIRECT_CONNECT arrives and drop it when the attempt or the match ends. It keeps the rules
    /// the format depends on:
    /// <list type="bullet">
    /// <item>only the peers DIRECT_CONNECT named, each with its own pair key, so no member can make
    /// or read a datagram between two others;</item>
    /// <item>exactly one receiver per peer, so nothing already received can be replayed to it;</item>
    /// <item>every sender gets a new epoch (this attempt's random prefix and a count that goes up),
    /// so no key ever reuses a nonce;</item>
    /// <item>a new sender to a peer retires the one it replaces, which the peer would no longer
    /// accept anyway.</item>
    /// </list>
    /// Safe to use from more than one thread.
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

        /// <summary>
        /// in_netId is this player's net id; in_pairKeys holds, for each peer's net id, the 16 bytes
        /// of that peer's pairKey in DIRECT_CONNECT.
        /// </summary>
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
                    // Each pair's key is its own; the same one for two peers means one secret is shared beyond a pair
                    if (sameBytes(other, peer.Value)) throw new ArgumentException("two peers were given the same pair key");
                }
                m_pairKeys[peer.Key] = (byte[])peer.Value.Clone();
            }
            m_cipher = in_cipher;
            m_netId = in_netId;
            m_prefix = (byte[])in_prefix.Clone();
        }

        public int NetId { get { return m_netId; } }

        /// Whether in_netId is one of this attempt's peers.
        public bool IsPeer(int in_netId)
        {
            return m_pairKeys.ContainsKey(in_netId);
        }

        /// The receiver for packets from in_peerNetId, always the same one for this attempt. Throws for a net id that is not a peer.
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

        /// The current sender to in_peerNetId, made on first use. Throws for a net id that is not a peer.
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

        /// <summary>
        /// A new sender to in_peerNetId, in a new epoch, for when this player starts sending to it
        /// again (a rebuilt socket, say). The one it replaces is retired and refuses to seal.
        /// </summary>
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
