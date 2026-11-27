// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace BrainCloudTests
{
    /// <summary>
    /// How RelayService.ConnectFromRoomReady reads a ROOM_READY event, for each kind of server,
    /// without a server: the events here have the shapes brainCloud sends.
    /// </summary>
    [TestFixture]
    public class TestRelayRoomReady
    {
        // A brainCloud relay server, with secure relay connections enabled for the app
        private const string RELAY = @"{""service"":""lobby"",""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""23823:relayLobby:7"",""passcode"":""pc1"",
            ""connectData"":{""address"":""16.55.84.249"",""ports"":{""ws"":9001,""tcp"":9000,""udp"":9002},
                ""secureUrl"":""wss://node-1.hosted.example.com/9001""}}}";

        // The same server without secure relay connections enabled for the app
        private const string RELAY_NO_SECURE = @"{""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""23823:relayLobby:7"",""passcode"":""pc1"",
            ""connectData"":{""address"":""16.55.84.249"",""ports"":{""ws"":9001,""tcp"":9000,""udp"":9002}}}}";

        // Edgegap: one port per protocol, keyed by the protocol
        private const string EDGEGAP = @"{""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""l2"",""passcode"":""pc2"",
            ""connectData"":{""address"":""203.0.113.5"",""ports"":{""udp"":31001,""ws"":31002,""wss"":31003}}}}";

        private const string GAMELIFT = @"{""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""l3"",""passcode"":""pc3"",
            ""connectData"":{""address"":""198.51.100.7"",""ports"":{""gamelift"":7777}}}}";

        private const string I3D = @"{""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""l4"",""passcode"":""pc4"",
            ""connectData"":{""address"":""198.51.100.8"",""ports"":{""i3d"":8888}}}}";

        private const string DIRECT = @"{""operation"":""ROOM_READY"",""data"":{
            ""lobbyId"":""l5"",""passcode"":"""",
            ""connectData"":{""type"":""direct"",""peers"":[{""cxId"":""a"",""netId"":0},{""cxId"":""b"",""netId"":1}]}}}";

        private const string DIRECT_NO_LOBBY = @"{""operation"":""ROOM_READY"",""data"":{
            ""passcode"":"""",
            ""connectData"":{""type"":""direct"",""peers"":[{""cxId"":""a"",""netId"":0},{""cxId"":""b"",""netId"":1}]}}}";

        private static Dictionary<string, object> Parse(string in_json)
        {
            return (Dictionary<string, object>)JsonReader.Deserialize(in_json);
        }

        private static string Resolve(string in_json, RelayConnectionType in_preferred, bool in_ssl,
            out RelayConnectionType out_type, out RelayConnectOptions out_options)
        {
            return BrainCloudRelay.ResolveRoomReady(in_preferred, in_ssl, Parse(in_json), out out_type, out out_options);
        }

        [TestCase(RelayConnectionType.UDP, 9002)]
        [TestCase(RelayConnectionType.TCP, 9000)]
        [TestCase(RelayConnectionType.WEBSOCKET, 9001)]
        public void RelayServer_UsesThePortForThePreferredType(RelayConnectionType in_type, int in_port)
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY, in_type, false, out type, out options), Is.Null);
            Assert.That(type, Is.EqualTo(in_type));
            Assert.That(options.host, Is.EqualTo("16.55.84.249"));
            Assert.That(options.port, Is.EqualTo(in_port));
            Assert.That(options.ssl, Is.False);
            Assert.That(options.passcode, Is.EqualTo("pc1"));
            Assert.That(options.lobbyId, Is.EqualTo("23823:relayLobby:7"));
        }

        [Test]
        public void RelayServer_SecureWebSocket_UsesTheSecureUrlAsGiven()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY, RelayConnectionType.WEBSOCKET, true, out type, out options), Is.Null);
            Assert.That(type, Is.EqualTo(RelayConnectionType.WEBSOCKET));
            Assert.That(options.ssl, Is.True);
            Assert.That(options.host, Is.EqualTo("wss://node-1.hosted.example.com/9001"));
            Assert.That(options.port, Is.EqualTo(0));
        }

        [Test]
        public void SslIsIgnoredForUdpAndTcp()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY, RelayConnectionType.UDP, true, out type, out options), Is.Null);
            Assert.That(options.ssl, Is.False);
            Assert.That(options.port, Is.EqualTo(9002));
        }

        [Test]
        public void SecureWebSocketAskedFor_ButNoneOffered_FailsRatherThanFallBack()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY_NO_SECURE, RelayConnectionType.WEBSOCKET, true, out type, out options), Does.Contain("secure WebSocket"));
        }

        [Test]
        public void Edgegap_UsesThePortForThePreferredType()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(EDGEGAP, RelayConnectionType.UDP, false, out type, out options), Is.Null);
            Assert.That(options.host, Is.EqualTo("203.0.113.5"));
            Assert.That(options.port, Is.EqualTo(31001));
        }

        [Test]
        public void Edgegap_OffersNoSecureWebSocket_EvenWithAWssPort()
        {
            // Its wss port is on a bare IP address, which its certificate would not match
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(EDGEGAP, RelayConnectionType.WEBSOCKET, true, out type, out options), Does.Contain("secure WebSocket"));
        }

        [Test]
        public void Edgegap_PortNotOffered_SaysWhichAreOffered()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            string error = Resolve(EDGEGAP, RelayConnectionType.TCP, false, out type, out options);
            Assert.That(error, Does.Contain("no tcp port"));
            Assert.That(error, Does.Contain("udp"));
        }

        [TestCase(GAMELIFT, "198.51.100.7", 7777, "pc3")]
        [TestCase(I3D, "198.51.100.8", 8888, "pc4")]
        public void GameLiftAndI3d_UseTheirOnePortOverWebSocketWithoutSsl(string in_json, string in_host, int in_port, string in_passcode)
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(in_json, RelayConnectionType.UDP, false, out type, out options), Is.Null);
            Assert.That(type, Is.EqualTo(RelayConnectionType.WEBSOCKET));
            Assert.That(options.ssl, Is.False);
            Assert.That(options.host, Is.EqualTo(in_host));
            Assert.That(options.port, Is.EqualTo(in_port));
            Assert.That(options.passcode, Is.EqualTo(in_passcode));

            // They have no secure WebSocket, so asking for one fails rather than connect insecurely
            Assert.That(Resolve(in_json, RelayConnectionType.WEBSOCKET, true, out type, out options), Does.Contain("secure WebSocket"));
        }

        [Test]
        public void DirectRoom_ConnectsDirectly_WithTheLobbyIdAlone()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            // Whatever was preferred, and with or without SSL: there is no server to choose a port of
            foreach (RelayConnectionType preferred in new[] { RelayConnectionType.UDP, RelayConnectionType.WEBSOCKET, RelayConnectionType.TCP })
            {
                foreach (bool ssl in new[] { false, true })
                {
                    Assert.That(Resolve(DIRECT, preferred, ssl, out type, out options), Is.Null);
                    Assert.That(type, Is.EqualTo(RelayConnectionType.DIRECT));
                    Assert.That(options.lobbyId, Is.EqualTo("l5"));
                    Assert.That(options.host, Is.Null);
                }
            }
            Assert.That(Resolve(DIRECT_NO_LOBBY, RelayConnectionType.UDP, false, out type, out options), Does.Contain("no lobbyId"));
        }

        [Test]
        public void TheEventsDataObjectAloneIsAccepted()
        {
            var data = (Dictionary<string, object>)Parse(RELAY)["data"];
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(BrainCloudRelay.ResolveRoomReady(RelayConnectionType.UDP, false, data, out type, out options), Is.Null);
            Assert.That(options.port, Is.EqualTo(9002));
            Assert.That(options.passcode, Is.EqualTo("pc1"));
        }

        [Test]
        public void AMissingPasscodeOrLobbyId_IsRefused()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY.Replace(@"""passcode"":""pc1"",", ""), RelayConnectionType.UDP, false, out type, out options), Does.Contain("no passcode"));
            Assert.That(Resolve(RELAY.Replace(@"""lobbyId"":""23823:relayLobby:7"",", ""), RelayConnectionType.UDP, false, out type, out options), Does.Contain("no lobbyId"));
        }

        [Test]
        public void ConnectFromRoomReady_ReportsAnUnusableEventToTheFailureCallback()
        {
            var client = new BrainCloudClient();
            foreach (string json in new[] { "not json", "[1, 2]", null, DIRECT_NO_LOBBY })
            {
                int status = 0, reason = 0;
                string error = null;
                object cbObject = null;
                client.RelayService.ConnectFromRoomReady(RelayConnectionType.UDP, false, json, null,
                    (s, r, e, o) => { status = s; reason = r; error = e; cbObject = o; }, "cb");
                Assert.That(status, Is.EqualTo(StatusCodes.FORBIDDEN), "for " + json);
                Assert.That(reason, Is.EqualTo(ReasonCodes.RS_CLIENT_ERROR), "for " + json);
                Assert.That(error, Does.Contain("cannot connect from ROOM_READY"), "for " + json);
                Assert.That(cbObject, Is.EqualTo("cb"));
            }

            // No failure callback: nothing to report to, and nothing thrown
            Assert.That(() => client.RelayService.ConnectFromRoomReady(RelayConnectionType.UDP, false, "not json"), Throws.Nothing);
        }

        [Test]
        public void TryGetConnectOptionsFromRoomReady_GivesTheChoiceWithoutConnecting()
        {
            var relay = new BrainCloudClient().RelayService;
            RelayConnectionType type;
            RelayConnectOptions options;
            string error;

            Assert.That(relay.TryGetConnectOptionsFromRoomReady(RelayConnectionType.UDP, true, GAMELIFT, out type, out options, out error), Is.False);
            Assert.That(error, Does.Contain("secure WebSocket"));

            Assert.That(relay.TryGetConnectOptionsFromRoomReady(RelayConnectionType.TCP, false, RELAY, out type, out options, out error), Is.True);
            Assert.That(error, Is.Null);
            Assert.That(type, Is.EqualTo(RelayConnectionType.TCP));
            Assert.That(options.port, Is.EqualTo(9000));
            Assert.That(relay.IsConnected(), Is.False, "it only works out the options");

            Assert.That(relay.TryGetConnectOptionsFromRoomReady(RelayConnectionType.TCP, false, "not json", out type, out options, out error), Is.False);
            Assert.That(error, Is.Not.Null);
        }

        [Test]
        public void ConnectFromRoomReady_HandsAUsableEventToConnect()
        {
            // Connect refuses a client that is not authenticated, which shows the event got that far
            var client = new BrainCloudClient();
            int reason = 0;
            client.RelayService.ConnectFromRoomReady(RelayConnectionType.UDP, false, RELAY, null,
                (s, r, e, o) => { reason = r; });
            Assert.That(reason, Is.EqualTo(ReasonCodes.RS_NO_API_SESSION_ERROR));
        }

        [Test]
        public void OtherEventsAndBadInput_AreRefused()
        {
            RelayConnectionType type;
            RelayConnectOptions options;
            Assert.That(Resolve(RELAY.Replace("ROOM_READY", "ROOM_ASSIGNED"), RelayConnectionType.UDP, false, out type, out options), Does.Contain("not ROOM_ASSIGNED"));
            Assert.That(BrainCloudRelay.ResolveRoomReady(RelayConnectionType.UDP, false, null, out type, out options), Is.Not.Null);
            Assert.That(Resolve(@"{""operation"":""ROOM_READY"",""data"":{}}", RelayConnectionType.UDP, false, out type, out options), Does.Contain("no connectData"));
            Assert.That(Resolve(RELAY, RelayConnectionType.INVALID, false, out type, out options), Does.Contain("WEBSOCKET, TCP or UDP"));
        }
    }
}
