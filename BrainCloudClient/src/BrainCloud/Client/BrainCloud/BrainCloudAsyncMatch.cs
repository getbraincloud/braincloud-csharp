//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudAsyncMatch
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudAsyncMatch(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Creates an instance of an asynchronous match.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_jsonOpponentIds">
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
        /// <param name="in_pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="in_matchId">
        /// Optional match identifier. An id will be generated if not provided.
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
        ///     "gameId": "16245",
        ///     "ownerId": "0df9f282-183b-4d67-866e-670fb35a2376",
        ///     "matchId": "b55d12d6-f01f-45c5-827c-ded706374524",
        ///     "version": 0,
        ///     "players": [
        ///         {
        ///             "playerId": "0df9f282-183b-4d67-866e-670fb35a2376",
        ///             "playerName": "UserB",
        ///             "pictureUrl": null
        ///         },
        ///         {
        ///             "playerId": "4693ec75-3a99-4577-aef7-0f41d299339c",
        ///             "playerName": "Presto1",
        ///             "pictureUrl": null
        ///         }
        ///     ],
        ///     "status": {
        ///         "status": "NOT_STARTED",
        ///         "currentPlayer": "0df9f282-183b-4d67-866e-670fb35a2376"
        ///     },
        ///     "summary": null,
        ///     "createdAt": 1415641372974,
        ///     "updatedAt": 1415641372974
        /// }
        /// </returns>
        public void CreateMatch(
            string in_jsonOpponentIds,
            string in_pushNotificationMessage,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            CreateMatchInternal(in_jsonOpponentIds, null, in_pushNotificationMessage, in_matchId, null, null, in_success, in_failure, in_cbObject);
        }

        /// <summary>
        /// Creates an instance of an asynchronous match with an initial turn.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - Create
        /// </remarks>
        /// <param name="in_jsonOpponentIds">
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
        /// <param name="in_jsonMatchState">
        /// JSON string blob provided by the caller
        /// </param>
        /// <param name="in_pushNotificationMessage">
        /// Optional push notification message to send to the other party.
        /// Refer to the Push Notification functions for the syntax required.
        /// </param>
        /// <param name="in_matchId">
        /// Optional match identifier. An id will be generated if not provided.
        /// </param>
        /// <param name="in_nextPlayer">
        /// Optionally, force the next player player to be a specific player
        /// </param>
        /// <param name="in_jsonSummary">
        /// Optional JSON string defining what the other player will see as a summary of the game when listing their games
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
        ///     "gameId": "145677",
        ///     "ownerId": "0df9f282-183b-4d67-866e-670fb35a2376",
        ///     "matchId": "b55d12d6-f01f-45c5-827c-ded706374524",
        ///     "version": 1,
        ///     "players": [
        ///         {
        ///             "playerId": "0df9f282-183b-4d67-866e-670fb35a2376",
        ///             "playerName": "UserB",
        ///             "pictureUrl": null
        ///         },
        ///         {
        ///             "playerId": "4693ec75-3a99-4577-aef7-0f41d299339c",
        ///             "playerName": "Presto1",
        ///             "pictureUrl": null
        ///         }
        ///     ],
        ///     "status": {
        ///         "status": "PENDING",
        ///         "currentPlayer": "4693ec75-3a99-4577-aef7-0f41d299339c"
        ///     },
        ///         "summary": {
        ///         "currentMap": "asdf"
        ///     },
        ///     "createdAt": 1415641372974,
        ///     "updatedAt": 1415641372974
        /// }                                                                     
        /// </returns>
        public void CreateMatchWithInitialTurn(
            string in_jsonOpponentIds,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_matchId,
            string in_nextPlayer,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            CreateMatchInternal(
                in_jsonOpponentIds,
                in_jsonMatchState == null ? "{}" : in_jsonMatchState,
                in_pushNotificationMessage,
                in_matchId,
                in_nextPlayer,
                in_jsonSummary,
                in_success, in_failure, in_cbObject);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "gameId": "145677",
        ///         "ownerId": "2bd7abc6-c2ec-4946-a1a8-02bad38540ad",
        ///         "matchId": "1aac24b2-7976-4fd7-b7c6-44dere6d26a4",
        ///         "version": 1,
        ///         "players": [
        ///             {
        ///                 "playerId": "2bd7abc6-c2ec-4946-a1a8-02bad38540ad",
        ///                 "playerName": "UserB",
        ///                 "pictureUrl": null
        ///             },
        ///             {
        ///                 "playerId": "11c9dd4d-9ed1-416d-baw2-5228c1efafac",
        ///                 "playerName": "UserA",
        ///                 "pictureUrl": null
        ///             }
        ///         ],
        ///         "status": {
        ///             "status": "PENDING",
        ///             "currentPlayer": "11c9dd4d-9ed1-416d-baw2-5228c1efafac"
        ///         },
        ///         "summary": {
        ///             "resources": 1234
        ///         },
        ///         "createdAt": 1442507219609,
        ///         "updatedAt": 1442507319700
        ///     }
        /// }
        /// </returns>
        public void SubmitTurn(
            string in_ownerId,
            string in_matchId,
            UInt64 in_version,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_nextPlayer,
            string in_jsonSummary,
            string in_jsonStatistics,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;
            data["version"] = in_version;
            data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonMatchState);

            if (Util.IsOptionalParameterValid(in_nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = in_nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            if (Util.IsOptionalParameterValid(in_jsonStatistics))
            {
                data["statistics"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonStatistics);
            }

            if (Util.IsOptionalParameterValid(in_pushNotificationMessage))
            {
                data["pushContent"] = in_pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.SubmitTurn, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "gameId": "145677",
        ///         "ownerId": "2bd723c6-c2ec-4946-a1a8-02b7a38540ad",
        ///         "matchId": "1aac24b2-7976-4fd7-b7c6-44d7ae6d26a4",
        ///         "version": 2,
        ///         "players": [
        ///             {
        ///                 "playerId": "2bd723c6-c2ec-4946-a1a8-02b7a38540ad",
        ///                 "playerName": "UserA",
        ///                 "pictureUrl": null
        ///             },
        ///             {
        ///                 "playerId": "11c2dd4d-9ed1-416d-bd04-5228c1efafac",
        ///                 "playerName": "UserB",
        ///                 "pictureUrl": null
        ///             }
        ///         ],
        ///         "status": {
        ///             "status": "PENDING",
        ///             "currentPlayer": "11c2dd4d-9ed1-416d-bd04-5228c1efafac"
        ///         },
        ///         "summary": {
        ///             "resources": 2564
        ///         },
        ///         "createdAt": 1442507219609,
        ///         "updatedAt": 1442507550372
        ///     }
        /// }
        /// </returns>
        public void UpdateMatchSummaryData(
            string in_ownerId,
            string in_matchId,
            UInt64 in_version,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;
            data["version"] = in_version;

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.UpdateMatchSummary, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {}
        /// }
        /// </returns>
        public void CompleteMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Complete, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "gameId": "10299",
        ///         "ownerId": "11c9dd4d-9ed1-416d-bd04-5228c1efafac",
        ///         "matchId": "0d4c1803-887a-4f20-a2e4-73eeedba411e",
        ///         "version": 1,
        ///         "players": [
        ///             {
        ///                 "playerId": "11c9dd4d-9ed1-416d-bd04-5228c1efafac",
        ///                 "playerName": "UserB",
        ///                 "pictureUrl": null
        ///             },
        ///             {
        ///                 "playerId": "2bd7abc6-c2ec-4946-a1a8-02b7a38540ad",
        ///                 "playerName": "UserA",
        ///                 "pictureUrl": null
        ///             }
        ///         ],
        ///         "status": {
        ///             "status": "PENDING",
        ///             "currentPlayer": "2bd7abc6-c2ec-4946-a1a8-02b7a38540ad"
        ///         },
        ///         "summary": null,
        ///         "statistics": {},
        ///         "matchState": {},
        ///         "createdAt": 1442508171624,
        ///         "updatedAt": 1442508171632
        ///     }
        /// }
        /// </returns>
        public void ReadMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "gameId": "14577",
        ///         "ownerId": "2bd32bc6-c2ec-4916-a1a8-02b7a38540ad",
        ///         "matchId": "1aac24b2-7976-4fd7-b7c6-44d32e6d26a4",
        ///         "turns": [
        ///             {
        ///                 "playerId": "2bd32bc6-c2ec-4916-a1a8-02b7a38540ad",
        ///                 "matchState": {
        ///                     "color": "red"
        ///                 },
        ///                 "createdAt": 1442507319697
        ///             },
        ///             {
        ///                 "playerId": "11c9324d-9ed1-416d-b124-5228c1efafac",
        ///                 "matchState": {},
        ///                 "createdAt": 1442507703667
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void ReadMatchHistory(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.ReadMatchHistory, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are NOT in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatches
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "results": [
        ///             {
        ///                 "gameId": "123456",
        ///                 "ownerId": "7630f98e-1236-4ead-88ee-27ce63b2db2c",
        ///                 "matchId": "97c8d087-40d8-45c2-aa2b-6d0d83424ec5",
        ///                 "version": 1,
        ///                 "players": [
        ///                     {
        ///                         "playerId": "7630f98e-13b6-4ead-88ee-27ce63b2db2c",
        ///                         "playerName": "UserA-122217922",
        ///                         "pictureUrl": null
        ///                     },
        ///                     {
        ///                         "playerId": "efab2d0b-90a1-48cf-8678-ae81d7aaed89",
        ///                         "playerName": "UserB-122217922",
        ///                         "pictureUrl": null
        ///                     },
        ///                     {
        ///                         "playerId": "b28ff14a-364a-40b3-ac4e-d2b23983519c",
        ///                         "playerName": "UserC-338593317",
        ///                         "pictureUrl": null
        ///                     }
        ///                 ],
        ///                 "status": {
        ///                     "status": "PENDING",
        ///                     "currentPlayer": "efab2d0b-90a1-48cf-8678-ae81d7aaed89"
        ///                 },
        ///                 "summary": null,
        ///                 "createdAt": 1442586020180,
        ///                 "updatedAt": 1442586020188
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void FindMatches(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatches, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns all matches that are in a COMPLETE state for which the player is involved.
        /// </summary>
        /// <remarks>
        /// Service Name - AsyncMatch
        /// Service Operation - FindMatchesCompleted
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.e is received.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "results": [
        ///             {
        ///                 "gameId": "10170",
        ///                 "ownerId": "9ad4f990-5466-4d00-a334-de834e1ac4ec",
        ///                 "matchId": "877dd25d-ea21-4857-ba2a-2134d0f5ace2",
        ///                 "version": 2,
        ///                 "players": [
        ///                     {
        ///                         "playerId": "9ad4f990-5466-4d00-a334-de834e1ac4ec",
        ///                         "playerName": "",
        ///                         "pictureUrl": null
        ///                     },
        ///                     {
        ///                         "playerId": "963a2079-6e7a-48de-a4f2-8ab16c811975",
        ///                         "playerName": "",
        ///                         "pictureUrl": null
        ///                     }
        ///                 ],
        ///                 "status": {
        ///                     "status": "COMPLETE",
        ///                     "currentPlayer": "963a2079-6e7a-48de-a4f2-8ab16c811975"
        ///                 },
        ///                 "summary": null,
        ///                 "createdAt": 1442586358023,
        ///                 "updatedAt": 1442586374787
        ///             }
        ///         ]
        ///     }
        /// }
        /// </returns>
        public void FindCompleteMatches(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.FindMatchesCompleted, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {}
        /// }
        /// </returns>
        public void AbandonMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Abandon, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Removes the match and match history from the server. DEBUG ONLY, in production it is recommended
        ///	the user leave it as completed.
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>
        /// {
        ///     "status": 200,
        ///     "data": {}
        /// }
        /// </returns>
        public void DeleteMatch(
            string in_ownerId,
            string in_matchId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["ownerId"] = in_ownerId;
            data["matchId"] = in_matchId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.DeleteMatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        private void CreateMatchInternal(
            string in_jsonOpponentIds,
            string in_jsonMatchState,
            string in_pushNotificationMessage,
            string in_matchId,
            string in_nextPlayer,
            string in_jsonSummary,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["players"] = JsonReader.Deserialize<object[]>(in_jsonOpponentIds);

            if (Util.IsOptionalParameterValid(in_jsonMatchState))
            {
                data["matchState"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonMatchState);
            }

            if (Util.IsOptionalParameterValid(in_matchId))
            {
                data["matchId"] = in_matchId;
            }

            if (Util.IsOptionalParameterValid(in_nextPlayer))
            {
                Dictionary<string, object> status = new Dictionary<string, object>();
                status["currentPlayer"] = in_nextPlayer;
                data["status"] = status;
            }

            if (Util.IsOptionalParameterValid(in_jsonSummary))
            {
                data["summary"] = JsonReader.Deserialize<Dictionary<string, object>>(in_jsonSummary);
            }

            if (Util.IsOptionalParameterValid(in_pushNotificationMessage))
            {
                data["pushContent"] = in_pushNotificationMessage;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.AsyncMatch, ServiceOperation.Create, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
