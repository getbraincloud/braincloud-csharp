using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPresence : TestFixtureBase
    {
        [Test]
        public void ForcePush()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.ForcePush(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetPresenceOfFriends()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.GetPresenceOfFriends(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetPresenceOfGroup()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.GetPresenceOfGroup(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_GROUP_ID);
        }

        [Test]
        public void GetPresenceOfUsers()
        {
            List<string> data = new List<string>();
            data.Add("aaa-bbb-ccc");
            data.Add("bbb-ccc-ddd");

            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.GetPresenceOfUsers(
                data,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void RegisterListenersForFriends()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.RegisterListenersForFriends(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void RegisterListenersForGroup()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.RegisterListenersForGroup(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_GROUP_ID);
        }

        [Test]
        public void RegisterListenersForProfiles()
        {
            List<string> data = new List<string>();
            data.Add("aaa-bbb-ccc");
            data.Add("bbb-ccc-ddd");

            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.RegisterListenersForProfiles(
                data,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void SetVisibility()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.SetVisibility(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.PRESENCE_NOT_INITIALIZED);
        }

        [Test]
        public void StopListening()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.StopListening(
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.PRESENCE_NOT_INITIALIZED);
        }

        [Test]
        public void UpdateActivity()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.PresenceService.UpdateActivity(
                "{\"test\":\"thing\"}",
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_PARAMETER_TYPE);
        }
    }
}