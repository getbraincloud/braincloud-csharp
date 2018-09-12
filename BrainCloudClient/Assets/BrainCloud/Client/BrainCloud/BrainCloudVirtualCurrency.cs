//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using System;

namespace BrainCloud
{

    public class BrainCloudVirtualCurrency
    {
        private BrainCloudClient _client;

        public BrainCloudVirtualCurrency(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets the player's currency for the given currency type
        /// or all currency types if null passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - VirtalCurrency
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
        public void GetCurrency(
            string currencyType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.VirtualCurrencyServiceCurrencyId.Value] = currencyType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.GetPlayerVC, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the parents's currency for the given currency type
        /// or all currency types if null passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - VirtalCurrency
        /// Service Operation - GetParentVC
        /// </remarks>
        /// <param name="currencyType">
        /// The currency type to retrieve or null
        /// if all currency types are being requested.
        /// </param>
        /// <param name="levelName">
        /// The parent level name
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
        public void GetParentCurrency(
            string currencyType, string levelName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.VirtualCurrencyServiceCurrencyId.Value] = currencyType;
            data[OperationParam.AuthenticateServiceAuthenticateLevelName.Value] = levelName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.GetParentVC, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the peers's currency for the given currency type
        /// or all currency types if null passed in.
        /// </summary>
        /// <remarks>
        /// Service Name - VirtalCurrency
        /// Service Operation - GetPeerVC
        /// </remarks>
        /// <param name="currencyType">
        /// The currency type to retrieve or null
        /// if all currency types are being requested.
        /// </param>
        /// <param name="peerCode">
        /// The peer code
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
        public void GetPeerCurrency(
            string currencyType, string peerCode,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.VirtualCurrencyServiceCurrencyId.Value] = currencyType;
            data[OperationParam.AuthenticateServiceAuthenticatePeerCode.Value] = peerCode;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.GetPeerVC, data, callback);
            _client.SendRequest(sc);
        }
    }
}
