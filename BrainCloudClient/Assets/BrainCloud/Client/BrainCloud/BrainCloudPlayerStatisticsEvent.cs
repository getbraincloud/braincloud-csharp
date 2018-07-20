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
        private BrainCloudClient _client;

        public BrainCloudPlayerStatisticsEvent(BrainCloudClient client)
        {
            _client = client;
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
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void TriggerStatsEvent(
            string eventName,
            int eventMultiplier,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticEventServiceEventName.Value] = eventName;
            data[OperationParam.PlayerStatisticEventServiceEventMultiplier.Value] = eventMultiplier;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.Trigger, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// See documentation for TriggerStatsEvent for more
        /// documentation.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatisticsEvent
        /// Service Operation - TriggerMultiple
        /// </remarks>
        /// <param name="jsonData">
        /// jsonData
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
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            object[] events = JsonReader.Deserialize<object[]>(jsonData);
            data[OperationParam.PlayerStatisticEventServiceEvents.Value] = events;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatisticsEvent, ServiceOperation.TriggerMultiple, data, callback);
            _client.SendRequest(sc);
        }
    }
}
