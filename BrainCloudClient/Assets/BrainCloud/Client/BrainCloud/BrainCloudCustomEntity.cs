// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{
    using System;
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
        /// Creates new custom entity.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - CreateEntity
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="in_jsonEntityData">The entity's data as a json string</param>
        /// <param name="in_jsonEntityAcl">The entity's access control list as json. A null acl implies default permissions which make the entity readable/writeable by only the user. @param timeToLive @param isOwned</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void CreateEntity(
        string entityType,
        string dataJson,
        string acl,
        string timeToLive,
        bool isOwned,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceDataJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(dataJson);
            data[OperationParam.CustomEntityServiceAcl.Value] = JsonReader.Deserialize<Dictionary<string, object>>(acl);
            data[OperationParam.CustomEntityServiceTimeToLive.Value] = timeToLive;
            data[OperationParam.CustomEntityServiceIsOwned.Value] = isOwned;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.CreateCustomEntity, data, callback);
            _client.SendRequest(sc);
        }

        public void GetEntityPage(
        string entityType,
        string jsonContext,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {

            var context = JsonReader.Deserialize<Dictionary<string, object>>(jsonContext);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetEntityPage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the page of custom entities from the server based on the encoded context and specified page offset.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetEntityPageOffset
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_context @param in_pageOffset</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetEntityPageOffset(
        string entityType,
        string context,
        int pageOffset,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceContext.Value] = context;
            data[OperationParam.CustomEntityServicePageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCustomEntityPageOffset, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reads the specified custom entity from the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - ReadEntity
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="in_entityId">The entity id as defined by the system</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ReadEntity(
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
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.ReadCustomEntity, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Increments fields on the specified custom entity owned by the user on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - IncrementData
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="in_entityId">The entity id as defined by the system</param>
        /// <param name="in_fieldsJson">Specific fields, as JSON, within entity's custom data, with respective increment amount.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void IncrementData(
        string entityType,
        string entityId,
        string fieldsJson,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.IncrementData, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Increments the specified fields, of the singleton owned by the user, by the specified amount within the custom entity data on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - IncrementSingletonData
        /// </remarks>
        /// <param name="in_entityType">The type of custom entity being updated.</param>
        /// <param name="in_fieldsJson">Specific fields, as JSON, within entity's custom data, with respective increment amount.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void IncrementSingletonData(
            string entityType,
            string fieldsJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.IncrementSingletonData, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Replaces the specified custom entity's data, and optionally updates the acl and expiry, on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateEntity
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_entityId @param in_version</param>
        /// <param name="in_jsonEntityData">The entity's data as a json string</param>
        /// <param name="in_jsonEntityAcl">The entity's access control list as json. A null acl implies default permissions which make the entity readable/writeable by only the user. @param timeToLive</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateEntity(
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
            data[OperationParam.CustomEntityServiceTimeToLive.Value] = timeToLive;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateCustomEntity, data, callback);
            _client.SendRequest(sc);
        }



        /// <summary>
        /// Replaces the specified custom entity's data, and optionally updates the acl and expiry, on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateEntityFields
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_entityId @param in_version @param in_fieldsJson</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateEntityFields(
        string entityType,
        string entityId,
        int version,
        string fieldsJson,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson); ;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateCustomEntityFields, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// For sharded custom collection entities. Sets the specified fields within custom entity data on the server, enforcing ownership/ACL permissions.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateEntityFieldsSharded
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_entityId @param in_version @param in_fieldsJson</param>
        /// <param name="in_shardKeyJson">The shard key field(s) and value(s), as JSON, applicable to the entity being updated.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateEntityFieldsSharded(
            string entityType,
            string entityId,
            int version,
            string fieldsJson,
            string shardKeyJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson); ;
            data[OperationParam.CustomEntityServiceShardKeyJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(shardKeyJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateCustomEntityFieldsShards, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// deletes entities based on the delete criteria.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - DeleteEntities
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="in_deleteCriteria">Json string of criteria wanted for deletion</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void DeleteEntities(
        string entityType,
        string deleteCriteria,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceDeleteCriteria.Value] = JsonReader.Deserialize<Dictionary<string, object>>(deleteCriteria); ;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.DeleteEntities, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Deletes the specified custom entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetCount
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_whereJson</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetCount(
        string entityType,
        string whereJson,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceWhereJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(whereJson); ;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCount, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Deletes the specified custom entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - DeleteEntity
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="in_jsonEntityData">The entity's data as a json string @param version</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void DeleteEntity(
        string entityType,
        string entityId,
        int version,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceEntityId.Value] = entityId;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.DeleteCustomEntity, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Service Name - CustomEntity
        /// </summary>
        /// <remarks>
        /// Service Operation - GetRandomEntitiesMatching
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_whereJson @param in_maxReturn</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetRandomEntitiesMatching(
        string entityType,
        string whereJson,
        int maxReturn,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceWhereJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(whereJson);
            data[OperationParam.CustomEntityServiceMaxReturn.Value] = maxReturn;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetRandomEntitiesMatching, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Deletes the specified custom entity singleton, owned by the session's user,
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - DeleteSingleton
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_version</param>

        public void DeleteSingleton(
        string entityType,
        int version,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.DeleteSingleton, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Reads the custom entity singleton owned by the session's user.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - ReadSingleton
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ReadSingleton(
        string entityType,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.ReadSingleton, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Partially updates the data, of the singleton owned by the user for the specified custom entity type,
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateSingletonFields
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_version @param in_fieldsJson</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateSingletonFields(
        string entityType,
        int version,
        string fieldsJson,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateSingletonFields, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates the singleton owned by the user for the specified custom entity type on the server,
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateSingleton
        /// </remarks>
        /// <param name="in_entityType">The entity type as defined by the user @param in_version @param in_dataJson @param in_acl @param in_timeToLive</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateSingleton(
        string entityType,
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
            data[OperationParam.CustomEntityServiceVersion.Value] = version;
            data[OperationParam.CustomEntityServiceDataJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(dataJson);
            data[OperationParam.CustomEntityServiceAcl.Value] = JsonReader.Deserialize<Dictionary<string, object>>(acl);
            data[OperationParam.CustomEntityServiceTimeToLive.Value] = timeToLive;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateSingleton, data, callback);
            _client.SendRequest(sc);
        }
    }
}
