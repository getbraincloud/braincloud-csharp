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

            _bc.PresenceService.ForcePush(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetPresenceOfFriends()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.GetPresenceOfFriends(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetPresenceOfGroup()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.GetPresenceOfGroup(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetPresenceOfUsers()
        {
            List<string> data = new List<string>();
            data.Add("aaa-bbb-ccc");
            data.Add("bbb-ccc-ddd");

            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.GetTournamentStatus(
                data,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void RegisterListenersForFriends()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.RegisterListenersForFriends(
                "brainCloud",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void RegisterListenersForGroup()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.RegisterListenersForGroup(
                "aaa-bbb-ccc",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void RegisterListenersForProfiles()
        {
            List<string> data = new List<string>();
            data.Add("aaa-bbb-ccc");
            data.Add("bbb-ccc-ddd");

            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.RegisterListenersForProfiles(
                data,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void SetVisibility()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.SetVisibility(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, PRESENCE_NOT_INITIALIZED);
        }

        [Test]
        public void StopListening()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.StopListening(
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, PRESENCE_NOT_INITIALIZED);
        }

        [Test]
        public void UpdateActivity()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PresenceService.UpdateActivity(
                "{\"test\":\"thing\"}",
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, PRESENCE_NOT_INITIALIZED);
        }
    }
}