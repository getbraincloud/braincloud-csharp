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
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudDataStream(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Creates custom data stream page event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomPageEvent
        /// </remarks>
        /// <param name="in_eventName">
        /// The name of the event
        /// </param>
        /// <param name="in_jsonEventProperties">
        /// The properties of the event
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
        public void CustomPageEvent(
            string in_eventName,
            string in_jsonEventProperties,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = in_eventName;

            if (Util.IsOptionalParameterValid(in_jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomPageEvent, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Creates custom data stream screen event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomScreenEvent
        /// </remarks>
        /// <param name="in_eventName">
        /// The name of the event
        /// </param>
        /// <param name="in_jsonEventProperties">
        /// The properties of the event
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
        public void CustomScreenEvent(
            string in_eventName,
            string in_jsonEventProperties,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = in_eventName;
            
            if (Util.IsOptionalParameterValid(in_jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomScreenEvent, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

        /// <summary>
        /// Creates custom data stream track event
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - CustomTrackEvent
        /// </remarks>
        /// <param name="in_eventName">
        /// The name of the event
        /// </param>
        /// <param name="in_jsonEventProperties">
        /// The properties of the event
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
        public void CustomTrackEvent(
            string in_eventName,
            string in_jsonEventProperties,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamEventName.Value] = in_eventName;
            
            if (Util.IsOptionalParameterValid(in_jsonEventProperties))
            {
                Dictionary<string, object> eventProperties = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonEventProperties);
                data[OperationParam.DataStreamEventProperties.Value] = eventProperties;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.CustomTrackEvent, data, callback);
            m_brainCloudClientRef.SendRequest(serverCall);
        }

    }
}
