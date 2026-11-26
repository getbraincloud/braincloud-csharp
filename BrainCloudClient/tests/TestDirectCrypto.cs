// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

namespace BrainCloudTests
{
    /// <summary>
    /// The cryptography for direct connect, with no server: the cipher against the test vectors in
    /// RFC 8439, the key derivation against RFC 5869, SHA-256 and HMAC against the platform's own,
    /// and the datagram format against fixed values that other client libraries can check too.
    /// </summary>
    [TestFixture]
    public class TestDirectCrypto
    {
        private static byte[] H(string in_hex)
        {
            byte[] b = new byte[in_hex.Length / 2];
            for (int i = 0; i < b.Length; ++i) b[i] = Convert.ToByte(in_hex.Substring(2 * i, 2), 16);
            return b;
        }

        private static string Hex(byte[] in_bytes)
        {
            return BitConverter.ToString(in_bytes).Replace("-", "").ToLowerInvariant();
        }

        private static byte[] Sequence(int in_from, int in_count)
        {
            return Enumerable.Range(in_from, in_count).Select(i => (byte)i).ToArray();
        }

        #region ChaCha20 and Poly1305 (RFC 8439)

        [Test]
        public void ChaCha20_BlockFunction_Rfc8439_2_3_2()
        {
            byte[] block = new byte[64];
            ChaCha20.Block(Sequence(0, 32), H("000000090000004a00000000"), 1, block);
            Assert.That(Hex(block), Is.EqualTo("10f1e7e4d13b5915500fdd1fa32071c4c7d1f4c733c068030422aa9ac3d46c4ed2826446079faa0914c2d705d98b02a2b5129cd1de164eb9cbd083e8a2503c4e"));
        }

        [Test]
        public void ChaCha20_Encryption_Rfc8439_2_4_2()
        {
            byte[] plaintext = H("4c616469657320616e642047656e746c656d656e206f662074686520636c617373206f66202739393a204966204920636f756c64206f6666657220796f75206f6e6c79206f6e652074697020666f7220746865206675747572652c2073756e73637265656e20776f756c642062652069742e");
            byte[] output = new byte[plaintext.Length];
            ChaCha20.Xor(Sequence(0, 32), H("000000000000004a00000000"), 1, plaintext, 0, output, 0, plaintext.Length);
            Assert.That(Hex(output), Is.EqualTo("6e2e359a2568f98041ba0728dd0d6981e97e7aec1d4360c20a27afccfd9fae0bf91b65c5524733ab8f593dabcd62b3571639d624e65152ab8f530c359f0861d807ca0dbf500d6a6156a38e088a22b65e52bc514d16ccf806818ce91ab77937365af90bbf74a35be6b40b8eedf2785e42874d"));
        }

        [Test]
        public void Poly1305_Rfc8439_2_5_2()
        {
            var mac = new Poly1305(H("85d6be7857556d337f4452fe42d506a80103808afb0db2fd4abff6af4149f51b"), 0);
            byte[] message = Encoding.ASCII.GetBytes("Cryptographic Forum Research Group");
            mac.Update(message, 0, message.Length);
            Assert.That(Hex(mac.Finish()), Is.EqualTo("a8061dc1305136c6c22b8baf0c0127a9"));
        }

