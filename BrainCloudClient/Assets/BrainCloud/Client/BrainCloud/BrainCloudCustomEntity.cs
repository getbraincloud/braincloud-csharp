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

    public class BrainCloudCustomEntity
    {
        private BrainCloudClient _client;

        public BrainCloudCustomEntity(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a Custom Entity
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - CreateCustomEntity
        /// </remarks>
        /// <param name="entityType">
        /// The Entity Type
        /// </param>
        /// <param name="dataJson">
        /// The entity data
        /// </param>
        /// <param name="acl">
        /// 
        /// <param name="timeToLive">
        /// The Entity Type
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
        public void CreateCustomEntity(
        string entityType,
        string dataJson,
        string acl,
        string timeToLive,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceDataJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(dataJson);
            data[OperationParam.CustomEntityServiceAcl.Value] = JsonReader.Deserialize<Dictionary<string, object>>(acl); 
            data[OperationParam.CustomEntityServiceTimeToLive.Value] = timeToLive;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemDefinition, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves page of custom entity from the server, 
        ///with language fields limited to the text for the 
        ///current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetCustomEntityPage
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="rowsPerPage">
        /// quantity of rows per page
        /// </param>
        /// <param name="searchJson">
        /// 
        /// </param>
        /// <param name="sortJson">
        /// 
        /// </param>
        /// <param name="doCount">
        /// 
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>//this good
        public void GetCustomEntityPage(
        string entityType,
        int rowsPerPage,
        string searchJson,
        string sortJson,
        bool doCount,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceRowsPerPage.Value] = rowsPerPage;
            data[OperationParam.CustomEntityServiceSearchJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(searchJson);
            data[OperationParam.CustomEntityServiceSortJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(sortJson);
            data[OperationParam.CustomEntityServiceDoCount.Value] = doCount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemsPage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the page of custom entity from the 
        ///server based on the encoded context and 
        ///specified page offset, with language fields 
        ///limited to the text for the current or default 
        ///language.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetCustomEntityPageOffset
        /// </remarks>
        /// <param name="entityType">
        /// </param>
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
        /// </param>this good
        public void GetCustomEntityPageOffset(
        string entityType,
        string context,
        int pageOffset,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value]= entityType;
            data[OperationParam.CustomEntityServiceContext.Value] = context;
            data[OperationParam.CustomEntityServicePageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - ReadCustomEntity
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="entityId">
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
        public void ReadCustomEntity(
        string entityType,
        string entityId,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }

                /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - Custom Entity
        /// Service Operation - UpdateCustomEntity
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="entityId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="dataJson">
        /// </param>
        /// <param name="acl">
        /// </param>
        /// <param name="timeToLive">
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
        public void UpdateCustomEntity(
        string entityType,
        string entityId,
        int version,
        string dataJson,
        string acl,
        string timeToLive,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;
            data[OperationParam.CustomEntityServiceDataJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(dataJson);
            data[OperationParam.CustomEntityServiceAcl.Value] = JsonReader.Deserialize<Dictionary<string, object>>(acl);
            data[OperationParam.CustomEntityServiceVersion.Value] = timeToLive;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }

        

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateCustomEntityFields
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
        public void UpdateCustomEntityFields(
        string context,
        int pageOffset,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceContext.Value] = context;
            data[OperationParam.CustomEntityServicePageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCatalogItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }
    }
}
