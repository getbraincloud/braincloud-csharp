// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST // Comment out to force use of old WWW class on Unity 5.3+
#endif

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
#if DOT_NET || GODOT
    using System.Net.Http;
    using System.Threading;
#else
#if USE_WEB_REQUEST
#if UNITY_5_3
    using UnityEngine.Experimental.Networking;
#else
    using UnityEngine.Networking;
#endif
#endif
    using UnityEngine;
#endif

    // This class stores the request state of the request.
    public class RequestState
    {
        internal enum eWebRequestStatus
        {
            /// <summary>
            /// Pending status indicating web request is still active
            /// </summary>
            STATUS_PENDING = 0,

            /// <summary>
            /// Done status indicating web request has completed successfully
            /// </summary>
            STATUS_DONE = 1,

            /// <summary>
            /// Error status indicating there was a network error or error http code returned
            /// </summary>
            STATUS_ERROR = 2
        }

        public long PacketId { get; set; }

        public DateTime TimeSent { get; set; }

        public int Retries { get; set; }

        public string Signature { get; set; } // We process the signature on the background thread

        public byte[] ByteArray { get; set; } // We also process the byte array on the background thread

        public string RequestString { get; set; }

#if !(DOT_NET || GODOT)
#if USE_WEB_REQUEST
        public UnityWebRequest WebRequest { get; set; }
#else
        public WWW WebRequest { get; set; } // Unity uses WWW objects to make HTTP calls cross-platform
#endif
#endif

#if DOT_NET || GODOT
        public bool IsCancelled { get; private set; }
        public HttpResponseMessage WebRequest { get; set; }
        public CancellationTokenSource CancelToken { get; set; }

        public string DotNetResponseString { get; set; }

        private volatile eWebRequestStatus m_dotNetRequestStatus = eWebRequestStatus.STATUS_PENDING;
        internal eWebRequestStatus DotNetRequestStatus
        {
            get { return m_dotNetRequestStatus; }
            set { m_dotNetRequestStatus = value; }
        }

        private volatile HttpResult m_requestResult = null;
        internal HttpResult RequestResult
        {
            get { return m_requestResult; }
            set { m_requestResult = value; }
        }
#endif
        public List<object> MessageList { get; set; }

        public bool LoseThisPacket { get; set; }

        public bool PacketRequiresLongTimeout { get; set; }

        public bool PacketNoRetry { get; set; }

        public RequestState()
        {
            CleanupRequest();
        }

        public void CancelRequest()
        {
            try
            {
#if DOT_NET || GODOT
                // Kill the task - we've timed out
                IsCancelled = true;
                if (WebRequest != null)
                {
                    CancelToken.Cancel();
                }
#endif
            }
            catch (Exception) { }

            CleanupRequest();
        }

        private void CleanupRequest()
        {
            if (WebRequest == null)
            {
                return;
            }
#if USE_WEB_REQUEST || DOT_NET || GODOT
            WebRequest.Dispose();
#else
            /* Disposing of the www class causes unity editor to lock up
            if (WebRequest != null)
            {
                WebRequest.Dispose(); // TODO: We should test to see how this fairs if we Dispose in the .exe instead of the Editor
            }*/
#endif
            WebRequest = null;
        }
    }
}
