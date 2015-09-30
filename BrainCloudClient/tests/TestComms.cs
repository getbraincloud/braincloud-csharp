using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestComms : TestFixtureNoAuth
    {     
        [Test]
        public void TestBadUrl()
        {
            BrainCloudClient.Get().Initialize(_serverUrl + "unitTestFail", _secret, _appId, _version);
            BrainCloudClient.Get ().EnableLogging(true);

            TestResult tr = new TestResult();
            tr.SetTimeToWaitSecs(120);
            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal("abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT);
        }
    }
}