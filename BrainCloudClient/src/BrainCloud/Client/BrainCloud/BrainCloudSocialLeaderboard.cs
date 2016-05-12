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
    public class BrainCloudSocialLeaderboard
    {
        private BrainCloudClient _brainCloudClient;

        public BrainCloudSocialLeaderboard(BrainCloudClient brainCloudClient)
        {
            _brainCloudClient = brainCloudClient;
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

        public enum SortOrder
        {

            HIGH_TO_LOW,
            LOW_TO_HIGH
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
        /// Service Operation - GET_SOCIAL_LEADERBOARD
        /// </remarks>
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve
        /// </param>
        /// <param name="replaceName">
        /// If true, the currently logged in player's name will be replaced
        /// by the string "You".
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
        public void GetSocialLeaderboard(
            string leaderboardId,
            bool replaceName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = replaceName;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetSocialLeaderboard, data, callback);
            _brainCloudClient.SendRequest(sc);
        }

        /// <summary>
        /// Reads multiple social leaderboards.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GET_MULTI_SOCIAL_LEADERBOARD
        /// </remarks>
        /// <param name="leaderboardIds">
        /// Array of leaderboard id strings
        /// </param>
        /// <param name="leaderboardResultCount">
        /// Maximum count of entries to return for each leaderboard.
        /// </param>
        /// <param name="replaceName">
        /// If true, the currently logged in player's name will be replaced
        /// by the string "You".
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
        /// <returns> JSON string representing the entries in the leaderboard.
        /// Note that the friend summary data is returned for each record
        /// in the leaderboard.
        ///
        /// {
        ///   "server_time": 1445952203123,
        ///   "leaderboards": [
        ///    {
        ///     "leaderboardId": "default",
        ///     "leaderboard": [
        ///      {
        ///       "externalId": "pacer5",
        ///       "name": "Rollo",
        ///       "pictureUrl": "http://localhost:8080/gameserver/s3/portal/g/eggies/metadata/pacers/pacer5.png",
        ///       "playerId": "pacer5",
        ///       "authenticationType": null,
        ///       "score": 100000,
        ///       "data": {
        ///        "pacerTag": null,
        ///        "pacerLeaderboardTag": {}
        ///       },
        ///       "createdAt": null,
        ///       "updatedAt": null
        ///      },
        ///      {
        ///       "externalId": "pacer4",
        ///       "name": "Chirp",
        ///       "pictureUrl": "http://localhost:8080/gameserver/s3/portal/g/eggies/metadata/pacers/pacer4.png",
        ///       "playerId": "pacer4",
        ///       "authenticationType": null,
        ///       "score": 80000,
        ///       "data": {
        ///        "pacerTag": null,
        ///        "pacerLeaderboardTag": {}
        ///       },
        ///       "createdAt": null,
        ///       "updatedAt": null
        ///      }
        ///     ],
        ///     "self": {
        ///      "externalId": null,
        ///      "name": null,
        ///      "pictureUrl": null,
        ///      "playerId": "49390659-33bd-4812-b0c4-ab04e614ec98",
        ///      "authenticationType": null,
        ///      "score": 10,
        ///      "data": {
        ///       "nickname": "batman"
        ///      },
        ///      "createdAt": 1445952060607,
        ///      "updatedAt": 1445952060607,
        ///      "summaryFriendData": null
        ///     },
        ///     "selfIndex": 5
        ///    }
        ///   ]
        ///  }
        ////
        public void GetMultiSocialLeaderboard(IList<string> leaderboardIds,
                                              int leaderboardResultCount,
                                              bool replaceName,
                                              SuccessCallback success = null,
                                              FailureCallback failure = null,
                                              object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardIds.Value] = leaderboardIds;
            data[OperationParam.SocialLeaderboardServiceLeaderboardResultCount.Value] = leaderboardResultCount;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = replaceName;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetMultiSocialLeaderboard, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="startIndex">
        /// The index at which to start the page.
        /// </param>
        /// <param name="endIndex">
        /// The index at which to end the page.
        /// </param>
        /// <param name="includeLeaderboardSize">
        /// Whether to return the leaderboard size
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
            string leaderboardId,
            SortOrder sort,
            int startIndex,
            int endIndex,
            bool includeLeaderboardSize,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceStartIndex.Value] = startIndex;
            data[OperationParam.SocialLeaderboardServiceEndIndex.Value] = endIndex;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = includeLeaderboardSize;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="startIndex">
        /// The index at which to start the page.
        /// </param>
        /// <param name="endIndex">
        /// The index at which to end the page.
        /// </param>
        /// <param name="includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="versionId">
        /// The historial version to retrieve.
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
        /// <returns> JSON string representing the entries in the leaderboard.
        /// See GetGlobalLeaderboardPage documentation. Note that historial leaderboards do not
        /// include the 'timeBeforeReset' parameter.
        /// </returns>
        public void GetGlobalLeaderboardPageByVersion(
            string leaderboardId,
            SortOrder sort,
            int startIndex,
            int endIndex,
            bool includeLeaderboardSize,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceStartIndex.Value] = startIndex;
            data[OperationParam.SocialLeaderboardServiceEndIndex.Value] = endIndex;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = includeLeaderboardSize;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="beforeCount">
        /// The count of number of players before the current player to include.
        /// </param>
        /// <param name="afterCount">
        /// The count of number of players after the current player to include.
        /// </param>
        /// <param name="includeLeaderboardSize">
        /// Whether to return the leaderboard size
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
            string leaderboardId,
            SortOrder sort,
            int beforeCount,
            int afterCount,
            bool includeLeaderboardSize,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceBeforeCount.Value] = beforeCount;
            data[OperationParam.SocialLeaderboardServiceAfterCount.Value] = afterCount;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = includeLeaderboardSize;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardView, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve.
        /// </param>
        /// <param name="sort">
        /// Sort key Sort order of page.
        /// </param>
        /// <param name="beforeCount">
        /// The count of number of players before the current player to include.
        /// </param>
        /// <param name="afterCount">
        /// The count of number of players after the current player to include.
        /// </param>
        /// <param name="includeLeaderboardSize">
        /// Whether to return the leaderboard size
        /// </param>
        /// <param name="versionId">
        /// The historial version to retrieve. Use -1 for current leaderboard.
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
        /// <returns> JSON string representing the entries in the leaderboard.
        /// See GetGlobalLeaderboardView documentation. Note that historial leaderboards do not
        /// include the 'timeBeforeReset' parameter.
        /// </returns>
        public void GetGlobalLeaderboardViewByVersion(
            string leaderboardId,
            SortOrder sort,
            int beforeCount,
            int afterCount,
            bool includeLeaderboardSize,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceBeforeCount.Value] = beforeCount;
            data[OperationParam.SocialLeaderboardServiceAfterCount.Value] = afterCount;
            data[OperationParam.SocialLeaderboardServiceIncludeLeaderboardSize.Value] = includeLeaderboardSize;
            if (versionId != -1)
            {
                data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardView, data, callback);
            _brainCloudClient.SendRequest(sc);
        }

        /// <summary>
        /// Gets the global leaderboard versions.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardVersions
        /// </remarks>
        /// <param name="leaderboardId">In_leaderboard identifier.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
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
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetGlobalLeaderboardVersions, data, callback);
            _brainCloudClient.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the social leaderboard for a group.
        /// </summary>
        /// <remarks>
        /// Service Name - ocialLeaderboard
        /// Service Operation - GetGlobalLeaderboardVersions
        /// </remarks>
        /// <param name="leaderboardId">The leaderboard to read</param>
        /// <param name="groupId">The group ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void GetGroupSocialLeaderboard(
            string leaderboardId,
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceGroupId.Value] = groupId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGroupSocialLeaderboard, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The leaderboard to post to
        /// </param>
        /// <param name="score">
        /// The score to post
        /// </param>
        /// <param name="data">
        /// Optional user-defined data to post with the score
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data": null
        /// }
        /// </returns>
        public void PostScoreToLeaderboard(
            string leaderboardId,
            long score,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceScore.Value] = score;
            if (Util.IsOptionalParameterValid(jsonData))
            {
                var customData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.SocialLeaderboardServiceData.Value] = customData;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.PostScore, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The leaderboard to post to
        /// </param>
        /// <param name="score">
        /// The score to post
        /// </param>
        /// <param name="data">
        /// Optional user-defined data to post with the score
        /// </param>
        /// <param name="leaderboardType">
        /// leaderboard type
        /// </param>
        /// <param name="rotationType">
        /// Type of rotation
        /// </param>
        /// <param name="rotationReset">
        /// Date to reset the leaderboard
        /// </param>
        /// <param name="retainedCount">
        /// Hpw many rotations to keep
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///   }
        /// }
        /// </returns>
        public void PostScoreToDynamicLeaderboard(
            string leaderboardId,
            long score,
            string jsonData,
            SocialLeaderboardType leaderboardType,
            RotationType rotationType,
            DateTime? rotationReset,
            int retainedCount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceScore.Value] = score;
            if (Util.IsOptionalParameterValid(jsonData))
            {
                var customData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.SocialLeaderboardServiceData.Value] = customData;
            }
            data[OperationParam.SocialLeaderboardServiceLeaderboardType.Value] = leaderboardType.ToString();
            data[OperationParam.SocialLeaderboardServiceRotationType.Value] = rotationType.ToString();

            if (rotationReset.HasValue)
                data[OperationParam.SocialLeaderboardServiceRotationReset.Value] = rotationReset.Value.ToString("dd-MM-yyyy HH:mm");

            data[OperationParam.SocialLeaderboardServiceRetainedCount.Value] = retainedCount;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.PostScoreDynamic, data, callback);
            _brainCloudClient.SendRequest(sc);
        }

        /// <summary>
        /// Reset the player's score for the given social leaderboard id.
        /// </summary>
        /// <remarks>
        /// Service Name - SocialLeaderboard
        /// Service Operation - Reset
        /// </remarks>
        /// <param name="leaderboardId">
        /// The leaderboard to post to
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data": null
        /// }
        /// </returns>
        public void ResetLeaderboardScore(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.Reset, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The id of the leaderboard
        /// </param>
        /// <param name="replaceName">
        /// True if the player's name should be replaced with "You"
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///   //same as GetLeaderboard
        ///   }
        /// }
        /// </returns>
        public void GetCompletedLeaderboardTournament(
            string leaderboardId,
            bool replaceName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = replaceName;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetCompletedTournament, data, callback);
            _brainCloudClient.SendRequest(sc);
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
        /// <param name="leaderboardId">
        /// The leaderboard the tournament was on
        /// </param>
        /// <param name="eventName">
        /// The player statistics event name to trigger
        /// </param>
        /// <param name="eventMultiplier">
        /// The multiplier to associate with the event
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":{
        ///   }
        /// }
        /// </returns>
        public void TriggerSocialLeaderboardTournamentReward(
            string leaderboardId,
            string eventName,
            ulong eventMultiplier,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceEventName.Value] = eventName;
            data[OperationParam.SocialLeaderboardServiceEventMultiplier.Value] = eventMultiplier;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.RewardTournament, data, callback);
            _brainCloudClient.SendRequest(sc);
        }
    }
}

