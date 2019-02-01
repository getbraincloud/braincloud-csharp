//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
namespace BrainCloud
{
    public class BrainCloudRoomServer
    {
        /// <summary>
        /// 
        /// </summary>
        internal BrainCloudRoomServer(RSComms in_comms)
        {
            m_commsLayer = in_comms;
        }

        /// <summary>
        /// Use Echo() in order to properly calculate the Last ping received
        /// </summary>
        public long LastPing { get { return m_commsLayer.LastPing; } }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(eRSConnectionType in_connectionType = eRSConnectionType.WEBSOCKET, Dictionary<string, object> in_options = null, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
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
        public void RegisterCallback(RSCallback in_callback)
        {
            m_commsLayer.RegisterCallback(in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterCallback()
        {
            m_commsLayer.DeregisterCallback();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="in_message"></param>
        /// <param name="in_options"></param>
        public void Send(string in_message, Dictionary<string, object> in_options = null)
        {
            m_commsLayer.Send(in_message, in_options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="in_message"></param>
        /// <param name="in_options"></param>
        public void Echo(string in_message, Dictionary<string, object> in_options = null)
        {
            m_commsLayer.Echo(in_message, in_options);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Ping()
        {
            m_commsLayer.Ping();
        }

        #region private
        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private RSComms m_commsLayer;
        #endregion

    }
}
