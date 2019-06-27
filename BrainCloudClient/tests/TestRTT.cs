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
        // public void TestEnableRTTAfterRegister()
        // {
        //     TestResult trA = new TestResult(_bc);
        //     _bc.Client.AuthenticationService.AuthenticateUniversal(GetUser(Users.UserA).Id, GetUser(Users.UserA).Password, false, trA.ApiSuccess, trA.ApiError);
        //     trA.Run();
            
        //     TestResult tr4 = new TestResult(_bc);
        //     _bc.RTTService.RegisterRTTChatCallback(onRTTCallback);
        //     tr4.Run();

        //     TestResult tr = new TestResult(_bc);
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     TestResult tr2 = new TestResult(_bc);
        //     _bc.ChatService.GetChannelId("gl", "valid", tr2.ApiSuccess, tr2.ApiError);
        //     tr2.Run();
        //     var data = tr.m_response["data"] as Dictionary<string, object>;
        //     string channelId = data["channelId"] as string;

        //     TestResult tr3 = new TestResult(_bc);
        //     _bc.ChatService.ChannelConnect(channelId, 50, tr3.ApiSuccess, tr3.ApiError);
        //     tr3.Run();

        //     // wait some time
        //     DateTime _testPauseStart = DateTime.Now;
        //     TimeSpan _testPauseDuration = TimeSpan.FromSeconds(0.25);

        //     //make a timer for the test.
        //     while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
        //     {
        //         //putting the test into a while loop until it passes this condition
        //         float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
        //         Console.WriteLine(time);
        //     }

        //     TestResult tr5 = new TestResult(_bc);
        //     _bc.ChatService.PostChatMessageSimple(channelId, "test message", true, tr5.ApiSuccess, tr5.ApiError);
        //     tr5.Run();

        //     //now to to check that we received callback.
        //     //Is there a way we do this usually?

        //     //make a timer for the test.
        //     while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
        //     {
        //         //putting the test into a while loop until it passes this condition
        //         float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
        //         Console.WriteLine(time);
        //     }

        //     TestResult tr6 = new TestResult(_bc);
        //     _bc.RTTService.DeregisterRTTChatCallback();
        //     tr6.Run();
        // }

        // [Test]
        // public void TestEnableRTTBeforeRegister()
        // {
        //     TestResult trA = new TestResult(_bc);
        //     _bc.Client.AuthenticationService.AuthenticateUniversal(GetUser(Users.UserA).Id, GetUser(Users.UserA).Password, false, trA.ApiSuccess, trA.ApiError);
        //     trA.Run();
            
        //     TestResult tr = new TestResult(_bc);
        //     _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, tr.ApiSuccess, tr.ApiError);
        //     tr.Run();

        //     // wait some time
        //     DateTime _testPauseStart = DateTime.Now;
        //     TimeSpan _testPauseDuration = TimeSpan.FromSeconds(2);

        //     //make a timer for the test.
        //     while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
        //     {
        //         //putting the test into a while loop until it passes this condition
        //         float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
        //         Console.WriteLine(time);
        //     }

        //     TestResult tr2 = new TestResult(_bc);
        //     _bc.ChatService.GetChannelId("gl", "valid", tr2.ApiSuccess, tr2.ApiError);
        //     tr2.Run();
        //     var data = tr.m_response["data"] as Dictionary<string, object>;
        //     string channelId = data["channelId"] as string;

        //     TestResult tr3 = new TestResult(_bc);
        //     _bc.ChatService.ChannelConnect(channelId, 50, tr3.ApiSuccess, tr3.ApiError);
        //     tr3.Run();

        //     TestResult tr4 = new TestResult(_bc);
        //     _bc.RTTService.RegisterRTTChatCallback(onRTTCallback);
        //     tr4.Run();

        //     _testPauseStart = DateTime.Now;
        //     _testPauseDuration = TimeSpan.FromSeconds(1);
        //     //make a timer for the test.
        //     while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
        //     {
        //         //putting the test into a while loop until it passes this condition
        //         float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
        //         Console.WriteLine(time);
        //     }

        //     TestResult tr5 = new TestResult(_bc);
        //     _bc.ChatService.PostChatMessageSimple(channelId, "test message", true, tr5.ApiSuccess, tr5.ApiError);
        //     tr5.Run();

        //     //now to to check that we received callback.
        //     //Is there a way we do this usually?

        //     _testPauseStart = DateTime.Now;
        //     _testPauseDuration = TimeSpan.FromSeconds(1);

        //     //make a timer for the test.
        //     while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
        //     {
        //         //putting the test into a while loop until it passes this condition
        //         float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
        //         Console.WriteLine(time);
        //     }

        //     TestResult tr6 = new TestResult(_bc);
        //     _bc.RTTService.DeregisterRTTChatCallback();
        //     tr6.Run();
        // }

        [Test]
        public void TestEnableRTTWithRegister()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.RTTService.EnableRTT(RTTConnectionType.WEBSOCKET, (SuccessCallback)OnEnableRTTSuccess + tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            TestResult tr2 = new TestResult(_bc);
            _bc.RTTService.RegisterRTTChatCallback(onRTTChatCallback);
            tr2.Run();
        }

        public void OnEnableRTTSuccess(string eventJson, object obj)
        {
            // the callback responded to
            Console.WriteLine("OnEnableRTTSuccess");

            TestResult tr1 = new TestResult(_bc);
            _bc.ChatService.GetChannelId("gl", "valid", tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            var data = tr1.m_response["data"] as Dictionary<string, object>;
            m_channelId = data["channelId"] as string;

            TestResult tr2 = new TestResult(_bc);
            _bc.ChatService.ChannelConnect(m_channelId, 50, (SuccessCallback)onChannelConnect + tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }
        private string m_channelId = "";

        private void onChannelConnect(string json, object obj)
        {

            // the callback responded to
            Console.WriteLine("CHANNEL Connected");
            TestResult tr5 = new TestResult(_bc);
            _bc.ChatService.PostChatMessageSimple(channelId, "test message", true, tr5.ApiSuccess, tr5.ApiError);
            tr5.Run();

        }

        private void onRTTChatCallback(string json, object obj)
        {
            // the callback responded to
            Console.WriteLine("CHANNEL Connected");
        }
    }
}