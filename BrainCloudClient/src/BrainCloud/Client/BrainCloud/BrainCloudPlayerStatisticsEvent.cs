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
    public class BrainCloudPlayerStatisticsEvent
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPlayerStatisticsEvent(BrainCloudClient in_brainCloud)
        {
            m_brainCloudClientRef = in_brainCloud;
        }

        [Obsolete("This has been deprecated. Use TriggerStatsEvent instead - removal after September 1 2017")]
        public void TriggerPlayerStatisticsEvent(
            string in_eventName,
            int in_eventMultiplier,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticEventServiceEventName.Value] = in_eventName;
            data[OperationParam.PlayerStatisticEventServiceEventMultiplier.Value] = in_eventMultiplier;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.Trigger, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Trigger an event server side that will increase the user statistics.
        /// This may cause one or more awards to be sent back to the user -
        /// could be achievements, experience, etc. Achievements will be sent by this
        /// client library to the appropriate awards service (Apple Game Center, etc).
        ///
        /// This mechanism supercedes the PlayerStatisticsService API methods, since
        /// PlayerStatisticsService API method only update the raw statistics without
        /// triggering the rewards.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatisticsEvent
        /// Service Operation - Trigger
        ///
        /// @see BrainCloudPlayerStatistics
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void TriggerStatsEvent(
            string in_eventName,
            int in_eventMultiplier,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticEventServiceEventName.Value] = in_eventName;
            data[OperationParam.PlayerStatisticEventServiceEventMultiplier.Value] = in_eventMultiplier;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.Trigger, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("This has been deprecated. Use TriggerStatsEvents instead - removal after September 1 2017")]
        public void TriggerPlayerStatisticsEvents(
            string in_jsonData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            object[] events = JsonReader.Deserialize<object[]>(in_jsonData);
            data[OperationParam.PlayerStatisticEventServiceEvents.Value] = events;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.TriggerMultiple, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// See documentation for TriggerStatsEvent for more
        /// documentation.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatisticsEvent
        /// Service Operation - TriggerMultiple
        /// </remarks>
        /// <param name="in_jsonData">
        /// in_jsonData
        ///   [
        ///     {
        ///       "eventName": "event1",
        ///       "eventMultiplier": 1
        ///     },
        ///     {
        ///       "eventName": "event2",
        ///       "eventMultiplier": 1
        ///     }
        ///   ]
        /// </param>
        public void TriggerStatsEvents(
            string in_jsonData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object> ();
            object[] events = JsonReader.Deserialize<object[]>(in_jsonData);
            data[OperationParam.PlayerStatisticEventServiceEvents.Value] = events;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.TriggerMultiple, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
