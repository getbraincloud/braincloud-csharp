// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

#if ((UNITY_5_3_OR_NEWER) && !UNITY_WEBPLAYER && (!UNITY_IOS || ENABLE_IL2CPP)) || UNITY_2018_3_OR_NEWER
#define USE_WEB_REQUEST // Comment out to force use of old WWW class on Unity 5.3+
using BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp;
using System.Collections.Generic;
#endif

namespace BrainCloud.Internal
{
    using BrainCloud.Common;
    using BrainCloud.JsonFx.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

#if (DOT_NET || GODOT || DISABLE_SSL_CHECK)
    using System.Net;
#endif
#if DOT_NET || GODOT
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Threading;
    using BrainCloud.ModernHttpClient;
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
        /// Enables automatic re-authentication when the user's session expires
        /// </summary>
        public bool AutoReconnectEnabled { get; private set; } = false;

        public void EnableAutoReconnect(bool enabled)
        {
            AutoReconnectEnabled = enabled;
        }

        /// <summary>
        ///Compress bundles sent from the client to the server for faster sending of large bundles.
        /// </summary>
        public bool SupportsCompression { get; private set; } = true;

        public void EnableCompression(bool compress)
        {
            SupportsCompression = compress;
        }

        /// <summary>
        /// Byte size threshold that determines if the message size is something we want to compress or not.
        /// We make an initial value, but recevie the value for future calls based on the servers auth response
        /// </summary>
        public int ClientSideCompressionThreshold { get; private set; } = 51200;

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
        private long _expectedIncomingPacketId = JsonResponseBundleV2.NO_PACKET_EXPECTED;

        /// <summary>
        /// Default time for how long a session should expire before timing out.
        /// </summary>
        private long _defaultPlayerSessionExpiry => 5 * 60; // 5 Minutes

        /// <summary>
        /// Modify expiry timeout value by this percentage.
        /// </summary>
        private double _idleTimeoutModifier => 0.85;

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
        private int _identicalFailedAuthAttemptThreshold = 3;

        ///<summary>
        ///The current number of identical failed attempts at authenticating. This 
        ///will reset when a successful authentication is made.
        ///<summary>
        private int _failedAuthenticationAttempts = 0;

        ///<summary>
        ///A blank reference for response data so we don't need to continually allocate new dictionaries when trying to
        ///make the data blank again.
        ///<summary>
        private Dictionary<string, object> blankResponseData = new Dictionary<string, object>();

        ///<summary>
        ///An array that stores the most recent response jsons as dictionaries.
        ///<summary>
        private Dictionary<string, object>[] _recentResponseJsonData = { new Dictionary<string, object>(), new Dictionary<string, object>() };

        /// <summary>
        /// When we have too many authentication errors under the same credentials, 
        /// the client will not be able to try and authenticate again until the timer is up.
        /// </summary>
        private TimeSpan _authenticationTimeoutDuration = TimeSpan.FromSeconds(30);

        /// <summary>
        /// When the authentication timer began 
        /// </summary>
        private DateTime _authenticationTimeoutStart;

        /// a checker to see what the packet Id we are receiving is 
        private long receivedPacketIdChecker = 0;

        /// <summary>
        /// Debug value to introduce packet loss for testing retries etc.
        /// </summary>
        //private double _debugPacketLossRate = 0;

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
        
        private LongSessionCallback _autoReconnectCallback;

        private List<FileUploader> _fileUploads = new List<FileUploader>();

#if DOT_NET || GODOT
        private HttpClient _httpClient = new HttpClient(new NativeMessageHandler());
#endif

        // For handling local session errors
        private int _cachedStatusCode;
        private int _cachedReasonCode;
        private string _cachedStatusMessage;

        // For kill switch
        private bool _killSwitchEngaged;
        public bool KillSwitchEngaged
        {
            get => _killSwitchEngaged;
            set => _killSwitchEngaged = value;
        }
        private int _killSwitchErrorCount;
        private string _killSwitchService;
        private string _killSwitchOperation;

        private bool _authInProgress = false;
        public bool AuthenticateInProgress
        {
            get => _authInProgress;
            set => _authInProgress = value;
        }

        private bool _isAuthenticated = false;

        public bool Authenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public long GetReceivedPacketId()
        {
            return receivedPacketIdChecker;
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
        private List<int> _packetTimeouts = new() { 15, 20, 35, 50 };
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
            _packetTimeouts = new List<int> { 15, 20, 35, 50 };
        }

        private readonly int[] _listAuthPacketTimeouts = { 15, 30, 60 };
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

        // Json Serialization
        private JsonWriterSettings _writerSettings = new(); // Used to adjust settings such as maxdepth while serializing. A new JsonWriterSettings does not need to be created everytime we serialize.
        private StringBuilder _stringBuilderOutput;         // String builder necessary for writing serialized json to a string. Unity complains when this is instantiated at compilation.
        private readonly string JSON_ERROR_MESSAGE = "You have exceeded the max json depth, increase the MaxDepth using the MaxDepth variable in BrainCloudClient.cs";

        private static int _maxDepth = 25; // Set to the default maxDepth within JsonFx SDK
        public int MaxDepth
        {
            get => _maxDepth;
            set
            {
                _maxDepth = value;
                _writerSettings.MaxDepth = _maxDepth;
            }
        }

        /// <summary>
        /// This flag is set when _cacheMessagesOnNetworkError is true
        /// and a timeout occurs. It is reset when a call is made 
        /// to either RetryCachedMessages or FlushCachedMessages
        /// </summary>
        private bool _blockingQueue = false;

