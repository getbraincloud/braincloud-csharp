//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudLobby
    {
        /// <summary>
        /// 
        /// </summary>
        public BrainCloudLobby(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Finds a lobby matching the specified parameters
        /// </summary>
        ///
        /// otherUserCxIds support coming soon!
        /// 
        public void FindLobby(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo, Dictionary<string, object> in_filterJson, int in_timeoutSecs,
            bool in_isReady, Dictionary<string, object> in_extraJson, string in_teamCode, string[] in_otherUserCxIds = null,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyRating.Value] = in_rating;
            data[OperationParam.LobbyMaxSteps.Value] = in_maxSteps;
            data[OperationParam.LobbyAlgorithm.Value] = in_algo;
            data[OperationParam.LobbyFilterJson.Value] = in_filterJson;
            data[OperationParam.LobbyTimeoutSeconds.Value] = in_timeoutSecs;
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
        /// Like findLobby, but explicitely geared toward creating new lobbies
        /// </summary>
        /// 
        /// otherUserCxIds support coming soon!
        /// 
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
        /// Finds a lobby matching the specified parameters, or creates one
        /// </summary>
        /// 
        /// otherUserCxIds support coming soon!
        /// 
        public void FindOrCreateLobby(string in_roomType, int in_rating, int in_maxSteps,
            Dictionary<string, object> in_algo,
            Dictionary<string, object> in_filterJson, int in_timeoutSecs,
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
            data[OperationParam.LobbyTimeoutSeconds.Value] = in_timeoutSecs;
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
        /// Gets data for the given lobby instance <lobbyId>.
        /// </summary>
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
        /// updates the ready state of the player
        /// </summary>
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
        /// valid only for the owner of the group -- edits the overally lobby config data
        /// </summary>
        public void UpdateLobbyConfig(string in_lobbyID, Dictionary<string, object> in_settings,
                                SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbySettings.Value] = in_settings;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.UpdateLobbyConfig, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// switches to the specified team (if allowed). Note - may be blocked by cloud code script
        /// </summary>
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
        /// sends LOBBY_SIGNAL_DATA message to all lobby members
        /// </summary>
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
        /// User joins the specified lobby. 
        /// </summary>
        ///
        /// Coming soon!
        ///
        /// public void JoinLobby(string in_lobbyID, bool in_isReady, string in_toTeamName, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        /// {
        ///     Dictionary<string, object> data = new Dictionary<string, object>();
        ///     data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
        ///     data[OperationParam.LobbyIsReady.Value] = in_isReady;
        ///     data[OperationParam.LobbyToTeamName.Value] = in_toTeamName;
        /// 
        ///     ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
        ///     ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.JoinLobby, data, callback);
        ///     m_clientRef.SendRequest(sc);
        /// }

        /// <summary>
        /// User leaves the specified lobby. if the user was the owner, a new owner will be chosen
        /// </summary>
        /// 
        public void LeaveLobby(string in_lobbyID, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.LeaveLobby, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Adds a list of users to the specified lobby. 
        /// </summary>
        ///
        /// Coming soon!
        /// 
        /// public void AddOthersToLobby(string in_lobbyID, string [] in_otherUserCxIds, bool in_isReady, string in_toTeamName, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        /// {
        ///     Dictionary<string, object> data = new Dictionary<string, object>();
        ///     data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
        ///     data[OperationParam.LobbyIsReady.Value] = in_isReady;
        ///     data[OperationParam.LobbyToTeamName.Value] = in_toTeamName;
        /// 
        ///     ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
        ///     ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.AddOthersToLobby, data, callback);
        ///     m_clientRef.SendRequest(sc);
        /// }


        /// <summary>
        /// Only valid from the owner of the lobby -- removes the specified member from the lobby
        /// </summary>
        /// 
        public void RemoveMember(string in_lobbyID, string in_connectionId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyIdentifier.Value] = in_lobbyID;
            data[OperationParam.LobbyConnectionId.Value] = in_connectionId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.RemoveMember, data, callback);
            m_clientRef.SendRequest(sc);
        }

        #region private
        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion
    }
}
