//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudTime
    {
        private BrainCloudClient _client;

        public BrainCloudTime (BrainCloudClient inBrainCloudClient)
        {
            _client = inBrainCloudClient;
        }

        /// <summary>
        /// Method returns the server time in UTC. This is in UNIX millis time format.
        /// For instance 1396378241893 represents 2014-04-01 2:50:41.893 in GMT-4.
        /// </summary>
        /// <remarks>
        /// Service Name - Time
        /// Service Operation - Read
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
        public void ReadServerTime(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Time, ServiceOperation.Read, null, callback);
            _client.SendRequest(sc);
        }
    }
}

