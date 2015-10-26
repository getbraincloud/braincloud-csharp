//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
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
        /// <param name="in_customRedemptionInfo">
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "gameId": "123456",
        ///         "scanCode": "1200347",
        ///         "codeType": "default",
        ///         "version": 2,
        ///         "codeState": "Redeemed",
        ///         "customCodeInfo": {},
        ///         "customRedemptionInfo": {},
        ///         "redeemedByProfileId": "28adw61e-5634-49ae-9b09-f61930ce6e43",
        ///         "redeemedByProfileName": "",
        ///         "invalidationReason": null,
        ///         "createdAt": 0,
        ///         "activatedAt": null,
        ///         "redeemedAt": 1445875694706,
        ///         "invalidatedAt": null
        ///     }
        /// }
        /// </returns>
        public void RedeemCode(
            string in_scanCode,
            string in_codeType,
            string in_customRedemptionInfo,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.RedemptionCodeServiceScanCode.Value] = in_scanCode;
            data[OperationParam.RedemptionCodeServiceCodeType.Value] = in_codeType;

            if (Util.IsOptionalParameterValid(in_customRedemptionInfo))
            {
                Dictionary<string, object> customRedemptionInfo = JsonReader.Deserialize<Dictionary<string, object>>(in_customRedemptionInfo);
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
        ///     "status": 200,
        ///     "data": {
        ///         "codes": [
        ///             {
        ///                 "gameId": "123456",
        ///                 "scanCode": "999999",
        ///                 "codeType": "default",
        ///                 "version": 2,
        ///                 "codeState": "Redeemed",
        ///                 "customCodeInfo": {},
        ///                 "customRedemptionInfo": {},
        ///                 "redeemedByProfileId": "28d0745e-5634-49ae-9b09-f61930ce6e43",
        ///                 "redeemedByProfileName": "",
        ///                 "invalidationReason": null,
        ///                 "createdAt": 0,
        ///                 "activatedAt": null,
        ///                 "redeemedAt": 1445456503428,
        ///                 "invalidatedAt": null
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void GetRedeemedCodes(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.RedemptionCode, ServiceOperation.GetRedeemedCodes, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
