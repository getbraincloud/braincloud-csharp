using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestOneWayMatch : TestFixtureBase
    {
        [Test]
        public void TestStartMatch()
        {
            string streamId = StartMatch();
            CancelMatch(streamId);
        }

        [Test]
        public void TestCancelMatch()
        {
            string streamId = StartMatch();
            CancelMatch(streamId);
        }

        [Test]
        public void TestCompleteMatch()
        {
            string streamId = StartMatch();
            TestResult tr = new TestResult(_bc);

            _bc.OneWayMatchService.CompleteMatch(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #region Helper functions

        private string StartMatch()
        {
            TestResult tr = new TestResult(_bc);
            string streamId = "";

            _bc.OneWayMatchService.StartMatch(
                GetUser(Users.UserB).ProfileId,
                1000,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                streamId = (string)(((Dictionary<string, object>)tr.m_response["data"])["playbackStreamId"]);
            }

            return streamId;
        }

        private void CancelMatch(string streamId)
        {
            TestResult tr = new TestResult(_bc);
            _bc.OneWayMatchService.CancelMatch(
                streamId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        #endregion
    }
}