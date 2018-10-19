//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;
using System;

namespace BrainCloud
{
    public class BrainCloudAppStore
    {
        private BrainCloudClient _client;

        public BrainCloudAppStore(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// currency type.
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - GetInventory
        /// </remarks>
        /// <param name="platform">
        /// The store platform. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="userCurrency">
        /// The currency to retrieve the sales
        /// inventory for. This is only used for Steam and Facebook stores.
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetSalesInventory(
            string platform,
            string userCurrency,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            GetSalesInventoryByCategory(platform, userCurrency, null, success, failure, cbObject);
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// currency type and category.
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - GetInventory
        /// </remarks>
        /// <param name="storeId">
        /// The store storeId. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="userCurrency">
        /// The currency to retrieve the sales
        /// inventory for. This is only used for Steam and Facebook stores.
        /// </param>
        /// <param name="category">
        /// The product category
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetSalesInventoryByCategory(
            string storeId,
            string userCurrency,
            string category,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;

            Dictionary<string, object> priceInfoCriteria = new Dictionary<string, object>();
            if (Util.IsOptionalParameterValid(userCurrency))
            {
                priceInfoCriteria[OperationParam.ProductServiceGetInventoryUserCurrency.Value] = userCurrency;
            }
            data[OperationParam.AppStoreServicePriceInfoCriteria.Value] = priceInfoCriteria;

            if (Util.IsOptionalParameterValid(category))
            {
                data[OperationParam.ProductServiceGetInventoryCategory.Value] = category;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.GetInventory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the eligible promotions for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - EligiblePromotions
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetEligiblePromotions(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.EligiblePromotions, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Verify Purchase with the associated StoreId
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - VERIFY_PURCHASE
        /// </remarks>
        /// <param name="storeId">
        /// The store storeId. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="receiptJson">
        /// The specific store data required
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void VerifyPurchase(
            string storeId,
            string receiptJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;

            var receiptData = JsonReader.Deserialize<Dictionary<string, object>>(receiptJson);
            data[OperationParam.AppStoreServiceReceiptData.Value] = receiptData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.VerifyPurchase, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Start A Two Staged Purchase Transaction
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - START_PURCHASE
        /// </remarks>
        /// <param name="storeId">
        /// The store storeId. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="purchaseJson">
        /// The specific store data required
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void StartPurchase(
            string storeId,
            string purchaseJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;

            var purchaseData = JsonReader.Deserialize<Dictionary<string, object>>(purchaseJson);
            data[OperationParam.AppStoreServicePurchaseData.Value] = purchaseData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.StartPurchase, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finalize A Two Staged Purchase Transaction
        /// </summary>
        /// <remarks>
        /// Service Name - AppStore
        /// Service Operation - FINALIZE_PURCHASE
        /// </remarks>
        /// <param name="storeId">
        /// The store storeId. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// /// <param name="transactionId">
        /// The Transaction Id returned in Start Transaction
        /// </param>
        /// <param name="transactionJson">
        /// The specific store data required
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void FinalizePurchase(
            string storeId,
            string transactionId,
            string transactionJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;
            data[OperationParam.AppStoreServiceTransactionId.Value] = transactionId;

            var transactionData = JsonReader.Deserialize<Dictionary<string, object>>(transactionJson);
            data[OperationParam.AppStoreServiceTransactionData.Value] = transactionData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.FinalizePurchase, data, callback);
            _client.SendRequest(sc);
        }
    }
}
