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
    public class BrainCloudTournament
    {
        private BrainCloudClient _client;
        public BrainCloudTournament(BrainCloudClient brainCloudClientRef)
        {
            _client = brainCloudClientRef;
        }

        /// <summary>
        /// Get tournament status associated with a leaderboard.
        /// </summary>
        /// <remarks>
        /// Service Name - tournament
        /// Service Operation - GET_TOURNAMENT_STATUS
        /// </remarks>
        /// <param name="scriptName">
        /// The name of the script to be run
        /// </param>
        /// <param name="jsonScriptData">
        /// Data to be sent to the script in json format
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


        public void JoinTournament(
            string leaderboardId,
            string tournamentCode,
            int initialScore,
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


        public void PostTournamentScore(
            string leaderboardId,
            int score,
            string jsonData,
            long roundStartedEpoch,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LeaderboardId.Value] = leaderboardId;
            data[OperationParam.Score.Value] = score;
            data[OperationParam.RoundStartedEpoch.Value] = roundStartedEpoch;
            
            if (Util.IsOptionalParameterValid(jsonData))
            {
                Dictionary<string, object> scoreData = JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
                data[OperationParam.Data.Value] = scoreData;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Tournament, ServiceOperation.PostTournamentScore, data, callback);
            _client.SendRequest(sc);
        }


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
    }
}
