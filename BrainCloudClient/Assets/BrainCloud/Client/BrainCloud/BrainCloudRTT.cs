//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;
using System;

namespace BrainCloud
{
    public class BrainCloudRTT
    {
        /// <summary>
        /// 
        /// </summary>
        public BrainCloudRTT(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Requests the event server address
        /// </summary>
        public void RequestClientConnection(  SuccessCallback success = null, FailureCallback failure = null,  object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.RTTRegistration, ServiceOperation.RequestClientConnection, null, callback);
                m_clientRef.SendRequest(sc);
        }

        #region private
        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion

    }
}