        // RFC 8439 appendix A.3: key, text, tag. Vectors 5 to 11 are the carry and reduction edge cases.
        private static readonly string[][] POLY1305_VECTORS =
        {
            new[] { "0000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000", "00000000000000000000000000000000" },
            new[] { "0000000000000000000000000000000036e5f6b5c5e06070f0efca96227a863e", "416e79207375626d697373696f6e20746f20746865204945544620696e74656e6465642062792074686520436f6e7472696275746f7220666f72207075626c69636174696f6e20617320616c6c206f722070617274206f6620616e204945544620496e7465726e65742d4472616674206f722052464320616e6420616e792073746174656d656e74206d6164652077697468696e2074686520636f6e74657874206f6620616e204945544620616374697669747920697320636f6e7369646572656420616e20224945544620436f6e747269627574696f6e222e20537563682073746174656d656e747320696e636c756465206f72616c2073746174656d656e747320696e20494554462073657373696f6e732c2061732077656c6c206173207772697474656e20616e6420656c656374726f6e696320636f6d6d756e69636174696f6e73206d61646520617420616e792074696d65206f7220706c6163652c207768696368206172652061646472657373656420746f", "36e5f6b5c5e06070f0efca96227a863e" },
            new[] { "36e5f6b5c5e06070f0efca96227a863e00000000000000000000000000000000", "416e79207375626d697373696f6e20746f20746865204945544620696e74656e6465642062792074686520436f6e7472696275746f7220666f72207075626c69636174696f6e20617320616c6c206f722070617274206f6620616e204945544620496e7465726e65742d4472616674206f722052464320616e6420616e792073746174656d656e74206d6164652077697468696e2074686520636f6e74657874206f6620616e204945544620616374697669747920697320636f6e7369646572656420616e20224945544620436f6e747269627574696f6e222e20537563682073746174656d656e747320696e636c756465206f72616c2073746174656d656e747320696e20494554462073657373696f6e732c2061732077656c6c206173207772697474656e20616e6420656c656374726f6e696320636f6d6d756e69636174696f6e73206d61646520617420616e792074696d65206f7220706c6163652c207768696368206172652061646472657373656420746f", "f3477e7cd95417af89a6b8794c310cf0" },
            new[] { "1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0", "2754776173206272696c6c69672c20616e642074686520736c6974687920746f7665730a446964206779726520616e642067696d626c6520696e2074686520776162653a0a416c6c206d696d737920776572652074686520626f726f676f7665732c0a416e6420746865206d6f6d65207261746873206f757467726162652e", "4541669a7eaaee61e708dc7cbcc5eb62" },
            new[] { "0200000000000000000000000000000000000000000000000000000000000000", "ffffffffffffffffffffffffffffffff", "03000000000000000000000000000000" },
            new[] { "02000000000000000000000000000000ffffffffffffffffffffffffffffffff", "02000000000000000000000000000000", "03000000000000000000000000000000" },
            new[] { "0100000000000000000000000000000000000000000000000000000000000000", "fffffffffffffffffffffffffffffffff0ffffffffffffffffffffffffffffff11000000000000000000000000000000", "05000000000000000000000000000000" },
            new[] { "0100000000000000000000000000000000000000000000000000000000000000", "fffffffffffffffffffffffffffffffffbfefefefefefefefefefefefefefefe01010101010101010101010101010101", "00000000000000000000000000000000" },
            new[] { "0200000000000000000000000000000000000000000000000000000000000000", "fdffffffffffffffffffffffffffffff", "faffffffffffffffffffffffffffffff" },
            new[] { "0100000000000000040000000000000000000000000000000000000000000000", "e33594d7505e43b900000000000000003394d7505e4379cd01000000000000000000000000000000000000000000000001000000000000000000000000000000", "14000000000000005500000000000000" },
            new[] { "0100000000000000040000000000000000000000000000000000000000000000", "e33594d7505e43b900000000000000003394d7505e4379cd010000000000000000000000000000000000000000000000", "13000000000000000000000000000000" }
        };

        [Test]
        public void Poly1305_Rfc8439_AppendixA3([Range(0, 10)] int in_index)
        {
            string[] v = POLY1305_VECTORS[in_index];
            byte[] text = H(v[1]);
            var whole = new Poly1305(H(v[0]), 0);
            whole.Update(text, 0, text.Length);
            Assert.That(Hex(whole.Finish()), Is.EqualTo(v[2]), "vector #" + (in_index + 1));

            // Fed in uneven pieces, which must not change the result
            var pieces = new Poly1305(H(v[0]), 0);
            for (int i = 0, step = 1; i < text.Length; i += step, step = step % 23 + 7)
            {
                pieces.Update(text, i, Math.Min(step, text.Length - i));
            }
            Assert.That(Hex(pieces.Finish()), Is.EqualTo(v[2]), "vector #" + (in_index + 1) + " in pieces");
        }

