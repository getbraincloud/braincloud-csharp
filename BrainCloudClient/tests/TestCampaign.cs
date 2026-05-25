// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using NUnit.Framework;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCampaign : TestFixtureBase
    {
        [Test]
        public void TestGetMyCampaigns()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Campaign.GetMyCampaigns(
                null,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }
    }
}
