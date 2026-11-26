// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;

    /// <summary>
    /// SHA-256 (FIPS 180-4), for deriving the keys players use when connected directly. In plain C#
    /// for the same reason as ChaCha20Poly1305: the library takes nothing from the platform's
    /// cryptography under Unity.
    /// </summary>
    internal sealed class Sha256
    {
        public const int HASH_LENGTH = 32;
        public const int BLOCK_LENGTH = 64;

        private static readonly uint[] K =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        private readonly uint[] m_h = { 0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a, 0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19 };
        private readonly uint[] m_w = new uint[64];
        private readonly byte[] m_buffer = new byte[BLOCK_LENGTH];
        private int m_buffered = 0;
        private ulong m_length = 0;
        private bool m_finished = false;

        public static byte[] Hash(byte[] in_data)
        {
            var sha = new Sha256();
            sha.Update(in_data, 0, in_data.Length);
            return sha.Finish();
        }

        public void Update(byte[] in_data, int in_offset, int in_length)
        {
            if (m_finished) throw new InvalidOperationException("SHA-256 already finished");
            update(in_data, in_offset, in_length);
        }

        private void update(byte[] in_data, int in_offset, int in_length)
        {
            m_length += (ulong)in_length;
            int i = 0;
            if (m_buffered > 0)
            {
                int take = Math.Min(BLOCK_LENGTH - m_buffered, in_length);
                Buffer.BlockCopy(in_data, in_offset, m_buffer, m_buffered, take);
                m_buffered += take;
                i = take;
                if (m_buffered < BLOCK_LENGTH) return;
                processBlock(m_buffer, 0);
                m_buffered = 0;
            }
            for (; i + BLOCK_LENGTH <= in_length; i += BLOCK_LENGTH) processBlock(in_data, in_offset + i);
            if (i < in_length)
            {
                Buffer.BlockCopy(in_data, in_offset + i, m_buffer, 0, in_length - i);
                m_buffered = in_length - i;
            }
        }

        /// The 32-byte hash. The object cannot be used after this.
        public byte[] Finish()
        {
            if (m_finished) throw new InvalidOperationException("SHA-256 already finished");
            m_finished = true;
            ulong bits = m_length * 8;
            // A 1 bit, zeros up to 56 bytes into a block, then the length in bits
            byte[] padding = new byte[(m_buffered < 56 ? 56 : 120) - m_buffered + 8];
            padding[0] = 0x80;
            for (int i = 0; i < 8; ++i) padding[padding.Length - 1 - i] = (byte)(bits >> (8 * i));
            update(padding, 0, padding.Length);

            byte[] hash = new byte[HASH_LENGTH];
            for (int i = 0; i < 8; ++i)
            {
                hash[4 * i] = (byte)(m_h[i] >> 24);
                hash[4 * i + 1] = (byte)(m_h[i] >> 16);
                hash[4 * i + 2] = (byte)(m_h[i] >> 8);
                hash[4 * i + 3] = (byte)m_h[i];
            }
            return hash;
        }

        private void processBlock(byte[] b, int o)
        {
            unchecked
            {
                uint[] w = m_w;
                for (int i = 0; i < 16; ++i)
                {
                    w[i] = ((uint)b[o + 4 * i] << 24) | ((uint)b[o + 4 * i + 1] << 16) | ((uint)b[o + 4 * i + 2] << 8) | b[o + 4 * i + 3];
                }
                for (int i = 16; i < 64; ++i)
                {
                    uint s0 = rotr(w[i - 15], 7) ^ rotr(w[i - 15], 18) ^ (w[i - 15] >> 3);
                    uint s1 = rotr(w[i - 2], 17) ^ rotr(w[i - 2], 19) ^ (w[i - 2] >> 10);
                    w[i] = w[i - 16] + s0 + w[i - 7] + s1;
                }

                uint a = m_h[0], bb = m_h[1], c = m_h[2], d = m_h[3], e = m_h[4], f = m_h[5], g = m_h[6], h = m_h[7];
                for (int i = 0; i < 64; ++i)
                {
                    uint t1 = h + (rotr(e, 6) ^ rotr(e, 11) ^ rotr(e, 25)) + ((e & f) ^ (~e & g)) + K[i] + w[i];
                    uint t2 = (rotr(a, 2) ^ rotr(a, 13) ^ rotr(a, 22)) + ((a & bb) ^ (a & c) ^ (bb & c));
                    h = g; g = f; f = e; e = d + t1;
                    d = c; c = bb; bb = a; a = t1 + t2;
                }
                m_h[0] += a; m_h[1] += bb; m_h[2] += c; m_h[3] += d;
                m_h[4] += e; m_h[5] += f; m_h[6] += g; m_h[7] += h;
            }
        }

        private static uint rotr(uint v, int n)
        {
            return (v >> n) | (v << (32 - n));
        }
    }

    /// <summary>HMAC-SHA256 (RFC 2104) and HKDF-SHA256 (RFC 5869).</summary>
    internal static class HmacSha256
    {
        public static byte[] Compute(byte[] in_key, byte[] in_data)
        {
            byte[] key = in_key.Length > Sha256.BLOCK_LENGTH ? Sha256.Hash(in_key) : in_key;
            byte[] inner = new byte[Sha256.BLOCK_LENGTH];
            byte[] outer = new byte[Sha256.BLOCK_LENGTH];
            for (int i = 0; i < Sha256.BLOCK_LENGTH; ++i)
            {
                byte k = i < key.Length ? key[i] : (byte)0;
                inner[i] = (byte)(k ^ 0x36);
                outer[i] = (byte)(k ^ 0x5c);
            }

            var sha = new Sha256();
            sha.Update(inner, 0, inner.Length);
            sha.Update(in_data, 0, in_data.Length);
            byte[] innerHash = sha.Finish();

            sha = new Sha256();
            sha.Update(outer, 0, outer.Length);
            sha.Update(innerHash, 0, innerHash.Length);
            return sha.Finish();
        }

        /// <summary>
        /// HKDF-SHA256: in_length bytes of key material from in_ikm, with in_salt (null or empty
        /// means a block of zeros, as the RFC says) and in_info.
        /// </summary>
        public static byte[] Hkdf(byte[] in_salt, byte[] in_ikm, byte[] in_info, int in_length)
        {
            if (in_length < 0 || in_length > 255 * Sha256.HASH_LENGTH) throw new ArgumentException("HKDF can give at most 8160 bytes");
            byte[] salt = (in_salt == null || in_salt.Length == 0) ? new byte[Sha256.HASH_LENGTH] : in_salt;
            byte[] info = in_info ?? new byte[0];
            byte[] prk = Compute(salt, in_ikm);

            byte[] okm = new byte[in_length];
            byte[] t = new byte[0];
            for (int done = 0, counter = 1; done < in_length; ++counter)
            {
                byte[] input = new byte[t.Length + info.Length + 1];
                Buffer.BlockCopy(t, 0, input, 0, t.Length);
                Buffer.BlockCopy(info, 0, input, t.Length, info.Length);
                input[input.Length - 1] = (byte)counter;
                t = Compute(prk, input);
                int n = Math.Min(t.Length, in_length - done);
                Buffer.BlockCopy(t, 0, okm, done, n);
                done += n;
            }
            return okm;
        }
    }
}
