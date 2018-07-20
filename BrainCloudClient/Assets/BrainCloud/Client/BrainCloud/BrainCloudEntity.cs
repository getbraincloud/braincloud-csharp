//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;

namespace BrainCloud
{
    public class BrainCloudEntity
    {
        private BrainCloudClient _client;

        public BrainCloudEntity(BrainCloudClient brainCloudClient)
        {
            _client = brainCloudClient;
        }

        /// <summary>
        /// Method creates a new entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Create
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// permissions which make the entity readable/writeable by only the user.
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
            string jsonEntityData,
            string jsonEntityAcl,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.EntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.EntityServiceAcl.Value] = acl;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.Create, data, callback);
            _client.SendRequest(serverCall);
        }


        /// <summary> Method returns all user entities that match the given type.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadByType
        /// </remarks>
        /// <param name="entityType">
        /// The entity type to search for
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
        public void GetEntitiesByType(
            string entityType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadByType, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Method updates a new entity on the server. This operation results in the entity
        /// data being completely replaced by the passed in JSON string.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Update
        /// </remarks>
        /// <param name="entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// permissions which make the entity readable/writeable by only the user.
        /// </param>
        /// <param name="version">
        /// Current version of the entity. If the version of the
        /// entity on the server does not match the version passed in, the
        /// server operation will fail. Use -1 to skip version checking.
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
            string entityType,
            string jsonEntityData,
            string jsonEntityAcl,
            int version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = entityId;
            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.EntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.EntityServiceAcl.Value] = acl;
            }
            data[OperationParam.EntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.Update, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method updates a shared entity owned by another user. This operation results in the entity
        /// data being completely replaced by the passed in JSON string.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - UpdateShared
        /// </remarks>
        /// <param name="entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="targetProfileId">
        /// The id of the entity's owner
        /// </param>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        public void UpdateSharedEntity(
            string entityId,
            string targetProfileId,
            string entityType,
            string jsonEntityData,
            int version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = entityId;
            data[OperationParam.EntityServiceTargetPlayerId.Value] = targetProfileId;

            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.EntityServiceData.Value] = entityData;

            data[OperationParam.EntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.UpdateShared, data, callback);
            _client.SendRequest(sc);
        }

        /*Unavailable for now...
         * public void UpdateEntityPartial(string entityId, string entityType, string jsonEntityData, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            // TODO: actually call the right method...
            UpdateEntity(entityId, entityType, jsonEntityData, success, failure, cbObject);
        }
         */

        /// <summary>
        /// Method deletes the given entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
            data[OperationParam.EntityServiceEntityId.Value] = entityId;
            data[OperationParam.EntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.Delete, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method updates a singleton entity on the server. This operation results in the entity
        /// data being completely replaced by the passed in JSON string. If the entity doesn't exist it is created.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Update_Singleton
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        public void UpdateSingleton(
            string entityType,
            string jsonEntityData,
            string jsonEntityAcl,
            int version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var entityData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityData);
            data[OperationParam.EntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(jsonEntityAcl))
            {
                var acl = JsonReader.Deserialize<Dictionary<string, object>>(jsonEntityAcl);
                data[OperationParam.EntityServiceAcl.Value] = acl;
            }

            data[OperationParam.EntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.UpdateSingleton, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Method deletes the given singleton on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        public void DeleteSingleton(
            string entityType,
            int version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityType.Value] = entityType;
            data[OperationParam.EntityServiceVersion.Value] = version;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.DeleteSingleton, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method to get a specific entity.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Read
        /// </remarks>
        /// <param name="entityId">
        /// The id of the entity
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
        public void GetEntity(
            string entityId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = entityId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.Read, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves a singleton entity on the server. If the entity doesn't exist, null is returned.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadSingleton
        /// </remarks>
        /// <param name="entityType">
        /// The entity type as defined by the user
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
        public void GetSingleton(
            string entityType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityType.Value] = entityType;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadSingleton, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a shared entity for the given profile and entity ID.
        /// An entity is shared if its ACL allows for the currently logged
        /// in user to read the data.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - READ_SHARED_ENTITY
        /// </remarks>
        /// <param name="profileId">
        /// The the profile ID of the user who owns the entity
        /// </param>
        /// <param name="entityId">
        /// The ID of the entity that will be retrieved
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
        public void GetSharedEntityForProfileId(
            string profileId,
            string entityId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceTargetPlayerId.Value] = profileId;
            data[OperationParam.EntityServiceEntityId.Value] = entityId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadSharedEntity, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all shared entities for the given profile id.
        /// An entity is shared if its ACL allows for the currently logged
        /// in user to read the data.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadShared
        /// </remarks>
        /// <param name="profileId">
        /// The profile id to retrieve shared entities for
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
        public void GetSharedEntitiesForProfileId(
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceTargetPlayerId.Value] = profileId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadShared, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method gets list of entities from the server base on type and/or where clause
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - GET_LIST
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
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetList, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets list of shared entities for the specified user based on type and/or where clause
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - GET_LIST
        /// </remarks>
        /// <param name="profileId">
        /// The profile ID to retrieve shared entities for
        /// </param>
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
        public void GetSharedEntitiesListForProfileId(
            string profileId,
            string whereJson,
            string orderByJson,
            int maxReturn,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            data[OperationParam.EntityServiceTargetPlayerId.Value] = profileId;
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
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.ReadSharedEntitiesList, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets a count of entities based on the where clause
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
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
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetListCount, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method uses a paging system to iterate through user entities.
        /// After retrieving a page of entities with this method,
        /// use GetPageOffset() to retrieve previous or next pages.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
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
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetPage, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Method to retrieve previous or next pages after having called
        /// the GetPage method.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
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
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetPageOffset, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Partial increment of entity data field items. Partial set of items incremented as specified.
        /// </summary>
        /// <remarks>
        /// Service Name - entity
        /// Service Operation - INCREMENT_USER_ENTITY_DATA
        /// </remarks>
        /// <param name="entityId">The entity to increment</param>
        /// <param name="jsonData">The subset of data to increment</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void IncrementUserEntityData(
            string entityId,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            data[OperationParam.EntityServiceEntityId.Value] = entityId;
            if (Util.IsOptionalParameterValid(jsonData))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.EntityServiceData.Value] = where;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.IncrementUserEntityData, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Partial increment of shared entity data field items. Partial set of items incremented as specified.
        /// </summary>
        /// <remarks>
        /// Service Name - entity
        /// Service Operation - INCREMENT_SHARED_USER_ENTITY_DATA
        /// </remarks>
        /// <param name="entityId">The entity to increment</param>
        /// <param name="targetProfileId">Profile ID of the entity owner</param>
        /// <param name="jsonData">The subset of data to increment</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void IncrementSharedUserEntityData(
            string entityId,
            string targetProfileId,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();

            data[OperationParam.EntityServiceEntityId.Value] = entityId;
            data[OperationParam.EntityServiceTargetPlayerId.Value] = targetProfileId;
            if (Util.IsOptionalParameterValid(jsonData))
            {
                var where = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.EntityServiceData.Value] = where;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.IncrementSharedUserEntityData, data, callback);
            _client.SendRequest(serverCall);
        }
    }
}
