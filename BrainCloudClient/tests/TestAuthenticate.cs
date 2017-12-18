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
            _bc.Client.OverrideCountryCode(null);
            _bc.Client.OverrideLanguageCode(null);
        }

        [Test]
        public void TestAuthenticateUniversal()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        /*[Test]
        public void TestAuthenticateAnonymous()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AuthenticationService.AuthenticateAnonymous(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }*/

        [Test]
        public void TestAuthenticateEmailPassword()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetEmailPassword()
        {
            TestResult tr = new TestResult(_bc);

            string email = "braincloudunittest@gmail.com";

            _bc.Client.AuthenticationService.ResetEmailPassword(
                email,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestAuthenticateWithHeartbeat()
        {
            TestResult tr = new TestResult(_bc);

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
            TestResult tr = new TestResult(_bc);

            string countryCode = "RU";

            _bc.Client.OverrideCountryCode(countryCode);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
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
            TestResult tr = new TestResult(_bc);

            string languageCode = "ru";

            _bc.Client.OverrideLanguageCode(languageCode);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string code = data["languageCode"] as string;
            Assert.AreEqual(languageCode, code);
        }

        [Test]
        public void TestAuthFirst()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.ReadUserState(tr.ApiSuccess, tr.ApiError);

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            tr.Run();
        }
    }
}