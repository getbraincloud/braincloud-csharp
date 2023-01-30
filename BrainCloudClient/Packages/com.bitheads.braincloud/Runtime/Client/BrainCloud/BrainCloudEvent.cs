//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Internal;

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
        /// Delete a list of events out of the user's incoming mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - event
        /// Service Operation - DELETE_INCOMING_EVENTS
        /// </remarks>
        /// <param name="in_eventIds">
        /// Collection of event ids
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
        public void DeleteIncomingEvents(
            string[] in_eventIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceEvIds.Value] = in_eventIds;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteIncomingEvents, data, callback);
            _client.SendRequest(sc);
        }
        
        /// /// <summary>
        /// Delete any events older than the given date out of the user's incoming mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - event
        /// Service Operation - DELETE_INCOMING_EVENTS_OLDER_THAN
        /// </remarks>
        /// <param name="in_dateMillis">
        /// CreatedAt cut-off time whereby older events will be deleted (In UTC since Epoch)
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
        public void DeleteIncomingEventsOlderThan(
            int in_dateMillis,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceDateMillis.Value] = in_dateMillis;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteIncomingEventsOlderThan, data, callback);
            _client.SendRequest(sc);
        }
        
        /// <summary>
        /// Delete any events of the given type older than the given date out of the user's incoming mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - event
        /// Service Operation - DELETE_INCOMING_EVENTS_BY_TYPE_OLDER_THAN
        /// </remarks>
        /// <param name="in_eventId">
        /// The event id
        /// </param>
        /// <param name="in_dateMillis">
        /// CreatedAt cut-off time whereby older events will be deleted (In UTC since Epoch)
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
        public void DeleteIncomingEventsByTypeOlderThan(
            string in_eventId,
            int in_dateMillis,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceDateMillis.Value] = in_dateMillis;
            data[OperationParam.EventServiceEventType.Value] = in_eventId;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteIncomingEventsByTypeOlderThan, data, callback);
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
