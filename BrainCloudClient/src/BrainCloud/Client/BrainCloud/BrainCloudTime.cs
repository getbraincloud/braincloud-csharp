//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudTime
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudTime (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Method returns the server time in UTC. This is in UNIX millis time format.
        /// For instance 1396378241893 represents 2014-04-01 2:50:41.893 in GMT-4.
        /// </summary>
        /// <remarks>
        /// Service Name - Time
        /// Service Operation - Read
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
        /// <returns> A JSON string such as:
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "server_time":1396378241893
        ///   }
        /// }
        /// </returns>
        public void ReadServerTime(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Time, ServiceOperation.Read, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

