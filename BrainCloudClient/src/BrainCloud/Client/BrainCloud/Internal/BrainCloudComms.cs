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
using BrainCloud;
using JsonFx.Json;

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


    internal sealed class BrainCloudComms
    {
        /// <summary>
        /// The maximum number of retries for a packet.
        /// </summary>
        private static int MAX_RETRIES = 5;

        /// <summary>
        /// The maximum number of messages in a bundle.
        /// Note that this is somewhat arbitrary - using the size
        /// of the packet would be a more appropriate measuring stick.
        /// </summary>
        private static int MAX_MESSAGES_IN_BUNDLE = 50;

        /// <summary>
        /// The id of m_expectedIncomingPacketId when no packet expected
        /// </summary>
        private static int NO_PACKET_EXPECTED = -1;

        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_brainCloudClientRef;
        
        /// <summary>
        /// Set to true once Initialize has been called.
        /// </summary>
        private bool m_initialized = false;

        /// <summary>
        /// Set to false if you want to shutdown processing on the Update.
        /// </summary>
        private bool m_enabled = true;

        /// <summary>
        /// The next packet id to send
        /// </summary>
        private long m_packetId = 0;

        /// <summary>
        /// The packet id we're expecting
        /// </summary>
        private long m_expectedIncomingPacketId = NO_PACKET_EXPECTED;

        /// <summary>
        /// The service calls that are waiting to be sent.
        /// </summary>
        private List<ServerCall> m_serviceCallsWaiting = new List<ServerCall>();

        /// <summary>
        /// The service calls that have been sent for which we are waiting for a reply
        /// </summary>
        private List<ServerCall> m_serviceCallsInProgress = new List<ServerCall>();

        /// <summary>
        /// The current request state. Null if no request is in progress.
        /// </summary>
        private RequestState m_activeRequest = null;

        /// <summary>
        /// The last time a packet was sent
        /// </summary>
        private DateTime m_lastTimePacketSent;

        /// <summary>
        /// How long we wait to send a heartbeat if no packets have been sent or received.
        /// This value is set to a percentage of the heartbeat timeout sent by the authenticate response.
        /// </summary>
        private TimeSpan m_idleTimeout = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Debug value to introduce packet loss for testing retries etc.
        /// </summary>
        private double m_debugPacketLossRate = 0;

        /// <summary>
        /// The event handler callback method
        /// </summary>
        private EventCallback m_eventCallback;



        private bool m_isAuthenticated = false;
        public bool Authenticated
        {
            get
            {
                return m_isAuthenticated;
            }
        }

        private string m_sessionID;
        public string SessionID
        {
            get
            {
                return m_sessionID;
            }
        }

        private string m_serverURL = "";
        public string ServerURL
        {
            get
            {
                return m_serverURL;
            }
        }

        private string m_secretKey = "";
        public string SecretKey
        {
            get
            {
                return m_secretKey;
            }
        }


        public BrainCloudComms(BrainCloudClient in_client)
        {
            #if (DOT_NET)
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
            #endif
            m_brainCloudClientRef = in_client;
        }


        /// <summary>
        /// Initialize the communications library with the specified serverURL and secretKey.
        /// </summary>
        /// <param name="serverURL">Server URL.</param>
        /// <param name="secretKey">Secret key.</param>
        public void Initialize(string serverURL, string secretKey)
        {
            m_packetId = 0;
            m_expectedIncomingPacketId = NO_PACKET_EXPECTED;

            m_serverURL = serverURL;
            m_secretKey = secretKey;

            m_initialized = true;
        }

        public void RegisterEventCallback(EventCallback in_cb)
        {
            m_eventCallback = in_cb;
        }

        public void DeregisterEventCallback()
        {
            m_eventCallback = null;
        }


        /// <summary>
        /// The update method needs to be called periodically to send/receive responses
        /// and run the associated callbacks.
        /// </summary>
        public void Update()
        {
            // basic flow here is to:
            // 1- process existing requests
            // 2- send next request
            // 3- handle heartbeat/timeouts
            
            if (!m_initialized)
            {
                return;
            }
            if (!m_enabled)
            {
                return;
            }
            
            // process current request
            if (m_activeRequest != null)
            {
                RequestState.eWebRequestStatus status = GetWebRequestStatus(m_activeRequest);
                if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                {
                    // do nothing with the error right now - let the timeout code handle it
                }
                else if (status == RequestState.eWebRequestStatus.STATUS_DONE)
                {
                    ResetIdleTimer();

                    // note that active request is set to null if exception is to be thrown
                    HandleResponseBundle(GetWebRequestResponse(m_activeRequest));

                    m_activeRequest = null;
                }          
            }
            
            // send the next message if we're ready
            if (m_activeRequest == null)
            {
                m_activeRequest = CreateAndSendNextRequestBundle();
            }
            
            // is it time for a retry?
            if (m_activeRequest != null)
            {
                if (DateTime.Now.Subtract(m_activeRequest.TimeSent) >= GetPacketTimeout(m_activeRequest))
                {
                    m_activeRequest.CancelRequest();

                    if (!ResendMessage(m_activeRequest))
                    {
                        // we've reached the retry limit - send timeout error to all client callbacks

                        RequestState.eWebRequestStatus status = GetWebRequestStatus(m_activeRequest);
                        if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                        {
                            m_brainCloudClientRef.Log("Timeout with network error: " + GetWebRequestResponse(m_activeRequest));
                        }
                        else
                        {
                            m_brainCloudClientRef.Log("Timeout no reply from server");
                        }

                        m_activeRequest = null;

                        // Fake a message bundle to keep the callback logic in one place
                        TriggerCommsError(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT, "Timeout trying to reach brainCloud server");
                    }
                }
            }
            
            // is it time for a heartbeat?
            if (Authenticated)
            {
                if (DateTime.Now.Subtract(m_lastTimePacketSent) >= m_idleTimeout)
                {
                    SendHeartbeat();
                }
            }
        }


        /// <summary>
        /// Method fakes a json error from the server and sends
        /// it along to the response callbacks.
        /// </summary>
        /// <param name="in_status">In_status.</param>
        /// <param name="in_reasonCode">In_reason code.</param>
        /// <param name="in_statusMessage">In_status message.</param>
        private void TriggerCommsError(int in_status, int in_reasonCode, string in_statusMessage)
        {
            // error json format is
            // {
            // "reason_code": 40316,
            // "status": 403,
            // "status_message": "Processing exception: Invalid game ID in authentication request",
            // "severity": "ERROR"
            // }

            int numMessagesToReturn = 0;
            lock(m_serviceCallsInProgress)
            {
                numMessagesToReturn = m_serviceCallsInProgress.Count;
            }
            if (numMessagesToReturn <= 0)
            {
                return;
            }

            JsonResponseErrorBundleV2 bundleObj = new JsonResponseErrorBundleV2();
            bundleObj.packetId = m_expectedIncomingPacketId;
            bundleObj.responses = new JsonErrorMessage[numMessagesToReturn];
            for (int i = 0; i < numMessagesToReturn; ++i)
            {
                bundleObj.responses[i] = new JsonErrorMessage(in_status, in_reasonCode, in_statusMessage);
            }
            string jsonError = JsonWriter.Serialize(bundleObj);
            HandleResponseBundle(jsonError);
        }
        
        
        /// <summary>
        /// Shuts down the communications layer.
        /// Make sure to only call this from the main thread!
        /// </summary>
        public void ShutDown()
        {
            lock(m_serviceCallsWaiting)
            {
                m_serviceCallsWaiting.Clear();
            }

            // force a log out
            ServerCallback callback = BrainCloudClient.CreateServerCallback(null, null, null);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Logout, null, callback);
            AddToQueue(sc);

            m_activeRequest = null;

            // calling update will try to send the logout
            Update();

            // and then dump the comms layer
            ResetCommunication();
        }


        /// <summary>
        /// Resets the idle timer.
        /// </summary>
        private void ResetIdleTimer()
        {
            m_lastTimePacketSent = DateTime.Now;
        }


        /// <summary>
        /// Handles the response bundle and calls registered callbacks.
        /// </summary>
        /// <param name="in_jsonData">The received message bundle.</param>
        private void HandleResponseBundle(string in_jsonData)
        {
            m_brainCloudClientRef.Log("INCOMING: " + in_jsonData);

            JsonResponseBundleV2 bundleObj = JsonReader.Deserialize<JsonResponseBundleV2>(in_jsonData);
            long receivedPacketId = (long) bundleObj.packetId;
            if (m_expectedIncomingPacketId == NO_PACKET_EXPECTED || m_expectedIncomingPacketId != receivedPacketId)
            {
                m_brainCloudClientRef.Log("Dropping duplicate packet");
                return;
            }
            m_expectedIncomingPacketId = NO_PACKET_EXPECTED;

            Dictionary<string, object>[] responseBundle = bundleObj.responses;
            Dictionary<string, object> response = null;
            IList<Exception> exceptions = new List<Exception>(); 

            for (int j = 0; j < responseBundle.Length; ++j)
            {
                response = responseBundle[j];
                int statusCode = (int) response["status"];
                string data = "";

                //
                // It's important to note here that a user error callback *might* call
                // ResetCommunications() based on the error being returned.
                // ResetCommunications will clear the m_serviceCallsInProgress List
                // effectively removing all registered callbacks for this message bundle.
                // It's also likely that the developer will want to call authenticate next.
                // We need to ensure that this is supported as it's the best way to 
                // reset the brainCloud communications after a session invalid or network
                // error is triggered.
                //
                // This is safe to do from the main thread but just in case someone
                // calls this method from another thread, we lock on m_serviceCallsWaiting
                //
                ServerCall sc = null;
                lock (m_serviceCallsWaiting)
                {
                    if (m_serviceCallsInProgress.Count > 0)
                    {
                        sc = m_serviceCallsInProgress[0] as ServerCall;
                        m_serviceCallsInProgress.RemoveAt(0);
                    }
                }

                // its a success response
                if (statusCode == 200)
                {
                    if (response[OperationParam.ServiceMessageData.Value] != null)
                    {
                        Dictionary<string, object> responseData = (Dictionary<string, object>) response[OperationParam.ServiceMessageData.Value];
                        
                        // send the data back as not formatted
                        data = JsonWriter.Serialize(response);
                        
                        // save the session ID
                        try
                        {
                            if (getJsonString(responseData, OperationParam.ServiceMessageSessionId.Value, null) != null)
                            {
                                m_sessionID = (string)responseData[OperationParam.ServiceMessageSessionId.Value];
                                m_isAuthenticated = true;  // TODO confirm authentication
                            }
                            
                            // save the profile ID
                            if (getJsonString(responseData, OperationParam.ServiceMessageProfileId.Value, null) != null)
                            {
                                m_brainCloudClientRef.AuthenticationService.ProfileId = (string)responseData[OperationParam.ServiceMessageProfileId.Value];
                            }
                        }
                        catch (Exception e)
                        {
                            m_brainCloudClientRef.Log("SessionId or ProfileId do not exist " + e.ToString());
                        }
                    }

                    // now try to execute the callback
                    if (sc != null)
                    {
                        if (sc.GetService().Equals(ServiceName.PlayerState.Value)
                            && (sc.GetOperation().Equals(ServiceOperation.FullReset.Value)
                                || sc.GetOperation().Equals(ServiceOperation.Logout.Value)))
                        {
                            // we reset the current player or logged out
                            // we are no longer authenticated
                            m_isAuthenticated = false;
                            m_brainCloudClientRef.AuthenticationService.ClearSavedProfileID();
                        }
                        else if (sc.GetService().Equals(ServiceName.Authenticate.Value)
                            && sc.GetOperation().Equals(ServiceOperation.Authenticate.Value))
                        {
                            ProcessAuthenticate(data);
                        }
                        
                        // // only process callbacks that are real
                        if (sc.GetCallback() != null)
                        {
                            try
                            {
                                sc.GetCallback().OnSuccessCallback(data);
                            }
                            catch(Exception e)
                            {
                                m_brainCloudClientRef.Log (e.StackTrace);
                                exceptions.Add (e);
                            }
                        }
                    }
                }
                else if (statusCode >= 400 || statusCode == 202)
                {
                    object reasonCodeObj = null, statusMessageObj = null;
                    int reasonCode = 0;
                    string statusMessage = "";
                    
                    if (response.TryGetValue("reason_code", out reasonCodeObj))
                    {
                        reasonCode = (int) reasonCodeObj;
                    }
                    if (response.TryGetValue ("status_message", out statusMessageObj))
                    {
                        statusMessage = (string) statusMessageObj;
                    }
                    
                    if (reasonCode == ReasonCodes.SESSION_EXPIRED
                        || reasonCode == ReasonCodes.SESSION_NOT_FOUND_ERROR)
                    {
                        m_isAuthenticated = false;
                        m_brainCloudClientRef.Log ("Received session expired or not found, need to re-authenticate");
                    }

                    if (sc != null && sc.GetOperation().Equals(ServiceOperation.Logout.Value))
                    {
                        if (reasonCode == ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT)
                        {
                            m_isAuthenticated = false;
                            m_brainCloudClientRef.Log("Could not communicate with the server on logout due to network timeout");
                        }
                    }
                    
                    // now try to execute the callback
                    if (sc != null && sc.GetCallback() != null)
                    {
                        try
                        {
                            sc.GetCallback().OnErrorCallback(statusCode, reasonCode, statusMessage);
                        }
                        catch(Exception e)
                        {
                            m_brainCloudClientRef.Log (e.StackTrace);
                            exceptions.Add (e);
                        }
                    }
                }
            }

            if (bundleObj.events != null && m_eventCallback != null)
            {
                Dictionary<string, Dictionary<string, object>[]> eventsJsonObj = new Dictionary<string, Dictionary<string, object>[]>();
                eventsJsonObj["events"] = bundleObj.events;
                string eventsAsJson = JsonWriter.Serialize(eventsJsonObj);
                try
                {
                    m_eventCallback(eventsAsJson);
                }
                catch(Exception e)
                {
                    m_brainCloudClientRef.Log (e.StackTrace);
                    exceptions.Add (e);
                }
            }

            if (exceptions.Count > 0)
            {
                m_activeRequest = null; // to make sure we don't reprocess this message

                throw new Exception("User callback handlers threw " + exceptions.Count +" exception(s)."
                                    +" See the Unity log for callstacks or inner exception for first exception thrown.",
                                    exceptions[0]);
            }
        }


        /// <summary>
        /// Creates the request state object and sends the message bundle
        /// </summary>
        /// <returns>The and send next request bundle.</returns>
        private RequestState CreateAndSendNextRequestBundle()
        {
            RequestState requestState = null;
            lock(m_serviceCallsWaiting)
            {
                if (m_serviceCallsWaiting.Count > 0)
                {
                    int numMessagesWaiting = m_serviceCallsWaiting.Count;
                    if (numMessagesWaiting > MAX_MESSAGES_IN_BUNDLE)
                    {
                        numMessagesWaiting = MAX_MESSAGES_IN_BUNDLE;
                    }
                    
                    if (m_serviceCallsInProgress.Count > 0)
                    {
                        // this should never happen
                        m_brainCloudClientRef.Log ("ERROR - in progress queue is not empty but we're ready for the next message!");
                        m_serviceCallsInProgress.Clear ();
                    }

                    m_serviceCallsInProgress = m_serviceCallsWaiting.GetRange(0, numMessagesWaiting);
                    m_serviceCallsWaiting.RemoveRange(0, numMessagesWaiting);
                }

                if (m_serviceCallsInProgress.Count > 0)
                {
                    requestState = new RequestState();
                    
                    // prepare json data for server
                    List<object> messageList = new List<object>();
                    
                    ServerCall scIndex;
                    for (int i = 0; i < m_serviceCallsInProgress.Count; ++i)
                    {
                        scIndex = m_serviceCallsInProgress[i] as ServerCall;
                        
                        Dictionary<string, object> message = new Dictionary<string, object>();
                        message[OperationParam.ServiceMessageService.Value] = scIndex.Service;
                        message[OperationParam.ServiceMessageOperation.Value] = scIndex.Operation;
                        message[OperationParam.ServiceMessageData.Value] = scIndex.GetJsonData();
                        
                        messageList.Add(message);

                        if (scIndex.GetOperation ().Equals (ServiceOperation.FullReset.Value)
                            || scIndex.GetOperation ().Equals(ServiceOperation.Logout.Value))
                        {
                            requestState.IsSessionTerminatingPacket = true;
                        }
                    }
                    
                    SendMessage(requestState, messageList);
                }
            } // unlock m_serviceCallsWaiting

            return requestState;
        }

        /// <summary>
        /// Sends the message, caches the message list and increments the packet id.
        /// </summary>
        /// <param name="requestState">Request state.</param>
        /// <param name="messageList">Message list.</param>
        private void SendMessage(RequestState requestState, List<object> messageList)
        {
            requestState.PacketId = m_packetId;
            m_expectedIncomingPacketId = m_packetId;
            requestState.MessageList = messageList;
            ++m_packetId;

            InternalSendMessage(requestState);
        }

        /// <summary>
        /// Resends a message bundle. Returns true if sent or
        /// false if max retries has been reached.
        /// </summary>
        /// <returns><c>true</c>, if message was resent, <c>false</c> if max retries hit.</returns>
        /// <param name="requestState">Request state.</param>
        private bool ResendMessage(RequestState requestState)
        {
            ++m_activeRequest.Retries;
            if (m_activeRequest.Retries >= MAX_RETRIES)
            {
                return false;
            }
            InternalSendMessage(requestState);
            return true;
        }

        /// <summary>
        /// Method creates the web request and sends it immediately.
        /// Relies upon the requestState PacketId and MessageList being
        /// set appropriately.
        /// </summary>
        /// <param name="requestState">Request state.</param>
        private void InternalSendMessage(RequestState requestState)
        {
            // bundle up the data into a string
            Dictionary<string, object> packet = new Dictionary<string, object>();
            packet[OperationParam.ServiceMessagePacketId.Value] = requestState.PacketId;
            packet[OperationParam.ServiceMessageSessionId.Value] = m_sessionID;
            packet[OperationParam.ServiceMessageMessages.Value] = requestState.MessageList;
            
            string jsonRequestString = JsonWriter.Serialize(packet);
            string sig = CalculateMD5Hash(jsonRequestString + m_secretKey);
            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequestString);

            requestState.Signature = sig;
            requestState.ByteArray = byteArray;

            if (m_debugPacketLossRate > 0.0)
            {
                System.Random r = new System.Random();
                requestState.LoseThisPacket = r.NextDouble () > m_debugPacketLossRate;
            }

            if (!requestState.LoseThisPacket)
            {
                #if !(DOT_NET)
                Dictionary<string, string> formTable = new Dictionary<string, string>();
                formTable["Content-Type"] = "application/json; charset=utf-8";
                formTable["X-SIG"] = sig;
                WWW request = new WWW(m_serverURL, byteArray, formTable);
                #else
                WebRequest request = WebRequest.Create(m_serverURL);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";
                request.Headers.Add("X-SIG", sig);
                request.ContentLength = byteArray.Length;
                // TODO: Convert to using a task as BeginGetRequestStream can block for minutes
                requestState.AsyncResult = request.BeginGetRequestStream(new AsyncCallback(GetRequestCallback), requestState);


                #endif

                requestState.WebRequest = request;
            }
            requestState.RequestString = jsonRequestString;
            requestState.TimeSent = DateTime.Now;

            ResetIdleTimer();
            
            m_brainCloudClientRef.Log("OUTGOING " 
                                      + (requestState.Retries > 0 ? " Retry(" + requestState.Retries +"): " : ": ")
                                      + jsonRequestString);

        }


        /// <summary>
        /// Gets the web request status.
        /// </summary>
        /// <returns>The web request status.</returns>
        /// <param name="in_requestState">In_request state.</param>
        private RequestState.eWebRequestStatus GetWebRequestStatus(RequestState in_requestState)
        {
            RequestState.eWebRequestStatus status = RequestState.eWebRequestStatus.STATUS_PENDING;

            // for testing packet loss, some packets are flagged to be lost
            // and should always return status pending no matter what the real
            // status is
            if (m_activeRequest.LoseThisPacket)
            {
                return status;
            }

#if !(DOT_NET)
            if (m_activeRequest.WebRequest.error != null)
            {
                status = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
            else if (m_activeRequest.WebRequest.isDone)
            {
                status = RequestState.eWebRequestStatus.STATUS_DONE;
            }
#else
            status = m_activeRequest.DotNetRequestStatus;
#endif
            return status;
        }


        /// <summary>
        /// Gets the web request response.
        /// </summary>
        /// <returns>The web request response.</returns>
        /// <param name="in_requestState">In_request state.</param>
        private string GetWebRequestResponse(RequestState in_requestState)
        {
            string response = "";
#if !(DOT_NET)
            if (m_activeRequest.WebRequest.error != null)
            { 
                response = m_activeRequest.WebRequest.error;
            }
            else
            {
                response = m_activeRequest.WebRequest.text;
            }
#else
            response = m_activeRequest.DotNetResponseString;
#endif
            return response;
        }


        /// <summary>
        /// Method staggers the packet timeout value based on the currentRetry
        /// </summary>
        /// <returns>The packet timeout.</returns>
        /// <param name="currentRetryNumber">Current retry number.</param>
        private TimeSpan GetPacketTimeout(RequestState in_requestState)
        {
            int currentRetry = in_requestState.Retries;
            TimeSpan ret;

            // if this is a delete player or logout we change the
            // timeout behaviour
            if (in_requestState.IsSessionTerminatingPacket)
            {
                switch (currentRetry)
                {
                case 0:
                    ret = TimeSpan.FromSeconds(15);
                    break;
                case 1:
                    ret = TimeSpan.FromSeconds(15);
                    break;
                case 2:
                    ret = TimeSpan.FromSeconds(2);
                    break;
                case 3:
                    ret = TimeSpan.FromSeconds(2);
                    break;
                case 4:
                default:
                    ret = TimeSpan.FromSeconds(1);
                    break;
                }
            }
            else
            {
                switch (currentRetry)
                {
                case 0:
                    ret = TimeSpan.FromSeconds(3);
                    break;
                case 1:
                    ret = TimeSpan.FromSeconds(5);
                    break;
                case 2:
                    ret = TimeSpan.FromSeconds(5);
                    break;
                case 3:
                    ret = TimeSpan.FromSeconds(10);
                    break;
                case 4:
                default:
                    ret = TimeSpan.FromSeconds(10);
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Sends the heartbeat.
        /// </summary>
        private void SendHeartbeat()
        {
            ServerCall sc = new ServerCall(ServiceName.HeartBeat, ServiceOperation.Read, null, null);
            AddToQueue(sc);
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

        /// <summary>
        /// Adds a server call to the internal queue.
        /// </summary>
        /// <param name="call">The server call to execute</param>
        internal void AddToQueue(ServerCall call)
        {
            lock (m_serviceCallsWaiting)
            {
                m_serviceCallsWaiting.Add(call);
            }
        }

        /// <summary>
        /// Enables the communications layer.
        /// </summary>
        /// <param name="in_value">If set to <c>true</c> in_value.</param>
        public void EnableComms(bool in_value)
        {
            m_enabled = in_value;
        }

        /// <summary>
        /// Resets the communication layer. Clients will need to
        /// reauthenticate after this method is called.
        /// </summary>
        internal void ResetCommunication()
        {
            lock (m_serviceCallsWaiting)
            {
                m_isAuthenticated = false;
                m_serviceCallsWaiting.Clear();
                m_serviceCallsInProgress.Clear();
                m_activeRequest = null;
            }
        }


#if (DOT_NET)
        private void GetRequestCallback(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            if (requestState.IsCancelled)
            { 
                return;
            }
            WebRequest webRequest = (WebRequest)requestState.WebRequest;

            try
            {
                // End the operation

                Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);
                //m_brainCloudClientRef.Log("GetRequestStreamCallback - JsonRequeststring GOING OUT: " + requestState.JsonRequestString);

                // Convert the string into a byte array.
                byte[] byteArray = Encoding.UTF8.GetBytes(requestState.RequestString);

                // Write to the request stream.
                postStream.Write(byteArray, 0, requestState.RequestString.Length);
                postStream.Close();

                // Start the asynchronous operation to get the response
                webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), requestState);
            }
            catch (Exception ex)
            {
                m_brainCloudClientRef.Log("GetResponseCallback - Exception: " + ex.ToString());
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            RequestState requestState = (RequestState)asynchronousResult.AsyncState;
            if (requestState.IsCancelled)
            { 
                return;
            }

            //a callback method to end receiving the data
            try
            {
                WebRequest webRequest = requestState.WebRequest;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);

                requestState.DotNetResponseString = streamRead.ReadToEnd();
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_DONE;

                // Close the stream object
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse
                response.Close();
            }
            catch (WebException wex)
            {
                m_brainCloudClientRef.Log("GetResponseCallback - WebException: " + wex.ToString());
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
            catch (Exception ex)
            {
                m_brainCloudClientRef.Log("GetResponseCallback - Exception: " + ex.ToString());
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
        }
#endif


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


        private void ProcessAuthenticate(string jsonString)
        {
            Dictionary<string, object> jsonMessage = (Dictionary<string, object>) JsonReader.Deserialize(jsonString);
            Dictionary<string, object> jsonData = (Dictionary<string, object>) jsonMessage["data"];

            long playerSessionExpiry = getJsonLong(jsonData, OperationParam.AuthenticateServicePlayerSessionExpiry.Value, 5*60);
            long idleTimeout = (long) (playerSessionExpiry * 0.85);

            m_idleTimeout = TimeSpan.FromSeconds(idleTimeout);
        }


        private static String getJsonString(Dictionary<string, object> jsonData, String key, String defaultReturn)
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


        private static long getJsonLong(Dictionary<string, object> jsonData, String key, long defaultReturn)
        {
            try
            {
                object value = jsonData[key];
                if (value is System.Int64)
                    return (long)value;
                if (value is System.Int32)
                    return (int)value;
                return defaultReturn;
            }
            catch (KeyNotFoundException)
            {
                return defaultReturn;
            }
        }


        #region Json parsing objects

        // Classes to handle JSON serialization - do not
        // try to make variables conform to coding standards as
        // they must match json variable name format exactly

        private class JsonResponseBundleV2
        {
            public long packetId = 0;
            public Dictionary<string, object>[] responses = null;
            public Dictionary<string, object>[] events = null;

            
            public JsonResponseBundleV2()
            {}
        }
       
        private class JsonResponseErrorBundleV2
        {
            public long packetId;
            public JsonErrorMessage[] responses;
            
            public JsonResponseErrorBundleV2()
            {}
        }

        private class JsonErrorMessage
        {
            public int reason_code;
            public int status;
            public string status_message;
            public string severity = "ERROR";
            
            public JsonErrorMessage()
            {}
            
            public JsonErrorMessage(int in_status, int in_reasonCode, string in_statusMessage)
            {
                status = in_status;
                reason_code = in_reasonCode;
                status_message = in_statusMessage;
            }
        }
        #endregion
    }
}