        [Test]
        public void Aead_Seal_Rfc8439_2_8_2()
        {
            byte[] plaintext = H("4c616469657320616e642047656e746c656d656e206f662074686520636c617373206f66202739393a204966204920636f756c64206f6666657220796f75206f6e6c79206f6e652074697020666f7220746865206675747572652c2073756e73637265656e20776f756c642062652069742e");
            byte[] sealedText = BrainCloud.Internal.ChaCha20Poly1305.Seal(H("808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9f"), H("070000004041424344454647"), H("50515253c0c1c2c3c4c5c6c7"), plaintext, 0, plaintext.Length);
            Assert.That(Hex(sealedText), Is.EqualTo("d31a8d34648e60db7b86afbc53ef7ec2a4aded51296e08fea9e2b5a736ee62d63dbea45e8ca9671282fafb69da92728b1a71de0a9e060b2905d6a5b67ecd3b3692ddbd7f2d778b8c9803aee328091b58fab324e4fad675945585808b4831d7bc3ff4def08e4b7a9de576d26586cec64b6116" + "1ae10b594f09e26a7e902ecbd0600691"));
        }

        [Test]
        public void Aead_Open_Rfc8439_AppendixA5()
        {
            byte[] received = H("64a0861575861af460f062c79be643bd5e805cfd345cf389f108670ac76c8cb24c6cfc18755d43eea09ee94e382d26b0bdb7b73c321b0100d4f03b7f355894cf332f830e710b97ce98c8a84abd0b948114ad176e008d33bd60f982b1ff37c8559797a06ef4f0ef61c186324e2b3506383606907b6a7c02b0f9f6157b53c867e4b9166c767b804d46a59b5216cde7a4e99040c5a40433225ee282a1b0a06c523eaf4534d7f83fa1155b0047718cbc546a0d072b04b3564eea1b422273f548271a0bb2316053fa76991955ebd63159434ecebb4e466dae5a1073a6727627097a1049e617d91d361094fa68f0ff77987130305beaba2eda04df997b714d6c6f2c29a6ad5cb4022b02709b" + "eead9d67890cbb22392336fea1851f38");
            byte[] plaintext = BrainCloud.Internal.ChaCha20Poly1305.Open(H("1c9240a5eb55d38af333888604f6b5f0473917c1402b80099dca5cbc207075c0"), H("000000000102030405060708"), H("f33388860000000000004e91"), received, 0, received.Length);
            Assert.That(plaintext, Is.Not.Null);
            Assert.That(Hex(plaintext), Is.EqualTo("496e7465726e65742d4472616674732061726520647261667420646f63756d656e74732076616c696420666f722061206d6178696d756d206f6620736978206d6f6e74687320616e64206d617920626520757064617465642c207265706c616365642c206f72206f62736f6c65746564206279206f7468657220646f63756d656e747320617420616e792074696d652e20497420697320696e617070726f70726961746520746f2075736520496e7465726e65742d447261667473206173207265666572656e6365206d6174657269616c206f7220746f2063697465207468656d206f74686572207468616e206173202fe2809c776f726b20696e2070726f67726573732e2fe2809d"));
            Assert.That(Encoding.ASCII.GetString(plaintext), Does.StartWith("Internet-Drafts are draft documents"));
        }

