using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestScript : TestFixtureBase
    {
        [Test]
        public void TestRunScript()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ScriptService.RunScript(
                "testScript",
                Helpers.CreateJsonPair("testParm1", 1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}