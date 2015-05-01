//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

#if (DOT_NET)
using System.Net;
using System.Web;
using System.Threading;
using System.Diagnostics;
#else
using UnityEngine;
#endif

using System.IO;
using LitJson;
using BrainCloud;

namespace BrainCloud.Internal
{

    #region Processed Server Call Class
    public class ServerCallProcessed
    {
        internal ServerCall ServerCall
        {
            get;
            set;
        }
        public string Data
        {
            get;
            set;
        }
    }
    #endregion

//[Serializable]
    internal sealed class BrainCloudComms
    {
        #region Private Member Variables

        private static List<ServerCall> m_serviceCallsWaiting = new List<ServerCall>();
        private static List<ServerCall> m_serviceCallsInProgress = new List<ServerCall>();

        private static List<ServerCallProcessed> m_serviceCallsSuccessful = new List<ServerCallProcessed>();
        private static List<ServerCallProcessed> m_serviceCallsErroneous = new List<ServerCallProcessed>();

#if !(DOT_NET)
        private static List<RequestState> m_activeRequestStates = new List<RequestState>();
        private static List<RequestState> m_toSendRequestStates = new List<RequestState>();
#endif

        private object  m_processingEvent = new object();
        private volatile bool m_processingBool = false;
#if (DOT_NET)
        private volatile bool m_processThread = true;
#endif

        private int m_heartBeatInterval = 60000;
        private ITimer m_heartBeatTimer;
        private ITimer m_timeoutTimer;

        private int m_numRetry;
        private BrainCloudClient m_brainCloudClientRef;
        private DateTime m_lastpacketSend;
        private long m_idleInterval;
        private bool m_initialized = false;
        #endregion

        public BrainCloudComms(BrainCloudClient in_client)
        {
            // used for other unity platforms???
#if (DOT_NET)
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
#endif
            m_brainCloudClientRef = in_client;
        }

        #region Initialize Cloud

        public void Initialize(string serverURL, string secretKey)
        {
            m_packetId = 0;
            m_serverURL = serverURL;
            m_secretKey = secretKey;

            // for now tick every half second
            if (!m_initialized)
                EnableComms(true);

            m_numRetry = 0;
            m_triggeredForReset = false;

            m_initialized = true;
        }

        #endregion

        #region Properties

        private bool m_triggeredForReset;
        //private bool m_triggeredEnableComms = false;
        //private bool m_commsTrigger = false;

        private bool m_isAuthenticated;
        public bool Authenticated
        {
            get
            {
                return m_isAuthenticated;    //no public "set"
            }
        }

        private ulong m_packetId;
        public ulong PacketId
        {
            get
            {
                return m_packetId;    //no public "set"
            }
        }
        private string m_sessionID;
        public string SessionID
        {
            get
            {
                return m_sessionID;    //no public "set"
            }
        }

        private string m_userID;
        public string UserID
        {
            get
            {
                return m_userID;    //no public "set"
            }
        }

        private string m_serverURL = "";
        public string ServerURL
        {
            get
            {
                return m_serverURL;    //no public "set"
            }
        }

        private string m_secretKey = "";
        public string SecretKey
        {
            get
            {
                return m_secretKey;    //no public "set"
            }
        }

        #endregion



        #region Public Methods
        public void SetHeartbeatInterval(int milliseconds)
        {
            m_heartBeatInterval = milliseconds;
        }

        internal void AddToQueue(ServerCall call)
        {
            lock (m_serviceCallsWaiting)
            {
                m_serviceCallsWaiting.Add(call);
                //m_brainCloudClientRef.Log("AddToQueue --- " + call.GetService() + "  " + call.GetOperation() + "  count = " + m_serviceCallsWaiting.Count);
            }
            // the calls are actually processed on the background thread in .net
            // while in unity they are processed on the main thread and spat out using their
            // WWW class to make aSync calls
        }

