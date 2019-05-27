using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlaybackStream : TestFixtureBase
    {
        [Test]
        public void TestStartStream()
        {
            TestResult tr = new TestResult(_bc);
            string streamId = "";

            _bc.PlaybackStreamService.StartStream(
                GetUser(Users.UserB).ProfileId,
                true,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                streamId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["playbackStreamId"];
            }

            EndStream(streamId);
        }

        [Test]
        public void TestEndStream()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.EndStream(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDeleteStream()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.DeleteStream(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestAddEvent()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.AddEvent(
                streamId,
                Helpers.CreateJsonPair("data", 1),
                Helpers.CreateJsonPair("total", 5),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            EndStream(streamId);
        }
        
        [Test]
        public void TestGetStreamSummariesForInitiatingPlayer()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.GetRecentStreamsForInitiatingPlayer(
                GetUser(Users.UserB).ProfileId, 5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            EndStream(streamId);
        }
        
        [Test]
        public void TestGetRecentStreamsForInitiatingPlayer()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();
            
            _bc.PlaybackStreamService.GetRecentStreamsForInitiatingPlayer(
                GetUser(Users.UserA).ProfileId,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            EndStream(streamId);
        }
        
        [Test]
        public void TestGetRecentStreamsForTargetPlayer()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.GetRecentStreamsForTargetPlayer(
                GetUser(Users.UserB).ProfileId,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            EndStream(streamId);
        }

        [Test]
        public void TestReadStream()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = StartStream();

            _bc.PlaybackStreamService.ReadStream(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            EndStream(streamId);
        }

        #region Helper Functions

        /// <summary>
        /// Starts a new stream with the default player
        /// </summary>
        /// <returns> streamId </returns>
        private string StartStream()
        {
            TestResult tr = new TestResult(_bc);
            string streamId = "";

            _bc.PlaybackStreamService.StartStream(
                GetUser(Users.UserB).ProfileId,
                true,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                streamId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["playbackStreamId"];
            }

            return streamId;
        }

        private void EndStream(string streamId)
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlaybackStreamService.EndStream(
                streamId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private string GetStreamId()
        {
            TestResult tr = new TestResult(_bc);

            string streamId = "";

            _bc.PlaybackStreamService.GetRecentStreamsForTargetPlayer(
                GetUser(Users.UserB).ProfileId, 5,
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