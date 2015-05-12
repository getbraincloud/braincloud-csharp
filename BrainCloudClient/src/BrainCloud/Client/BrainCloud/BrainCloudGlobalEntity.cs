//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudGlobalEntity
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudGlobalEntity (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Method creates a new entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_timeToLive">
        /// Sets expiry time for entity if > 0
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "",
        ///      version : 1,
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      data :
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void CreateEntity(
            string in_entityType,
            long in_timeToLive,
            string in_jsonEntityAcl,
            string in_jsonEntityData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object> ();
            data[OperationParam.GlobalEntityServiceEntityType.Value] = in_entityType;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = in_timeToLive;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityData);
            data [OperationParam.GlobalEntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityAcl);
                data [OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall (ServiceName.GlobalEntity, ServiceOperation.Create, data, callback);
            m_brainCloudClientRef.SendRequest (serverCall);
        }

        /// <summary>
        /// Method creates a new entity on the server with an indexed id.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - CreateWithIndexedId
        /// </remarks>
        /// <param name="in_entityType">
        /// The entity type as defined by the user
        /// </param>
        /// <param name="in_indexedId">
        /// A secondary ID that will be indexed
        /// </param>
        /// <param name="in_timeToLive">
        /// Sets expiry time for entity if > 0
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json. A null acl implies default
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "hhjdyjghjd",
        ///      version : 1,
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      data :
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void CreateEntityWithIndexedId(
            string in_entityType,
            string in_indexedId,
            long in_timeToLive,
            string in_jsonEntityAcl,
            string in_jsonEntityData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityType.Value] = in_entityType;
            data[OperationParam.GlobalEntityServiceIndexedId.Value] = in_indexedId;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = in_timeToLive;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityData);
            data[OperationParam.GlobalEntityServiceData.Value] = entityData;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityAcl);
                data[OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.CreateWithIndexedId, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Update
        /// </remarks>
        /// <param name="in_entityId">
        /// The entity ID
        /// </param>
        /// <param name="in_version">
        /// The version of the entity to update
        /// </param>
        /// <param name="in_jsonEntityData">
        /// The entity's data as a json string
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "",
        ///      version : 1,
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      data :
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void UpdateEntity(
            string in_entityId,
            int in_version,
            string in_jsonEntityData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = in_version;

            Dictionary<string, object> entityData = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityData);
            data[OperationParam.GlobalEntityServiceData.Value] = entityData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Update, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity's Acl on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - UpdateAcl
        /// </remarks>
        /// <param name="in_entityId">
        /// The entity ID
        /// </param>
        /// <param name="in_version">
        /// The version of the entity to update
        /// </param>
        /// <param name="in_jsonEntityAcl">
        /// The entity's access control list as json.
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "",
        ///      version : 1,
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      data :
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void UpdateEntityAcl(
            string in_entityId,
            int in_version,
            string in_jsonEntityAcl,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = in_version;

            if (Util.IsOptionalParameterValid(in_jsonEntityAcl))
            {
                Dictionary<string, object> acl = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonEntityAcl);
                data[OperationParam.GlobalEntityServiceAcl.Value] = acl;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.UpdateAcl, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method updates an existing entity's time to live on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - UpdateTimeToLive
        /// </remarks>
        /// <param name="in_entityId">
        /// The entity ID
        /// </param>
        /// <param name="in_version">
        /// The version of the entity to update
        /// </param>
        /// <param name="in_timeToLive">
        /// Sets expiry time for entity if > 0
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "",
        ///      version : 1,
        ///      data :
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void UpdateEntityTimeToLive(
            string in_entityId,
            int in_version,
            long in_timeToLive,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = in_version;
            data[OperationParam.GlobalEntityServiceTimeToLive.Value] = in_timeToLive;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.UpdateTimeToLive, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method deletes an existing entity on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="in_entityId">
        /// The entity ID
        /// </param>
        /// <param name="in_version">
        /// The version of the entity to delete
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
        ///   "data":
        ///   {
        ///   }
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
            data[OperationParam.GlobalEntityServiceEntityId.Value] = in_entityId;
            data[OperationParam.GlobalEntityServiceVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Delete, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method reads an existing entity from the server.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - Read
        /// </remarks>
        /// <param name="in_entityId">
        /// The entity ID
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
        ///   "data":
        ///   {
        ///      gameId : "game",
        ///      entityId : "hucfshugvgvfhug",
        ///      ownerId : "ubfcbvfbsdvbb",
        ///      entityType : "",
        ///      entityIndexedId : "",
        ///      version : 1,
        ///      timeToLive: 0,
        ///      expiresAt : 9947395735758975,
        ///      data :
        ///      {
        ///          field : "value"
        ///      },
        ///      acl :
        ///      {
        ///          other, 1
        ///      },
        ///      createdAt : 74889479874,
        ///      updatedAt : 73847474897487
        ///
        ///   }
        /// }
        /// </returns>
        public void ReadEntity(
            string in_entityId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceEntityId.Value] = in_entityId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.Read, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets list of entities from the server base on type and/or where clause
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetList
        /// </remarks>
        /// <param name="in_where">
        /// Mongo style query string
        /// </param>
        /// <param name="in_orderBy">
        /// Sort order
        /// </param>
        /// <param name="in_maxReturn">
        /// The maximum number of entities to return
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
        ///   "data":
        ///   {
        ///      entities : [
        ///          {
        ///              gameId : "game",
        ///              entityId : "hucfshugvgvfhug",
        ///              ownerId : "ubfcbvfbsdvbb",
        ///              entityType : "",
        ///              entityIndexedId : "",
        ///              version : 1,
        ///              timeToLive: 0,
        ///              expiresAt : 9947395735758975,
        ///              data :
        ///              {
        ///                  field : "value"
        ///              },
        ///              acl :
        ///              {
        ///                  other, 1
        ///              },
        ///              createdAt : 74889479874,
        ///              updatedAt : 73847474897487
        ///          },
        ///          {
        ///              gameId : "game",
        ///              entityId : "dgdgg",
        ///              ownerId : "ubfcbvfbsdvbb",
        ///              entityType : "",
        ///              entityIndexedId : "",
        ///              version : 1,
        ///              timeToLive: 0,
        ///              expiresAt : 9947395735758975,
        ///              data :
        ///              {
        ///                  field : "value2"
        ///              },
        ///              acl :
        ///              {
        ///                  other, 1
        ///              },
        ///              createdAt : 74889479874,
        ///              updatedAt : 73847474897487
        ///          }
        ///       ]
        ///   }
        /// }
        /// </returns>
        public void GetList(
            string in_where,
            string in_orderBy,
            int in_maxReturn,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_where))
            {
                Dictionary<string, object> where = JsonReader.Deserialize<Dictionary<string, object>> (in_where);
                data[OperationParam.GlobalEntityServiceWhere.Value] = where;
            }
            if (Util.IsOptionalParameterValid(in_orderBy))
            {
                Dictionary<string, object> orderBy = JsonReader.Deserialize<Dictionary<string, object>> (in_orderBy);
                data[OperationParam.GlobalEntityServiceOrderBy.Value] = orderBy;
            }
            data[OperationParam.GlobalEntityServiceMaxReturn.Value] = in_maxReturn;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetList, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets list of entities from the server base on indexed id
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetListByIndexedId
        /// </remarks>
        /// <param name="in_entityIndexedId">
        /// The entity indexed Id
        /// </param>
        /// <param name="in_maxReturn">
        /// The maximum number of entities to return
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
        ///   "data":
        ///   {
        ///      entities : [
        ///          {
        ///              gameId : "game",
        ///              entityId : "hucfshugvgvfhug",
        ///              ownerId : "ubfcbvfbsdvbb",
        ///              entityType : "",
        ///              entityIndexedId : "",
        ///              version : 1,
        ///              timeToLive: 0,
        ///              expiresAt : 9947395735758975,
        ///              data :
        ///              {
        ///                  field : "value"
        ///              },
        ///              acl :
        ///              {
        ///                  other, 1
        ///              },
        ///              createdAt : 74889479874,
        ///              updatedAt : 73847474897487
        ///          },
        ///          {
        ///              gameId : "game",
        ///              entityId : "dgdgg",
        ///              ownerId : "ubfcbvfbsdvbb",
        ///              entityType : "",
        ///              entityIndexedId : "",
        ///              version : 1,
        ///              timeToLive: 0,
        ///              expiresAt : 9947395735758975,
        ///              data :
        ///              {
        ///                  field : "value2"
        ///              },
        ///              acl :
        ///              {
        ///                  other, 1
        ///              },
        ///              createdAt : 74889479874,
        ///              updatedAt : 73847474897487
        ///          }
        ///
        ///   }
        /// }
        /// </returns>
        public void GetListByIndexedId(
            string in_entityIndexedId,
            int in_maxReturn,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalEntityServiceIndexedId.Value] = in_entityIndexedId;
            data[OperationParam.GlobalEntityServiceMaxReturn.Value] = in_maxReturn;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetListByIndexedId, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Method gets a count of entities based on the where clause
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalEntity
        /// Service Operation - GetListCount
        /// </remarks>
        /// <param name="in_where">
        /// Mongo style query string
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
        ///   "data":
        ///   {
        ///      "entityListCount":5
        ///   }
        /// }
        /// </returns>
        public void GetListCount(
            string in_where,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_where))
            {
                Dictionary<string, object> where = JsonReader.Deserialize<Dictionary<string, object>> (in_where);
                data[OperationParam.GlobalEntityServiceWhere.Value] = where;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalEntity, ServiceOperation.GetListCount, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }
    }
}
