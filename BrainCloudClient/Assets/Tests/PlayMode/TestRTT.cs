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
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to enable or disable RTT with WS", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestRTTHeartBeat()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            var timeBefore = DateTime.Now;
            while ((DateTime.Now - timeBefore).TotalSeconds < 90.0)
            {
                yield return new WaitForFixedUpdate();
            }
            
            LogResults("Failed to get RTT Heartbeat", _tc.bcWrapper.RTTService.IsRTTEnabled());
        }

        [UnityTest]
        public IEnumerator TestRTTChatCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            //Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            string channelId = "20001:gl:valid";

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

            yield return new WaitForSeconds(20);
            //Assert.True(receivedChat);
            LogResults("Didn't receive chat message", receivedChat);
            //Now deregister and make sure we dont receive it
            _tc.bcWrapper.RTTService.DeregisterRTTChatCallback();
            
            //Send a chat message again
            receivedChat = false;
            _tc.bcWrapper.ChatService.PostChatMessageSimple(channelId, "Unit test message 2", true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            yield return new WaitForSeconds(10);
            
            //Assert.False(receivedChat);
            LogResults("Did receive chat message, expected to not receive message", !receivedChat);
        }

        [UnityTest]
        public IEnumerator TestFailingRTTCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            int successfulCallback = 0;

            //Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            string channelId = "20001:gl:valid";

            //Connect to channel
            _tc.bcWrapper.ChatService.ChannelConnect(channelId, 50, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            bool passedThrough;
            //Register for RTT chat
            _tc.bcWrapper.RTTService.RegisterRTTChatCallback((string json) =>
            {
                //throw exception to cause callback to fail
                passedThrough = false;
                throw new Exception("This callback failed (Not really)");
            });

            _tc.bcWrapper.ChatService.PostChatMessageSimple(channelId, "Unit test message", true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            yield return new WaitForSeconds(20);

            passedThrough = true;

            LogResults("Got through entire callback", passedThrough);
        }
        
        [UnityTest]
        public IEnumerator TestRTTMessagingCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            int successfulCallback = 0;
            //Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.RTTService.RegisterRTTMessagingCallback((response =>
            {
                Debug.LogWarning($"Response: {response}");
                successfulCallback++;
            }));
            //Grabbed a random profileId from portal
            string[] profileId = {_tc.bcWrapper.Client.ProfileId, "3a1a1f9f-ce3d-4f65-95b7-8b832ab32d35"};
            //"{\"Title\":\"}";//
            string messageContent = "{\"subject\":\"Title of Message\",\"text\":\"Hello World !\"}";
            _tc.bcWrapper.MessagingService.SendMessage
                (
                    profileId,
                    messageContent,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );

            yield return _tc.StartCoroutine(_tc.Run());
            yield return new WaitForSeconds(5);
            LogResults("Function Send Message Failed", successfulCallback > 0);
        }

        [UnityTest]
        public IEnumerator TestRTTLobbyCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
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
            //Assert.True(receivedLobby);
            LogResults("Didn't receive lobby message", receivedLobby);
        }

        [UnityTest]
        public IEnumerator TestRTTEventCallback()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            // Enable RTT
            _tc.bcWrapper.RTTService.EnableRTT(_tc.ApiSuccess, _tc.ApiError);
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
            //Assert.True(receivedEvent);
            LogResults("Didn't receive event message", receivedEvent);
        }
    }
}