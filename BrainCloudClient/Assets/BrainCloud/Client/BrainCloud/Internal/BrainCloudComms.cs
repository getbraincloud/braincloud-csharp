//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

#if (DOT_NET || DISABLE_SSL_CHECK)
using System.Net;
#endif
#if DOT_NET
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
#else
using UnityEngine;
#endif

using JsonFx.Json;

namespace BrainCloud.Internal
{
    #region Processed Server Call Class
    public class ServerCallProcessed
    {
        internal ServerCall ServerCall { get; set; }
        public string Data { get; set; }
    }
    #endregion

    internal sealed class BrainCloudComms
    {
        /// <summary>
        /// The id of _expectedIncomingPacketId when no packet expected
        /// </summary>
        private static int NO_PACKET_EXPECTED = -1;

        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient _clientRef;

        /// <summary>
        /// Set to true once Initialize has been called.
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// Set to false if you want to shutdown processing on the Update.
        /// </summary>
        private bool _enabled = true;

        /// <summary>
        /// The next packet id to send
        /// </summary>
        private long _packetId = 0;

        /// <summary>
        /// The packet id we're expecting
        /// </summary>
        private long _expectedIncomingPacketId = NO_PACKET_EXPECTED;

        /// <summary>
        /// The service calls that are waiting to be sent.
        /// </summary>
        private List<ServerCall> _serviceCallsWaiting = new List<ServerCall>();

        /// <summary>
        /// The service calls that have been sent for which we are waiting for a reply
        /// </summary>
        private List<ServerCall> _serviceCallsInProgress = new List<ServerCall>();

        /// <summary>
        /// The service calls in the timeout queue.
        /// </summary>
        private List<ServerCall> _serviceCallsInTimeoutQueue = new List<ServerCall>();

        /// <summary>
        /// The current request state. Null if no request is in progress.
        /// </summary>
        private RequestState _activeRequest = null;

        /// <summary>
        /// The last time a packet was sent
        /// </summary>
        private DateTime _lastTimePacketSent;

        /// <summary>
        /// How long we wait to send a heartbeat if no packets have been sent or received.
        /// This value is set to a percentage of the heartbeat timeout sent by the authenticate response.
        /// </summary>
        private TimeSpan _idleTimeout = TimeSpan.FromSeconds(5 * 60);

        /// <summary>
        /// The maximum number of messages in a bundle.
        /// This is set to a value from the server on authenticate
        /// </summary>
        private int _maxBundleMessages = 10;

        /// <summary>
        /// The maximum number of sequential errors before client lockout
        /// This is set to a value from the server on authenticate
        /// </summary>
        private int _killSwitchThreshold = 11;

        ///<summary>
        ///The maximum number of attempts that the client can use
        ///while trying to successfully authenticate before the client 
        ///is disabled.
        ///<summary>
        private int _failedAuthenticationAttemptThreshold = 4; 
        
        ///<summary>
        ///The current number of failed attempts at authenticating. This 
        ///will reset when a successful authentication is made.
        ///<summary>
        private int _failedAuthenticationAttempts = 0;

        ///<summary>
        ///A bool that keeps track of the type of fakeErrorResponse to send to back 
        ///<summary>
        private bool _tooManyFailedAuthentications;
        
        ///<summary>
        ///An array that stores the most recently used profileIds.
        ///<summary>
        private String[] _recentlyUsedProfileIdsForAuth = {"", ""}; 

        /// <summary>
        /// Debug value to introduce packet loss for testing retries etc.
        /// </summary>
        private double _debugPacketLossRate = 0;

        /// <summary>
        /// The event handler callback method
        /// </summary>
        private EventCallback _eventCallback;

        /// <summary>
        /// The reward handler callback method
        /// </summary>
        private RewardCallback _rewardCallback;

        private FileUploadSuccessCallback _fileUploadSuccessCallback;

        private FileUploadFailedCallback _fileUploadFailedCallback;

        private FailureCallback _globalErrorCallback;

        private NetworkErrorCallback _networkErrorCallback;

        private List<FileUploader> _fileUploads = new List<FileUploader>();

#if DOT_NET
        private HttpClient _httpClient = new HttpClient();
#endif

        //For handling local session errors
        private int _cachedStatusCode;
        private int _cachedReasonCode;
        private string _cachedStatusMessage;

        //For kill switch
        private bool _killSwitchEngaged;
        private int _killSwitchErrorCount;
        private string _killSwitchService;
        private string _killSwitchOperation;

        private bool _isAuthenticated = false;
        public bool Authenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        internal void setAuthenticated()
        {
            _isAuthenticated = true;
        }

        public Dictionary<string, string> AppIdSecretMap
        {
            get; private set;
        }

        public string AppId
        {
            get; private set;
        }

        public string SecretKey
        {
            get
            {
                if (AppIdSecretMap.ContainsKey(AppId))
                {
                    return AppIdSecretMap[AppId];
                }
                else
                {
                    return "NO SECRET DEFINED FOR '" + AppId + "'";
                }
            }
        }

        public string SessionID
        {
            get; private set;
        }
        internal void setSessionId(String sessionId)
        {
            SessionID = sessionId;
        }

        public string ServerURL
        {
            get; private set;
        }

        public string UploadURL
        {
            get; private set;
        }

        private int _uploadLowTransferRateTimeout = 120;
        public int UploadLowTransferRateTimeout
        {
            get { return _uploadLowTransferRateTimeout; }
            set { _uploadLowTransferRateTimeout = value; }
        }

        private int _uploadLowTransferRateThreshold = 50;
        public int UploadLowTransferRateThreshold
        {
            get { return _uploadLowTransferRateThreshold; }
            set { _uploadLowTransferRateThreshold = value; }
        }

        /// <summary>
        /// A list of packet timeouts. Index represents the packet attempt number.
        /// </summary>
        private List<int> _packetTimeouts = new List<int> { 15, 10, 10 };
        public List<int> PacketTimeouts
        {
            get
            {
                return _packetTimeouts;
            }
            set
            {
                _packetTimeouts = value;
            }
        }
        public void SetPacketTimeoutsToDefault()
        {
            _packetTimeouts = new List<int> { 15, 10, 10 };
        }

        private int _authPacketTimeoutSecs = 15;
        public int AuthenticationPacketTimeoutSecs
        {
            get
            {
                return _authPacketTimeoutSecs;
            }
            set
            {
                _authPacketTimeoutSecs = value;
            }
        }

        private bool _oldStyleStatusResponseInErrorCallback = false;
        public bool OldStyleStatusResponseInErrorCallback
        {
            get
            {
                return _oldStyleStatusResponseInErrorCallback;
            }
            set
            {
                _oldStyleStatusResponseInErrorCallback = value;
            }
        }

        private bool _cacheMessagesOnNetworkError = false;
        public void EnableNetworkErrorMessageCaching(bool enabled)
        {
            _cacheMessagesOnNetworkError = enabled;
        }

