// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using BrainCloud.JsonFx.Json;
    using BrainCloud.Internal;

    public class BrainCloudGlobalApp
    {
        private BrainCloudClient _client;

        public BrainCloudGlobalApp(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Read game's global properties
        /// </summary>
        /// <remarks>
        /// Service Name - globalApp
        /// Service Operation - READ_PROPERTIES
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void ReadProperties(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalApp, ServiceOperation.ReadProperties, null, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Returns a list of properties, identified by the property names provided.
        /// </summary>
        /// <remarks>
        /// Service Name - globalApp
        /// Service Operation - READ_SELECTED_PROPERTIES
        /// </remarks>
        /// <param name="propertyNames">Specifies which properties to return</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void ReadSelectedProperties(
            string[] propertyNames,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalAppPropertyNames.Value] = propertyNames;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalApp, ServiceOperation.ReadSelectedProperties, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Returns a list of properties, identified by the categories provided.
        /// </summary>
        /// <remarks>
        /// Service Name - globalApp
        /// Service Operation - READ_PROPERTIES_CATEGORIES
        /// </remarks>
        /// <param name="categories">Specifies which category to return</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void ReadPropertiesInCategories(
            string[] categories,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalAppCategories.Value] = categories;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalApp, ServiceOperation.ReadPropertiesInCategories, data, callback);
            _client.SendRequest(serverCall);
        }
    }
}
