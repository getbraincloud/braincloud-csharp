// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.Internal;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        private static readonly byte[] PAIR_KEY = Sequence(0, 16);
        private static readonly byte[] EPOCH = Sequence(0x10, 8);
        private const string KEY_0_TO_1 = "2204a6dd79af04b43d319cab844520fd9406438526448b81e075b7198e9e45c1";
        private const string KEY_1_TO_0 = "4ffdc8cbd25c2fd112b06daae019d3871bab182bbd2d35562ed4304483feb29f";
        private const string KEY_3_TO_7 = "2ccba3754420ea1da0c41b9b6bf6525dc83c3852878cfc6f0f8e9ff8ff791773";
        private const string KEY_0_TO_1_OTHER_EPOCH = "c13baac947ab539a2a24c90ab1522c86f9112d6e40e35310d193df201d7fff75";
        // A PUNCH (type 16) with punch id 1, from net id 0 to net id 1, in EPOCH, sequence 5
        private const string PUNCH_DATAGRAM = "b100011011121314151617000000000000000519918d80a24f673f459f5f353173c9685a6e6e2e40";
        private static readonly byte[] PUNCH = { 16, 0, 0, 0, 1 };
        // hmac-sha256: the key from 0 to 1, and the same PUNCH datagram, authenticated only
        private const string KEY_HMAC_0_TO_1 = "0c3cf0a6686f46f4848d36128969a1bc1ca024809751e8ba972b2d4f42552732";
        private const string PUNCH_DATAGRAM_HMAC = "b100011011121314151617000000000000000510000000018b81b3f2d111c7a9e48463375ac28c13";

        [Test]
        public void DeriveKey_OneKeyPerDirectionAndEpoch_FixedValues()
        {
            string cipher = DirectPacket.CIPHER_CHACHA20_POLY1305;
            Assert.That(Hex(DirectPacket.DeriveKey(PAIR_KEY, cipher, 0, 1, EPOCH)), Is.EqualTo(KEY_0_TO_1));
            Assert.That(Hex(DirectPacket.DeriveKey(PAIR_KEY, cipher, 1, 0, EPOCH)), Is.EqualTo(KEY_1_TO_0));
            Assert.That(Hex(DirectPacket.DeriveKey(PAIR_KEY, cipher, 3, 7, EPOCH)), Is.EqualTo(KEY_3_TO_7));
            Assert.That(Hex(DirectPacket.DeriveKey(PAIR_KEY, cipher, 0, 1, Sequence(0x20, 8))), Is.EqualTo(KEY_0_TO_1_OTHER_EPOCH));
        }

        [Test]
        public void DeriveKey_RefusesAnUnknownCipherOrBadInput()
        {
            string cipher = DirectPacket.CIPHER_CHACHA20_POLY1305;
            Assert.That(() => DirectPacket.DeriveKey(PAIR_KEY, "aes-128-gcm", 0, 1, EPOCH), Throws.ArgumentException);
            Assert.That(() => DirectPacket.DeriveKey(Sequence(0, 4), cipher, 0, 1, EPOCH), Throws.ArgumentException, "a short key could be guessed");
            Assert.That(() => DirectPacket.DeriveKey(Sequence(0, 17), cipher, 0, 1, EPOCH), Throws.ArgumentException);
            Assert.That(() => DirectPacket.DeriveKey(PAIR_KEY, cipher, 0, 1, Sequence(0, 7)), Throws.ArgumentException);
            Assert.That(() => DirectPacket.DeriveKey(PAIR_KEY, cipher, 0, 256, EPOCH), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Seal_FixedDatagram()
        {
            byte[] datagram = DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, EPOCH, 5, PUNCH, 0, PUNCH.Length);
            Assert.That(Hex(datagram), Is.EqualTo(PUNCH_DATAGRAM));
            Assert.That(datagram.Length, Is.EqualTo(PUNCH.Length + DirectPacket.OVERHEAD));
            Assert.That(DirectPacket.OVERHEAD, Is.EqualTo(35));
        }

        [Test]
        public void Seal_FromAnOffset()
        {
            byte[] buffer = new byte[20];
            Buffer.BlockCopy(PUNCH, 0, buffer, 9, PUNCH.Length);
            Assert.That(Hex(DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, EPOCH, 5, buffer, 9, PUNCH.Length)), Is.EqualTo(PUNCH_DATAGRAM));
        }

        [Test]
        public void Open_ReadsTheHeaderAndThePacket_FromABiggerReceiveBuffer()
        {
            byte[] datagram = H(PUNCH_DATAGRAM);
            byte[] buffer = new byte[1500];
            Buffer.BlockCopy(datagram, 0, buffer, 0, datagram.Length);

            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            Assert.That(DirectPacket.TryReadHeader(buffer, datagram.Length, out sender, out receiver, out epoch, out sequence), Is.True);
            Assert.That(sender, Is.EqualTo(0));
            Assert.That(receiver, Is.EqualTo(1));
            Assert.That(epoch, Is.EqualTo(EPOCH));
            Assert.That(sequence, Is.EqualTo(5UL));
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, buffer, datagram.Length), Is.EqualTo(PUNCH));
        }

        [Test]
        public void Open_RefusesOtherIdsTheOtherDirectionsKeyAndAnyChange()
        {
            byte[] datagram = H(PUNCH_DATAGRAM);
            // The ids the key belongs to are checked against the header before any decrypting
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 2, datagram, datagram.Length), Is.Null, "addressed to someone else");
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 3, 1, datagram, datagram.Length), Is.Null, "from someone else");
            // A packet cannot be bounced back to its sender: the other direction has another key
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_1_TO_0), 0, 1, datagram, datagram.Length), Is.Null);
            for (int i = 1; i < datagram.Length; ++i)
            {
                byte[] changed = (byte[])datagram.Clone();
                changed[i] ^= 0x01;
                Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, changed, changed.Length), Is.Null, "byte " + i);
            }
        }

        [Test]
        public void TryReadHeader_RefusesOtherDatagramsAndBadLengths()
        {
            int sender, receiver;
            byte[] epoch;
            ulong sequence;
            byte[] datagram = H(PUNCH_DATAGRAM);
            byte[] stun = (byte[])datagram.Clone();
            stun[0] = 0x01;   // a STUN Binding response starts 0x01 0x01
            Assert.That(DirectPacket.TryReadHeader(stun, stun.Length, out sender, out receiver, out epoch, out sequence), Is.False, "another format");
            Assert.That(DirectPacket.TryReadHeader(datagram, DirectPacket.OVERHEAD - 1, out sender, out receiver, out epoch, out sequence), Is.False, "too short");
            Assert.That(DirectPacket.TryReadHeader(datagram, datagram.Length + 1, out sender, out receiver, out epoch, out sequence), Is.False, "longer than the buffer");
            Assert.That(DirectPacket.TryReadHeader(null, 0, out sender, out receiver, out epoch, out sequence), Is.False);
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, datagram, datagram.Length + 1), Is.Null, "longer than the buffer");
        }

        [Test]
        public void Seal_SequenceIsBigEndianInTheHeader()
        {
            byte[] datagram = DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, EPOCH, 0x0102030405060708UL, new byte[0], 0, 0);
            Assert.That(Hex(datagram.Take(DirectPacket.HEADER_LENGTH).ToArray()), Is.EqualTo("b10001" + "1011121314151617" + "0102030405060708"));
        }

        #endregion

        #region Sender and receiver

        [Test]
        public void Sender_CountsUpFromZero_AndTheReceiverTakesEachOnce()
        {
            var sender = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, EPOCH);
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);

            byte[] first = sender.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(Hex(first), Is.Not.EqualTo(PUNCH_DATAGRAM), "sequence 0, not 5");
            byte[] second = sender.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(sender.NextSequence, Is.EqualTo(2UL));

            Assert.That(receiver.Open(second, second.Length), Is.EqualTo(PUNCH), "out of order is fine");
            Assert.That(receiver.Open(first, first.Length), Is.EqualTo(PUNCH));
            Assert.That(receiver.Open(first, first.Length), Is.Null, "a repeat");
            Assert.That(receiver.Open(second, second.Length), Is.Null, "a repeat");
        }

        /// A sender from in_sender to in_receiver in an attempt of its own, with in_key as their pair key.
        private static DirectSender FreshSender(byte[] in_key, int in_sender, int in_receiver)
        {
            var keys = new Dictionary<int, byte[]> { { in_receiver, in_key } };
            return new DirectSession(DirectPacket.CIPHER_CHACHA20_POLY1305, in_sender, keys).SenderFor(in_receiver);
        }

        /// The key net ids in_a and in_b share in these tests: PAIR_KEY for 0 and 1, and a different one for every other pair.
        private static byte[] PairKeyFor(int in_a, int in_b)
        {
            byte[] key = (byte[])PAIR_KEY.Clone();
            key[15] ^= (byte)(16 * Math.Min(in_a, in_b) + Math.Max(in_a, in_b) - 1);
            return key;
        }

        /// A session for net id in_me with each of in_peers, each pair with its own key.
        private static DirectSession Session(int in_me, params int[] in_peers)
        {
            return new DirectSession(DirectPacket.CIPHER_CHACHA20_POLY1305, in_me, in_peers.ToDictionary(peer => peer, peer => PairKeyFor(in_me, peer)));
        }

        private static byte[] EpochOf(byte[] in_datagram)
        {
            return in_datagram.Skip(3).Take(DirectPacket.EPOCH_LENGTH).ToArray();
        }

        [Test]
        public void Session_EachNewSenderHasItsOwnEpoch()
        {
            var session = Session(0, 1, 2);
            var a = session.SenderFor(1);
            Assert.That(session.SenderFor(1), Is.SameAs(a), "the current sender until it is restarted");
            var b = session.RestartSender(1);
            byte[] fromB = b.Seal(PUNCH, 0, PUNCH.Length);
            var toOther = session.SenderFor(2);
            byte[] fromOther = toOther.Seal(PUNCH, 0, PUNCH.Length);
            var c = session.RestartSender(1);
            byte[] fromC = c.Seal(PUNCH, 0, PUNCH.Length);

            Assert.That(DirectPacket.SamePrefix(EpochOf(fromB), EpochOf(fromC)), Is.True, "one prefix for the attempt");
            Assert.That(DirectPacket.EpochCount(EpochOf(fromC)), Is.GreaterThan(DirectPacket.EpochCount(EpochOf(fromB))));
            Assert.That(DirectPacket.EpochCount(EpochOf(fromOther)), Is.Not.EqualTo(DirectPacket.EpochCount(EpochOf(fromB))));
            // Same sequence number, different key: the same nonce is never used twice with one key
            Assert.That(Hex(fromB.Skip(DirectPacket.HEADER_LENGTH).ToArray()), Is.Not.EqualTo(Hex(fromC.Skip(DirectPacket.HEADER_LENGTH).ToArray())));
        }

        [Test]
        public void Session_ARestartedSenderRefusesToSeal()
        {
            var session = Session(0, 1);
            var old = session.SenderFor(1);
            old.Seal(PUNCH, 0, PUNCH.Length);
            var replacement = session.RestartSender(1);
            Assert.That(() => old.Seal(PUNCH, 0, PUNCH.Length), Throws.InvalidOperationException);
            Assert.That(session.SenderFor(1), Is.SameAs(replacement));
            Assert.That(() => replacement.Seal(PUNCH, 0, PUNCH.Length), Throws.Nothing);
        }

        [Test]
        public void Session_HasOneReceiverPerPeer()
        {
            var session = Session(1, 0, 2);
            Assert.That(session.ReceiverFor(0), Is.SameAs(session.ReceiverFor(0)));
            Assert.That(session.ReceiverFor(2), Is.Not.SameAs(session.ReceiverFor(0)));

            // So a datagram already received cannot be replayed to it, even after a restart on either side
            var peer = Session(0, 1);
            byte[] first = peer.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(session.ReceiverFor(0).Open(first, first.Length), Is.EqualTo(PUNCH));
            Assert.That(session.ReceiverFor(0).Open(first, first.Length), Is.Null);
            byte[] restarted = peer.RestartSender(1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(session.ReceiverFor(0).Open(restarted, restarted.Length), Is.EqualTo(PUNCH));
            Assert.That(session.ReceiverFor(0).Open(first, first.Length), Is.Null);
        }

        [TestCase(DirectPacket.CIPHER_CHACHA20_POLY1305)]
        [TestCase(DirectPacket.CIPHER_HMAC_SHA256)]
        public void Session_NoMemberCanPassItselfOffAsAnother(string cipher)
        {
            // Three players, each pair with its own key, as DIRECT_CONNECT gives them
            byte[] ab = Sequence(0x40, 16), ac = Sequence(0x50, 16), bc = Sequence(0x60, 16);
            var a = new DirectSession(cipher, 0, new Dictionary<int, byte[]> { { 1, ab }, { 2, ac } });
            var b = new DirectSession(cipher, 1, new Dictionary<int, byte[]> { { 0, ab }, { 2, bc } });
            var c = new DirectSession(cipher, 2, new Dictionary<int, byte[]> { { 0, ac }, { 1, bc } });

            byte[] aToB = a.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(b.ReceiverFor(0).Open(aToB, aToB.Length), Is.EqualTo(PUNCH));
            byte[] cToB = c.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(b.ReceiverFor(2).Open(cToB, cToB.Length), Is.EqualTo(PUNCH));

            // C has a key with each of them, but not theirs. It forges "A to B" as well as it can: A's
            // own epoch prefix, read off the wire, and a higher count, which B would take from A.
            // Neither of its keys makes one B accepts, and neither opens what A sends B.
            byte[] aPrefix = EpochOf(aToB).Take(4).ToArray();
            byte[] forgedEpoch = DirectPacket.MakeEpoch(aPrefix, DirectPacket.EpochCount(EpochOf(aToB)) + 1000);
            foreach (byte[] key in new[] { ac, bc })
            {
                byte[] forged = new DirectSender(key, cipher, 0, 1, forgedEpoch).Seal(PUNCH, 0, PUNCH.Length);
                Assert.That(b.ReceiverFor(0).Open(forged, forged.Length), Is.Null);
                Assert.That(new DirectReceiver(key, cipher, 0, 1).Open(aToB, aToB.Length), Is.Null);
            }
            byte[] next = a.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(b.ReceiverFor(0).Open(next, next.Length), Is.EqualTo(PUNCH), "B still takes A's packets");

            // The check above is a real one: the same forgery made with A and B's own key is taken
            byte[] withTheirKey = new DirectSender(ab, cipher, 0, 1, forgedEpoch).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(b.ReceiverFor(0).Open(withTheirKey, withTheirKey.Length), Is.EqualTo(PUNCH));
        }

        [Test]
        public void Session_PrefixIsRandomPerAttempt()
        {
            byte[] a = EpochOf(FreshSender(PAIR_KEY, 0, 1).Seal(PUNCH, 0, PUNCH.Length));
            byte[] b = EpochOf(FreshSender(PAIR_KEY, 0, 1).Seal(PUNCH, 0, PUNCH.Length));
            Assert.That(DirectPacket.SamePrefix(a, b), Is.False, "a 1 in 2^32 chance of a false failure");
            Assert.That(DirectPacket.EpochCount(a), Is.EqualTo(1u));
            Assert.That(DirectPacket.MakeEpoch(PREFIX, 0x01020304), Is.EqualTo(new byte[] { 0xA1, 0xB2, 0xC3, 0xD4, 1, 2, 3, 4 }));
        }

        [Test]
        public void Session_RefusesWhatIsNotAnAttempt()
        {
            string cipher = DirectPacket.CIPHER_CHACHA20_POLY1305;
            Assert.That(() => new DirectSession(cipher, 0, new Dictionary<int, byte[]> { { 1, new byte[15] } }), Throws.ArgumentException, "a short key");
            Assert.That(() => new DirectSession("aes-256-gcm", 0, new Dictionary<int, byte[]> { { 1, PAIR_KEY } }), Throws.ArgumentException);
            Assert.That(() => new DirectSession(cipher, 256, new Dictionary<int, byte[]> { { 1, PAIR_KEY } }), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new DirectSession(cipher, 0, new Dictionary<int, byte[]> { { 256, PAIR_KEY } }), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new DirectSession(cipher, 0, new Dictionary<int, byte[]>()), Throws.ArgumentException, "no peers");
            Assert.That(() => new DirectSession(cipher, 0, null), Throws.ArgumentException);
            Assert.That(() => new DirectSession(cipher, 0, new Dictionary<int, byte[]> { { 0, PAIR_KEY } }), Throws.ArgumentException, "itself as a peer");
            Assert.That(() => new DirectSession(cipher, 0, new Dictionary<int, byte[]> { { 1, PAIR_KEY }, { 2, (byte[])PAIR_KEY.Clone() } }), Throws.ArgumentException,
                "the same key for two peers: a secret shared beyond a pair");

            // Only the peers it was given
            var session = Session(0, 1, 3);
            Assert.That(session.IsPeer(1) && session.IsPeer(3), Is.True);
            Assert.That(session.IsPeer(2) || session.IsPeer(0), Is.False);
            Assert.That(() => session.ReceiverFor(2), Throws.ArgumentException);
            Assert.That(() => session.SenderFor(2), Throws.ArgumentException);
            Assert.That(() => session.RestartSender(2), Throws.ArgumentException);
            Assert.That(() => session.ReceiverFor(0), Throws.ArgumentException, "not from itself either");
            Assert.That(DirectPacket.SupportedCiphers, Is.EqualTo(new[] { DirectPacket.CIPHER_CHACHA20_POLY1305, DirectPacket.CIPHER_HMAC_SHA256 }), "most preferred first");
            Assert.That(DirectPacket.IsSupported(DirectPacket.CIPHER_CHACHA20_POLY1305), Is.True);
            Assert.That(DirectPacket.IsSupported(DirectPacket.CIPHER_HMAC_SHA256), Is.True);
            Assert.That(DirectPacket.IsSupported("aes-256-gcm"), Is.False);
            Assert.That(DirectPacket.IsSupported(null), Is.False);
        }

        [Test]
        public void Receiver_FollowsANewEpoch_AndNeverGoesBack()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            var before = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, DirectPacket.MakeEpoch(PREFIX, 1));
            byte[] old1 = before.Seal(PUNCH, 0, PUNCH.Length), old2 = before.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(old1, old1.Length), Is.EqualTo(PUNCH));

            // The peer started sending again: a new epoch, from sequence 0
            var after = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, DirectPacket.MakeEpoch(PREFIX, 2));
            byte[] new1 = after.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(new1, new1.Length), Is.EqualTo(PUNCH));

            // The old epoch is behind: neither a replay nor an unseen datagram of it is taken now
            Assert.That(receiver.Open(old1, old1.Length), Is.Null);
            Assert.That(receiver.Open(old2, old2.Length), Is.Null);
            Assert.That(receiver.Open(new1, new1.Length), Is.Null, "a repeat in the new epoch");
        }

        private static readonly byte[] PREFIX = { 0xA1, 0xB2, 0xC3, 0xD4 };

        private static byte[] SealIn(uint in_count, out DirectSender out_sender)
        {
            out_sender = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, DirectPacket.MakeEpoch(PREFIX, in_count));
            return out_sender.Seal(PUNCH, 0, PUNCH.Length);
        }

        [Test]
        public void Receiver_NeverGoesBack_EvenToAnEpochItNeverSaw()
        {
            // The peer restarted twice. Epoch 2's datagrams were held back (dropped and recorded by
            // an attacker, or just delayed) and arrive after epoch 3's: they must not take over.
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            DirectSender s1, s2, s3;
            byte[] e1 = SealIn(1, out s1), e2a = SealIn(2, out s2), e3a = SealIn(3, out s3);
            byte[] e2b = s2.Seal(PUNCH, 0, PUNCH.Length), e3b = s3.Seal(PUNCH, 0, PUNCH.Length);

            Assert.That(receiver.Open(e1, e1.Length), Is.EqualTo(PUNCH));
            Assert.That(receiver.Open(e3a, e3a.Length), Is.EqualTo(PUNCH));
            Assert.That(receiver.Open(e2a, e2a.Length), Is.Null, "an earlier epoch, never seen");
            Assert.That(receiver.Open(e2b, e2b.Length), Is.Null);
            Assert.That(receiver.Open(e3b, e3b.Length), Is.EqualTo(PUNCH), "still on the latest epoch");
        }

        [Test]
        public void Receiver_KeepsThePrefixItFirstAccepted()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            DirectSender s1;
            byte[] first = SealIn(5, out s1);
            Assert.That(receiver.Open(first, first.Length), Is.EqualTo(PUNCH));

            // Another process's prefix, even with a higher count and the right key, is not this peer
            byte[] other = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1,
                DirectPacket.MakeEpoch(new byte[] { 1, 2, 3, 4 }, 99)).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(other, other.Length), Is.Null);
            byte[] next = s1.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(next, next.Length), Is.EqualTo(PUNCH));
        }

        [Test]
        public void Receiver_AForgedDatagramDoesNotTakeTheSequenceOfTheGenuineOne()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            DirectSender sender;
            byte[] first = SealIn(1, out sender);
            Assert.That(receiver.Open(first, first.Length), Is.EqualTo(PUNCH));

            byte[] genuine = sender.Seal(PUNCH, 0, PUNCH.Length);
            byte[] forged = (byte[])genuine.Clone();
            forged[DirectPacket.HEADER_LENGTH] ^= 0x01;   // same epoch and sequence, changed ciphertext
            Assert.That(receiver.Open(forged, forged.Length), Is.Null);
            Assert.That(receiver.Open(genuine, genuine.Length), Is.EqualTo(PUNCH), "the window only records what decrypted");
        }

        [Test]
        public void Receiver_RefusesAForgedHigherCount()
        {
            // The right prefix and a higher count is what a genuine restart looks like, so the receiver
            // has to derive that epoch's key and try. A forgery must fail there and change nothing.
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            DirectSender sender;
            byte[] first = SealIn(1, out sender);
            Assert.That(receiver.Open(first, first.Length), Is.EqualTo(PUNCH));

            byte[] genuine = sender.Seal(PUNCH, 0, PUNCH.Length);
            byte[] forged = (byte[])genuine.Clone();
            forged[3 + DirectPacket.EPOCH_LENGTH - 1] = 2;   // the count, 1 -> 2
            Assert.That(receiver.Open(forged, forged.Length), Is.Null);
            Assert.That(receiver.Open(genuine, genuine.Length), Is.EqualTo(PUNCH), "still on epoch 1");

            DirectSender next;
            byte[] restart = SealIn(2, out next);
            Assert.That(receiver.Open(restart, restart.Length), Is.EqualTo(PUNCH), "the real epoch 2 is still taken");
        }

        [Test]
        public void Receiver_ANewEpochStartsAFreshWindow()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            DirectSender s1, s2;
            byte[] a0 = SealIn(1, out s1), a1 = s1.Seal(PUNCH, 0, PUNCH.Length), a2 = s1.Seal(PUNCH, 0, PUNCH.Length);
            foreach (var d in new[] { a0, a1, a2 }) Assert.That(receiver.Open(d, d.Length), Is.EqualTo(PUNCH));

            // The same sequence numbers again, in the next epoch: new datagrams, not repeats
            byte[] b0 = SealIn(2, out s2), b1 = s2.Seal(PUNCH, 0, PUNCH.Length), b2 = s2.Seal(PUNCH, 0, PUNCH.Length);
            foreach (var d in new[] { b1, b0, b2 }) Assert.That(receiver.Open(d, d.Length), Is.EqualTo(PUNCH));
            foreach (var d in new[] { b0, b1, b2 }) Assert.That(receiver.Open(d, d.Length), Is.Null, "now they are repeats");
        }

        [Test]
        public void Receiver_FromManyThreads_TakesEachDatagramOnce()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            var sender = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, EPOCH);
            byte[][] datagrams = Enumerable.Range(0, 64).Select(i => sender.Seal(PUNCH, 0, PUNCH.Length)).ToArray();
            int accepted = 0;
            System.Threading.Tasks.Parallel.For(0, 64 * 8, i =>
            {
                byte[] d = datagrams[i % 64];
                if (receiver.Open(d, d.Length) != null) System.Threading.Interlocked.Increment(ref accepted);
            });
            Assert.That(accepted, Is.EqualTo(64));
        }

        [Test]
        public void Datagram_LongestIsTheMaximum()
        {
            int most = DirectPacket.MAX_DATAGRAM_LENGTH - DirectPacket.OVERHEAD;
            byte[] key = H(KEY_0_TO_1);
            byte[] longest = DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, key, 0, 1, EPOCH, 0, new byte[most], 0, most);
            Assert.That(longest.Length, Is.EqualTo(DirectPacket.MAX_DATAGRAM_LENGTH));
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, key, 0, 1, longest, longest.Length), Is.EqualTo(new byte[most]));
            Assert.That(() => DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, key, 0, 1, EPOCH, 0, new byte[most + 1], 0, most + 1), Throws.ArgumentException);

            byte[] tooLong = new byte[DirectPacket.MAX_DATAGRAM_LENGTH + 1];
            Buffer.BlockCopy(longest, 0, tooLong, 0, longest.Length);
            int s, r;
            byte[] epoch;
            ulong sequence;
            Assert.That(DirectPacket.TryReadHeader(tooLong, tooLong.Length, out s, out r, out epoch, out sequence), Is.False);
            Assert.That(DirectPacket.TryReadHeader(tooLong, longest.Length, out s, out r, out epoch, out sequence), Is.True, "only the length received counts");
        }

        [Test]
        public void Sender_FromManyThreads_NeverRepeatsASequence()
        {
            var sender = FreshSender(PAIR_KEY, 0, 1);
            var seen = new System.Collections.Concurrent.ConcurrentDictionary<ulong, bool>();
            System.Threading.Tasks.Parallel.For(0, 2000, i =>
            {
                byte[] datagram = sender.Seal(PUNCH, 0, PUNCH.Length);
                int s, r;
                byte[] epoch;
                ulong sequence;
                DirectPacket.TryReadHeader(datagram, datagram.Length, out s, out r, out epoch, out sequence);
                Assert.That(seen.TryAdd(sequence, true), Is.True, "sequence " + sequence + " used twice");
            });
            Assert.That(seen.Count, Is.EqualTo(2000));
        }

        [Test]
        public void Receiver_RefusesOtherPeersAndForgeries()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1);
            byte[] toSomeoneElse = FreshSender(PAIR_KEY, 0, 2).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(toSomeoneElse, toSomeoneElse.Length), Is.Null);
            byte[] ownPacket = FreshSender(PAIR_KEY, 1, 0).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(ownPacket, ownPacket.Length), Is.Null, "this player's own packet reflected back");
            byte[] otherKey = FreshSender(Sequence(1, 16), 0, 1).Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(otherKey, otherKey.Length), Is.Null, "another pair's key");

            // A forged datagram in a new epoch must not move the receiver off the real one
            var real = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_CHACHA20_POLY1305, 0, 1, EPOCH);
            byte[] r1 = real.Seal(PUNCH, 0, PUNCH.Length), r2 = real.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(r1, r1.Length), Is.EqualTo(PUNCH));
            byte[] forged = (byte[])r2.Clone();
            forged[3] ^= 0xFF;   // a different epoch, with a tag that cannot match
            Assert.That(receiver.Open(forged, forged.Length), Is.Null);
            Assert.That(receiver.Open(r2, r2.Length), Is.EqualTo(PUNCH), "still on the real epoch");
        }

        #endregion

        #region Cipher implementation

        /// The library's own implementation, counting its use.
        private sealed class CountingAead : IDirectAead
        {
            public int Seals, Opens;
            private readonly ManagedChaCha20Poly1305 m_inner = new ManagedChaCha20Poly1305();

            public byte[] Seal(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_plaintext, int in_offset, int in_length)
            {
                ++Seals;
                return m_inner.Seal(in_key, in_nonce, in_aad, in_plaintext, in_offset, in_length);
            }

            public byte[] Open(byte[] in_key, byte[] in_nonce, byte[] in_aad, byte[] in_sealed, int in_offset, int in_length)
            {
                ++Opens;
                return m_inner.Open(in_key, in_nonce, in_aad, in_sealed, in_offset, in_length);
            }
        }

        [Test]
        public void Cipher_EverySealAndOpenUsesTheInstalledImplementation()
        {
            IDirectAead original = DirectCipher.ChaCha20Poly1305Implementation;
            var counting = new CountingAead();
            try
            {
                DirectCipher.ChaCha20Poly1305Implementation = counting;
                var a = Session(0, 1);
                var b = Session(1, 0);
                byte[] datagram = a.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
                Assert.That(b.ReceiverFor(0).Open(datagram, datagram.Length), Is.EqualTo(PUNCH));
                Assert.That(counting.Seals, Is.EqualTo(1));
                Assert.That(counting.Opens, Is.EqualTo(1));

                // And it is the same cipher: the fixed datagram is unchanged
                Assert.That(Hex(DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, EPOCH, 5, PUNCH, 0, PUNCH.Length)), Is.EqualTo(PUNCH_DATAGRAM));
            }
            finally
            {
                DirectCipher.ChaCha20Poly1305Implementation = original;
            }
        }

        [Test]
        public void Cipher_WithNoEncryption_OnlyAuthenticationIsSupportedAndNothingEncrypts()
        {
            // As in a game that does not define BRAINCLOUD_DIRECT_CONNECT_ENCRYPTION
            IDirectAead original = DirectCipher.ChaCha20Poly1305Implementation;
            try
            {
                DirectCipher.ChaCha20Poly1305Implementation = null;
                Assert.That(DirectPacket.SupportedCiphers, Is.EqualTo(new[] { DirectPacket.CIPHER_HMAC_SHA256 }));
                Assert.That(DirectPacket.IsSupported(DirectPacket.CIPHER_CHACHA20_POLY1305), Is.False);
                Assert.That(() => Session(0, 1), Throws.ArgumentException);
                Assert.That(() => DirectPacket.Seal(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, EPOCH, 5, PUNCH, 0, PUNCH.Length), Throws.ArgumentException);
                Assert.That(() => DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_0_TO_1), 0, 1, H(PUNCH_DATAGRAM), PUNCH_DATAGRAM.Length / 2), Throws.ArgumentException);

                // Authenticated direct connect still works
                var a = new DirectSession(DirectPacket.CIPHER_HMAC_SHA256, 0, new Dictionary<int, byte[]> { { 1, PAIR_KEY } });
                var b = new DirectSession(DirectPacket.CIPHER_HMAC_SHA256, 1, new Dictionary<int, byte[]> { { 0, PAIR_KEY } });
                byte[] datagram = a.SenderFor(1).Seal(PUNCH, 0, PUNCH.Length);
                Assert.That(b.ReceiverFor(0).Open(datagram, datagram.Length), Is.EqualTo(PUNCH));
            }
            finally
            {
                DirectCipher.ChaCha20Poly1305Implementation = original;
            }
        }

        #endregion

        #region Authentication only (hmac-sha256)

        [Test]
        public void Hmac_KeyAndDatagram_MatchTheFixedValues()
        {
            Assert.That(Hex(DirectPacket.DeriveKey(PAIR_KEY, DirectPacket.CIPHER_HMAC_SHA256, 0, 1, EPOCH)), Is.EqualTo(KEY_HMAC_0_TO_1));
            Assert.That(KEY_HMAC_0_TO_1, Is.Not.EqualTo(KEY_0_TO_1), "the cipher's name is part of the key");
            byte[] datagram = DirectPacket.Seal(DirectPacket.CIPHER_HMAC_SHA256, H(KEY_HMAC_0_TO_1), 0, 1, EPOCH, 5, PUNCH, 0, PUNCH.Length);
            Assert.That(Hex(datagram), Is.EqualTo(PUNCH_DATAGRAM_HMAC));
            Assert.That(datagram.Skip(DirectPacket.HEADER_LENGTH).Take(PUNCH.Length).ToArray(), Is.EqualTo(PUNCH), "the packet travels readable");
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_HMAC_SHA256, H(KEY_HMAC_0_TO_1), 0, 1, datagram, datagram.Length), Is.EqualTo(PUNCH));
        }

        [Test]
        public void Hmac_EveryChangedByteIsRefused()
        {
            byte[] datagram = H(PUNCH_DATAGRAM_HMAC);
            for (int i = 0; i < datagram.Length; ++i)
            {
                byte[] changed = (byte[])datagram.Clone();
                changed[i] ^= 0x01;
                Assert.That(DirectPacket.Open(DirectPacket.CIPHER_HMAC_SHA256, H(KEY_HMAC_0_TO_1), 0, 1, changed, changed.Length), Is.Null, "byte " + i);
            }
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_HMAC_SHA256, H(KEY_0_TO_1), 0, 1, datagram, datagram.Length), Is.Null, "another key");
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_CHACHA20_POLY1305, H(KEY_HMAC_0_TO_1), 0, 1, datagram, datagram.Length), Is.Null, "the other cipher");
        }

        [Test]
        public void Hmac_RepeatsAndOldEpochsAreRefused_AsWithEncryption()
        {
            var receiver = new DirectReceiver(PAIR_KEY, DirectPacket.CIPHER_HMAC_SHA256, 0, 1);
            var first = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_HMAC_SHA256, 0, 1, DirectPacket.MakeEpoch(PREFIX, 1));
            byte[] d1 = first.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(d1, d1.Length), Is.EqualTo(PUNCH));
            Assert.That(receiver.Open(d1, d1.Length), Is.Null, "a repeat");
            var second = new DirectSender(PAIR_KEY, DirectPacket.CIPHER_HMAC_SHA256, 0, 1, DirectPacket.MakeEpoch(PREFIX, 2));
            byte[] d2 = second.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(d2, d2.Length), Is.EqualTo(PUNCH));
            byte[] old = first.Seal(PUNCH, 0, PUNCH.Length);
            Assert.That(receiver.Open(old, old.Length), Is.Null, "an earlier epoch");
        }

        [Test]
        public void Hmac_LongestDatagramIsTheMaximum()
        {
            int most = DirectPacket.MAX_DATAGRAM_LENGTH - DirectPacket.OVERHEAD;
            byte[] key = H(KEY_HMAC_0_TO_1);
            byte[] longest = DirectPacket.Seal(DirectPacket.CIPHER_HMAC_SHA256, key, 0, 1, EPOCH, 0, new byte[most], 0, most);
            Assert.That(longest.Length, Is.EqualTo(DirectPacket.MAX_DATAGRAM_LENGTH));
            Assert.That(DirectPacket.Open(DirectPacket.CIPHER_HMAC_SHA256, key, 0, 1, longest, longest.Length), Is.EqualTo(new byte[most]));
            Assert.That(() => DirectPacket.Seal(DirectPacket.CIPHER_HMAC_SHA256, key, 0, 1, EPOCH, 0, new byte[most + 1], 0, most + 1), Throws.ArgumentException);
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

        [TestCase(63UL)]
        [TestCase(64UL)]
        [TestCase(65UL)]
        [TestCase(128UL)]
        public void ReplayWindow_JumpsAhead(ulong in_jump)
        {
            // A shift by 64 or more must clear the history rather than wrap
            var window = new ReplayWindow();
            window.Accept(0);
            window.Accept(5);
            window.Accept(5 + in_jump);
            for (ulong s = 6; s < 5 + in_jump; ++s)
            {
                bool inWindow = (5 + in_jump) - s < ReplayWindow.SIZE;
                Assert.That(window.IsNew(s), Is.EqualTo(inWindow), "sequence " + s + " after a jump of " + in_jump);
            }
            Assert.That(window.IsNew(5), Is.False);
            Assert.That(window.IsNew(5 + in_jump), Is.False);
        }

        [Test]
        public void ReplayWindow_StartsAnywhere_EvenNearTheTop()
        {
            var window = new ReplayWindow();
            window.Accept(1000);
            Assert.That(window.IsNew(999), Is.True, "below the first one seen, within the window");
            window.Accept(999);
            Assert.That(window.IsNew(999), Is.False);

            var top = new ReplayWindow();
            top.Accept(ulong.MaxValue - 1);
            Assert.That(top.IsNew(ulong.MaxValue), Is.True);
            top.Accept(ulong.MaxValue);
            Assert.That(top.IsNew(ulong.MaxValue - 1), Is.False);
            Assert.That(top.IsNew(ulong.MaxValue - 63), Is.True);
        }

        #endregion

        #region Guards

        [Test]
        public void FinishingTwice_IsRefused()
        {
            var mac = new Poly1305(Sequence(0, 32), 0);
            mac.Finish();
            Assert.That(() => mac.Finish(), Throws.InvalidOperationException);
            Assert.That(() => mac.Update(new byte[1], 0, 1), Throws.InvalidOperationException);
            var sha = new Sha256();
            sha.Finish();
            Assert.That(() => sha.Finish(), Throws.InvalidOperationException);
            Assert.That(() => sha.Update(new byte[1], 0, 1), Throws.InvalidOperationException);
        }

        [Test]
        public void ChaCha20_RefusesToWrapTheBlockCounter()
        {
            byte[] data = new byte[128];
            Assert.That(() => ChaCha20.Xor(Sequence(0, 32), new byte[12], uint.MaxValue, data, 0, data, 0, 128), Throws.ArgumentException);
            Assert.That(() => ChaCha20.Xor(Sequence(0, 32), new byte[12], uint.MaxValue, data, 0, data, 0, 64), Throws.Nothing, "the last block is fine");
        }

        [Test]
        public void Aead_SealFromAnOffset()
        {
            byte[] key = Sequence(1, 32), nonce = Sequence(100, 12), plaintext = Sequence(50, 40);
            byte[] buffer = new byte[60];
            Buffer.BlockCopy(plaintext, 0, buffer, 13, 40);
            Assert.That(BrainCloud.Internal.ChaCha20Poly1305.Seal(key, nonce, null, buffer, 13, 40),
                Is.EqualTo(BrainCloud.Internal.ChaCha20Poly1305.Seal(key, nonce, null, plaintext, 0, 40)));
            Assert.That(() => BrainCloud.Internal.ChaCha20Poly1305.Seal(key, nonce, null, buffer, 30, 40), Throws.ArgumentException, "past the end");
        }

        #endregion
    }
}
