using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using System;
using System.Threading;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestRTT : TestFixtureBase
    {
        [Test]
        public void TestEnableDisableRTTWithWS()
        {
            TestResult tr = new TestResult(_bc);
            _bc.RTTService.DisableRTT(); // This shouldn't callback error
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRTTHeartBeat()
        {
            
            TestResult tr = new TestResult(_bc);
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Run for 90sec and see if the heartbeat did its job
            var timeBefore = DateTime.Now;
            while ((DateTime.Now - timeBefore).TotalSeconds < 90.0) // 90sec
            {
                _bc.Update();
                Thread.Sleep(16);
            }
            Assert.True(_bc.RTTService.IsRTTEnabled());
        }

        [Test]
        public void TestRTTChatCallback()
        {
            TestResult tr = new TestResult(_bc);

            // Enable RTT
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Get Channel Id
            _bc.ChatService.GetChannelId("gl", "valid", tr.ApiSuccess, tr.ApiError);
            tr.Run();
            string channelId = (tr.m_response["data"] as Dictionary<string, object>)["channelId"] as string;

            // Connect to channel
            _bc.ChatService.ChannelConnect(channelId, 50, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Register for RTT chat
            bool receivedChat = false;
            _bc.RTTService.RegisterRTTChatCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "chat" &&
                    response["operation"] as string == "INCOMING")
                {
                    receivedChat = true;
                }
            });

            // Send a chat message
            _bc.ChatService.PostChatMessageSimple(channelId, "Unit test message", true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Now check if we get the chat message
            var timeBefore = DateTime.Now;
            while (!receivedChat && (DateTime.Now - timeBefore).TotalSeconds < 30.0) // 30sec wait is enough (heck, 10sec is enough)
            {
                _bc.Update();
                Thread.Sleep(16); // Simulate 60 fps
            }
            Assert.True(receivedChat);

            // Now deregister and make sure we don't receive it
            _bc.RTTService.DeregisterRTTChatCallback();

            // Send a chat message again
            receivedChat = false; // Reset
            _bc.ChatService.PostChatMessageSimple(channelId, "Unit test message 2", true, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Wait 10sec, and make sure we don't get the event this time
            timeBefore = DateTime.Now;
            while (!receivedChat && (DateTime.Now - timeBefore).TotalSeconds < 10.0) // 10sec
            {
                _bc.Update();
                Thread.Sleep(16);
            }
            Assert.False(receivedChat);
        }

        [Test]
        public void TestRTTLobbyCallback()
        {
            TestResult tr = new TestResult(_bc);

            // Enable RTT
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Register for RTT lobby
            bool receivedLobby = false;
            _bc.RTTService.RegisterRTTLobbyCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "lobby")
                {
                    receivedLobby = true;
                }
            });

            // Create a lobby
            _bc.LobbyService.CreateLobby("MATCH_UNRANKED", 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Now check if we get the lobby message
            // Wait 300sec, creating lobby can take time
            var timeBefore = DateTime.Now;
            while (!receivedLobby && (DateTime.Now - timeBefore).TotalSeconds < 300.0)
            {
                _bc.Update();
                Thread.Sleep(16);
            }
            Assert.True(receivedLobby);
        }

        [Test]
        public void TestRTTEventCallback()
        {
            TestResult tr = new TestResult(_bc);

            // Enable RTT
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Register for RTT lobby
            bool receivedEvent = false;
            _bc.RTTService.RegisterRTTEventCallback((string json) =>
            {
                var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
                if (response["service"] as string == "event")
                {
                    receivedEvent = true;
                }
            });

            // Send an event
            var profileId = _bc.Client.AuthenticationService.ProfileId;
            _bc.EventService.SendEvent(profileId, "test", "{\"testData\":42}", tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Now check if we get the event message
            var timeBefore = DateTime.Now;
            while (!receivedEvent && (DateTime.Now - timeBefore).TotalSeconds < 30.0)
            {
                _bc.Update();
                Thread.Sleep(16);
            }
            Assert.True(receivedEvent);
        }
    }
}