        /// <summary>
        /// This flag is set when _cacheMessagesOnNetworkError is true
        /// and a timeout occurs. It is reset when a call is made 
        /// to either RetryCachedMessages or FlushCachedMessages
        /// </summary>
        private bool _blockingQueue = false;

        public BrainCloudComms(BrainCloudClient client)
        {
#if DISABLE_SSL_CHECK
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;
#endif
            AppIdSecretMap = new Dictionary<string, string>();
            _clientRef = client;
            ResetErrorCache();
        }


        /// <summary>
        /// Initialize the communications library with the specified serverURL and secretKey.
        /// </summary>
        /// <param name="serverURL">Server URL.</param>
        /// /// <param name="appId">AppId</param>
        /// <param name="secretKey">Secret key.</param>
        public void Initialize(string serverURL, string appId, string secretKey)
        {
            _packetId = 0;
            _expectedIncomingPacketId = NO_PACKET_EXPECTED;

            ServerURL = serverURL;

            string suffix = @"/dispatcherv2";
            UploadURL = ServerURL.EndsWith(suffix) ? ServerURL.Substring(0, ServerURL.Length - suffix.Length) : ServerURL;
            UploadURL += @"/uploader";

            AppIdSecretMap[appId] = secretKey;
            AppId = appId;

            _blockingQueue = false;
            _initialized = true;
        }

        /// <summary>
        /// Initialize the communications library with the specified serverURL and secretKey.
        /// </summary>
        /// <param name="serverURL">Server URL.</param>
        /// <param name="defaultAppId">default appId </param>
        /// /// <param name="appIdSecretMap">map of appId -> secrets, to allow the client to safely switch between apps with secret being secure</param>
        public void InitializeWithApps(string serverURL, string defaultAppId, Dictionary<string, string> appIdSecretMap)
        {
            AppIdSecretMap.Clear();
            AppIdSecretMap = appIdSecretMap;

            Initialize(serverURL, defaultAppId, AppIdSecretMap[defaultAppId]);
        }

        public void RegisterEventCallback(EventCallback cb)
        {
            _eventCallback = cb;
        }

        public void DeregisterEventCallback()
        {
            _eventCallback = null;
        }

        public void RegisterRewardCallback(RewardCallback cb)
        {
            _rewardCallback = cb;
        }

        public void DeregisterRewardCallback()
        {
            _rewardCallback = null;
        }

        public void RegisterFileUploadCallbacks(FileUploadSuccessCallback success, FileUploadFailedCallback failure)
        {
            _fileUploadSuccessCallback = success;
            _fileUploadFailedCallback = failure;
        }

        public void DeregisterFileUploadCallbacks()
        {
            _fileUploadSuccessCallback = null;
            _fileUploadFailedCallback = null;
        }

        public void RegisterGlobalErrorCallback(FailureCallback callback)
        {
            _globalErrorCallback = callback;
        }

        public void DeregisterGlobalErrorCallback()
        {
            _globalErrorCallback = null;
        }

        public void RegisterNetworkErrorCallback(NetworkErrorCallback callback)
        {
            _networkErrorCallback = callback;
        }

        public void DeregisterNetworkErrorCallback()
        {
            _networkErrorCallback = null;
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

            if (!_initialized)
            {
                return;
            }
            if (!_enabled)
            {
                return;
            }
            if (_blockingQueue)
            {
                return;
            }

            // process current request
            bool bypassTimeout = false;
            if (_activeRequest != null)
            {
                RequestState.eWebRequestStatus status = GetWebRequestStatus(_activeRequest);
                if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                {
                    // Force the timeout to be elapsed because we have completed the request with error
                    // or else, do nothing with the error right now - let the timeout code handle it
                    bypassTimeout = (_activeRequest.Retries >= GetMaxRetriesForPacket(_activeRequest));
                }
                else if (status == RequestState.eWebRequestStatus.STATUS_DONE)
                {
                    ResetIdleTimer();

                    // note that active request is set to null if exception is to be thrown
                    HandleResponseBundle(GetWebRequestResponse(_activeRequest));

                    _activeRequest = null;
                }
            }

            // is it time for a retry?
            if (_activeRequest != null)
            {
                if (bypassTimeout || DateTime.Now.Subtract(_activeRequest.TimeSent) >= GetPacketTimeout(_activeRequest))
                {
                    // grab status/response before cancelling the request as in Unity, the www object
                    // will set internal status fields to null when www object is disposed
                    RequestState.eWebRequestStatus status = GetWebRequestStatus(_activeRequest);
                    string errorResponse = "";
                    if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                    {
                        errorResponse = GetWebRequestResponse(_activeRequest);
                    }
                    _activeRequest.CancelRequest();

                    if (!ResendMessage(_activeRequest))
                    {
                        // we've reached the retry limit - send timeout error to all client callbacks
                        if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                        {
                            _clientRef.Log("Timeout with network error: " + errorResponse);
                        }
                        else
                        {
                            _clientRef.Log("Timeout no reply from server");
                        }

                        _activeRequest = null;

                        // if we're doing caching of messages on timeout, kick it in now!
                        if (_cacheMessagesOnNetworkError && _networkErrorCallback != null)
                        {
                            _clientRef.Log("Caching messages");
                            _blockingQueue = true;

                            // and insert the inProgress messages into head of wait queue
                            lock (_serviceCallsInTimeoutQueue)
                            {
                                _serviceCallsInTimeoutQueue.InsertRange(0, _serviceCallsInProgress);
                                _serviceCallsInProgress.Clear();
                            }

#if UNITY_EDITOR
                            BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnNetworkError("NetworkError");
#endif

                            _networkErrorCallback();
                        }
                        else
                        {
                            // Fake a message bundle to keep the callback logic in one place
                            TriggerCommsError(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT, "Timeout trying to reach brainCloud server");
                        }
                    }
                }
            }
            else // send the next message if we're ready
            {
                _activeRequest = CreateAndSendNextRequestBundle();
            }

            // is it time for a heartbeat?
            if (_isAuthenticated && !_blockingQueue)
            {
                if (DateTime.Now.Subtract(_lastTimePacketSent) >= _idleTimeout)
                {
                    SendHeartbeat();
                }
            }

            RunFileUploadCallbacks();
        }

        #region File Upload

