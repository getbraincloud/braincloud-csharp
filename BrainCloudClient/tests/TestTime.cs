using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTime : TestFixtureBase
    {
        [Test]
        public void TestReadServerTime()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TimeService.ReadServerTime(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}