//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using BrainCloud.Internal;

namespace BrainCloud
{

    public class BrainCloudProduct
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudProduct(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Gets the player's currency for the given currency type
        /// or all currency types if null passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - GetPlayerVC
        /// </remarks>
        /// <param name="in_currencyType">
        /// The currency type to retrieve or null
        /// if all currency types are being requested.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data": {
        ///     "updatedAt": 1395693676208,
        ///     "currencyMap": {
        ///       "gold": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 0,
        ///         "awarded": 0
        ///       }
        ///     },
        ///     "playerId": "6ea79853-4025-4159-8014-60a6f17ac4e6",
        ///     "createdAt": 1395693676208
        ///   }
        /// }
        /// </returns>
        public void GetCurrency(
            string in_currencyType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GetPlayerVC, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Award player the passed-in amount of currency. Returns
        /// JSON representing the new currency values.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - AwardVC
        /// </remarks>
        /// <param name="in_currencyType">
        /// The currency type to award.
        /// </param>
        /// <param name="in_amount">
        /// The amount of currency to award.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data": {
        ///     "updatedAt": 1395693913234,
        ///     "currencyMap": {
        ///       "gems": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 0,
        ///         "awarded": 0
        ///       },
        ///       "gold": {
        ///         "purchased": 0,
        ///         "balance": 123,
        ///         "consumed": 0,
        ///         "awarded": 123
        ///       }
        ///     },
        ///     "playerId": "acf11847-055f-470d-abb7-b93052201491",
        ///     "createdAt": 1395693907421
        ///   }
        /// }
        /// </returns>
        public void AwardCurrency(
            string in_currencyType,
            ulong in_amount,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = in_amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.AwardVC, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Consume the passed-in amount of currency from the player.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - ConsumePlayerVC
        /// </remarks>
        /// <param name="in_currencyType">
        /// The currency type to consume.
        /// </param>
        /// <param name="in_amount">
        /// The amount of currency to consume.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data": {
        ///     "updatedAt": 1395693913234,
        ///     "currencyMap": {
        ///       "gems": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 0,
        ///         "awarded": 0
        ///       },
        ///       "gold": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 123,
        ///         "awarded": 123
        ///       }
        ///     },
        ///     "playerId": "acf11847-055f-470d-abb7-b93052201491",
        ///     "createdAt": 1395693907421
        ///   }
        /// }
        /// </returns>
        public void ConsumeCurrency(
            string in_currencyType,
            ulong in_amount,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = in_amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ConsumePlayerVC, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Resets the player's currency back to zero.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - ResetPlayerVC
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void ResetCurrency(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ResetPlayerVC, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// currency type.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - GetInventory
        /// </remarks>
        /// <param name="in_platform">
        /// The store platform. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="in_userCurrency">
        /// The currency to retrieve the sales
        /// inventory for. This is only used for Steam and Facebook stores.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///      "product_inventory":[
        ///          {
        ///              "gameId":"com.roger.football",
        ///              "itemId":"0000001",
        ///              "title":"Item 0000001",
        ///              "description":"Buy 5 footballs",
        ///              "imageUrl":"http:",
        ///              "fbUrl":"http:",
        ///              "currency":{"footballs":5},
        ///              "priceData":{"currency":"USD","price":1000}
        ///           }
        ///       ],
        ///       "server_time":1398960658981
        ///    }
        /// }
        /// </returns>
        public void GetSalesInventory(
            string in_platform,
            string in_userCurrency,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            GetSalesInventoryByCategory(in_platform, in_userCurrency, null, in_success, in_failure, in_cbObject);
        }

        /// <summary>
        /// Method gets the active sales inventory for the passed-in
        /// currency type and category.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - GetInventory
        /// </remarks>
        /// <param name="in_platform">
        /// The store platform. Valid stores are:
        /// - itunes
        /// - facebook
        /// - appworld
        /// - steam
        /// - windows
        /// - windowsPhone
        /// - googlePlay
        /// </param>
        /// <param name="in_userCurrency">
        /// The currency to retrieve the sales
        /// inventory for. This is only used for Steam and Facebook stores.
        /// </param>
        /// <param name="in_category">
        /// The product category
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///      "product_inventory":[
        ///          {
        ///              "gameId":"com.roger.football",
        ///              "itemId":"0000001",
        ///              "title":"Item 0000001",
        ///              "description":"Buy 5 footballs",
        ///              "imageUrl":"http:",
        ///              "fbUrl":"http:",
        ///              "currency":{"footballs":5},
        ///              "priceData":{"currency":"USD","price":1000}
        ///           }
        ///       ],
        ///       "server_time":1398960658981
        ///    }
        /// }
        /// </returns>
        public void GetSalesInventoryByCategory(
            string in_platform,
            string in_userCurrency,
            string in_category,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceGetInventoryPlatform.Value] = in_platform;
            if (Util.IsOptionalParameterValid(in_userCurrency))
            {
                data[OperationParam.ProductServiceGetInventoryUserCurrency.Value] = in_userCurrency;
            }
            if (Util.IsOptionalParameterValid(in_category))
            {
                data[OperationParam.ProductServiceGetInventoryCategory.Value] = in_category;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GetInventory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Initialize Steam Transaction
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - INITIALIZE_STEAM_TRANSACTION
        /// </remarks>
        /// <param name="in_language">
        /// ISO 639-1 language code
        /// </param>
        /// <param name="in_items">
        /// Items to purchase
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "steamStatus" : 200,
        ///      "steamData" :
        ///      {
        ///      }
        ///   }
        /// }
        /// </returns>
        public void StartSteamTransaction(
            String in_language,
            String in_itemId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceLanguage.Value] = in_language;
            data[OperationParam.ProductServiceItemId.Value] = in_itemId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.StartSteamTransaction, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Finalize Steam Transaction. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - FINALIZE_STEAM_TRANSACTION
        /// </remarks>
        /// <param name="in_transId">
        /// Steam transaction id
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "steamStatus" : 200,
        ///      "steamData" :
        ///      {
        ///      }
        ///   }
        /// }
        /// </returns>
        public void FinalizeSteamTransaction(
            string in_transId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceTransId.Value] = in_transId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.FinalizeSteamTransaction, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Verify Microsoft Receipt. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - VERIFY_MICROSOFT_RECEIPT
        /// </remarks>
        /// <param name="in_receipt">
        /// Receipt XML
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "result" : "OK"
        ///   }
        /// }
        /// </returns>
        public void VerifyMicrosoftReceipt(
            string in_receipt,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceReceipt.Value] = in_receipt;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.VerifyMicrosoftReceipt, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns the eligible promotions for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - Product
        /// Service Operation - EligiblePromotions
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        /// "promotions": [
        ///  {
        ///   "gameId": "10019",
        ///   "promotionId": 9,
        ///   "type": "SCHEDULED",
        ///   "name": "session >= 2",
        ///   "message": "test1",
        ///   "enabled": true,
        ///   "targetAllUsers": false,
        ///   "segments": [
        ///    5
        ///   ],
        ///   "prices": [
        ///    {
        ///     "itemId": "regGems150",
        ///     "priceId": 1
        ///    }
        ///   ],
        ///   "notifications": [
        ///    {
        ///     "trigger": "ACTIVATED",
        ///     "notificationTemplateId": 10
        ///    }
        ///   ],
        ///   "startAt": 1415374185745,
        ///   "endAt": 1415806185745,
        ///   "createdAt": 0,
        ///   "updatedAt": 1415729753294
        ///  }
        /// ]
        /// }
        /// }
        /// </returns>
        public void GetEligiblePromotions(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.EligiblePromotions, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Verify ITunes Receipt. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - OP_CASH_IN_RECEIPT
        /// </remarks>
        /// <param name="in_base64EncReceiptData">
        /// Base64 encoded receipt data
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "playerCurrency" : {
        ///         "playerId" : "sfhsjdfhfjhf",
        ///         "currencyMap" : {
        ///             "coin" : {
        ///                 "purchased" : NumberLong(0),
        ///                 "balance" : NumberLong(5000),
        ///                 "consumed" : NumberLong(0),
        ///                 "awarded" : NumberLong(5000)
        ///             },
        ///             "bar" : {
        ///                 "purchased" : NumberLong(0),
        ///                 "balance" : NumberLong(2),
        ///                 "consumed" : NumberLong(0),
        ///                 "awarded" : NumberLong(2)
        ///             }
        ///         },
        ///         "createdAt" : 763578645786,
        ///         "updatedAt" : 8692486255764,
        ///       },
        ///       "appleReceipt" : "gsgsfvgvg",
        ///       "status" : 0,
        ///       "server_time" : 987490827457
        ///   }
        /// }
        /// </returns>
        public void VerifyItunesReceipt(
            String in_base64EncReceiptData,
            SuccessCallback in_callback = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> message = new Dictionary<string, object>();
            message[OperationParam.ProductServiceOpCashInReceiptReceipt.Value] = in_base64EncReceiptData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_callback, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.CashInReceipt, message, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Confirm Facebook Purchase. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - FB_CONFIRM_PURCHASE
        /// </remarks>
        /// <param name="in_signedRequest">
        /// signed_request object received from Facebook
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "result" : "OK"
        ///   }
        /// }
        /// </returns>
        public void ConfirmFacebookPurchase(
            string in_signedRequest,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceSignedRequest.Value] = in_signedRequest;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.FbConfirmPurchase, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Confirm GooglePlay Purchase. On success, the player will be awarded the 
        /// associated currencies.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - CONFIRM_GOOGLEPLAY_PURCHASE
        /// </remarks>
        /// <param name="in_orderId">
        /// GooglePlay order id
        /// </param>
        /// <param name="in_productId">
        /// GooglePlay product id
        /// </param>
        /// <param name="in_token">
        /// GooglePlay token string
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status": 200,
        ///   "data":
        ///   {
        ///      "result" : "OK"
        ///   }
        /// }
        /// </returns>
        public void ConfirmGooglePlayPurchase(
            string in_orderId,
            string in_productId,
            string in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceOrderId.Value] = in_orderId;
            data[OperationParam.ProductServiceProductId.Value] = in_productId;
            data[OperationParam.ProductServiceToken.Value] = in_token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GooglePlayConfirmPurchase, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Awards currency in a parent app
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - AWARD_PARENT_VC
        /// </remarks>
        /// <param name="in_currencyType">The ID of the parent currency</param>
        /// <param name="in_amount">The amount of currency to award</param>
        /// <param name="in_parentLevel">The level of the parent containing the currency</param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The user object sent to the callback</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "updatedAt": 1441990173583,
        ///         "currencyMap": {
        ///             "credits": {
        ///                 "purchased": 0,
        ///                 "balance": 123,
        ///                 "consumed": 24,
        ///                 "awarded": 147
        ///             },
        ///             "gold": {
        ///                 "purchased": 0,
        ///                 "balance": 100,
        ///                 "consumed": 0,
        ///                 "awarded": 100
        ///             }
        ///         },
        ///         "playerId": "5b33nje0-89a5-4b29-b130-effg3s451fa9",
        ///         "createdAt": 1441990131453
        ///     }
        /// }
        /// </returns>
        public void AwardParentCurrency(
            string in_currencyType,
            int in_amount,
            string in_parentLevel,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = in_amount;
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = in_parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.AwardParentCurrency, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Consumes currency in a parent app
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - CONSUME_PARENT_VC
        /// </remarks>
        /// <param name="in_currencyType">The ID of the parent currency</param>
        /// <param name="in_amount">The amount of currency to consume</param>
        /// <param name="in_parentLevel">The level of the parent containing the currency</param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The user object sent to the callback</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "updatedAt": 1441990173583,
        ///         "currencyMap": {
        ///             "credits": {
        ///                 "purchased": 0,
        ///                 "balance": 123,
        ///                 "consumed": 24,
        ///                 "awarded": 147
        ///             },
        ///             "gold": {
        ///                 "purchased": 0,
        ///                 "balance": 100,
        ///                 "consumed": 0,
        ///                 "awarded": 100
        ///             }
        ///         },
        ///         "playerId": "5b33nje0-89a5-4b29-b130-effg3s451fa9",
        ///         "createdAt": 1441990131453
        ///     }
        /// }
        /// </returns>
        public void ConsumeParentCurrency(
            string in_currencyType,
            int in_amount,
            string in_parentLevel,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;
            data[OperationParam.ProductServiceCurrencyAmount.Value] = in_amount;
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = in_parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ConsumeParentCurrency, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets information on a single currency in a parent app
        /// or all currency types if a null type is passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - GET_PARENT_VC
        /// </remarks>
        /// <param name="in_currencyType">The ID of the parent currency or null to get all currencies</param>
        /// <param name="in_parentLevel">The level of the parent containing the currency</param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The user object sent to the callback</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "updatedAt": 1441990173583,
        ///         "currencyMap": {
        ///             "credits": {
        ///                 "purchased": 0,
        ///                 "balance": 123,
        ///                 "consumed": 24,
        ///                 "awarded": 147
        ///             }
        ///         },
        ///         "playerId": "5b33nje0-89a5-4b29-b130-effg3s451fa9",
        ///         "createdAt": 1441990131453
        ///     }
        /// }
        /// </returns>
        public void GetParentCurrency(
            string in_currencyType,
            string in_parentLevel,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_currencyType))
            {
                data[OperationParam.ProductServiceCurrencyId.Value] = in_currencyType;
            }
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = in_parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.GetParentCurrency, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Resets all currencies in a parent app
        /// </summary>
        /// <remarks>
        /// Service Name - product
        /// Service Operation - RESET_PARENT_VC
        /// </remarks>
        /// <param name="in_parentLevel">The level of the parent containing the currencies</param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The user object sent to the callback</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
        public void ResetParentCurrency(
            string in_parentLevel,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = in_parentLevel;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Product, ServiceOperation.ResetParentCurrency, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
