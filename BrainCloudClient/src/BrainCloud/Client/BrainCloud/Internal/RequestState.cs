//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;

#if (DOT_NET)
using System.Net;
#else
using UnityEngine;
#endif


namespace BrainCloud.Internal
{
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


        private long m_packetId;
        public long PacketId
        {
            get
            {
                return m_packetId;
            }
            set
            {
                m_packetId = value;
            }
        }

        private DateTime m_timeSent;
        public DateTime TimeSent
        {
            get
            {
                return m_timeSent;
            }
            set
            {
                m_timeSent = value;
            }
        }

        private int m_retries;
        public int Retries
        {
            get
            {
                return m_retries;
            }
            set
            {
                m_retries = value;
            }
        }

        // we process the signature on the background thread
        private string m_sig = "";
        public string Signature
        {
            get
            {
                return m_sig;
            }
            set
            {
                m_sig = value;
            }
        }

        // we also process the byte array on the background thread
        private byte[] m_byteArray = null;
        public byte[] ByteArray
        {
            get
            {
                return m_byteArray;
            }
            set
            {
                m_byteArray = value;
            }
        }

#if !(DOT_NET)
        // unity uses WWW objects to make http calls cross platform
        private WWW request;
        public WWW WebRequest
#else
        // while .net projects can use the WebRequest Object
        private IAsyncResult m_asyncResult;
        public IAsyncResult AsyncResult
        {
            get
            {
                return m_asyncResult;
            }
            set
            {
                m_asyncResult = value;
            }
        }

        private WebRequest request;

        private bool m_isCancelled = false;
        public bool IsCancelled
        {
            get
            {
                return m_isCancelled;
            }
        }
        public WebRequest WebRequest
#endif
        {
            get
            {
                return request;
            }
            set
            {
                request = value;
            }
        }

        private string m_requestString = "";
        public string RequestString
        {
            get
            {
                return m_requestString;
            }
            set
            {
                m_requestString = value;
            }
        }

#if DOT_NET
        private string m_dotNetesponseString = "";
        public string DotNetResponseString
        {
            get
            {
                return m_dotNetesponseString;
            }
            set
            {
                m_dotNetesponseString = value;
            }
        }

        private eWebRequestStatus m_dotNetRequestStatus = eWebRequestStatus.STATUS_PENDING;
        internal eWebRequestStatus DotNetRequestStatus
        {
            get
            {
                return m_dotNetRequestStatus;
            }
            set
            {
                m_dotNetRequestStatus = value;
            }
        }
#endif

        private List<object> m_messageList;
        public List<object> MessageList
        {
            get
            {
                return m_messageList;
            }
            set
            {
                m_messageList = value;
            }
        }

        private bool m_loseThisPacket;
        public bool LoseThisPacket
        {
            get
            {
                return m_loseThisPacket;
            }
            set
            {
                m_loseThisPacket = value;
            }
        }

        private bool m_packetRequiresLongTimeout = false;
        public bool PacketRequiresLongTimeout
        {
            get
            {
                return m_packetRequiresLongTimeout;
            }
            set
            {
                m_packetRequiresLongTimeout = value;
            }
        }

        private bool m_packetNoRetry = false;
        public bool PacketNoRetry
        {
            get
            {
                return m_packetNoRetry;
            }
            set
            {
                m_packetNoRetry = value;
            }
        }

        public RequestState()
        {
            request = null;
        }

        public void CancelRequest()
        {
            try
            {
#if DOT_NET
                // kill the task - we've timed out
                m_isCancelled = true;
                if (WebRequest != null)
                {

                    WebRequest.Abort();
                }
#else
                /* disposing of the www class causes unity editor to lock up
                if (WebRequest != null)
                {
                    WebRequest.Dispose();
                }*/
#endif
            }
            catch (Exception)
            {
            }
        }
    }
}