        public BrainCloudComms(BrainCloudClient client)
        {
#if DOT_NET || GODOT
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif
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
        /// <param name="appId">AppId</param>
        /// <param name="secretKey">Secret key.</param>
        public void Initialize(string serverURL, string appId, string secretKey)
        {
            ResetCommunication(); //resets comms, packetId and SessionId
            _expectedIncomingPacketId = JsonResponseBundleV2.NO_PACKET_EXPECTED;

            ServerURL = serverURL;

            string suffix = @"/dispatcherv2";
            Uri url = ValidateURL(serverURL);
            ServerURL = url.AbsoluteUri;

            string formatURL = ServerURL.EndsWith(suffix) ? ServerURL.Substring(0, ServerURL.Length - suffix.Length) : ServerURL;

            //get rid of trailing "/" for format URL
            if (formatURL.Length > 0 && formatURL.EndsWith("/"))
            {
                formatURL = formatURL.TrimEnd('/');
            }

            UploadURL = formatURL;
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
        /// <param name="appIdSecretMap">map of appId -> secrets, to allow the client to safely switch between apps with secret being secure</param>
        public void InitializeWithApps(string serverURL, string defaultAppId, Dictionary<string, string> appIdSecretMap)
        {
            AppIdSecretMap.Clear();
            AppIdSecretMap = appIdSecretMap;

            Initialize(serverURL, defaultAppId, AppIdSecretMap[defaultAppId]);
        }

        private Uri ValidateURL(string value)
        {
            try
            {
                UriBuilder builder = new UriBuilder(value)
                {
                    Scheme = Uri.UriSchemeHttps
                };

                if ((string.IsNullOrWhiteSpace(builder.Path) || builder.Path == "/") &&
                    !builder.Path.Contains("dispatcherv2"))
                {
                    builder.Path += builder.Path.EndsWith("/") ? "dispatcherv2" : "/dispatcherv2";
                }

                builder.Path = builder.Path.TrimEnd('/');

                return builder.Uri;
            }
            catch
            {
                _clientRef.Log("URL provided is not valid. Reverting to default URL: https://api.braincloudservers.com/dispatcherv2");
                return new Uri("https://api.braincloudservers.com/dispatcherv2");
            }
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
        
        public void RegisterAutoReconnectCallback(LongSessionCallback callback)
        {
            _autoReconnectCallback = callback;
        }
        
        public void DeregisterAutoReconnectCallback()
        {
            _autoReconnectCallback = null;
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
            RequestState.eWebRequestStatus status = RequestState.eWebRequestStatus.STATUS_PENDING;
            if (_activeRequest != null)
            {
                status = GetWebRequestStatus(_activeRequest);
                if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                {
                    // Force the timeout to be elapsed because we have completed the request with error
                    // or else, do nothing with the error right now - let the timeout code handle it
                    bypassTimeout = (_activeRequest.Retries >= GetMaxRetriesForPacket(_activeRequest));
                }
                else if (status == RequestState.eWebRequestStatus.STATUS_DONE)
                {

#if USE_WEB_REQUEST
                    // HttpStatusCode.OK
                    if (_activeRequest.WebRequest.responseCode == 200)
                    {
                        ResetIdleTimer();
                        HandleResponseBundle(GetWebRequestResponse(_activeRequest));
                        DisposeUploadHandler();
                        _activeRequest = null;
                    }
                    // HttpStatusCode.ServiceUnavailable
                    else if (_activeRequest.WebRequest.responseCode == 503 ||
                             _activeRequest.WebRequest.responseCode == 502 ||
                             _activeRequest.WebRequest.responseCode == 504)
                    {
                        // Packet in progress
                        _clientRef.Log("Packet in progress");
                        RetryRequest(status, bypassTimeout);
                        return;
                    }
                    else
                    {
                        // Error Callback
                        var errorResponse = GetWebRequestResponse(_activeRequest);
                        if (_serviceCallsInProgress.Count > 0)
                        {
                            ServerCallback sc = _serviceCallsInProgress[0].GetCallback();
                            sc?.OnErrorCallback(404, (int)_activeRequest.WebRequest.responseCode, errorResponse);
                        }
                    }
#elif DOT_NET || GODOT
                    //HttpStatusCode.OK
                    if ((int)_activeRequest.WebRequest.Result.StatusCode == 200)
                    {
                        ResetIdleTimer();
                        HandleResponseBundle(GetWebRequestResponse(_activeRequest));
                        _activeRequest = null;
                    }
                    //HttpStatusCode.ServiceUnavailable
                    else if ((int)_activeRequest.WebRequest.Result.StatusCode == 503 ||
                             (int)_activeRequest.WebRequest.Result.StatusCode == 502 ||
                             (int)_activeRequest.WebRequest.Result.StatusCode == 504)
                    {
                        //Packet in progress
                        _clientRef.Log("Packet in progress");
                        RetryRequest(status, bypassTimeout);
                        return;
                    }
                    else
                    {
                        //Error Callback
                        var errorResponse = GetWebRequestResponse(_activeRequest);
                        if (_serviceCallsInProgress.Count > 0)
                        {
                            ServerCallback sc = _serviceCallsInProgress[0].GetCallback();
                            sc?.OnErrorCallback(404, (int)_activeRequest.WebRequest.Result.StatusCode, errorResponse);
                        }
                    }
#endif
                }
            }

            // is it time for a retry?
            RetryRequest(status, bypassTimeout);

            // is it time for a heartbeat?
            if (_isAuthenticated && !_blockingQueue)
            {
                if (DateTime.Now.Subtract(_lastTimePacketSent) >= _idleTimeout)
                {
                    SendHeartbeat();
                }
            }

            // if the client is currently locked on authentication calls. 
            if (tooManyAuthenticationAttempts())
            {
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("TIMER ON");
                    _clientRef.Log(DateTime.Now.Subtract(_authenticationTimeoutStart).ToString());
                }
                // check the timeout, has enough time passed?
                if (DateTime.Now.Subtract(_authenticationTimeoutStart) >= _authenticationTimeoutDuration)
                {
                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log("TIMER FINISHED");
                    }
                    //if the wait time is up they're free to make authentication calls again
                    _killSwitchEngaged = false;
                    ResetKillSwitch();
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
                        _fileUploadSuccessCallback(_fileUploads[i].UploadId, _fileUploads[i].Response);
                    }

                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log("Upload success: " + _fileUploads[i].UploadId + " | " + _fileUploads[i].StatusCode + "\n" + _fileUploads[i].Response);
                    }
                    _fileUploads.RemoveAt(i);
                }
                else if (_fileUploads[i].Status == FileUploader.FileUploaderStatus.CompleteFailed)
                {
                    if (_fileUploadFailedCallback != null)
                    {
                        _fileUploadFailedCallback(_fileUploads[i].UploadId, _fileUploads[i].StatusCode, _fileUploads[i].ReasonCode, _fileUploads[i].Response);
                    }

                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log("Upload failed: " + _fileUploads[i].UploadId + " | " + _fileUploads[i].StatusCode + "\n" + _fileUploads[i].Response);
                    }
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

            if (_clientRef.LoggingEnabled)
            {
                _clientRef.Log("GetUploadProgress could not find upload ID " + uploadId);
            }
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

            string[] responses = new string[numMessagesToReturn];
            for (int i = 0; i < numMessagesToReturn; ++i)
            {
                responses[i] = JsonParser.GetJsonErrorMessage(status, reasonCode, statusMessage);
            }

            HandleResponseBundle(JsonParser.GetJsonResponseErrorBundleV2(_expectedIncomingPacketId, responses));
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

            DisposeUploadHandler();
            _activeRequest = null;

            // and then dump the comms layer
            ResetCommunication();
        }

        internal void ClearAllRequests()
        {
            lock (_serviceCallsWaiting)
            {
                _serviceCallsWaiting.Clear();
            }

            DisposeUploadHandler();
            _activeRequest = null;
        }

        // see BrainCloudClient.RetryCachedMessages() docs
        public void RetryCachedMessages()
        {
            if (_blockingQueue)
            {
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Retrying cached messages");
                }

                if (_activeRequest != null)
                {
                    // this is definitely an error in the comms lib if it happens. 
                    // we attempt to cancel it but this is uncharted territory.

                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log("ERROR - retrying cached messages but there is an active request!");
                    }
                    _activeRequest.CancelRequest();
                    DisposeUploadHandler();
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
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Flushing cached messages");
                }

                // try to cancel if request is in progress (shouldn't happen)
                if (_activeRequest != null)
                {
                    _activeRequest.CancelRequest();
                    DisposeUploadHandler();
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
                                "Timeout trying to reach brainCloud server, please check the URL and/or certificates for server");
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
        /// Starts timeout of authentication calls.
        /// </summary>
        private void ResetAuthenticationTimer()
        {
            _authenticationTimeoutStart = DateTime.Now;
        }

        ///<summary>
        ///keeps track of if the client has made too many authentication attempts.
        ///<summary>
        private bool tooManyAuthenticationAttempts()
        {
            return _failedAuthenticationAttempts >= _identicalFailedAuthAttemptThreshold;
        }

        /// <summary>
        /// Saves the profileId and sessionIds from the authentication.
        /// </summary>
        private void SaveProfileAndSessionIds(string jsonData)
        {
            // save the session ID
            string sessionId = JsonParser.GetString(jsonData, OperationParam.ServiceMessageSessionId);
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                SessionID = sessionId;
                _isAuthenticated = true;
                _authInProgress = false;
            }

