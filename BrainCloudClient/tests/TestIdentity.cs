using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestIdentity : TestFixtureBase
    {
        [Test]
        public void TestSwitchToChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.SwitchToChildProfile(
                null,
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.PlayerStateService.DeleteUser(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToSingletonChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToParentProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Instance.IdentityService.SwitchToParentProfile(
                ParentLevel,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDetachParent()
        {
            GoToChildProfile();

            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void AttachParentWithIdentity()
        {
            GoToChildProfile();

            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestUser user = GetUser(Users.UserA);
            BrainCloudClient.Instance.IdentityService.AttachParentWithIdentity(
                user.Id,
                user.Password,
                AuthenticationType.Universal,
                null,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetChildProfiles()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.GetChildProfiles(
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachEmailIdentity()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.AttachEmailIdentity(
                "id_" + GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetIdentites()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.GetIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetExpiredIdentites()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.GetExpiredIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRefreshIdentity()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.RefreshIdentity(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                AuthenticationType.Universal,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, 40464);
        }

        [Test]
        public void TestAttachPeerProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.AttachPeerProfile(
                PeerName,
                GetUser(Users.UserA).Id + "_peer",
                GetUser(Users.UserA).Password,
                AuthenticationType.Universal,
                null,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DetachPeer();
        }

        [Test]
        public void TestDetachPeer()
        {
            AttachPeer(Users.UserA, AuthenticationType.Universal);

            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.DetachPeer(
                PeerName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPeerProfiles()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.GetPeerProfiles(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}