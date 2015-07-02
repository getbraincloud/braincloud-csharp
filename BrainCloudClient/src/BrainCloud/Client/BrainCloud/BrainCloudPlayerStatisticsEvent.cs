//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
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

        /// <summary>
        /// Trigger an event server side that will increase the players statistics.
        /// This may cause one or more awards to be sent back to the player -
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
        /// <returns> The JSON returned in the callback is as follows:
        ///     {
        ///      "status": 200,
        ///      "data": {
        ///        "experiencePoints": 25,
        ///        "achievementsGranted": [
        ///          "ach01"
        ///        ],
        ///        "currencyAwarded": {
        ///          "gems": {
        ///            "purchased": 0,
        ///            "balance": 10,
        ///            "consumed": 0,
        ///            "awarded": 10
        ///          },
        ///          "gold": {
        ///            "purchased": 0,
        ///            "balance": 2000,
        ///            "consumed": 0,
        ///            "awarded": 2000
        ///          }
        ///        },
        ///        "totalPlayerStatistics": {
        ///          "minions": 100,
        ///          "wood": 55,
        ///          "iron": 5
        ///        },
        ///        "experienceLevel": 2,
        ///        "experienceLevelsAwarded": [
        ///          2
        ///        ],
        ///        "playerStatisticsAwarded": {
        ///          "minions": 100,
        ///          "wood": 5,
        ///          "iron": 5
        ///        },
        ///        "experiencePointsAwarded": 15,
        ///        "currency": {
        ///          "gems": {
        ///            "purchased": 0,
        ///            "balance": 20,
        ///            "consumed": 0,
        ///            "awarded": 20
        ///          },
        ///          "gold": {
        ///            "purchased": 0,
        ///            "balance": 3000,
        ///            "consumed": 0,
        ///            "awarded": 3000
        ///          }
        ///        }
        ///      }
        ///    }
        /// </returns>
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
        /// See documentation for TriggerPlayerStatisticsEvent for more
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
        public void TriggerPlayerStatisticsEvents(
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
