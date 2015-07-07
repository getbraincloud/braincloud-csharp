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
        private readonly string _otherPlayerid = "0f79d5fd-103f-4e9d-aecf-15228eb13f74";

        [Test]
        public void TestStartMatch()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().OneWayMatchService.StartMatch(
                _otherPlayerid,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CancelMatch(GetStreamId());
        }

        [Test]
        public void TestCancelMatch()
        {
            StartMatch();
            string streamId = GetStreamId();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().OneWayMatchService.CancelMatch(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCompleteMatch()
        {
            StartMatch();
            string streamId = GetStreamId();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().OneWayMatchService.CompleteMatch(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #region Helper functions

        private void StartMatch()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().OneWayMatchService.StartMatch(
                _otherPlayerid,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private void CancelMatch(string streamId)
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().OneWayMatchService.CancelMatch(
                streamId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        private string GetStreamId()
        {
            TestResult tr = new TestResult();

            string streamId = "";

            BrainCloudClient.Get().PlaybackStreamService.GetStreamSummariesForTargetPlayer(
                _otherPlayerid,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                Dictionary<string, object>[] streams = (Dictionary<string, object>[])(((Dictionary<string, object>)(tr.m_response["data"]))["streams"]);
                streamId = (string)streams[0]["playbackStreamId"];
            }

            return streamId;
        }

        #endregion
    }
}