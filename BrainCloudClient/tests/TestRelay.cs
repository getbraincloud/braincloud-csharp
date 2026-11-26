// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestRelay : TestFixtureBase
    {
        private bool isRunning = true;
        private int successCount = 0;
        private RelayConnectOptions connectOptions = new RelayConnectOptions();
        private RelayConnectionType connectionType = RelayConnectionType.UDP; // Change this to try different connection type
        private bool secure = false;
        private bool noSecureUrl = false;
        private bool useHelper = false;
        private Dictionary<string, object> roomReadyData = null;
        private string roomReadyJson = null;

        void FullFlow(RelayConnectionType in_connectionType, bool in_secure = false, bool in_useHelper = false)
        {
            successCount = 0;
            isRunning = true;
            connectionType = in_connectionType;
            secure = in_secure;
            noSecureUrl = false;
            useHelper = in_useHelper;
            roomReadyData = null;
            roomReadyJson = null;

            _bc.Client.EnableLogging(true);
            _bc.RTTService.RegisterRTTLobbyCallback(onLobbyEvent);
            _bc.RTTService.EnableRTT(onRTTEnabled, onFailed);

            // Main event loop
            var timeStart = DateTime.Now;
            while (isRunning)
            {
                _bc.Update();
                Thread.Sleep(10);

                // We timeout after 5 minutes (give it time if the system is slow and no server is up)
                if ((DateTime.Now - timeStart).TotalSeconds > 5.0 * 60.0) break;
            }

            // secureUrl is only sent when the app has relayServerSecureEnabled
            if (noSecureUrl) Assert.Ignore("connectData has no secureUrl");
            Assert.That(successCount == 3); // Test result
        }

        void onFailed(int status, int reasonCode, string jsonError, object cbObject)
        {
            Console.WriteLine("Error: " + jsonError);

            if (jsonError == "{\"status\":403,\"reason_code\":90300,\"status_message\":\"Invalid NetId: 40\",\"severity\":\"ERROR\"}")
            {
                // This one was on purpose
                successCount++;
                isRunning = false;
                return;
            }

            isRunning = false;
            successCount = 0;
        }

        void onRTTEnabled(string jsonResponse, object cbObject)
        {
            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;
            _bc.LobbyService.FindOrCreateLobby("READY_START_V2", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, null, onFailed);
        }

        void onLobbyEvent(string json)
        {
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;

            switch (response["operation"] as string)
            {
                case "DISBANDED":
                    if (noSecureUrl) break;
                    var reason = data["reason"] as Dictionary<string, object>;
                    var reasonCode = (int)reason["code"];
                    if (reasonCode == ReasonCodes.RTT_ROOM_READY)
                        connectToRelay();
                    else
                        onFailed(0, 0, "DISBANDED != RTT_ROOM_READY", null);
                    break;

                // ROOM_READY contains information on how to connect to the relay server.
                case "ROOM_READY":
                    roomReadyData = data;
                    roomReadyJson = json;
                    var connectData = data["connectData"] as Dictionary<string, object>;
                    var ports = connectData["ports"] as Dictionary<string, object>;

                    connectOptions.ssl = false;
                    connectOptions.host = connectData["address"] as string;

                    if (secure)
                    {
                        // wss uses the server-provided URL as-is; no port
                        if (!connectData.ContainsKey("secureUrl"))
                        {
                            noSecureUrl = true;
                            isRunning = false;
                            break;
                        }
                        connectOptions.ssl = true;
                        connectOptions.host = connectData["secureUrl"] as string;
                        connectOptions.port = 0;
                    }
                    else if (connectionType == RelayConnectionType.WEBSOCKET)
                        connectOptions.port = (int)ports["ws"];
                    else if (connectionType == RelayConnectionType.TCP)
                        connectOptions.port = (int)ports["tcp"];
                    else if (connectionType == RelayConnectionType.UDP)
                        connectOptions.port = (int)ports["udp"];

                    connectOptions.passcode = data["passcode"] as string;
                    connectOptions.lobbyId = data["lobbyId"] as string;
                    break;
            }
        }

        void connectToRelay()
        {
            _bc.RelayService.RegisterSystemCallback(systemCallback);
            _bc.RelayService.RegisterRelayCallback(relayCallback);
            if (useHelper)
            {
                // One run passes the event's JSON as the lobby callback received it, the other the parsed data
                if (secure)
                    _bc.RelayService.ConnectFromRoomReady(connectionType, secure, roomReadyData, onRelayConnected, onFailed);
                else
                    _bc.RelayService.ConnectFromRoomReady(connectionType, secure, roomReadyJson, onRelayConnected, onFailed);
            }
            else
            {
                _bc.RelayService.Connect(connectionType, connectOptions, onRelayConnected, onFailed);
            }
        }

        void onRelayConnected(string jsonResponse, object cbObject)
        {
            Console.WriteLine("On Relay Connected");

            short myNetId = _bc.RelayService.GetNetIdForProfileId(_bc.Client.AuthenticationService.ProfileId);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World!");
            _bc.RelayService.Send(bytes, (ulong)myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);
        }

        void systemCallback(string json)
        {
            Console.WriteLine("systemCallback: " + json);

            Dictionary<string, object> parsedDict = (Dictionary<string, object>)JsonReader.Deserialize(json);
            if (parsedDict["op"] as string == "CONNECT")
            {
                successCount++;
                if (successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
        }

        void relayCallback(short netId, byte[] data)
        {
            string message = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            Console.WriteLine("relayCallback: " + message);
            
            if (message == "Hello World!")
            {
                successCount++;
                if (successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
        }

        void sendToWrongNetId()
        {
            short myNetId = BrainCloudRelay.MAX_PLAYERS; // Wrong net id, should be < 40 or ALL_PLAYERS (0x000000FFFFFFFFFF)
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("To Bad Id");
            _bc.RelayService.Send(bytes, (ulong)myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);
        }

        [Test]
        public void TestFullFlowWS()
        {
            FullFlow(RelayConnectionType.WEBSOCKET);
        }

        [Test]
        public void TestFullFlowWSS()
        {
            if (Environment.GetEnvironmentVariable("BC_SKIP_WSS") != null) Assert.Ignore("BC_SKIP_WSS set");
            FullFlow(RelayConnectionType.WEBSOCKET, true);
        }

        [Test]
        public void TestFullFlowTCP()
        {
            FullFlow(RelayConnectionType.TCP);
        }

        [Test]
        public void TestFullFlowUDP()
        {
            FullFlow(RelayConnectionType.UDP);
        }

        [Test]
        public void TestFullFlowUDPFromRoomReady()
        {
            FullFlow(RelayConnectionType.UDP, false, true);
        }

        [Test]
        public void TestFullFlowWSSFromRoomReady()
        {
            if (Environment.GetEnvironmentVariable("BC_SKIP_WSS") != null) Assert.Ignore("BC_SKIP_WSS set");
            FullFlow(RelayConnectionType.WEBSOCKET, true, true);
        }
    }
}