        [Test]
        public void Aead_AnyChangedByte_IsRefused()
        {
            byte[] key = Sequence(1, 32), nonce = Sequence(100, 12), aad = Sequence(200, 7), plaintext = Sequence(50, 40);
            byte[] sealedText = BrainCloud.Internal.ChaCha20Poly1305.Seal(key, nonce, aad, plaintext, 0, plaintext.Length);
            for (int i = 0; i < sealedText.Length; ++i)
            {
                byte[] changed = (byte[])sealedText.Clone();
                changed[i] ^= 0x01;
                Assert.That(BrainCloud.Internal.ChaCha20Poly1305.Open(key, nonce, aad, changed, 0, changed.Length), Is.Null, "byte " + i);
            }
            byte[] otherAad = (byte[])aad.Clone();
            otherAad[0] ^= 0x80;
            Assert.That(BrainCloud.Internal.ChaCha20Poly1305.Open(key, nonce, otherAad, sealedText, 0, sealedText.Length), Is.Null, "the associated data");
            Assert.That(BrainCloud.Internal.ChaCha20Poly1305.Open(key, Sequence(101, 12), aad, sealedText, 0, sealedText.Length), Is.Null, "the nonce");
            Assert.That(BrainCloud.Internal.ChaCha20Poly1305.Open(key, nonce, aad, sealedText, 0, 15), Is.Null, "shorter than a tag");
        }

        [Test]
        public void Aead_RoundTrip_EveryLengthUpToAFewBlocks()
        {
            var random = new Random(7);
            for (int length = 0; length <= 200; ++length)
            {
                byte[] key = new byte[32], nonce = new byte[12], aad = new byte[length % 19], plaintext = new byte[length];
                random.NextBytes(key); random.NextBytes(nonce); random.NextBytes(aad); random.NextBytes(plaintext);
                byte[] sealedText = BrainCloud.Internal.ChaCha20Poly1305.Seal(key, nonce, aad, plaintext, 0, length);
                Assert.That(sealedText.Length, Is.EqualTo(length + 16));
                byte[] opened = BrainCloud.Internal.ChaCha20Poly1305.Open(key, nonce, aad, sealedText, 0, sealedText.Length);
                Assert.That(opened, Is.EqualTo(plaintext), "length " + length);
            }
        }

        #endregion

        #region SHA-256, HMAC and HKDF

        [Test]
        public void Sha256_KnownAnswers()
        {
            Assert.That(Hex(Sha256.Hash(new byte[0])), Is.EqualTo("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"));
            Assert.That(Hex(Sha256.Hash(Encoding.ASCII.GetBytes("abc"))), Is.EqualTo("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad"));
            Assert.That(Hex(Sha256.Hash(Encoding.ASCII.GetBytes("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"))),
                Is.EqualTo("248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1"));
        }

        [Test]
        public void Sha256AndHmac_MatchThePlatform_ForEveryLengthAcrossTheBlockBoundaries()
        {
            // The library does not use the platform's cryptography, but the tests can check against it
            var random = new Random(11);
            using (var platformSha = System.Security.Cryptography.SHA256.Create())
            {
                for (int length = 0; length <= 200; ++length)
                {
                    byte[] data = new byte[length];
                    random.NextBytes(data);
                    Assert.That(Hex(Sha256.Hash(data)), Is.EqualTo(Hex(platformSha.ComputeHash(data))), "SHA-256 of " + length + " bytes");

                    byte[] key = new byte[(length * 7) % 150];
                    random.NextBytes(key);
                    using (var platformHmac = new System.Security.Cryptography.HMACSHA256(key))
                    {
                        Assert.That(Hex(HmacSha256.Compute(key, data)), Is.EqualTo(Hex(platformHmac.ComputeHash(data))),
                            "HMAC of " + length + " bytes with a " + key.Length + "-byte key");
                    }
                }
            }
        }

