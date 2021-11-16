using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestRTT : TestFixtureBase
    {
    
        [UnityTest]
        public IEnumerator TestEnableDisableRTTWithWS()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.RTTService.DisableRTT();  //This shouldn't callback error
            _tc.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestRTTHeartBeat()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            var timeBefore = DateTime.Now;
            while ((DateTime.Now - timeBefore).TotalSeconds < 90.0)
            {
                yield return new WaitForFixedUpdate();
            }
            Assert.True(_tc.bcWrapper.RTTService.IsRTTEnabled());
        }

        [UnityTest]
        public IEnumerator TestRTTChatCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            //Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Get Channel Id
            _tc.bcWrapper.ChatService.GetChannelId("gl", "valid", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            string channelId = (_tc.m_response["data"] as Dictionary<string, object>)["channelId"] as string;

            //Connect to channel
            _tc.bcWrapper.ChatService.ChannelConnect(channelId, 50, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Register for RTT chat
            bool receivedChat = false;
            _tc.bcWrapper.RTTService.RegisterRTTChatCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "chat" &&
                    response["operation"] as string == "INCOMING")
                {
                    receivedChat = true;
                }
            });
            
            //Send a chat message
            _tc.bcWrapper.ChatService.PostChatMessageSimple(channelId, "Unit test message", true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Now check if we get the chat message
            var timeBefore = DateTime.Now;
            while (!receivedChat && (DateTime.Now - timeBefore).TotalSeconds < 30)
            {
                //Client Update on its own within the wrapper
                yield return new WaitForFixedUpdate();
            }
            Assert.True(receivedChat);

            //Now deregister and make sure we dont receive it
            _tc.bcWrapper.RTTService.DeregisterRTTChatCallback();
            
            //Send a chat message again
            receivedChat = false;
            _tc.bcWrapper.ChatService.PostChatMessageSimple(channelId, "Unit test message 2", true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            // Wait 10sec, and make sure we don't get the event this time
            timeBefore = DateTime.Now;
            while (!receivedChat && (DateTime.Now - timeBefore).TotalSeconds < 10)
            {
                //Client Update on its own within the wrapper
                yield return new WaitForFixedUpdate();
            }
            Assert.False(receivedChat);
        }

        [UnityTest]
        public IEnumerator TestRTTLobbyCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            // Register for RTT lobby
            bool receivedLobby = false;
            _tc.bcWrapper.RTTService.RegisterRTTLobbyCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "lobby")
                {
                    receivedLobby = true;
                }
            });

            // Create a lobby
            _tc.bcWrapper.LobbyService.CreateLobby
                (
                    "MATCH_UNRANKED", 
                    0, 
                    true, 
                    new Dictionary<string, object>(), 
                    "all", 
                    new Dictionary<string, object>(), 
                    null, 
                    _tc.ApiSuccess, 
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());

            // Now check if we get the lobby message
            // Wait 300sec, creating lobby can take time
            var timeBefore = DateTime.Now;
            while (!receivedLobby && (DateTime.Now - timeBefore).TotalSeconds < 300.0)
            {
                //Client Update on its own within the wrapper
                yield return new WaitForFixedUpdate();
            }
            Assert.True(receivedLobby);
        }

        [UnityTest]
        public IEnumerator TestRTTEventCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            // Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            // Register for RTT lobby
            bool receivedEvent = false;
            _tc.bcWrapper.RTTService.RegisterRTTEventCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "event")
                {
                    receivedEvent = true;
                }
            });

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
            yield return _tc.StartCoroutine(_tc.Run());

            // Now check if we get the event message
            var timeBefore = DateTime.Now;
            while (!receivedEvent && (DateTime.Now - timeBefore).TotalSeconds < 30.0)
            {
                //Client Update on its own within the wrapper
                yield return new WaitForFixedUpdate();
            }
            Assert.True(receivedEvent);
        }
    }
}