using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestIdentity : TestFixtureBase
    {
        [Test]
        public void TestSwitchToChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToChildProfile(
                null,
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToSingletonChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToParentProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().IdentityService.SwitchToParentProfile(
                ParentLevel,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetChildProfiles()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.GetChildProfiles(
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}