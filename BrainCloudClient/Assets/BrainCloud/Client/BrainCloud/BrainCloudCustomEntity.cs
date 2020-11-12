//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
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
        /// <param name="isOwned">
        /// The entity data
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
        [Obsolete("This has been deprecated use overload with 2 arguments entityType and context - removal after September 1 2021")]
        public void GetEntityPage(
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
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCustomEntityPage, data, callback);
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
        public void GetEntityPageOffset(
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
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCustomEntityPageOffset, data, callback);
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
        ///Increments the specified fields by the specified amount within custom entity data on the server, enforcing ownership/ACL permissions.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - IncrementData
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
            data[OperationParam.CustomEntityServiceFieldsJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fieldsJson);;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.UpdateCustomEntityFields, data, callback);
            _client.SendRequest(sc);
        }

                /// <summary>
        /// Deletes Entities based on the criteria passed in
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - DeleteEntities
        /// </remarks>
        /// <param name="entityType">
        /// The Entity Type
        /// </param>
        /// <param name="deleteCriteria">
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
        public void DeleteEntities(
        string entityType,
        string deleteCriteria,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceDeleteCriteria.Value] = JsonReader.Deserialize<Dictionary<string, object>>(deleteCriteria);;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.DeleteEntities, data, callback);
            _client.SendRequest(sc);
        }

                /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetCount
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="whereJson">
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
        public void GetCount(
        string entityType,
        string whereJson,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceWhereJson.Value] = JsonReader.Deserialize<Dictionary<string, object>>(whereJson);;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.GetCount, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - DeleteCustomEntity
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
        ///Gets a list of up to maxReturn randomly selected custom entities from the server 
        ///based on the entity type and where condition.
        /// </summary>
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - GetRandomEntitiesMatching
        /// </remarks>
        /// <param name="entityType">
        /// type of entities
        /// </param>
        /// <param name="whereJson">
        /// Mongo style query string
        /// </param>
        /// <param name="maxReturn">
        /// number of max returns
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
        ///Deletes the specified custom entity singleton, owned by the session's user, for the specified entity type, on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Custom Entity
        /// Service Operation - DeleteSingleton
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="version">
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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.CustomEntityServiceEntityType.Value] = entityType;
            data[OperationParam.CustomEntityServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.CustomEntity, ServiceOperation.DeleteSingleton, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        ///Reads the custom entity singleton owned by the session's user.
        /// </summary>
        /// <remarks>
        /// Service Name - Custom Entity
        /// Service Operation - ReadSingleton
        /// </remarks>
        /// <param name="entityType">
        /// </param>
        /// <param name="version">
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

        /// </summary>
        ///Partially updates the data, of the singleton owned by the user for the specified custom entity type, with the specified fields, on the server
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation - UpdateSingletonFields
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

        /// </summary>
        ///Updates the singleton owned by the user for the specified custom entity type on the server, creating the singleton if it does not exist. This operation results in the owned singleton's data being completely replaced by the passed in JSON object.
        /// <remarks>
        /// Service Name - CustomEntity
        /// Service Operation -UpdateSingleton
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
