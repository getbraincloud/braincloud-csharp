// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    using BrainCloud.Internal;
    using System.Collections.Generic;

    public class BrainCloudCampaign
    {
        private BrainCloudClient _client;

        public BrainCloudCampaign(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns the list of campaigns the current player is participating in,
        /// providing campaign, campaign scenario, and participation details.
        /// </summary>
        /// <remarks>
        /// Service Name - campaign
        /// Service Operation - GET_MY_CAMPAIGNS
        /// </remarks>
        /// <param name="optionsJson">
        /// Optional parameters (reserved for future use).
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
        public void GetMyCampaigns(
            Dictionary<string, object> optionsJson = null,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (optionsJson != null && optionsJson.Count > 0)
            {
                data[OperationParam.CampaignOptionsJson] = optionsJson;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Campaign, ServiceOperation.GetMyCampaigns, data, callback);
            _client.SendRequest(sc);
        }
    }
}