        /// <summary>
        /// Checks the status of active file uploads
        /// </summary>
        private void RunFileUploadCallbacks()
        {
            for (int i = _fileUploads.Count - 1; i >= 0; i--)
            {
                _fileUploads[i].Update();
                if (_fileUploads[i].Status == FileUploader.FileUploaderStatus.CompleteSuccess)
                {
                    if (_fileUploadSuccessCallback != null)
                    {
#if UNITY_EDITOR
                        BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnEvent(string.Format("{0} {1}", _fileUploads[i].UploadId, _fileUploads[i].Response));
#endif

                        _fileUploadSuccessCallback(_fileUploads[i].UploadId, _fileUploads[i].Response);
                    }

                    _clientRef.Log("Upload success: " + _fileUploads[i].UploadId + " | " + _fileUploads[i].StatusCode + "\n" + _fileUploads[i].Response);
                    _fileUploads.RemoveAt(i);
                }
                else if (_fileUploads[i].Status == FileUploader.FileUploaderStatus.CompleteFailed)
                {
                    if (_fileUploadFailedCallback != null)
                    {
#if UNITY_EDITOR
                        BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnFailedResponse(_fileUploads[i].Response);
#endif

                        _fileUploadFailedCallback(_fileUploads[i].UploadId, _fileUploads[i].StatusCode, _fileUploads[i].ReasonCode, _fileUploads[i].Response);

                    }

                    _clientRef.Log("Upload failed: " + _fileUploads[i].UploadId + " | " + _fileUploads[i].StatusCode + "\n" + _fileUploads[i].Response);
                    _fileUploads.RemoveAt(i);
                }
            }
        }

        public void CancelUpload(string uploadFileId)
        {
            FileUploader uploader = GetFileUploader(uploadFileId);
            if (uploader != null) uploader.CancelUpload();
        }

        public double GetUploadProgress(string uploadFileId)
        {
            FileUploader uploader = GetFileUploader(uploadFileId);
            if (uploader != null) return uploader.Progress;
            else return -1;
        }

        public long GetUploadBytesTransferred(string uploadFileId)
        {
            FileUploader uploader = GetFileUploader(uploadFileId);
            if (uploader != null) return uploader.BytesTransferred;
            else return -1;
        }

        public long GetUploadTotalBytesToTransfer(string uploadFileId)
        {
            FileUploader uploader = GetFileUploader(uploadFileId);
            if (uploader != null) return uploader.TotalBytesToTransfer;
            else return -1;
        }

        private FileUploader GetFileUploader(string uploadId)
        {
            for (int i = 0; i < _fileUploads.Count; i++)
            {
                if (_fileUploads[i].UploadId == uploadId) return _fileUploads[i];
            }
            _clientRef.Log("GetUploadProgress could not find upload ID " + uploadId);
            return null;
        }

        #endregion

        /// <summary>
        /// Method fakes a json error from the server and sends
        /// it along to the response callbacks.
        /// </summary>
        /// <param name="status">status.</param>
        /// <param name="reasonCode">reason code.</param>
        /// <param name="statusMessage">status message.</param>
        private void TriggerCommsError(int status, int reasonCode, string statusMessage)
        {
            // error json format is
            // {
            // "reason_code": 40316,
            // "status": 403,
            // "status_message": "Processing exception: Invalid game ID in authentication request",
            // "severity": "ERROR"
            // }

            int numMessagesToReturn = 0;
            lock (_serviceCallsInProgress)
            {
                numMessagesToReturn = _serviceCallsInProgress.Count;
            }
            if (numMessagesToReturn <= 0)
            {
                numMessagesToReturn = 1; // for when we want to send to only global error callback
            }

            JsonResponseErrorBundleV2 bundleObj = new JsonResponseErrorBundleV2();
            bundleObj.packetId = _expectedIncomingPacketId;
            bundleObj.responses = new JsonErrorMessage[numMessagesToReturn];
            for (int i = 0; i < numMessagesToReturn; ++i)
            {
                bundleObj.responses[i] = new JsonErrorMessage(status, reasonCode, statusMessage);
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
            lock (_serviceCallsWaiting)
            {
                _serviceCallsWaiting.Clear();
            }

            // force a log out
            ServerCallback callback = BrainCloudClient.CreateServerCallback(null, null, null);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Logout, null, callback);
            AddToQueue(sc);

            _activeRequest = null;

            // calling update will try to send the logout
            Update();

            // and then dump the comms layer
            ResetCommunication();
        }

        // see BrainCloudClient.RetryCachedMessages() docs
        public void RetryCachedMessages()
        {
            if (_blockingQueue)
            {
                _clientRef.Log("Retrying cached messages");

                if (_activeRequest != null)
                {
                    // this is definitely an error in the comms lib if it happens. 
                    // we attempt to cancel it but this is uncharted territory.

                    _clientRef.Log("ERROR - retrying cached messages but there is an active request!");
                    _activeRequest.CancelRequest();
                    _activeRequest = null;
                }

                --_packetId;
                _activeRequest = CreateAndSendNextRequestBundle();
                _blockingQueue = false;
            }
        }

        // see BrainCloudClient.FlushCachedMessages() docs
        public void FlushCachedMessages(bool sendApiErrorCallbacks)
        {
            if (_blockingQueue)
            {
                _clientRef.Log("Flushing cached messages");

                // try to cancel if request is in progress (shouldn't happen)
                if (_activeRequest != null)
                {
                    _activeRequest.CancelRequest();
                    _activeRequest = null;
                }

                // then flush the message queues
                List<ServerCall> callsToProcess = new List<ServerCall>();
                lock (_serviceCallsInTimeoutQueue)
                {
                    for (int i = 0, isize = _serviceCallsInTimeoutQueue.Count; i < isize; ++i)
                    {
                        callsToProcess.Add(_serviceCallsInTimeoutQueue[i]);
                    }
                    _serviceCallsInTimeoutQueue.Clear();
                }
                lock (_serviceCallsWaiting)
                {
                    for (int i = 0, isize = _serviceCallsWaiting.Count; i < isize; ++i)
                    {
                        callsToProcess.Add(_serviceCallsWaiting[i]);
                    }
                    _serviceCallsWaiting.Clear();
                }
                lock (_serviceCallsInProgress)
                {
                    _serviceCallsInProgress.Clear(); // shouldn't be anything in here...
                }

                // and send api error callbacks if required
                if (sendApiErrorCallbacks)
                {
                    for (int i = 0, isize = callsToProcess.Count; i < isize; ++i)
                    {
                        ServerCall sc = callsToProcess[i];
                        if (sc.GetCallback() != null)
                        {
                            sc.GetCallback().OnErrorCallback(
                                StatusCodes.CLIENT_NETWORK_ERROR,
                                ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT,
                                "Timeout trying to reach brainCloud server");
                        }
                    }
                }
                _blockingQueue = false;
            }
        }


        internal void InsertEndOfMessageBundleMarker()
        {
            this.AddToQueue(new EndOfBundleMarker());
        }

        /// <summary>
        /// Resets the idle timer.
        /// </summary>
        private void ResetIdleTimer()
        {
            _lastTimePacketSent = DateTime.Now;
        }


