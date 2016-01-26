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
        [Test]
        public void TestNoSession()
        {
            //BrainCloudClient.Get().ResetCommunication();
            //BrainCloudClient.Get().Initialize(_serverUrl, _secret, _appId, _version);
            //BrainCloudClient.Get().EnableLogging(true);

            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION);
        }

        [Test]
        public void TestSessionTimeout()
        {
            //BrainCloudClient.Get().ResetCommunication();
            //BrainCloudClient.Get().Initialize(_serverUrl, _secret, _appId, _version);
            //BrainCloudClient.Get().EnableLogging(true);

            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                false, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Console.WriteLine("\nWaiting for session to expire...");
            Thread.Sleep(61 * 1000);

            BrainCloudClient.Get().TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);

            BrainCloudClient.Get().TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED);
        }

        [Test]
        public void TestBadUrl()
        {
            BrainCloudClient.Get().Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
            BrainCloudClient.Get ().EnableLogging(true);

            DateTime timeStart = DateTime.Now;
            TestResult tr = new TestResult();
            tr.SetTimeToWaitSecs(120);
            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);

            DateTime timeEnd = DateTime.Now;
            TimeSpan delta = timeEnd.Subtract(timeStart);
            Assert.True (delta >= TimeSpan.FromSeconds (13) && delta <= TimeSpan.FromSeconds(17));
        }

        [Test]
        public void TestPacketTimeouts()
        {
            try 
            {
                BrainCloudClient.Get().Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
                BrainCloudClient.Get ().EnableLogging(true);
                BrainCloudClient.Get ().SetPacketTimeouts(new List<int> {3, 3, 3});

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult();
                tr.SetTimeToWaitSecs(120);
                BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
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

        }
        /*
        [Test]
        public void Test503()
        {
            try 
            {
                BrainCloudClient.Get().Initialize("http://localhost:5432", _secret, _appId, _version);
                BrainCloudClient.Get ().EnableLogging(true);

                DateTime timeStart = DateTime.Now;
                TestResult tr = new TestResult();
                tr.SetTimeToWaitSecs(120);
                BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
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

            BrainCloudClient.Get().Initialize(_serverUrl, _secret, _appId, _version);
            BrainCloudClient.Get().EnableLogging(true);

            TestResult tr = new TestResult();
            BrainCloudClient.Get().EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine (tr.m_statusMessage);
            Assert.True (tr.m_statusMessage.StartsWith("{"));

            BrainCloudClient.Get().SetOldStyleStatusMessageErrorCallback(true);
            tr.Reset();

            BrainCloudClient.Get().EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine (tr.m_statusMessage);
            Assert.False (tr.m_statusMessage.StartsWith("{"));

            // try now using 900 client timeout
            BrainCloudClient.Get().Initialize("http://localhost:5432", _secret, _appId, _version);

            tr.Reset();
            BrainCloudClient.Get().SetOldStyleStatusMessageErrorCallback(false);
            BrainCloudClient.Get().EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine (tr.m_statusMessage);
            Assert.True (tr.m_statusMessage.StartsWith("{"));

            tr.Reset();
            BrainCloudClient.Get().SetOldStyleStatusMessageErrorCallback(true);
            BrainCloudClient.Get().EntityService.CreateEntity("type", "", "", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(-1, -1);
            Console.Out.WriteLine (tr.m_statusMessage);
            Assert.False (tr.m_statusMessage.StartsWith("{"));

            BrainCloudClient.Get().SetOldStyleStatusMessageErrorCallback(false);
            BrainCloudClient.Get ().ResetCommunication();
        }
    }
}