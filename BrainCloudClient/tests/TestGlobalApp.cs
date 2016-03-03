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
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalAppService.ReadProperties(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}