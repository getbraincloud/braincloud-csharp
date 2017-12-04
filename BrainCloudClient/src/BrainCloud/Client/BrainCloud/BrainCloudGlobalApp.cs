//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
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

    }
}
