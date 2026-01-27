// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;
    using System;

    public class BrainCloudAppStore
    {
        private BrainCloudClient _client;

        public BrainCloudAppStore(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - GET_INVENTORY
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="userCurrency">The currency type to retrieve the sales inventory for.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
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
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - GET_INVENTORY
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="userCurrency">The currency type to retrieve the sales inventory for.</param>
        /// <param name="category">The product category</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
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
                priceInfoCriteria[OperationParam.AppStoreServiceUserCurrency.Value] = userCurrency;
            }
            data[OperationParam.AppStoreServicePriceInfoCriteria.Value] = priceInfoCriteria;

            if (Util.IsOptionalParameterValid(category))
            {
                data[OperationParam.AppStoreServiceCategory.Value] = category;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.GetInventory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the eligible promotions for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - ELIGIBLE_PROMOTIONS
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
        /// Before making a purchase with the IAP store, you will need to store the purchase
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - CACHE_PURCHASE_PAYLOAD_CONTEXT
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="iapId">The IAP product id as configured on brainCloud</param>
        /// <param name="payload">The payload retrieved for the IAP product</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void CachePurchasePayloadContext(
            string storeId,
            string iapId,
            string payload,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;
            data[OperationParam.AppStoreServiceIAPId.Value] = iapId;
            data[OperationParam.AppStoreServicePayload.Value] = payload;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.CachePurchasePayloadContext, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Verifies that purchase was properly made at the store.
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - VERIFY_PURCHASE
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="jsonReceiptData">The specific store data required</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void VerifyPurchase(
            string storeId,
            string receiptJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AppStoreServiceStoreId.Value] = storeId;

            Dictionary<string, object> receiptData;

            try
            {
                receiptData = JsonReader.Deserialize<Dictionary<string, object>>(receiptJson);
                data[OperationParam.AppStoreServiceReceiptData.Value] = receiptData;
            }
            catch
            {
                //not a valid json string, pass it as string directly
                data[OperationParam.AppStoreServiceReceiptData.Value] = receiptJson;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.VerifyPurchase, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Start A Two Staged Purchase Transaction
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - START_PURCHASE
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="jsonPurchaseData">Specific data for starting a two-stage purchase</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
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
        /// Service Name - appStore
        /// Service Operation - FINALIZE_PURCHASE
        /// </remarks>
        /// <param name="storeId">The store platform. Valid stores are: itunes facebook appworld steam windows windowsPhone googlePlay metaHorizon</param>
        /// <param name="transactionId">The transaction id returned from startPurchase</param>
        /// <param name="jsonTransactionData">Specific transaction data for finalizing purchase</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
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

        /// <summary>
        /// Returns up-to-date eligible 'promotions' for the user and a 'promotionsRefreshed' flag indicating whether the user's promotion info required refreshing.
        /// </summary>
        /// <remarks>
        /// Service Name - appStore
        /// Service Operation - REFRESH_PROMOTIONS
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void RefreshPromotions(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = new ServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AppStore, ServiceOperation.RefreshPromotions, data, callback);
            _client.SendRequest(sc);
        }
    }
}