        public void Update()
        {
            bool enableComms = false;

            lock (m_processingEvent)
            {
#if !(DOT_NET)
                if (m_processingBool)
                {
                    ProcessQueueHelper();
                }

                // unity responses have to be done on the main loop
                // but we still process them off of the main thread
                lock (m_toSendRequestStates)
                {
                    lock (m_activeRequestStates)
                    {
                        if (m_toSendRequestStates.Count > 0)
                        {
                            List<RequestState> itemsProcessed = new List<RequestState>();
                            RequestState tempRequest;
                            for (int i = 0; i < m_toSendRequestStates.Count; ++i)
                            {
                                try
                                {
                                    tempRequest = m_toSendRequestStates[i];
                                    if (tempRequest != null)
                                    {
                                        //send the WWW response on the main thread
                                        //Hashtable formTable = new Hashtable();
                                        //formTable.Add("Content-Type", "application/json; charset=utf-8");
                                        //formTable.Add("X-SIG", tempRequest.Signature);
                                        //WWW request = new WWW(m_serverURL, tempRequest.ByteArray, formTable);

                                        // [dsl] Latest of Unity uses Dictionary instead of Hashtable. Apparently it's better.
                                        // More info here: http://stackoverflow.com/questions/301371/why-is-dictionary-preferred-over-hashtable
                                        Dictionary<string, string> formTable = new Dictionary<string, string>();
                                        formTable["Content-Type"] = "application/json; charset=utf-8";
                                        formTable["X-SIG"] = tempRequest.Signature;
                                        WWW request = new WWW(m_serverURL, tempRequest.ByteArray, formTable);
                                        m_brainCloudClientRef.Log("GOING OUT URL: " +m_serverURL);

                                        tempRequest.WebRequest = request;
                                        m_activeRequestStates.Add(tempRequest);
                                        m_brainCloudClientRef.Log("RequestState - JsonRequeststring GOING OUT: " + tempRequest.Jsonstring);
                                        itemsProcessed.Add(tempRequest);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    m_brainCloudClientRef.Log("BeginGetRequestStream - Exception: " + ex.ToString());
                                    //HandleErrorCallInProgressCalls(ex.ToString());
                                    //m_toSendRequestStates.Clear();
                                    //itemsToRemoveToSend.Add(tempRequest);
                                }// end try catch
                            }// end for

                            // we send them from here
                            if (itemsProcessed.Count > 0)
                            {
                                EnableTimeOutTimer(true);
                            }

                            for (int i = 0; i < itemsProcessed.Count; ++i )
                                m_toSendRequestStates.Remove(itemsProcessed[i]);
                        }
                    }
                }

                // process all active request
                List<RequestState> itemsToRemove = new List<RequestState>();
                bool bNeedsRetry = false;
                List<RequestState> listToRetry = new List<RequestState>();

                lock (m_activeRequestStates)
                {
                    if (m_activeRequestStates.Count > 0)
                    {
                        RequestState tempRequest;
                        //Response tempResponse;
                        for (int i = 0; i < m_activeRequestStates.Count; ++i)
                        {
                            tempRequest = m_activeRequestStates[i];
                            try
                            {
                                if (tempRequest != null && tempRequest.WebRequest != null)
                                {
                                    if (tempRequest.WebRequest.error != null)
                                    {
                                        bNeedsRetry = true;
                                        listToRetry.Add(tempRequest);
                                        itemsToRemove.Add(tempRequest);

                                        // in this case try to process it again ?
                                        m_brainCloudClientRef.Log("INCOMING -- Error : " + tempRequest.WebRequest.error + " url: " + tempRequest.WebRequest.url);
                                        m_brainCloudClientRef.Log("INCOMING -- JSON : " + tempRequest.Jsonstring);
                                        continue;
                                    }
                                    else if (tempRequest.WebRequest.isDone)
                                    {
                                        try
                                        {
                                            ProcessResponse(tempRequest.WebRequest.text);
                                            itemsToRemove.Add(tempRequest);
                                            enableComms = true;

                                            // ensure to remove all packet ids that are the same
                                            itemsToRemove.AddRange(
                                                m_activeRequestStates.FindAll(
                                                    x => x.PacketId == tempRequest.PacketId &&
                                                    x != tempRequest));

                                        }
                                        catch (System.Exception e)
                                        {
                                            // in this case try to process it again ?
                                            m_brainCloudClientRef.Log("INCOMING -- Exception : " + e);
                                            m_brainCloudClientRef.Log("INCOMING -- Fail : " + tempRequest.Jsonstring);
                                        }
                                    }
                                }

                            }
                            catch (System.Exception e)
                            {
                                m_brainCloudClientRef.Log("INCOMING -- Exception : " + e);
                                m_brainCloudClientRef.Log("INCOMING -- Fail : " + tempRequest.Jsonstring);
                            }
                        }
                    }

                    for (int i = 0; i < itemsToRemove.Count; ++i)
                    {
                        m_activeRequestStates.Remove(itemsToRemove[i]);
                    }

                    // these will be retried in a bit
                    if (bNeedsRetry) // due to error
                    {
                        m_toSendRequestStates.AddRange(listToRetry);
                        //return;
                    }
                }

                // disable heart beat timer
                if (m_serviceCallsSuccessful.Count > 0 || m_serviceCallsErroneous.Count > 0)
                    EnableHeartBeatTimer(false);
#endif

                // process the successful calls
                ProcessCallbackList(m_serviceCallsSuccessful, true);

                // process the error calls
                ProcessCallbackList(m_serviceCallsErroneous, false);
            }

            if (m_triggeredForReset)
            {
                ResetComms();
            }
#if !(DOT_NET)
            else if (enableComms)
            {
                EnableProcessEvents(true);
                EnableHeartBeatTimer(true);
                EnableTimeOutTimer(false);
            }
#endif

            if (m_heartBeatTimer != null)
            {
                m_heartBeatTimer.Update();
            }
            if (m_timeoutTimer != null)
            {
                m_timeoutTimer.Update();
            }
        }


        /**
         * Shuts down this system
         */
        public void ShutDown()
        {
            // force a log out
            ServerCallback callback = BrainCloudClient.CreateServerCallback(null, null, null);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Logout, null, callback);
            AddToQueue(sc);
            // force allowed to process events
            EnableProcessEvents(true);
            ProcessQueueHelper();
            // then reset communications
            ResetCommunication();

            Update();
        }

