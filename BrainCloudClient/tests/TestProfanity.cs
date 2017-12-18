using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestProfanity : TestFixtureBase
    {
        [Test]
        public void TestProfanityCheck()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProfanityService.ProfanityCheck(
                "shitbird fly away", "en", true, true, true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestProfanityReplaceText()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProfanityService.ProfanityReplaceText(
                "shitbird fly away", "*", "en", false, false, false,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestProfanityIdentifyBadWords()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProfanityService.ProfanityIdentifyBadWords(
                "shitbird fly away", "en", true, false, false,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}