// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.Common;
using NUnit.Core;
using NUnit.Framework;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestACL : TestFixtureBase
    {
        [Test]
        public void Test()
        {
            ACL acl = new ACL(ACL.Access.ReadWrite);

            Dictionary<string, object> jsonObj = new Dictionary<string, object> { { "other", 1 } };
            
            acl = ACL.CreateFromJson(jsonObj);

            Assert.That(ACL.Access.ReadOnly == acl.Other);
        }
    }
}
