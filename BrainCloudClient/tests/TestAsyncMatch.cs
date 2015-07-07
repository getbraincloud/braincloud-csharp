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
        private readonly string _otherPlayerid = "0f79d5fd-103f-4e9d-aecf-15228eb13f74";
        private readonly string _platform = "BC";

        [Test]
        public void TestCreateMatch()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
                null,
                null,
                tr.ApiSuccess, tr.ApiError);

            string matchId = "";

            if (tr.Run())
            {
                matchId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["matchId"];
            }

            AbandonMatch(matchId);
        }

        [Test]
        public void TestAbandonMatch()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.AbandonMatch(
                Helpers.GetMyPlayerId(),
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCompleteMatch()
        {
            string matchId = CreateMatchWithInitialTurn();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.CompleteMatch(
                Helpers.GetMyPlayerId(),
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        /*[Test]
        public void TestDeleteMatch()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.DeleteMatch(
                Helpers.GetMyPlayerId(),
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }*/

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
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.ReadMatch(
                matchId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestReadMatchHistory()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.ReadMatch(
                Helpers.GetMyPlayerId(),
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestCreateMatchWithInitialTurn()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatchWithInitialTurn(
                JsonWriter.Serialize(players),
                Helpers.CreateJsonPair("blob", 1),
                null,
                null,
                null,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            string matchId = "";

            if (tr.Run())
            {
                matchId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["matchId"];
            }

            AbandonMatch(matchId);
        }

        [Test]
        public void TestSubmitTurn()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.SubmitTurn(
                Helpers.GetMyPlayerId(),
                matchId,
                0,
                Helpers.CreateJsonPair("blob", 1),
                null,
                _otherPlayerid,
                Helpers.CreateJsonPair("map", "level1"),
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestUpdateMatchSummaryData()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AsyncMatchService.UpdateMatchSummaryData(
                Helpers.GetMyPlayerId(),
                matchId,
                0,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        #region Helper functions

        private string CreateMatch()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
                null,
                null,
                tr.ApiSuccess, tr.ApiError);

            string matchId = "";

            if (tr.Run())
            {
                matchId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["matchId"];
            }

            return matchId;
        }

        private string CreateMatchWithInitialTurn()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", _otherPlayerid } };

            BrainCloudClient.Get().AsyncMatchService.CreateMatchWithInitialTurn(
                JsonWriter.Serialize(players),
                Helpers.CreateJsonPair("map", "level1"),
                null,
                null,
                null,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            string matchId = "";

            if (tr.Run())
            {
                matchId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["matchId"];
            }

            return matchId;
        }

        private void AbandonMatch(string matchId)
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().AsyncMatchService.AbandonMatch(
               Helpers.GetMyPlayerId(),
               matchId,
               tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        #endregion
    }
}