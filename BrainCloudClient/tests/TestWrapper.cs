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
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            string profileId = _bc.Client.AuthenticationService.ProfileId;
            string anonId = _bc.Client.AuthenticationService.AnonymousId;

            Assert.AreEqual(profileId, _bc.GetStoredProfileId());
            Assert.AreEqual(anonId, _bc.GetStoredAnonymousId());

            _bc.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Assert.AreEqual(profileId, _bc.GetStoredProfileId());
            Assert.AreEqual(anonId, _bc.GetStoredAnonymousId());
        }

        [Test]
        public void TestAuthenticateUniversal()
        {
            _bc.Client.AuthenticationService.ClearSavedProfileID();
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);

            _bc.AuthenticateUniversal(
                GetUser(Users.UserA).Id + "W",
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReconnect()
        {
            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Reconnect(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}