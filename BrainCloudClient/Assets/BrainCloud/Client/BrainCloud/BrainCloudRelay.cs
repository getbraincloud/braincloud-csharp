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
        /// <param name="in_message"></param>
        /// <param name="in_options"></param>
        public void Send(string in_message)
        {
            m_commsLayer.Send(in_message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="in_options"></param>
        public void Send(byte[] in_data)
        {
            m_commsLayer.Send(in_data);
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
