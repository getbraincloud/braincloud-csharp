using System;
using System.IO;
using System.Reflection;
using NUnit;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

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