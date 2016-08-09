using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureNoAuth
    {
        [TearDown]
        public void Cleanup()
        {
            BrainCloudClient.Instance.OverrideCountryCode(null);
            BrainCloudClient.Instance.OverrideLanguageCode(null);
        }

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

        [Test]
        public void TestSetCountryCode()
        {
            TestResult tr = new TestResult();

            string countryCode = "ru";

            BrainCloudClient.Instance.OverrideCountryCode(countryCode);

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string code = data["countryCode"] as string;
            Assert.AreEqual(countryCode, code);
        }

        [Test]
        public void TestSetLanguageCode()
        {
            TestResult tr = new TestResult();

            string languageCode = "ru";

            BrainCloudClient.Instance.OverrideLanguageCode(languageCode);

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string code = data["languageCode"] as string;
            Assert.AreEqual(languageCode, code);
        }
    }
}