//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using BrainCloud.Internal;

    public class BrainCloudRTT
    {
        /// <summary>
        /// 
        /// </summary>
        internal BrainCloudRTT(RTTComms in_comms, BrainCloudClient in_client)
        {
            m_commsLayer = in_comms;
            m_clientRef = in_client;
        }

        /// <summary>
        /// Enables Real Time event for this session.
        /// Real Time events are disabled by default. Usually events
        /// need to be polled using GET_EVENTS. By enabling this, events will
        /// be received instantly when they happen through a TCP connection to an Event Server.
        ///
        ///This function will first call requestClientConnection, then connect to the address
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void EnableRTT(RTTConnectionType in_connectionType = RTTConnectionType.WEBSOCKET, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            m_commsLayer.EnableRTT(in_connectionType, in_success, in_failure, cb_object);
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
        /// 
        /// </summary>
        public void RegisterRTTEventCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Event, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTEventCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Event);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTChatCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Chat, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTChatCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Chat);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTPresenceCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Presence, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTPresenceCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Presence);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTMessagingCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Messaging, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTMessagingCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Messaging);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTLobbyCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.Lobby, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTLobbyCallback()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.Lobby);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTAsyncMatchCallback(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.AsyncMatch, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTBlockchainRefresh(RTTCallback in_callback)
        {
            m_commsLayer.RegisterRTTCallback(ServiceName.UserItems, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTBlockchainRefresh()
        {
            m_commsLayer.DeregisterRTTCallback(ServiceName.UserItems);
        }

        /// <summary>
        /// 
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
        public void RequestClientConnection(  SuccessCallback success = null, FailureCallback failure = null,  object cbObject = null)
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
