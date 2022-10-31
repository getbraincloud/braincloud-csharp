using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.JsonFx.Json;

namespace BrainCloud
{
    public class BrainCloudBlockchain
    {
        private BrainCloudClient _client;

        public BrainCloudBlockchain(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Retrieves the blockchain items owned by the caller.
        /// </summary>
        public void GetBlockchainItems(
            string in_integrationID = "default", 
            string in_contextJson = "{}",
            SuccessCallback in_success = null, 
            FailureCallback in_failure = null, 
            object in_cbObject = null)
        {
            var context = JsonReader.Deserialize<Dictionary<string, object>>(in_contextJson);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.BlockChainIntegrationId.Value] = in_integrationID;
            data[OperationParam.BlockChainContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.BlockChain, ServiceOperation.GetBlockchainItems, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the uniqs owned by the caller.
        /// </summary>
        public void GetUniqs(
            string in_integrationID = "default",
            string in_contextJson = "",
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            var context = JsonReader.Deserialize<Dictionary<string, object>>(in_contextJson);
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.BlockChainIntegrationId.Value] = in_integrationID;
            data[OperationParam.BlockChainContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.BlockChain, ServiceOperation.GetUniqs, data, callback);
            _client.SendRequest(sc);
        }
    }
}