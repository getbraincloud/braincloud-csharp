using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestClient : TestFixtureNoAuth
    {
        private bool _killSwitchEngaged;

        //[Test]
        public void TestKillSwitch()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            for (int i = 0; i < 3; ++i)
            {
                BrainCloudClient.Instance.EntityService.UpdateEntity(
                "FAIL",
                "FAIL",
                Helpers.CreateJsonPair("test", 1),
                Helpers.CreateJsonPair("test", 1),
                -1,
                tr.ApiSuccess, tr.ApiError);

                tr.RunExpectFail(404, ReasonCodes.UPDATE_FAILED);
            }

            BrainCloudClient.Instance.TimeService.ReadServerTime(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            while (!_killSwitchEngaged)
            {
                BrainCloudClient.Instance.EntityService.UpdateEntity(
                "FAIL",
                "FAIL",
                Helpers.CreateJsonPair("test", 1),
                Helpers.CreateJsonPair("test", 1),
                -1,
                tr.ApiSuccess, (int statusCode, int reasonCode, string message, object cbObj) =>
                {
                    OnErrorCheck(statusCode, reasonCode);
                    tr.ApiError(statusCode, reasonCode, message, cbObj);
                });

                tr.RunExpectFail(404, ReasonCodes.UPDATE_FAILED);
            }

            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(900, ReasonCodes.UPDATE_FAILED);
        }

        void OnErrorCheck(int statusCode, int reasonCode)
        {
            if (reasonCode == ReasonCodes.CLIENT_DISABLED)
                _killSwitchEngaged = true;
        }
    }
}