        /**
         * Clears all pending and in progress messages from the message queue(s).
         */
        public void ResetCommunication()
        {
            m_triggeredForReset = true;
        }

        public static String getJsonString(JsonData jsonData, String key, String defaultReturn)
        {

            try
            {
                return (String)jsonData[key];
            }
            catch (KeyNotFoundException)
            {
                return defaultReturn;
            }

        }

        public static long getJsonLong(JsonData jsonData, String key, long defaultReturn)
        {

            try
            {
                JsonData value = jsonData[key];
                if (value.IsLong)
                    return (long)value;
                if (value.IsInt)
                    return (int)value;
                return defaultReturn;
            }
            catch (KeyNotFoundException)
            {
                return defaultReturn;
            }

        }

        #endregion

        #region RequestState Class Helper
        private class RequestState
        {
            // This class stores the request state of the request.
            private ulong m_packetId;
            public ulong PacketId
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
#if !(DOT_NET)
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

            // unity uses WWW objects to make http calls cross platform
            private WWW request;
            public WWW WebRequest
#else
            // while .net projects can use the WebRequest Object
            private WebRequest request;
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
            private string m_jsonstring;
            public string Jsonstring
            {
                get
                {
                    return m_jsonstring;
                }
                set
                {
                    m_jsonstring = value;
                }
            }
            public RequestState()
            {
                request = null;
                m_jsonstring = "";
            }
        }

#if UNITY_WP8 || DOT_NET
        private bool AcceptAllCertifications(object sender,
                                             System.Security.Cryptography.X509Certificates.X509Certificate certification,
                                             System.Security.Cryptography.X509Certificates.X509Chain chain,
                                             System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // TODO: we should only be accepting certificates from places we deem safe [smrj]
            // right now accepting all! - not that secure!
            return true;
        }
#endif

        #endregion

