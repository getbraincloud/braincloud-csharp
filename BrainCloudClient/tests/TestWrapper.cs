using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestWrapper : TestFixtureNoAuth
    {
        [Test]
        public void TestAuthenticateAnonymous()
        {
            BrainCloudWrapper.Instance.ResetStoredAnonymousId();
            BrainCloudWrapper.Instance.ResetStoredProfileId();

            TestResult tr = new TestResult();
            BrainCloudWrapper.Instance.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            string profileId = BrainCloudClient.Instance.AuthenticationService.ProfileId;
            string anonId = BrainCloudClient.Instance.AuthenticationService.AnonymousId;

            Assert.AreEqual(profileId, BrainCloudWrapper.Instance.GetStoredProfileId());
            Assert.AreEqual(anonId, BrainCloudWrapper.Instance.GetStoredAnonymousId());

            BrainCloudWrapper.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudWrapper.Instance.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Assert.AreEqual(profileId, BrainCloudWrapper.Instance.GetStoredProfileId());
            Assert.AreEqual(anonId, BrainCloudWrapper.Instance.GetStoredAnonymousId());
        }

        [Test]
        public void TestAuthenticateUniversal()
        {
            BrainCloudWrapper.Client.AuthenticationService.ClearSavedProfileID();
            BrainCloudWrapper.Instance.ResetStoredAnonymousId();
            BrainCloudWrapper.Instance.ResetStoredProfileId();

            TestResult tr = new TestResult();

            BrainCloudWrapper.Instance.AuthenticateUniversal(
                GetUser(Users.UserA).Id + "W",
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReconnect()
        {
            TestResult tr = new TestResult();
            BrainCloudWrapper.Instance.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudWrapper.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudWrapper.Instance.Reconnect(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudWrapper.Client.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}