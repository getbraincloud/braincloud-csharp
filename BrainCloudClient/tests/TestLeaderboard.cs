using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using System.Diagnostics;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestLeaderboard : TestFixtureBase
    {
        private readonly string _globalLeaderboardId = "testLeaderboard";
        private readonly string _socialLeaderboardId = "testSocialLeaderboard";
        private readonly string _dynamicLeaderboardId = "csTestDynamicLeaderboard";

        private readonly string _groupLeaderboardId = "groupLeaderboardConfig";
      
        private static Random _random = new Random();

        [Test]
        public void TestGetSocialLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetSocialLeaderboard(
                _globalLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSocialLeaderboardIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetSocialLeaderboardIfExists(
                _globalLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetSocialLeaderboardIfExists(
                "nonExistentLeaderboard",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSocialLeaderboardByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetSocialLeaderboardByVersion(
                _globalLeaderboardId,
                true,
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSocialLeaderboardByVersionIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetSocialLeaderboardByVersionIfExists(
                _globalLeaderboardId,
                true,
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetSocialLeaderboardByVersionIfExists(
                "nonExistentLeaderboard",
                true,
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetMultiSocialLeaderboard()
        {
            PostScoreToGlobalLeaderboard();
            PostScoreToDynamicLeaderboardHighValue();

            TestResult tr = new TestResult(_bc);

            List<string> lbIds = new List<string>();
            lbIds.Add(_globalLeaderboardId);
            lbIds.Add(_dynamicLeaderboardId);

            _bc.LeaderboardService.GetMultiSocialLeaderboard(
                lbIds,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToLeaderboard()
        {
            PostScoreToGlobalLeaderboard();
        }

        [Test]
        public void TestRemovePlayerScore()
        {
            PostScoreToGlobalLeaderboard();

            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.RemovePlayerScore(
                _globalLeaderboardId,
                -1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        public void PostScoreToGlobalLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToLeaderboard(
                _globalLeaderboardId,
                1000,
                Helpers.CreateJsonPair("testDataKey", 400),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageHigh()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPage(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageLow()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPage(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageFail()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPage(
                "thisDoesNotExistLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.INTERNAL_SERVER_ERROR, 40499);
        }

        [Test]
        public void TestGetGlobalLeaderboardPageIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPageIfExists(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetGlobalLeaderboardPageIfExists(
                "nonExistentLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPageByVersion(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageByVersionIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardPageByVersionIfExists(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetGlobalLeaderboardPageByVersionIfExists(
                "nonExistentLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardVersions()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardVersions(
                _globalLeaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardView()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardView(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardViewIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardViewIfExists(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetGlobalLeaderboardViewIfExists(
                "nonExistentLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardViewByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardViewByVersion(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardViewByVersionIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetGlobalLeaderboardViewByVersionIfExists(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetGlobalLeaderboardViewByVersionIfExists(
                "nonExistentLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardHighValue()
        {
            PostScoreToDynamicLeaderboardHighValue();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardUsingConfig()
        {
            TestResult tr = new TestResult(_bc);

            int score = 10;
            string scoreData = "{\"nickname\": \"Tarnished\"}";
            var configJson = new Dictionary<string, object>();
            configJson["leaderboardType"] = "HIGH_VALUE";
            configJson["rotationType"] = "DAYS";
            configJson["numDaysToRotate"] = "4";
            configJson["resetAt"] = (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddDays(5)));
            configJson["retainedCount"] = 2;

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUsingConfig(_dynamicLeaderboardId, score, scoreData, JsonWriter.Serialize(configJson), tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        public void PostScoreToDynamicLeaderboardHighValue()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddDays(5))),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        public void PostScoreToDynamicGroupLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicGroupLeaderboardUTC(
                _dynamicLeaderboardId,
                _groupLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddDays(5))),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        

        [Test]
        public void TestPostScoreToDynamicLeaderboardDays()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardDaysUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LOW_VALUE,
                null,
                5,
                3,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardLowValue()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LOW_VALUE,
                BrainCloudSocialLeaderboard.RotationType.NEVER,
                null,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardCumulative()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.CUMULATIVE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddDays(5))),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardLastValue()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE,
                BrainCloudSocialLeaderboard.RotationType.DAILY,
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddHours(15))),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardNullRotationTime()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.PostScoreToDynamicLeaderboardUTC(
                _dynamicLeaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE,
                BrainCloudSocialLeaderboard.RotationType.NEVER,
                null,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void TestPostScoreToDynamicGroupLeaderboardDaysUTC()
        {
            TestResult tr = new TestResult(_bc);
            GroupACL groupAcl = new GroupACL();
            _bc.GroupService.CreateGroup(
                "a-group-id",
                "test",
                false,
                groupAcl,
                "",
                "",
                "",
                tr.ApiSuccess,
                tr.ApiError);
            tr.Run();

            var response = tr.m_response;
            var objData = response["data"] as Dictionary<string, object>;
            var leaderboardId = objData["groupId"] as string;
            
            
            _bc.LeaderboardService.PostScoreToDynamicGroupLeaderboardUTC(
                _dynamicLeaderboardId + "_" + BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE.ToString() + "_" + _random.Next(),
                leaderboardId,
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(TimeUtil.LocalTimeToUTCTime(System.DateTime.Now.AddDays(5))),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            
            _bc.GroupService.DeleteGroup(leaderboardId,-1,tr.ApiSuccess,tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGroupSocialLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testLBGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.GetGroupSocialLeaderboard(
                _socialLeaderboardId,
                id,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGroupSocialLeaderboardByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testLBGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.GetGroupSocialLeaderboardByVersion(
                _socialLeaderboardId,
                id,
                0,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPlayersSocialLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetPlayersSocialLeaderboard(
                _socialLeaderboardId,
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPlayersSocialLeaderboardIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetPlayersSocialLeaderboardIfExists(
                _socialLeaderboardId,
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetPlayersSocialLeaderboardIfExists(
                "nonExistentLeaderboard",
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPlayersSocialLeaderboardByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetPlayersSocialLeaderboardByVersion(
                _socialLeaderboardId,
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPlayersSocialLeaderboardByVersionIfExists()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LeaderboardService.GetPlayersSocialLeaderboardByVersionIfExists(
                _socialLeaderboardId,
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.LeaderboardService.GetPlayersSocialLeaderboardByVersionIfExists(
                "nonExistentLeaderboard",
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                0,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestListAllLeaderboards()
        {
            TestResult tr = new TestResult(_bc);
            _bc.LeaderboardService.ListLeaderboards( tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardEntryCount()
        {
            TestResult tr = new TestResult(_bc);
            _bc.LeaderboardService.GetGlobalLeaderboardEntryCount(_globalLeaderboardId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardEntryCountByVersion()
        {
            TestResult tr = new TestResult(_bc);
            _bc.LeaderboardService.GetGlobalLeaderboardEntryCountByVersion(_globalLeaderboardId, 1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPlayerScore()
        {
            TestResult tr = new TestResult(_bc);
            _bc.LeaderboardService.GetPlayerScore(_globalLeaderboardId, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPlayerScores()
        {
            TestResult tr = new TestResult(_bc);
            _bc.LeaderboardService.GetPlayerScores(_globalLeaderboardId, -1, 4, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPlayerScoresFromLeaderboards()
        {
            PostScoreToGlobalLeaderboard();
            PostScoreToDynamicLeaderboardHighValue();

            TestResult tr = new TestResult(_bc);

            List<string> lbIds = new List<string>();
            lbIds.Add(_globalLeaderboardId);
            lbIds.Add(_dynamicLeaderboardId);

            _bc.LeaderboardService.GetPlayerScoresFromLeaderboards(
                lbIds,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToGroupLeaderboard()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.PostScoreToGroupLeaderboard(
                _groupLeaderboardId,
                id,
                0,
                Helpers.CreateJsonPair("testy", 400),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRemoveGroupScore()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.PostScoreToGroupLeaderboard(
                _groupLeaderboardId,
                id,
                100,
                Helpers.CreateJsonPair("testy", 400),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.LeaderboardService.RemoveGroupScore(
                _groupLeaderboardId,
                id,
                -1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGroupLeaderboardView()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.GetGroupLeaderboardView(
                _groupLeaderboardId,
                id,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGroupLeaderboardViewByVersion()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup("testLBGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            _bc.LeaderboardService.GetGroupLeaderboardViewByVersion(
                _groupLeaderboardId,
                id,
                1,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDeleteDynamicLeaderboards()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.RunScript
            (
                "CleanupLeaderboards",
                Helpers.CreateJsonPair("leaderboardId", _dynamicLeaderboardId),
                tr.ApiSuccess,
                tr.ApiError
            );

            tr.Run();
        }
    }
}