        private void EnableTimeOutTimer(bool in_value)
        {
            // start it up!
            if (in_value)
            {
                // for now tick every half second
                if (m_timeoutTimer == null)
                {
                    double timeoutMs = 20 * 1000;
#if !(DOT_NET)
                    m_timeoutTimer = new SingleThreadedTimer(timeoutMs);
#else
                    m_timeoutTimer = new MultiThreadedTimer(timeoutMs);
#endif
                    m_timeoutTimer.SetEventHandler(OnTimeOut);
                }

                m_timeoutTimer.Stop();
                m_timeoutTimer.Start();

            }
            else
            {
                if (m_timeoutTimer != null)
                {
                    m_timeoutTimer.Stop();
                }
            }
        }


        private void EnableHeartBeatTimer(bool in_value, bool in_tearDown = false)
        {
            // start it up!
            if (in_value)
            {
                // for now tick every half second
                if (m_heartBeatTimer == null)
                {
#if !(DOT_NET)
                    m_heartBeatTimer = new SingleThreadedTimer(m_heartBeatInterval);
#else
                    m_heartBeatTimer = new MultiThreadedTimer(m_heartBeatInterval);
#endif
                    m_heartBeatTimer.SetEventHandler(OnHeartBeat);
                    m_heartBeatTimer.Start();
                }
                else
                {
                    m_heartBeatTimer.Stop();
                    m_heartBeatTimer.Start();
                }
            }
            else
            {
                if (m_heartBeatTimer != null)
                {
                    m_heartBeatTimer.Stop ();
                }
            }
        }

        /**
         * EnableComms the Communications
         * in_value:
         */
        public void EnableComms(bool in_value)
        {
            //m_brainCloudClientRef.Log("EnableComms -- " + Environment.StackTrace);

            // Enable everything
            if (in_value)
            {
                // we can start the heart beat timer back up
                EnableProcessEvents(true);
                EnableHeartBeatTimer(in_value);
                CreateLoaderThread();
            }
            else
            {
                EnableProcessEvents(false, true);
                // heart beat timer we need to pause
                EnableHeartBeatTimer(in_value);
                EnableTimeOutTimer(in_value);
            }

            //m_brainCloudClientRef.Log("EnableComms(bool in_value) -- " + in_value);
        }

        private void ResetComms()
        {
            m_isAuthenticated = false;

            // tear all waiting / inprogress / callbacks down
            lock (m_serviceCallsWaiting)
            {
                m_serviceCallsWaiting.Clear();
            }

            lock (m_serviceCallsInProgress)
            {
                m_serviceCallsInProgress.Clear();
            }

            lock (m_serviceCallsSuccessful)
            {
                m_serviceCallsSuccessful.Clear();
            }

            lock (m_serviceCallsErroneous)
            {
                m_serviceCallsErroneous.Clear();
            }

            m_numRetry = 0;
            m_triggeredForReset = false;
        }

        // processes the callback lists for success and errors with appropriate safe gaurds
        private void ProcessCallbackList(List<ServerCallProcessed> in_list, bool in_Success)
        {
            lock (in_list)
            {
                if (in_list.Count > 0)
                {
                    //m_brainCloudClientRef.Log("NumItems Processed Start ---" + in_Success);
                    m_numRetry = 0;
                    //m_brainCloudClientRef.Log("NumItems Processed Start ----- " + in_list.Count + " + " + in_Success);
                    ServerCallProcessed toProcess;

                    while(in_list.Count > 0)
                    {
                        toProcess = in_list[0];
                        in_list.RemoveAt(0);

                        if (toProcess.ServerCall.GetCallback() != null)
                        {
                            try
                            {
                                if (in_Success)
                                {
                                    toProcess.ServerCall.GetCallback().OnSuccessCallback(toProcess.Data);
                                    if (toProcess.ServerCall.GetService().Equals(ServiceName.Authenticate.Value))
                                    {
                                        processAuthenticate(toProcess.Data);
                                    }
                                }
                                else
                                {
                                    toProcess.ServerCall.GetCallback().OnErrorCallback(toProcess.Data);
                                }
                            }
                            catch(Exception)
                            {
                                // callback funtion threw an exception... nothing left to do except rethrow it
                                throw;
                            }
                        }
                    }
                    //m_brainCloudClientRef.Log("NumItems Processed " + in_list.Count);
                }
            }
        }


