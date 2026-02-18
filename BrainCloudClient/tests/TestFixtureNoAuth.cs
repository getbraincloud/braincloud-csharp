// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using NUnit.Core;
using NUnit.Framework;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureNoAuth : TestFixtureBase
    {
        public override bool ShouldAuthenticate()
        {
            return false;
        }
    }
}