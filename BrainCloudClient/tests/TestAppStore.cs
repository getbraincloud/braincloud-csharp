using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAppStore : TestFixtureBase
    {
        [Test]
        public void TestVerifyPurchase()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.VerifyPurchase("_invalid_store_id_", "{}", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }

        [Test]
        public void TestGetEligiblePromotions()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.GetEligiblePromotions(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetSalesInventory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.GetSalesInventory("_invalid_store_id_", "_invalid_user_currency_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }

        [Test]
        public void TestGetSalesInventoryByCategory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.GetSalesInventoryByCategory("_invalid_store_id_", "_invalid_user_currency_", "_invalid_category_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }

        [Test]
        public void TestStartPurchaseFail()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.StartPurchase("_invalid_store_id_", "{}", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }
        
        [Test]
        public void TestStartPurchase()
        {
            //Had to run StartPurchase through a cloud code script to skip permissions for testing with mock store
            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunScript("TestPurchase","{}", tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestFinalizePurchase()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.FinalizePurchase("_invalid_store_id_", "_invalid_transaction_id_", "{}", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }

        [Test]
        public void TestRefreshPromotions()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.RefreshPromotions(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
