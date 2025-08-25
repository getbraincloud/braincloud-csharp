using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;
using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
namespace Tests.PlayMode
{
    public class TestRelay : TestFixtureBase
    {

        private RelayConnectOptions connectOptions;
        private RelayConnectionType connectionType = RelayConnectionType.INVALID; // Change this to try different connection type
        private bool bSendWrongNetId;
        private bool bSystemCallbackConnect;
        private bool bRelayMessageReceived;
        private bool bIncludeEndMatch;
        
        [UnityTest]
        public IEnumerator TestRelayWebSocket()
        {
            Debug.Log("Now running...TestRelayWebSocket ");
            yield return _tc.StartCoroutine(FullFlow(RelayConnectionType.WEBSOCKET));
            
            LogResults($"Websocket Failed: SendWrongNetId: {bSendWrongNetId}, System Connect Callback: {bSystemCallbackConnect}, Relay Message Received: {bRelayMessageReceived}", _tc.successCount == 3);
        }
        
        [UnityTest]
        public IEnumerator TestRelayUDP()
        {
            Debug.Log("Now running...TestRelayUDP ");
            yield return _tc.StartCoroutine(FullFlow(RelayConnectionType.UDP));
            
            LogResults($"UDP Failed: SendWrongNetId: {bSendWrongNetId}, System Connect Callback: {bSystemCallbackConnect}, Relay Message Received: {bRelayMessageReceived}", _tc.successCount == 3);
        }
        
        [UnityTest]
        public IEnumerator TestRelayTCP()
        {
            Debug.Log("Now running...TestRelayTCP ");
            yield return _tc.StartCoroutine(FullFlow(RelayConnectionType.TCP));
            
            LogResults($"TCP Failed: SendWrongNetId: {bSendWrongNetId}, System Connect Callback: {bSystemCallbackConnect}, Relay Message Received: {bRelayMessageReceived}", _tc.successCount == 3);
        }

        [UnityTest]
        public IEnumerator TestRelayWSEndMatch()
        {
            Debug.Log("Now running...TestRelayWSEndMatch ");
            bIncludeEndMatch = true;
            yield return _tc.StartCoroutine(FullFlow(RelayConnectionType.WEBSOCKET));

            LogResults("Something went wrong..", _tc.successCount == 4);
        }

        private IEnumerator FullFlow(RelayConnectionType in_connectionType)
        {
            _tc.m_timeToWaitSecs = 1000;
            connectionType = in_connectionType;

            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.EnableLogging(true);
            _tc.bcWrapper.RTTService.RegisterRTTLobbyCallback(OnLobbyEvent);
            _tc.bcWrapper.RTTService.EnableRTT(OnRTTEnabledReady, OnFailed);
            
            yield return _tc.StartCoroutine(_tc.Run());
        }
        
        void OnFailed(int status, int reasonCode, string jsonError, object cbObject)
        {
            if (jsonError.Contains("Invalid NetId: 40"))
            {
                // This one was on purpose
                if (_tc.successCount <= 2)
                {
                    _tc.successCount++;
                    bSendWrongNetId = true;
                    _isRunning = false;
                    if (_tc.successCount == 3)
                    {
                        _tc.m_done = true;    
                    }
                    else
                    {
                        Debug.Log($"Didnt meet Success Count, instead of 3 it was {_tc.successCount}");
                    }
                    return;   
                }
            }
            _isRunning = false;
            _tc.successCount = 0;
            Debug.Log($"ONFAILED: Status: {status} || Reason Code: {reasonCode} || Json Error: {jsonError} || Object: {cbObject}");
        }
        
        void OnRTTEnabledReady(string jsonResponse, object cbObject)
        {
            var algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;
            _tc.bcWrapper.LobbyService.FindOrCreateLobby
                (
                    "READY_START_V2", 
                    0,  //rating
                    1, //max steps
                    algo, 
                    new Dictionary<string, object>(), //filter json
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
                    _tc.Server = new Server(data);
                    break;
            }
        }

        void ConnectToServer()
        {
            _tc.bcWrapper.RelayService.RegisterSystemCallback(systemCallback);
            _tc.bcWrapper.RelayService.RegisterRelayCallback(relayCallback);
            var port = 0;
            switch (connectionType)
            {
                case RelayConnectionType.TCP:
                    port = _tc.Server.TcpPort;
                    break;
                case RelayConnectionType.UDP:
                    port = _tc.Server.UdpPort;
                    break;
                case RelayConnectionType.WEBSOCKET:
                    port = _tc.Server.WsPort;
                    break;
            }
            connectOptions = new RelayConnectOptions
            (
                false,
                _tc.Server.Host,
                port,
                _tc.Server.Passcode,
                _tc.Server.LobbyId
            );
            
            _tc.bcWrapper.RelayService.Connect(connectionType, connectOptions, onRelayConnected, OnFailed);
        }

        void systemCallback(string json)
        {
            Dictionary<string, object> parsedDict = (Dictionary<string, object>)JsonReader.Deserialize(json);
            if (parsedDict["op"] as string == "CONNECT")
            {
                _tc.successCount++;
                bSystemCallbackConnect = true;
                if (_tc.successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
            else if (parsedDict["op"] as string == "END_MATCH")
            {
                _tc.successCount++;

                // Send an event
                var profileId = _tc.bcWrapper.Client.AuthenticationService.ProfileId;
                _tc.bcWrapper.EventService.SendEvent
                (
                    profileId, 
                    "test", 
                    "{\"testData\":42}", 
                    _tc.ApiSuccess, 
                    _tc.ApiError
                );            
            }
        }

        void relayCallback(short netId, byte[] data)
        {
            string message = System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
            if (message == "Hello World!")
            {
                _tc.successCount++;
                bRelayMessageReceived = true;
                if (_tc.successCount >= 2)
                {
                    sendToWrongNetId();
                }
            }
        }
        
        void sendToWrongNetId()
        {
            if (bIncludeEndMatch)
            {
                Debug.Log("Sending End Match...");
                Dictionary<string, object> json = new Dictionary<string, object>();
                json["cxId"] = _tc.bcWrapper.Client.RTTConnectionID;
                json["op"] = "END_MATCH";
                _tc.bcWrapper.RelayService.EndMatch(json);
            }
            else
            {
                short myNetId = BrainCloudRelay.MAX_PLAYERS; // Wrong net id, should be < 40 or ALL_PLAYERS (0x000000FFFFFFFFFF)
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes("To Bad Id");
                _tc.bcWrapper.RelayService.Send(bytes, (ulong)myNetId, true, true, BrainCloudRelay.CHANNEL_HIGH_PRIORITY_1);    
            }
        }
        
        void onRelayConnected(string jsonResponse, object cbObject)
        {
            var profileId = _tc.bcWrapper.Client.AuthenticationService.ProfileId;
            short myNetId = _tc.bcWrapper.RelayService.GetNetIdForProfileId(profileId);
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes("Hello World!");
            _tc.bcWrapper.RelayService.Send(bytes, (ulong)myNetId);
        }
    }
}
