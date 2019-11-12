//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST //Comment out to force use of old WWW class on Unity 5.3+
#endif

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;


#if (DOT_NET)
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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

        // we process the signature on the background thread
        public string Signature { get; set; }

        // we also process the byte array on the background thread
        public byte[] ByteArray { get; set; }

#if !(DOT_NET)
#if USE_WEB_REQUEST
        public UnityWebRequest WebRequest { get; set; }
#else
        // unity uses WWW objects to make http calls cross platform
        public WWW WebRequest { get; set; }
#endif
#else
        // while .net projects can use the WebRequest Object
        public IAsyncResult AsyncResult { get; set; }

        public bool IsCancelled { get; private set; }
        public Task<HttpResponseMessage> WebRequest { get; set; }
#endif

        public string RequestString { get; set; }

#if DOT_NET
        public CancellationTokenSource CancelToken { get; set; }

        public string DotNetResponseString { get; set; }

        private eWebRequestStatus m_dotNetRequestStatus = eWebRequestStatus.STATUS_PENDING;
        internal eWebRequestStatus DotNetRequestStatus
        {
            get { return m_dotNetRequestStatus; }
            set { m_dotNetRequestStatus = value; }
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
#if DOT_NET
                // kill the task - we've timed out
                IsCancelled = true;
                if (WebRequest != null)
                {
                    CancelToken.Cancel();
                }
#else
                CleanupRequest();
#endif
            }
            catch (Exception)
            {
            }
        }

        private void CleanupRequest()
        {
#if USE_WEB_REQUEST
            if (WebRequest == null) return;
            WebRequest.Dispose();
#else

            /* disposing of the www class causes unity editor to lock up
            if (WebRequest != null)
            {
                WebRequest.Dispose();
            }*/
#endif

            WebRequest = null;
        }
    }
}
