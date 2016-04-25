using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureNoAuth
    {
        [Test]
        public void TestAuthenticateUniversal()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        /*[Test]
        public void TestAuthenticateAnonymous()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateAnonymous(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }*/

        [Test]
        public void TestAuthenticateEmailPassword()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetEmailPassword()
        {
            TestResult tr = new TestResult();

            string email = "braincloudunittest@gmail.com";

            BrainCloudClient.Instance.AuthenticationService.ResetEmailPassword(
                email,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestAuthenticateWithHeartbeat()
        {
            TestResult tr = new TestResult();

            // Insert heartbeat as first packet. This would normally cause the
            // server to reject the second authenticate packet but with the
            // new comms change, this should result in the heartbeat being
            // removed from the message bundle.
            BrainCloudClient.Get().SendHeartbeat();

            BrainCloudClient.Get().AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}