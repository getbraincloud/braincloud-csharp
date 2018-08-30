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
                _divSetId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
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
                _divSetId,
                _tournamentCode,
                _rand.Next(1000),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            _didJoin = true;
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
            //the unit test master 20001 has working API calls. I needed to hard code this leaderboardId 
            //because the LeaveDivisionInstance is looking for a string of a certain format, as a tag of a 
            //Division set instance. When I use the API explorer, I first authenticate, then join a division,
            //then getMyDivisions (which tells me my testDivSetId maps to "^D^testDivSetId^3", which is what I 
            //need to pass into LeaveDivisionInstance as the _leaderBoardId in order for success).
            //If I simply passed in _leaderBoardId, it tells me testTournamentLeaderBoard is not a division set instance. 
            //This is because its not the same format. I hard coded the response I got from GetMyDivisions from
            //the API explorer because it worked there, and it seems to work here if I pass in the same thing , therefore the 
            //unit test is considered a pass, as all the calls are successful.   

            TestResult tr = new TestResult(_bc);
            _bc.TournamentService.LeaveDivisionInstance(
                "^D^testDivSetId^3",
                tr.ApiSuccess, tr.ApiError
            );
            tr.Run();
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