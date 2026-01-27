// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

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
        /// Reads an existing item definition from the server, with language fields
        /// </summary>
        /// <remarks>
        /// Service Name - itemCatalog
        /// Service Operation - GET_CATALOG_ITEM_DEFINITION
        /// </remarks>
        /// <param name="defId">The identifier of the catalog item definition to retrieve</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
        /// Retrieve a page of catalog items from the server, with language fields
        /// </summary>
        /// <remarks>
        /// Service Name - itemCatalog
        /// Service Operation - GET_CATALOG_ITEMS_PAGE
        /// </remarks>
        /// <param name="context">The pagination context returned from a previous catalog page request</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
        /// Gets a page of catalog items from the server based on the encoded
        /// </summary>
        /// <remarks>
        /// Service Name - itemCatalog
        /// Service Operation - GET_CATALOG_ITEMS_PAGE_OFFSET
        /// </remarks>
        /// <param name="context">The pagination context returned from a previous catalog page request</param>
        /// <param name="pageOffset">The page offset relative to the current context</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

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
