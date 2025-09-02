using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAppStore : TestFixtureBase
    {
        private const string _email = "UnityTestee@bctestuser.com";
        private const string _password = "12345";

        [Test]
        public void TestCachePurchasePayloadContext()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AppStoreService.CachePurchasePayloadContext("_invalid_store_id_", "_invalid_iap_id_", "_invalid_payload_", tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_STORE_ID);
        }

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

        [Test]
        public void TestMockStoreCachePayloadPurchaseContextThenVerifyPurchase()
        {
            TestResult tr = new TestResult(_bc);

            _bc.AuthenticateEmailPassword
            (
                _email,
                _password,
                true,
                tr.ApiSuccess,
                tr.ApiError
            );

            tr.Run();

            _bc.AppStoreService.GetSalesInventory // googlePlay has a product with payload included
            (
                "googlePlay",
                string.Empty,
                tr.ApiSuccess,
                tr.ApiError
            );

            tr.Run();

            // Get data needed to do CachePurchasePayloadContext and VerifyPurchase (mock)
            var data = tr.m_response;
            var products = ((data["data"] as Dictionary<string, object>)["productInventory"] as Dictionary<string, object>[])[0];

            string itemId = products["itemId"].ToString();
            string payload = products["payload"].ToString();
            string iapId = (products["priceData"] as Dictionary<string, object>)["id"].ToString();

            _bc.AppStoreService.CachePurchasePayloadContext
            (
                "mock",
                iapId,
                payload,
                tr.ApiSuccess,
                tr.ApiError
            );

            tr.Run();

            Random rng = new Random((int)DateTime.UtcNow.Ticks);
            var stop = DateTime.UtcNow.AddSeconds(rng.NextDouble() + rng.Next(3, 6)); // Simulate user & store processing time
            while (DateTime.UtcNow < stop)
            {
                _bc.Update();
                Thread.Sleep(16);
            }

            // Setup mock receiptData
            var transactions = new Dictionary<string, object>[1];
            transactions[0] = new Dictionary<string, object>()
            {
                { "transactionId", Guid.NewGuid().ToString()                               },
                { "itemId",        itemId                                                  },
                { "sandbox",       true                                                    },
                { "quantity",      1                                                       },
                { "payload",       payload                                                 },
                { "iapId",         iapId                                                   },
                { "receiptTime",   (ulong)TimeUtil.UTCDateTimeToUTCMillis(DateTime.UtcNow) }
            };

            string jsonScriptData = JsonWriter.Serialize(new Dictionary<string, object>()
            {
                { "storeId",     "mock"                                                             },
                { "receiptData", new Dictionary<string, object>() {{ "transactions", transactions }}}
            });

            _bc.ScriptService.RunScript // Need to run script in order to do VerifyPurchase (mock)
            (
                "/VerifyPurchaseMockStore",
                jsonScriptData,
                tr.ApiSuccess,
                tr.ApiError
            );

            tr.Run();

            // Ensure VerifyPurchase response returns the data we expect
            data = tr.m_response;

            Assert.That(data, Is.Not.Null, "Script response is null");
            Assert.That(data, Is.Not.Empty, "Script response is empty");
            Assert.That(data["status"], Is.EqualTo(200), "Response status is not 200");
            Assert.That(data["data"], Is.Not.Null, "Script response data is null");
            Assert.That(data["data"], Is.Not.Empty, "Script response data is empty");

            var response = (data["data"] as Dictionary<string, object>)["response"] as Dictionary<string, object>;

            Assert.That(response, Is.Not.Null, "VerifyPurchase response is null");
            Assert.That(response, Is.Not.Empty, "VerifyPurchase response is empty");

            if (response.ContainsKey("errorMessage"))
            {
                Console.WriteLine(response["errorMessage"]);
            }

            Assert.That(response, Contains.Key("success"), "VerifyPurchase response did not contain a success key");
            Assert.That(response["success"], Is.True, "VerifyPurchase response did not return a success");

            Assert.That(response, Contains.Key("data"), "VerifyPurchase did not return data");
            Assert.That(response["data"], Is.Not.Null, "VerifyPurchase data is null");
            Assert.That(response["data"], Is.Not.Empty, "VerifyPurchase data is empty");
            Assert.That(response["data"], Contains.Key("data"), "VerifyPurchase data does not contain post results");
            Assert.That((response["data"] as Dictionary<string, object>)["data"], Is.Not.Null, "VerifyPurchase post results is null");
            Assert.That((response["data"] as Dictionary<string, object>)["data"], Is.Not.Empty, "VerifyPurchase post results is empty");

            var summary = ((response["data"] as Dictionary<string, object>)
                          ["data"] as Dictionary<string, object>)
                          ["transactionSummary"] as Dictionary<string, object>;

            Assert.That(summary, Is.Not.Null, "transactionSummary is null");
            Assert.That(summary, Is.Not.Empty, "transactionSummary is empty");
            Assert.That(summary["unprocessedCount"], Is.Zero, "There are unprocessed transactions");
            Assert.That(summary["processedCount"], Is.Not.Zero, "Processed transactions is zero");
            Assert.That(summary["processedCount"], Is.EqualTo(1), "There are more than one processed transactions");

            var details = (summary["transactionDetails"] as Dictionary<string, object>[])[0];

            Assert.That(details, Is.Not.Null, "transactionDetails[0] is null");
            Assert.That(details, Is.Not.Empty, "transactionDetails[0] is empty");
            Assert.That((summary["transactionDetails"] as Dictionary<string, object>[]).Length,
                        Is.EqualTo(summary["processedCount"]),
                        "The amount of transactions does not equal the amount that were processed");
            Assert.That(details["processed"], Is.True, "The transaction was not processed");
            Assert.That(details["itemId"].ToString(), Is.EqualTo(itemId), "itemId from transaction does not match what was sent");
            Assert.That(details["payload"].ToString(), Is.EqualTo(payload), "payload from transaction does not match what was sent");
        }
    }
}