        [Test]
        public void Hkdf_Rfc5869_TestCases1To3()
        {
            Assert.That(Hex(HmacSha256.Hkdf(H("000102030405060708090a0b0c"), H("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b"), H("f0f1f2f3f4f5f6f7f8f9"), 42)),
                Is.EqualTo("3cb25f25faacd57a90434f64d0362f2a2d2d0a90cf1a5a4c5db02d56ecc4c5bf34007208d5b887185865"));
            Assert.That(Hex(HmacSha256.Hkdf(Sequence(0x60, 80), Sequence(0x00, 80), Sequence(0xb0, 80), 82)),
                Is.EqualTo("b11e398dc80327a1c8e7f78c596a49344f012eda2d4efad8a050cc4c19afa97c59045a99cac7827271cb41c65e590e09da3275600c2f09b8367793a9aca3db71cc30c58179ec3e87c14c01d5c1f3434f1d87"));
            Assert.That(Hex(HmacSha256.Hkdf(new byte[0], H("0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b0b"), new byte[0], 42)),
                Is.EqualTo("8da4e775a563c18f715f802a063c5a31b8a11f5c5ee1879ec3454e5f3c738d2d9d201395faa4b61a96c8"));
        }

        #endregion

        #region The direct connect datagram

        // Fixed values every client library's implementation must reproduce. The keys were computed
        // independently (Python's hmac and hashlib) and the datagram with .NET's own ChaCha20-Poly1305.
        private static readonly byte[] TOKEN = Sequence(0, 16);
        private const string KEY_0_TO_1 = "246699f49576f5e494f9ee0f8d158a2fdb9cfd24f9bb381b94f5528356ae7093";
        private const string KEY_1_TO_0 = "07f331df147f3bffaaf8f102917be5d903abd7ce332bbeec31ee6dcf85e7e0a8";
        private const string KEY_3_TO_7 = "0ebc8dd6b6c74b9fc1d7f4ab820f5cee6706149b8e5efc876cd34cba67f84c1b";
        // A PUNCH (type 16) with punch id 1, from net id 0 to net id 1, sequence 5
        private const string PUNCH_DATAGRAM = "b100010000000000000005ec1aeb2e7f00db64c4cba904dcb3a6a18ab10085a9";

        [Test]
        public void DeriveKey_OneKeyPerDirection_FixedValues()
        {
            Assert.That(Hex(DirectPacket.DeriveKey(TOKEN, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1)), Is.EqualTo(KEY_0_TO_1));
            Assert.That(Hex(DirectPacket.DeriveKey(TOKEN, DirectPacket.CIPHER_CHACHA20_POLY1305, 1, 0)), Is.EqualTo(KEY_1_TO_0));
            Assert.That(Hex(DirectPacket.DeriveKey(TOKEN, DirectPacket.CIPHER_CHACHA20_POLY1305, 3, 7)), Is.EqualTo(KEY_3_TO_7));
        }

