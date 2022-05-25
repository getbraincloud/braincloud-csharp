//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

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

        public void AddCallbacks(ServerCallback in_callback)
        {
            //Adding Successes
            MulticastDelegate successToAdd = in_callback.m_fnSuccessCallback;
            var successList = successToAdd.GetInvocationList();
            for (int i = 0; i < successList.Length; i++)
            {
                m_fnSuccessCallback += successList[i] as SuccessCallback;
            }
            
            //Adding Failures
            MulticastDelegate failureToAdd = in_callback.m_fnFailureCallback;
            var failureList = failureToAdd.GetInvocationList();
            for (int i = 0; i < failureList.Length; i++)
            {
                m_fnFailureCallback += failureList[i] as FailureCallback;
            }
        }

        public bool AreCallbacksNull()
        {
            return m_fnSuccessCallback == null && m_fnFailureCallback == null;
        }
        #endregion
    }
}
