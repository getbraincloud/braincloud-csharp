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
    public class BrainCloudPlayerStatistics
    {
        private BrainCloudClient m_brainCloudClientRef;
        public BrainCloudPlayerStatistics(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Read all available player statistics.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Read
        /// </remarks>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "statistics":{
        ///       "minions":0,
        ///       "wood":50,
        ///       "pantelons":3,
        ///       "iron":0
        ///     }
        ///   }
        /// }
        /// </returns>
        public void ReadAllPlayerStats(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Read, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Reads a subset of player statistics as defined by the input JSON.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - ReadSubset
        /// </remarks>
        /// <param name="in_jsonData">
        /// The json data containing the subset of statistics to read:
        /// ex. [ "pantaloons", "minions" ]
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON with the subset of global statistics:
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "statisticsExceptions":{
        ///     },
        ///     "statistics":{
        ///       "wood":11,
        ///       "minions":1
        ///     }
        ///   }
        /// }
        /// </returns>
        public void ReadPlayerStatsSubset(
            string in_jsonData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            List<object> statsSubset = JsonReader.Deserialize<List<object>> (in_jsonData);
            data[OperationParam.PlayerStatisticsServiceStats.Value] = statsSubset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ReadSubset, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Reset all of the statistics for this player back to their initial value.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Reset
        ///
        /// </remarks>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        ///
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void ResetAllPlayerStats(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Reset, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Atomically increment (or decrement) player statistics.
        /// Any rewards that are triggered from player statistic increments
        /// will be considered. Player statistics are defined through the brainCloud portal.
        /// Note also that the "xpCapped" property is returned (true/false depending on whether
        /// the xp cap is turned on and whether the player has hit it).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="in_jsonData">
        /// The JSON encoded data to be sent to the server as follows:
        /// {
        ///   stat1: 10,
        ///   stat2: -5.5,
        /// }
        /// would increment stat1 by 10 and decrement stat2 by 5.5.
        /// For the full statistics grammer see the api.braincloudservers.com site.
        /// There are many more complex operations supported such as:
        /// {
        ///   stat1:INC_TO_LIMIT#9#30
        /// }
        /// which increments stat1 by 9 up to a limit of 30.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the new value of the statistics and any rewards that were triggered:
        ///  {
        ///    "status":200,
        ///    "data":{
        ///      "experiencePoints":10,
        ///      "xpCapped":false,
        ///      "rewardDetails":{
        ///        "xp":{
        ///          "experienceLevels":[
        ///            {
        ///              "level":1,
        ///              "reward":{
        ///                "currency":{
        ///                  "gold":1000
        ///                }
        ///              }
        ///            }
        ///          ]
        ///        }
        ///      },
        ///      "rewards":{
        ///        "experienceLevels":[
        ///          1
        ///        ],
        ///        "currency":{
        ///          "gold":1000
        ///        }
        ///      },
        ///      "experienceLevel":1,
        ///      "statistics":{
        ///        "LIVES":1
        ///      },
        ///      "currency":{
        ///        "gems":{
        ///          "purchased":0,
        ///          "balance":10,
        ///          "consumed":0,
        ///          "awarded":10
        ///        },
        ///        "gold":{
        ///          "purchased":0,
        ///          "balance":2000,
        ///          "consumed":0,
        ///          "awarded":2000
        ///        }
        ///      }
        ///    }
        ///  }
        /// </returns>
        public void IncrementPlayerStats(
            string in_jsonData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> statsData = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonData);
            data[OperationParam.PlayerStatisticsServiceStats.Value] = statsData;

            SuccessCallback successCallbacks = m_brainCloudClientRef.GetGamificationService().CheckForAchievementsToAward;
            if (in_success != null)
            {
                successCallbacks += in_success;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(successCallbacks, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Atomically increment (or decrement) player statistics.
        /// Any rewards that are triggered from player statistic increments
        /// will be considered. Player statistics are defined through the brainCloud portal.
        /// Note also that the "xpCapped" property is returned (true/false depending on whether
        /// the xp cap is turned on and whether the player has hit it).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="in_dictData">
        /// Stats name and their increments:
        /// {
        ///  {"stat1", 10},
        ///  {"stat1", -5}
        /// }
        ///
        /// would increment stat1 by 10 and decrement stat2 by 5.
        /// For the full statistics grammer see the api.braincloudservers.com site.
        /// There are many more complex operations supported such as:
        /// {
        ///   stat1:INC_TO_LIMIT#9#30
        /// }
        /// which increments stat1 by 9 up to a limit of 30.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the new value of the statistics and any rewards that were triggered:
        ///  {
        ///    "status":200,
        ///    "data":{
        ///      "experiencePoints":10,
        ///      "xpCapped":false,
        ///      "rewardDetails":{
        ///        "xp":{
        ///          "experienceLevels":[
        ///            {
        ///              "level":1,
        ///              "reward":{
        ///                "currency":{
        ///                  "gold":1000
        ///                }
        ///              }
        ///            }
        ///          ]
        ///        }
        ///      },
        ///      "rewards":{
        ///        "experienceLevels":[
        ///          1
        ///        ],
        ///        "currency":{
        ///          "gold":1000
        ///        }
        ///      },
        ///      "experienceLevel":1,
        ///      "statistics":{
        ///        "LIVES":1
        ///      },
        ///      "currency":{
        ///        "gems":{
        ///          "purchased":0,
        ///          "balance":10,
        ///          "consumed":0,
        ///          "awarded":10
        ///        },
        ///        "gold":{
        ///          "purchased":0,
        ///          "balance":2000,
        ///          "consumed":0,
        ///          "awarded":2000
        ///        }
        ///      }
        ///    }
        ///  }
        /// </returns>
        public void IncrementPlayerStats(
            Dictionary<string, object> in_dictData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsServiceStats.Value] = in_dictData;

            SuccessCallback successCallbacks = m_brainCloudClientRef.GetGamificationService().CheckForAchievementsToAward;
            if (in_success != null)
            {
                successCallbacks += in_success;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(successCallbacks, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns JSON representing the next experience level for the player.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - ReadNextXpLevel
        /// </remarks>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the next experience level for the player.
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "xp_level":{
        ///       "gameId":"com.bitheads.unityexample",
        ///       "numericLevel":2,
        ///       "experience":20,
        ///       "reward":{
        ///         "globalGameStatistics":null,
        ///         "experiencePoints":null,
        ///         "playerStatistics":null,
        ///         "achievement":null,
        ///         "currencies":{
        ///           "gems":10,
        ///           "gold":2000
        ///         }
        ///       },
        ///       "facebookAction":"",
        ///       "statusTitle":"Jester",
        ///       "key":{
        ///         "gameId":"com.bitheads.unityexample",
        ///         "numericLevel":2,
        ///         "primaryKey":true
        ///       }
        ///     }
        ///   }
        /// }
        /// </returns>
        public void GetNextExperienceLevel(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ReadNextXpLevel, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Increments the player's experience. If the player goes up a level,
        /// the new level details will be returned along with a list of rewards.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="in_xpValue">
        /// The amount to increase the player's experience by
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the player's .
        /// {
        ///   "status":200,
        ///   "data":{
        ///     "statisticsExceptions":{
        ///     },
        ///     "milestones":{
        ///     },
        ///     "experiencePoints":110,
        ///     "quests":{
        ///     },
        ///     "experienceLevel":1,
        ///     "statistics":{
        ///       "minions":0,
        ///       "wood":50,
        ///       "pantelons":3,
        ///       "iron":0
        ///     }
        ///   }
        /// }
        /// </returns>
        public void IncrementExperiencePoints(
            int in_xpValue,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsExperiencePoints.Value] = in_xpValue;

            SuccessCallback successCallbacks = m_brainCloudClientRef.GetGamificationService().CheckForAchievementsToAward;
            if (in_success != null)
            {
                successCallbacks += in_success;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(successCallbacks, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sets the player's experience to an absolute value. Note that this
        /// is simply a set and will not reward the player if their level changes
        /// as a result.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - SetXpPoints
        /// </remarks>
        /// <param name="in_xpValue">
        /// The amount to set the the player's experience to
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> The JSON returned in the callback is as follows.
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void SetExperiencePoints(
            int in_xpValue,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsExperiencePoints.Value] = in_xpValue;

            SuccessCallback successCallbacks = m_brainCloudClientRef.GetGamificationService().CheckForAchievementsToAward;
            if (in_success != null)
            {
                successCallbacks += in_success;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(successCallbacks, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.SetXpPoints, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

    }
}