        [Test]
        public void DeriveKey_RefusesAnUnknownCipherOrBadInput()
        {
            Assert.That(() => DirectPacket.DeriveKey(TOKEN, "aes-128-gcm", 0, 1), Throws.ArgumentException);
            Assert.That(() => DirectPacket.DeriveKey(new byte[0], DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1), Throws.ArgumentException);
            Assert.That(() => DirectPacket.DeriveKey(TOKEN, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 256), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Seal_FixedDatagram()
        {
            byte[] punch = { 16, 0, 0, 0, 1 };
            byte[] datagram = DirectPacket.Seal(H(KEY_0_TO_1), 0, 1, 5, punch, 0, punch.Length);
            Assert.That(Hex(datagram), Is.EqualTo(PUNCH_DATAGRAM));
            Assert.That(datagram.Length, Is.EqualTo(punch.Length + DirectPacket.OVERHEAD));
        }

        [Test]
        public void Open_ReadsTheHeaderAndThePacket()
        {
            byte[] datagram = H(PUNCH_DATAGRAM);
            int sender, receiver;
            ulong sequence;
            Assert.That(DirectPacket.TryReadHeader(datagram, datagram.Length, out sender, out receiver, out sequence), Is.True);
            Assert.That(sender, Is.EqualTo(0));
            Assert.That(receiver, Is.EqualTo(1));
            Assert.That(sequence, Is.EqualTo(5UL));
            Assert.That(DirectPacket.Open(H(KEY_0_TO_1), datagram, datagram.Length), Is.EqualTo(new byte[] { 16, 0, 0, 0, 1 }));
        }

        [Test]
        public void Open_RefusesTheOtherDirectionsKeyAndAnyChange()
        {
            byte[] datagram = H(PUNCH_DATAGRAM);
            // A packet cannot be bounced back to its sender: the other direction has another key
            Assert.That(DirectPacket.Open(H(KEY_1_TO_0), datagram, datagram.Length), Is.Null);
            for (int i = 1; i < datagram.Length; ++i)
            {
                byte[] changed = (byte[])datagram.Clone();
                changed[i] ^= 0x01;
                Assert.That(DirectPacket.Open(H(KEY_0_TO_1), changed, changed.Length), Is.Null, "byte " + i);
            }
        }

        [Test]
        public void TryReadHeader_RefusesOtherDatagrams()
        {
            int sender, receiver;
            ulong sequence;
            byte[] datagram = H(PUNCH_DATAGRAM);
            byte[] stun = (byte[])datagram.Clone();
            stun[0] = 0x01;   // a STUN Binding response starts 0x01 0x01
            Assert.That(DirectPacket.TryReadHeader(stun, stun.Length, out sender, out receiver, out sequence), Is.False, "another format");
            Assert.That(DirectPacket.TryReadHeader(datagram, DirectPacket.OVERHEAD - 1, out sender, out receiver, out sequence), Is.False, "too short");
            Assert.That(DirectPacket.TryReadHeader(null, 0, out sender, out receiver, out sequence), Is.False);
        }

        [Test]
        public void Seal_SequenceIsBigEndianInTheHeader()
        {
            byte[] datagram = DirectPacket.Seal(H(KEY_0_TO_1), 0, 1, 0x0102030405060708UL, new byte[0], 0, 0);
            Assert.That(Hex(datagram.Take(DirectPacket.HEADER_LENGTH).ToArray()), Is.EqualTo("b100010102030405060708"));
            int sender, receiver;
            ulong sequence;
            DirectPacket.TryReadHeader(datagram, datagram.Length, out sender, out receiver, out sequence);
            Assert.That(sequence, Is.EqualTo(0x0102030405060708UL));
        }

        #endregion

        #region Replay window

        [Test]
        public void ReplayWindow_DropsRepeatsAndTheTooOld()
        {
            var window = new ReplayWindow();
            Assert.That(window.IsNew(0), Is.True);
            window.Accept(0);
            Assert.That(window.IsNew(0), Is.False, "a repeat");

            window.Accept(10);
            Assert.That(window.IsNew(5), Is.True, "late but within the window");
            window.Accept(5);
            Assert.That(window.IsNew(5), Is.False);
            Assert.That(window.IsNew(10), Is.False);
            Assert.That(window.IsNew(11), Is.True);

            window.Accept(100);
            Assert.That(window.IsNew(100 - ReplayWindow.SIZE), Is.False, "too old");
            Assert.That(window.IsNew(100 - ReplayWindow.SIZE + 1), Is.True, "the oldest still in the window");
            Assert.That(window.IsNew(10), Is.False, "fell out of the window");
        }

        [Test]
        public void ReplayWindow_StartsAnywhere_AndJumpsAhead()
        {
            var window = new ReplayWindow();
            window.Accept(1000);
            Assert.That(window.IsNew(999), Is.True, "below the first one seen, within the window");
            window.Accept(999);
            Assert.That(window.IsNew(999), Is.False);

            window.Accept(1000 + 1000);
            Assert.That(window.IsNew(1000), Is.False, "now too old");
            Assert.That(window.IsNew(1999), Is.True);
            Assert.That(window.IsNew(2000), Is.False);
        }

        #endregion
    }
}
