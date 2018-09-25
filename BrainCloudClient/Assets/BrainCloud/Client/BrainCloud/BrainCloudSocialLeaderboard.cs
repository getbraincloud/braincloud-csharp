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
        private BrainCloudClient _client;

        public BrainCloudSocialLeaderboard(BrainCloudClient client)
        {
            _client = client;
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
        /// Service Name - leaderboard
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
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetSocialLeaderboard, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns the social leaderboard by its version. A player's social leaderboard is
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
        /// Service Name - leaderboard
        /// Service Operation - GET_SOCIAL_LEADERBOARD
        /// </remarks>
        /// <param name="leaderboardId">
        /// The id of the leaderboard to retrieve
        /// </param>
        /// <param name="replaceName">
        /// If true, the currently logged in player's name will be replaced
        /// by the string "You".
        /// </param>
        /// <param name="versionId">
        /// The version
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
        public void GetSocialLeaderboardByVersion(
            string leaderboardId,
            bool replaceName,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceReplaceName.Value] = replaceName;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetSocialLeaderboardByVersion, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Reads multiple social leaderboards.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetMultiSocialLeaderboard, data, callback);
            _client.SendRequest(sc);
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
        /// Service Name - leaderboard
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
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetGlobalLeaderboardPage(
            string leaderboardId,
            SortOrder sort,
            int startIndex,
            int endIndex,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceStartIndex.Value] = startIndex;
            data[OperationParam.SocialLeaderboardServiceEndIndex.Value] = endIndex;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Method returns a page of global leaderboard results. By using a non-current version id, 
        /// the user can retrieve a historical leaderboard. See GetGlobalLeaderboardVersions method
        /// to retrieve the version id.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
        /// <param name="versionId">
        /// The historical version to retrieve.
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
        public void GetGlobalLeaderboardPageByVersion(
            string leaderboardId,
            SortOrder sort,
            int startIndex,
            int endIndex,
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
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGlobalLeaderboardPage, data, callback);
            _client.SendRequest(sc);
        }
        
        /// <summary>
        /// Method returns a view of global leaderboard results that centers on the current player.
        ///
        /// Leaderboards entries contain the player's score and optionally, some user-defined
        /// data associated with the score.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetGlobalLeaderboardView(
            string leaderboardId,
            SortOrder sort,
            int beforeCount,
            int afterCount,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            GetGlobalLeaderboardViewByVersion(leaderboardId, sort, beforeCount, afterCount, -1, success, failure, cbObject);
        }

        /// <summary>
        /// Method returns a view of global leaderboard results that centers on the current player.
        /// By using a non-current version id, the user can retrieve a historical leaderboard.
        /// See GetGlobalLeaderboardVersions method to retrieve the version id.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
        public void GetGlobalLeaderboardViewByVersion(
            string leaderboardId,
            SortOrder sort,
            int beforeCount,
            int afterCount,
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
            if (versionId != -1)
            {
                data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;
            }

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGlobalLeaderboardView, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the global leaderboard versions.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GetGlobalLeaderboardVersions
        /// </remarks>
        /// <param name="leaderboardId">In_leaderboard identifier.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void GetGlobalLeaderboardVersions(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGlobalLeaderboardVersions, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the social leaderboard for a group.
        /// </summary>
        /// <remarks>
        /// Service Name - ocialLeaderboard
        /// Service Operation - GET_GROUP_SOCIAL_LEADERBOARD
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the social leaderboard for a group by its version.
        /// </summary>
        /// <remarks>
        /// Service Name - ocialLeaderboard
        /// Service Operation - GET_GROUP_SOCIAL_LEADERBOARD_BY_VERSION
        /// </remarks>
        /// <param name="leaderboardId">The leaderboard to read</param>
        /// <param name="groupId">The group ID</param>
        /// <param name="versionId">The version ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void GetGroupSocialLeaderboardByVersion(
            string leaderboardId,
            string groupId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceGroupId.Value] = groupId;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGroupSocialLeaderboardByVersion, data, callback);
            _client.SendRequest(sc);
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
        /// Service Name - leaderboard
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
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.PostScore, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Removes a player's score from the leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - REMOVE_PLAYER_SCORE
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
        /// </param>
        /// <param name="versionId">
        /// The version of the leaderboard
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
        public void RemovePlayerScore(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.RemovePlayerScore, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Post the players score to the given social leaderboard.
        /// Pass leaderboard config data to dynamically create if necessary.
        /// You can optionally send a user-defined json string of data
        /// with the posted score. This string could include information
        /// relevant to the posted score.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
        /// Date to reset the leaderboard UTC
        /// </param>
        /// <param name="retainedCount">
        /// How many rotations to keep
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
                data[OperationParam.SocialLeaderboardServiceRotationResetTime.Value] = Util.DateTimeToUnixTimestamp(rotationReset.Value);

            data[OperationParam.SocialLeaderboardServiceRetainedCount.Value] = retainedCount;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.PostScoreDynamic, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Post the players score to the given social leaderboard with a rotation type of DAYS.
        /// Pass leaderboard config data to dynamically create if necessary.
        /// You can optionally send a user-defined json string of data
        /// with the posted score. This string could include information
        /// relevant to the posted score.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
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
        /// <param name="rotationReset">
        /// Date to reset the leaderboard UTC
        /// </param>
        /// <param name="retainedCount">
        /// How many rotations to keep
        /// </param>
        /// <param name="numDaysToRotate">
        /// How many days between each rotation
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
        public void PostScoreToDynamicLeaderboardDays(
            string leaderboardId,
            long score,
            string jsonData,
            SocialLeaderboardType leaderboardType,
            DateTime? rotationReset,
            int retainedCount,
            int numDaysToRotate,
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
            data[OperationParam.SocialLeaderboardServiceRotationType.Value] = "DAYS";

            if (rotationReset.HasValue)
                data[OperationParam.SocialLeaderboardServiceRotationResetTime.Value] = Util.DateTimeToUnixTimestamp(rotationReset.Value);

            data[OperationParam.SocialLeaderboardServiceRetainedCount.Value] = retainedCount;
            data[OperationParam.NumDaysToRotate.Value] = numDaysToRotate;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.PostScoreDynamic, data, callback);
            _client.SendRequest(sc);
        }
     
        /// <summary>
        /// Retrieve the social leaderboard for a list of players.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_PLAYERS_SOCIAL_LEADERBOARD
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
        /// </param>
        /// <param name="profileIds">
        /// The IDs of the players
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
        public void GetPlayersSocialLeaderboard(
            string leaderboardId,
            IList<string> profileIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceProfileIds.Value] = profileIds;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetPlayersSocialLeaderboard, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the social leaderboard for a list of players by their version.
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
        /// </param>
        /// <param name="profileIds">
        /// The IDs of the players
        /// </param>
        /// <param name="versionId">
        /// The version
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
        public void GetPlayersSocialLeaderboardByVersion(
            string leaderboardId,
            IList<string> profileIds,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceProfileIds.Value] = profileIds;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;


            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetPlayersSocialLeaderboardByVersion, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve a list of all leaderboards
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - LIST_LEADERBOARDS
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
        public void ListLeaderboards(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.ListAllLeaderboards, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets the number of entries in a global leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_GLOBAL_LEADERBOARD_ENTRY_COUNT
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
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
        public void GetGlobalLeaderboardEntryCount(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            GetGlobalLeaderboardEntryCountByVersion(leaderboardId, -1, success, failure, cbObject);
        }

        /// <summary>
        /// Gets the number of entries in a global leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_GLOBAL_LEADERBOARD_ENTRY_COUNT
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
        /// </param>
        /// <param name="versionId">
        /// The version of the leaderboard
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
        public void GetGlobalLeaderboardEntryCountByVersion(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;

            if (versionId > -1)
                data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetGlobalLeaderboardEntryCount, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets a player's score from a leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_PLAYER_SCORE
        /// </remarks>
        /// <param name="leaderboardId">
        /// The ID of the leaderboard
        /// </param>
        /// <param name="versionId">
        /// The version of the leaderboard. Use -1 for current.
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
        public void GetPlayerScore(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.SocialLeaderboardServiceVersionId.Value] = versionId;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetPlayerScore, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Gets a player's score from multiple leaderboards
        /// </summary>
        /// <remarks>
        /// Service Name - leaderboard
        /// Service Operation - GET_PLAYER_SCORES_FROM_LEADERBOARDS
        /// </remarks>
        /// <param name="leaderboardIds">
        /// A collection of leaderboardIds to retrieve scores from
        /// </param>
        /// <param name="versionId">
        /// The version of the leaderboard. Use -1 for current.
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
        public void GetPlayerScoresFromLeaderboards(
            IList<string> leaderboardIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardIds.Value] = leaderboardIds;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            var sc = new ServerCall(ServiceName.Leaderboard, ServiceOperation.GetPlayerScoresFromLeaderboards, data, callback);
            _client.SendRequest(sc);
        }
    }
}

