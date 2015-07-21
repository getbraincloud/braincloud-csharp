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
    public class BrainCloudOneWayMatch
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudOneWayMatch(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Starts a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - StartMatch
        /// </remarks>
        /// <param name="in_otherPlayerId"> The player to start a match with </param>
        /// <param name="in_rangeDelta"> The range delta used for the initial match search </param>
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///    "status": 200,
        ///    "data": {
        ///        "playbackStreamId": "d18719db-9d02-2341-b62f-8e2f013369be",
        ///        "initiatingPlayerId": "d175f6ac-9221-4adc-aea4-f25f2426ff62",
        ///        "targetPlayerId": "07a0d23e-996b-4488-90ae-cb438342423a54",
        ///        "status": "STARTED",
        ///        "summary": {},
        ///        "initialSharedData": {
        ///            "entities": [],
        ///            "statistics": {}
        ///        },
        ///        "events": [],
        ///        "createdAt": 1437419496282,
        ///        "updatedAt": 1437419496282
        ///    }
        /// }
        /// </returns>
        public void StartMatch(
            string in_otherPlayerId,
            long in_rangeDelta,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlayerId.Value] = in_otherPlayerId;
            data[OperationParam.OfflineMatchServiceRangeDelta.Value] = in_rangeDelta;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.StartMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Cancels a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - CancelMatch
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// The playback stream id returned in the start match
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
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void CancelMatch(
            string in_playbackStreamId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlaybackStreamId.Value] = in_playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.CancelMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Completes a match
        /// </summary>
        /// <remarks>
        /// Service Name - OneWayMatch
        /// Service Operation - CompleteMatch
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// The playback stream id returned in the initial start match
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
        ///   "status": 200,
        ///   "data": null
        /// }
        /// </returns>
        public void CompleteMatch(
            string in_playbackStreamId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.OfflineMatchServicePlaybackStreamId.Value] = in_playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.OneWayMatch, ServiceOperation.CompleteMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

