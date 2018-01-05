//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;
using BrainCloud.Common;

namespace BrainCloud
{
    public class BrainCloudGlobalEntity
    {
        private BrainCloudClient _client;

        public BrainCloudGlobalEntity(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method creates a new entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Create
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="timeToLive">
        /// Sets expiry time for entity in milliseconds if > 0
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string
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
        public void CreateEntity(
            string entityType,
            long timeToLive,
            string jsonEntityAcl,
            string jsonEntityData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityType.Value] = entityType;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = timeToLive;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.GlobalEntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Create, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method creates a new entity on the server with an indexed id.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - CreateWithIndexedId
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="indexedId">
        /// A secondary ID that will be indexed
        /// </param>
        /// <param name="timeToLive">
        /// Sets expiry time for entity in milliseconds if > 0
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string
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
        public void CreateEntityWithIndexedId(
            string entityType,
            string indexedId,
            long timeToLive,
            string jsonEntityAcl,
            string jsonEntityData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityType.Value] = entityType;
            data[OperationParam.GlobalEntityServiceIndexedId.Value] = indexedId;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = timeToLive;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.GlobalEntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.CreateWithIndexedId, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Update
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity to update
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string
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
        public void UpdateEntity(
            string entityId,
            int version,
            string jsonEntityData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.GlobalEntityServiceData.Value] = entityData;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Update, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity's Acl on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - UpdateAcl
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity to update
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json.
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
        public void UpdateEntityAcl(
            string entityId,
            int version,
            string jsonEntityAcl,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.UpdateAcl, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity's time to live on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - UpdateTimeToLive
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity to update
        /// </param>
        /// <param name="timeToLive">
        /// Sets expiry time for entity in milliseconds if > 0
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
        public void UpdateEntityTimeToLive(
            string entityId,
            int version,
            long timeToLive,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = timeToLive;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.UpdateTimeToLive, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method deletes an existing entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity to delete
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
        public void DeleteEntity(
            string entityId,
            int version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Delete, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method reads an existing entity from the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Read
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
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
        public void ReadEntity(
            string entityId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Read, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets list of entities from the server base on type and/or where clause
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetList
        /// </remarks>
        /// <param name="whereJson">
        /// Mongo style query string
        /// </param>
        /// <param name="orderByJson">
        /// Sort order
        /// </param>
        /// <param name="maxReturn">
        /// The maximum number of entities to return
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
        public void GetList(
            string whereJson,
            string orderByJson,
            int maxReturn,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(whereJson))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(whereJson);
                data[OperationParam.GlobalEntityServiceWhere.Value] = where;
            }
            if (Util.IsOptionalParameterValid(orderByJson))
            {
                var orderBy = JsonReader.Deserialize<Dictionary<string, object>>(orderByJson);
                data[OperationParam.GlobalEntityServiceOrderBy.Value] = orderBy;
            }
            data[OperationParam.GlobalEntityServiceMaxReturn.Value] = maxReturn;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetList, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets list of entities from the server base on indexed id
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetListByIndexedId
        /// </remarks>
        /// <param name="entityIndexedId">
        /// The entity indexed Id
        /// </param>
        /// <param name="maxReturn">
        /// The maximum number of entities to return
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
        public void GetListByIndexedId(
            string entityIndexedId,
            int maxReturn,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceIndexedId.Value] = entityIndexedId;
            data[OperationParam.GlobalEntityServiceMaxReturn.Value] = maxReturn;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetListByIndexedId, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets a count of entities based on the where clause
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetListCount
        /// </remarks>
        /// <param name="whereJson">
        /// Mongo style query string
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
        public void GetListCount(
            string whereJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(whereJson))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(whereJson);
                data[OperationParam.GlobalEntityServiceWhere.Value] = where;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetListCount, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method uses a paging system to iterate through Global Entities.
        /// After retrieving a page of Global Entities with this method,
        /// use GetPageOffset() to retrieve previous or next pages.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetPage
        /// </remarks>
        /// <param name="jsonContext">The json context for the page request.
        /// See the portal appendix documentation for format</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        /// 
        public void GetPage(
            string jsonContext,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            var context = JsonReader.Deserialize<Dictionary<string, object>>(jsonContext);
            data[OperationParam.GlobalEntityServiceContext.Value] = context;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetPage, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method to retrieve previous or next pages after having called
        /// the GetPage method.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetPageOffset
        /// </remarks>
        /// <param name="context">
        /// The context string returned from the server from a previous call
        /// to GetPage() or GetPageOffset()
        /// </param>
        /// <param name="pageOffset">
        /// The positive or negative page offset to fetch. Uses the last page
        /// retrieved using the context string to determine a starting point.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        /// 
        public void GetPageOffset(
            string context,
            int pageOffset,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            data[OperationParam.GlobalEntityServiceContext.Value] = context;
            data[OperationParam.GlobalEntityServicePageOffset.Value] = pageOffset;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetPageOffset, data, callback);
            _client.SendRequest(serverCall);
        }


        /// <summary>
        /// Partial increment of global entity data field items. Partial set of items incremented as specified.
        /// </summary>
        /// <remarks>
        /// Service Name - globalEntity
        /// Service Operation - INCREMENT_GLOBAL_ENTITY_DATA
        /// </remarks>
        /// <param name="entityId">The entity to increment</param>
        /// <param name="jsonData">The subset of data to increment</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void IncrementGlobalEntityData(
            string entityId,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            if (Util.IsOptionalParameterValid(jsonData))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.GlobalEntityServiceData.Value] = where;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.IncrementGlobalEntityData, data, callback);
            _client.SendRequest(serverCall);
        }
        
        /// <summary>
        /// Gets a list of up to randomCount randomly selected entities from the server based on the where condition and specified maximum return count.
        /// </summary>
        /// <remarks>
        /// Service Name - globalEntity
        /// Service Operation - GET_RANDOM_ENTITIES_MATCHING
        /// </remarks>
        /// <param name="where">Mongo style query string</param>
        /// <param name="maxReturn">The maximum number of entities to return</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void GetRandomEntitiesMatching(
            string whereJson,
            int maxReturn,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(whereJson))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(whereJson);
                data[OperationParam.GlobalEntityServiceWhere.Value] = where;
            }
            
