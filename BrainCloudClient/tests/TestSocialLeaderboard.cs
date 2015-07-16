using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestSocialLeaderboard : TestFixtureBase
    {
        private readonly string _globalLeaderboardId = "testLeaderboard";
        private readonly string _socialLeaderboardId = "testSocialLeaderboard";
        private readonly string _dynamicLeaderboardId = "testDynamicLeaderboard";
        private readonly string _eventId = "tournamentRewardTest";

        [Test]
        public void TestGetLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetLeaderboard(
                _globalLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }


        [Test]
        public void TestPostScoreToLeaderboard()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.PostScoreToLeaderboard(
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

            BrainCloudClient.Get().SocialLeaderboardService.ResetLeaderboardScore(
                _globalLeaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageHigh()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardPage(
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

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardPage(
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

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardPage(
                "thisDoesNotExistLeaderboard",
                BrainCloudSocialLeaderboard.SortOrder.LOW_TO_HIGH,
                0,
                10,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.INTERNAL_SERVER_ERROR, 0);
        }

        [Test]
        public void TestGetCompletedLeaderboardTournament()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetCompletedLeaderboardTournament(
                _socialLeaderboardId,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardPageByVersion()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardPageByVersion(
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

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardVersions(
                _globalLeaderboardId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardView()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardView(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetGlobalLeaderboardViewByVersion()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.GetGlobalLeaderboardViewByVersion(
                _globalLeaderboardId,
                BrainCloudSocialLeaderboard.SortOrder.HIGH_TO_LOW,
                5,
                5,
                true,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTriggerSocialLeaderboardTournamentReward()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.TriggerSocialLeaderboardTournamentReward(
                _socialLeaderboardId,
                _eventId,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardHighValue()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.HIGH_VALUE.ToString(),
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

            BrainCloudClient.Get().SocialLeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.LOW_VALUE.ToString(),
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LOW_VALUE,
                BrainCloudSocialLeaderboard.RotationType.WEEKLY,
                System.DateTime.Now.AddDays(5),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestPostScoreToDynamicLeaderboardCumulative()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().SocialLeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.CUMULATIVE.ToString(),
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

            BrainCloudClient.Get().SocialLeaderboardService.PostScoreToDynamicLeaderboard(
                _dynamicLeaderboardId + "-" + BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE.ToString(),
                100,
                Helpers.CreateJsonPair("testDataKey", 400),
                BrainCloudSocialLeaderboard.SocialLeaderboardType.LAST_VALUE,
                BrainCloudSocialLeaderboard.RotationType.DAILY,
                System.DateTime.Now.AddHours(15),
                5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}