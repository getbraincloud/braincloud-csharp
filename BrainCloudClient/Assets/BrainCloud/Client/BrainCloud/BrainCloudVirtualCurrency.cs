// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.Internal;
    using System;
    public class BrainCloudVirtualCurrency
    {
        private BrainCloudClient _client;

        public BrainCloudVirtualCurrency(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Retrieve the user's currency account. Optional parameter: `vcId` (if retrieving a specific currency).
        /// </summary>
        /// <remarks>
        /// Service Name - virtualCurrency
        /// Service Operation - GET_PLAYER_VC
        /// </remarks>
        /// <param name="vcId">Optional currency id to retrieve (pass NULL to get all currencies)</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
        /// Retrieve the parent user's currency account. Optional parameter: `vcId` (if retrieving a specific currency).
        /// </summary>
        /// <remarks>
        /// Service Name - virtualCurrency
        /// Service Operation - GET_PARENT_VC
        /// </remarks>
        /// <param name="vcId">Optional currency id to retrieve (pass NULL to get all currencies)</param>
        /// <param name="levelName">The parent level name</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
        /// Retrieve the peer user's currency account. Optional parameter: `vcId` (if retrieving a specific currency).
        /// </summary>
        /// <remarks>
        /// Service Name - virtualCurrency
        /// Service Operation - GET_PEER_VC
        /// </remarks>
        /// <param name="vcId">Optional currency id to retrieve (pass NULL to get all currencies)</param>
        /// <param name="peerCode">The peer code identifying the other user</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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

        /// <summary>
        /// Reset player's currency to zero
        /// </summary>
        /// <remarks>
        /// Service Name - virtualCurrency
        /// Service Operation - RESET_PLAYER_VC
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ResetCurrency(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.ResetPlayerVC, data, callback);
            _client.SendRequest(sc);
        }
        #region Obsolete
        [Obsolete("For security reasons calling this API from the client is not recommended, and is rejected at the server by default. To over-ride, enable the 'Allow Currency Calls from Client' compatibility setting in the Design Portal.")]
        public void AwardCurrency(
            string currencyType,
            ulong amount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.VirtualCurrencyServiceCurrencyId.Value] = currencyType;
            data[OperationParam.VirtualCurrencyServiceCurrencyAmount.Value] = amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.AwardVC, data, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("For security reasons calling this API from the client is not recommended, and is rejected at the server by default. To over-ride, enable the 'Allow Currency Calls from Client' compatibility setting in the Design Portal.")]
        public void ConsumeCurrency(
            string currencyType,
            ulong amount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.VirtualCurrencyServiceCurrencyId.Value] = currencyType;
            data[OperationParam.VirtualCurrencyServiceCurrencyAmount.Value] = amount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.VirtualCurrency, ServiceOperation.ConsumePlayerVC, data, callback);
            _client.SendRequest(sc);
        }
        #endregion
    }
}