            data[OperationParam.GlobalEntityServiceMaxReturn.Value] = maxReturn;
            
            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetRandomEntitiesMatching, data, callback);
            _client.SendRequest(serverCall);
        }
        
        /// <summary>
        /// Method updates an existing entity's Owner and Acl on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - globalEntity
        /// Service Operation - UPDATE_ENTITY_OWNER_AND_ACL
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity
        /// </param>
        /// <param name="ownerId">
        /// The owner ID
        /// </param>
        /// <param name="acl">
        /// The entity's access control list
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
        public void UpdateEntityOwnerAndAcl(
            string entityId,
            long version,
            string ownerId,
            ACL acl,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;
            data[OperationParam.OwnerId.Value] = ownerId;
            data[OperationParam.GlobalEntityServiceAcl.Value] = JsonReader.Deserialize(acl.ToJsonString());

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.UpdateEntityOwnerAndAcl, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method clears the owner id of an existing entity and sets the Acl on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - globalEntity
        /// Service Operation - UPDATE_ENTITY_OWNER_AND_ACL
        /// </remarks>
        /// <param name="entityId">
        /// The entity ID
        /// </param>
        /// <param name="version">
        /// The version of the entity
        /// </param>
        /// <param name="acl">
        /// The entity's access control list
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
        public void MakeSystemEntity(
            string entityId,
            long version,
            ACL acl,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = version;
            data[OperationParam.GlobalEntityServiceAcl.Value] = JsonReader.Deserialize(acl.ToJsonString());

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.MakeSystemEntity, data, callback);
            _client.SendRequest(serverCall);
        }
    }
}
