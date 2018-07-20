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
    public class BrainCloudPlaybackStream
    {
        private BrainCloudClient _client;

        public BrainCloudPlaybackStream(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Starts a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - StartStream
        /// </remarks>
        /// <param name="targetPlayerId">
        /// The player to start a stream with
        /// </param>
        /// <param name="includeSharedData">
        /// Whether to include shared data in the stream
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
        public void StartStream(
            string targetPlayerId,
            bool includeSharedData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServiceTargetPlayerId.Value] = targetPlayerId;
            data[OperationParam.PlaybackStreamServiceIncludeSharedData.Value] = includeSharedData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.StartStream, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reads a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - ReadStream
        /// </remarks>
        /// <param name="playbackStreamId">
        /// Identifies the stream to read
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
        public void ReadStream(
            string playbackStreamId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.ReadStream, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Ends a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - EndStream
        /// </remarks>
        /// <param name="playbackStreamId">
        /// Identifies the stream to read
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
        public void EndStream(
            string playbackStreamId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.EndStream, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Deletes a stream
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - DeleteStream
        /// </remarks>
        /// <param name="playbackStreamId">
        /// Identifies the stream to read
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
        public void DeleteStream(
            string playbackStreamId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = playbackStreamId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.DeleteStream, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Adds a stream event
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - AddEvent
        /// </remarks>
        /// <param name="playbackStreamId">
        /// Identifies the stream to read
        /// </param>
        /// <param name="eventData">
        /// Describes the event
        /// </param>
        /// <param name="summary">
        /// Current summary data as of this event
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
        public void AddEvent(
            string playbackStreamId,
            string eventData,
            string summary,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServicePlaybackStreamId.Value] = playbackStreamId;

            if (Util.IsOptionalParameterValid(eventData))
            {
                Dictionary<string, object> jsonEventData = JsonReader.Deserialize<Dictionary<string, object>>(eventData);
                data[OperationParam.PlaybackStreamServiceEventData.Value] = jsonEventData;
            }

            if (Util.IsOptionalParameterValid(summary))
            {
                Dictionary<string, object> jsonSummary = JsonReader.Deserialize<Dictionary<string, object>>(summary);
                data[OperationParam.PlaybackStreamServiceSummary.Value] = jsonSummary;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.AddEvent, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets recent streams for initiating player
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - GetRecentSteamsForInitiatingPlayer
        /// </remarks>
        /// <param name="initiatingPlayerId">
        /// The player that started the stream
        /// </param>
        /// <param name="maxNumStreams">
        /// The player that started the stream
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
        public void GetRecentStreamsForInitiatingPlayer(
            string initiatingPlayerId,
            int maxNumStreams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServiceInitiatingPlayerId.Value] = initiatingPlayerId;
            data[OperationParam.PlaybackStreamServiceMaxNumberOfStreams.Value] = maxNumStreams;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.GetRecentStreamsForInitiatingPlayer, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets recent streams for target player
        /// </summary>
        /// <remarks>
        /// Service Name - PlaybackStream
        /// Service Operation - GetRecentSteamsForTargetPlayer
        /// </remarks>
        /// <param name="targetPlayerId">
        /// The player that started the stream
        /// </param>
        /// <param name="maxNumStreams">
        /// The player that started the stream
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
        public void GetRecentStreamsForTargetPlayer(
            string targetPlayerId,
            int maxNumStreams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlaybackStreamServiceTargetPlayerId.Value] = targetPlayerId;
            data[OperationParam.PlaybackStreamServiceMaxNumberOfStreams.Value] = maxNumStreams;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlaybackStream, ServiceOperation.GetRecentStreamsForTargetPlayer, data, callback);
            _client.SendRequest(sc);
        }
    }
}
