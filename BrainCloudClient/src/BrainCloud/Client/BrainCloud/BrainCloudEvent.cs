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
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudEvent (BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
        }

        [Obsolete("Use new method signature - removal after March 22 2016")]
        public void SendEvent(
            string toPlayerId,
            string eventType,
            string jsonEventData,
            bool recordLocally,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.EventServiceSendToId.Value] = toPlayerId;
            data[OperationParam.EventServiceSendEventType.Value] = eventType;

            if (Util.IsOptionalParameterValid(jsonEventData))
            {
                Dictionary<string, object> eventData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEventData);
                data[OperationParam.EventServiceSendEventData.Value] = eventData;
            }

            data[OperationParam.EventServiceSendRecordLocally.Value] = recordLocally;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends an event to the designated player id with the attached json data.
        /// Any events that have been sent to a player will show up in their
        /// incoming event mailbox. If the recordLocally flag is set to true,
        /// a copy of this event (with the exact same event id) will be stored
        /// in the sending player's "sent" event mailbox.
        /// </summary>
        /// <remarks>
        /// Service Name - Event
        /// Service Operation - Send
        /// </remarks>
        /// <param name="toPlayerId">
        /// The id of the player who is being sent the event
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
            string toPlayerId,
            string eventType,
            string jsonEventData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.EventServiceSendToId.Value] = toPlayerId;
            data[OperationParam.EventServiceSendEventType.Value] = eventType;

            if (Util.IsOptionalParameterValid(jsonEventData))
            {
                Dictionary<string, object> eventData = JsonReader.Deserialize<Dictionary<string, object>> (jsonEventData);
                data[OperationParam.EventServiceSendEventData.Value] = eventData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.Send, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Use new method signature - removal after March 22 2016")]
        public void UpdateIncomingEventData(
            string fromPlayerId,
            ulong eventId,
            string jsonEventData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceUpdateEventDataFromId.Value] = fromPlayerId;
            data[OperationParam.EventServiceUpdateEventDataEventId.Value] = eventId;

            if (Util.IsOptionalParameterValid(jsonEventData))
            {
                Dictionary<string, object> eventData = JsonReader.Deserialize<Dictionary<string, object>>(jsonEventData);
                data[OperationParam.EventServiceUpdateEventDataData.Value] = eventData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.UpdateEventData, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Updates an event in the player's incoming event mailbox.
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
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Use new method signature - removal after March 22 2016")]
        public void DeleteIncomingEvent(
            string fromPlayerId,
            ulong eventId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceDeleteIncomingFromId.Value] = fromPlayerId;
            data[OperationParam.EventServiceDeleteIncomingEventId.Value] = eventId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteIncoming, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Delete an event out of the player's incoming mailbox.
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
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Removal after March 22 2016")]
        public void DeleteSentEvent(
            string toPlayerId,
            ulong eventId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceDeleteSentToId.Value] = toPlayerId;
            data[OperationParam.EventServiceDeleteSentEventId.Value] = eventId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.DeleteSent, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Use new method signature - removal after March 22 2016")]
        public void GetEvents(
            bool includeIncomingEvents,
            bool includeSentEvents,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.EventServiceIncludeIncomingEvents.Value] = includeIncomingEvents;
            data[OperationParam.EventServiceIncludeSentEvents.Value] = includeSentEvents;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Event, ServiceOperation.GetEvents, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Get the events currently queued for the player.
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
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
