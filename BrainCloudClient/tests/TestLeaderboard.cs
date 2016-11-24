using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestLeaderboard : TestFixtureBase
    {
        private readonly string _globalLeaderboardId = "testLeaderboard";
        private readonly string _socialLeaderboardId = "testSocialLeaderboard";
        private readonly string _dynamicLeaderboardId = "csTestDynamicLeaderboard";
        private readonly string _eventId = "tournamentRewardTest";

        private static Random _random = new Random();

        [Test]
        public void TestGetSocialLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetSocialLeaderboard(
                _globalLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetMultiSocialLeaderboard()
        {
            PostScoreToNonDynamicLeaderboard();
            PostScoreToDynamicLeaderboardHighValue();

            TestResult tr = new TestResult();

            List<string> lbIds = new List<string>();
            lbIds.Add(_globalLeaderboardId);
            lbIds.Add(_dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE.ToString());

            BrainCloudClient.Instance.LeaderboardService.GetMultiSocialLeaderboard(
                lbIds,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToLeaderboard()
        {
            PostScoreToNonDynamicLeaderboard();
        }

        public void PostScoreToNonDynamicLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToLeaderboard(
                _globalLeaderboardId,
                1000,
                Helpers.CreateJsonPair("testDataKey", 400),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetLeaderboardScore()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.ResetLeaderboardScore(
                _globalLeaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageHigh()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardPage(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageLow()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardPage(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageFail()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardPage(
                "thisDoesNotExistLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.INTERNAL_SERVER_ERROR, 40499);
        }

        [Test]
        public void TestGetCompletedLeaderboardTournament()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetCompletedLeaderboardTournament(
                _socialLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageByVersion()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardPageByVersion(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                0,
                10,
                true,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardVersions()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardVersions(
                _globalLeaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardView()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardView(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardViewByVersion()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardViewByVersion(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTriggerSocialLeaderboardTournamentReward()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.TriggerSocialLeaderboardTournamentReward(
                _socialLeaderboardId,
                _eventId,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardHighValue()
        {
            PostScoreToDynamicLeaderboardHighValue();
        }

        public void PostScoreToDynamicLeaderboardHighValue()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE.ToString() + "-" + _random.Next(),
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                System.DateTime.Now.AddDays(5),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardLowValue()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.LOW_VALUE.ToString() + "-" + _random.Next(),
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
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.CUMULATIVE.ToString() + "-" + _random.Next(),
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.CUMULATIVE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                System.DateTime.Now.AddDays(5),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardLastValue()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE.ToString() + "-" + _random.Next(),
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE,
                BrainCloudSocialLeaderboard.RotationType.DAILY,
                System.DateTime.Now.AddHours(15),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardNullRotationTime()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE.ToString() + "-" + _random.Next(),
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
        public void TestGetGroupSocialLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GroupService.CreateGroup("testLBGroup", "test", null, null, null, null, null, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            var id = (string)data["groupId"];

            BrainCloudClient.Instance.LeaderboardService.GetGroupSocialLeaderboard(
                _socialLeaderboardId,
                id,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.GroupService.DeleteGroup(id, -1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPlayersSocialLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.LeaderboardService.GetPlayersSocialLeaderboard(
                _socialLeaderboardId,
                new[] { GetUser(Users.UserA).ProfileId, GetUser(Users.UserB).ProfileId },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestListAllLeaderboards()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.LeaderboardService.ListLeaderboards( tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardEntryCount()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardEntryCount(_globalLeaderboardId, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardEntryCountByVersion()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.LeaderboardService.GetGlobalLeaderboardEntryCountByVersion(_globalLeaderboardId, 1, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}