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
        // [Test]
        // public void TestRequestClientConnection()
        // {
        //     TestResult tr = new TestResult(_bc);
        //     _bc.RTTService.RequestClientConnection(tr.ApiSuccess, tr.ApiError);
        //     tr.Run();
        // }

        // C# doesn't support TCP RTT (Yet)
        // [Test]
        // public void TestEnableDisableRTTWithTCP()
        // {
        //     TestResult tr = new TestResult(_bc);
        //     _bc.RTTService.DisableRTT();
        //     _bc.RTTService.EnableRTT(RTTConnectionType.TCP, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();
        // }

        // [Test]
        // public void TestEnableDisableRTTWithWS()
        // {
        //     TestResult tr = new TestResult(_bc);
        //     _bc.RTTService.DisableRTT(); // This shouldn't callback error
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();
        // }

        [Test]
        public void TestRTTHeartBeat()
        {
            
            TestResult tr = new TestResult(_bc);
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            // Run for 90sec and see if the heartbeat did its job
            int startTime = Environment.TickCount;
            while (Environment.TickCount - startTime < 90000) // 90sec
            {
                _bc.Update();
                Thread.Sleep(100);
            }
            Assert.True(_bc.RTTService.IsRTTEnabled());
        }

        // [Test]
        // public void TestRTTChatCallback()
        // {
        //     TestResult tr = new TestResult(_bc);

        //     // Enable RTT
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Get Channel Id
        //     _bc.ChatService.GetChannelId("gl", "valid", tr.ApiSuccess, tr.ApiError);
        //     tr.Run();
        //     string channelId = (tr.m_response["data"] as Dictionary<string, object>)["channelId"] as string;

        //     // Connect to channel
        //     _bc.ChatService.ChannelConnect(channelId, 50, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Register for RTT chat
        //     bool receivedChat = false;
        //     _bc.RTTService.RegisterRTTChatCallback((string json) =>
        //     {
        //         var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
        //         if (response["service"] as string == "chat" &&
        //             response["operation"] as string == "INCOMING")
        //         {
        //             receivedChat = true;
        //         }
        //     });

        //     // Send a chat message
        //     _bc.ChatService.PostChatMessageSimple(channelId, "Unit test message", true, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Now check if we get the chat message
        //     int timeout = 100; // 10 seconds
        //     while (!receivedChat && timeout > 0)
        //     {
        //         _bc.Update();
        //         Thread.Sleep(100);
        //         timeout--;
        //     }
        //     Assert.True(receivedChat);

        //     // Now deregister and make sure we don't receive it
        //     _bc.RTTService.DeregisterRTTChatCallback();

        //     // Send a chat message again
        //     receivedChat = false; // Reset
        //     _bc.ChatService.PostChatMessageSimple(channelId, "Unit test message 2", true, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Wait 10sec, and make sure we don't get the event this time
        //     timeout = 100; // 10 seconds
        //     while (!receivedChat && timeout > 0)
        //     {
        //         _bc.Update();
        //         Thread.Sleep(100);
        //         timeout--;
        //     }
        //     Assert.False(receivedChat);
        // }

        // [Test]
        // public void TestRTTLobbyCallback()
        // {
        //     TestResult tr = new TestResult(_bc);

        //     // Enable RTT
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Register for RTT lobby
        //     bool receivedLobby = false;
        //     _bc.RTTService.RegisterRTTLobbyCallback((string json) =>
        //     {
        //         var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
        //         if (response["service"] as string == "lobby")
        //         {
        //             receivedLobby = true;
        //         }
        //     });

        //     // Create a lobby
        //     _bc.LobbyService.CreateLobby("MATCH_UNRANKED", 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Now check if we get the lobby message
        //     // Wait 60sec, creating lobby can take time
        //     int timeout = 600; // 60 seconds
        //     while (!receivedLobby && timeout > 0)
        //     {
        //         _bc.Update();
        //         Thread.Sleep(100);
        //         timeout--;
        //     }
        //     Assert.True(receivedLobby);
        // }

        // [Test]
        // public void TestRTTEventCallback()
        // {
        //     TestResult tr = new TestResult(_bc);

        //     // Enable RTT
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Register for RTT lobby
        //     bool receivedEvent = false;
        //     _bc.RTTService.RegisterRTTEventCallback((string json) =>
        //     {
        //         var response = JsonReader.Deserialize<Dictionary<string, object>>(json);
        //         if (response["service"] as string == "event")
        //         {
        //             receivedEvent = true;
        //         }
        //     });

        //     // Send an event
        //     var profileId = _bc.Client.AuthenticationService.ProfileId;
        //     _bc.EventService.SendEvent(profileId, "test", "{\"testData\":42}", tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // Now check if we get the event message
        //     int timeout = 100; // 10 seconds
        //     while (!receivedEvent && timeout > 0)
        //     {
        //         _bc.Update();
        //         Thread.Sleep(100);
        //         timeout--;
        //     }
        //     Assert.True(receivedEvent);
        // }
    }
}