        private void OnHeartBeat()
        {
            if (this != null && this.Authenticated)
            {
                TimeSpan idleTime = DateTime.Now - m_lastpacketSend;
                m_brainCloudClientRef.Log("heartBeatCheck:" + idleTime.TotalSeconds + " " + m_idleInterval);
                if (idleTime.TotalSeconds > m_idleInterval)
                {
                    m_brainCloudClientRef.Log("heartBeatTrigger:" + idleTime.TotalSeconds + " " + m_idleInterval);
                    ServerCall sc = new ServerCall(ServiceName.HeartBeat, ServiceOperation.Read, null, null);
                    AddToQueue(sc);
                }
            }
        }

        private int REQUEST_MESSAGE_LIMIT = 50;
        private void ProcessQueueHelper()
        {
            // only process the queue if we have some in there
            lock (m_serviceCallsWaiting)
            {
                if (m_serviceCallsWaiting.Count > 0)
                {
                    m_lastpacketSend = DateTime.Now;

                    EnableProcessEvents(false);
#if (DOT_NET)
                    EnableHeartBeatTimer(false);
#endif
                    // only bundle up to REQUEST_MESSAGE_LIMIT at a time
                    int numMessagesWaiting = m_serviceCallsWaiting.Count;
                    if (numMessagesWaiting > REQUEST_MESSAGE_LIMIT)
                    {
                        numMessagesWaiting = REQUEST_MESSAGE_LIMIT;
                    }

                    // add the messages that are waiting to be in progress and then
                    // remove them accordingly
                    lock (m_serviceCallsInProgress)
                    {
                        m_serviceCallsInProgress = m_serviceCallsWaiting.GetRange(0, numMessagesWaiting);
                        m_serviceCallsWaiting.RemoveRange(0, numMessagesWaiting);

                        // prepare json data for server
                        //JArray jsonMessageList = new JArray();
                        JsonData jsonMessageList = new JsonData();

                        ServerCall scIndex;
                        for (int i = 0; i < m_serviceCallsInProgress.Count; ++i)
                        {
                            scIndex = m_serviceCallsInProgress[i] as ServerCall;
                            JsonData newObj = new JsonData();
                            newObj[OperationParam.ServiceMessageService.Value] = scIndex.Service;
                            newObj[OperationParam.ServiceMessageOperation.Value] = scIndex.Operation;
                            newObj[OperationParam.ServiceMessageData.Value] = scIndex.GetJsonData();

                            jsonMessageList.Add(newObj);
                        }

                        // create a web request
                        try
                        {
                            // now actually bundle up the data into a string
                            JsonData bundledObj = new JsonData();
                            bundledObj[OperationParam.ServiceMessagePacketId.Value] = m_packetId++;
                            bundledObj[OperationParam.ServiceMessageUserId.Value] = m_userID;
                            bundledObj[OperationParam.ServiceMessageSessionId.Value] = m_sessionID;
                            bundledObj[OperationParam.ServiceMessageMessages.Value] = jsonMessageList;

                            string jsonRequeststring = JsonMapper.ToJson(bundledObj);
                            string sig = CalculateMD5Hash(jsonRequeststring + m_secretKey);
                            //Set the length of the message
                            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequeststring);


#if !(DOT_NET)
                            WWW request = null;
#else
                            // TODO: keep the same web request around?
                            WebRequest request = WebRequest.Create(m_serverURL);
                            request.ContentType = "application/json; charset=utf-8";
                            request.Method = "POST"; // TODO: confirm if all these web requests are POST requests
                            request.Headers.Add("X-SIG", sig);
                            request.ContentLength = byteArray.Length;
#endif
                            RequestState requestState = new RequestState();
                            requestState.PacketId = m_packetId - 1;
                            requestState.WebRequest = request;
                            requestState.Jsonstring = jsonRequeststring;
#if !(DOT_NET)
                            requestState.Signature = sig;
                            requestState.ByteArray = byteArray;

                            lock (m_toSendRequestStates)
                            {
                                m_toSendRequestStates.Add(requestState);
                            }
#else

                            m_brainCloudClientRef.Log("RequestState - JsonRequeststring GOING OUT: " + jsonRequeststring);
                            request.BeginGetRequestStream(new AsyncCallback(GetRequestCallback), requestState);

                            EnableTimeOutTimer(true);
                            m_brainCloudClientRef.Log("m_serviceCallsInProgress - EnableHeartBeatTimer true ");
                            EnableHeartBeatTimer(true);
                            m_brainCloudClientRef.Log("m_serviceCallsInProgress - EnableHeartBeatTimer end ");
#endif
                        }
                        catch (Exception ex)
                        {
                            m_brainCloudClientRef.Log("BeginGetRequestStream - Exception: " + ex.ToString());
                            HandleErrorCallInProgressCalls(ex.ToString());
                        }
                    }
                } // end conditional < 0
            } // end lock
        }

#if (DOT_NET)
        private void ProcessQueue()
        {
            // when triggerred to false the queue will be finished
            while (m_processThread)
            {
                lock (m_processingEvent)
                {
                    if (m_processingBool)
                    {
                        ProcessQueueHelper();
                    }
                }
                Thread.Sleep(10);
            }

            //m_brainCloudClientRef.Log("ProcessQueue ---- COMPLETE!!!");
        }

