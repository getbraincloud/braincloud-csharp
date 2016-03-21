﻿//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;

namespace BrainCloud
{
    public class BrainCloudEntity
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudEntity(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Method creates a new entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// permissions which make the entity readable/writeable by only the player.
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
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void CreateEntity(
            string in_entityType,
            string in_jsonEntityData,
            string in_jsonEntityAcl,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceCreateEntityType.Value] = in_entityType;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityData);
            data[OperationParam.EntityServiceCreateData.Value] = entityData;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityAcl);
                data[OperationParam.EntityServiceCreateAcl.Value] = acl;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.Create, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }


        /// <summary> Method returns all player entities that match the given type.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadByType
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type to search for
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
        /// <returns> JSON including the entities matching the given type
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "entities": [
        ///       {
        ///         "entityId": "113db68a-48ad-4fc9-9f44-5fd36fc6445f",
        ///         "entityType": "person",
        ///         "version": 1,
        ///         "data": {
        ///           "name": "john",
        ///           "age": 30
        ///         },
        ///         "acl": {
        ///           "other": 0
        ///         },
        ///         "createdAt": 1395943044322,
        ///         "updatedAt": 1395943044322
        ///       },
        ///       {
        ///         "entityId": "255db68a-48ad-4fc9-9f44-5fd36fc6445f",
        ///         "entityType": "person",
        ///         "version": 1,
        ///         "data": {
        ///           "name": "mary",
        ///           "age": 25
        ///         },
        ///         "acl": {
        ///           "other": 0
        ///         },
        ///         "createdAt": 1395943044322,
        ///         "updatedAt": 1395943044322
        ///       }
        ///     ]
        ///   }
        /// </returns>
        public void GetEntitiesByType(
            string in_entityType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceGetEntitiesByTypeEntityType.Value] = in_entityType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadByType, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Method updates a new entity on the server. This operation results in the entity
        /// data being completely replaced by the passed in JSON string.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Update
        /// </remarks>
        /// <param name="in_entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// permissions which make the entity readable/writeable by only the player.
        /// </param>
        /// <param name="in_version">
        /// Current version of the entity. If the version of the
        /// entity on the server does not match the version passed in, the
        /// server operation will fail. Use -1 to skip version checking.
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
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void UpdateEntity(
            string in_entityId,
            string in_entityType,
            string in_jsonEntityData,
            string in_jsonEntityAcl,
            int in_version,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.EntityServiceCreateEntityType.Value] = in_entityType;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityData);
            data[OperationParam.EntityServiceCreateData.Value] = entityData;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityAcl);
                data[OperationParam.EntityServiceCreateAcl.Value] = acl;
            }
            data[OperationParam.EntityServiceUpdateVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.Update, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method updates a shared entity owned by another player. This operation results in the entity
        /// data being completely replaced by the passed in JSON string.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - UpdateShared
        /// </remarks>
        /// <param name="in_entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="in_targetPlayerId">
        /// The id of the entity's owner
        /// </param>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="in_version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void UpdateSharedEntity(
            string in_entityId,
            string in_targetPlayerId,
            string in_entityType,
            string in_jsonEntityData,
            int in_version,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.EntityServiceCreateEntityTargetPlayerId.Value] = in_targetPlayerId;

            data[OperationParam.EntityServiceCreateEntityType.Value] = in_entityType;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityData);
            data[OperationParam.EntityServiceCreateData.Value] = entityData;

            data[OperationParam.EntityServiceUpdateVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.UpdateShared, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /*Unavailable for now...
         * public void UpdateEntityPartial(string entityId, string entityType, string jsonEntityData, SuccessCallback in_success, FailureCallback in_failure, object in_cbObject)
        {
            // TODO: actually call the right method...
            UpdateEntity(entityId, entityType, jsonEntityData, in_success, in_failure, in_cbObject);
        }
         */

        /// <summary>
        /// Method deletes the given entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="in_entityId">
        /// The id of the entity to update
        /// </param>
        /// <param name="in_version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        /// <returns> The JSON returned in the callback is as follows. Note that status 200 is returned
        /// whether or not the given entity was found on the server.
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void DeleteEntity(
            string in_entityId,
            int in_version,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.EntityServiceUpdateVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.Delete, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method updates a singleton entity on the server. This operation results in the entity
        /// data being completely replaced by the passed in JSON string. If the entity doesn't exist it is created.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Update_Singleton
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string.
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="in_version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        ///   "status":200,
        ///    "data" :   {
        ///         "entityId": "113db68a-48ad-4fc9-9f44-5fd36fc6445f",
        ///         "entityType": "settings",
        ///         "version": 1,
        ///         "data": {
        ///           "name": "john",
        ///           "age": 30
        ///         },
        ///         "createdAt": 1395943044322,
        ///         "updatedAt": 1395943044322
        ///       },
        /// }
        /// </returns>
        public void UpdateSingleton(
            string in_entityType,
            string in_jsonEntityData,
            string in_jsonEntityAcl,
            int in_version,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceCreateEntityType.Value] = in_entityType;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityData);
            data[OperationParam.EntityServiceCreateData.Value] = entityData;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEntityAcl);
                data[OperationParam.EntityServiceCreateAcl.Value] = acl;
            }

            data[OperationParam.EntityServiceUpdateVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.UpdateSingleton, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Method deletes the given singleton on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_version">
        /// Current version of the entity. If the version of the
        ///  entity on the server does not match the version passed in, the
        ///  server operation will fail. Use -1 to skip version checking.
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
        /// <returns> The JSON returned in the callback is as follows. Note that status 200 is returned
        /// whether or not the given entity was found on the server.
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void DeleteSingleton(
            string in_entityType,
            int in_version,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceCreateEntityType.Value] = in_entityType;
            data[OperationParam.EntityServiceUpdateVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.DeleteSingleton, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method to get a specific entity.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - Read
        /// </remarks>
        /// <param name="in_entityId">
        /// The id of the entity
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
        public void GetEntity(
            string in_entityId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceEntityId.Value] = in_entityId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.Read, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves a singleton entity on the server. If the entity doesn't exist, null is returned.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadSingleton
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
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
        public void GetSingleton(
            string in_entityType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServiceGetEntitiesByTypeEntityType.Value] = in_entityType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadSingleton, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a shared entity for the given player and entity ID.
        /// An entity is shared if its ACL allows for the currently logged
        /// in player to read the data.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadShared
        /// </remarks>
        /// <param name="in_playerId">
        /// The the profile ID of the player who owns the entity
        /// </param>
        /// <param name="in_entityId">
        /// The ID of the entity that will be retrieved
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
        /// 	"status": 200,
        /// 	"data": {
        /// 		"entityId": "544db68a-48ad-4fc9-9f44-5fd36fc6445f",
        /// 		"entityType": "publicInfo",
        /// 		"version": 1,
        /// 		"data": {
        /// 			"name": "john",
        /// 			"age": 30
        /// 		},
        /// 		"acl": {
        /// 			"other": 1
        /// 		},
        /// 		"createdAt": 1395943044322,
        /// 		"updatedAt": 1395943044322
        /// 	}
        /// }
        /// </returns>
        public void GetSharedEntityForPlayerId(
            string in_playerId,
            string in_entityId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServicePlayerId.Value] = in_playerId;
            data[OperationParam.EntityServiceEntityId.Value] = in_entityId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadSharedEntity, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns all shared entities for the given player id.
        /// An entity is shared if its ACL allows for the currently logged
        /// in player to read the data.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - ReadShared
        /// </remarks>
        /// <param name="in_playerId">
        /// The player id to retrieve shared entities for
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
        /// <returns> JSON including the shared entities for the given player id
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "entities": [
        ///       {
        ///         "entityId": "544db68a-48ad-4fc9-9f44-5fd36fc6445f",
        ///         "entityType": "publicInfo",
        ///         "version": 1,
        ///         "data":
        ///         {
        ///           "name": "john",
        ///           "age": 30
        ///         },
        ///         "acl":
        ///         {
        ///           "other": 1
        ///         },
        ///         "createdAt": 1395943044322,
        ///         "updatedAt": 1395943044322
        ///       }
        ///     ]
        ///   }
        /// }
        /// </returns>
        public void GetSharedEntitiesForPlayerId(
            string in_playerId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EntityServicePlayerId.Value] = in_playerId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Entity, ServiceOperation.ReadShared, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_jsonContext">The json context for the page request.
        /// See the portal appendix documentation for format</param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The callback object</param>
        /// 
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "results": {
        ///             "moreBefore": false,
        ///             "count": 200,
        ///             "items": [
        ///                 {
        ///                     "entityId": "00edfd8e-5028-45d5-95d4-b1869cf2afaa",
        ///                     "entityType": "testEntity",
        ///                     "version": 1,
        ///                     "data": {
        ///                         "testName": "Test Name 01"
        ///                     },
        ///                     "acl": {
        ///                         "other": 2
        ///                     },
        ///                     "createdAt": 1437505537168,
        ///                     "updatedAt": 1437505537168
        ///              }],
        ///              "page": 1,
        ///              "moreAfter": true
        ///         },
        ///         "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJlbnRpdHlUeXBlIjoiYnVpbGRpbmci
        ///              LCJnYW1lSWQiOiIxMDI4NyIsIiRvciI6W3sib3duZXJJZCI6Ijk5MjM4ZmFiLTd
        ///              12TItNDdiYy1iMDExLWJjMThhN2IyOWY3NiJ9LHsiYWNsLm90aGVyIjp7IiRuZS
        ///              I6MH19XX0sInNvcnRDcml0ZXJpYSI6eyJjcmVhdGVkQXQiOjEsInVwZGF0ZWRBd
        ///              CI6LTF9LCJwYWdpbmF0aW9uIjp7InJvd3NQZXJQYWdlIjo1MCwicGFnZU51bWJl
        ///              ciI6NH0sIm9wdGlvbnMiOm51bGx9"
        ///     }
        /// }
        /// </returns>
        public void GetPage(
            string in_jsonContext,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Dictionary<string, object> context = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonContext);
            data[OperationParam.GlobalEntityServiceContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetPage, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method to retrieve previous or next pages after having called
        /// the GetPage method.
        /// </summary>
        /// <remarks>
        /// Service Name - Entity
        /// Service Operation - GetPageOffset
        /// </remarks>
        /// <param name="in_context">
        /// The context string returned from the server from a previous call
        /// to GetPage() or GetPageOffset()
        /// </param>
        /// <param name="in_pageOffset">
        /// The positive or negative page offset to fetch. Uses the last page
        /// retrieved using the context string to determine a starting point.
        /// </param>
        /// <param name="in_success">The success callback</param>
        /// <param name="in_failure">The failure callback</param>
        /// <param name="in_cbObject">The callback object</param>
        /// 
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "results": {
        ///             "moreBefore": true,
        ///             "count": 200,
        ///             "items": [
        ///                 {
        ///                     "entityId": "00adfd8e-5028-45d5-95d4-b1869cf2afaa",
        ///                     "entityType": "testEntity",
        ///                     "version": 1,
        ///                     "data": {
        ///                         "testName": "Test Name 01"
        ///                     },
        ///                     "acl": {
        ///                         "other": 2
        ///                     },
        ///                     "createdAt": 1437505537168,
        ///                     "updatedAt": 1437505537168
        ///              }],
        ///              "page": 2,
        ///              "moreAfter": false
        ///         },
        ///         "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJlbnRpdHlUeXBlIjoiYnVpbGRpbmci
        ///              LCJnYW1lSWQiOiIxMDI4NyIsIiRvciI6W3sib3duZXJJZCI6Ijk5MjM4ZmFiLTd
        ///              12TItNDdiYy1iMDExLWJjMThhN2IyOWY3NiJ9LHsiYWNsLm90aGVyIjp7IiRuZS
        ///              I6MH19XX0sInNvcnRDcml0ZXJpYSI6eyJjcmVhdGVkQXQiOjEsInVwZGF0ZWRBd
        ///              CI6LTF9LCJwYWdpbmF0aW9uIjp7InJvd3NQZXJQYWdlIjo1MCwicGFnZU51bWJl
        ///              ciI6NH0sIm9wdGlvbnMiOm51bGx9"
        ///     }
        /// }
        /// </returns>
        public void GetPageOffset(
            string in_context,
            int in_pageOffset,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.GlobalEntityServiceContext.Value] = in_context;
            data[OperationParam.GlobalEntityServicePageOffset.Value] = in_pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.Entity, ServiceOperation.GetPageOffset, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }
    }
}
