//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System.Collections.Generic;
using BrainCloud.Internal;
using System;


    public class BrainCloudProduct
    {
        private BrainCloudClient _client;

        public BrainCloudProduct(BrainCloudClient client)
        {
            _client = client;
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
