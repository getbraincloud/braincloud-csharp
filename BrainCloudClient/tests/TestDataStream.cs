using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestDataStream : TestFixtureBase
    {
        [Test]
        public void TestCustomPageEvent()
        {
            TestResult tr = new TestResult(_bc);

            _bc.DataStreamService.CustomPageEvent(
                "testPageEvent",
                Helpers.CreateJsonPair("test1", 1332),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCustomScreenEvent()
        {
            TestResult tr = new TestResult(_bc);

            _bc.DataStreamService.CustomScreenEvent(
                "testScreenEvent",
                Helpers.CreateJsonPair("test2", 132),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCustomTrackEvent()
        {
            TestResult tr = new TestResult(_bc);

            _bc.DataStreamService.CustomTrackEvent(
                "testTrackEvent",
                Helpers.CreateJsonPair("test3", 12),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSubmitCrashReport()
        {
            TestResult tr = new TestResult(_bc);

            _bc.DataStreamService.SubmitCrashReport(
                "testCrashReport", "test", "{\"test\":\"7\"}", "test", "test", "test", "test", true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}
