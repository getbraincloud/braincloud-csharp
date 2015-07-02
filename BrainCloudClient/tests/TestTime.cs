using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTime : TestFixtureBase
    {
        [Test]
        public void TestReadServerTime()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().TimeService.ReadServerTime(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}