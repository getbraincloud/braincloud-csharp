using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalApp : TestFixtureBase
    {
        [Test]
        public void TestGlobalAppReadProperties()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalAppService.ReadProperties(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void TestGlobalAppReadSelectedProperties()
        {
            TestResult tr = new TestResult(_bc);
            string[] propertyIds =
            {
                "prop1"
            };

            _bc.GlobalAppService.ReadSelectedProperties(propertyIds,tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void TestGlobalAppReadPropertiesInCategories()
        {
            TestResult tr = new TestResult(_bc);
            string[] categories =
            {
                "Test"
            };
            _bc.GlobalAppService.ReadPropertiesInCategories(categories, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}