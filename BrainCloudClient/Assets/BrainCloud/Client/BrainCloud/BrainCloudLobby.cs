//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------
#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST //Comment out to force use of old WWW class on Unity 5.3+
#endif

namespace BrainCloud
{
#if USE_WEB_REQUEST
#if UNITY_5_3
using UnityEngine.Experimental.Networking;
#else
    using UnityEngine.Networking;
#endif
#endif

    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class BrainCloudLobby
    {
        public Dictionary<string, long> PingData { get; private set; }

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
        /// Finds a lobby matching the specified parameters WITH PING DATA.  GetRegionsForLobbies and PingRegions must be successfully responded to
        /// prior to calling.
        /// </summary>
        /// 
        public void FindLobbyWithPingData(string in_roomType, int in_rating, int in_maxSteps,
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

            attachPingDataAndSend(data, ServiceOperation.FindLobbyWithPingData, success, failure, cbObject);
        }

        /// <summary>
        /// Like findLobby, but explicitely geared toward creating new lobbies
        /// </summary>
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
        /// Like findLobby, but explicitely geared toward creating new lobbies WITH PING DATA.  GetRegionsForLobbies and PingRegions must be successfully responded to
        /// prior to calling.
        /// </summary>
        /// 
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
        /// Finds a lobby matching the specified parameters, or creates one
        /// </summary>
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
        /// Finds a lobby matching the specified parameters, or creates one WITH PING DATA.  GetRegionsForLobbies and PingRegions must be successfully responded to
        /// prior to calling.
        /// </summary>
        /// 
        public void FindOrCreateLobbyWithPingData(string in_roomType, int in_rating, int in_maxSteps,
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

            attachPingDataAndSend(data, ServiceOperation.FindOrCreateLobbyWithPingData, success, failure, cbObject);
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
        /// User joins the specified lobby WITH PING DATA.  GetRegionsForLobbies and PingRegions must be successfully responded to
        /// prior to calling.
        /// </summary>
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

        /// <summary>
        /// Cancel this members Find, Join and Searching of Lobbies
        /// </summary>
        /// 
        public void CancelFindRequest(string in_roomType, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_roomType;
            data[OperationParam.LobbyConnectionId.Value] = m_clientRef.RTTConnectionID;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.CancelFindRequest, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the region settings for each of the given lobby types. Upon SuccessCallback or afterwards, call PingRegions to start retrieving appropriate data.  
        /// Once that completes, the associated region Ping Data is retrievable via PingData and all associated <>WithPingData APIs are useable
        /// </summary>
        /// 
        public void GetRegionsForLobbies(string[] in_roomTypes, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyTypes.Value] = in_roomTypes;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(onRegionForLobbiesSuccess + success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetRegionsForLobbies, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /**
         * Gets a map keyed by rating of the visible lobby instances matching the given type and rating range.
         * any ping data provided in the criteriaJson will be ignored.
         *
         * Service Name - Lobby
         * Service Operation - GetLobbyInstances
         *
         * @param lobbyType The type of lobby to look for.
         * @param criteriaJson A JSON object used to describe filter criteria.
         */
        public void GetLobbyInstances(string in_lobbyType, Dictionary<string, object> criteriaJson, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_lobbyType;
            data[OperationParam.LobbyCritera.Value] = criteriaJson;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetLobbyInstances, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /**
         * Gets a map keyed by rating of the visible lobby instances matching the given type and rating range.
         * Only lobby instances in the regions that satisfy the ping portion of the criteriaJson (based on the values provided in pingData) will be returned.
         *
         * Service Name - Lobby
         * Service Operation - GetLobbyInstancesWithPingData
         *
         * @param lobbyType The type of lobby to look for.
         * @param criteriaJson A JSON object used to describe filter criteria.
         */
        public void GetLobbyInstancesWithPingData(string in_lobbyType, Dictionary<string,object> criteriaJson,
            SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.LobbyRoomType.Value] = in_lobbyType;
            data[OperationParam.LobbyCritera.Value] = criteriaJson;
            
            attachPingDataAndSend(data, ServiceOperation.GetLobbyInstancesWithPingData, success, failure, cbObject);
        }
        
        /// <summary>
        /// Retrieves associated PingData averages to be used with all associated <>WithPingData APIs.
        /// Call anytime after GetRegionsForLobbies before proceeding. 
        /// </summary>
        /// 
        public void PingRegions(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            if (m_pingRegionSuccessCallback != null)
            {
                queueFailure(failure, ReasonCodes.MISSING_REQUIRED_PARAMETER,
                    "Ping is already happening.", cbObject);
                return;
            }

            PingData = new Dictionary<string, long>();

            // now we have the region ping data, we can start pinging each region and its defined target, if its a PING type.
            Dictionary<string, object> regionInner = null;
            string targetStr = "";
            if (m_regionPingData.Count > 0)
            {
                m_pingRegionSuccessCallback = success;
                m_pingRegionObject = cbObject;

                foreach (var regionMap in m_regionPingData)
                {
                    regionInner = (Dictionary<string, object>)regionMap.Value;

                    if (regionInner.ContainsKey("type") && regionInner["type"] as string == "PING")
                    {
                        m_cachedPingResponses[regionMap.Key] = new List<long>();
                        targetStr = (string)regionInner["target"];

                        lock (m_regionTargetsToProcess)
                        {
                            for (int i = 0; i < MAX_PING_CALLS; ++i)
                                m_regionTargetsToProcess.Add(new KeyValuePair<string, string>(regionMap.Key, targetStr));
                        }
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
            lock(m_regionTargetsToProcess)
            {
                if (m_regionTargetsToProcess.Count > 0)
                {
                    for (int i = 0; i < NUM_PING_CALLS_IN_PARRALLEL && m_regionTargetsToProcess.Count > 0; ++i)
                    {
                        KeyValuePair<string, string> pair = m_regionTargetsToProcess[0];
                        m_regionTargetsToProcess.RemoveAt(0);
                        pingHost(pair.Key, pair.Value);
                    }   
                }
                else if (m_regionPingData.Count == PingData.Count && m_pingRegionSuccessCallback != null)
                {
                    string pingStr = JsonWriter.Serialize(PingData);
                    
                    if (m_clientRef.LoggingEnabled)
                    {
                        m_clientRef.Log("PINGS: " + pingStr);
                    }

                    m_pingRegionSuccessCallback(pingStr, m_pingRegionObject);
                    m_pingRegionSuccessCallback = null;
                }
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
                failure.jsonError = JsonWriter.Serialize(jsonError);
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

        private void pingHost(string in_region, string in_target)
        {
#if DOT_NET
            PingUpdateSystem(in_region, in_target);
#else       
            in_target = "http://" + in_target;

            if (m_clientRef.Wrapper != null)
                m_clientRef.Wrapper.StartCoroutine(HandlePingReponse(in_region, in_target));
#endif
        }

#if DOT_NET
        private void PingUpdateSystem(string in_region, string in_target)
        {
            System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
            try
            {
                pinger.PingCompleted += (o, e) =>
                {
                    if (e.Error == null && e.Reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        handlePingTimeResponse(e.Reply.RoundtripTime, in_region);
                    }
                };

                pinger.SendAsync(in_target, null);
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
        }
#else
        private IEnumerator HandlePingReponse(string in_region, string in_target)
        {
            long sentPing = DateTime.Now.Ticks;
#if USE_WEB_REQUEST
            UnityWebRequest _request = UnityWebRequest.Get(in_target);
            yield return _request.SendWebRequest();
#else
            WWWForm postForm = new WWWForm();
            WWW _request = new WWW(in_target, postForm);
#endif
            if (_request.error == null && !_request.isNetworkError)
            {
                handlePingTimeResponse((DateTime.Now.Ticks - sentPing) / 10000, in_region);
            }
        }
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
        private List<KeyValuePair<string, string>> m_regionTargetsToProcess = new List<KeyValuePair<string, string>>();
        private SuccessCallback m_pingRegionSuccessCallback = null;
        private object m_pingRegionObject = null;

        private const int MAX_PING_CALLS = 4;
        private const int NUM_PING_CALLS_IN_PARRALLEL = 2;

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
