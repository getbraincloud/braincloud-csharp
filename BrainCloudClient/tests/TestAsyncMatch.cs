using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAsyncMatch : TestFixtureBase
    {
        private readonly string _platform = "BC";

        [Test]
        public void TestCreateMatch()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", GetUser(Users.UserB).ProfileId } };

            _bc.AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
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
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.AbandonMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCompleteMatch()
        {
            string matchId = CreateMatchWithInitialTurn();
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.CompleteMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDeleteMatch()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.DeleteMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindCompleteMatches()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.FindCompleteMatches(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindMatches()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.FindMatches(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadMatch()
        {
            string matchId = CreateMatchWithInitialTurn();
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.ReadMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestReadMatchHistory()
        {
            string matchId = CreateMatch();
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.ReadMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestCreateMatchWithInitialTurn()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", GetUser(Users.UserB).ProfileId } };

            _bc.AsyncMatchService.CreateMatchWithInitialTurn(
                JsonWriter.Serialize(players),
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
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.SubmitTurn(
                GetUser(Users.UserA).ProfileId,
                matchId,
                0,
                Helpers.CreateJsonPair("blob", 1),
                null,
                GetUser(Users.UserB).ProfileId,
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
            TestResult tr = new TestResult(_bc);

            _bc.AsyncMatchService.UpdateMatchSummaryData(
                GetUser(Users.UserA).ProfileId,
                matchId,
                0,
                Helpers.CreateJsonPair("map", "level1"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            AbandonMatch(matchId);
        }

        [Test]
        public void TestCompleteMatchWithSummaryData()
        {

        }

        [Test]
        public void TestAbandonMatchWithSummaryData()
        {

        }

        #region Helper functions

        private string CreateMatch()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", GetUser(Users.UserB).ProfileId } };

            _bc.AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
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
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", GetUser(Users.UserB).ProfileId } };

            _bc.AsyncMatchService.CreateMatchWithInitialTurn(
                JsonWriter.Serialize(players),
                Helpers.CreateJsonPair("map", "level1"),
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
            TestResult tr = new TestResult(_bc);
            _bc.AsyncMatchService.AbandonMatch(
               GetUser(Users.UserA).ProfileId,
               matchId,
               tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        #endregion
    }
}