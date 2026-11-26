// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Runtime.CompilerServices;

// The unit tests check the cipher against the RFC test vectors.
[assembly: InternalsVisibleTo("UnitTest")]

namespace BrainCloud.Internal
{
    using System;

    /// <summary>
    /// AEAD_CHACHA20_POLY1305 (RFC 8439): encrypts and authenticates the packets players send each
    /// other when they are connected directly. Written in plain C#, like the library's MD5, so that
    /// it needs nothing from the platform: .NET Standard 2.0 (Unity 2019.1) has no ChaCha20-Poly1305
    /// of its own, and it runs the same under Mono, IL2CPP and .NET.
    /// </summary>
    internal static class ChaCha20Poly1305
    {
        public const int KEY_LENGTH = 32;
        public const int NONCE_LENGTH = 12;
        public const int TAG_LENGTH = 16;

        /// <summary>
        /// Encrypts in_length bytes of in_plaintext from in_offset, and authenticates them together
        /// with in_aad. Returns the ciphertext followed by the 16-byte tag.
        /// </summary>
        public static byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length)
        {
            checkKeyAndNonce(in_key, in_nonce);
            byte[] output = new byte[in_length + TAG_LENGTH];
            ChaCha20.Xor(in_key, in_nonce, 1, in_plaintext, in_offset, output, 0, in_length);
            byte[] tag = computeTag(in_key, in_nonce, in_aad, output, 0, in_length);
            Buffer.BlockCopy(tag, 0, output, in_length, TAG_LENGTH);
            return output;
        }

