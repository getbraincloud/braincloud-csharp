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

        public void OnSuccessCallback( string responseData )
        {
            if ( m_fnSuccessCallback != null )
            {
                m_fnSuccessCallback(responseData, m_cbObject);
            }
        }

        public void OnErrorCallback(string errorData)
        {
            if ( m_fnFailureCallback != null )
            {
                m_fnFailureCallback(errorData, m_cbObject);
            }
        }
        #endregion
    }
}