        /// <summary>
        /// Handles the response bundle and calls registered callbacks.
        /// </summary>
        /// <param name="jsonData">The received message bundle.</param>
        private void HandleResponseBundle(string jsonData)
        {
            _clientRef.Log(String.Format("{0} - {1}\n{2}", "RESPONSE", DateTime.Now, jsonData));

            if (string.IsNullOrEmpty(jsonData))
            {
                _clientRef.Log("ERROR - Incoming packet data was null or empty! This is probably a network issue.");
                return;
            }

            JsonResponseBundleV2 bundleObj = JsonReader.Deserialize<JsonResponseBundleV2>(jsonData);
            long receivedPacketId = (long)bundleObj.packetId;
            if (_expectedIncomingPacketId == NO_PACKET_EXPECTED || _expectedIncomingPacketId != receivedPacketId)
            {
                _clientRef.Log("Dropping duplicate packet");
                return;
            }
            _expectedIncomingPacketId = NO_PACKET_EXPECTED;

            Dictionary<string, object>[] responseBundle = bundleObj.responses;
            Dictionary<string, object> response = null;
            IList<Exception> exceptions = new List<Exception>();

            for (int j = 0; j < responseBundle.Length; ++j)
            {
                response = responseBundle[j];
                int statusCode = (int)response["status"];
                string data = "";

                //
                // It's important to note here that a user error callback *might* call
                // ResetCommunications() based on the error being returned.
                // ResetCommunications will clear the _serviceCallsInProgress List
                // effectively removing all registered callbacks for this message bundle.
                // It's also likely that the developer will want to call authenticate next.
                // We need to ensure that this is supported as it's the best way to 
                // reset the brainCloud communications after a session invalid or network
                // error is triggered.
                //
                // This is safe to do from the main thread but just in case someone
                // calls this method from another thread, we lock on _serviceCallsWaiting
                //
                ServerCall sc = null;
                lock (_serviceCallsWaiting)
                {
                    if (_serviceCallsInProgress.Count > 0)
                    {
                        sc = _serviceCallsInProgress[0] as ServerCall;
                        _serviceCallsInProgress.RemoveAt(0);
                    }
                }

                // its a success response
                if (statusCode == 200)
                {
                    ResetKillSwitch();

                    Dictionary<string, object> responseData = null;
                    if (response[OperationParam.ServiceMessageData.Value] != null)
                    {
                        responseData = (Dictionary<string, object>)response[OperationParam.ServiceMessageData.Value];

                        // send the data back as not formatted
                        data = JsonWriter.Serialize(response);

                        // save the session ID
                        string sessionId = GetJsonString(responseData, OperationParam.ServiceMessageSessionId.Value, null);
                        if (sessionId != null)
                        {
                            SessionID = sessionId;
                            _isAuthenticated = true;
                        }

                        string profileId = GetJsonString(responseData, OperationParam.ProfileId.Value, null);
                        if (profileId != null)
                        {
                            _clientRef.AuthenticationService.ProfileId = profileId;
                        }
                    }

                    // now try to execute the callback
                    if (sc != null)
                    {
                        string operation = sc.GetOperation();
                        bool bIsPeerScriptUploadCall = false;
                        try
                        {
                            bIsPeerScriptUploadCall = operation == ServiceOperation.RunPeerScript.Value &&
                                                     response.ContainsKey(OperationParam.ServiceMessageData.Value) &&
                                                     ((Dictionary<string, object>)response[OperationParam.ServiceMessageData.Value]).ContainsKey("response") &&
                                                     ((Dictionary<string, object>)((Dictionary<string, object>)response[OperationParam.ServiceMessageData.Value])["response"]).ContainsKey(OperationParam.ServiceMessageData.Value) &&
                                                     ((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)response[OperationParam.ServiceMessageData.Value])["response"])[OperationParam.ServiceMessageData.Value]).ContainsKey("fileDetails");
                        }
                        catch (Exception) { }

                        if (operation == ServiceOperation.FullReset.Value ||
                            operation == ServiceOperation.Logout.Value)
                        {
                            // we reset the current player or logged out
                            // we are no longer authenticated
                            _isAuthenticated = false;
                            SessionID = "";
                            _clientRef.AuthenticationService.ClearSavedProfileID();
                            ResetErrorCache();
                        }
                        else if (operation == ServiceOperation.Authenticate.Value)
                        {
                            ProcessAuthenticate(data);
                        }
                        // switch to child
                        else if (operation.Equals(ServiceOperation.SwitchToChildProfile.Value) ||
                            operation.Equals(ServiceOperation.SwitchToParentProfile.Value))
                        {
                            ProcessSwitchResponse(data);
                        }
                        else if (operation == ServiceOperation.PrepareUserUpload.Value || bIsPeerScriptUploadCall)
                        {
                            var uploadData = (Dictionary<string, object>)response[OperationParam.ServiceMessageData.Value];
                            string peerCode = bIsPeerScriptUploadCall && sc.GetJsonData().Contains("peer") ? (string)sc.GetJsonData()["peer"] : "";
                            var fileData = peerCode == "" ? (Dictionary<string, object>)uploadData["fileDetails"] :
                                (Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)uploadData["response"])[OperationParam.ServiceMessageData.Value])["fileDetails"];

                            if (fileData.ContainsKey("uploadId") && fileData.ContainsKey("localPath"))
                            {

                                string uploadId = (string)fileData["uploadId"];
                                string localPath = (string)fileData["localPath"];
                                var uploader = new FileUploader(uploadId, localPath, UploadURL, SessionID,
                                    _uploadLowTransferRateTimeout, _uploadLowTransferRateThreshold, _clientRef, peerCode);
#if DOT_NET
                                uploader.HttpClient = _httpClient;
#endif
                                _fileUploads.Add(uploader);
                                uploader.Start();
                            }
                        }

                        // // only process callbacks that are real
                        if (sc.GetCallback() != null)
                        {
                            try
                            {
#if UNITY_EDITOR
                                BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnSuccess(data);
#endif

                                sc.GetCallback().OnSuccessCallback(data);
                            }
                            catch (Exception e)
                            {
                                _clientRef.Log(e.StackTrace);
                                exceptions.Add(e);
                            }
                        }