            // save the profile Id
            string profileId = JsonParser.GetString(jsonData, OperationParam.ProfileId);
            if (!string.IsNullOrWhiteSpace(profileId))
            {
                _clientRef.AuthenticationService.ProfileId = profileId;
            }
        }

        /// <summary>
        /// Handles the response bundle and calls registered callbacks.
        /// </summary>
        /// <param name="jsonData">The received message bundle.</param>
        private void HandleResponseBundle(string jsonData)
        {
            if (_clientRef.LoggingEnabled)
            {
                _clientRef.Log(string.Format("{0} - {1}\n{2}", "RESPONSE", DateTime.Now, jsonData));
            }

            JsonResponseBundleV2 bundleObj = DeserializeJsonBundle(jsonData);
            if (bundleObj.IsEmpty || bundleObj.IsError)
            {
                _cachedReasonCode = ReasonCodes.JSON_PARSING_ERROR;
                _cachedStatusCode = StatusCodes.CLIENT_NETWORK_ERROR;
                _cachedStatusMessage = "Received an invalid json format response, check your network settings.";
                _cacheMessagesOnNetworkError = true;
                lock (_serviceCallsWaiting)
                {
                    if (_serviceCallsInProgress.Count > 0)
                    {
                        var serverCall = _serviceCallsInProgress[0];
                        if (serverCall?.GetCallback() != null)
                        {
                            serverCall.GetCallback().OnErrorCallback(_cachedStatusCode, _cachedReasonCode, _cachedStatusMessage);
                            _serviceCallsInProgress.RemoveAt(0);
                        }
                    }
                }
                _clientRef.Log(_cachedStatusMessage);
                return;
            }

            string[] responseBundle = bundleObj.responses;
            string response = string.Empty;
            long receivedPacketId = bundleObj.packetId;
            receivedPacketIdChecker = receivedPacketId;

            // if the receivedPacketId is NO_PACKET_EXPECTED (-1), its a serious error, which cannot be retried
            // errors for whcih NO_PACKET_EXPECTED are:
            // json parsing error, missing packet id, app secret changed via the portal
            if (receivedPacketId != JsonResponseBundleV2.NO_PACKET_EXPECTED && (_expectedIncomingPacketId == JsonResponseBundleV2.NO_PACKET_EXPECTED || _expectedIncomingPacketId != receivedPacketId))
            {
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Dropping duplicate packet");
                }

                for (int j = 0; j < responseBundle.Length; ++j)
                {
                    lock (_serviceCallsInProgress)
                    {
                        if (_serviceCallsInProgress.Count > 0)
                        {
                            _serviceCallsInProgress.RemoveAt(0);
                        }
                    }
                }
                return;
            }

            _expectedIncomingPacketId = JsonResponseBundleV2.NO_PACKET_EXPECTED;
            IList<Exception> exceptions = new List<Exception>();

            ServerCall sc = null;
            ServerCallback callback = null;
            string service = string.Empty;
            string operation = string.Empty;
            string responseData = string.Empty;
            for (int j = 0; j < responseBundle.Length; ++j)
            {
                response = responseBundle[j];
                sc = null;
                callback = null;
                service = string.Empty;
                operation = string.Empty;
                responseData = string.Empty;

                int statusCode = JsonParser.GetValue<int>(response, "status") is int code && code > 0 ? code
                                                                                                      : StatusCodes.BAD_REQUEST;

                /*
                 * It's important to note here that a user error callback *might* call
                 * ResetCommunications() based on the error being returned.
                 * ResetCommunications will clear the _serviceCallsInProgress List
                 * effectively removing all registered callbacks for this message bundle.
                 * It's also likely that the developer will want to call authenticate next.
                 * We need to ensure that this is supported as it's the best way to 
                 * reset the brainCloud communications after a session invalid or network
                 * error is triggered.
                 * 
                 * This is safe to do from the main thread but just in case someone
                 * calls this method from another thread, we lock on _serviceCallsWaiting
                */
                lock (_serviceCallsWaiting)
                {
                    if (_serviceCallsInProgress.Count > 0)
                    {
                        sc = _serviceCallsInProgress[0];
                        _serviceCallsInProgress.RemoveAt(0);
                    }
                }

                if (statusCode == 200) // A success response
                {
                    ResetKillSwitch();
                    service = sc.GetService();
                    if (JsonParser.TryGetString(response, out responseData, OperationParam.ServiceMessageData))
                    {
                        if (service == ServiceName.Authenticate || service == ServiceName.Identity)
                        {
                            // Reset authenticate timeout
                            _authPacketTimeoutSecs = _listAuthPacketTimeouts[0];
                            SaveProfileAndSessionIds(responseData);
                        }
                    }

                    // now try to execute the callback
                    if (sc != null)
                    {
                        callback = sc.GetCallback();
                        operation = sc.GetOperation();
                        string fileDetails = string.Empty;
                        if (operation == ServiceOperation.RunPeerScript)
                        {
                            JsonParser.TryGetString(responseData, out fileDetails, OperationParam.ServiceMessageData, "response", OperationParam.ServiceMessageData, "fileDetails");
                        }

                        if (operation == ServiceOperation.FullReset ||
                            operation == ServiceOperation.Logout)
                        {
                            // we reset the current player or logged out
                            // we are no longer authenticated
                            _isAuthenticated = false;
                            SessionID = "";
                            if (operation == ServiceOperation.FullReset)
                            {
                                _clientRef.AuthenticationService.ClearSavedProfileID();
                            }

                            ResetErrorCache();
                        }
                        // either off of authenticate or identity call, be sure to save the profileId and sessionId
                        else if (operation == ServiceOperation.Authenticate)
                        {
                            ProcessAuthenticate(responseData);
                        }
                        // switch to child
                        else if (operation.Equals(ServiceOperation.SwitchToChildProfile) ||
                                 operation.Equals(ServiceOperation.SwitchToParentProfile))
                        {
                            ProcessSwitchResponse(responseData);
                        }
                        else if (operation == ServiceOperation.PrepareUserUpload || !string.IsNullOrWhiteSpace(fileDetails))
                        {
                            string peerCode = !string.IsNullOrWhiteSpace(fileDetails) && sc.GetJsonData().Contains("peer") ? (string)sc.GetJsonData()["peer"] : string.Empty;
                            fileDetails = string.IsNullOrWhiteSpace(peerCode) ? JsonParser.GetString(responseData, "fileDetails") : fileDetails;

                            if (JsonParser.TryGetString(fileDetails, out string uploadId, "uploadId") &&
                                JsonParser.TryGetString(fileDetails, out string guid, "localPath"))
                            {
                                string fileName = JsonParser.GetString(fileDetails, "cloudFilename");
                                var uploader = new FileUploader(uploadId,
                                                                guid,
                                                                UploadURL,
                                                                SessionID,
                                                                _uploadLowTransferRateTimeout,
                                                                _uploadLowTransferRateThreshold,
                                                                _clientRef,
                                                                peerCode)
                                {
                                    FileName = fileName
                                };

                                if (_clientRef.FileService.FileStorage.ContainsKey(guid))
                                {
                                    uploader.TotalBytesToTransfer = _clientRef.FileService.FileStorage[guid].Length;
                                }
#if DOT_NET || GODOT
                                uploader.HttpClient = _httpClient;
#endif
                                _fileUploads.Add(uploader);
                                uploader.Start();
                            }
                        }

                        // only process callbacks that are real
                        if (callback != null)
                        {
                            try
                            {
                                callback.OnSuccessCallback(response);
                            }
                            catch (Exception e)
                            {
                                if (_clientRef.LoggingEnabled)
                                {
                                    _clientRef.Log(e.StackTrace);
                                }
                                exceptions.Add(e);
                            }
                        }

                        _failedAuthenticationAttempts = 0;

                        // now deal with rewards
                        if (_rewardCallback != null && !string.IsNullOrWhiteSpace(responseData))
                        {
                            try
                            {
                                Dictionary<string, object> rewards = null;

                                // it's an operation that return a reward
                                if (operation == ServiceOperation.Authenticate)
                                {
                                    if (JsonParser.GetString(responseData, "rewards") is string outerRewards && !string.IsNullOrWhiteSpace(outerRewards))
                                    {
                                        if (JsonParser.GetString(outerRewards, "rewards") is string innerRewards && !string.IsNullOrWhiteSpace(innerRewards))
                                        {
                                            if (innerRewards.Length > 5) // Minimum a Json string can be
                                            {
                                                // we found rewards
                                                rewards = JsonReader.Deserialize<Dictionary<string, object>>(outerRewards);
                                            }
                                        }
                                    }
                                }
                                else if (operation == ServiceOperation.Update ||
                                         operation == ServiceOperation.Trigger ||
                                         operation == ServiceOperation.TriggerMultiple)
                                {
                                    if (JsonParser.GetString(responseData, "rewards") is string innerRewards && !string.IsNullOrWhiteSpace(innerRewards))
                                    {
                                        if (innerRewards.Length > 5) // Minimum a Json string can be
                                        {
                                            // we found rewards
                                            rewards = JsonReader.Deserialize<Dictionary<string, object>>(responseData);
                                        }
                                    }
                                }

                                if (rewards != null)
                                {
                                    var theReward = new Dictionary<string, object>
                                    {
                                        ["rewards"] = rewards,
                                        ["service"] = service,
                                        ["operation"] = operation
                                    };

                                    var apiRewards = new Dictionary<string, object>
                                    {
                                        ["apiRewards"] = new List<object> { theReward }
                                    };

                                    _rewardCallback(_clientRef.SerializeJson(apiRewards));
                                }
                            }
                            catch (Exception e)
                            {
                                if (_clientRef.LoggingEnabled)
                                {
                                    _clientRef.Log(e.StackTrace);
                                }
                                exceptions.Add(e);
                            }
                        }
                    }
                }
                else // If non-200
                {
                    int reasonCode = 0;
                    string errorJson = "";
                    callback = sc.GetCallback();
                    operation = sc.GetOperation();

                    //if it was an authentication call 
                    if (operation == ServiceOperation.Authenticate)
                    {
                        //if we haven't already gone above the threshold and are waiting for the timer or a 200 response to reset things
                        if (!tooManyAuthenticationAttempts())
                        {
                            _failedAuthenticationAttempts++;
                            if (tooManyAuthenticationAttempts())
                            {
                                ResetAuthenticationTimer();
                            }
                        }

                        _authInProgress = false;
                    }

                    if (JsonParser.GetValue<int>(response, "reason_code") is int reasonCodeVal && reasonCodeVal != default)
                    {
                        reasonCode = reasonCodeVal;
                    }

                    if (_oldStyleStatusResponseInErrorCallback)
                    {
                        if (JsonParser.GetString(response, "status_message") is string statusMessage && !string.IsNullOrWhiteSpace(statusMessage))
                        {
                            errorJson = statusMessage;
                        }
                    }
                    else
                    {
                        errorJson = response;
                    }

                    // If the authenticated session has expired, and long session is enabled, attempt to re-authenticate and retry lost call(s)
                    if (reasonCode == ReasonCodes.PLAYER_SESSION_EXPIRED && AutoReconnectEnabled && operation != ServiceOperation.Authenticate && _isAuthenticated)
                    {
                        // Save the call that failed
                        ServerCall expiredServerCall = sc;
                        var otherServiceCallsInProgress = new List<ServerCall>(_serviceCallsInProgress);
                        _serviceCallsInProgress.Clear();
                        _clientRef.Log("Session expired. Attempting reconnect . . .");
                        _packetId = 0;

                        // Retry failed/missed call(s) on successful re-authentication
                        SuccessCallback successCallback = (response2, cbObject) =>
                        {
                            _clientRef.Log(string.Format("Success | {0}", response2));

                            if (expiredServerCall != null)
                            {
                                // Re-queue the call that failed...
                                _serviceCallsWaiting.Add(expiredServerCall);

                                // ...and any other calls in the bundle as they will fail too
                                _serviceCallsWaiting.AddRange(otherServiceCallsInProgress);
                            }
                            
                            if(_autoReconnectCallback != null)
                            {
                                _autoReconnectCallback(response2);
                            }

                            // Next Update loop will handle the re-authenticate request/response
                            return;
                        };

                        FailureCallback failureCallback = (status, code, error, cbObject) =>
                        {
                            _clientRef.Log(string.Format("Long session re-authentication failed. | {0}  {1}  {2}", status, code, error));

                            AutoReconnectEnabled = false;
                            
                            if(_autoReconnectCallback != null)
                            {
                                _autoReconnectCallback(error);
                            }

                            expiredServerCall?.GetCallback()?.OnErrorCallback(status, code, error);
                        };

                        // Attempt to re-authenticate
                        _clientRef.AuthenticationService.AuthenticateAnonymous(false, successCallback, failureCallback);

                        return;
                    }

                    if (reasonCode == ReasonCodes.PLAYER_SESSION_EXPIRED ||
                        reasonCode == ReasonCodes.NO_SESSION ||
                        reasonCode == ReasonCodes.PLAYER_SESSION_LOGGED_OUT)
                    {
                        _isAuthenticated = false;
                        SessionID = "";

                        if (_clientRef.LoggingEnabled)
                        {
                            _clientRef.Log("Received session expired or not found, need to re-authenticate");
                        }

                        // cache error if session related
                        _cachedStatusCode = statusCode;
                        _cachedReasonCode = reasonCode;

                        if (JsonParser.GetString(response, "status_message") is string statusMessage && !string.IsNullOrWhiteSpace(statusMessage))
                        {
                            _cachedStatusMessage = statusMessage;
                        }
                    }

                    if (operation == ServiceOperation.Logout)
                    {
                        if (reasonCode == ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT)
                        {
                            _isAuthenticated = false;
                            SessionID = "";
                            if (_clientRef.LoggingEnabled)
                            {
                                _clientRef.Log("Could not communicate with the server on logout due to network timeout");
                            }
                        }
                    }

                    // now try to execute the callback
                    if (callback != null)
                    {
                        try
                        {
                            callback.OnErrorCallback(statusCode, reasonCode, errorJson);
                        }
                        catch (Exception e)
                        {
                            if (_clientRef.LoggingEnabled)
                            {
                                _clientRef.Log(e.StackTrace);
                            }
                            exceptions.Add(e);
                        }
                    }

                    if (_globalErrorCallback != null)
                    {
                        object cbObject = null;
                        if (callback != null)
                        {
                            cbObject = callback.m_cbObject;
                            // if this is the internal BrainCloudWrapper callback object return the user-supplied
                            // callback object instead
                            if (cbObject != null && cbObject is WrapperAuthCallbackObject)
                            {
                                cbObject = ((WrapperAuthCallbackObject)cbObject)._cbObject;
                            }
                        }

                        _globalErrorCallback(statusCode, reasonCode, errorJson, cbObject);
                    }

                    UpdateKillSwitch(sc.Service, sc.Operation, statusCode);
                }
            }

            if (!string.IsNullOrWhiteSpace(bundleObj.events) && _eventCallback != null)
            {
                try
                {
                    _eventCallback(bundleObj.events);
                }
                catch (Exception e)
                {
                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log(e.StackTrace);
                    }
                    exceptions.Add(e);
                }
            }

            if (exceptions.Count > 0)
            {
                DisposeUploadHandler();
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
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Client disabled due to repeated errors from a single API call: " + service + " | " + operation);
                }
            }

            // Authentication check for kill switch. Did the client make an authentication call?
            if (operation == ServiceOperation.Authenticate)
            {
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Failed Authentication Call");
                }

                string num;
                num = _failedAuthenticationAttempts.ToString();
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("Current number of failed authentications: " + num);
                }

                //have the attempts gone beyond the threshold?
                if (tooManyAuthenticationAttempts())
                {
                    //we have a problem now, it seems they are contiuously trying to authenticate and sending us too many errors.
                    //we are going to now engage the killswitch and disable the client. This will act differently however. client will not
                    //be able to send an authentication request for a time. 
                    if (_clientRef.LoggingEnabled)
                    {
                        _clientRef.Log("Too many repeat authentication failures");
                    }
                    _killSwitchEngaged = true;
                    ResetAuthenticationTimer();
                }
            }
        }

        private void ResetKillSwitch()
        {
            _killSwitchErrorCount = 0;
            _killSwitchService = null;
            _killSwitchOperation = null;

            //reset the amount of failed attempts upon a successful attempt
            _failedAuthenticationAttempts = 0;
            _recentResponseJsonData[0] = blankResponseData;
            _recentResponseJsonData[1] = blankResponseData;
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
                        // put auth first
                        ServerCall call = null;
                        int numMessagesWaiting = _serviceCallsWaiting.Count;
                        for (int i = 0; i < _serviceCallsWaiting.Count; ++i)
                        {
                            call = _serviceCallsWaiting[i];
                            if (call.GetType() == typeof(EndOfBundleMarker))
                            {
                                // if the first message is marker, just throw it away
                                if (i == 0)
                                {
                                    _serviceCallsWaiting.RemoveAt(0);
                                    --i;
                                    --numMessagesWaiting;
                                    continue;
                                }
                                else // otherwise cut off the bundle at the marker and toss marker away
                                {
                                    numMessagesWaiting = i;
                                    _serviceCallsWaiting.RemoveAt(i);
                                    break;
                                }
                            }

                            if (call.GetOperation() == ServiceOperation.Authenticate)
                            {
                                if (i != 0)
                                {
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

                        if (numMessagesWaiting <= 0)
                        {
                            return null;
                        }

                        if (_serviceCallsInProgress.Count > 0)
                        {
                            // this should never happen
                            if (_clientRef.LoggingEnabled)
                            {
                                _clientRef.Log("ERROR - in progress queue is not empty but we're ready for the next message!");
                            }
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
                    List<object> messageList = new();
                    bool isAuth = false;

                    ServerCall scIndex;
                    string operation = "";
                    string service = "";
                    for (int i = 0; i < _serviceCallsInProgress.Count; ++i)
                    {
                        scIndex = _serviceCallsInProgress[i];
                        operation = scIndex.GetOperation();
                        service = scIndex.GetService();

                        // don't send heartbeat if it was generated by comms (null callbacks)
                        // and there are other messages in the bundle - it's unnecessary
                        if (service.Equals(ServiceName.HeartBeat)
                            && operation.Equals(ServiceOperation.Read)
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
                        message[OperationParam.ServiceMessageService] = scIndex.Service;
                        message[OperationParam.ServiceMessageOperation] = scIndex.Operation;
                        message[OperationParam.ServiceMessageData] = scIndex.GetJsonData();

                        messageList.Add(message);

                        if (operation.Equals(ServiceOperation.Authenticate))
                        {
                            requestState.PacketNoRetry = true;
                        }

                        if (operation.Equals(ServiceOperation.Authenticate) ||
                            operation.Equals(ServiceOperation.ResetEmailPassword) ||
                            operation.Equals(ServiceOperation.ResetEmailPasswordAdvanced) ||
                            operation.Equals(ServiceOperation.ResetUniversalIdPassword) ||
                            operation.Equals(ServiceOperation.ResetUniversalIdPasswordAdvanced) ||
                            operation.Equals(ServiceOperation.GetServerVersion))
                        {
                            isAuth = true;
                        }

                        if (operation.Equals(ServiceOperation.FullReset) ||
                            operation.Equals(ServiceOperation.Logout))
                        {
                            requestState.PacketRequiresLongTimeout = true;
                        }
                    }

                    requestState.PacketId = _packetId;
                    _expectedIncomingPacketId = _packetId;
                    requestState.MessageList = messageList;
                    ++_packetId;

                    if (!_killSwitchEngaged && !tooManyAuthenticationAttempts())
                    {
                        if (_isAuthenticated || isAuth)
                        {
                            if (_clientRef.LoggingEnabled)
                            {
                                _clientRef.Log("SENDING REQUEST");
                            }
                            InternalSendMessage(requestState);
                        }
                        else
                        {
                            FakeErrorResponse(requestState, _cachedStatusCode, _cachedReasonCode, _cachedStatusMessage);
                            requestState = null;
                        }
                    }
                    else
                    {
                        if (tooManyAuthenticationAttempts())
                        {
                            FakeErrorResponse(requestState, StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_DISABLED_FAILED_AUTH,
                                "Client has been disabled due to identical repeat Authentication calls that are throwing errors. Authenticating with the same credentials is disabled for 30 seconds");
                            requestState = null;
                        }
                        else
                        {
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
            packet[OperationParam.ServiceMessagePacketId] = requestState.PacketId;
            packet[OperationParam.ServiceMessageSessionId] = SessionID;
            if (AppId != null && AppId.Length > 0)
            {
                packet[OperationParam.ServiceMessageGameId] = AppId;
            }
            packet[OperationParam.ServiceMessageMessages] = requestState.MessageList;

            string jsonRequestString = SerializeJson(packet);

            if (_clientRef.LoggingEnabled)
            {
                _clientRef.Log(string.Format("{0} - {1}\n{2}", "REQUEST" + (requestState.Retries > 0 ? " Retry(" + requestState.Retries + ")" : ""), DateTime.Now, jsonRequestString));
            }

            ResetIdleTimer();

            TriggerCommsError(statusCode, reasonCode, statusMessage);
            DisposeUploadHandler();
            _activeRequest = null;
        }

        internal string SerializeJson(object payload)
        {
            //Unity doesn't like when we create a new StringBuilder outside of this method.
            _stringBuilderOutput = new StringBuilder();
            using (JsonWriter writer = new JsonWriter(_stringBuilderOutput, _writerSettings))
            {
                try
                {
                    writer.Write(payload);
                }
                catch (JsonSerializationException exception)
                {
                    // Contains will fail if one input is off, so I had to break it up like this for more consistency
                    // IE: The maxiumum depth of 24 was exceeded. Check for cycles in object graph.
                    if (exception.Message.Contains("The maxiumum depth") &&
                        exception.Message.Contains("exceeded"))
                    {
                        lock (_serviceCallsInProgress)
                        {
                            if (_serviceCallsInProgress.Count > 0)
                            {
                                for (int i = _serviceCallsInProgress.Count - 1; i >= 0; --i)
                                {
                                    var serviceCall = _serviceCallsInProgress[i];
                                    if (serviceCall?.GetCallback() != null)
                                    {
                                        serviceCall.GetCallback().OnErrorCallback(900, ReasonCodes.JSON_REQUEST_MAXDEPTH_EXCEEDS_LIMIT, JSON_ERROR_MESSAGE);
                                        _serviceCallsInProgress.RemoveAt(i);
                                    }
                                    else
                                    {
                                        _clientRef.Log("JSON Exception: " + JSON_ERROR_MESSAGE, true);
                                    }
                                }
                            }
                            else
                            {
                                _clientRef.Log("JSON Exception: " + JSON_ERROR_MESSAGE, true);
                            }
                        }
                    }

                    _clientRef.Log("JSON Exception: " + exception.Message, true);
                }
            }

            return _stringBuilderOutput.ToString();
        }

        internal Dictionary<string, object> DeserializeJson(string jsonData)
        {
            if (JsonParser.TryGetString(jsonData, out _, "packetId"))
            {
                JsonResponseBundleV2 responseBundle = DeserializeJsonBundle(jsonData);
                if (responseBundle.responses == null || responseBundle.responses.Length == 0)
                {
                    return null;
                }

                return JsonReader.Deserialize<Dictionary<string, object>>(responseBundle.responses[0]);
            }

            return JsonReader.Deserialize<Dictionary<string, object>>(jsonData);
        }

        /// <summary>
        /// Method creates the web request and sends it immediately.
        /// Relies upon the requestState PacketId and MessageList being
        /// set appropriately.
        /// </summary>
        /// <param name="requestState">Request state.</param>
        private void InternalSendMessage(RequestState requestState)
        {
#if DOT_NET || GODOT
            // During retry, the RequestState is reused so we have to make sure its state goes back to PENDING.
            // Unity uses the info stored in the WWW object and it's recreated here so it's not an issue.
            requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_PENDING;
#endif

            // bundle up the data into a string
            Dictionary<string, object> packet = new Dictionary<string, object>();
            packet[OperationParam.ServiceMessagePacketId] = requestState.PacketId;
            packet[OperationParam.ServiceMessageSessionId] = SessionID;
            if (AppId != null && AppId.Length > 0)
            {
                packet[OperationParam.ServiceMessageGameId] = AppId;
            }
            packet[OperationParam.ServiceMessageMessages] = requestState.MessageList;

            string jsonRequestString = SerializeJson(packet);
            string sig = CalculateMD5Hash(jsonRequestString + SecretKey);

            byte[] byteArray = Encoding.UTF8.GetBytes(jsonRequestString);

            requestState.Signature = sig;

            bool compressMessage = SupportsCompression &&                               // compression enabled
                                   ClientSideCompressionThreshold >= 0 &&               // server says we can compress
                                   byteArray.Length >= ClientSideCompressionThreshold;  // and byte array is greater or equal to the threshold

            //if the packet we're sending is larger than the size before compressing, then we want to compress it otherwise we're good to send it. AND we have to support compression
            if (compressMessage)
            {
                byteArray = Compress(byteArray);
            }

            requestState.ByteArray = byteArray;

            /*
            if (_debugPacketLossRate > 0.0)
            {
                System.Random r = new System.Random();
                requestState.LoseThisPacket = r.NextDouble() > _debugPacketLossRate;
            }
            */

            // if (!requestState.LoseThisPacket)
            {
#if !(DOT_NET || GODOT)
                Dictionary<string, string> formTable = new Dictionary<string, string>();
#if USE_WEB_REQUEST
                UnityWebRequest request = UnityWebRequest.Post(ServerURL, formTable);
                request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
                request.SetRequestHeader("X-SIG", sig);

                if (AppId != null && AppId.Length > 0)
                {
                    request.SetRequestHeader("X-APPID", AppId);
                }

                if (compressMessage)
                {
#if DOT_NET || GODOT
                    request.SetRequestHeader("Accept-Encoding", "gzip");
#endif
                    request.SetRequestHeader("Content-Encoding", "gzip");
                }

                request.uploadHandler = new UploadHandlerRaw(byteArray);
                request.SendWebRequest();
#else
                formTable["Content-Type"] = "application/json; charset=utf-8";
                formTable["X-SIG"] = sig;
                if (AppId != null && AppId.Length > 0)
                {
                    formTable["X-APPID"] = AppId;
                }

                if (compressMessage)
                {
                    formTable["Accept-Encoding"] = "gzip";
                    formTable["Content-Encoding"] = "gzip";
                }

                WWW request = new WWW(ServerURL, byteArray, formTable);
#endif
                requestState.WebRequest = request;
#else

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, new Uri(ServerURL));

                req.Content = new ByteArrayContent(byteArray);

                if (compressMessage)
                {
                    req.Headers.Add("Accept-Encoding", "gzip");
                    req.Content.Headers.Add("Content-Encoding", "gzip");
                }

                req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
                {
                    CharSet = Encoding.UTF8.WebName
                };

                req.Headers.Add("X-SIG", sig);
                if (AppId != null && AppId.Length > 0)
                {
                    req.Headers.Add("X-APPID", AppId);
                }

                req.Method = HttpMethod.Post;

                CancellationTokenSource source = new CancellationTokenSource();
                requestState.CancelToken = source;

                requestState.RequestString = jsonRequestString;
                requestState.TimeSent = DateTime.Now;
                ResetIdleTimer();
                TimeSpan packetTimeout = GetPacketTimeout(requestState);
                //_ is a discard feature for C# however the request will still await.
                _ = InternalSendMessageAsync(req, requestState, packetTimeout);
#endif
                requestState.RequestString = jsonRequestString;
                requestState.TimeSent = DateTime.Now;

                ResetIdleTimer();

                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log(string.Format("{0} - {1}\n{2}", "REQUEST" + (requestState.Retries > 0 ? " Retry(" + requestState.Retries + ")" : ""), DateTime.Now, jsonRequestString));
                }
            }
        }

        private byte[] Compress(byte[] raw)
        {
            var outputStream = new MemoryStream();
            using (var stream = new GZipStream(outputStream, CompressionMode.Compress, true))
            {
                stream.Write(raw, 0, raw.Length);
            }
            return outputStream.ToArray();
        }

        private byte[] Decompress(byte[] compressedBytes)
        {
            using (var inputStream = new MemoryStream(compressedBytes))
            using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new MemoryStream())
            {
                gZipStream.CopyTo(outputStream);
                outputStream.Read(compressedBytes, 0, compressedBytes.Length);
                return outputStream.ToArray();
            }
        }

        private bool IsGzip(byte[] data)
        {
            if (data == null || data.Length < 2) return false;
            return data[0] == 0x1F && data[1] == 0x8B;
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
#if USE_WEB_REQUEST
            if (!string.IsNullOrWhiteSpace(_activeRequest.WebRequest.error))
            {
                status = RequestState.eWebRequestStatus.STATUS_ERROR;
            }

            else if (_activeRequest.WebRequest.downloadHandler.isDone)
            {
                status = RequestState.eWebRequestStatus.STATUS_DONE;
            }
            else if (_activeRequest.WebRequest.isDone)
            {
                status = RequestState.eWebRequestStatus.STATUS_DONE;
            }
#elif DOT_NET || GODOT
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
#if USE_WEB_REQUEST
#if UNITY_2018 || UNITY_2019
            if (_activeRequest.WebRequest.isNetworkError)
            {
                Debug.LogWarning("Failed to communicate with the server. For example, the request couldn't connect or it could not establish a secure channel");
            }
            else if (_activeRequest.WebRequest.isHttpError)
            {
                Debug.LogWarning("Something went wrong, received a isHttpError flag. Examples for this to happen are: failure to resolve a DNS entry, a socket error or a redirect limit being exceeded. When this property returns true, the error property will contain a human-readable string describing the error.");
            }
#elif UNITY_2020_1_OR_NEWER
            if (_activeRequest.WebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning("Failed to communicate with the server. For example, the request couldn't connect or it could not establish a secure channel");
            }
            else if (_activeRequest.WebRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogWarning("The server returned an error response. The request succeeded in communicating with the server, but received an error as defined by the connection protocol.");
            }
            else if (_activeRequest.WebRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogWarning("Error processing data. The request succeeded in communicating with the server, but encountered an error when processing the received data. For example, the data was corrupted or not in the correct format.");
            }
#endif
            if (!string.IsNullOrWhiteSpace(_activeRequest.WebRequest.error))
            {
                response = _activeRequest.WebRequest.error;
            }

            response = _activeRequest.WebRequest.downloadHandler.text;

            if (response.Contains("Security violation 47") ||
                response.StartsWith("<"))
            {
                Debug.LogWarning("Please re-select app in brainCloud settings, something went wrong");
            }

#elif DOT_NET || GODOT
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
                if (DateTime.Now.Subtract(requestState.TimeSent) > TimeSpan.FromSeconds(_authPacketTimeoutSecs))
                {
                    for (int i = 0; i < _listAuthPacketTimeouts.Length; i++)
                    {
                        if (_listAuthPacketTimeouts[i] == _authPacketTimeoutSecs)
                        {
                            if (i + 1 < _listAuthPacketTimeouts.Length)
                            {
                                _authPacketTimeoutSecs = _listAuthPacketTimeouts[i + 1];
                                break;
                            }
                        }
                    }
                }
                return TimeSpan.FromSeconds(_authPacketTimeoutSecs);
            }

            int currentRetry = requestState.Retries;
            TimeSpan ret;

            // if this is a delete player, or logout we change the timeout behaviour
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
        /// Checks if json is valid then returns json object
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        private JsonResponseBundleV2 DeserializeJsonBundle(string jsonData)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                if (_clientRef.LoggingEnabled)
                {
                    _clientRef.Log("ERROR - Incoming packet data was null or empty! This is probably a network issue.");
                }

                return JsonResponseBundleV2.CreateEmpty();
            }

            jsonData = jsonData.Trim();
            if ((jsonData.StartsWith("{") && jsonData.EndsWith("}")) || // For object
                (jsonData.StartsWith("[") && jsonData.EndsWith("]"))) // For array
            {
                try
                {
                    return new JsonResponseBundleV2(jsonData);
                }
                catch (Exception ex)
                {
                    ResendMessage(_activeRequest);

                    _clientRef.Log(ex.Message);

                    return JsonResponseBundleV2.CreateEmpty();
                }
            }

            return JsonResponseBundleV2.CreateEmpty();
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
                DisposeUploadHandler();
                _activeRequest = null;
                _clientRef.AuthenticationService.ProfileId = "";
                SessionID = "";
                _packetId = 0;
            }
        }

        private string CalculateMD5Hash(string input)
        {
#if !(DOT_NET || GODOT)
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

            StringBuilder sb = new();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Handles authenticate-specific data from successful request
        /// </summary>
        private void ProcessAuthenticate(string jsonData)
        {
            // We want to extract the compressIfLarger amount
            ClientSideCompressionThreshold = jsonData.Contains("compressIfLarger") ? JsonParser.GetValue<int>(jsonData, "compressIfLarger")
                                                                                   : ClientSideCompressionThreshold;

            _idleTimeout = TimeSpan.FromSeconds((long)((JsonParser.GetValue<long>(jsonData, OperationParam.AuthenticateServicePlayerSessionExpiry)
                           is long expiry && expiry > 0 ? expiry : _defaultPlayerSessionExpiry) * _idleTimeoutModifier));

            if (JsonParser.GetValue<int>(jsonData, "maxBundleMsgs") is int maxBundleMsgs && maxBundleMsgs > 0)
            {
                _maxBundleMessages = maxBundleMsgs;
            }

            if (JsonParser.GetValue<int>(jsonData, "maxKillCount") is int maxKillCount && maxKillCount > 0)
            {
                _killSwitchThreshold = maxKillCount;
            }

            ResetErrorCache();
            _isAuthenticated = true;
        }

        /// <summary>
        /// Switches AppId to one provided from Json.
        /// </summary>
        private void ProcessSwitchResponse(string jsonData)
        {
            if (JsonParser.GetString(jsonData, "switchToAppId") is string switchToAppId && !string.IsNullOrWhiteSpace(switchToAppId))
            {
                AppId = switchToAppId;
            }
        }

        /// <summary>
        /// Attempts to create and send next request bundle.
        /// If to many attempts have been made, the request becomes an error
        /// </summary>
        /// <param name="status">Current Request Status</param>
        /// <param name="bypassTimeout">Was there an error on the request?</param>
        private void RetryRequest(RequestState.eWebRequestStatus status, bool bypassTimeout)
        {
            if (_activeRequest != null)
            {
                if (bypassTimeout || DateTime.Now.Subtract(_activeRequest.TimeSent) >= GetPacketTimeout(_activeRequest))
                {
                    if (_clientRef.LoggingEnabled)
                    {
                        string errorResponse = "";
                        // we've reached the retry limit - send timeout error to all client callbacks
                        if (status == RequestState.eWebRequestStatus.STATUS_ERROR)
                        {
                            errorResponse = GetWebRequestResponse(_activeRequest);
                            if (!string.IsNullOrWhiteSpace(errorResponse))
                            {
                                _clientRef.Log("Timeout with network error: " + errorResponse);
                            }
                            else
                            {
                                _clientRef.Log("Timeout with network error: Please check the URL and/or certificates for server");
                            }
                        }
                        else
                        {
                            _clientRef.Log("Timeout no reply from server");
                        }
                    }
                    if (!ResendMessage(_activeRequest))
                    {
                        DisposeUploadHandler();
                        _activeRequest = null;

                        // if we're doing caching of messages on timeout, kick it in now!
                        if (_cacheMessagesOnNetworkError && _networkErrorCallback != null)
                        {
                            if (_clientRef.LoggingEnabled)
                            {
                                _clientRef.Log("Caching messages");
                            }
                            _blockingQueue = true;

                            // and insert the inProgress messages into head of wait queue
                            lock (_serviceCallsInTimeoutQueue)
                            {
                                _serviceCallsInTimeoutQueue.InsertRange(0, _serviceCallsInProgress);
                                _serviceCallsInProgress.Clear();
                            }

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

        private void DisposeUploadHandler()
        {
#if USE_WEB_REQUEST
            if (_activeRequest != null &&
                _activeRequest.WebRequest != null &&
                _activeRequest.WebRequest.uploadHandler != null)
            {
                _activeRequest.WebRequest.Dispose();
            }
#endif
        }

        public void AddCallbackToAuthenticateRequest(ServerCallback in_callback)
        {
            bool inProgress = false;
            for (int i = 0; i < _serviceCallsInProgress.Count && !inProgress; ++i)
            {
                if (_serviceCallsInProgress[i].Operation == ServiceOperation.Authenticate)
                {
                    inProgress = true;
                    _serviceCallsInProgress[i].GetCallback().AddAuthCallbacks(in_callback);
                }
            }
        }

        public bool IsAuthenticateRequestInProgress()
        {
            bool inProgress = false;
            for (int i = 0; i < _serviceCallsInProgress.Count && !inProgress; ++i)
            {
                if (_serviceCallsInProgress[i].Operation == ServiceOperation.Authenticate)
                {
                    inProgress = true;
                }
            }
            return inProgress;
        }
    
#if (DOT_NET || GODOT)
        private async Task<HttpResult> SendAsync(HttpRequestMessage request, TimeSpan timeout, CancellationToken externalToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var linkedCts  = CancellationTokenSource.CreateLinkedTokenSource(
                timeoutCts.Token, externalToken);

            try
            {
                HttpResponseMessage response = await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseContentRead,
                    linkedCts.Token).ConfigureAwait(false);

                HttpContent content = response.Content;
                string responseString;

                if (content.Headers.ContentEncoding.ToString() != "gzip")
                {
                    responseString = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
                else
                {
                    var byteArray           = await content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    var decompressedByteArray = IsGzip(byteArray) ? Decompress(byteArray) : byteArray;
                    responseString          = Encoding.UTF8.GetString(decompressedByteArray, 0, decompressedByteArray.Length);
                }

                if (!response.IsSuccessStatusCode)
                    return HttpResult.HttpError(response.StatusCode, responseString);

                return HttpResult.Success(responseString, response.StatusCode);
            }
            catch (TaskCanceledException)
            {
                return timeoutCts.IsCancellationRequested
                    ? HttpResult.Timeout()
                    : HttpResult.Cancelled();
            }
            catch (HttpRequestException ex)
            {
                return HttpResult.NetworkError(ex.Message);
            }
            catch (Exception ex)
            {
                return HttpResult.UnknownError(ex.Message);
            }
        }

        private void ProcessHttpResult(HttpResult result, RequestState requestState)
        {
            if (result.IsSuccess)
            {
                ResetIdleTimer();
                HandleResponseBundle(result.Content);
                _activeRequest = null;
                return;
            }

            switch (result.FailureType)
            {
                case HttpFailureType.Timeout:
                    if (_clientRef.LoggingEnabled)
                        _clientRef.Log("Request timed out (client-side timeout).");
                    requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
                    break;

                case HttpFailureType.Cancelled:
                    if (_clientRef.LoggingEnabled)
                        _clientRef.Log("Request was cancelled.");
                    requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
                    break;

                case HttpFailureType.NetworkError:
                    if (_clientRef.LoggingEnabled)
                        _clientRef.Log("Network error: " + result.ErrorMessage);
                    requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
                    break;

                case HttpFailureType.HttpError:
                    int statusCode = result.StatusCode.HasValue ? (int)result.StatusCode.Value : 0;

                    if (statusCode == 503 || statusCode == 502 || statusCode == 504)
                    {
                        if (_clientRef.LoggingEnabled)
                            _clientRef.Log("Server temporarily unavailable, retrying...");
                        requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_PENDING;
                        return;
                    }

                    if (_serviceCallsInProgress.Count > 0)
                    {
                        ServerCallback sc = _serviceCallsInProgress[0].GetCallback();
                        if (sc != null)
                            sc.OnErrorCallback(404, statusCode, result.Content);
                    }
                    requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
                    break;

                case HttpFailureType.Unknown:
                default:
                    if (_clientRef.LoggingEnabled)
                        _clientRef.Log("Unknown error: " + result.ErrorMessage);
                    requestState.DotNetRequestStatus = RequestState.eWebRequestStatus.STATUS_ERROR;
                    break;
            }
        }

        private async Task InternalSendMessageAsync(HttpRequestMessage req, RequestState requestState, TimeSpan timeout)
        {
            HttpResult result = await SendAsync(req, timeout);
            ProcessHttpResult(result, requestState);
        }
#endif
    }

    #region HttpResult

#if DOT_NET || GODOT
    public enum HttpFailureType
    {
        Timeout,
        NetworkError,
        HttpError,
        Cancelled,
        Unknown
    }

    internal class HttpResult
    {
        public bool             IsSuccess    { get; }
        public HttpStatusCode?  StatusCode   { get; }
        public string           Content      { get; }
        public HttpFailureType? FailureType  { get; }
        public string           ErrorMessage { get; }

        // When an HTTP status code is present
        private HttpResult(bool isSuccess, HttpStatusCode statusCode, string content,
                           HttpFailureType? failureType, string errorMessage)
        {
            IsSuccess    = isSuccess;
            StatusCode   = statusCode;
            Content      = content;
            FailureType  = failureType;
            ErrorMessage = errorMessage;
        }

        // When there is no HTTP status code (transport-level failures)
        private HttpResult(bool isSuccess, string content,
                           HttpFailureType? failureType, string errorMessage)
        {
            IsSuccess    = isSuccess;
            StatusCode   = null;
            Content      = content;
            FailureType  = failureType;
            ErrorMessage = errorMessage;
        }

        public static HttpResult Success(string content, HttpStatusCode code)
            => new HttpResult(true, code, content, null, null);

        public static HttpResult HttpError(HttpStatusCode code, string content)
            => new HttpResult(false, code, content, HttpFailureType.HttpError, null);

        public static HttpResult Timeout()
            => new HttpResult(false, null, HttpFailureType.Timeout, null);

        public static HttpResult NetworkError(string message)
            => new HttpResult(false, null, HttpFailureType.NetworkError, message);

        public static HttpResult Cancelled()
            => new HttpResult(false, null, HttpFailureType.Cancelled, null);

        public static HttpResult UnknownError(string message)
            => new HttpResult(false, null, HttpFailureType.Unknown, message);
    }
#endif

    #endregion

    #region brainCloud JsonResponseBundleV2

    internal readonly struct JsonResponseBundleV2
    {
        public const int NO_PACKET_EXPECTED = -1; // The Id of _expectedIncomingPacketId when no packet expected

        public const int EMPTY_RESPONSE_BUNDLE = int.MinValue; // The Id we use when we want to denote an "empty" struct

        public readonly long packetId;
        public readonly string events;
        public readonly string[] responses;

        public bool IsError => packetId == NO_PACKET_EXPECTED;

        public bool IsEmpty => packetId == EMPTY_RESPONSE_BUNDLE;

        public static JsonResponseBundleV2 CreateEmpty() => new(EMPTY_RESPONSE_BUNDLE);

        public static string GetErrorJson(int status, int reason_code, string status_message)
            => JsonParser.GetJsonErrorMessage(status, reason_code, status_message);

        internal JsonResponseBundleV2(long id)
        {
            packetId = id;
            events = string.Empty;
            responses = null;
        }

        internal JsonResponseBundleV2(string jsonData)
        {
            JsonParser.GetJsonResponseBundleV2(jsonData, out string packetId, out string events, out string[] responses);

            this.packetId = long.TryParse(packetId, out long result) ? result : NO_PACKET_EXPECTED;
            if (this.packetId < 0)
            {
                throw new Exception($"packetId is not a valid value! packetId: {this.packetId}");
            }

            this.events = !string.IsNullOrWhiteSpace(events) ? $"{{\"events\":[{events}]}}" : string.Empty;
            this.responses = responses != null && responses.Length > 0 ? responses : null;
        }
    }

    #endregion
}

/*
 * Extending JsonParser here just for JsonResponseBundleV2 handling.
 */
namespace BrainCloud.Common
{
    using System;

    public static partial class JsonParser
    {
        internal static void GetJsonResponseBundleV2(string jsonData, out string packetId,
                                                                      out string events,
                                                                      out string[] responses)
        {
            packetId = string.Empty;
            events = string.Empty;
            responses = null;

            char current;
            bool insideProperty = false;
            bool splitToResponses = false;
            bool skipValue = false;

            sbHelper.Clear();

            for (int i = 1; i < jsonData.Length; i++)
            {
                current = jsonData[i];

                if (current == '\"' && jsonData[i - 1] != '\\')
                {
                    insideProperty = !insideProperty;
                    if (insideProperty)
                    {
                        sbHelper.Clear();
                    }
                }
                else if (insideProperty)
                {
                    sbHelper.Append(current);
                }
                else if (!insideProperty && sbHelper.Length > 0)
                {
                    if (skipValue)
                    {
                        skipValue = false; // This was the string value of an unknown key; discard it
                    }
                    else
                    {
                        switch (sbHelper.ToString())
                        {
                            case "packetId":
                                sbHelper.Clear();
                                while (jsonData[i + 1] != ',')
                                {
                                    current = jsonData[++i];
                                    sbHelper.Append(current);
                                }
                                packetId = sbHelper.ToString().Trim();
                                break;
                            case "responses":
                                splitToResponses = true;
                                splitArrays.Clear();
                                break;
                            case "events":
                                splitToResponses = false;
                                break;
                            default: // Unknown key
                                // i is at ':', check the value type to decide how to skip
                                current = jsonData[i + 1];
                                if (current == '"' || current == '{' || current == '[')
                                {
                                    skipValue = true; // String/object/array; skipped below
                                }
                                else // Scalar; skip now
                                {
                                    while (i + 1 < jsonData.Length && current != ',' && current != '}')
                                    {
                                        i++;
                                        current = jsonData[i + 1];
                                    }
                                }
                                break;
                        }
                    }

                    sbHelper.Clear();
                }
                else if (current == '{' || current == '[')
                {
                    int level = 1, start = i + 1;

                    while (level > 0)
                    {
                        current = jsonData[++i];

                        switch (current)
                        {
                            case '{':
                            case '[':
                                level++;
                                goto default;
                            case '}':
                            case ']':
                                level--;
                                if (level == 0 && splitToResponses) // Last element
                                {
                                    int len = i - start;
                                    if (len > 0)
                                    {
                                        splitArrays.Add(jsonData.Substring(start, len).Trim());
                                    }
                                }
                                goto default;
                            case ',':
                                if (level == 1 && splitToResponses)
                                {
                                    int len = i - start;
                                    splitArrays.Add(jsonData.Substring(start, len).Trim());
                                    start = i + 1;
                                    continue;
                                }
                                goto default;
                            case '"':
                                if (jsonData[i - 1] != '\\')
                                {
                                    while (i < jsonData.Length)
                                    {
                                        current = jsonData[i];
                                        sbHelper.Append(current);

                                        if (jsonData[++i] == '"' && current != '\\')
                                        {
                                            current = jsonData[i];
                                            goto default;
                                        }
                                    }

                                    throw new Exception("JsonParser could not parse this property's value!");
                                }
                                goto default;
                            default:
                                if (level != 0)
                                {
                                    sbHelper.Append(current);
                                }
                                continue;
                        }
                    }

                    if (skipValue)
                    {
                        skipValue = false; // Unknown key; discard it
                    }
                    else if (splitToResponses)
                    {
                        responses = splitArrays.Count > 0 ? splitArrays.ToArray() : null;
                    }
                    else
                    {
                        events = sbHelper.ToString();
                    }

                    sbHelper.Clear();
                }
            }
        }

        internal static string GetJsonResponseErrorBundleV2(long packetId, string[] responses)
        {
            sbHelper.Clear();

            for (int i = 0; i < responses.Length;)
            {
                sbHelper.Append(responses[i]);
                if (++i < responses.Length)
                {
                    sbHelper.Append(',');
                }
            }

            return $"{{\"packetId\":{packetId},\"responses\":[{sbHelper}]}}";
        }

        internal static string GetJsonErrorMessage(int status, int reason_code, string status_message)
        {
            return $"{{\"status\":{status},\"reason_code\":{reason_code},\"status_message\":\"{status_message}\",\"severity\":\"ERROR\"}}";
        }
    }
}
