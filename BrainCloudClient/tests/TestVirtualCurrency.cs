using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestVirtualCurrency : TestFixtureBase
    {
        [Test]
        public void TestGetCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetCurrency("_invalid_id_", tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetParentCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetParentCurrency("_invalid_id_", "_invalid_level_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.MISSING_PLAYER_PARENT);
        }

        [Test]
        public void TestGetPeerCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetPeerCurrency("_invalid_id_", "_invalid_peer_code_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.PROFILE_PEER_NOT_FOUND);
        }
    }
}
