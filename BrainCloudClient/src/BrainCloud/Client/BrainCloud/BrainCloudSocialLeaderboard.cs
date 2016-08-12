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

        /// <summary>
        /// Retrieve the social leaderboard for a list of players.
        /// </summary>
        /// <remarks>
        /// Service Name - socialLeaderboard
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
            var sc = new ServerCall(ServiceName.SocialLeaderboard, ServiceOperation.GetPlayersSocialLeaderboard, data, callback);
            _brainCloudClient.SendRequest(sc);
        }
    }
}

