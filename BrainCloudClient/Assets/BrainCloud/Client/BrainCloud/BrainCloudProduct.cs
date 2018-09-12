//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using System;

namespace BrainCloud
{

    public class BrainCloudProduct
    {
        private BrainCloudClient _client;

        public BrainCloudProduct(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the player's currency for the given currency type
        /// or all currency types if null passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - GetPlayerVC
        /// </remarks>
        /// <param name="currencyType">
        /// The currency type to retrieve or null
        /// if all currency types are being requested.
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudVirtualCurrency.getCurrency")]
        public void GetCurrency(
            string currencyType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = currencyType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GetPlayerVC, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// currency type.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.GetSalesInventory")]
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
        /// Service Name - Product
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.GetSalesInventoryByCategory")]
        public void GetSalesInventoryByCategory(
            string platform,
            string userCurrency,
            string category,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceGetInventoryPlatform.Value] = platform;
            if (Util.IsOptionalParameterValid(userCurrency))
            {
                data[OperationParam.ProductServiceGetInventoryUserCurrency.Value] = userCurrency;
            }
            if (Util.IsOptionalParameterValid(category))
            {
                data[OperationParam.ProductServiceGetInventoryCategory.Value] = category;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GetInventory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Initialize Steam Transaction
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - INITIALIZE_STEAM_TRANSACTION
        /// </remarks>
        /// <param name="language">
        /// ISO 639-1 language code
        /// </param>
        /// <param name="items">
        /// Items to purchase
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.StartPurchase")]
        public void StartSteamTransaction(
            string language,
            string itemId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceLanguage.Value] = language;
            data[OperationParam.ProductServiceItemId.Value] = itemId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.StartSteamTransaction, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finalize Steam Transaction. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - FINALIZE_STEAM_TRANSACTION
        /// </remarks>
        /// <param name="transId">
        /// Steam transaction id
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.FinalizePurchase")]
        public void FinalizeSteamTransaction(
            string transId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceTransId.Value] = transId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.FinalizeSteamTransaction, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Verify Microsoft Receipt. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - VERIFY_MICROSOFT_RECEIPT
        /// </remarks>
        /// <param name="receipt">
        /// Receipt XML
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.VerifyPurchase")]
        public void VerifyMicrosoftReceipt(
            string receipt,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceReceipt.Value] = receipt;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.VerifyMicrosoftReceipt, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the eligible promotions for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.GetEligiblePromotions")]
        public void GetEligiblePromotions(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.EligiblePromotions, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Verify ITunes Receipt. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - OP_CASH_IN_RECEIPT
        /// </remarks>
        /// <param name="base64EncReceiptData">
        /// Base64 encoded receipt data
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.VerifyPurchase")]
        public void VerifyItunesReceipt(
            string base64EncReceiptData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> message = new Dictionary<string, object>();
            message[OperationParam.ProductServiceOpCashInReceiptReceipt.Value] = base64EncReceiptData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.CashInReceipt, message, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Confirm Facebook Purchase. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - FB_CONFIRM_PURCHASE
        /// </remarks>
        /// <param name="signedRequest">
        /// signed_request object received from Facebook
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.VerifyPurchase")]
        public void ConfirmFacebookPurchase(
            string signedRequest,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceSignedRequest.Value] = signedRequest;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.FbConfirmPurchase, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Confirm GooglePlay Purchase. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - CONFIRM_GOOGLEPLAY_PURCHASE
        /// </remarks>
        /// <param name="orderId">
        /// GooglePlay order id
        /// </param>
        /// <param name="productId">
        /// GooglePlay product id
        /// </param>
        /// <param name="token">
        /// GooglePlay token string
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
        [Obsolete("Will be removed September 2019, Please use BrainCloudAppStore.VerifyPurchase")]
        public void ConfirmGooglePlayPurchase(
            string orderId,
            string productId,
            string token,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceOrderId.Value] = orderId;
            data[OperationParam.ProductServiceProductId.Value] = productId;
            data[OperationParam.ProductServiceToken.Value] = token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GooglePlayConfirmPurchase, data, callback);
            _client.SendRequest(sc);
        }

        #region Deprecated
        [Obsolete("Method is now available in Cloud Code only for security. If you need to use it client side, enable 'Allow Currency Calls from Client' on the brainCloud dashboard")]
        public void AwardCurrency(
            string currencyType,
            ulong amount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.AwardVC, data, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("Method is now available in Cloud Code only for security. If you need to use it client side, enable 'Allow Currency Calls from Client' on the brainCloud dashboard")]
        public void ConsumeCurrency(
            string currencyType,
            ulong amount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ConsumePlayerVC, data, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("Method is now available in Cloud Code only for security. If you need to use it client side, enable 'Allow Currency Calls from Client' on the brainCloud dashboard")]
        public void ResetCurrency(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ResetPlayerVC, null, callback);
            _client.SendRequest(sc);
        }

        #endregion
    }
}
