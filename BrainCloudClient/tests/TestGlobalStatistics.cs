using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalStatistics : TestFixtureBase
    {
        [Test]
        public void TestIncrementGlobalStats()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> stats = new Dictionary<string, object> { { "TestStat", "RESET" } };

            BrainCloudClient.Instance.GlobalStatisticsService.IncrementGlobalStats(
                JsonWriter.Serialize(stats),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadAllGlobalStats()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalStatisticsService.ReadAllGlobalStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadGlobalStatsSubset()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalStatisticsService.ReadGlobalStatsSubset(
                new string[] { "TestStat" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadGlobalStatsForCategory()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Instance.GlobalStatisticsService.ReadGlobalStatsForCategory(
                "Test",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestProcessStats()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> stats = new Dictionary<string, object> { { "TestStat", "RESET" } };

            BrainCloudClient.Instance.GlobalStatisticsService.ProcessStatistics(
                stats,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}