        /// <summary>
        /// Checks the tag at the end of in_length bytes of in_sealed from in_offset, and if it is
        /// right, decrypts the rest. Returns the plaintext, or null when the tag does not match: the
        /// data, the associated data, the key or the nonce is not what it was sealed with.
        /// </summary>
        public static byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length)
        {
            checkKeyAndNonce(in_key, in_nonce);
            if (in_length < TAG_LENGTH) return null;
            int cipherLength = in_length - TAG_LENGTH;
            byte[] expected = computeTag(in_key, in_nonce, in_aad, in_sealed, in_offset, cipherLength);

            // Compare every byte whatever the result, so the time taken says nothing about the tag
            int difference = 0;
            for (int i = 0; i < TAG_LENGTH; ++i) difference |= expected[i] ^ in_sealed[in_offset + cipherLength + i];
            if (difference != 0) return null;

            byte[] plaintext = new byte[cipherLength];
            ChaCha20.Xor(in_key, in_nonce, 1, in_sealed, in_offset, plaintext, 0, cipherLength);
            return plaintext;
        }

        /// RFC 8439 section 2.8: Poly1305 over the AAD and ciphertext, each padded to 16 bytes, then both lengths
        private static byte[] computeTag(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_cipher, int in_offset, int in_length)
        {
            // The one-time Poly1305 key is the first half of key stream block 0 (section 2.6)
            byte[] block0 = new byte[64];
            ChaCha20.Block(in_key, in_nonce, 0, block0);
            var mac = new Poly1305(block0, 0);

            byte[] zeros = new byte[16];
            int aadLength = in_aad == null ? 0 : in_aad.Length;
            if (aadLength > 0) mac.Update(in_aad, 0, aadLength);
            mac.Update(zeros, 0, (16 - aadLength % 16) % 16);
            mac.Update(in_cipher, in_offset, in_length);
            mac.Update(zeros, 0, (16 - in_length % 16) % 16);

            byte[] lengths = new byte[16];
            writeU64LE(lengths, 0, (ulong)aadLength);
            writeU64LE(lengths, 8, (ulong)in_length);
            mac.Update(lengths, 0, 16);
            return mac.Finish();
        }

        private static void checkKeyAndNonce(byte[] in_key, byte[] in_nonce)
        {
            if (in_key == null || in_key.Length != KEY_LENGTH) throw new ArgumentException("the key must be 32 bytes");
            if (in_nonce == null || in_nonce.Length != NONCE_LENGTH) throw new ArgumentException("the nonce must be 12 bytes");
        }

        private static void writeU64LE(byte[] b, int o, ulong v)
        {
            for (int i = 0; i < 8; ++i) b[o + i] = (byte)(v >> (8 * i));
        }
    }

    /// <summary>The ChaCha20 stream cipher (RFC 8439 section 2.4), with a 96-bit nonce and 32-bit block counter.</summary>
    internal static class ChaCha20
    {
        /// One 64-byte block of key stream (section 2.3).
        public static void Block(byte[] in_key, byte[] in_nonce, uint in_counter, byte[] out_block)
        {
            uint[] state = initialState(in_key, in_nonce, in_counter);
            uint[] working = new uint[16];
            keyStreamBlock(state, working, out_block);
        }

        /// <summary>
        /// XORs in_length bytes of in_input from in_inOffset with the key stream starting at block
        /// in_counter, into out_output from in_outOffset. Encrypting and decrypting are the same.
        /// </summary>
        public static void Xor(byte[] in_key, byte[] in_nonce, uint in_counter, byte[] in_input, int in_inOffset, byte[] out_output, int in_outOffset, int in_length)
        {
            uint[] state = initialState(in_key, in_nonce, in_counter);
            uint[] working = new uint[16];
            byte[] block = new byte[64];
            for (int done = 0; done < in_length; done += 64)
            {
                keyStreamBlock(state, working, block);
                int n = Math.Min(64, in_length - done);
                for (int i = 0; i < n; ++i) out_output[in_outOffset + done + i] = (byte)(in_input[in_inOffset + done + i] ^ block[i]);
                unchecked { ++state[12]; }
            }
        }

        private static uint[] initialState(byte[] in_key, byte[] in_nonce, uint in_counter)
        {
            uint[] s = new uint[16];
            // "expand 32-byte k"
            s[0] = 0x61707865; s[1] = 0x3320646e; s[2] = 0x79622d32; s[3] = 0x6b206574;
            for (int i = 0; i < 8; ++i) s[4 + i] = readU32LE(in_key, 4 * i);
            s[12] = in_counter;
            for (int i = 0; i < 3; ++i) s[13 + i] = readU32LE(in_nonce, 4 * i);
            return s;
        }

        private static void keyStreamBlock(uint[] in_state, uint[] x, byte[] out_block)
        {
            Array.Copy(in_state, x, 16);
            for (int round = 0; round < 10; ++round)
            {
                // Columns, then diagonals
                quarterRound(x, 0, 4, 8, 12);
                quarterRound(x, 1, 5, 9, 13);
                quarterRound(x, 2, 6, 10, 14);
                quarterRound(x, 3, 7, 11, 15);
                quarterRound(x, 0, 5, 10, 15);
                quarterRound(x, 1, 6, 11, 12);
                quarterRound(x, 2, 7, 8, 13);
                quarterRound(x, 3, 4, 9, 14);
            }
            for (int i = 0; i < 16; ++i)
            {
                uint v = unchecked(x[i] + in_state[i]);
                out_block[4 * i] = (byte)v;
                out_block[4 * i + 1] = (byte)(v >> 8);
                out_block[4 * i + 2] = (byte)(v >> 16);
                out_block[4 * i + 3] = (byte)(v >> 24);
            }
        }

        private static void quarterRound(uint[] x, int a, int b, int c, int d)
        {
            unchecked
            {
                x[a] += x[b]; x[d] = rotl(x[d] ^ x[a], 16);
                x[c] += x[d]; x[b] = rotl(x[b] ^ x[c], 12);
                x[a] += x[b]; x[d] = rotl(x[d] ^ x[a], 8);
                x[c] += x[d]; x[b] = rotl(x[b] ^ x[c], 7);
            }
        }

        private static uint rotl(uint v, int n)
        {
            return (v << n) | (v >> (32 - n));
        }

        internal static uint readU32LE(byte[] b, int o)
        {
            return (uint)b[o] | ((uint)b[o + 1] << 8) | ((uint)b[o + 2] << 16) | ((uint)b[o + 3] << 24);
        }
    }

    /// <summary>
    /// The Poly1305 one-time authenticator (RFC 8439 section 2.5), fed in pieces. The accumulator is
    /// kept in five 26-bit limbs so every product fits in 64 bits (the "donna" 32-bit method).
    /// </summary>
    internal sealed class Poly1305
    {
        private const uint MASK26 = 0x3ffffff;

        private readonly uint r0, r1, r2, r3, r4;
        private readonly uint s1, s2, s3, s4;
        private readonly uint pad0, pad1, pad2, pad3;
        private uint h0, h1, h2, h3, h4;
        private readonly byte[] m_buffer = new byte[16];
        private int m_buffered = 0;

        /// in_key holds the 32-byte one-time key from in_offset: r, then s.
        public Poly1305(byte[] in_key, int in_offset)
        {
            // r is clamped as section 2.5 requires, then split into 26-bit limbs
            r0 = ChaCha20.readU32LE(in_key, in_offset + 0) & 0x3ffffff;
            r1 = (ChaCha20.readU32LE(in_key, in_offset + 3) >> 2) & 0x3ffff03;
            r2 = (ChaCha20.readU32LE(in_key, in_offset + 6) >> 4) & 0x3ffc0ff;
            r3 = (ChaCha20.readU32LE(in_key, in_offset + 9) >> 6) & 0x3f03fff;
            r4 = (ChaCha20.readU32LE(in_key, in_offset + 12) >> 8) & 0x00fffff;
            s1 = r1 * 5; s2 = r2 * 5; s3 = r3 * 5; s4 = r4 * 5;
            pad0 = ChaCha20.readU32LE(in_key, in_offset + 16);
            pad1 = ChaCha20.readU32LE(in_key, in_offset + 20);
            pad2 = ChaCha20.readU32LE(in_key, in_offset + 24);
            pad3 = ChaCha20.readU32LE(in_key, in_offset + 28);
        }

        public void Update(byte[] in_data, int in_offset, int in_length)
        {
            int i = 0;
            if (m_buffered > 0)
            {
                int take = Math.Min(16 - m_buffered, in_length);
                Buffer.BlockCopy(in_data, in_offset, m_buffer, m_buffered, take);
                m_buffered += take;
                i = take;
                if (m_buffered < 16) return;
                processBlock(m_buffer, 0, 1u << 24);
                m_buffered = 0;
            }
            for (; i + 16 <= in_length; i += 16) processBlock(in_data, in_offset + i, 1u << 24);
            if (i < in_length)
            {
                Buffer.BlockCopy(in_data, in_offset + i, m_buffer, 0, in_length - i);
                m_buffered = in_length - i;
            }
        }

        /// The 16-byte tag. The object cannot be used after this.
        public byte[] Finish()
        {
            if (m_buffered > 0)
            {
                // A short last block gets a 1 byte after it, and no high bit
                m_buffer[m_buffered] = 1;
                for (int i = m_buffered + 1; i < 16; ++i) m_buffer[i] = 0;
                processBlock(m_buffer, 0, 0);
            }

            unchecked
            {
                // Carry fully
                uint c;
                c = h1 >> 26; h1 &= MASK26; h2 += c;
                c = h2 >> 26; h2 &= MASK26; h3 += c;
                c = h3 >> 26; h3 &= MASK26; h4 += c;
                c = h4 >> 26; h4 &= MASK26; h0 += c * 5;
                c = h0 >> 26; h0 &= MASK26; h1 += c;

                // g = h + 5 - 2^130; use it in place of h when it is not negative, i.e. h >= 2^130 - 5
                uint g0 = h0 + 5; c = g0 >> 26; g0 &= MASK26;
                uint g1 = h1 + c; c = g1 >> 26; g1 &= MASK26;
                uint g2 = h2 + c; c = g2 >> 26; g2 &= MASK26;
                uint g3 = h3 + c; c = g3 >> 26; g3 &= MASK26;
                uint g4 = h4 + c - (1u << 26);

                uint useG = (g4 >> 31) - 1;   // all ones when g4 did not go negative
                uint useH = ~useG;
                h0 = (h0 & useH) | (g0 & useG);
                h1 = (h1 & useH) | (g1 & useG);
                h2 = (h2 & useH) | (g2 & useG);
                h3 = (h3 & useH) | (g3 & useG);
                h4 = (h4 & useH) | (g4 & useG);

                // Back to four 32-bit words, then add s modulo 2^128
                uint w0 = h0 | (h1 << 26);
                uint w1 = (h1 >> 6) | (h2 << 20);
                uint w2 = (h2 >> 12) | (h3 << 14);
                uint w3 = (h3 >> 18) | (h4 << 8);

                ulong f = (ulong)w0 + pad0; w0 = (uint)f;
                f = (ulong)w1 + pad1 + (f >> 32); w1 = (uint)f;
                f = (ulong)w2 + pad2 + (f >> 32); w2 = (uint)f;
                f = (ulong)w3 + pad3 + (f >> 32); w3 = (uint)f;

                byte[] tag = new byte[16];
                writeU32LE(tag, 0, w0);
                writeU32LE(tag, 4, w1);
                writeU32LE(tag, 8, w2);
                writeU32LE(tag, 12, w3);
                return tag;
            }
        }

        private void processBlock(byte[] m, int o, uint in_hibit)
        {
            unchecked
            {
                h0 += ChaCha20.readU32LE(m, o + 0) & MASK26;
                h1 += (ChaCha20.readU32LE(m, o + 3) >> 2) & MASK26;
                h2 += (ChaCha20.readU32LE(m, o + 6) >> 4) & MASK26;
                h3 += (ChaCha20.readU32LE(m, o + 9) >> 6) & MASK26;
                h4 += (ChaCha20.readU32LE(m, o + 12) >> 8) | in_hibit;

                // h *= r, modulo 2^130 - 5 (the s values fold the high part back in, times 5)
                ulong d0 = (ulong)h0 * r0 + (ulong)h1 * s4 + (ulong)h2 * s3 + (ulong)h3 * s2 + (ulong)h4 * s1;
                ulong d1 = (ulong)h0 * r1 + (ulong)h1 * r0 + (ulong)h2 * s4 + (ulong)h3 * s3 + (ulong)h4 * s2;
                ulong d2 = (ulong)h0 * r2 + (ulong)h1 * r1 + (ulong)h2 * r0 + (ulong)h3 * s4 + (ulong)h4 * s3;
                ulong d3 = (ulong)h0 * r3 + (ulong)h1 * r2 + (ulong)h2 * r1 + (ulong)h3 * r0 + (ulong)h4 * s4;
                ulong d4 = (ulong)h0 * r4 + (ulong)h1 * r3 + (ulong)h2 * r2 + (ulong)h3 * r1 + (ulong)h4 * r0;

                ulong c;
                c = d0 >> 26; h0 = (uint)d0 & MASK26; d1 += c;
                c = d1 >> 26; h1 = (uint)d1 & MASK26; d2 += c;
                c = d2 >> 26; h2 = (uint)d2 & MASK26; d3 += c;
                c = d3 >> 26; h3 = (uint)d3 & MASK26; d4 += c;
                c = d4 >> 26; h4 = (uint)d4 & MASK26;
                h0 += (uint)c * 5;
                uint c2 = h0 >> 26; h0 &= MASK26; h1 += c2;
            }
        }

        private static void writeU32LE(byte[] b, int o, uint v)
        {
            b[o] = (byte)v; b[o + 1] = (byte)(v >> 8); b[o + 2] = (byte)(v >> 16); b[o + 3] = (byte)(v >> 24);
        }
    }
}
