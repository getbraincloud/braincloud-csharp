using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestComms : TestFixtureNoAuth
    {
        private int _globalErrorCount;

        [TearDown]
        public void Cleanup()
        {
            _bc.Client.DeregisterGlobalErrorCallback();
            _globalErrorCount = 0;
        }

        [Test]
        public void TestNoSession()
        {
            //_bc.ResetCommunication();
            //_bc.Initialize(_serverUrl, _secret, _appId, _version);
            //_bc.EnableLogging(true);

            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION);
        }

        [Test]
        public void TestSessionTimeout()
        {
            //_bc.ResetCommunication();
            //_bc.Initialize(_serverUrl, _secret, _appId, _version);
            //_bc.EnableLogging(true);

            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Console.WriteLine("\nWaiting for session to expire...");
            Thread.Sleep(61 * 1000);

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);
        }

        //[Test] //TODO Fix
        public void TestBadUrl()
        {
            _bc.Init(ServerUrl + "unitTestFail", Secret, AppId, Version);
            _bc.Client.EnableLogging(true);

            DateTime timeStart = DateTime.Now;
            TestResult tr = new TestResult(_bc);
            tr.SetTimeToWaitSecs(120);
            _bc.Client.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

            DateTime timeEnd = DateTime.Now;
            TimeSpan delta = timeEnd.Subtract(timeStart);
            Assert.True(delta >= TimeSpan.FromSeconds(13) && delta <= TimeSpan.FromSeconds(17));
        }

        //[Test] //TODO Fix
        public void TestPacketTimeouts()
        {
            try
            {
                _bc.Init(ServerUrl + "unitTestFail", Secret, AppId, Version);
                _bc.Client.EnableLogging(true);
                _bc.Client.SetPacketTimeouts(new List<int> { 3, 3, 3 });

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult(_bc);
                tr.SetTimeToWaitSecs(120);
                _bc.Client.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);


                DateTime timeEnd = DateTime.Now;
                TimeSpan delta = timeEnd.Subtract(timeStart);
                if (delta < TimeSpan.FromSeconds(8) && delta > TimeSpan.FromSeconds(15))
                {
                    Console.WriteLine("Failed timing check - took " + delta.TotalSeconds + " seconds");
                    Assert.Fail();
                }

            }
            finally
            {
                // reset to defaults
                _bc.Client.SetPacketTimeoutsToDefault();
            }
        }

        public void MessageCacheGlobalError()
        {
            
        }

        //[Test] //TODO Fix
        public void TestMessageCache()
        {
            try
            {
                _bc.Init(ServerUrl + "unitTestFail", Secret, AppId, Version);
                _bc.Client.EnableNetworkErrorMessageCaching(true);
                _bc.Client.EnableLogging(true);
                _bc.Client.SetPacketTimeouts(new List<int> { 1, 1, 1 });

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult(_bc);
                tr.SetTimeToWaitSecs(30);
                _bc.Client.RegisterNetworkErrorCallback(tr.NetworkError);
                _bc.Client.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

                _bc.Client.RetryCachedMessages();
                tr.Reset();
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

                _bc.Client.FlushCachedMessages(true);
                // unable to catch the api callback in this case using tr...

                //tr.Reset();
                //tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);
            }
            finally
            {
                // reset to defaults
                _bc.Client.SetPacketTimeoutsToDefault();
                _bc.Client.EnableNetworkErrorMessageCaching(false);
                _bc.Client.DeregisterNetworkErrorCallback();
            }
        }


        /*
        [Test]
        public void Test503()
        {
            try 
            {
                _bc.Initialize("http://localhost:5432", _secret, _appId, _version);
                BrainCloudClient.Get ().EnableLogging(true);

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult(_bc);
                tr.SetTimeToWaitSecs(120);
                _bc.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);
                
                
                DateTime timeEnd = DateTime.Now;
                TimeSpan delta = timeEnd.Subtract(timeStart);
                if (delta < TimeSpan.FromSeconds (8) && delta > TimeSpan.FromSeconds(15))
                {
                    Console.WriteLine("Failed timing check - took " + delta.TotalSeconds + " seconds");
                    Assert.Fail ();
                }
                
            }
            finally
            {
                // reset to defaults
                BrainCloudClient.Get ().SetPacketTimeoutsToDefault();
            }
            
        }*/

        [Test]
        public void TestErrorCallback()
        {

            _bc.Init(ServerUrl, Secret, AppId, Version);
            _bc.Client.EnableLogging(true);

            TestResult tr = new TestResult(_bc);
            _bc.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.True(tr.m_statusMessage.StartsWith("{"));

            _bc.Client.SetOldStyleStatusMessageErrorCallback(true);
            tr.Reset();

            _bc.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.False(tr.m_statusMessage.StartsWith("{"));

            // try now using 900 client timeout
            _bc.Init("http://localhost:5432", Secret, AppId, Version);

            tr.Reset();
            _bc.Client.SetOldStyleStatusMessageErrorCallback(false);
            _bc.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.True(tr.m_statusMessage.StartsWith("{"));

            tr.Reset();
            _bc.Client.SetOldStyleStatusMessageErrorCallback(true);
            _bc.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.False(tr.m_statusMessage.StartsWith("{"));

            _bc.Client.SetOldStyleStatusMessageErrorCallback(false);
            _bc.Client.ResetCommunication();
        }

        [Test]
        public void TestGlobalErrorCallback()
        {
            _bc.Client.RegisterGlobalErrorCallback(GlobalErrorHandler);
            TestResult tr = new TestResult(_bc);

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION);

            Assert.AreEqual(_globalErrorCount, 1);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.EntityService.UpdateEntity(
                "fakeId",
                "type",
                Helpers.CreateJsonPair("test", 2),
                null,
                -1,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(404, 40332);

            Assert.AreEqual(_globalErrorCount, 2);
        }

        [Test]
        public void TestGlobalErrorCallbackUsingWrapper()
        {
            _bc.Client.RegisterGlobalErrorCallback(GlobalErrorHandler);
            TestResult tr = new TestResult(_bc);

            _bc.AuthenticateUniversal("", "zzz", true, tr.ApiSuccess, tr.ApiError, this);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.TOKEN_DOES_NOT_MATCH_USER);

            Assert.AreEqual(_globalErrorCount, 1);
        }

        [Test]
        public void TestMessageBundleMarker()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            _bc.Client.InsertEndOfMessageBundleMarker();

            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);
            _bc.Client.InsertEndOfMessageBundleMarker();

            // to make sure it doesn't die on first message being marker
            _bc.Client.InsertEndOfMessageBundleMarker();

            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);
            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);
            
            // should result in three packets
            tr.Run();
            tr.Run();
            tr.Run();
        }

        [Test]
        public void TestAuthFirst()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);

            _bc.Client.InsertEndOfMessageBundleMarker();

            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);



            // should result in two packets
            tr.RunExpectFail(403, ReasonCodes.NO_SESSION);
            tr.Run();
            tr.Run();
        }

        private void GlobalErrorHandler(int status, int reasonCode, string jsonError, object cbObject)
        {
            if (cbObject != null)
            {
                if (cbObject.GetType().ToString() == "BrainCloud.Internal.WrapperAuthCallbackObject")
                {
                    Console.WriteLine("GlobalErrorHandler received internal WrapperAuthCallbackObject object: " + cbObject.GetType().ToString());
                    throw new Exception("GlobalErrorHandler received internal WrapperAuthCallbackObject object");
                }
            }

            _globalErrorCount++;
            Console.Out.WriteLine("Global error: " + jsonError);
            Console.Out.WriteLine("Callback object: " + cbObject);
        }
    }

    [TestFixture]
    public class TestCommsNoAuth : TestFixtureBase
    {
        [Test]
        public void TestRetry30Sec()
        {
            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunScript("TestTimeoutRetry", Helpers.CreateJsonPair("testParm1", 1), tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRetry45Sec()
        {
            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunScript("TestTimeoutRetry45", Helpers.CreateJsonPair("testParm1", 1), tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);
        }
    }
}