                        // now deal with rewards
                        if (_rewardCallback != null && responseData != null)
                        {
                            try
                            {
                                Dictionary<string, object> rewards = null;

                                // it's an operation that return a reward
                                if (operation == ServiceOperation.Authenticate.Value)
                                {
                                    object objRewards = null;
                                    if (responseData.TryGetValue("rewards", out objRewards))
                                    {
                                        Dictionary<string, object> outerRewards = (Dictionary<string, object>)objRewards;
                                        if (outerRewards.TryGetValue("rewards", out objRewards))
                                        {
                                            Dictionary<string, object> innerRewards = (Dictionary<string, object>)objRewards;
                                            if (innerRewards.Count > 0)
                                            {
                                                // we found rewards
                                                rewards = outerRewards;
                                            }
                                        }
                                    }
                                }
                                else if (operation == ServiceOperation.Update.Value ||
                                    operation == ServiceOperation.Trigger.Value ||
                                    operation == ServiceOperation.TriggerMultiple.Value)
                                {
                                    object objRewards = null;
                                    if (responseData.TryGetValue("rewards", out objRewards))
                                    {
                                        Dictionary<string, object> innerRewards = (Dictionary<string, object>)objRewards;
                                        if (innerRewards.Count > 0)
                                        {
                                            // we found rewards
                                            rewards = responseData;
                                        }
                                    }
                                }

                                if (rewards != null)
                                {
                                    Dictionary<string, object> theReward = new Dictionary<string, object>();
                                    theReward["rewards"] = rewards;
                                    theReward["service"] = sc.GetService();
                                    theReward["operation"] = sc.GetOperation();
                                    Dictionary<string, object> apiRewards = new Dictionary<string, object>();
                                    List<object> rewardList = new List<object>();
                                    rewardList.Add(theReward);
                                    apiRewards["apiRewards"] = rewardList;

                                    string rewardsAsJson = JsonWriter.Serialize(apiRewards);

#if UNITY_EDITOR
                                    BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnReward(rewardsAsJson);
#endif

                                    _rewardCallback(rewardsAsJson);
                                }
                            }
                            catch (Exception e)
                            {
                                _clientRef.Log(e.StackTrace);
                                exceptions.Add(e);
                            }
                        }
                    }
                }
                else //if non-200
                {
                    object reasonCodeObj = null, statusMessageObj = null;
                    int reasonCode = 0;
                    string errorJson = "";

                    //if it was an authentication call 
                    if(sc.GetOperation() == "AUTHENTICATE")
                    {
                        //we handle the logic in the kill switch, but we want to keep track of the new profileId being used, and keep note of the last one we used. 
                        //compare the current response profileId with the last used one.
                        string profileIdInUse = ""; 
                        object profileIdObj = null;
                        //store the last used profile Id into slot 1 and the newest in slot 0. This way we can keep both stored for when testing the killswitch conditions, and track changes. 
                        _recentlyUsedProfileIdsForAuth[1] = _recentlyUsedProfileIdsForAuth[0];
                        response.TryGetValue("profileId", out profileIdObj);
                        _recentlyUsedProfileIdsForAuth[0] = (string)profileIdObj;
                    }

                    if (response.TryGetValue("reason_code", out reasonCodeObj))
                    {
                        reasonCode = (int)reasonCodeObj;
                    }

                    if (_oldStyleStatusResponseInErrorCallback)
                    {
                        if (response.TryGetValue("status_message", out statusMessageObj))
                        {
                            errorJson = (string)statusMessageObj;
                        }
                    }
                    else
                    {
                        errorJson = JsonWriter.Serialize(response);
                    }

                    if (reasonCode == ReasonCodes.PLAYER_SESSION_EXPIRED
                        || reasonCode == ReasonCodes.NO_SESSION
                        || reasonCode == ReasonCodes.PLAYER_SESSION_LOGGED_OUT)
                    {
                        _isAuthenticated = false;
                        SessionID = "";
                        _clientRef.Log("Received session expired or not found, need to re-authenticate");

                        // cache error if session related
                        _cachedStatusCode = statusCode;
                        _cachedReasonCode = reasonCode;

                        object status = null;
                        if (response.TryGetValue("status_message", out status))
                        {
                            _cachedStatusMessage = status as string;
                        }
                    }

                    if (sc != null && sc.GetOperation() == ServiceOperation.Logout.Value)
                    {
                        if (reasonCode == ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT)
                        {
                            _isAuthenticated = false;
                            SessionID = "";
                            _clientRef.Log("Could not communicate with the server on logout due to network timeout");
                        }
                    }

                    // now try to execute the callback
                    if (sc != null && sc.GetCallback() != null)
                    {
                        try
                        {
                            sc.GetCallback().OnErrorCallback(statusCode, reasonCode, errorJson);
                        }
                        catch (Exception e)
                        {
                            _clientRef.Log(e.StackTrace);
                            exceptions.Add(e);
                        }
                    }

                    if (_globalErrorCallback != null)
                    {
                        object cbObject = null;
                        if (sc != null && sc.GetCallback() != null)
                        {
                            cbObject = sc.GetCallback().m_cbObject;
                            // if this is the internal BrainCloudWrapper callback object return the user-supplied
                            // callback object instead
                            if (cbObject != null && cbObject is WrapperAuthCallbackObject)
                            {
                                cbObject = ((WrapperAuthCallbackObject)cbObject)._cbObject;
                            }
                        }

#if UNITY_EDITOR
                        BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnFailedResponse(errorJson);
#endif

                        _globalErrorCallback(statusCode, reasonCode, errorJson, cbObject);
                    }

                    UpdateKillSwitch(sc.Service, sc.Operation, statusCode);
                }
            }

#if UNITY_EDITOR
            //Send Events to the Unity Plugin
            if (bundleObj.events != null)
            {
                try
                {
                    Dictionary<string, Dictionary<string, object>[]> eventsJsonObjUnity =
                        new Dictionary<string, Dictionary<string, object>[]>();
                    eventsJsonObjUnity["events"] = bundleObj.events;
                    string eventsAsJsonUnity = JsonWriter.Serialize(eventsJsonObjUnity);

                    BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnEvent(eventsAsJsonUnity);
                }
                catch (Exception)
                {
                    //Ignored
                }
            }
