// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using BrainCloud.Internal;

    public class BrainCloudRTT
    {
        /// <summary>
        /// Listen to real time events.
        /// </summary>


        internal BrainCloudRTT(RTTComms in_comms, BrainCloudClient in_client)
        {
            m_commsLayer = in_comms;
            m_clientRef = in_client;
        }

        /// <summary>
        /// Enables Real Time event for this session.
        /// </summary>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        /// <param name="useWebSocket">Use web sockets instead of TCP for the internal connections. Default is true</param>


        public void EnableRTT(SuccessCallback in_success, FailureCallback in_failure, RTTConnectionType in_connectionType = RTTConnectionType.WEBSOCKET, object cb_object = null)
        {
            m_commsLayer.EnableRTT(in_success, in_failure, in_connectionType, cb_object);
        }

        /// <summary>
        /// Disables Real Time event for this session.
        /// </summary>
        public void DisableRTT()
        {
            m_commsLayer.DisableRTT();
        }

        /// <summary>
        /// Returns true if RTT is enabled
        /// </summary>
        public bool IsRTTEnabled()
        {
            return m_commsLayer.IsRTTEnabled();
        }

        /// <summary>
        /// Returns rtt connectionstatus
        /// </summary>
        public RTTConnectionStatus GetConnectionStatus()
        {
            return m_commsLayer.GetConnectionStatus();
        }

        /// <summary>
        /// Listen to real time events.
        /// </summary>

        public void RegisterRTTEventCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Event, in_callback);
        }

        /// <summary>
        /// Listen to real time messaging.
        /// </summary>


        public void DeregisterRTTEventCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Event);
        }

        /// <summary>
        /// Listen to real time chat messages.
        /// </summary>

        public void RegisterRTTChatCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Chat, in_callback);
        }

        /// <summary>
        /// Listen to real time presence events.
        /// </summary>


        public void DeregisterRTTChatCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Chat);
        }

        /// <summary>
        /// Listen to real time presence events.
        /// </summary>

        public void RegisterRTTPresenceCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Presence, in_callback);
        }

        /// <summary>
        /// Listen to real time blockchain events.
        /// </summary>

        public void DeregisterRTTPresenceCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Presence);
        }

        /// <summary>
        /// Listen to real time messaging.
        /// </summary>
        public void RegisterRTTMessagingCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Messaging, in_callback);
        }

        /// <summary>
        /// Listen to real time lobby events.
        /// </summary>


        public void DeregisterRTTMessagingCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Messaging);
        }

        /// <summary>
        /// Listen to real time lobby events.
        /// </summary>

        public void RegisterRTTLobbyCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Lobby, in_callback);
        }

        /// <summary>
        /// Listen to real time blockchain events.
        /// </summary>
        public void DeregisterRTTLobbyCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Lobby);
        }

        /// <summary>
        /// Listen to real time blockchain events.
        /// </summary>

        public void RegisterRTTAsyncMatchCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.AsyncMatch, in_callback);
        }

        /// <summary>
        /// Listen to real time blockchain events.
        /// </summary>
        public void RegisterRTTBlockchainRefresh(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.UserItems, in_callback);
        }

        /// <summary>
        /// Clear all set RTT callbacks
        /// </summary>

        public void DeregisterRTTBlockchainRefresh()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.UserItems);
        }

        /// <summary>
        /// Clear all set RTT callbacks
        /// </summary>

        public void RegisterRTTBlockchainItemEvent(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.BlockChain, in_callback);
        }

        /// <summary>
        /// Clear all set RTT callbacks
        /// </summary>

        public void DeregisterRTTBlockchainItemEvent()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.BlockChain);
        }
        /// <summary>
        /// Clear all set RTT callbacks
        /// </summary>

        public void DeregisterRTTAsyncMatchCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.AsyncMatch);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterAllRTTCallbacks()
        {
            m_commsLayer.DeregisterAllRTTCallbacks();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetRTTHeartBeatSeconds(int in_value)
        {
            m_commsLayer.SetRTTHeartBeatSeconds(in_value);
        }

        /// <summary>
        /// Requests the event server address
        /// </summary>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void RequestClientConnection(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.RTTRegistration, ServiceOperation.RequestClientConnection, null, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        public string getRTTConnectionID()
        {
            return m_commsLayer.RTTConnectionID;
        }

        #region private

        /// <summary>
        /// Reference to the RTT Comms
        /// </summary>
        private RTTComms m_commsLayer;

        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion

    }
}
