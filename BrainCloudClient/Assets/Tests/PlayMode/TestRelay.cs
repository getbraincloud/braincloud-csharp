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
    public class TestRelay : TestFixtureBase
    {
        private bool _isRunning;
        
        private RelayConnectOptions connectOptions;
        private RelayConnectionType connectionType = RelayConnectionType.UDP; // Change this to try different connection type
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestRelayWebSocket()
        {
            yield return _testingContainer.StartCoroutine(FullFlow(RelayConnectionType.WEBSOCKET));
            
            Debug.Log($"Successful Counts: {_successCount}");
            Assert.True(_successCount == 3);
        }
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestRelayUDP()
        {
            yield return _testingContainer.StartCoroutine(FullFlow(RelayConnectionType.UDP));
            
            Debug.Log($"Successful Counts: {_successCount}");
            Assert.True(_successCount == 3);
        }
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestRelayTCP()
        {
            yield return _testingContainer.StartCoroutine(FullFlow(RelayConnectionType.TCP));
            
            Debug.Log($"Successful Counts: {_successCount}");
            Assert.True(_successCount == 3);
        }

        private IEnumerator FullFlow(RelayConnectionType in_connectionType)
        {
            connectionType = in_connectionType;
            
            _testingContainer.RunAuth();
            while (_testingContainer.m_done)
                yield return new WaitForFixedUpdate();
            _testingContainer.bcWrapper.Client.EnableLogging(true);
            _testingContainer.bcWrapper.RTTService.RegisterRTTLobbyCallback(OnLobbyEvent);
            _testingContainer.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, OnRTTEnabled, OnFailed);
            _testingContainer.StartRun();
            _isRunning = true;
            var timeStart = DateTime.Now;
            while (_isRunning)
            {
                _testingContainer.bcWrapper.Update();
                if((DateTime.Now - timeStart).TotalSeconds > 5.0 * 60.0)
                {
                    Debug.Log("Times Up");
                    break;
                }
                yield return new WaitForFixedUpdate();
            }
        }
        
        void OnFailed(int status, int reasonCode, string jsonError, object cbObject)
        {
            if (jsonError == "{\"status\":403,\"reason_code\":90300,\"status_message\":\"Invalid NetId: 40\",\"severity\":\"ERROR\"}")
            {
                // This one was on purpose
                _successCount++;
                _isRunning = false;
                return;
            }
            _isRunning = false;
            _successCount = 0;
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
            _testingContainer.bcWrapper.LobbyService.FindOrCreateLobby
                (
                    "READY_START_V2", 
                    0,  //rating
                    1, //max steps
                    algo, 
                    new Dictionary<string, object>(), //filter json
                    0,  //timeout seconds
                    true,   //isReady
                    new Dictionary<string, object>(), //Extra Json
                    "all",  //team code
                    new Dictionary<string, object>(),   //settings 
                    null,   //other user cxIds
                    null,   //success
                    OnFailed        
                );
        }
        
        void OnLobbyEvent(string json)
        {
            var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = response["data"] as Dictionary<string, object>;

            switch (response["operation"] as string)
            {
                case "DISBANDED":
                    var reason = data["reason"] as Dictionary<string, object>;
                    var reasonCode = (int)reason["code"];
                    
                    if (reasonCode == ReasonCodes.RTT_ROOM_READY)
                    {
                        ConnectToServer();
                    }
                    else
                    {
                        OnFailed(0, 0, "DISBANDED != RTT_ROOM_READY", null);
                    }
                    break;
                // ROOM_READY contains information on how to connect to the 
                // relay server.
                case "ROOM_READY":
                    _testingContainer.Server = new Server(data);
                    break;
            }
        }

        void ConnectToServer()
        {
            _testingContainer.bcWrapper.RelayService.RegisterSystemCallback(systemCallback);
            _testingContainer.bcWrapper.RelayService.RegisterRelayCallback(relayCallback);
            var port = 0;
            switch (connectionType)
            {
                case RelayConnectionType.TCP:
                    port = _testingContainer.Server.TcpPort;
                    break;
                case RelayConnectionType.UDP:
                    port = _testingContainer.Server.UdpPort;
                    break;
                case RelayConnectionType.WEBSOCKET:
                    port = _testingContainer.Server.WsPort;
                    break;
            }
            connectOptions = new RelayConnectOptions
            (
                false,
                _testingContainer.Server.Host,
                port,
                _testingContainer.Server.Passcode,
                _testingContainer.Server.LobbyId
            );
            
            _testingContainer.bcWrapper.RelayService.Connect(connectionType, connectOptions, onRelayConnected, OnFailed);
        }

        void systemCallback(string json)
        {
            Dictionary<string, object> parsedDict = (Dictionary<string, object>)JsonReader.Deserialize(json);
            if (parsedDict["op"] as string == "CONNECT")
            {
                _successCount++;
                if (_successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
        }

        void relayCallback(short netId, byte[] data)
        {
            string message = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            if (message == "Hello World!")
            {
                _successCount++;
                if (_successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
        }
        
        void sendToWrongNetId()
        {
            short myNetId = BrainCloudRelay.MAX_PLAYERS; // Wrong net id, should be < 40 or ALL_PLAYERS (0x000000FFFFFFFFFF)
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("To Bad Id");
            _testingContainer.bcWrapper.RelayService.Send(bytes, (ulong)myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);
        }
        
        void onRelayConnected(string jsonResponse, object cbObject)
        {
            var profileId = _testingContainer.bcWrapper.Client.AuthenticationService.ProfileId;
            short myNetId = _testingContainer.bcWrapper.RelayService.GetNetIdForProfileId(profileId);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World!");
            _testingContainer.bcWrapper.RelayService.Send(bytes, (ulong)myNetId);
        }
    }
}