#endif


            if (bundleObj.events != null && _eventCallback != null)
            {
                Dictionary<string, Dictionary<string, object>[]> eventsJsonObj = new Dictionary<string, Dictionary<string, object>[]>();
                eventsJsonObj["events"] = bundleObj.events;
                string eventsAsJson = JsonWriter.Serialize(eventsJsonObj);
                try
                {
                    _eventCallback(eventsAsJson);
                }
                catch (Exception e)
                {
                    _clientRef.Log(e.StackTrace);
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0)
            {
                _activeRequest = null; // to make sure we don't reprocess this message

                throw new Exception("User callback handlers threw " + exceptions.Count + " exception(s)."
                                    + " See the Unity log for callstacks or inner exception for first exception thrown.",
                                    exceptions[0]);
            }
        }

        private void UpdateKillSwitch(string service, string operation, int statusCode)
        {
            if (statusCode == StatusCodes.CLIENT_NETWORK_ERROR) return;

            if (_killSwitchService == null)
            {
                _killSwitchService = service;
                _killSwitchOperation = operation;
                _killSwitchErrorCount++;
            }
            else if (service == _killSwitchService && operation == _killSwitchOperation)
                _killSwitchErrorCount++;

            if (!_killSwitchEngaged && _killSwitchErrorCount >= _killSwitchThreshold)
            {
                _killSwitchEngaged = true;
                _clientRef.Log("Client disabled due to repeated errors from a single API call: " + service + " | " + operation);
            }

            //Authentication check for kill switch. 
            //did the client make an authentication call?
            if(operation == ServiceOperation.Authenticate.Value)
            {
                _clientRef.Log("Failed authentication call");
                //increment the amount of failed authentication attempts
                _failedAuthenticationAttempts++;
                string num;
                num = _failedAuthenticationAttempts.ToString();
                _clientRef.Log("Current number of failed authentications: " + num);
                
                //check the recently used profileIds, are they the same?
                if(_recentlyUsedProfileIdsForAuth[0] == _recentlyUsedProfileIdsForAuth[1])
                {
                    //give a suggestion to the client if we catch a recurring problem. 
                    _clientRef.Log("Same profileId used and still failing, try a different profileId");
                }

                //have the attempts gone beyond the threshold?
                if(_failedAuthenticationAttempts >= _failedAuthenticationAttemptThreshold)
                {
                    //we have a problem now, it seems they are contiuously trying to authenticate and sending us too many errors.
                    //we are going to now engage the killswitch and disable the client. May as well have it behave the same as if
                    //the client were failing too many of a regular api call.  
                    _killSwitchEngaged = true;
                    _tooManyFailedAuthentications = true;
                    _clientRef.Log("Too many repeat authentication failures");
                }
                
            }
        }

        private void ResetKillSwitch()
        {
            _killSwitchErrorCount = 0;
            _killSwitchService = null;
            _killSwitchOperation = null;

            //reset the amount of failed attempts and 
            _failedAuthenticationAttempts = 0;
            _recentlyUsedProfileIdsForAuth[0] = "";
            _recentlyUsedProfileIdsForAuth[1] = "";
            _tooManyFailedAuthentications = false;
        }

        /// <summary>
        /// Creates the request state object and sends the message bundle
        /// </summary>
        /// <returns>The and send next request bundle.</returns>
        private RequestState CreateAndSendNextRequestBundle()
        {
            RequestState requestState = null;
            lock (_serviceCallsWaiting)
            {
                if (_blockingQueue)
                {
                    _serviceCallsInProgress.InsertRange(0, _serviceCallsInTimeoutQueue);
                    _serviceCallsInTimeoutQueue.Clear();
                }
                else
                {
                    if (_serviceCallsWaiting.Count > 0)
                    {

                        int numMessagesWaiting = _serviceCallsWaiting.Count;

                        //put auth first
                        for (int i = 0; i < numMessagesWaiting; ++i)
                        {
                            if (_serviceCallsWaiting[i].GetType() == typeof(EndOfBundleMarker))
                                break;

                            if (_serviceCallsWaiting[i].GetOperation() == ServiceOperation.Authenticate.Value)
                            {
                                if (i != 0)
                                {
                                    var call = _serviceCallsWaiting[i];
                                    _serviceCallsWaiting.RemoveAt(i);
                                    _serviceCallsWaiting.Insert(0, call);
                                }

                                numMessagesWaiting = 1;
                                break;
                            }
                        }

                        if (numMessagesWaiting > _maxBundleMessages)
                        {
                            numMessagesWaiting = _maxBundleMessages;
                        }

                        // check for end of bundle markers
                        for (int i = 0; i < numMessagesWaiting; ++i)
                        {
                            if (_serviceCallsWaiting[i].GetType() == typeof(EndOfBundleMarker))
                            {
                                // if the first message is marker, just throw it away
                                if (i == 0)
                                {
                                    _serviceCallsWaiting.RemoveAt(0);
                                    --i;
                                    --numMessagesWaiting;
                                }
                                else // otherwise cut off the bundle at the marker and toss marker away
                                {
                                    numMessagesWaiting = i;
                                    _serviceCallsWaiting.RemoveAt(i);
                                    break;
                                }
                            }
                        }

                        if (numMessagesWaiting <= 0)
                        {
                            return null;
                        }

                        if (_serviceCallsInProgress.Count > 0)
                        {
                            // this should never happen
                            _clientRef.Log("ERROR - in progress queue is not empty but we're ready for the next message!");
                            _serviceCallsInProgress.Clear();
                        }

                        _serviceCallsInProgress = _serviceCallsWaiting.GetRange(0, numMessagesWaiting);
                        _serviceCallsWaiting.RemoveRange(0, numMessagesWaiting);
                    }
                }

                if (_serviceCallsInProgress.Count > 0)
                {
                    requestState = new RequestState();

                    // prepare json data for server
                    List<object> messageList = new List<object>();
                    bool isAuth = false;

                    ServerCall scIndex;
                    for (int i = 0; i < _serviceCallsInProgress.Count; ++i)
                    {
                        scIndex = _serviceCallsInProgress[i] as ServerCall;
                        string operation = scIndex.GetOperation();
                        string service = scIndex.GetService();

                        // don't send heartbeat if it was generated by comms (null callbacks)
                        // and there are other messages in the bundle - it's unnecessary
                        if (service.Equals(ServiceName.HeartBeat.Value)
                            && operation.Equals(ServiceOperation.Read.Value)
                            && (scIndex.GetCallback() == null
                                || scIndex.GetCallback().AreCallbacksNull()))
                        {
                            if (_serviceCallsInProgress.Count > 1)
                            {
                                _serviceCallsInProgress.RemoveAt(i);
                                --i;
                                continue;
                            }
                        }

                        Dictionary<string, object> message = new Dictionary<string, object>();
                        message[OperationParam.ServiceMessageService.Value] = scIndex.Service;
                        message[OperationParam.ServiceMessageOperation.Value] = scIndex.Operation;
                        message[OperationParam.ServiceMessageData.Value] = scIndex.GetJsonData();

                        messageList.Add(message);

                        if (operation.Equals(ServiceOperation.Authenticate.Value))
                        {
                            requestState.PacketNoRetry = true;
                        }

                        if (operation.Equals(ServiceOperation.Authenticate.Value) ||
                            operation.Equals(ServiceOperation.ResetEmailPassword.Value) ||
                            operation.Equals(ServiceOperation.ResetEmailPasswordAdvanced.Value))
                        {
                            isAuth = true;
                        }

                        if (operation.Equals(ServiceOperation.FullReset.Value) ||
                            operation.Equals(ServiceOperation.Logout.Value))
                        {
                            requestState.PacketRequiresLongTimeout = true;
                        }
                    }

                    requestState.PacketId = _packetId;
                    _expectedIncomingPacketId = _packetId;
                    requestState.MessageList = messageList;
                    ++_packetId;

                    if (!_killSwitchEngaged)
                    {
                        if (_isAuthenticated || isAuth)
                            InternalSendMessage(requestState);
                        else
                        {
                            FakeErrorResponse(requestState, _cachedStatusCode, _cachedReasonCode, _cachedStatusMessage);
                            requestState = null;
                        }
                    }
                    else
                    {
                        //not really needed, but will be able to help distinguish if its from an authentication call specifically. 
                        if(_tooManyFailedAuthentications == true)
                        {
                            FakeErrorResponse(requestState, StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_DISABLED,
                                "Client has been disabled due to repeated errors from an authentication call");
                            requestState = null;  
                        }
                        else{
                            FakeErrorResponse(requestState, StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_DISABLED,
                                "Client has been disabled due to repeated errors from a single API call");
                            requestState = null;   
                        }
                    }
                }
            } // unlock _serviceCallsWaiting

            return requestState;
        }

        /// <summary>
        /// Creates a fake response to stop packets being sent to the server without a valid session.
        /// </summary>
        private void FakeErrorResponse(RequestState requestState, int statusCode, int reasonCode, string statusMessage)
        {
            Dictionary<string, object> packet = new Dictionary<string, object>();
            packet[OperationParam.ServiceMessagePacketId.Value] = requestState.PacketId;
            packet[OperationParam.ServiceMessageSessionId.Value] = SessionID;
            if (AppId != null && AppId.Length > 0)
            {
                packet[OperationParam.ServiceMessageGameId.Value] = AppId;
            }
            packet[OperationParam.ServiceMessageMessages.Value] = requestState.MessageList;

            string jsonRequestString = JsonWriter.Serialize(packet);

            _clientRef.Log(string.Format("{0} - {1}\n{2}", "REQUEST" + (requestState.Retries > 0 ? " Retry(" + requestState.Retries + ")" : ""), DateTime.Now, jsonRequestString));

            ResetIdleTimer();

            TriggerCommsError(statusCode, reasonCode, statusMessage);
            _activeRequest = null;
        }

        /// <summary>
        /// Method creates the web request and sends it immediately.
        /// Relies upon the requestState PacketId and MessageList being
        /// set appropriately.
        /// </summary>
        /// <param name="requestState">Request state.</param>
        private void InternalSendMessage(RequestState requestState)
        {
#if DOT_NET
            // During retry, the RequestState is reused so we have to make sure its state goes back to PENDING.
            // Unity uses the info stored in the WWW object and it's recreated here so it's not an issue.
            requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_PENDING;
#endif

            // bundle up the data into a string
            Dictionary<string, object> packet = new Dictionary<string, object>();
            packet[OperationParam.ServiceMessagePacketId.Value] = requestState.PacketId;
            packet[OperationParam.ServiceMessageSessionId.Value] = SessionID;
            if (AppId != null && AppId.Length > 0)
            {
                packet[OperationParam.ServiceMessageGameId.Value] = AppId;
            }
            packet[OperationParam.ServiceMessageMessages.Value] = requestState.MessageList;

            string jsonRequestString = JsonWriter.Serialize(packet);
            string sig = CalculateMD5Hash(jsonRequestString + SecretKey);


#if UNITY_EDITOR
            //Sending Data to the Unity Debug Plugin for ease of developer debugging when in the Editor
            try
            {
                BrainCloudUnity.BrainCloudPlugin.ResponseEvent.ClearLastSentRequest();
                Dictionary<string, object> requestData =
                    JsonReader.Deserialize<Dictionary<string, object>>(jsonRequestString);
                Dictionary<string, object>[] messagesDataList = (Dictionary<string, object>[])requestData["messages"];

                foreach (var messagesData in messagesDataList)
                {
                    var serviceValue = messagesData["service"];
                    var operationValue = messagesData["operation"];
                    var dataList = messagesData["data"];
                    var dataValue = JsonWriter.Serialize(dataList);

                    BrainCloudUnity.BrainCloudPlugin.ResponseEvent.OnSentRequest(
                        string.Format("{0} {1}", serviceValue, operationValue), dataValue);
                }
            }
            catch (Exception)
            {
                //Ignored
            }
#endif


            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequestString);

            requestState.Signature = sig;
            requestState.ByteArray = byteArray;

            if (_debugPacketLossRate > 0.0)
            {
                System.Random r = new System.Random();
                requestState.LoseThisPacket = r.NextDouble() > _debugPacketLossRate;
            }

            if (!requestState.LoseThisPacket)
            {
#if !(DOT_NET)
                Dictionary<string, string> formTable = new Dictionary<string, string>();
                formTable["Content-Type"] = "application/json; charset=utf-8";
                formTable["X-SIG"] = sig;
                if (AppId != null && AppId.Length > 0)
                {
                    formTable["X-APPID"] = AppId;
                }
                WWW request = new WWW(ServerURL, byteArray, formTable);
                requestState.WebRequest = request;
#else

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, new Uri(ServerURL));
                req.Content = new ByteArrayContent(byteArray);
                req.Headers.Add("X-SIG", sig);
                if (AppId != null && AppId.Length > 0) 
                {
                    req.Headers.Add("X-APPID", AppId);
                }
                req.Method = HttpMethod.Post;

                CancellationTokenSource source = new CancellationTokenSource();
                requestState.CancelToken = source;

                Task<HttpResponseMessage> httpRequest = _httpClient.SendAsync(req, HttpCompletionOption.ResponseContentRead, source.Token);
                requestState.WebRequest = httpRequest;
                httpRequest.ContinueWith(async (t) =>
                {
                    await AsyncHttpTaskCallback(t, requestState);
                });
