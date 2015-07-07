using NUnit;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureNoAuth
    {
        private readonly string _defaultUserId = "abc";
        private readonly string _defaultUserPassword = "abc";
        private readonly string _defaultUserEmail = "abcTest.email.2015@abcmail.ca";

        [Test]
        public void TestAuthenticateUniversal()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                _defaultUserId,
                _defaultUserPassword,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        /*[Test]
        public void TestAuthenticateAnonymous()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateAnonymous(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }*/

        [Test]
        public void TestAuthenticateEmailPassword()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateEmailPassword(
                _defaultUserEmail,
                _defaultUserPassword,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetEmailPassword()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.AuthenticateEmailPassword(
                _defaultUserEmail,
                _defaultUserPassword,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            tr.Reset();

            BrainCloudClient.Get().AuthenticationService.ResetEmailPassword(
                _defaultUserEmail,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}