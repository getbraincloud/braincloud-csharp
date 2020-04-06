using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestRelay : TestFixtureBase
    {
        private bool isRunning = true;
        private int successCount = 0;
        private RelayConnectOptions connectOptions = new RelayConnectOptions();
        private RelayConnectionType connectionType = RelayConnectionType.UDP; // Change this to try different connection type

        void FullFlow(RelayConnectionType in_connectionType)
        {
            connectionType = in_connectionType;
            _bc.Client.EnableLogging(true);

            // Main event loop
            var timeStart = DateTime.Now;
            while (isRunning)
            {
                _bc.Update();
                Thread.Sleep(10);

                // We timeout after 2minutes
                if ((DateTime.Now - timeStart).TotalSeconds > 120.0) break;
            }

            Assert.True(successCount == 2);
        }

        void onFailed(int status, int reasonCode, string jsonError, object cbObject)
        {
            Console.WriteLine("Error: " + jsonError);
            isRunning = false;
            successCount = 0;
        }

        void onAuthenticated(string jsonResponse, object cbObject)
        {
            _bc.RTTService.RegisterRTTLobbyCallback(onLobbyEvent);
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, onRTTEnabled, onFailed);
        }

        void onRTTEnabled(string jsonResponse, object cbObject)
        {
            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;
            _bc.LobbyService.FindOrCreateLobby("READY_START", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, null, onFailed);
        }

        void onLobbyEvent(string json)
        {
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;

            switch (response["operation"] as string)
            {
                case "DISBANDED":
                    var reason = data["reason"] as Dictionary<string, object>;
                    var reasonCode = (int)reason["code"];
                    if (reasonCode == ReasonCodes.RTT_ROOM_READY)
                        connectToRelay();
                    else
                        onFailed(0, 0, "DISBANDED != RTT_ROOM_READY", null);
                    break;

                // ROOM_READY contains information on how to connect to the relay server.
                case "ROOM_READY":
                    var connectData = data["connectData"] as Dictionary<string, object>;
                    var ports = connectData["ports"] as Dictionary<string, object>;

                    connectOptions.ssl = false;
                    connectOptions.host = connectData["address"] as string;

                    if (connectionType == RelayConnectionType.WEBSOCKET)
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
            _bc.RelayService.Connect(connectionType, connectOptions, onRelayConnected, onFailed);
        }

        void onRelayConnected(string jsonResponse, object cbObject)
        {
            Console.WriteLine("On Relay Connected");

            short myNetId = _bc.RelayService.GetNetIdForProfileId(_bc.Client.AuthenticationService.ProfileId);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World!");
            _bc.RelayService.Send(bytes, myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);
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
                    isRunning = false;
                }
            }
        }

        void relayCallback(byte[] data)
        {
            string message = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            Console.WriteLine("relayCallback: " + message);
            
            if (message == "Hello World!")
            {
                successCount++;
                if (successCount >= 2)
                {
                    isRunning = false;
                }
            }
        }


        // [Test]
        // public void TestFullFlowWS()
        // {
        //     FullFlow(RelayConnectionType.WEBSOCKET);
        // }

        // [Test]
        // public void TestFullFlowTCP()
        // {
        //     FullFlow(RelayConnectionType.TCP);
        // }

        // [Test]
        // public void TestFullFlowUDP()
        // {
        //     FullFlow(RelayConnectionType.UDP);
        // }
    }
}
