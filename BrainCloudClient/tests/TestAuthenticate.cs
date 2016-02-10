using NUnit;
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

            BrainCloudClient.Instance.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            tr.Reset();

            BrainCloudClient.Instance.AuthenticationService.ResetEmailPassword(
                GetUser(Users.UserA).Email,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}