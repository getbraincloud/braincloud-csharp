using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestItemCatalog : TestFixtureBase
    {
        [Test]
        public void TestGetCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetCurrency(null, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetParentCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetParentCurrency(null, "_invalid_level_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.MISSING_PLAYER_PARENT);
        }

        [Test]
        public void TestGetPeerCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.GetPeerCurrency(null, "_invalid_peer_code_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.PROFILE_PEER_NOT_FOUND);
        }

        [Test]
        public void TestResetCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.VirtualCurrencyService.ResetCurrency(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
