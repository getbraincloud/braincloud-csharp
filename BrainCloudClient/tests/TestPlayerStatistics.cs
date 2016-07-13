using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlayerStatistics : TestFixtureBase
    {

        #region User Statistics

        [Test]
        public void TestReadAllPlayerStats()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.ReadAllPlayerStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerStatsSubset()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.ReadPlayerStatsSubset(
                new string[] { "currency", "highestScore" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerStatsForCategory()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Instance.PlayerStatisticsService.ReadPlayerStatsForCategory(
                "Test",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestIncrementPlayerStats()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> stats = new Dictionary<string, object> { { "highestScore", "RESET" } };

            BrainCloudClient.Instance.PlayerStatisticsService.IncrementPlayerStats(
                JsonWriter.Serialize(stats),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetAllPlayerStats()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.ResetAllPlayerStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestProcessStats()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> stats = new Dictionary<string, object> { { "highestScore", "RESET" } };

            BrainCloudClient.Instance.PlayerStatisticsService.ProcessStatistics(
                stats,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

        #region XP System

        [Test]
        public void TestIncrementExperiencePoints()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.IncrementExperiencePoints(
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetNextExperienceLevel()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.GetNextExperienceLevel(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetExperiencePoints()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PlayerStatisticsService.SetExperiencePoints(
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

    }
}