using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestIdentity : TestFixtureBase
    {
        private readonly string _childAppId = "10326";
        private readonly string _parentLevel = "Master";

        [Test]
        public void TestSwitchToChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToChildProfile(
                null,
                _childAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToParentProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToChildProfile(
                null,
                _childAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().IdentityService.SwitchToParentProfile(
                _parentLevel,
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