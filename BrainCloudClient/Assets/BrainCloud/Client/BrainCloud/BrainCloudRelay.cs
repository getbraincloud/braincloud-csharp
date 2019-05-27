//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
namespace BrainCloud
{
    public class BrainCloudRelay
    {
        /// <summary>
        /// 
        /// </summary>
        internal BrainCloudRelay(RelayComms in_comms)
        {
            m_commsLayer = in_comms;
        }

        /// <summary>
        /// Use Ping() in order to properly calculate the Last ping received
        /// </summary>
        public long LastPing { get { return m_commsLayer.Ping; } }

        /// <summary>
        /// NetId retrieved from the connected call
        /// </summary>
        public short NetId { get { return m_commsLayer.NetId; } }

        /// <summary>
        /// Start off a connection, based off connection type to brainClouds Relay Servers.  Connect options come in from "ROOM_ASSIGNED" lobby callback
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_options">
        ///             in_options["ssl"] = false;
        ///             in_options["host"] = "168.0.1.192";
        ///             in_options["port"] = 9000;
        ///             in_options["passcode"] = "somePasscode"
        ///             in_options["lobbyId"] = "55555:v5v:001";
        ///</param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(RelayConnectionType in_connectionType = RelayConnectionType.WEBSOCKET, Dictionary<string, object> in_options = null, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            m_commsLayer.Connect(in_connectionType, in_options, in_success, in_failure, cb_object);
        }

        /// <summary>
        /// Disables relay event for this session.
        /// </summary>
        public void Disconnect()
        {
            m_commsLayer.Disconnect();
        }

        /// <summary>
        /// Is Connected
        /// </summary>
        public bool IsConnected()
        {
            return m_commsLayer.IsConnected();
        }

        /// <summary>
        /// Register callback, so that data is received on the main thread
        /// </summary>
        public void RegisterDataCallback(RSDataCallback in_callback)
        {
            m_commsLayer.RegisterDataCallback(in_callback);
        }

        /// <summary>
        /// Deregister the data callback
        /// </summary>
        public void DeregisterDataCallback()
        {
            m_commsLayer.DeregisterDataCallback();
        }

        /// <summary>
        /// send byte array representation of data
        /// </summary>
        /// <param in_message="message to be sent"></param>
        /// <param to_netId="the net id to send to, RelayComms.TO_ALL_PLAYERS to relay to all"></param>
        /// <param in_reliable="send this reliably or not"></param>
        /// <param in_ordered="received this ordered or not"></param>
        /// <param in_channel="0,1,2,3 (max of four channels)"></param>
        public void Send(byte[] in_data, short to_netId, bool in_reliable = true, bool in_ordered = true, int in_channel = 0)
        {
            m_commsLayer.Send(in_data, to_netId, in_reliable, in_ordered, in_channel);
        }

        /// <summary>
        /// Set the ping interval.
        /// </summary>
        public void SetPingInterval(float in_interval)
        {
            m_commsLayer.SetPingInterval(in_interval);
        }

        #region private
        /// <summary>
        /// Reference to the Relay Comms
        /// </summary>
        private RelayComms m_commsLayer;
        #endregion

    }
}
