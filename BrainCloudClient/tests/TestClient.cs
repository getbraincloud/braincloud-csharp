using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestClient : TestFixtureNoAuth
    {
        private bool _killSwitchEngaged;

        [Test]
        public void TestKillSwitch()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            for (int i = 0; i < 3; ++i)
            {
                _bc.EntityService.UpdateEntity(
                "FAIL",
                "FAIL",
                Helpers.CreateJsonPair("test", 1),
                Helpers.CreateJsonPair("test", 1),
                -1,
                tr.ApiSuccess, tr.ApiError);

                tr.RunExpectFail(404, ReasonCodes.UPDATE_FAILED);
            }

            _bc.TimeService.ReadServerTime(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            int failureIndex = 0;
            while (!_killSwitchEngaged)
            {
                _bc.EntityService.UpdateEntity(
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

                if(_killSwitchEngaged)
                {
                    break;
                }

                tr.RunExpectFail();

                failureIndex++;

                Assert.That(failureIndex < 13, "UpdateEntity called too many times without killswitch turning on/");
            }

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(900, ReasonCodes.CLIENT_DISABLED);
        }

        void OnErrorCheck(int statusCode, int reasonCode)
        {
            if (reasonCode == ReasonCodes.CLIENT_DISABLED)
                _killSwitchEngaged = true;
        }
    }
}