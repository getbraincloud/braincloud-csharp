//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudSocialLeaderboard
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudSocialLeaderboard (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        public enum SocialLeaderboardType
        {
            HIGH_VALUE,
            CUMULATIVE,
            LAST_VALUE,
            LOW_VALUE
        }

        public enum RotationType
        {
            NEVER,
            DAILY,
            WEEKLY,
            MONTHLY,
            YEARLY
        }

        public enum FetchType
        {
            HIGHEST_RANKED
        }
        private string FetchTypeToString(FetchType fetchType)
        {
            if (fetchType == FetchType.HIGHEST_RANKED)
            {
                return "HIGHEST_RANKED";
            }
            return "";
        }

        public enum SortOrder
        {

            HIGH_TO_LOW,
            LOW_TO_HIGH
        }

        private string SortOrderToString(SortOrder sortOrder)
        {
            if (sortOrder == SortOrder.HIGH_TO_LOW)
            {
                return "HIGH_TO_LOW";
            }
            if (sortOrder == SortOrder.LOW_TO_HIGH)
            {
                return "LOW_TO_HIGH";
            }
            return "";
        }

        /// <summary>
        /// Method returns the social leaderboard. A player's social leaderboard is
        /// comprised of players who are recognized as being your friend.
        /// For now, this applies solely to Facebook connected players who are
        /// friends with the logged in player (who also must be Facebook connected).
        /// In the future this will expand to other identification means (such as
        /// Game Centre, Google circles etc).
        ///
        /// Leaderboards entries contain the player's score and optionally, some user-defined
        /// data associated with the score. The currently logged in player will also
        /// be returned in the social leaderboard.
        ///
        /// Note: If no friends have played the game, the bestScore, createdAt, updatedAt
        /// will contain NULL.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetLeaderboard
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve
        /// </param>
        /// <param name="in_replaceName">
        /// If true, the currently logged in player's name will be replaced
        /// by the string "You".
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// Note that the friend summary data is returned for each record
        /// in the leaderboard.
        ///
        /// {
        ///   "status": 200,
        ///   "data": {
        ///     "social_leaderboard": [
        ///       {
        ///         "updatedAt": 1395840936348,
        ///         "pictureUrl": null,
        ///         "playerId": "26f8bb07-3e94-458b-8485-f9031c3a6ef1",
        ///         "createdAt": 1395840936348,
        ///         "name": "You",
        ///         "otherData": null,
        ///         "authenticationType": null,
        ///         "externalId": null,
        ///         "summaryFriendData": null,
        ///         "score": 20000
        ///       },
        ///       {
        ///         "updatedAt": 1395840936351,
        ///         "pictureUrl": null,
        ///         "playerId": "3ad8bc09-4a34-e324-1231-3b2c1c3a6bc6",
        ///         "createdAt": 1395840936351,
        ///         "name": "Jenny Goldsmith",
        ///         "otherData": null,
        ///         "authenticationType": null,
        ///         "externalId": null,
        ///         "summaryFriendData": null,
        ///         "score": 10000
        ///       }
        ///     ],
        ///     "timeBeforeReset": 588182412,
        ///     "server_time": 1395840957588
        ///   }
        /// }
        /// </returns>
        public void GetLeaderboard(
            string in_leaderboardId,
            bool in_replaceName,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = in_replaceName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetLeaderboard, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns the global leaderboard.
        ///
        /// Leaderboards entries contain the player's score and optionally, some user-defined
        /// data associated with the score.
        ///
        /// Note: If no leaderboard records exist then this method will empty list.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboard
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve
        /// </param>
        /// <param name="in_fetchType">
        /// The type of scores to return.
        /// </param>
        /// <param name="in_maxResults">
        /// The maximum number of scores returned.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// Note that the friend summary data is returned for each record
        /// in the leaderboard.
        ///
        /// {
        ///  "status": 200,
        ///  "data": {
        ///   "leaderboardId": "abc",
        ///   "social_leaderboard": [
        ///    {
        ///     "playerId": "8c86f306-73ea-4536-9c92-aba086064d2c",
        ///     "score": 10,
        ///     "data": {
        ///      "nickname": "batman"
        ///     },
        ///     "createdAt": 1433863814394,
        ///     "updatedAt": 1433863814394,
        ///     "index": 0,
        ///     "rank": 1,
        ///     "name": "",
        ///     "summaryFriendData": {
        ///      "xp": 12,
        ///      "favColour": "red"
        ///     }
        ///    },
        ///    {
        ///     "playerId": "ab21c0af-9d3e-4a81-b3c8-ddc1fb77d9a1",
        ///     "score": 8,
        ///     "data": {
        ///      "nickname": "robin"
        ///     },
        ///     "createdAt": 1433864253221,
        ///     "updatedAt": 1433864253221,
        ///     "index": 1,
        ///     "rank": 2,
        ///     "name": "",
        ///     "summaryFriendData": null
        ///    }
        ///   ],
        ///   "timeBeforeReset": 48136284,
        ///   "server_time": 1433864263716
        ///  }
        /// }
        /// </returns>
        [Obsolete("Use GetGlobalLeaderboardPage instead")]
        public void GetGlobalLeaderboard(
            string in_leaderboardId,
            FetchType in_fetchType,
            int in_maxResults ,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceFetchType.Value] = FetchTypeToString(in_fetchType);
            data[OperationParam.SocialLeaderboardServiceMaxResults.Value] = in_maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboard, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a page of global leaderboard results.
        ///
        /// Leaderboards entries contain the player's score and optionally, some user-defined
        /// data associated with the score.
        ///
        /// Note: This method allows the client to retrieve pages from within the global leaderboard list
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardPage
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="in_sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="in_startIndex">
        /// The index at which to start the page.
        /// </param>
        /// <param name="in_endIndex">
        /// The index at which to end the page.
        /// </param>
        /// <param name="in_includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// Note that the friend summary data is returned for each record
        /// in the leaderboard.
        ///
        /// {
        ///  "status": 200,
        ///  "data": {
        ///   "leaderboardId": "abc",
        ///   "moreBefore": false,
        ///   "timeBeforeReset": 48085996,
        ///   "leaderboard": [
        ///    {
        ///     "playerId": "8c86f306-73ea-4536-9c92-aba086064d2c",
        ///     "score": 10,
        ///     "data": {
        ///      "nickname": "batman"
        ///     },
        ///     "createdAt": 1433863814394,
        ///     "updatedAt": 1433863814394,
        ///     "index": 0,
        ///     "rank": 1,
        ///     "name": "",
        ///     "summaryFriendData": {
        ///      "xp": 12,
        ///      "favColour": "red"
        ///     }
        ///    },
        ///    {
        ///     "playerId": "ab21c0af-9d3e-4a81-b3c8-ddc1fb77d9a1",
        ///     "score": 8,
        ///     "data": {
        ///      "nickname": "robin"
        ///     },
        ///     "createdAt": 1433864253221,
        ///     "updatedAt": 1433864253221,
        ///     "index": 1,
        ///     "rank": 2,
        ///     "name": "",
        ///     "summaryFriendData": null
        ///    }
        ///   ],
        ///   "server_time": 1433864314004,
        ///   "moreAfter": false
        ///  }
        /// }
        /// </returns>
        public void GetGlobalLeaderboardPage(
            string in_leaderboardId,
            SortOrder in_sort,
            int in_startIndex,
            int in_endIndex,
            bool in_includeLeaderboardSize,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = SortOrderToString(in_sort);
            data[OperationParam.SocialLeaderboardServiceStartIndex.Value] = in_startIndex;
            data[OperationParam.SocialLeaderboardServiceEndIndex.Value] = in_endIndex;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = in_includeLeaderboardSize;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a page of global leaderboard results. By using a non-current version id, 
        /// the user can retrieve a historial leaderboard. See GetGlobalLeaderboardVersions method
        /// to retrieve the version id.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardPage
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="in_sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="in_startIndex">
        /// The index at which to start the page.
        /// </param>
        /// <param name="in_endIndex">
        /// The index at which to end the page.
        /// </param>
        /// <param name="in_includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="in_versionId">
        /// The historial version to retrieve.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// See GetGlobalLeaderboardPage documentation. Note that historial leaderboards do not
        /// include the 'timeBeforeReset' parameter.
        /// </returns>
        public void GetGlobalLeaderboardPageByVersion(
            string in_leaderboardId,
            SortOrder in_sort,
            int in_startIndex,
            int in_endIndex,
            bool in_includeLeaderboardSize,
            int in_versionId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = SortOrderToString(in_sort);
            data[OperationParam.SocialLeaderboardServiceStartIndex.Value] = in_startIndex;
            data[OperationParam.SocialLeaderboardServiceEndIndex.Value] = in_endIndex;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = in_includeLeaderboardSize;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = in_versionId;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a view of global leaderboard results that centers on the current player.
        ///
        /// Leaderboards entries contain the player's score and optionally, some user-defined
        /// data associated with the score.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardView
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="in_sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="in_beforeCount">
        /// The count of number of players before the current player to include.
        /// </param>
        /// <param name="in_afterCount">
        /// The count of number of players after the current player to include.
        /// </param>
        /// <param name="in_includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// Note that the friend summary data is returned for each record
        /// in the leaderboard.
        ///
        /// {
        ///  "status": 200,
        ///  "data": {
        ///   "leaderboardId": "abc",
        ///   "moreBefore": false,
        ///   "timeBeforeReset": 48085996,
        ///   "leaderboard": [
        ///    {
        ///     "playerId": "8c86f306-73ea-4536-9c92-aba086064d2c",
        ///     "score": 10,
        ///     "data": {
        ///      "nickname": "batman"
        ///     },
        ///     "createdAt": 1433863814394,
        ///     "updatedAt": 1433863814394,
        ///     "index": 0,
        ///     "rank": 1,
        ///     "name": "",
        ///     "summaryFriendData": {
        ///      "xp": 12,
        ///      "favColour": "red"
        ///     }
        ///    },
        ///    {
        ///     "playerId": "ab21c0af-9d3e-4a81-b3c8-ddc1fb77d9a1",
        ///     "score": 8,
        ///     "data": {
        ///      "nickname": "robin"
        ///     },
        ///     "createdAt": 1433864253221,
        ///     "updatedAt": 1433864253221,
        ///     "index": 1,
        ///     "rank": 2,
        ///     "name": "",
        ///     "summaryFriendData": null
        ///    }
        ///   ],
        ///   "server_time": 1433864314004,
        ///   "moreAfter": false
        ///  }
        /// }
        /// </returns>
        public void GetGlobalLeaderboardView(
            string in_leaderboardId,
            SortOrder in_sort,
            int in_beforeCount,
            int in_afterCount,
            bool in_includeLeaderboardSize,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = SortOrderToString(in_sort);
            data[OperationParam.SocialLeaderboardServiceBeforeCount.Value] = in_beforeCount;
            data[OperationParam.SocialLeaderboardServiceAfterCount.Value] = in_afterCount;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = in_includeLeaderboardSize;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardView, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a view of global leaderboard results that centers on the current player.
        /// By using a non-current version id, the user can retrieve a historial leaderboard.
        /// See GetGlobalLeaderboardVersions method to retrieve the version id.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardView
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="in_sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="in_beforeCount">
        /// The count of number of players before the current player to include.
        /// </param>
        /// <param name="in_afterCount">
        /// The count of number of players after the current player to include.
        /// </param>
        /// <param name="in_includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="in_versionId">
        /// The historial version to retrieve. Use -1 for current leaderboard.
        /// </param> 
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> JSON string representing the entries in the leaderboard.
        /// See GetGlobalLeaderboardView documentation. Note that historial leaderboards do not
        /// include the 'timeBeforeReset' parameter.
        /// </returns>
        public void GetGlobalLeaderboardViewByVersion(
            string in_leaderboardId,
            SortOrder in_sort,
            int in_beforeCount,
            int in_afterCount,
            bool in_includeLeaderboardSize,
            int in_versionId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = SortOrderToString(in_sort);
            data[OperationParam.SocialLeaderboardServiceBeforeCount.Value] = in_beforeCount;
            data[OperationParam.SocialLeaderboardServiceAfterCount.Value] = in_afterCount;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = in_includeLeaderboardSize;
            if (in_versionId != -1)
            {
                data[OperationParam.SocialLeaderboardServiceVersionId.Value] = in_versionId;
            }
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardView, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets the global leaderboard versions.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardVersions
        /// </remarks>
        /// <param name="in_leaderboardId">In_leaderboard identifier.</param>
        /// <param name="in_success">The success callback.</param>
        /// <param name="in_failure">The failure callback.</param>
        /// <param name="in_cbObject">The user object sent to the callback.</param>
        /// <returns> JSON string representing the leaderboard versions.
        /// {
        ///   "status": 200, 
        ///   "data": {
        ///     "leaderboardId":"L1",
        ///     "leaderboardType":"HIGH_VALUE",
        ///     "rotationType":"WEEKLY",
        ///     "retainedCount":2,
        ///     "versions":[  
        ///        {  
        ///         "versionId":27,
        ///         "startingAt":1434499200000,
        ///         "endingAt":1435104000000
        ///        },
        ///        {  
        ///         "versionId":26,
        ///         "startingAt":1433894400000,
        ///         "endingAt":1434499200000
        ///        }
        ///        ]
        ///     }
        ///   }
        /// }
        /// </returns>
        public void GetGlobalLeaderboardVersions(
            string in_leaderboardId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardVersions, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Post the players score to the given social leaderboard.
        /// You can optionally send a user-defined json string of data
        /// with the posted score. This string could include information
        /// relevant to the posted score.
        ///
        /// Note that the behaviour of posting a score can be modified in
        /// the brainCloud portal. By default, the server will only keep
        /// the player's best score.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - PostScore
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The leaderboard to post to
        /// </param>
        /// <param name="in_score">
        /// The score to post
        /// </param>
        /// <param name="in_data">
        /// Optional user-defined data to post with the score
        /// </param>
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
        /// {
        ///   "status":200,
        ///   "data": null
        /// }
        /// </returns>
        public void PostScoreToLeaderboard(
            string in_leaderboardId,
            long in_score,
            string in_jsonData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceScore.Value] = in_score;
            if (Util.IsOptionalParameterValid(in_jsonData))
            {
                Dictionary<string, object> customData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonData);
                data[OperationParam.SocialLeaderboardServiceData.Value] = customData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.PostScore, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Post the players score to the given social leaderboard.
        /// Pass leaderboard config data to dynamically create if necessary.
        /// You can optionally send a user-defined json string of data
        /// with the posted score. This string could include information
        /// relevant to the posted score.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - PostScoreDynamic
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The leaderboard to post to
        /// </param>
        /// <param name="in_score">
        /// The score to post
        /// </param>
        /// <param name="in_data">
        /// Optional user-defined data to post with the score
        /// </param>
        /// <param name="in_leaderboardType">
        /// leaderboard type
        /// </param>
        /// <param name="in_rotationType">
        /// Type of rotation
        /// </param>
        /// <param name="in_rotationReset">
        /// Date to reset the leaderboard (Date is converted to "dd-mm-yyyy hh:ss" format)
        /// </param>
        /// <param name="in_retainedCount">
        /// Hpw many rotations to keep
        /// </param>
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
        /// {
        ///   "status":200,
        ///   "data":{
        ///   }
        /// }
        /// </returns>
        public void PostScoreToDynamicLeaderboard(
            string in_leaderboardId,
            long in_score,
            string in_jsonData,
            SocialLeaderboardType in_leaderboardType,
            RotationType in_rotationType,
            DateTime? in_rotationReset,
            int in_retainedCount,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceScore.Value] = in_score;
            if (Util.IsOptionalParameterValid(in_jsonData))
            {
                Dictionary<string, object> customData = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonData);
                data[OperationParam.SocialLeaderboardServiceData.Value] = customData;
            }
            data[OperationParam.SocialLeaderboardServiceLeaderboardType.Value] = in_leaderboardType.ToString();
            data[OperationParam.SocialLeaderboardServiceRotationType.Value] = in_rotationType.ToString();

            if (in_rotationReset.HasValue)            
                data[OperationParam.SocialLeaderboardServiceRotationReset.Value] = in_rotationReset.Value.ToString("d-MM-yyyy HH:mm");
            
            data[OperationParam.SocialLeaderboardServiceRetainedCount.Value] = in_retainedCount;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.PostScoreDynamic, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Reset the player's score for the given social leaderboard id.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - Reset
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The leaderboard to post to
        /// </param>
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
        /// {
        ///   "status":200,
        ///   "data": null
        /// }
        /// </returns>
        public void ResetLeaderboardScore(
            string in_leaderboardId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.Reset, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// If a social leaderboard has been configured to reset periodically, each period
        /// can be considered to be a tournament. When the leaderboard resets, the tournament
        /// has ended and participants can be ranked based on their final scores.
        ///
        /// This API method will return the sorted leaderboard including:
        /// the player
        /// the game's pacers
        /// all friends who participated in the tournament
        ///
        /// This API method will return the leaderboard results for a particular
        /// tournament only once. If the method is called twice, the second call
        /// will yield an empty result.
        ///
        /// Note that if the leaderboard has not been configured to reset, the concept of a
        /// tournament does not apply.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetCompletedTournament
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The id of the leaderboard
        /// </param>
        /// <param name="in_replaceName">
        /// True if the player's name should be replaced with "You"
        /// </param>
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
        /// {
        ///   "status":200,
        ///   "data":{
        ///   //same as GetLeaderboard
        ///   }
        /// }
        /// </returns>
        public void GetCompletedLeaderboardTournament(
            string in_leaderboardId,
            bool in_replaceName,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = in_replaceName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetCompletedTournament, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// This method triggers a reward (via a player statistics event)
        /// to the currently logged in player for ranking at the
        /// completion of a tournament.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - RewardTournament
        /// </remarks>
        /// <param name="in_leaderboardId">
        /// The leaderboard the tournament was on
        /// </param>
        /// <param name="in_eventName">
        /// The player statistics event name to trigger
        /// </param>
        /// <param name="in_eventMultiplier">
        /// The multiplier to associate with the event
        /// </param>
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
        /// {
        ///   "status":200,
        ///   "data":{
        ///   }
        /// }
        /// </returns>
        public void TriggerSocialLeaderboardTournamentReward(
            string in_leaderboardId,
            string in_eventName,
            ulong in_eventMultiplier,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = in_leaderboardId;
            data[OperationParam.SocialLeaderboardServiceEventName.Value] = in_eventName;
            data[OperationParam.SocialLeaderboardServiceEventMultiplier.Value] = in_eventMultiplier;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.RewardTournament, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

