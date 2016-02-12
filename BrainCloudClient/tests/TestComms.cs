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

        [SetUp]
        public void RegisterCallbacks()
        {
            BrainCloudClient.Instance.RegisterGlobalErrorCallback(GlobalErrorHandler);
        }

        [TearDown]
        public void Cleanup()
        {
            BrainCloudClient.Instance.DeregisterGlobalErrorCallback();
            _globalErrorCount = 0;
        }

        [Test]
        public void TestNoSession()
        {
            //BrainCloudClient.Instance.ResetCommunication();
            //BrainCloudClient.Instance.Initialize(_serverUrl, _secret, _appId, _version);
            //BrainCloudClient.Instance.EnableLogging(true);

            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION);
        }

        [Test]
        public void TestSessionTimeout()
        {
            //BrainCloudClient.Instance.ResetCommunication();
            //BrainCloudClient.Instance.Initialize(_serverUrl, _secret, _appId, _version);
            //BrainCloudClient.Instance.EnableLogging(true);

            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Console.WriteLine("\nWaiting for session to expire...");
            Thread.Sleep(61 * 1000);

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);
        }

        [Test]
        public void TestBadUrl()
        {
            BrainCloudClient.Instance.Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
            BrainCloudClient.Instance.EnableLogging(true);

            DateTime timeStart = DateTime.Now;
            TestResult tr = new TestResult();
            tr.SetTimeToWaitSecs(120);
            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

            DateTime timeEnd = DateTime.Now;
            TimeSpan delta = timeEnd.Subtract(timeStart);
            Assert.True(delta >= TimeSpan.FromSeconds(13) && delta <= TimeSpan.FromSeconds(17));
        }

        [Test]
        public void TestPacketTimeouts()
        {
            try
            {
                BrainCloudClient.Instance.Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
                BrainCloudClient.Instance.EnableLogging(true);
                BrainCloudClient.Instance.SetPacketTimeouts(new List<int> { 3, 3, 3 });

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult();
                tr.SetTimeToWaitSecs(120);
                BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
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
                BrainCloudClient.Instance.SetPacketTimeoutsToDefault();
            }
        }

        public void MessageCacheGlobalError()
        {
            
        }

        [Test]
        public void TestMessageCache()
        {
            try
            {
                BrainCloudClient.Get().Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
                BrainCloudClient.Get().EnableCachedMessagesOnTimeout(true);
                BrainCloudClient.Get().EnableLogging(true);
                BrainCloudClient.Get().SetPacketTimeouts(new List<int> { 1, 1, 1 });

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult();
                tr.SetTimeToWaitSecs(30);
                BrainCloudClient.Get().RegisterGlobalErrorCallback(tr.ApiError);
                BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

                BrainCloudClient.Get().RetryCachedMessages();
                tr.Reset();
                tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

                BrainCloudClient.Get().FlushCachedMessages(true);
                // unable to catch the api callback in this case using tr...

                //tr.Reset();
                //tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);
            }
            finally
            {
                // reset to defaults
                BrainCloudClient.Get().SetPacketTimeoutsToDefault();
                BrainCloudClient.Get().EnableCachedMessagesOnTimeout(false);
                BrainCloudClient.Get().DeregisterGlobalErrorCallback();
            }
        }


        /*
        [Test]
        public void Test503()
        {
            try 
            {
                BrainCloudClient.Instance.Initialize("http://localhost:5432", _secret, _appId, _version);
                BrainCloudClient.Get ().EnableLogging(true);

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult();
                tr.SetTimeToWaitSecs(120);
                BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
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

            BrainCloudClient.Instance.Initialize(_serverUrl, _secret, _appId, _version);
            BrainCloudClient.Instance.EnableLogging(true);

            TestResult tr = new TestResult();
            BrainCloudClient.Instance.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.True(tr.m_statusMessage.StartsWith("{"));

            BrainCloudClient.Instance.SetOldStyleStatusMessageErrorCallback(true);
            tr.Reset();

            BrainCloudClient.Instance.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.False(tr.m_statusMessage.StartsWith("{"));

            // try now using 900 client timeout
            BrainCloudClient.Instance.Initialize("http://localhost:5432", _secret, _appId, _version);

            tr.Reset();
            BrainCloudClient.Instance.SetOldStyleStatusMessageErrorCallback(false);
            BrainCloudClient.Instance.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.True(tr.m_statusMessage.StartsWith("{"));

            tr.Reset();
            BrainCloudClient.Instance.SetOldStyleStatusMessageErrorCallback(true);
            BrainCloudClient.Instance.EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine(tr.m_statusMessage);
            Assert.False(tr.m_statusMessage.StartsWith("{"));

            BrainCloudClient.Instance.SetOldStyleStatusMessageErrorCallback(false);
            BrainCloudClient.Instance.ResetCommunication();
        }

        [Test]
        public void TestGlobalErrorCallback()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION);
            
            Assert.AreEqual(_globalErrorCount, 1);

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.EntityService.UpdateEntity(
                "fakeId",
                "type",
                Helpers.CreateJsonPair("test", 2),
                null,
                -1,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(404, 40332);
            
            Assert.AreEqual(_globalErrorCount, 2);
        }

        private void GlobalErrorHandler(int status, int reasonCode, string jsonError, object cbObject)
        {
            _globalErrorCount++;
            Console.Out.WriteLine("Global error: " + jsonError);
        }
    }
}