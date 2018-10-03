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
        public void TestGetCurrency()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.GetCurrency(
                null,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

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

        [Test]
        public void TestGetEligiblePromotions()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.GetEligiblePromotions(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.GetSalesInventory(
                _platform,
                "CAD",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventoryByCategory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ProductService.GetSalesInventoryByCategory(
                _platform,
                "CAD",
                _productCatagory,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}