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
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToChildProfile(
                null,
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.PlayerStateService.DeleteUser(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToSingletonChildProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestSwitchToParentProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToSingletonChildProfile(
                ChildAppId,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.IdentityService.SwitchToParentProfile(
                ParentLevel,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDetachParent()
        {
            GoToChildProfile();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void AttachParentWithIdentity()
        {
            GoToChildProfile();

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachParent(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestUser user = GetUser(Users.UserA);
            _bc.IdentityService.AttachParentWithIdentity(
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
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetChildProfiles(
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestAttachEmailIdentity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachEmailIdentity(
                "id_" + GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetIdentites()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetExpiredIdentites()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetExpiredIdentities(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestRefreshIdentity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.RefreshIdentity(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                AuthenticationType.Universal,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, 40464);
        }

        [Test]
        public void TestAttachPeerProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachPeerProfile(
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

            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachPeer(
                PeerName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetPeerProfiles()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.GetPeerProfiles(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
        
        [Test]
        public void TestAttachNonLoginUniversalId()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachNonLoginUniversalId("braincloudtest@gmail.com", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(202, ReasonCodes.DUPLICATE_IDENTITY_TYPE);
        }

        [Test]
        public void TestUpdateUniversalIdLogin()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.UpdateUniversalIdLogin("braincloudtest@gmail.com", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, ReasonCodes.NEW_CREDENTIAL_IN_USE);
        }
    }
}