//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System;
using System.Collections.Generic;
using System.Text;
using BrainCloud.Internal;
using BrainCloud.JsonFx.Json;

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

        /// <summary>
        /// Sends a crash report data
        /// </summary>
        /// <remarks>
        /// Service Name - DataStream
        /// Service Operation - SubmitCrashReport
        /// </remarks>
        /// <param name="crashType">
        /// The type of the crash
        /// </param>
        /// <param name="errorMsg">
        /// The error message
        /// </param>
        /// <param name="crashJson">
        /// The data from the error
        /// </param>
        /// <param name="crashLog">
        /// The crash logs
        /// </param>
        /// <param name="userName">
        /// The user email
        /// </param>
        /// <param name="userEmail">
        /// The user email
        /// </param>
        /// <param name="userNotes">
        /// The notes related to the user
        /// </param>
        /// <param name="userSubmitted">
        /// The bool passed by the user
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
        public void SubmitCrashReport(
            string crashType,
            string errorMsg,
            string crashJson,
            string crashLog,
            string userName,
            string userEmail,
            string userNotes,
            bool userSubmitted,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DataStreamCrashType.Value] = crashType;
            data[OperationParam.DataStreamErrorMsg.Value] = errorMsg;
            Dictionary<string, object> crashInfo = JsonReader.Deserialize<Dictionary<string, object>>(crashJson);
            data[OperationParam.DataStreamCrashInfo.Value] = crashInfo;
            data[OperationParam.DataStreamCrashLog.Value] = crashLog;
            data[OperationParam.DataStreamUserName.Value] = userName;
            data[OperationParam.DataStreamUserEmail.Value] = userEmail;
            data[OperationParam.DataStreamUserNotes.Value] = userNotes;
            data[OperationParam.DataStreamUserSubmitted.Value] = userSubmitted;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.DataStream, ServiceOperation.SubmitCrashReport, data, callback);
            _client.SendRequest(serverCall);
        }

    }
}
