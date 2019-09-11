using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestUserItemsService : TestFixtureBase
    {

        List<object> testItems = new List<object>();

        [Test]
        public void AwardUserItem()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.AwardUserItem("sword001", 6, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            //get a list of the items 
            var data = tr.m_response["data"] as Dictionary<string, object>;
            var items = data["items"] as Dictionary<string, object>;
            foreach (KeyValuePair<string, object> item in items) {
                 testItems.Add(item.Key);
            }
        }

        [Test]
        public void DropUserItem()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.DropUserItem(testItems[0] as string, 1, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void GetUserItemsPage()
        {
            string criteria = "{\"test\": \"Testing\"}";
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.GetUserItemsPage(criteria, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void GetUserItemsPageOffset()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJnYW1lSWQiOiIyMDAwMSIsInBsYXllcklkIjoiNmVhYWU4M2EtYjZkMy00NTM5LWExZjAtZTIxMmMzYjUzMGIwIiwiZ2lmdGVkVG8iOm51bGx9LCJzb3J0Q3JpdGVyaWEiOnt9LCJwYWdpbmF0aW9uIjp7InJvd3NQZXJQYWdlIjoxMDAsInBhZ2VOdW1iZXIiOm51bGx9LCJvcHRpb25zIjpudWxsfQ";
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.GetUserItemsPageOffset(context, 1, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void GetUserItem()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.GetUserItem(testItems[1] as string, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void GiveUserItemTo()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.GiveUserItemTo(GetUser(Users.UserB).ProfileId, testItems[2] as string, 1, 1, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void PurchaseUserItem()
        {
            TestResult tr = new TestResult(_bc);
            _bc.UserItemsService.PurchaseUserItem("sword001", 1, null, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
        
        [Test]
        public void ReceiveUserItemFrom()
        {
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.ReceiveUserItemFrom(GetUser(Users.UserB).ProfileId, testItems[2] as string,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.ITEM_NOT_FOUND);
        }
        
        [Test]
        public void SellUserItem()
        {
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.SellUserItem(testItems[3] as string, 1, 1, null, true,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }

        [Test]
        public void UpdateUserItemData()
        {
            string newItemData = "{\"test\": \"Testing\"}";
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.UpdateUserItemData(testItems[4] as string, 1, newItemData,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }

        [Test]
        public void UseUserItem()
        {
            string newItemData = "{\"test\": \"Testing\"}";
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.UseUserItem(testItems[4] as string, 2, newItemData, true,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }

        [Test]
        public void PublishUserItemToBlockchain()
        {
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.PublishUserItemToBlockchain("invalidForNow", 1,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.ITEM_NOT_FOUND);
        }

        [Test]
        public void RefreshBlockchainUserItems()
        {
            TestResult tr2 = new TestResult(_bc);
            _bc.UserItemsService.RefreshBlockchainUserItems(
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }
    }
}