//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

    public class BrainCloudItemCatalog
    {
        private BrainCloudClient _client;

        public BrainCloudItemCatalog(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Reads an existing item definition from the 
        ///server, with language fields limited to the
        /// current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - ItemCatalog
        /// Service Operation - GetCatalogItemDefinition
        /// </remarks>
        /// <param name="defId">
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
        public void GetCatalogItemDefinition(
        string defId,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ItemCatalogServiceDefId.Value] = defId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.ItemCatalog, ServiceOperation.GetCatalogItemDefinition, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves page of catalog items from the server, 
        ///with language fields limited to the text for the 
        ///current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - ItemCatalog
        /// Service Operation - GetCatalogItemDefinition
        /// </remarks>
        /// <param name="context">
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
        public void GetCatalogItemsPage(
        string context,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            var contextData = JsonReader.Deserialize<Dictionary<string, object>>(context);
            data[OperationParam.ItemCatalogServiceContext.Value] = contextData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.ItemCatalog, ServiceOperation.GetCatalogItemsPage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the page of catalog items from the 
        ///server based on the encoded context and 
        ///specified page offset, with language fields 
        ///limited to the text for the current or default 
        ///language.
        /// </summary>
        /// <remarks>
        /// Service Name - ItemCatalog
        /// Service Operation - GetCatalogItemDefinition
        /// </remarks>
        /// <param name="context">
        /// </param>
        /// <param name="pageOffset">
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
        public void GetCatalogItemsPageOffset(
        string context,
        int pageOffset,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ItemCatalogServiceContext.Value] = context;
            data[OperationParam.ItemCatalogServicePageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.ItemCatalog, ServiceOperation.GetCatalogItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }
    }
}
