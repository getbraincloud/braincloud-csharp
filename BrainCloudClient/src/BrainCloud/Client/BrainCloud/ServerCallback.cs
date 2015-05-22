//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;

namespace BrainCloud
{
//[Serializable]
    public class ServerCallback
    {
        /// ServerCallback
        /// @param in_fnSuccessCallback : SuccessCallback
        /// @param in_fnFailureCallback : FailureCallback
        #region Constructor
        public ServerCallback(SuccessCallback in_fnSuccessCallback, FailureCallback in_fnFailureCallback, object in_cbObject)
        {
            m_fnFailureCallback = in_fnFailureCallback;
            m_fnSuccessCallback = in_fnSuccessCallback;
            m_cbObject = in_cbObject;
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
        #endregion
    }
}
