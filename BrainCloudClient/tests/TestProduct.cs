using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

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

            BrainCloudClient.Get().ProductService.AwardCurrency(
                _currencytype,
                200,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestConsumeCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ProductService.ConsumeCurrency(
                _currencytype,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetCurrency()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ProductService.GetCurrency(
                _currencytype,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetEligiblePromotions()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ProductService.GetEligiblePromotions(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventory()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ProductService.GetSalesInventory(
                _platform,
                "CAD",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetSalesInventoryByCategory()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ProductService.GetSalesInventoryByCategory(
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

            BrainCloudClient.Get().ProductService.ResetCurrency(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}