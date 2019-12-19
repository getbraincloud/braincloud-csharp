using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestProduct : TestFixtureBase
    {
        private readonly string _platform = "windows";
        private readonly string _productCatagory = "Test";
        private readonly string _currencytype = "credits";

        [Test]
        public void TestAwardCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.AwardCurrency(
                _currencytype,
                200,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestConsumeCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.ConsumeCurrency(
                _currencytype,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.ResetCurrency(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}