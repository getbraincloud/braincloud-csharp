using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestProduct : TestFixtureBase
    {
        private readonly string _currencytype = "credits";
        private readonly string _platform = "windows";
        private readonly string _productCatagory = "Test";

        [Test]
        public void TestAwardCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.AwardCurrency(
                _currencytype,
                200,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(403, ReasonCodes.CURRENCY_SECURITY_ERROR);
        }

        [Test]
        public void TestConsumeCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.ConsumeCurrency(
                _currencytype,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(403, ReasonCodes.CURRENCY_SECURITY_ERROR);
        }

        [Test]
        public void TestGetCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.GetCurrency(
                null,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetEligiblePromotions()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.GetEligiblePromotions(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventory()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.GetSalesInventory(
                _platform,
                "CAD",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventoryByCategory()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.GetSalesInventoryByCategory(
                _platform,
                "CAD",
                _productCatagory,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.ProductService.ResetCurrency(
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(403, ReasonCodes.CURRENCY_SECURITY_ERROR);
        }
    }
}