        private void GetRequestCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                RequestState requestState = (RequestState)asynchronousResult.AsyncState;
                WebRequest webRequest = (WebRequest)requestState.WebRequest;

                // End the operation

                Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);
                m_brainCloudClientRef.Log("GetRequestStreamCallback - JsonRequeststring GOING OUT: " + requestState.Jsonstring);

                // Convert the string into a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(requestState.Jsonstring);

                // Write to the request stream.
                postStream.Write(byteArray, 0, requestState.Jsonstring.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("BrainCloud - GetResponseCallback - Exception: " + ex.ToString());
                HandleErrorCallInProgressCalls(ex.ToString());
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            //a callback method to end receiving the data
            string jsonResponsestring = "";
            try
            {
                RequestState requestState = (RequestState)asynchronousResult.AsyncState;
                WebRequest webRequest = requestState.WebRequest;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);

                jsonResponsestring = streamRead.ReadToEnd();

                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BrainCloud - GetResponseCallback - Exception: " + ex.ToString());
                HandleErrorCallInProgressCalls(ex.ToString());
                return;
            }

            // and now handle the incoming data
            HandleSuccessCallInProgressCalls(jsonResponsestring);

            EnableHeartBeatTimer(true);
            EnableTimeOutTimer(false);
        }
#endif

        private void ProcessResponse(string in_response)
        {
            string jsonResponsestring = in_response;
            try
            {
                // and now handle the incoming data
                HandleSuccessCallInProgressCalls(jsonResponsestring);
            }
            catch (Exception ex)
            {
                m_brainCloudClientRef.Log("GetResponseCallback - Exception: " + ex.ToString());
                HandleErrorCallInProgressCalls(ex.ToString());
                return;
            }

        }

        private void HandleTimeOut()
        {
            m_brainCloudClientRef.Log("BrainCloud.OnTimeOut Timeout # " + (m_numRetry+1));

            EnableTimeOutTimer(false);
            ++m_numRetry;

            // we've already retried three times
            // now throw the error back
            if (m_numRetry >= 3)
            {
                HandleErrorCallInProgressCalls("Error Timeout --- Num Retries Already " + m_numRetry);
                m_numRetry = 0;
                return;
            }

#if !(DOT_NET)
            // remove all the ones waiting to be sent, they will be re-added when we retry
            lock (m_toSendRequestStates)
            {
                m_toSendRequestStates.Clear();
            }

            // remove all the active ones
            lock (m_activeRequestStates)
            {
                for (int i = 0; i < m_activeRequestStates.Count; ++i)
                {
                    m_toSendRequestStates.Add(m_activeRequestStates[i]);
                }
                m_activeRequestStates.Clear();
            }

#endif
            // grab all in progress items, add them to the queue to simulate retrying
            // and remove all in progress items
            lock (m_serviceCallsInProgress)
            {
                for (int i = 0; i < m_serviceCallsInProgress.Count; ++i)
                {
                    AddToQueue(m_serviceCallsInProgress[i]);
                }

                m_serviceCallsInProgress.Clear();
            }

            EnableProcessEvents(true);
            EnableHeartBeatTimer(true);
        }

        private void OnTimeOut()
        {
            HandleTimeOut();
        }

        private void EnableProcessEvents(bool in_value, bool in_tearDown = false)
        {
            if (in_value)
            {
#if (DOT_NET)
                m_processThread = true;
#endif
            }
            else
            {
                if (in_tearDown)
                {
#if (DOT_NET)
                    m_processThread = false;
#endif
                }
            }

            m_processingBool = in_value;
        }

        private void CreateLoaderThread()
        {
#if (DOT_NET)
            // processing thread handled via m_processingEvent
            Thread thread = new System.Threading.Thread( (ThreadStart)ProcessQueue );
            thread.Priority = System.Threading.ThreadPriority.BelowNormal;
            thread.IsBackground = true;
            thread.Name = "BrainCloud.m_loaderThread";
            thread.Start();
#endif
        }

        private string CalculateMD5Hash(string input)
        {
#if !(DOT_NET)
            MD5Unity.MD5 md5 = MD5Unity.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input); // UTF8, not ASCII
            byte[] hash = md5.ComputeHash(inputBytes);
