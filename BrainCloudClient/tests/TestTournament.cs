// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTournament : TestFixtureBase
    {
        private readonly string _divSetId = "testDivSetId";
        private readonly string _tournamentCode = "testTournament";
        private readonly string _leaderboardId = "testTournamentLeaderboard";
        private string _divisionInstanceId;
        private readonly int _score = 10;
        private readonly int _beforeAndAfterCount = 10;
        private readonly int _initialScore = 0;
        private readonly string _groupType = "csharpTest";
        private readonly string _groupName = "csharpTest";
        private readonly string _divisionGroupSetId = "bronzeGroup";
        private readonly string _groupTournamentId = "testGroupTournament";
        private readonly string _groupLeaderboardId = "groupTournament";
        private string _groupId;
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
        public void GetDivisionInfoExpectFail()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.GetDivisionInfo(
                "Invalid_Id",
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, ReasonCodes.DIVISION_SET_DOESNOT_EXIST);
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
        public void JoinDivisionExpectFail()
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
        public void JoinDivision()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.JoinDivision(
                _divSetId,
                _tournamentCode,
                _rand.Next(1000),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            Dictionary<string, object> data = tr.m_response["data"] as Dictionary<string, object>;
            _divisionInstanceId = data["leaderboardId"] as string;
            
        }


        [Test]
        public void JoinTournament()
        {
            JoinTestTournament();
            LeaveTestTournament();
        }

        [Test]
        public void LeaveDivisionInstanceExpectFail()
        {
            TestResult tr = new TestResult(_bc);
            _bc.TournamentService.LeaveDivisionInstance(
                "Invalid_id",
                tr.ApiSuccess, tr.ApiError
            );
            tr.RunExpectFail(500, ReasonCodes.NO_LEADERBOARD_FOUND);
        }
        
        [Test]
        public void LeaveDivisionInstance()
        {
            TestResult tr = new TestResult(_bc);
            _bc.TournamentService.LeaveDivisionInstance(
                _divisionInstanceId,
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
        public void PostTournamentScoreUTC()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.PostTournamentScoreUTC(
                _leaderboardId,
                _rand.Next(1000),
                null,
                (UInt64)Util.DateTimeToUnixTimestamp(DateTime.UtcNow),
                //(UInt64)((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow) -
                  // new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            LeaveTestTournament();
        }

        [Test]
        public void PostTournamentScoreWithResultsUTC()
        {
            JoinTestTournament();

            TestResult tr = new TestResult(_bc);

            _bc.TournamentService.PostTournamentScoreWithResultsUTC(
                _leaderboardId,
                _rand.Next(1000),
                null,
                (UInt64)((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                10,
                10,
                0,
                tr.ApiSuccess, tr.ApiError);

                Console.WriteLine("//////////////////////////////////////////"+DateTime.Now.Ticks+"//////////////////////////////////////////");
                Console.WriteLine("//////////////////////////////////////////"+DateTime.Now+"//////////////////////////////////////////");
                //Util.DateTimeToBcTimestamp(DateTime.Now)

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
        public void ViewRewardExpectFail()
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
        
        [Test]
        public void GetGroupDivisionInfo()
        {
            CreateGroup();
            TestResult tr = new TestResult(_bc);
            
            _bc.TournamentService.GetGroupDivisionInfo(_divisionGroupSetId, _groupId, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }
        
        [Test]
        public void GetGroupDivisions()
        {
            CreateGroup();
            TestResult tr = new TestResult(_bc);
            
            _bc.TournamentService.GetGroupDivisions(_groupId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            DeleteGroup();
        }
        
        [Test]
        public void GetGroupTournamentStatus()
        {
            CreateGroup();
            TestResult tr = new TestResult(_bc);
            
            _bc.TournamentService.GetGroupTournamentStatus(_groupLeaderboardId, _groupId, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            DeleteGroup();
        }
        
        [Test]
        public void GroupPostScoreTournamentTest()
        {
            CreateGroup();
            TestResult tr = new TestResult(_bc);
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            //First join division and tournament using group id
            _bc.TournamentService.JoinGroupDivision(
                _divisionGroupSetId,
                _groupTournamentId,
                _groupId,
                _initialScore,
                tr.ApiSuccess, 
                tr.ApiError
            );
            tr.Run();
            
            //Get group division leaderboard ID
            data = tr.m_response[OperationParam.Data] as Dictionary<string, object>;
            string divisonInstanceId = data[OperationParam.LeaderboardId] as string;
            
            _bc.TournamentService.JoinGroupTournament(
                _groupLeaderboardId, 
                _groupTournamentId, 
                _groupId, 
                _initialScore, 
                tr.ApiSuccess, 
                tr.ApiError);
            tr.Run();
            
            _bc.TournamentService.PostGroupTournamentScoreWithResults(
                _groupLeaderboardId,
                _groupId,
                _score,
                "{}",
                (UInt64)((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                _beforeAndAfterCount,
                _beforeAndAfterCount,
                _initialScore,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            _bc.TournamentService.PostGroupTournamentScore(
                _groupLeaderboardId,
                _groupId,
                _score,
                "{}",
                (UInt64)((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
                tr.ApiSuccess,
                tr.ApiError);
            tr.Run();
            
            _bc.TournamentService.LeaveGroupDivisionInstance(divisonInstanceId, _groupId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            _bc.TournamentService.LeaveGroupTournament(_groupLeaderboardId, _groupId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            DeleteGroup();
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
        
        private void CreateGroup()
        {
            TestResult tr = new TestResult(_bc);
            _bc.GroupService.CreateGroup(
                _groupName, 
                _groupType, 
                true, 
                new GroupACL(GroupACL.Access.ReadWrite, GroupACL.Access.ReadWrite),
                "{}", 
                Helpers.CreateJsonPair("testInc", 123),
                Helpers.CreateJsonPair("test", "test"),
                tr.ApiSuccess, 
                tr.ApiError
            );
            tr.Run();

            Dictionary<string, object> data = tr.m_response["data"] as Dictionary<string, object>;
            
            _groupId = data["groupId"] as string;
        }
        
        private void DeleteGroup()
        {
            TestResult tr = new TestResult(_bc);
            _bc.GroupService.DeleteGroup(
                _groupId,
                -1,
                tr.ApiSuccess, 
                tr.ApiError
            );
            tr.Run();
        }
    }
}
