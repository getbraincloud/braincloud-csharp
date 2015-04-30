//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using LitJson;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudPlaybackStream
    {

        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPlaybackStream(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Starts a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - StartStream
        /// </remarks>
        /// <param name="in_targetPlayerId">
        /// The player to start a stream with
        /// </param>
        /// <param name="in_includeSharedData">
        /// Whether to include shared data in the stream
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
        ///    "status": 200,
        ///    "data": {
        ///     "playbackStreamId": "b8da4619-2ddc-4184-b654-cd11d12a3275",
        ///     "gameId": "10000",
        ///     "initiatingPlayerId": "198bcafcd-6e84-4c30-9f6e-3f9f016440c6",
        ///     "targetPlayerId": "a6943c74-6655-4245-8b2b-13bb908d3f88",
        ///     "status": "STARTED",
        ///     "summary": {},
        ///     "initialSharedData": {
        ///      "entities": [],
        ///      "statistics": {}
        ///     },
        ///     "events": [],
        ///     "createdAt": 1425481184200,
        ///     "updatedAt": 1425481184200
        ///    }
        ///   }
        /// </returns>
        public void StartStream(
            string in_targetPlayerId,
            bool in_includeSharedData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServiceTargetPlayerId.Value] = in_targetPlayerId;
            data[OperationParam.PlaybackStreamServiceIncludeSharedData.Value] = in_includeSharedData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.StartStream, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Reads a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - ReadStream
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// Identifies the stream to read
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
        ///    "status": 200,
        ///    "data": {
        ///     "playbackStreamId": "b8da4619-2ddc-4184-b654-cd11d12a3275",
        ///     "gameId": "10000",
        ///     "initiatingPlayerId": "198bcafcd-6e84-4c30-9f6e-3f9f016440c6",
        ///     "targetPlayerId": "a6943c74-6655-4245-8b2b-13bb908d3f88",
        ///     "status": "COMPLETE",
        ///     "summary": { total : 5 },
        ///     "initialSharedData": {
        ///      "entities": [ {entry : 3}, {entry : 2 }],
        ///      "statistics": {}
        ///     },
        ///     "events": [],
        ///     "createdAt": 1425481184200,
        ///     "updatedAt": 1425481184200
        ///    }
        ///   }
        /// </returns>
        public void ReadStream(
            string in_playbackStreamId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = in_playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.ReadStream, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Ends a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - EndStream
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// Identifies the stream to read
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
        ///    "status": 200,
        ///    "data": null
        /// }
        /// </returns>
        public void EndStream(
            string in_playbackStreamId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = in_playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.EndStream, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Deletes a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - DeleteStream
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// Identifies the stream to read
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
        ///    "status": 200,
        ///    "data": null
        /// }
        /// </returns>
        public void DeleteStream(
            string in_playbackStreamId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = in_playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.DeleteStream, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Adds a stream event
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - AddEvent
        /// </remarks>
        /// <param name="in_playbackStreamId">
        /// Identifies the stream to read
        /// </param>
        /// <param name="in_eventData">
        /// Describes the event
        /// </param>
        /// <param name="in_summary">
        /// Current summary data as of this event
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
        ///    "status": 200,
        ///    "data": null
        /// }
        /// </returns>
        public void AddEvent(
            string in_playbackStreamId,
            string in_eventData,
            string in_summary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = in_playbackStreamId;

            JsonData jsonEventData = JsonMapper.ToObject(in_eventData);
            data[OperationParam.PlaybackStreamServiceEventData.Value] = jsonEventData;

            JsonData jsonSummary = JsonMapper.ToObject(in_summary);
            data[OperationParam.PlaybackStreamServiceSummary.Value] = jsonSummary;


            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.AddEvent, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets stream summaries for initiating player
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - GetStreamSummariesForInitiatingPlayer
        /// </remarks>
        /// <param name="in_initiatingPlayerId">
        /// The player that started the stream
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
        ///  "status": 200,
        ///  "data": {
        ///   "streams": [
        ///    {
        ///     "playbackStreamId": "b8da4619-2ddc-4184-b654-cd11d12a3275",
        ///     "gameId": "10000",
        ///     "initiatingPlayerId": "198bcadb-6e84-4c30-9f6e-3f9f016440c6",
        ///     "targetPlayerId": "a6943c74-6636-4245-8b2b-13bb908d3f88",
        ///     "status": "IN_PROGRESS",
        ///     "summary": {
        ///      "total": 5
        ///     },
        ///     "createdAt": 1425481184200,
        ///     "updatedAt": 1425484485139
        ///    }
        ///   ]
        ///  }
        /// }
        /// </returns>
        public void GetStreamSummariesForInitiatingPlayer(
            string in_initiatingPlayerId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServiceInitiatingPlayerId.Value] = in_initiatingPlayerId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.GetStreamSummariesForInitiatingPlayer, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets stream summaries for target player
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - GetStreamSummariesForTargetPlayer
        /// </remarks>
        /// <param name="in_targetPlayerId">
        /// The player that started the stream
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
        ///  "status": 200,
        ///  "data": {
        ///   "streams": [
        ///    {
        ///     "playbackStreamId": "b8da4619-2ddc-4184-b654-cd11d12a3275",
        ///     "gameId": "10000",
        ///     "initiatingPlayerId": "198bcadb-6e84-4c30-9f6e-3f9f016440c6",
        ///     "targetPlayerId": "a6943c74-6636-4245-8b2b-13bb908d3f88",
        ///     "status": "IN_PROGRESS",
        ///     "summary": {
        ///      "total": 5
        ///     },
        ///     "createdAt": 1425481184200,
        ///     "updatedAt": 1425484485139
        ///    }
        ///   ]
        ///  }
        /// }
        /// </returns>
        public void GetStreamSummariesForTargetPlayer(
            string in_targetPlayerId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.PlaybackStreamServiceTargetPlayerId.Value] = in_targetPlayerId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.GetStreamSummariesForTargetPlayer, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
