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
    public class BrainCloudGlobalApp
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudGlobalApp (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Method reads all the global properties of the game
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalApp
        /// Service Operation - ReadProperties
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON describing the global properties:
        /// {
        ///   "status":200,
        ///   "data": {
        ///     "pName": {
        ///       "name": "pName",
        ///	      "description": "pValue",
        ///	      "value": "pDescription"
        ///	    }
        ///   }
        /// }
        /// </returns>
        public void ReadProperties(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalApp, ServiceOperation.ReadProperties, null, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

    }
}
