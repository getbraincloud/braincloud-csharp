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
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudRedemptionCode(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Redeem a code.
        /// </summary>
        /// <remarks>
        /// Service Name - redemptionCode
        /// Service Operation - REDEEM_CODE
        /// </remarks>
        /// <param name="in_scanCode">
        /// The code to redeem
        /// </param>
        /// <param name="in_codeType">
        /// The type of code
        /// </param>
        /// <param name="in_jsonCustomRedemptionInfo">
        /// Optional - A JSON string containing custom redemption data
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
        public void RedeemCode(
            string in_scanCode,
            string in_codeType,
            string in_jsonCustomRedemptionInfo,
            SuccessCallback in_success,
            FailureCallback in_failure,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.RedemptionCodeServiceScanCode.Value] = in_scanCode;
            data[OperationParam.RedemptionCodeServiceCodeType.Value] = in_codeType;

            if (Util.IsOptionalParameterValid(in_jsonCustomRedemptionInfo))
            {
                Dictionary<string, object> customRedemptionInfo = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonCustomRedemptionInfo);
                data[OperationParam.RedemptionCodeServiceCustomRedemptionInfo.Value] = customRedemptionInfo;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.RedemptionCode, ServiceOperation.RedeemCode, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the codes already redeemed by player.
        /// </summary>
        /// <remarks>
        /// Service Name - redemptionCode
        /// Service Operation - GET_REDEEMED_CODES
        /// </remarks>
        /// <param name="in_codeType">
        /// Optional - The type of codes to retrieve. Returns all codes if left unspecified.
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
        public void GetRedeemedCodes(
            string in_codeType,
            SuccessCallback in_success,
            FailureCallback in_failure,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = null;

            if (Util.IsOptionalParameterValid(in_codeType))
            {
                data = new Dictionary<string, object>();
                data[OperationParam.RedemptionCodeServiceCodeType.Value] = in_codeType;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.RedemptionCode, ServiceOperation.GetRedeemedCodes, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

