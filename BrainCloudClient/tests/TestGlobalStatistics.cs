using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalStatistics : TestFixtureBase
    {
        [Test]
        public void TestIncrementGlobalStats()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> stats = new Dictionary<string, object> { { "TestStat", "RESET" } };

            _bc.GlobalStatisticsService.IncrementGlobalStats(
                JsonWriter.Serialize(stats),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadAllGlobalStats()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalStatisticsService.ReadAllGlobalStats(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadGlobalStatsSubset()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalStatisticsService.ReadGlobalStatsSubset(
                new string[] { "TestStat" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadGlobalStatsForCategory()
        {
            TestResult tr = new TestResult(_bc);
            
            _bc.GlobalStatisticsService.ReadGlobalStatsForCategory(
                "Test",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestProcessStats()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> stats = new Dictionary<string, object> { { "TestStat", "RESET" } };

            _bc.GlobalStatisticsService.ProcessStatistics(
                stats,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}