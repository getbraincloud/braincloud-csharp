using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAsyncMatch : TestFixtureBase
    {
        private readonly string _otherPlayerid = "029b1c4e-61ca-4a59-8475-9921349c9cc3";
        private readonly string _otherPlatform = "BC";
        private readonly string _matchId = "match01";

        [Test]
        public void TestCreateMatch()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _otherPlatform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
                null,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            string ownerId = "";

            if (tr.Run())
            {
                ownerId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["ownerId"];
            }

            AbandonMatch(ownerId);
        }

        [Test]
        public void TestAbandonMatch()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.AbandonMatch(
                ownerId,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCompleteMatch()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.CompleteMatch(
                ownerId,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDeleteMatch()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.DeleteMatch(
                ownerId,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindCompleteMatches()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.FindCompleteMatches(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindMatches()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.FindMatches(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadMatch()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.ReadMatch(
                ownerId,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(ownerId);
        }

        [Test]
        public void TestReadMatchHistory()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.ReadMatch(
                ownerId,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(ownerId);
        }

        [Test]
        public void TestCreateMatchWithInitialTurn()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _otherPlatform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatchWithInitialTurn(
                JsonWriter.Serialize(players),
                Helpers.CreateJsonPair("blob", 1),
                null,
                _matchId,
                null,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            string ownerId = "";

            if (tr.Run())
            {
                ownerId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["ownerId"];
            }

            AbandonMatch(ownerId);
        }

        [Test]
        public void TestSubmitTurn()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.SubmitTurn(
                ownerId,
                _matchId,
                1,
                Helpers.CreateJsonPair("blob", 1),
                null,
                null,
                Helpers.CreateJsonPair("map", "level1"),
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(ownerId);
        }

        [Test]
        public void TestUpdateMatchSummaryData()
        {
            string ownerId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.UpdateMatchSummaryData(
                ownerId,
                _matchId,
                1,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(ownerId);
        }

        #region Helper functions

        private string CreateMatch()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _otherPlatform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
                null,
                _matchId,
                tr.ApiSuccess, tr.ApiError);

            string ownerId = "";

            if (tr.Run())
            {
                ownerId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["ownerId"];
            }

            return ownerId;
        }

        private void AbandonMatch(string ownerId)
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().AsyncMatchService.AbandonMatch(
               ownerId,
               _matchId,
               tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        #endregion
    }
}