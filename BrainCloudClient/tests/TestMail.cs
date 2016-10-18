using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestMail : TestFixtureBase
    {
        [Test]
        public void TestSendBasicEmail()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MailService.SendBasicEmail(
                GetUser(Users.UserA).ProfileId,
                "Test Subject - TestSendBasicEmail",
                "Test body content message.",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSendAdvancedEmailSendGrid()
        {
            TestResult tr = new TestResult();

            var data = new Dictionary<string, object>();
            data["subject"] = "Test Subject - TestSendAdvancedEmailSendGrid";
            data["body"] = "Test body";
            //data["substitutions"] = new Dictionary<string, object>() { { "*replace*", "test" } };
            data["categories"] = new string[] { "unit-test" };

            BrainCloudClient.Instance.MailService.SendAdvancedEmail(
                GetUser(Users.UserB).ProfileId,
                JsonWriter.Serialize(data),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}