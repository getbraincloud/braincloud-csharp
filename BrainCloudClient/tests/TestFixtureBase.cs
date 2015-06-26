using NUnit;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        [SetUp]
        public void Setup()
        {
            BrainCloudClient.Get ().Initialize (
                "https://internal.braincloudservers.com/dispatcherv2",
                "60c732f5-6552-4795-8f69-674531b175be",
                "10170",
                "1.0.0");
            BrainCloudClient.Get ().EnableLogging(true);
        }

        [TearDown]
        public void TearDown()
        {
            BrainCloudClient.Get ().ResetCommunication();
        }
    }
}