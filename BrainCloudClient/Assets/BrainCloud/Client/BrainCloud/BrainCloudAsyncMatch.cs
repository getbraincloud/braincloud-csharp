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
    public class BrainCloudAsyncMatch
    {
        private BrainCloudClient _client;

        public BrainCloudAsyncMatch(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates an instance of an asynchronous match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="jsonOpponentIds">
        /// JSON string identifying the opponent platform and id for this match.
        ///
        /// Platforms are identified as:
        /// BC - a brainCloud profile id
        /// FB - a Facebook id
        ///
        /// An exmaple of this string would be:
        /// [
        ///     {
        ///         "platform": "BC",
        ///         "id": "some-braincloud-profile"
        ///     },
        ///     {
        ///         "platform": "FB",
        ///         "id": "some-facebook-id"
        ///     }
        /// ]
        /// </param>
        /// <param name="pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
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
        public void CreateMatch(
            string jsonOpponentIds,
            string pushNotificationMessage,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            CreateMatchInternal(jsonOpponentIds, null, pushNotificationMessage, null, null, null, success, failure, cbObject);
        }

        /// <summary>
        /// Creates an instance of an asynchronous match with an initial turn.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="jsonOpponentIds">
        /// JSON string identifying the opponent platform and id for this match.
        ///
        /// Platforms are identified as:
        /// BC - a brainCloud profile id
        /// FB - a Facebook id
        ///
        /// An exmaple of this string would be:
        /// [
        ///     {
        ///         "platform": "BC",
        ///         "id": "some-braincloud-profile"
        ///     },
        ///     {
        ///         "platform": "FB",
        ///         "id": "some-facebook-id"
        ///     }
        /// ]
        /// </param>
        /// <param name="jsonMatchState">
        /// JSON string blob provided by the caller
        /// </param>
        /// <param name="pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="nextPlayer">
        /// Optionally, force the next player player to be a specific player
        /// </param>
        /// <param name="jsonSummary">
        /// Optional JSON string defining what the other player will see as a summary of the game when listing their games
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
        public void CreateMatchWithInitialTurn(
            string jsonOpponentIds,
            string jsonMatchState,
            string pushNotificationMessage,
            string nextPlayer,
            string jsonSummary,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            CreateMatchInternal(
                jsonOpponentIds,
                jsonMatchState == null ? "{}" : jsonMatchState,
                pushNotificationMessage,
                null,
                nextPlayer,
                jsonSummary,
                success, failure, cbObject);
        }

        /// <summary>
        /// Submits a turn for the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - SubmitTurn
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identfier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="version">
        /// Game state version to ensure turns are submitted once and in order
        /// </param>
        /// <param name="jsonMatchState">
        /// JSON string blob provided by the caller
        /// </param>
        /// <param name="pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="nextPlayer">
        /// Optionally, force the next player player to be a specific player
        /// </param>
        /// <param name="jsonSummary">
        /// Optional JSON string that other players will see as a summary of the game when listing their games
        /// </param>
        /// <param name="jsonStatistics">
        /// Optional JSON string blob provided by the caller
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
        public void SubmitTurn(
            string ownerId,
            string matchId,
            UInt64 version,
            string jsonMatchState,
            string pushNotificationMessage,
            string nextPlayer,
            string jsonSummary,
            string jsonStatistics,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;
            data["version"] = version;
            data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonMatchState);

            if (Util.IsOptionalParameterValid(nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonSummary);
            }

            if (Util.IsOptionalParameterValid(jsonStatistics))
            {
                data["statistics"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonStatistics);
            }

            if (Util.IsOptionalParameterValid(pushNotificationMessage))
            {
                data["pushContent"] = pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.SubmitTurn, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows the current player (only) to update Summary data without having to submit a whole turn.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - UpdateMatchSummary
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identfier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
        /// </param>
        /// <param name="version">
        /// Game state version to ensure turns are submitted once and in order
        /// </param>
        /// <param name="jsonSummary">
        /// JSON string provided by the caller that other players will see as a summary of the game when listing their games
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
        public void UpdateMatchSummaryData(
            string ownerId,
            string matchId,
            UInt64 version,
            string jsonSummary,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;
            data["version"] = version;

            if (Util.IsOptionalParameterValid(jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonSummary);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.UpdateMatchSummary, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Marks the given match as complete.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Complete
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
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
        public void CompleteMatch(
            string ownerId,
            string matchId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Complete, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the current state of the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - ReadMatch
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
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
        public void ReadMatch(
            string ownerId,
            string matchId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatch, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns the match history of the given match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - ReadMatchHistory
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
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
        public void ReadMatchHistory(
            string ownerId,
            string matchId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatchHistory, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are NOT in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatches
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
        public void FindMatches(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatches, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatchesCompleted
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.e is received.
        /// </param>
        public void FindCompleteMatches(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatchesCompleted, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Marks the given match as abandoned.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Abandon
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
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
        public void AbandonMatch(
            string ownerId,
            string matchId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Abandon, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Removes the match and match history from the server. DEBUG ONLY, in production it is recommended
        /// the user leave it as completed.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Delete
        /// </remarks>
        /// <param name="ownerId">
        /// Match owner identifier
        /// </param>
        /// <param name="matchId">
        /// Match identifier
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
        public void DeleteMatch(
            string ownerId,
            string matchId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = ownerId;
            data["matchId"] = matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.DeleteMatch, data, callback);
            _client.SendRequest(sc);
        }

        private void CreateMatchInternal(
            string jsonOpponentIds,
            string jsonMatchState,
            string pushNotificationMessage,
            string matchId,
            string nextPlayer,
            string jsonSummary,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["players"] = JsonReader.Deserialize<object[]>(jsonOpponentIds);

            if (Util.IsOptionalParameterValid(jsonMatchState))
            {
                data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonMatchState);
            }

            if (Util.IsOptionalParameterValid(matchId))
            {
                data["matchId"] = matchId;
            }

            if (Util.IsOptionalParameterValid(nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(jsonSummary);
            }

            if (Util.IsOptionalParameterValid(pushNotificationMessage))
            {
                data["pushContent"] = pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Create, data, callback);
            _client.SendRequest(sc);
        }
    }
}
