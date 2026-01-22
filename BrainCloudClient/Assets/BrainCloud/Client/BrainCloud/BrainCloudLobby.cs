// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------
#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST
#endif

namespace BrainCloud
{
#if DOT_NET || GODOT
    using System.Net.Http;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;
#else
    using System.Net;
    using System.Net.Sockets;
    using UnityEngine;
#if USE_WEB_REQUEST
#if UNITY_5_3
    using UnityEngine.Experimental.Networking;
#else
    using UnityEngine.Networking;
#endif
#endif
#endif
    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BrainCloudLobby
    {
        public static bool UseHttps { get; set; } = false;
        public Dictionary<string, long> PingData { get; private set; }

        public BrainCloudLobby(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Finds a lobby matching the specified parameters. Asynchronous - returns 200 to indicate that matchmaking has started.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - FindLobby
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="maxSteps">The maximum number of steps to wait when looking for an applicable lobby. Each step is ~5 seconds.</param>
        /// <param name="algo">The algorithm to use for increasing the search scope.</param>
        /// <param name="filterJson">Used to help filter the list of rooms to consider. Passed to the matchmaking filter, if configured.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment</param>

        public void FindLobby(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo, Dictionary<string, object> in_filterJson,
            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbyMaxSteps.Value] = in_maxSteps;
            data[OperationParam.LobbyAlgorithm.Value] = in_algo;
            data[OperationParam.LobbyFilterJson.Value] = in_filterJson;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.FindLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Finds a lobby matching the specified parameters. Asynchronous - returns 200 to indicate that matchmaking has started. Uses attached ping data to resolve best location. GetRegionsForLobbies and PingRegions must be successfully responded to.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - FindLobbyWithPingData
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="maxSteps">The maximum number of steps to wait when looking for an applicable lobby. Each step is ~5 seconds.</param>
        /// <param name="algo">The algorithm to use for increasing the search scope.</param>
        /// <param name="filterJson">Used to help filter the list of rooms to consider. Passed to the matchmaking filter, if configured.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment</param>

        public void FindLobbyWithPingData(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo, Dictionary<string, object> in_filterJson,
            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbyMaxSteps.Value] = in_maxSteps;
            data[OperationParam.LobbyAlgorithm.Value] = in_algo;
            data[OperationParam.LobbyFilterJson.Value] = in_filterJson;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;

            attachPingDataAndSend(data, ServiceOperation.FindLobbyWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Creates a new lobby.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - CreateLobby
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment.</param>
        /// <param name="settings">Configuration data for the room.</param>

        public void CreateLobby(string in_roomType, int in_rating,
            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode,
            Dictionary<string, object> in_settings, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbySettings.Value] = in_settings;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.CreateLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Creates a new lobby. Uses attached ping data to resolve best location. GetRegionsForLobbies and PingRegions must be successfully responded to.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - CreateLobbyWithPingData
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment.</param>
        /// <param name="settings">Configuration data for the room.</param>

        public void CreateLobbyWithPingData(string in_roomType, int in_rating,
            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode,
            Dictionary<string, object> in_settings, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbySettings.Value] = in_settings;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;
            attachPingDataAndSend(data, ServiceOperation.CreateLobbyWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Adds the caller to the lobby entry queue and will create a lobby if none are found.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - FindOrCreateLobby
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="maxSteps">The maximum number of steps to wait when looking for an applicable lobby. Each step is ~5 seconds.</param>
        /// <param name="algo">The algorithm to use for increasing the search scope.</param>
        /// <param name="filterJson">Used to help filter the list of rooms to consider. Passed to the matchmaking filter, if configured.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="settings">Configuration data for the room.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment.</param>

        public void FindOrCreateLobby(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo,
            Dictionary<string, object> in_filterJson,
            bool in_isReady,
            Dictionary<string, object> in_extraJson, string in_teamCode,
            Dictionary<string, object> in_settings, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbyMaxSteps.Value] = in_maxSteps;
            data[OperationParam.LobbyAlgorithm.Value] = in_algo;
            data[OperationParam.LobbyFilterJson.Value] = in_filterJson;
            data[OperationParam.LobbySettings.Value] = in_settings;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.FindOrCreateLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Adds the caller to the lobby entry queue and will create a lobby if none are found. Uses attached ping data to resolve best location. GetRegionsForLobbies and PingRegions must be successfully responded to.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - FindOrCreateLobbyWithPingData
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="rating">The skill rating to use for finding the lobby. Provided as a separate parameter because it may not exactly match the user's rating (especially in cases where parties are involved).</param>
        /// <param name="maxSteps">The maximum number of steps to wait when looking for an applicable lobby. Each step is ~5 seconds.</param>
        /// <param name="algo">The algorithm to use for increasing the search scope.</param>
        /// <param name="filterJson">Used to help filter the list of rooms to consider. Passed to the matchmaking filter, if configured.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>
        /// <param name="settings">Configuration data for the room.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="teamCode">Preferred team for this user, if applicable. Send "" or null for automatic assignment.</param>

        public void FindOrCreateLobbyWithPingData(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo,
            Dictionary<string, object> in_filterJson,
            bool in_isReady,
            Dictionary<string, object> in_extraJson, string in_teamCode,
            Dictionary<string, object> in_settings, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbyMaxSteps.Value] = in_maxSteps;
            data[OperationParam.LobbyAlgorithm.Value] = in_algo;
            data[OperationParam.LobbyFilterJson.Value] = in_filterJson;
            data[OperationParam.LobbySettings.Value] = in_settings;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;

            attachPingDataAndSend(data, ServiceOperation.FindOrCreateLobbyWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Returns the data for the specified lobby, including member data.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - GetLobbyData
        /// </remarks>
        /// <param name="lobbyId">Id of chosen lobby.</param>

        public void GetLobbyData(string in_lobbyID,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetLobbyData, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Updates the ready status and extra json for the given lobby member.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - UpdateReady
        /// </remarks>
        /// <param name="lobbyId">The type of lobby to look for. Lobby types are defined in the portal.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>

        public void UpdateReady(string in_lobbyID, bool in_isReady, Dictionary<string, object> in_extraJson,
                                SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.UpdateReady, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Updates the ready status and extra json for the given lobby member.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - UpdateSettings
        /// </remarks>
        /// <param name="lobbyId">Id of the specfified lobby.</param>
        /// <param name="settings">Configuration data for the room.</param>

        public void UpdateSettings(string in_lobbyID, Dictionary<string, object> in_settings,
                                SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbySettings.Value] = in_settings;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.UpdateSettings, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Switches to the specified team (if allowed.)
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - SwitchTeam
        /// </remarks>
        /// <param name="lobbyId">Id of chosen lobby.</param>
        /// <param name="toTeamCode">Specified team code.</param>

        public void SwitchTeam(string in_lobbyID, string in_toTeamName,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyToTeamName.Value] = in_toTeamName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.SwitchTeam, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends LOBBY_SIGNAL_DATA message to all lobby members.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - SendSignal
        /// </remarks>
        /// <param name="lobbyId">Id of chosen lobby.</param>
        /// <param name="signalData">Signal data to be sent.</param>

        public void SendSignal(string in_lobbyID, Dictionary<string, object> in_signalData, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbySignalData.Value] = in_signalData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.SendSignal, data, callback);
            m_clientRef.SendRequest(sc);
        }


        /// <summary>
        /// Join specified lobby
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - JoinLobby
        /// </remarks>
        /// <param name="lobbyId">Id of the specfified lobby.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="toTeamCode">Specified team code.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>

        public void JoinLobby(string in_lobbyID,
                            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode, string[] in_otherUserCxIds = null,
                            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.JoinLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Join specified lobby. Uses attached ping data to resolve best location. GetRegionsForLobbies and PingRegions must be successfully responded to.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - JoinLobbyWithPingData
        /// </remarks>
        /// <param name="lobbyId">Id of the specfified lobby.</param>
        /// <param name="isReady">Initial ready-status of this user.</param>
        /// <param name="extraJson">Initial extra-data about this user.</param>
        /// <param name="toTeamCode">Specified team code.</param>
        /// <param name="otherUserCxIds">Array of other users (i.e. party members) to add to the lobby as well. Will constrain things so that only lobbies with room for all players will be considered.</param>

        public void JoinLobbyWithPingData(string in_lobbyID,
                            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode, string[] in_otherUserCxIds = null,
                            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (in_otherUserCxIds != null)
            {
                data[OperationParam.LobbyOtherUserCxIds.Value] = in_otherUserCxIds;
            }
            data[OperationParam.LobbyExtraJson.Value] = in_extraJson;
            data[OperationParam.LobbyTeamCode.Value] = in_teamCode;
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyIsReady.Value] = in_isReady;
            attachPingDataAndSend(data, ServiceOperation.JoinLobbyWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Causes the caller to leave the specified lobby. If the user was the owner, a new owner will be chosen. If user was the last member, the lobby will be deleted.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - LeaveLobby
        /// </remarks>
        /// <param name="lobbyId">Id of chosen lobby.</param>

        public void LeaveLobby(string in_lobbyID, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.LeaveLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Evicts the specified user from the specified lobby. The caller must be the owner of the lobby.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - RemoveMember
        /// </remarks>
        /// <param name="lobbyId">Id of chosen lobby.</param>
        /// <param name="cxId">Specified member to be removed from the lobby.</param>

        public void RemoveMember(string in_lobbyID, string in_connectionId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyConnectionId.Value] = in_connectionId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.RemoveMember, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Cancel this members Find, Join and Searching of Lobbies
        /// </summary>
        /// 
        public void CancelFindRequest(string in_roomType, string in_entryId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.EntryId.Value] = in_entryId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.CancelFindRequest, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the region settings for each of the given lobby types. Upon success or afterwards, call pingRegions to start retrieving appropriate data.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - GetRegionsForLobbies
        /// </remarks>
        /// <param name="roomTypes">Ids of the lobby types.</param>

        public void GetRegionsForLobbies(string[] in_roomTypes, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyTypes.Value] = in_roomTypes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(onRegionForLobbiesSuccess + success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetRegionsForLobbies, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets a map keyed by rating of the visible lobby instances matching the given type and rating range.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - GET_LOBBY_INSTANCES
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for.</param>
        /// <param name="criteriaJson">A JSON string used to describe filter criteria.</param>

        public void GetLobbyInstances(string in_lobbyType, Dictionary<string, object> criteriaJson, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_lobbyType;
            data[OperationParam.LobbyCritera.Value] = criteriaJson;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetLobbyInstances, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets a map keyed by rating of the visible lobby instances matching the given type and rating range.
        /// </summary>
        /// <remarks>
        /// Service Name - Lobby
        /// Service Operation - GET_LOBBY_INSTANCES_WITH_PING_DATA
        /// </remarks>
        /// <param name="lobbyType">The type of lobby to look for.</param>
        /// <param name="criteriaJson">A JSON string used to describe filter criteria.</param>

        public void GetLobbyInstancesWithPingData(string in_lobbyType, Dictionary<string, object> criteriaJson,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_lobbyType;
            data[OperationParam.LobbyCritera.Value] = criteriaJson;

            attachPingDataAndSend(data, ServiceOperation.GetLobbyInstancesWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Retrieves associated Ping Data averages to be used with all associated <>WithPingData APIs.
        /// </summary>

        public void PingRegions(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            if (m_pingRegionSuccessCallback != null)
            {
                queueFailure(failure, ReasonCodes.MISSING_REQUIRED_PARAMETER,
                    "Ping is already happening.", cbObject);
                return;
            }

            PingData = new Dictionary<string, long>();

            // Now we have the region ping data, we can start pinging each region and its defined target
            Dictionary<string, object> regionInner = null;
            if (m_regionPingData.Count > 0)
            {
                m_pingRegionSuccessCallback = success;
                m_pingRegionObject = cbObject;

                foreach (var regionMap in m_regionPingData)
                {
                    m_cachedPingResponses[regionMap.Key] = new List<long>();
                    regionInner = (Dictionary<string, object>)regionMap.Value;
                    RegionTarget regionTarget = new RegionTarget
                    {
                        region = regionMap.Key,
                        target = regionInner["target"].ToString(),
                        type = regionInner.ContainsKey("type") ? regionInner["type"].ToString().ToUpper()
                                                               : RegionTarget.PING_TYPE
                    };

                    lock (m_regionTargetsToProcess)
                    {
                        for (int i = 0; i < MAX_PING_CALLS; ++i)
                            m_regionTargetsToProcess.Add(regionTarget);
                    }
                }

                pingNextItemToProcess();
            }
            else
            {
                queueFailure(failure, ReasonCodes.MISSING_REQUIRED_PARAMETER,
                    "No Regions to Ping. Please call GetRegionsForLobbies and await the response before calling PingRegions.", cbObject);
            }
        }

        #region private
        private void pingNextItemToProcess()
        {
            lock (m_regionTargetsToProcess)
            {
                if (m_regionTargetsToProcess.Count > 0)
                {
                    RegionTarget regionTarget = m_regionTargetsToProcess[0];
                    m_regionTargetsToProcess.RemoveAt(0);
                    pingHost(regionTarget);

                    return;
                }
                else if (m_regionPingData.Count == PingData.Count && m_pingRegionSuccessCallback != null)
                {
                    string pingStr = m_clientRef.SerializeJson(PingData);

                    if (m_clientRef.LoggingEnabled)
                    {
                        m_clientRef.Log("PINGS: " + pingStr);
                    }

                    m_pingRegionSuccessCallback(pingStr, m_pingRegionObject);

                    m_pingRegionSuccessCallback = null;
#if !(DOT_NET || GODOT)
                    m_regionTargetIPs.Clear();
#endif
                    return;
                }

                m_pingRegionSuccessCallback = null;
#if !(DOT_NET || GODOT)
                m_regionTargetIPs.Clear();
#endif
            }
        }

        private void attachPingDataAndSend(Dictionary<string, object> in_data, ServiceOperation in_operation,
                                SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            bool hasPingData = PingData != null && PingData.Count > 0;
            if (hasPingData)
            {
                in_data[OperationParam.PingData.Value] = PingData;

                ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
                ServerCall sc = new ServerCall(ServiceName.Lobby, in_operation, in_data, callback);
                m_clientRef.SendRequest(sc);
            }
            else
            {
                queueFailure(failure, ReasonCodes.MISSING_REQUIRED_PARAMETER,
                    "Processing exception (message): Required message parameter 'pingData' is missing.  Please ensure PingData exists by first calling GetRegionsForLobbies and PingRegions, and waiting for response before proceeding.", cbObject);
            }
        }

        private void queueFailure(FailureCallback in_failure, int reasonCode, string status_message, object cbObject = null)
        {
            if (in_failure != null)
            {
                Dictionary<string, object> jsonError = new Dictionary<string, object>();
                jsonError["reason_code"] = reasonCode;
                jsonError["status"] = 400;
                jsonError["status_message"] = status_message;
                jsonError["severity"] = "ERROR";

                Failure failure = new Failure();
                failure.callback = in_failure;
                failure.status = 400;
                failure.reasonCode = reasonCode;
                failure.jsonError = m_clientRef.SerializeJson(jsonError);
                failure.cbObject = cbObject;
                m_failureQueue.Add(failure);
            }
        }

        public void Update()
        {
            // trigger failure events
            for (int i = 0; i < m_failureQueue.Count; ++i)
            {
                Failure failure = m_failureQueue[i];
                failure.callback(failure.status, failure.reasonCode, failure.jsonError, failure.cbObject);
            }
            m_failureQueue.Clear();
        }

        private void onRegionForLobbiesSuccess(string in_json, object in_obj)
        {
            PingData = new Dictionary<string, long>();

            Dictionary<string, object> jsonMessage = (Dictionary<string, object>)JsonReader.Deserialize(in_json);
            Dictionary<string, object> data = (Dictionary<string, object>)jsonMessage["data"];
            m_regionPingData = (Dictionary<string, object>)data["regionPingData"];
            m_lobbyTypeRegions = (Dictionary<string, object>)data["lobbyTypeRegions"];
        }

        private void pingHost(RegionTarget in_regionTarget)
        {
#if DOT_NET || GODOT
            if (in_regionTarget.IsHttpType)
            {
                HandleHTTPResponse(in_regionTarget.region, in_regionTarget.target);
            }
            else
            {
                HandlePingReponse(in_regionTarget.region, in_regionTarget.target);
            }
#else
            if (m_clientRef.Wrapper != null)
            {
#if UNITY_WEBGL
                m_clientRef.Wrapper.StartCoroutine(HandleHTTPResponse(in_regionTarget.region, in_regionTarget.target));
#else
                m_clientRef.Wrapper.StartCoroutine(in_regionTarget.IsHttpType ? HandleHTTPResponse(in_regionTarget.region, in_regionTarget.target)
                                                                              : HandlePingReponse(in_regionTarget.region, in_regionTarget.target));
#endif
            }
#endif
        }

#if DOT_NET || GODOT
        private void HandleHTTPResponse(string in_region, string in_target)
        {
            if (!in_target.StartsWith("http"))
            {
                in_target = (UseHttps ? "https://" : "http://") + in_target;
            }

            DateTime RoundtripTime = DateTime.UtcNow;

            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(100000000); // 10 seconds

            client.GetAsync(in_target).ContinueWith((Task<HttpResponseMessage> task) =>
            {
                if (task.IsCompleted && task.Result is HttpResponseMessage response && response.IsSuccessStatusCode)
                {
                    handlePingTimeResponse((long)(DateTime.UtcNow - RoundtripTime).TotalMilliseconds, in_region);
                }
                else
                {
                    pingNextItemToProcess();
                }

                client.Dispose();
            });
        }

        private void HandlePingReponse(string in_region, string in_target)
        {
            Ping pinger = new Ping();
            try
            {
                pinger.PingCompleted += (o, response) =>
                {
                    if (response.Error == null && response.Reply.Status == IPStatus.Success)
                    {
                        handlePingTimeResponse(response.Reply.RoundtripTime, in_region);
                    }
                    else
                    {
                        pingNextItemToProcess();
                    }
                };

                pinger.SendPingAsync(in_target, 10000);
            }
            catch (Exception) { }
            finally
            {
                pinger?.Dispose();
            }
        }
#else
        private IEnumerator HandleHTTPResponse(string in_region, string in_target)
        {
            if (!in_target.StartsWith("http"))
            {
                in_target = (UseHttps ? "https://" : "http://") + in_target;
            }

            DateTime RoundtripTime = DateTime.UtcNow;
#if USE_WEB_REQUEST
            UnityWebRequest request = UnityWebRequest.Get(in_target);
            request.timeout = 10;

            yield return request.SendWebRequest();
#else
            WWWForm postForm = new WWWForm();
            WWW request = new WWW(in_target, postForm);

            while (!request.isDone && (DateTime.UtcNow - RoundtripTime).TotalMilliseconds < 10000)
            {
                yield return null;
            }
#endif
            if (request.isDone && string.IsNullOrWhiteSpace(request.error))
            {
                handlePingTimeResponse((long)(DateTime.UtcNow - RoundtripTime).TotalMilliseconds, in_region);
            }
            else
            {
                pingNextItemToProcess();
            }

            request.Dispose();
        }

#if !UNITY_WEBGL
        private IEnumerator HandlePingReponse(string in_region, string in_target)
        {
            if (!m_regionTargetIPs.ContainsKey(in_target))
            {
                IPHostEntry host = Dns.GetHostEntry(in_target);
                foreach (IPAddress addresses in host.AddressList)
                {
                    if (addresses.AddressFamily == AddressFamily.InterNetwork)
                    {
                        m_regionTargetIPs.Add(in_target, addresses.ToString());
                        break;
                    }
                }
            }

            if (m_regionTargetIPs.ContainsKey(in_target))
            {
                DateTime ttl = DateTime.UtcNow;
                UnityEngine.Ping ping = new UnityEngine.Ping(m_regionTargetIPs[in_target]);
                while (!ping.isDone && (DateTime.UtcNow - ttl).TotalMilliseconds < 10000)
                {
                    yield return null;
                }

                if (ping.isDone && ping.time > 0)
                {
                    handlePingTimeResponse(ping.time, in_region);
                }
                else
                {
                    pingNextItemToProcess();
                }

                ping.DestroyPing();
            }
            else
            {
                pingNextItemToProcess();
            }
        }
#endif
#endif

        private void handlePingTimeResponse(long in_responseTime, string in_region)
        {
            m_cachedPingResponses[in_region].Add(in_responseTime);
            if (m_cachedPingResponses[in_region].Count == MAX_PING_CALLS)
            {
                long totalAccumulated = 0;
                long highestValue = 0;
                foreach (var pingResponse in m_cachedPingResponses[in_region])
                {
                    totalAccumulated += pingResponse;
                    if (pingResponse > highestValue)
                    {
                        highestValue = pingResponse;
                    }
                }

                // accumulated ALL, now subtract the highest value
                totalAccumulated -= highestValue;
                PingData[in_region] = totalAccumulated / (m_cachedPingResponses[in_region].Count - 1);
            }

            pingNextItemToProcess();
        }

        private Dictionary<string, object> m_regionPingData = new Dictionary<string, object>();
        private Dictionary<string, object> m_lobbyTypeRegions = new Dictionary<string, object>();
        private Dictionary<string, List<long>> m_cachedPingResponses = new Dictionary<string, List<long>>();

        struct RegionTarget
        {
            public const string PING_TYPE = "PING";
            public const string HTTP_TYPE = "HTTP";

            public string region;
            public string target;
            public string type;

            public bool IsPingType => type == PING_TYPE;
            public bool IsHttpType => type == HTTP_TYPE;
        }
        private List<RegionTarget> m_regionTargetsToProcess = new List<RegionTarget>();
        private SuccessCallback m_pingRegionSuccessCallback = null;
        private object m_pingRegionObject = null;

#if !(DOT_NET || GODOT)
        private Dictionary<string, string> m_regionTargetIPs = new Dictionary<string, string>();
#endif

        private const int MAX_PING_CALLS = 4;

        struct Failure
        {
            public FailureCallback callback;
            public int status;
            public int reasonCode;
            public string jsonError;
            public object cbObject;
        }
        private List<Failure> m_failureQueue = new List<Failure>();

        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion
    }
}
