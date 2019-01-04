//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudGlobalStatistics
    {
        private BrainCloudClient _client;
        public BrainCloudGlobalStatistics(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Method returns all of the global statistics.
        /// </summary>
        /// <remarks>
        /// Service Name - globalGameStatistics
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
        public void ReadAllGlobalStats(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GlobalStatistics, ServiceOperation.Read, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reads a subset of global statistics as defined by the input JSON.
        /// </summary>
        /// <remarks>
        /// Service Name - globalGameStatistics
        /// Service Operation - ReadSubset
        /// </remarks>
        /// <param name="globalStats">
        /// A list containing the statistics to read
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
        public void ReadGlobalStatsSubset(
            IList<string> globalStats,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStatisticsServiceStats.Value] = globalStats;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GlobalStatistics, ServiceOperation.ReadSubset, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method retrieves the global statistics for the given category.
        /// </summary>
        /// <remarks>
        /// Service Name - globalGameStatistics
        /// Service Operation - READ_FOR_CATEGORY
        /// </remarks>
        /// <param name="category">
        /// The global statistics category
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
        public void ReadGlobalStatsForCategory(
            string category,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GamificationServiceCategory.Value] = category;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GlobalStatistics, ServiceOperation.ReadForCategory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Atomically increment (or decrement) global statistics.
        /// Global statistics are defined through the brainCloud portal.
        /// </summary>
        /// <remarks>
        /// Service Name - globalGameStatistics
        /// Service Operation - UpdateIncrement
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
        public void IncrementGlobalStats(
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> stats = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
            data[OperationParam.PlayerStatisticsServiceStats.Value] = stats;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GlobalStatistics, ServiceOperation.UpdateIncrement, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Apply statistics grammar to a partial set of statistics.
        /// </summary>
        /// <remarks>
        /// Service Name - globalGameStatistics
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
            ServerCall sc = new ServerCall(ServiceName.GlobalStatistics, ServiceOperation.ProcessStatistics, data, callback);
            _client.SendRequest(sc);
        }
    }
}
