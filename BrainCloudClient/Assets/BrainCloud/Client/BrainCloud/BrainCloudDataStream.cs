// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

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
        /// <param name="eventName">Name of event</param>
        /// <param name="eventProperties">Properties of event</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// <param name="eventName">Name of event</param>
        /// <param name="eventProperties">Properties of event</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// <param name="eventName">Name of event</param>
        /// <param name="eventProperties">Properties of event</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
        /// Send crash report
        /// </summary>
        /// <param name="crashType">Identifies the crash category. Developer-defined, can be anything.</param>
        /// <param name="errorMsg">Short message describing the crash.</param>
        /// <param name="crashJson">Exception data.</param>
        /// <param name="crashLog">Client log up until the crash (if available.)</param>
        /// <param name="userName">Name provided by the user (if provided.)</param>
        /// <param name="userEmail">Email address to respond to (if provided.)</param>
        /// <param name="userNotes">Notes provided by the user (if provided.)</param>
        /// <param name="userSubmitted">User submitted flag.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



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
