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
        ///
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(RelayConnectionType in_connectionType = RelayConnectionType.WEBSOCKET, Dictionary<string, object> in_options = null, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            m_commsLayer.Connect(in_connectionType, in_options, in_success, in_failure, cb_object);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Disconnect()
        {
            m_commsLayer.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterDataCallback(RSDataCallback in_callback)
        {
            m_commsLayer.RegisterDataCallback(in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterDataCallback()
        {
            m_commsLayer.DeregisterDataCallback();
        }

        /// <summary>
        /// 
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
        /// 
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
