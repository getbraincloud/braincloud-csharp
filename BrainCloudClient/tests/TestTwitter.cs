using NUnit;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTwitter : TestFixtureBase
    {      
        [Test]
        public void Test()
        {
            TestResult tr = new TestResult();

            if (tr.Run())
            {
                // something
            }
        }
    }
}