#else
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input); // UTF8, not ASCII
            byte[] hash = md5.ComputeHash(inputBytes);
#endif

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        private void HandleSuccessCallInProgressCalls(string jsonData)
        {
            //OutputAllJsonData(jsonData);
            m_brainCloudClientRef.Log("GetResponseCallback - JsonResponsestring COMING IN: " + jsonData);
            JsonReader reader = new JsonReader(jsonData);
            JsonData root = JsonMapper.ToObject(reader);

            int i = 0;
            List<ServerCall> processed = new List<ServerCall>();
            JsonData jObject = null;
            for (int j = 0; j < root.Count; ++j)
            {
                jObject = root[j];
                //m_brainCloudClientRef.Log(" JsonData - " + JsonData.ToString());
                int statusCode = (int)jObject["status"];
                string data = "";

                if (statusCode == 200)
                {
                    if (jObject[OperationParam.ServiceMessageData.Value] != null)
                    {
                        // its a good response!
                        JsonData goodResponseData = (JsonData)jObject[OperationParam.ServiceMessageData.Value];

                        // send the data back as not formatted
                        data = JsonMapper.ToJson(jObject);

                        // TODO:: confirm session ID failure for read player state, read summary friend data

                        // save the session ID
                        try
                        {
                            if (getJsonString(goodResponseData, OperationParam.ServiceMessageSessionId.Value, null) != null)
                            {
                                m_sessionID = (string)goodResponseData[OperationParam.ServiceMessageSessionId.Value];
                                m_isAuthenticated = true;  // TODO confirm authentication
                            }

                            // save the profile ID
                            if (getJsonString(goodResponseData, OperationParam.ServiceMessageProfileId.Value, null) != null)
                            {
                                m_brainCloudClientRef.AuthenticationService.ProfileId = (string)goodResponseData[OperationParam.ServiceMessageProfileId.Value];
                            }

                            // save the user ID
                            if (getJsonString(goodResponseData, OperationParam.ServiceMessageUserId.Value, null) != null)
                            {
                                m_userID = (string)goodResponseData[OperationParam.ServiceMessageUserId.Value];
                            }
                        }
                        catch (Exception e)
                        {
                            // do nothing with this error [smrj]
                            // it means it can't find either sessionId or userId in the message thrown
                            m_brainCloudClientRef.Log("SessionId or UserId do not exist " + e.ToString());
                        }
                        m_brainCloudClientRef.Log("continuing to process");

                    }

                    // ensure that we send back the success callbacks
                    lock (m_serviceCallsSuccessful)
                    {
                        lock (m_serviceCallsInProgress)
                        {
                            if (i < m_serviceCallsInProgress.Count)
                            {
                                ServerCall sc = m_serviceCallsInProgress[i] as ServerCall;
                                if (sc != null)
                                {
                                    if (sc.GetService().Equals(ServiceName.PlayerState.Value) &&
                                            (sc.GetOperation().Equals(ServiceOperation.FullReset.Value) ||
                                             sc.GetOperation().Equals(ServiceOperation.Reset.Value) ||
                                             sc.GetOperation().Equals(ServiceOperation.Logout.Value)))

                                    {
                                        // we reset the current player or logged out
                                        // we are no longer authenticated
                                        m_isAuthenticated = false;
                                    }

                                    // // only process callbacks that are real
                                    if (sc.GetCallback() != null)
                                    {
                                        ServerCallProcessed newProcessed = new ServerCallProcessed();
                                        newProcessed.ServerCall = sc;
                                        newProcessed.Data = data;
                                        m_serviceCallsSuccessful.Add(newProcessed);
                                    }

                                    processed.Add(sc);
                                }

                            }
                        }
                    }
                    ++i;

                }
                else if (statusCode >= 400 || statusCode == 202)
                {
                    // this is an error!
                    // string message = (string)jObject[OperationParam.ServiceMessageStatusMessage.Value];
                    // pass on the entire returned json object
                    string message = JsonMapper.ToJson(jObject);
                    HandleErrorCallInProgressCalls( /*"Error : "+ reasonCode + " : " + */ message);
                    return;
                }
            }

            // and clear all handled in progress calls
            lock (m_serviceCallsInProgress)
            {
                for (int j = 0; j < processed.Count; ++j)
                {
                    m_serviceCallsInProgress.Remove(processed[j]);
                }
            }

            //m_brainCloudClientRef.Log("BrainCloud.ProcessQueue - Not Waiting ! -- HandleSuccessCallInProgressCalls");
#if (DOT_NET)
            EnableProcessEvents(true);
#endif
        }

        private void HandleErrorCallInProgressCalls(string error)
        {
            lock (m_serviceCallsErroneous)
            {
                int i = 0;

                List<ServerCall> processed = new List<ServerCall>();
                for (; i < m_serviceCallsInProgress.Count; ++i)
                {
                    ServerCallProcessed newProcessed = new ServerCallProcessed();
                    newProcessed.ServerCall = m_serviceCallsInProgress[i] as ServerCall;
                    newProcessed.Data = error;
                    m_serviceCallsErroneous.Add(newProcessed);
                    processed.Add(m_serviceCallsInProgress[i] as ServerCall);
                }

                lock (m_serviceCallsInProgress)
                {
                    for (int j = 0; j < processed.Count; ++j)
                    {
                        m_serviceCallsInProgress.Remove(processed[j]);
                    }
                }
            }

            //m_brainCloudClientRef.Log("BrainCloud.ProcessQueue - Not Waiting ! --- HandleErrorCallInProgressCalls");
#if (DOT_NET)
            EnableHeartBeatTimer(true);
            EnableProcessEvents(true);
#endif
        }

        private void OutputAllJsonData(string jsonData)
        {
            /*
            JsonTextReader jsonReader = new JsonTextReader(new stringReader(jsonData));
            while (jsonReader.Read())
            {
              if (jsonReader.Value != null)
                m_brainCloudClientRef.Log("Token: {0}, Value: {1}", jsonReader.TokenType, jsonReader.Value);
              else
                m_brainCloudClientRef.Log("Token: {0}", jsonReader.TokenType);
            }
             */
        }

        private void processAuthenticate(string jsonString)
        {
            try
            {
                JsonReader reader = new JsonReader(jsonString);
                JsonData jsonMessage = JsonMapper.ToObject(reader);

                JsonData jsonData = jsonMessage["data"];

                long playerSessionExpiry = getJsonLong(jsonData, OperationParam.AuthenticateServicePlayerSessionExpiry.Value, 1200);
                SetHeartbeatInterval((int)playerSessionExpiry * 1000 / 4);
                m_idleInterval = playerSessionExpiry / 2;
                m_brainCloudClientRef.Log("Setting time interval to " + (playerSessionExpiry * 1000 / 4));
                m_brainCloudClientRef.Log("Setting idle interval to " + m_idleInterval);
            }
            catch (System.Exception)
            {
            }

        }

    }
}
