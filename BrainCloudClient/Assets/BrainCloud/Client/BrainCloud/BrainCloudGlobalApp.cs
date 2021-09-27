//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
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

        public BrainCloudGlobalApp (BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method reads all the global properties of the game
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalApp
        /// Service Operation - ReadProperties
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// Returns a list of properties, identified by the propertyNames provided.
        /// If a property from the list isn't found, it just isn't returned (no error).
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalApp
        /// Service Operation - ReadSelectedProperties
        /// </remarks>
        /// <param name="propertyNames">
        /// Specifies which properties to return
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
        /// If a category from the list isn't found, it just isn't returned (no error).
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalApp
        /// Service Operation - ReadPropertiesInCategories
        /// </remarks>
        /// <param name="categories">
        /// Specifies which categories to return
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
