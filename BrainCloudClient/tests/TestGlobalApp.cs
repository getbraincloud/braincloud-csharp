using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalApp : TestFixtureBase
    {
        [Test]
        public void TestBrainCloudGlobalApp()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalAppService.ReadProperties(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}