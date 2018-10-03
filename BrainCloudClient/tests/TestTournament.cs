using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTournament : TestFixtureBase
    {
        private readonly string _divSetId = "testDivSetId";
        private readonly string _tournamentCode = "testTournament";
        private readonly string _leaderboardId = "testTournamentLeaderboard";

        private Random _rand = new Random();
        private bool _didJoin;

        [TearDown]
        public void Cleanup()
        {
            if (_didJoin)
            {
                LeaveTestTournament();
            }
        }

        [Test]
        public void ClaimTournamentReward()
        {
            int version = JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.ClaimTournamentReward(
                _leaderboardId,
                version,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, ReasonCodes.VIEWING_REWARD_FOR_NON_PROCESSED_TOURNAMENTS);
        }

        [Test]
        public void GetDivisionInfo()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.GetDivisionInfo(
                "Invalid_Id",
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, ReasonCodes.DIVISION_SET_DOESNOT_EXIST);
        }

        [Test]
        public void GetMyDivisions()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.GetMyDivisions(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void GetTournamentStatus()
        {
            int version = JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.GetTournamentStatus(
                _leaderboardId,
                version,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void JoinDivision()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.JoinDivision(
                "Invalid_Id",
                _tournamentCode,
                _rand.Next(1000),
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, ReasonCodes.DIVISION_SET_DOESNOT_EXIST);
            
        }


        [Test]
        public void JoinTournament()
        {
            JoinTestTournament();
            LeaveTestTournament();
        }

        [Test]
        public void LeaveDivisionInstance()
        {
            TestResult tr = new TestResult(_bc);
            _bc.TournamentService.LeaveDivisionInstance(
                "Invalid_id",
                tr.ApiSuccess, tr.ApiError
            );
            tr.RunExpectFail(400, ReasonCodes.LEADERBOARD_NOT_DIVISION_SET_INSTANCE);
        }

        [Test]
        public void LeaveTournament()
        {
            JoinTestTournament();
            LeaveTestTournament();
        }
        

        [Test]
        public void PostTournamentScore()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.PostTournamentScore(
                _leaderboardId,
                _rand.Next(1000),
                null,
                DateTime.Now,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            LeaveTestTournament();
        }

        [Test]
        public void PostTournamentScoreWithResults()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.PostTournamentScoreWithResults(
                _leaderboardId,
                _rand.Next(1000),
                null,
                DateTime.Now,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                10,
                10,
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            LeaveTestTournament();
        }

        [Test]
        public void ViewCurrentReward()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.ViewCurrentReward(
                _leaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            LeaveTestTournament();
        }

        [Test]
        public void ViewReward()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.ViewReward(
                _leaderboardId,
                -1,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, ReasonCodes.PLAYER_NOT_ENROLLED_IN_TOURNAMENT);

            LeaveTestTournament();
        }


        // Helpers
        private int JoinTestTournament()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.JoinTournament(
                _leaderboardId,
                _tournamentCode,
                _rand.Next(1000),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            _didJoin = true;

            _bc.TournamentService.GetTournamentStatus(
                _leaderboardId,
                -1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            return (int)((Dictionary<string, object>)tr.m_response["data"])["versionId"];
        }

        private void LeaveTestTournament()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.LeaveTournament(
                _leaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            _didJoin = false;
        }

    }
}