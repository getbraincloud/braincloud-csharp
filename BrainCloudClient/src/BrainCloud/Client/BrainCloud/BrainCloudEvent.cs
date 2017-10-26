//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudEvent
    {
        private BrainCloudClient _client;

        public BrainCloudEvent (BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Sends an event to the designated profile id with the attached json data.
        /// Any events that have been sent to a user will show up in their
        /// incoming event mailbox. If the recordLocally flag is set to true,
        /// a copy of this event (with the exact same event id) will be stored
        /// in the sending user's "sent" event mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - Event
        /// Service Operation - Send
        /// </remarks>
        /// <param name="toProfileId">
        /// The id of the user who is being sent the event
        /// </param>
        /// <param name="eventType">
        /// The user-defined type of the event.
        /// </param>
        /// <param name="jsonEventData">
        /// The user-defined data for this event encoded in JSON.
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
        public void SendEvent(
            string toProfileId,
            string eventType,
            string jsonEventData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.EventServiceSendToId.Value] = toProfileId;
            data[OperationParam.EventServiceSendEventType.Value] = eventType;

            if (Util.IsOptionalParameterValid(jsonEventData))
            {
                Dictionary<string, object> eventData = JsonReader.Deserialize<Dictionary<string, object>> (jsonEventData);
                data[OperationParam.EventServiceSendEventData.Value] = eventData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates an event in the user's incoming event mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - Event
        /// Service Operation - UpdateEventData
        /// </remarks>
        /// <param name="evId">
        /// The event id
        /// </param>
        /// <param name="jsonEventData">
        /// The user-defined data for this event encoded in JSON.
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
        public void UpdateIncomingEventData(
            string evId,
            string jsonEventData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EvId.Value] = evId;

            if (Util.IsOptionalParameterValid(jsonEventData))
            {
                Dictionary<string, object> eventData = JsonReader.Deserialize<Dictionary<string, object>> (jsonEventData);
                data[OperationParam.EventServiceUpdateEventDataData.Value] = eventData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.UpdateEventData, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Delete an event out of the user's incoming mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - Event
        /// Service Operation - DeleteIncoming
        /// </remarks>
        /// <param name="evId">
        /// The event id
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
        public void DeleteIncomingEvent(
            string evId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EvId.Value] = evId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteIncoming, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Get the events currently queued for the user.
        /// </summary>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void GetEvents(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.GetEvents, data, callback);
            _client.SendRequest(sc);
        }
    }
}