#endif

                requestState.RequestString = jsonRequestString;
                requestState.TimeSent = DateTime.Now;

                ResetIdleTimer();


                _clientRef.Log(string.Format("{0} - {1}\n{2}", "REQUEST" + (requestState.Retries > 0 ? " Retry(" + requestState.Retries + ")" : ""), DateTime.Now, jsonRequestString));

            }
        }

        /// <summary>
        /// Resends a message bundle. Returns true if sent or
        /// false if max retries has been reached.
        /// </summary>
        /// <returns><c>true</c>, if message was resent, <c>false</c> if max retries hit.</returns>
        /// <param name="requestState">Request state.</param>
        private bool ResendMessage(RequestState requestState)
        {
            if (_activeRequest.Retries >= GetMaxRetriesForPacket(requestState))
            {
                return false;
            }
            ++_activeRequest.Retries;
            InternalSendMessage(requestState);
            return true;
        }

        /// <summary>
        /// Gets the web request status.
        /// </summary>
        /// <returns>The web request status.</returns>
        /// <param name="requestState">request state.</param>
        private RequestState.eWebRequestStatus GetWebRequestStatus(RequestState requestState)
        {
            RequestState.eWebRequestStatus status = RequestState.eWebRequestStatus.STATUS_PENDING;

            // for testing packet loss, some packets are flagged to be lost
            // and should always return status pending no matter what the real
            // status is
            if (_activeRequest.LoseThisPacket)
            {
                return status;
            }

#if !(DOT_NET)
            if (!string.IsNullOrEmpty(_activeRequest.WebRequest.error))
            {
                status = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
            else if (_activeRequest.WebRequest.isDone)
            {
                status = RequestState.eWebRequestStatus.STATUS_DONE;
            }
#else
            status = _activeRequest.DotNetRequestStatus;
#endif
            return status;
        }


        /// <summary>
        /// Gets the web request response.
        /// </summary>
        /// <returns>The web request response.</returns>
        /// <param name="requestState">request state.</param>
        private string GetWebRequestResponse(RequestState requestState)
        {
            string response = "";
#if !(DOT_NET)
            if (!string.IsNullOrEmpty(_activeRequest.WebRequest.error))
            {
                response = _activeRequest.WebRequest.error;
            }
            else
            {
                response = _activeRequest.WebRequest.text;
            }
#else
            response = _activeRequest.DotNetResponseString;
#endif
            return response;
        }

        /// <summary>
        /// Method returns the maximum retries for the given packet
        /// </summary>
        /// <returns>The maximum retries for the given packet.</returns>
        /// <param name="requestState">The active request.</param>
        private int GetMaxRetriesForPacket(RequestState requestState)
        {
            if (requestState.PacketNoRetry)
            {
                return 0;
            }
            return _packetTimeouts.Count;
        }

        /// <summary>
        /// Method staggers the packet timeout value based on the currentRetry
        /// </summary>
        /// <returns>The packet timeout.</returns>
        /// <param name="requestState">The active request.</param>
        private TimeSpan GetPacketTimeout(RequestState requestState)
        {
            if (requestState.PacketNoRetry)
            {
                return TimeSpan.FromSeconds(_authPacketTimeoutSecs);
            }

            int currentRetry = requestState.Retries;
            TimeSpan ret;

            // if this is a delete player, or logout we change the
            // timeout behaviour
            if (requestState.PacketRequiresLongTimeout)
            {
                // unused as default timeouts are now quite long
            }

            if (currentRetry >= _packetTimeouts.Count)
            {
                int secs = 10;
                if (_packetTimeouts.Count > 0)
                {
                    secs = _packetTimeouts[_packetTimeouts.Count - 1];
                }
                ret = TimeSpan.FromSeconds(secs);
            }
            else
            {
                ret = TimeSpan.FromSeconds(_packetTimeouts[currentRetry]);
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

#if DISABLE_SSL_CHECK
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
            lock (_serviceCallsWaiting)
            {
                _serviceCallsWaiting.Add(call);
            }
        }

        /// <summary>
        /// Enables the communications layer.
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void EnableComms(bool value)
        {
            _enabled = value;
        }

        /// <summary>
        /// Resets the communication layer. Clients will need to
        /// reauthenticate after this method is called.
        /// </summary>
        internal void ResetCommunication()
        {
            lock (_serviceCallsWaiting)
            {
                _isAuthenticated = false;
                _blockingQueue = false;
                _serviceCallsWaiting.Clear();
                _serviceCallsInProgress.Clear();
                _serviceCallsInTimeoutQueue.Clear();
                _activeRequest = null;
                _clientRef.AuthenticationService.ProfileId = "";
                SessionID = "";
            }
        }


#if (DOT_NET)
        private async Task AsyncHttpTaskCallback(Task<HttpResponseMessage> asyncResult, RequestState requestState)
        {
            if (asyncResult.IsCanceled) return;

            HttpResponseMessage message = null;

            //a callback method to end receiving the data
            try
            {
                message = asyncResult.Result;
                HttpContent content = message.Content;

                // End the operation
                requestState.DotNetResponseString = await content.ReadAsStringAsync();
                requestState.DotNetRequestStatus = message.IsSuccessStatusCode ?
                    RequestState.eWebRequestStatus.STATUS_DONE : RequestState.eWebRequestStatus.STATUS_ERROR;
            }
            catch (WebException wex)
            {
                _clientRef.Log("GetResponseCallback - WebException: " + wex.ToString());
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
            }
            catch (Exception ex)
            {
                _clientRef.Log("GetResponseCallback - Exception: " + ex.ToString());
                requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
            }

            // Release the HttpResponseMessage
            if (message != null) message.Dispose();
        }
#endif


        private string CalculateMD5Hash(string input)
        {
#if !(DOT_NET)
            MD5Unity.MD5 md5 = MD5Unity.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input); // UTF8, not ASCII
            byte[] hash = md5.ComputeHash(inputBytes);
#else
#if UWP
            Windows.Security.Cryptography.MD5 md5 = Windows.Security.Cryptography.MD5.Create();
#else
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
#endif
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

        /// <summary>
        /// Handles authenticate-specific data from successful request
        /// </summary>
        /// <param name="jsonString"></param>
        private void ProcessAuthenticate(string jsonString)
        {
            Dictionary<string, object> jsonMessage = (Dictionary<string, object>)JsonReader.Deserialize(jsonString);
            Dictionary<string, object> jsonData = (Dictionary<string, object>)jsonMessage["data"];

            long playerSessionExpiry = GetJsonLong(jsonData, OperationParam.AuthenticateServicePlayerSessionExpiry.Value, 5 * 60);
            long idleTimeout = (long)(playerSessionExpiry * 0.85);

            _idleTimeout = TimeSpan.FromSeconds(idleTimeout);

            object bundleMsgs = null;
            jsonData.TryGetValue("maxBundleMsgs", out bundleMsgs);
            if (bundleMsgs != null) _maxBundleMessages = (int)bundleMsgs;

            object killCount = null;
            jsonData.TryGetValue("maxKillCount", out killCount);
            if (killCount != null) _killSwitchThreshold = (int)killCount;

            ResetErrorCache();
            _isAuthenticated = true;
        }

        private void ProcessSwitchResponse(string jsonString)
        {
            Dictionary<string, object> jsonMessage = (Dictionary<string, object>)JsonReader.Deserialize(jsonString);
            Dictionary<string, object> jsonData = (Dictionary<string, object>)jsonMessage["data"];

            if (jsonData.ContainsKey("switchToAppId"))
            {
                string switchToAppId = (string)jsonData["switchToAppId"];
                AppId = switchToAppId;
            }
        }

        private static string GetJsonString(Dictionary<string, object> jsonData, string key, string defaultReturn)
        {
            object retVal = null;
            jsonData.TryGetValue(key, out retVal);
            return retVal != null ? retVal as string : defaultReturn;
        }


        private static long GetJsonLong(Dictionary<string, object> jsonData, string key, long defaultReturn)
        {
            object outObj = null;
            if (jsonData.TryGetValue(key, out outObj))
            {
                if (outObj is long)
                    return (long)outObj;
                if (outObj is int)
                    return (int)outObj;
            }

            return defaultReturn;
        }

        /// <summary>
        /// Resets the cached error message for local session error handling to default
        /// </summary>
        private void ResetErrorCache()
        {
            _cachedStatusCode = StatusCodes.FORBIDDEN;
            _cachedReasonCode = ReasonCodes.NO_SESSION;
            _cachedStatusMessage = "No session";
        }
    }

    #region Json parsing objects

    // Classes to handle JSON serialization - do not
    // try to make variables conform to coding standards as
    // they must match json variable name format exactly

    internal class JsonResponseBundleV2
    {
        public long packetId = 0;
        public Dictionary<string, object>[] responses = null;
        public Dictionary<string, object>[] events = null;

        public JsonResponseBundleV2()
        { }
    }

    internal class JsonResponseErrorBundleV2
    {
        public long packetId;
        public JsonErrorMessage[] responses;

        public JsonResponseErrorBundleV2()
        { }
    }

    internal class JsonErrorMessage
    {
        public int reason_code;
        public int status;
        public string status_message;
        public string severity = "ERROR";

        public JsonErrorMessage()
        { }

        public JsonErrorMessage(int status, int reasonCode, string statusMessage)
        {
            this.status = status;
            reason_code = reasonCode;
            status_message = statusMessage;
        }

        public string GetJsonString()
        {
            return JsonWriter.Serialize(this);
        }
    }
    #endregion
}
