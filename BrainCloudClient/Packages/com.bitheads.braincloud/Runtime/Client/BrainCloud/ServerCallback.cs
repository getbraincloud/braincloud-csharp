//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using BrainCloud.Internal;

namespace BrainCloud
{
    using System;
//[Serializable]
    public class ServerCallback
    {
        /// ServerCallback
        /// @param fnSuccessCallback : SuccessCallback
        /// @param fnFailureCallback : FailureCallback
        #region Constructor
        public ServerCallback(SuccessCallback fnSuccessCallback, FailureCallback fnFailureCallback, object cbObject)
        {
            m_fnFailureCallback = fnFailureCallback;
            m_fnSuccessCallback = fnSuccessCallback;
            m_cbObject = cbObject;
        }
        #endregion

        #region Callback Delegate Definitions And Implementation
        public event SuccessCallback m_fnSuccessCallback;
        public event FailureCallback m_fnFailureCallback;
        public object m_cbObject;

        public void OnSuccessCallback( string jsonResponse )
        {
            if ( m_fnSuccessCallback != null )
            {
                m_fnSuccessCallback(jsonResponse, m_cbObject);
            }
        }

        public void OnErrorCallback(int statusCode, int reasonCode, string statusMessage)
        {
            if ( m_fnFailureCallback != null )
            {
                m_fnFailureCallback(statusCode, reasonCode, statusMessage, m_cbObject);
            }
        }
        
        //This function can only add callbacks for Authenticate requests. 
        public void AddAuthCallbacks(ServerCallback in_callback)
        {
            WrapperAuthCallbackObject callbackObject = in_callback.m_cbObject as WrapperAuthCallbackObject;
            if (callbackObject == null) return;
            
            m_fnSuccessCallback += callbackObject._successCallback;
            m_fnFailureCallback += callbackObject._failureCallback;
        }

        public bool AreCallbacksNull()
        {
            return m_fnSuccessCallback == null && m_fnFailureCallback == null;
        }
        #endregion
    }
}
