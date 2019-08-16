using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestUserInventoryManagement : TestFixtureBase
    {
        [Test]
        public void AwardUserItem()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserInventoryManagement.AwardUserItem("sword001", 5, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}