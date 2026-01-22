// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.JsonFx.Json;
    using BrainCloud.Internal;
    using System;

    public class BrainCloudTournament
    {
        private BrainCloudClient _client;
        public BrainCloudTournament(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Processes any outstanding rewards for the given player
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - CLAIM_TOURNAMENT_REWARD
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_versionId">Version of the tournament. Use -1 for the latest version.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ClaimTournamentReward(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.VersionId.Value] = versionId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.ClaimTournamentReward, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Get the status of a division
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - GET_DIVISION_INFO
        /// </remarks>
        /// <param name="in_divSetId">The id for the division</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetDivisionInfo(
            string divSetId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DivSetId.Value] = divSetId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.GetDivisionInfo, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns list of player's recently active divisions
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - GET_MY_DIVISIONS
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMyDivisions(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.GetMyDivisions, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Get tournament status associated with a leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - GET_TOURNAMENT_STATUS
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_versionId">Version of the tournament. Use -1 for the latest version.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetTournamentStatus(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.VersionId.Value] = versionId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.GetTournamentStatus, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Join the specified division.
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - JOIN_DIVISION
        /// </remarks>
        /// <param name="in_divSetId">The id for the division</param>
        /// <param name="in_tournamentCode">Tournament to join</param>
        /// <param name="in_initialScore">The initial score for players first joining a tournament Usually 0, unless leaderboard is LOW_VALUE</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void JoinDivision(
            string divSetId,
            string tournamentCode,
            long initialScore,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.DivSetId.Value] = divSetId;
            data[OperationParam.TournamentCode.Value] = tournamentCode;
            data[OperationParam.InitialScore.Value] = initialScore;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.JoinDivision, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Join the specified tournament.
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - JOIN_TOURNAMENT
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_tournamentCode">Tournament to join</param>
        /// <param name="in_initialScore">The initial score for players first joining a tournament Usually 0, unless leaderboard is LOW_VALUE</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void JoinTournament(
            string leaderboardId,
            string tournamentCode,
            long initialScore,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.TournamentCode.Value] = tournamentCode;
            data[OperationParam.InitialScore.Value] = initialScore;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.JoinTournament, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Removes player from division instance
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - LEAVE_DIVISION_INSTANCE
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void LeaveDivisionInstance(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.LeaveDivisionInstance, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Removes player's score from tournament leaderboard
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - LEAVE_TOURNAMENT
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void LeaveTournament(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.LeaveTournament, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Post the users score to the leaderboard - UTC time
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - POST_TOURNAMENT_SCORE
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_score">The score to post</param>
        /// <param name="in_jsonData">Optional data attached to the leaderboard entry</param>
        /// <param name="in_roundStartedTimeUTC">Time the user started the match resulting in the score being posted in UTC. Use UTC time in milliseconds since epoch</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void PostTournamentScoreUTC(
            string leaderboardId,
            long score,
            string jsonData,
            ulong roundStartTimeUTC,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.Score.Value] = score;
            data[OperationParam.RoundStartedEpoch.Value] = roundStartTimeUTC;

            if (Util.IsOptionalParameterValid(jsonData))
            {
                Dictionary<string, object> scoreData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.Data.Value] = scoreData;
            }
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.PostTournamentScore, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Post the users score to the leaderboard - UTC time
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - POST_TOURNAMENT_SCORE_WITH_RESULTS
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_score">The score to post</param>
        /// <param name="in_jsonData">Optional data attached to the leaderboard entry</param>
        /// <param name="in_roundStartedTimeUTC">Time the user started the match resulting in the score being posted in UTC. Use UTC time in milliseconds since epoch</param>
        /// <param name="in_sort">Sort key Sort order of page.</param>
        /// <param name="in_beforeCount">The count of number of players before the current player to include.</param>
        /// <param name="in_afterCount">The count of number of players after the current player to include.</param>
        /// <param name="in_initialScore">The initial score for players first joining a tournament Usually 0, unless leaderboard is LOW_VALUE</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void PostTournamentScoreWithResultsUTC(
             string leaderboardId,
             long score,
             string jsonData,
             ulong roundStartTimeUTC,
             BrainCloudSocialLeaderboard.SortOrder sort,
             int beforeCount,
             int afterCount,
             long initialScore,
             SuccessCallback success = null,
             FailureCallback failure = null,
             object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.SocialLeaderboardServiceLeaderboardId.Value] = leaderboardId;
            data[OperationParam.Score.Value] = score;
            data[OperationParam.RoundStartedEpoch.Value] = roundStartTimeUTC;
            data[OperationParam.InitialScore.Value] = initialScore;

            if (Util.IsOptionalParameterValid(jsonData))
            {
                Dictionary<string, object> scoreData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.Data.Value] = scoreData;
            }

            data[OperationParam.SocialLeaderboardServiceSort.Value] = sort.ToString();
            data[OperationParam.SocialLeaderboardServiceBeforeCount.Value] = beforeCount;
            data[OperationParam.SocialLeaderboardServiceAfterCount.Value] = afterCount;

            var callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            _client.SendRequest(new ServerCall(ServiceName.Tournament, ServiceOperation.PostTournamentScoreWithResults, data, callback));
        }

        /// <summary>
        /// Returns the user's expected reward based on the current scores
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - VIEW_CURRENT_REWARD
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ViewCurrentReward(
            string leaderboardId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.ViewCurrentReward, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the user's reward from a finished tournament
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - VIEW_REWARD
        /// </remarks>
        /// <param name="in_leaderboardId">The leaderboard for the tournament</param>
        /// <param name="in_versionId">Version of the tournament. Use -1 for the latest version.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void ViewReward(
            string leaderboardId,
            int versionId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.VersionId.Value] = versionId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.ViewReward, data, callback);
            _client.SendRequest(sc);
        }
    }
}
