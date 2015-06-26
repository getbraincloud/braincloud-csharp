using NUnit;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureBase
    {
        [Test]
        public void TestAuthenticateUniversal()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get ().AuthenticationService.AuthenticateUniversal(
                "abc", "abc", true, tr.ApiSuccess, tr.ApiError);
            if (tr.Run ())
            {
                // something
            }
        }
    }
}