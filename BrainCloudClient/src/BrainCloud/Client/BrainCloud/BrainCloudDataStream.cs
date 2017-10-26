//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using BrainCloud.Internal;
using JsonFx.Json;

namespace BrainCloud
{
    public class BrainCloudDataStream
    {
        private BrainCloudClient _client;

        public BrainCloudDataStream(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates custom data stream page event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomPageEvent
        /// </remarks>
        /// <param name="eventName">
        /// The name of the event
        /// </param>
        /// <param name="jsonEventProperties">
        /// The properties of the event
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
        public void CustomPageEvent(
            string eventName,
            string jsonEventProperties,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = eventName;

            if (Util.IsOptionalParameterValid(jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomPageEvent, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Creates custom data stream screen event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomScreenEvent
        /// </remarks>
        /// <param name="eventName">
        /// The name of the event
        /// </param>
        /// <param name="jsonEventProperties">
        /// The properties of the event
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
        public void CustomScreenEvent(
            string eventName,
            string jsonEventProperties,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = eventName;
            
            if (Util.IsOptionalParameterValid(jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomScreenEvent, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Creates custom data stream track event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomTrackEvent
        /// </remarks>
        /// <param name="eventName">
        /// The name of the event
        /// </param>
        /// <param name="jsonEventProperties">
        /// The properties of the event
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
        public void CustomTrackEvent(
            string eventName,
            string jsonEventProperties,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = eventName;
            
            if (Util.IsOptionalParameterValid(jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomTrackEvent, data, callback);
            _client.SendRequest(serverCall);
        }

    }
}
