using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.TestTools;
using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
namespace Tests.PlayMode
{
    public class TestS2S : TestFixtureBase
    {
        string _address;
        int _port;
        private int successCount = 0;
        private RelayConnectOptions connectOptions = new RelayConnectOptions();
        private RelayConnectionType connectionType = RelayConnectionType.UDP; // Change this to try different connection type

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestS2SWithEnumeratorPasses()
        {
            _testingContainer.RunAuth();
            while (_testingContainer.m_done)
                yield return new WaitForFixedUpdate();
            _testingContainer._bc.Client.EnableLogging(true);
            _testingContainer._bc.RTTService.RegisterRTTLobbyCallback(OnLobbyEvent);
            _testingContainer._bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, OnRTTEnabled, OnFailed);
            _testingContainer.StartRun();

            var timeStart = DateTime.Now;
            Debug.Log("Final Run");
            while (successCount < 4)
            {
                _testingContainer._bc.Update();
                if((DateTime.Now - timeStart).TotalSeconds > 5.0 * 60.0)
                {
                    Debug.Log("Times Up");
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
            
            Assert.True(successCount == 3);
        }
        
        void OnAuthenticated(string jsonResponse, object cbObject)
        {
            _testingContainer._bc.RTTService.RegisterRTTLobbyCallback(OnLobbyEvent);
            _testingContainer._bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, 
                OnRTTEnabled, OnFailed);
        }
        
        void OnFailed(int status, int reasonCode, string jsonError, object cbObject)
        {
            Debug.Log($"ONFAILED: Status: {status} || Reason Code: {reasonCode} || Json Error: {jsonError} || Object: {cbObject}");
        }
        
        void OnRTTEnabled(string jsonResponse, object cbObject)
        {
            var algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;
            _testingContainer._bc.LobbyService.FindOrCreateLobby(
                "READY_START_V2", 0, 1, algo, 
                new Dictionary<string, object>(), 0, true, 
                new Dictionary<string, object>(), "all", 
                new Dictionary<string, object>(), null, null, OnFailed);
        }
        
        void OnLobbyEvent(string json)
        {
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;

            switch (response["operation"] as string)
            {
                case "DISBANDED":
                    var reason = data["reason"] 
                        as Dictionary<string, object>;
                    var reasonCode = (int)reason["code"];
                    Debug.Log("Disbanded" + reasonCode);
                    if (reasonCode == ReasonCodes.RTT_ROOM_READY)
                    {
                        //ConnectToServer();
                    }
                    else
                        OnFailed(0, 0, "DISBANDED != RTT_ROOM_READY", null);
                    break;

                // ROOM_READY contains information on how to connect to the 
                // relay server.
                case "ROOM_READY":
                    //_message += "Room Ready\n";
                    Debug.Log("Room Ready");
                    var connectData = data["connectData"]
                        as Dictionary<string, object>;
                    var ports = connectData["ports"] 
                        as Dictionary<string, object>;
                    _testingContainer.Server = new Server(data);
                    _address = (string)connectData["address"];
                    _port = (int)ports["tcp"];
                    ConnectToServer();
                    break;
            }
        }

        void ConnectToServer()
        {
            _testingContainer._bc.RelayService.RegisterSystemCallback(systemCallback);
            _testingContainer._bc.RelayService.RegisterRelayCallback(relayCallback);
            connectOptions = new RelayConnectOptions
            (
                false,
                _testingContainer.Server.Host,
                _testingContainer.Server.TcpPort,
                _testingContainer.Server.Passcode,
                _testingContainer.Server.LobbyId
            );
            
            _testingContainer._bc.RelayService.Connect(connectionType, connectOptions, onRelayConnected, OnFailed);
        }

        void systemCallback(string json)
        {
            Debug.Log("systemCallback: " + json);

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
            Debug.Log("relayCallback: " + message);
            
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
            _testingContainer._bc.RelayService.Send(bytes, (ulong)myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);
        }
        
        void onRelayConnected(string jsonResponse, object cbObject)
        {
            Debug.Log("On Relay Connected");

            short myNetId = _testingContainer._bc.RelayService.GetNetIdForProfileId(_bc.Client.AuthenticationService.ProfileId);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World!");
            _testingContainer._bc.RelayService.Send(bytes, (ulong)myNetId);
        }
    }
}
