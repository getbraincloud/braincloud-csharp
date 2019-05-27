using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlayerStatistics : TestFixtureBase
    {

        #region User Statistics

        [Test]
        public void TestReadAllPlayerStats()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.ReadAllUserStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerStatsSubset()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.ReadUserStatsSubset(
                new string[] { "currency", "highestScore" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerStatsForCategory()
        {
            TestResult tr = new TestResult(_bc);
            
            _bc.PlayerStatisticsService.ReadUserStatsForCategory(
                "Test",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestIncrementPlayerStats()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> stats = new Dictionary<string, object> { { "highestScore", "RESET" } };

            _bc.PlayerStatisticsService.IncrementUserStats(
                JsonWriter.Serialize(stats),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetAllPlayerStats()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.ResetAllUserStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestProcessStats()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> stats = new Dictionary<string, object> { { "highestScore", "RESET" } };

            _bc.PlayerStatisticsService.ProcessStatistics(
                stats,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

        #region XP System

        [Test]
        public void TestIncrementExperiencePoints()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.IncrementExperiencePoints(
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetNextExperienceLevel()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.GetNextExperienceLevel(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetExperiencePoints()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsService.SetExperiencePoints(
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

    }
}