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
    public class BrainCloudOneWayMatch
    {
        private BrainCloudClient _client;

        public BrainCloudOneWayMatch(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Starts a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - StartMatch
        /// </remarks>
        /// <param name="otherPlayerId"> The player to start a match with </param>
        /// <param name="rangeDelta"> The range delta used for the initial match search </param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void StartMatch(
            string otherPlayerId,
            long rangeDelta,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlayerId.Value] = otherPlayerId;
            data[OperationParam.OfflineMatchServiceRangeDelta.Value] = rangeDelta;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.StartMatch, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Cancels a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - CancelMatch
        /// </remarks>
        /// <param name="playbackStreamId">
        /// The playback stream id returned in the start match
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
        public void CancelMatch(
            string playbackStreamId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlaybackStreamId.Value] = playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.CancelMatch, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Completes a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - CompleteMatch
        /// </remarks>
        /// <param name="playbackStreamId">
        /// The playback stream id returned in the initial start match
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
        public void CompleteMatch(
            string playbackStreamId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlaybackStreamId.Value] = playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.CompleteMatch, data, callback);
            _client.SendRequest(sc);
        }
    }
}

