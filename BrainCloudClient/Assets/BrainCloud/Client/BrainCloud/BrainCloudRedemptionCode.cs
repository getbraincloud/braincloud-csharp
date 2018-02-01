//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using BrainCloud.Internal;
using JsonFx.Json;
using System.Collections.Generic;

namespace BrainCloud
{
    public class BrainCloudRedemptionCode
    {
        private BrainCloudClient _client;

        public BrainCloudRedemptionCode(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Redeem a code.
        /// </summary>
        /// <remarks>
        /// Service Name - redemptionCode
        /// Service Operation - REDEEM_CODE
        /// </remarks>
        /// <param name="scanCode">
        /// The code to redeem
        /// </param>
        /// <param name="codeType">
        /// The type of code
        /// </param>
        /// <param name="jsonCustomRedemptionInfo">
        /// Optional - A JSON string containing custom redemption data
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
        public void RedeemCode(
            string scanCode,
            string codeType,
            string jsonCustomRedemptionInfo,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.RedemptionCodeServiceScanCode.Value] = scanCode;
            data[OperationParam.RedemptionCodeServiceCodeType.Value] = codeType;

            if (Util.IsOptionalParameterValid(jsonCustomRedemptionInfo))
            {
                Dictionary<string, object> customRedemptionInfo = JsonReader.Deserialize<Dictionary<string, object>>(jsonCustomRedemptionInfo);
                data[OperationParam.RedemptionCodeServiceCustomRedemptionInfo.Value] = customRedemptionInfo;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.RedemptionCode, ServiceOperation.RedeemCode, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the codes already redeemed by player.
        /// </summary>
        /// <remarks>
        /// Service Name - redemptionCode
        /// Service Operation - GET_REDEEMED_CODES
        /// </remarks>
        /// <param name="codeType">
        /// Optional - The type of codes to retrieve. Returns all codes if left unspecified.
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
        public void GetRedeemedCodes(
            string codeType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = null;

            if (Util.IsOptionalParameterValid(codeType))
            {
                data = new Dictionary<string, object>();
                data[OperationParam.RedemptionCodeServiceCodeType.Value] = codeType;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.RedemptionCode, ServiceOperation.GetRedeemedCodes, data, callback);
            _client.SendRequest(sc);
        }
    }
}

