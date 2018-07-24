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
    public class BrainCloudPlayerStatistics
    {
        private BrainCloudClient _client;
        public BrainCloudPlayerStatistics(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Read all available user statistics.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Read
        /// </remarks>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void ReadAllUserStats(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Read, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reads a subset of user statistics as defined by the input JSON.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - ReadSubset
        /// </remarks>
        /// <param name="userStats">
        /// A list containing the subset of statistics to read.
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void ReadUserStatsSubset(
            IList<string> playerStats,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsServiceStats.Value] = playerStats;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ReadSubset, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves the user statistics for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - READ_FOR_CATEGORY
        /// </remarks>
        /// <param name="category">
        /// The user statistics category
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
        public void ReadUserStatsForCategory(
            string category,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = category;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ReadForCategory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reset all of the statistics for this user back to their initial value.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Reset
        ///
        /// </remarks>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        ///
        /// </param>
        public void ResetAllUserStats(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Reset, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Atomically increment (or decrement) user statistics.
        /// Any rewards that are triggered from user statistic increments
        /// will be considered. User statistics are defined through the brainCloud portal.
        /// Note also that the "xpCapped" property is returned (true/false depending on whether
        /// the xp cap is turned on and whether the user has hit it).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="jsonData">
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
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void IncrementUserStats(
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> statsData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
            data[OperationParam.PlayerStatisticsServiceStats.Value] = statsData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Atomically increment (or decrement) user statistics.
        /// Any rewards that are triggered from user statistic increments
        /// will be considered. User statistics are defined through the brainCloud portal.
        /// Note also that the "xpCapped" property is returned (true/false depending on whether
        /// the xp cap is turned on and whether the user has hit it).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="dictData">
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
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void IncrementUserStats(
            Dictionary<string, object> dictData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsServiceStats.Value] = dictData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Apply statistics grammar to a partial set of statistics.
        /// </summary>
        /// <remarks>
        /// Service Name - playerStatistics
        /// Service Operation - PROCESS_STATISTICS
        /// </remarks>
        /// <param name="statisticsData">
        /// Example data to be passed to method:
        /// {
        ///     "DEAD_CATS": "RESET",
        ///     "LIVES_LEFT": "SET#9",
        ///     "MICE_KILLED": "INC#2",
        ///     "DOG_SCARE_BONUS_POINTS": "INC#10",
        ///     "TREES_CLIMBED": 1
        /// }
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void ProcessStatistics(
            Dictionary<string, object> statisticsData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsServiceStats.Value] = statisticsData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ProcessStatistics, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns JSON representing the next experience level for the user.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - ReadNextXpLevel
        /// </remarks>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void GetNextExperienceLevel(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.ReadNextXpLevel, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Increments the user's experience. If the user goes up a level,
        /// the new level details will be returned along with a list of rewards.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - Update
        /// </remarks>
        /// <param name="xpValue">
        /// The amount to increase the user's experience by
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void IncrementExperiencePoints(
            int xpValue,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsExperiencePoints.Value] = xpValue;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.Update, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sets the user's experience to an absolute value. Note that this
        /// is simply a set and will not reward the user if their level changes
        /// as a result.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerStatistics
        /// Service Operation - SetXpPoints
        /// </remarks>
        /// <param name="xpValue">
        /// The amount to set the the player's experience to
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SetExperiencePoints(
            int xpValue,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsExperiencePoints.Value] = xpValue;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerStatistics, ServiceOperation.SetXpPoints, data, callback);
            _client.SendRequest(sc);
        }

    }
}
