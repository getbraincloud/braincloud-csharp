//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.Common;
#if !XAMARIN
using BrainCloud.Entity;
using System;
#endif

#if !(DOT_NET)
using UnityEngine;
using UnityEngine.Assertions;
#else
using System.Globalization;
using System;
#endif

namespace BrainCloud
{
    #region Enums
    public enum eBrainCloudUpdateType
    {
        ALL,
        REST,
        RTT,

        MAX
    }
    #endregion

    #region Delegates

    /// <summary>
    /// Success callback for an API method.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the server</param>
    /// <param name="cbObject">The user supplied callback object</param>
    public delegate void SuccessCallback(string jsonResponse, object cbObject);

    /// <summary>
    /// Failure callback for an API method.
    /// </summary>
    /// <param name="status">The HTTP status code</param>
    /// <param name="reasonCode">The error reason code</param>
    /// <param name="jsonError">The error JSON string</param>
    /// <param name="cbObject">The user supplied callback object</param>
    public delegate void FailureCallback(int status, int reasonCode, string jsonError, object cbObject);

    /// <summary>
    /// Network error callback.
    /// </summary>
    public delegate void NetworkErrorCallback();

    /// <summary>
    /// Log callback to implement if providing a custom logging function.
    /// </summary>
    public delegate void LogCallback(string log);

    /// <summary>
    /// Callback method invoked when brainCloud events are received.
    /// TODO: should we deprecate this? [smrj], view RTTEventCallback
    /// </summary>
    public delegate void EventCallback(string jsonResponse);

    /// <summary>
    /// Callback method invoked when brainCloud rewards are received.
    /// </summary>
    public delegate void RewardCallback(string jsonResponse);

    /// <summary>
    /// Success callback for an RTT response method.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the server</param>
    public delegate void RTTCallback(string jsonResponse);

    /// <summary>
    /// Method called when a file upload has completed.
    /// </summary>
    /// <param name="fileUploadId">The file upload id</param>
    public delegate void FileUploadSuccessCallback(string fileUploadId, string jsonResponse);

    /// <summary>
    /// Method called when a file upload has failed.
    /// </summary>
    /// <param name="fileUploadId">The file upload id</param>
    /// <param name="statusCode">The http status of the operation</param>
    /// <param name="reasonCode">The reason code of the operation</param>
    /// <param name="jsonResponse">The JSON response describing the failure. This uses the
    /// usual brainCloud error format similar to this:</param>
    public delegate void FileUploadFailedCallback(string fileUploadId, int statusCode, int reasonCode, string jsonResponse);
    #endregion

    public class BrainCloudClient
    {
        /// <summary>Enable the usage of the BrainCloudWrapper singleton.</summary>
        public static bool EnableSingletonMode = false;
        public const string SingletonUseErrorMessage =
            "Singleton usage is disabled. If called by mistake, use your own variable that holds an instance of the bcWrapper/bcClient.";


        #region Private Data

        private string s_defaultServerURL = "https://sharedprod.braincloudservers.com/dispatcherv2";
        private static BrainCloudClient s_instance;

        private string _appVersion = "";
        private Platform _platform;
        private string _languageCode;
        private string _countryCode;
        private bool _initialized;
        private bool _loggingEnabled = false;
        private object _loggingMutex = new object();

        private LogCallback _logDelegate;

#if !XAMARIN
        private BCEntityFactory _entityFactory;
#endif
        private BrainCloudComms _comms;
        private BrainCloudEntity _entityService;
        private BrainCloudGlobalEntity _globalEntityService;
        private BrainCloudGlobalApp _globalAppService;
        private BrainCloudPresence _presenceService;
        private BrainCloudProduct _productService;
        private BrainCloudVirtualCurrency _virtualCurrencyService;
        private BrainCloudAppStore _appStore;
        private BrainCloudPlayerStatistics _playerStatisticsService;
        private BrainCloudGlobalStatistics _globalStatisticsService;
        private BrainCloudIdentity _identityService;
        private BrainCloudScript _scriptService;
        private BrainCloudMatchMaking _matchMakingService;
        private BrainCloudOneWayMatch _oneWayMatchService;
        private BrainCloudPlaybackStream _playbackStreamService;
        private BrainCloudGamification _gamificationService;
        private BrainCloudPlayerState _playerStateService;
        private BrainCloudFriend _friendService;
        private BrainCloudEvent _eventService;
        private BrainCloudSocialLeaderboard _leaderboardService;
        private BrainCloudAsyncMatch _asyncMatchService;
        private BrainCloudTime _timeService;
        private BrainCloudTournament _tournamentService;
        private BrainCloudAuthentication _authenticationService;
        private BrainCloudPushNotification _pushNotificationService;
        private BrainCloudPlayerStatisticsEvent _playerStatisticsEventService;
        private BrainCloudS3Handling _s3HandlingService;
        private BrainCloudRedemptionCode _redemptionCodeService;
        private BrainCloudDataStream _dataStreamService;
        private BrainCloudProfanity _profanityService;
        private BrainCloudFile _fileService;
        private BrainCloudGroup _groupService;
        private BrainCloudMail _mailService;
        private BrainCloudMessaging _messagingService;

        // RTT service
        private BrainCloudLobby _lobbyService;
        private BrainCloudChat _chatService;
        private BrainCloudRTT _rttService;
        private RTTComms _rttComms;

        #endregion Private Data

        #region Public Static
        public static ServerCallback CreateServerCallback(SuccessCallback success, FailureCallback failure, object cbObject = null)
        {
            ServerCallback newCallback = null;

            if (success != null || failure != null)
            {
                newCallback = new ServerCallback(success, failure, cbObject);
            }

            return newCallback;
        }
        #endregion

        #region Constructors

        public BrainCloudClient()
        {
            _comms = new BrainCloudComms(this);
            _entityService = new BrainCloudEntity(this);
#if !XAMARIN
            _entityFactory = new BCEntityFactory(_entityService);
#endif
            _globalEntityService = new BrainCloudGlobalEntity(this);

            _globalAppService = new BrainCloudGlobalApp(this);
            _presenceService = new BrainCloudPresence(this);
            _productService = new BrainCloudProduct(this);
            _virtualCurrencyService = new BrainCloudVirtualCurrency(this);
            _appStore = new BrainCloudAppStore(this);

            _playerStatisticsService = new BrainCloudPlayerStatistics(this);
            _globalStatisticsService = new BrainCloudGlobalStatistics(this);

            _identityService = new BrainCloudIdentity(this);
            _scriptService = new BrainCloudScript(this);
            _matchMakingService = new BrainCloudMatchMaking(this);
            _oneWayMatchService = new BrainCloudOneWayMatch(this);

            _playbackStreamService = new BrainCloudPlaybackStream(this);
            _gamificationService = new BrainCloudGamification(this);
            _playerStateService = new BrainCloudPlayerState(this);
            _friendService = new BrainCloudFriend(this);

            _eventService = new BrainCloudEvent(this);
            _leaderboardService = new BrainCloudSocialLeaderboard(this);
            _asyncMatchService = new BrainCloudAsyncMatch(this);
            _timeService = new BrainCloudTime(this);
            _tournamentService = new BrainCloudTournament(this);

            _authenticationService = new BrainCloudAuthentication(this);
            _pushNotificationService = new BrainCloudPushNotification(this);
            _playerStatisticsEventService = new BrainCloudPlayerStatisticsEvent(this);

            _s3HandlingService = new BrainCloudS3Handling(this);
            _redemptionCodeService = new BrainCloudRedemptionCode(this);
            _dataStreamService = new BrainCloudDataStream(this);
            _profanityService = new BrainCloudProfanity(this);
            _fileService = new BrainCloudFile(this);
            _groupService = new BrainCloudGroup(this);
            _mailService = new BrainCloudMail(this);
            _messagingService = new BrainCloudMessaging(this);

            // RTT 
            _lobbyService = new BrainCloudLobby(this);
            _chatService = new BrainCloudChat(this);
            _rttService = new BrainCloudRTT(this);
            _rttComms = new RTTComms(this);
        }

        //---------------------------------------------------------------

        #endregion

        #region Properties

        public bool Authenticated
        {
            get { return _comms.Authenticated; }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        /// <summary>Returns the sessionId or empty string if no session present.</summary>
        public string SessionID
        {
            get { return _comms != null ? _comms.SessionID : ""; }
        }

        public string AppId
        {
            get { return _comms != null ? _comms.AppId : ""; }
        }

        public string ProfileId
        {
            get { return AuthenticationService != null ? AuthenticationService.ProfileId : ""; }
        }

        public string RTTConnectionID
        {
            get { return _rttComms != null ? _rttComms.RTTConnectionID : ""; }
        }

        public string RTTEventServer
        {
            get { return _rttComms != null ? _rttComms.RTTEventServer : ""; }
        }

        public string AppVersion
        {
            get { return _appVersion; }
        }

        public string BrainCloudClientVersion
        {
            get { return Version.GetVersion(); }
        }

        public Platform ReleasePlatform
        {
            get { return _platform; }
        }

        public string LanguageCode
        {
            get { return string.IsNullOrEmpty(_languageCode) ? Util.GetIsoCodeForCurrentLanguage() : _languageCode; }
            set { _countryCode = value; }
        }

        public string CountryCode
        {
            get { return string.IsNullOrEmpty(_countryCode) ? Util.GetCurrentCountryCode() : _countryCode; }
            set { _countryCode = value; }
        }

        #endregion

        #region Service Properties

        internal BrainCloudComms Comms
        {
            get { return _comms; }
        }

        public BrainCloudEntity EntityService
        {
            get { return _entityService; }
        }

#if !XAMARIN
        public BCEntityFactory EntityFactory
        {
            get { return _entityFactory; }
        }
#endif

        public BrainCloudGlobalEntity GlobalEntityService
        {
            get { return _globalEntityService; }
        }

        public BrainCloudGlobalApp GlobalAppService
        {
            get { return _globalAppService; }
        }

        public BrainCloudPresence PresenceService
        {
            get { return _presenceService; }
        }

        public BrainCloudProduct ProductService
        {
            get { return _productService; }
        }

        public BrainCloudVirtualCurrency VirtualCurrencyService
        {
            get { return _virtualCurrencyService; }
        }

        public BrainCloudAppStore AppStoreService
        {
            get { return _appStore; }
        }

        public BrainCloudPlayerStatistics PlayerStatisticsService
        {
            get { return _playerStatisticsService; }
        }

        public BrainCloudGlobalStatistics GlobalStatisticsService
        {
            get { return _globalStatisticsService; }
        }

        public BrainCloudIdentity IdentityService
        {
            get { return _identityService; }
        }

        public BrainCloudScript ScriptService
        {
            get { return _scriptService; }
        }

        public BrainCloudMatchMaking MatchMakingService
        {
            get { return _matchMakingService; }
        }

        public BrainCloudOneWayMatch OneWayMatchService
        {
            get { return _oneWayMatchService; }
        }

        public BrainCloudPlaybackStream PlaybackStreamService
        {
            get { return _playbackStreamService; }
        }

        public BrainCloudGamification GamificationService
        {
            get { return _gamificationService; }
        }

        public BrainCloudPlayerState PlayerStateService
        {
            get { return _playerStateService; }
        }

        public BrainCloudFriend FriendService
        {
            get { return _friendService; }
        }

        public BrainCloudEvent EventService
        {
            get { return _eventService; }
        }

        public BrainCloudSocialLeaderboard SocialLeaderboardService
        {
            get { return _leaderboardService; }
        }

        public BrainCloudSocialLeaderboard LeaderboardService
        {
            get { return _leaderboardService; }
        }

        public BrainCloudAsyncMatch AsyncMatchService
        {
            get { return _asyncMatchService; }
        }

        public BrainCloudTime TimeService
        {
            get { return _timeService; }
        }

        public BrainCloudTournament TournamentService
        {
            get { return _tournamentService; }
        }

        public BrainCloudAuthentication AuthenticationService
        {
            get { return _authenticationService; }
        }

        public BrainCloudPushNotification PushNotificationService
        {
            get { return _pushNotificationService; }
        }

        public BrainCloudPlayerStatisticsEvent PlayerStatisticsEventService
        {
            get { return _playerStatisticsEventService; }
        }

        public BrainCloudS3Handling S3HandlingService
        {
            get { return _s3HandlingService; }
        }

        public BrainCloudRedemptionCode RedemptionCodeService
        {
            get { return _redemptionCodeService; }
        }

        public BrainCloudDataStream DataStreamService
        {
            get { return _dataStreamService; }
        }

        public BrainCloudProfanity ProfanityService
        {
            get { return _profanityService; }
        }

        public BrainCloudFile FileService
        {
            get { return _fileService; }
        }

        public BrainCloudGroup GroupService
        {
            get { return _groupService; }
        }

        public BrainCloudMail MailService
        {
            get { return _mailService; }
        }

        public BrainCloudRTT RTTService
        {
            get { return _rttService; }
        }

        public BrainCloudLobby LobbyService
        {
            get { return _lobbyService; }
        }

        public BrainCloudChat ChatService
        {
            get { return _chatService; }
        }

        public BrainCloudMessaging MessagingService
        {
            get { return _messagingService; }
        }

        #endregion

        #region Service Getters

        public BrainCloudEntity GetEntityService()
        {
            return EntityService;
        }

#if !XAMARIN
        public BCEntityFactory GetEntityFactory()
        {
            return EntityFactory;
        }
#endif

        public BrainCloudGlobalApp GetGlobalAppService()
        {
            return GlobalAppService;
        }

        public BrainCloudGlobalEntity GetGlobalEntityService()
        {
            return GlobalEntityService;
        }

        public BrainCloudPresence GetPresenceService()
        {
            return PresenceService;
        }

        public BrainCloudProduct GetProductService()
        {
            return ProductService;
        }

        public BrainCloudPlayerStatistics GetPlayerStatisticsService()
        {
            return PlayerStatisticsService;
        }

        public BrainCloudGlobalStatistics GetGlobalStatisticsService()
        {
            return GlobalStatisticsService;
        }

        public BrainCloudIdentity GetIdentityService()
        {
            return IdentityService;
        }

        public BrainCloudScript GetScriptService()
        {
            return ScriptService;
        }

        public BrainCloudMatchMaking GetMatchMakingService()
        {
            return MatchMakingService;
        }

        public BrainCloudOneWayMatch GetOneWayMatchService()
        {
            return OneWayMatchService;
        }

        public BrainCloudPlaybackStream GetPlaybackStreamService()
        {
            return PlaybackStreamService;
        }

        public BrainCloudGamification GetGamificationService()
        {
            return GamificationService;
        }

        public BrainCloudPlayerState GetPlayerStateService()
        {
            return _playerStateService;
        }

        public BrainCloudAsyncMatch GetAsyncMatchService()
        {
            return _asyncMatchService;
        }

        public BrainCloudFriend GetFriendService()
        {
            return _friendService;
        }

        public BrainCloudEvent GetEventService()
        {
            return _eventService;
        }

        public BrainCloudSocialLeaderboard GetSocialLeaderboardService()
        {
            return _leaderboardService;
        }

        public BrainCloudTime GetTimeService()
        {
            return _timeService;
        }

        public BrainCloudTournament GetTournamentService()
        {
            return _tournamentService;
        }

        public BrainCloudAuthentication GetAuthenticationService()
        {
            return _authenticationService;
        }

        public BrainCloudPushNotification GetPushNotificationService()
        {
            return _pushNotificationService;
        }

        public BrainCloudPlayerStatisticsEvent GetPlayerStatisticsEventService()
        {
            return _playerStatisticsEventService;
        }

        public BrainCloudS3Handling GetS3HandlingService()
        {
            return _s3HandlingService;
        }

        public BrainCloudRedemptionCode GetRedemptionCodeService
        {
            get { return _redemptionCodeService; }
        }

        public BrainCloudDataStream GetDataStreamService
        {
            get { return _dataStreamService; }
        }

        public BrainCloudProfanity GetProfanityService
        {
            get { return _profanityService; }
        }

        public BrainCloudFile GetFileService
        {
            get { return _fileService; }
        }

        public BrainCloudGroup GetGroupService
        {
            get { return _groupService; }
        }

        #endregion

        #region Getters

        /// <summary>Returns the sessionId or empty string if no session present.</summary>
        public string GetSessionId()
        {
            return SessionID;
        }

        /// <summary>
        /// Returns true if the user is currently authenticated.
        /// If a session time out or session invalidation is returned from executing a
        /// sever API call, this flag will reset back to false.
        /// </summary>
        public bool IsAuthenticated()
        {
            return Authenticated;
        }

        /// <summary>
        /// Returns true if brainCloud has been initialized.
        /// </summary>
        public bool IsInitialized()
        {
            return Initialized;
        }

        #endregion

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="secretKey">The secret key for your app</param>
        /// <param name="appId ">The app id</param>
        /// <param name="appVersion"> The app version</param>
        public void Initialize(string secretKey, string appId, string appVersion)
        {
            Initialize(s_defaultServerURL, secretKey, appId, appVersion);
        }

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="appId ">The app id</param>
        /// <param name="appIdSecrectMap">The map of appid to secret</param>
        /// <param name="appVersion"> The app version</param>
        public void InitializeWithApps(string defaultAppId, Dictionary<string, string> appIdSecrectMap, string appVersion)
        {
            InitializeWithApps(s_defaultServerURL, defaultAppId, appIdSecrectMap, appVersion);
        }

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="serverURL">The URL to the brainCloud server</param>
        /// <param name="appId ">The app id</param>
        /// <param name="appIdSecrectMap">The map of appid to secret</param>
        /// <param name="appVersion"> The app version</param>
        public void InitializeWithApps(string serverURL, string defaultAppId, Dictionary<string, string> appIdSecrectMap, string appVersion)
        {
            initializeHelper(serverURL, appIdSecrectMap[defaultAppId], defaultAppId, appVersion);

            // set up braincloud which does the message handling
            _comms.InitializeWithApps(serverURL, defaultAppId, appIdSecrectMap);

            _initialized = true;
        }

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="serverURL">The URL to the brainCloud server</param>
        /// <param name="secretKey">The secret key for your app</param>
        /// <param name="appId">The app id</param>
        /// <param name="appVersion">The app version</param>
        public void Initialize(string serverURL, string secretKey, string appId, string appVersion)
        {
            initializeHelper(serverURL, secretKey, appId, appVersion);

            // set up braincloud which does the message handling
            _comms.Initialize(serverURL, appId, secretKey);

            _initialized = true;
        }

        /// <summary>Initialize the identity aspects of brainCloud.</summary>
        /// <param name="profileId">The profile id</param>
        /// <param name="anonymousId">The anonymous id</param>
        public void InitializeIdentity(string profileId, string anonymousId)
        {
            AuthenticationService.Initialize(profileId, anonymousId);
        }

        /// <summary>Shuts down all systems needed for BrainCloudClient
        /// Only call this from the main thread.
        /// Should be used at the end of the app, and opposite of Initialize Client
        /// </summary>
        public void ShutDown()
        {
            _comms.ShutDown();
        }

        /// <summary>Update method needs to be called regularly in order
        /// to process incoming and outgoing messages.
        /// </summary>
        /// 
        public void Update(eBrainCloudUpdateType in_updateType = eBrainCloudUpdateType.ALL)
        {
            switch (in_updateType)
            {
                case eBrainCloudUpdateType.REST:
                    {
                        if (_comms != null) _comms.Update();
                    }
                    break;

                case eBrainCloudUpdateType.RTT:
                    {
                        if (_rttComms != null) _rttComms.Update();
                    }
                    break;

                default:
                case eBrainCloudUpdateType.ALL:
                    {
                        if (_rttComms != null) _rttComms.Update();
                        if (_comms != null) _comms.Update();
                    }
                    break;
            }
        }

        /// <summary>
        /// Enables Real Time event for this session.
        /// Real Time events are disabled by default. Usually events
        /// need to be polled using GET_EVENTS. By enabling this, events will
        /// be received instantly when they happen through a TCP connection to an Event Server.
        ///
        ///This function will first call requestClientConnection, then connect to the address
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void EnableRTT(eRTTConnectionType in_connectionType = eRTTConnectionType.WEBSOCKET, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            _rttComms.EnableRTT(in_connectionType, in_success, in_failure, cb_object);
        }

        /// <summary>
        /// Disables Real Time event for this session.
        /// </summary>
        public void DisableRTT()
        {
            _rttComms.DisableRTT();
        }

        /// <summary>
        /// Sets a callback handler for any out of band event messages that come from
        /// brainCloud.
        /// </summary>
        /// <param name="cb">eventCallback A function which takes a JSON string as it's only parameter.
        ///  The JSON format looks like the following:
        /// {
        ///   "events": [{
        ///      "fromPlayerId": "178ed06a-d575-4591-8970-e23a5d35f9df",
        ///      "eventId": 3967,
        ///      "createdAt": 1441742105908,
        ///      "gameId": "123",
        ///      "toPlayerId": "178ed06a-d575-4591-8970-e23a5d35f9df",
        ///      "eventType": "test",
        ///      "eventData": {"testData": 117}
        ///    }],
        ///    ]
        ///  }
        public void RegisterEventCallback(EventCallback cb)
        {
            _comms.RegisterEventCallback(cb);
        }

        /// <summary>
        /// De-registers the event callback.
        /// </summary>
        public void DeregisterEventCallback()
        {
            _comms.DeregisterEventCallback();
        }

        /// <summary>
        /// Sets a reward handler for any API call results that return rewards.
        /// </summary>
        /// <param name="cb">The reward callback handler.</param>
        /// <see cref="http://getbraincloud.com/apidocs">The brainCloud API docs site for more information on the return JSON</see>
        public void RegisterRewardCallback(RewardCallback cb)
        {
            _comms.RegisterRewardCallback(cb);
        }

        /// <summary>
        /// De-registers the reward callback.
        /// </summary>
        public void DeregisterRewardCallback()
        {
            _comms.DeregisterRewardCallback();
        }

        /// <summary>
        /// Registers the file upload callbacks.
        /// </summary>
        public void RegisterFileUploadCallbacks(FileUploadSuccessCallback success, FileUploadFailedCallback failure)
        {
            _comms.RegisterFileUploadCallbacks(success, failure);
        }

        /// <summary>
        /// De-registers the file upload callbacks.
        /// </summary>
        public void DeregisterFileUploadCallbacks()
        {
            _comms.DeregisterFileUploadCallbacks();
        }

        /// <summary>
        /// Failure callback invoked for all errors generated
        /// </summary>
        public void RegisterGlobalErrorCallback(FailureCallback callback)
        {
            _comms.RegisterGlobalErrorCallback(callback);
        }

        /// <summary>
        /// De-registers the global error callback.
        /// </summary>
        public void DeregisterGlobalErrorCallback()
        {
            _comms.DeregisterGlobalErrorCallback();
        }

        /// <summary>
        /// Registers a callback that is invoked for network errors.
        /// Note this is only called if EnableNetworkErrorMessageCaching
        /// has been set to true.
        /// </summary>
        public void RegisterNetworkErrorCallback(NetworkErrorCallback callback)
        {
            _comms.RegisterNetworkErrorCallback(callback);
        }

        /// <summary>
        /// De-registers the network error callback.
        /// </summary>
        public void DeregisterNetworkErrorCallback()
        {
            _comms.DeregisterNetworkErrorCallback();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTEventCallback(RTTCallback in_callback)
        {
            _rttComms.RegisterRTTCallback(ServiceName.Event, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTEventCallback()
        {
            _rttComms.DeregisterRTTCallback(ServiceName.Event);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTChatCallback(RTTCallback in_callback)
        {
            _rttComms.RegisterRTTCallback(ServiceName.Chat, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTChatCallback()
        {
            _rttComms.DeregisterRTTCallback(ServiceName.Chat);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTMessagingCallback(RTTCallback in_callback)
        {
            _rttComms.RegisterRTTCallback(ServiceName.Messaging, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTMessagingCallback()
        {
            _rttComms.DeregisterRTTCallback(ServiceName.Messaging);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTLobbyCallback(RTTCallback in_callback)
        {
            _rttComms.RegisterRTTCallback(ServiceName.Lobby, in_callback);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTLobbyCallback()
        {
            _rttComms.DeregisterRTTCallback(ServiceName.Lobby);
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterAllRTTCallbacks()
        {
            _rttComms.DeregisterAllRTTCallbacks();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetRTTHeartBeatSeconds(int in_value)
        {
            _rttComms.SetRTTHeartBeatSeconds(in_value);
        }

        /// <summary> Enable logging of brainCloud transactions (comms etc)</summary>
        /// <param name="enable">True if logging is to be enabled</param>
        public void EnableLogging(bool enable)
        {
            _loggingEnabled = enable;
        }

        /// <summary>Allow developers to register their own log handling routine</summary>
        /// <param name="logDelegate">The log delegate</param>
        public void RegisterLogDelegate(LogCallback logDelegate)
        {
            _logDelegate = logDelegate;
        }

        /// <summary>Get the Server URL</summary>
        public string GetUrl()
        {
            return _comms.ServerURL;
        }

        /// <summary>Resets all messages and calls to the server</summary>
        public void ResetCommunication()
        {
            _comms.ResetCommunication();
            _rttComms.DisableRTT();
            AuthenticationService.ClearSavedProfileID();
        }

        /// <summary>Enable Communications with the server. By default this is true</summary>
        /// <param name="value">True to enable comms, false otherwise.</param>
        public void EnableCommunications(bool value)
        {
            _comms.EnableComms(value);
        }

        /// <summary>
        /// Sets the packet timeouts using a list of integers that
        /// represent timeout values for each packet retry. The
        /// first item in the list represents the timeout for the first packet
        /// attempt, the second for the second packet attempt, and so on.
        ///
        /// The number of entries in this array determines how many packet
        /// retries will occur.
        ///
        /// By default, the packet timeout array is {10, 10, 10}
        ///
        /// Note that this method does not change the timeout for authentication
        /// packets (use SetAuthenticationPacketTimeout method).
        ///
        /// </summary>
        /// <param name="timeouts">An array of packet timeouts.</param>
        public void SetPacketTimeouts(List<int> timeouts)
        {
            _comms.PacketTimeouts = timeouts;
        }

        /// <summary>
        /// Sets the packet timeouts back to default.
        /// </summary>
        public void SetPacketTimeoutsToDefault()
        {
            _comms.SetPacketTimeoutsToDefault();
        }

        /// <summary>
        /// Returns the list of packet timeouts.
        /// </summary>
        public List<int> GetPacketTimeouts()
        {
            return _comms.PacketTimeouts;
        }

        /// <summary>
        /// Sets the authentication packet timeout which is tracked separately
        /// from all other packets. Note that authentication packets are never
        /// retried and so this value represents the total time a client would
        /// wait to receive a reply to an authentication API call. By default
        /// this timeout is set to 15 seconds.
        /// </summary>
        /// <param name="valueSecs">The timeout in seconds.</param>
        public void SetAuthenticationPacketTimeout(int timeoutSecs)
        {
            _comms.AuthenticationPacketTimeoutSecs = timeoutSecs;
        }

        /// <summary>
        /// Gets the authentication packet timeout which is tracked separately
        /// from all other packets. Note that authentication packets are never
        /// retried and so this value represents the total time a client would
        /// wait to receive a reply to an authentication API call. By default
        /// this timeout is set to 15 seconds.
        /// </summary>
        public int GetAuthenticationPacketTimeout()
        {
            return _comms.AuthenticationPacketTimeoutSecs;
        }

        /// <summary>
        /// Sets the error callback to return the status message instead of the
        /// error JSON string. This flag is used to conform to pre-2.17 client
        /// behavior.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c>, enable.</param>
        public void SetOldStyleStatusMessageErrorCallback(bool enabled)
        {
            _comms.OldStyleStatusResponseInErrorCallback = enabled;
        }

        /// <summary>
        /// Returns the low transfer rate timeout in secs
        /// </summary>
        public int GetUploadLowTransferRateTimeout()
        {
            return _comms.UploadLowTransferRateTimeout;
        }

        /// <summary>
        /// Sets the timeout in seconds of a low speed upload
        /// (i.e. transfer rate which is underneath the low transfer rate threshold).
        /// By default this is set to 120 secs.Setting this value to 0 will
        /// turn off the timeout. Note that this timeout method
        /// does not work on Unity mobile platforms.
        /// </summary>
        /// <param name="timeoutSecs"></param>
        public void SetUploadLowTransferRateTimeout(int timeoutSecs)
        {
            _comms.UploadLowTransferRateTimeout = timeoutSecs;
        }

        /// <summary>
        /// Returns the low transfer rate threshold in bytes/sec
        /// </summary>
        public int GetUploadLowTransferRateThreshold()
        {
            return _comms.UploadLowTransferRateThreshold;
        }

        /// <summary>
        /// Sets the low transfer rate threshold of an upload in bytes/sec.
        /// If the transfer rate dips below the given threshold longer
        /// than the specified timeout, the transfer will fail.
        /// By default this is set to 50 bytes/sec. Note that this timeout method
        /// does not work on Unity mobile platforms.
        /// </summary>
        /// <param name="bytesPerSec">The low transfer rate threshold in bytes/sec</param>
        public void SetUploadLowTransferRateThreshold(int bytesPerSec)
        {
            _comms.UploadLowTransferRateThreshold = bytesPerSec;
        }

        /// <summary>
        /// Enables the timeout message caching which is disabled by default.
        /// Once enabled, if a client side timeout is encountered
        /// (i.e. brainCloud server is unreachable presumably due to the client
        /// network being down) the SDK will do the following:
        ///
        /// 1 - cache the currently queued messages to brainCloud
        /// 2 - call the network error callback
        /// 3 - then expect the app to call either:
        ///     a) RetryCachedMessages() to retry sending to brainCloud
        ///     b) FlushCachedMessages() to dump all messages in the queue.
        ///
        /// Between steps 2 & 3, the app can prompt the user to retry connecting
        /// to brainCloud to determine whether to follow path 3a or 3b.
        ///
        /// Note that if path 3a is followed, and another timeout is encountered,
        /// the process will begin all over again from step 1.
        ///
        /// WARNING - the brainCloud SDK will cache *all* API calls sent
        /// when a timeout is encountered if this mechanism is enabled.
        /// This effectively freezes all communication with brainCloud.
        /// Apps must call either RetryCachedMessages() or FlushCachedMessages()
        /// for the brainCloud SDK to resume sending messages.
        /// ResetCommunication() will also clear the message cache.
        /// </summary>
        /// <param name="enabled">True if message should be cached on timeout</param>
        public void EnableNetworkErrorMessageCaching(bool enabled)
        {
            _comms.EnableNetworkErrorMessageCaching(enabled);
        }

        /// <summary>
        /// Attempts to resend any cached messages. If no messages are in the cache,
        /// this method does nothing.
        /// </summary>
        public void RetryCachedMessages()
        {
            _comms.RetryCachedMessages();
        }

        /// <summary>
        /// Flushes the cached messages to resume API call processing. This will dump
        /// all of the cached messages in the queue.
        /// </summary>
        /// <param name="sendApiErrorCallbacks">If set to <c>true</c> API error callbacks will
        /// be called for every cached message with statusCode CLIENT_NETWORK_ERROR and reasonCode CLIENT_NETWORK_ERROR_TIMEOUT.
        /// </param>
        public void FlushCachedMessages(bool sendApiErrorCallbacks)
        {
            _comms.FlushCachedMessages(sendApiErrorCallbacks);
        }

        /// <summary>
        /// Inserts a marker which will tell the brainCloud comms layer
        /// to close the message bundle off at this point. Any messages queued
        /// before this method was called will likely be bundled together in
        /// the next send to the server.
        ///
        /// To ensure that only a single message is sent to the server you would
        /// do something like this:
        ///
        /// InsertEndOfMessageBundleMarker()
        /// SomeApiCall()
        /// InsertEndOfMessageBundleMarker()
        ///
        /// </summary>
        public void InsertEndOfMessageBundleMarker()
        {
            _comms.InsertEndOfMessageBundleMarker();
        }

        /// <summary>
        /// Sets the country code sent to brainCloud when a user authenticates.
        /// Will override any auto detected country.
        /// </summary>
        /// <param name="countryCode">ISO 3166-1 two-letter country code</param>
        public void OverrideCountryCode(string countryCode)
        {
            _countryCode = countryCode;
        }

        /// <summary>
        /// Sets the language code sent to brainCloud when a user authenticates.
        /// If the language is set to a non-ISO 639-1 standard value the game default will be used instead.
        /// Will override any auto detected language.
        /// </summary>
        /// <param name="languageCode">ISO 639-1 two-letter language code</param>
        public void OverrideLanguageCode(string languageCode)
        {
            _languageCode = languageCode;
        }

        /// <summary>
        /// Normally not needed as the brainCloud SDK sends heartbeats automatically.
        /// Regardless, this is a manual way to send a heartbeat.
        /// </summary>
        public void SendHeartbeat(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCall sc = new ServerCall(ServiceName.HeartBeat, ServiceOperation.Read, null,
                new ServerCallback(success, failure, cbObject));
            _comms.AddToQueue(sc);
        }

        /// <summary>Method writes log if logging is enabled</summary>
        internal void Log(string log)
        {
#if UNITY_EDITOR
            BrainCloudUnity.BrainCloudPlugin.ResponseEvent.AppendLog(log);
#endif

            if (_loggingEnabled)
            {
                string formattedLog = "#BCC " + (log.Length < 14000 ? log : log.Substring(0, 14000) + " << (LOG TRUNCATED)");
                lock (_loggingMutex)
                {
                    if (_logDelegate != null)
                    {
                        _logDelegate(formattedLog);
                    }
                    else
                    {
#if !(DOT_NET)
                        Debug.Log(formattedLog);
#elif !XAMARIN
                        Console.WriteLine(formattedLog);
#endif
                    }
                }
            }
        }

        /// <summary>Sends a service request message to the server. </summary>
        /// <param name="serviceMessage">The message to send</param>
        internal void SendRequest(ServerCall serviceMessage)
        {
            // pass this directly to the brainCloud Class
            // which will add it to its queue and send back responses accordingly
            _comms.AddToQueue(serviceMessage);
        }

        private void initializeHelper(string serverURL, string secretKey, string appId, string appVersion)
        {
            string error = null;
            if (string.IsNullOrEmpty(serverURL))
                error = "serverURL was null or empty";
            else if (string.IsNullOrEmpty(secretKey))
                error = "secretKey was null or empty";
            else if (string.IsNullOrEmpty(appId))
                error = "appId was null or empty";
            else if (string.IsNullOrEmpty(appVersion))
                error = "appVerson was null or empty";

            if (error != null)
            {
#if !(DOT_NET)
                Debug.LogError("ERROR | Failed to initialize brainCloud - " + error);
#elif !XAMARIN
                Console.WriteLine("ERROR | Failed to initialize brainCloud - " + error);
#endif
                return;
            }

            // TODO: what is our default c# platform?
            Platform platform = Platform.Windows;
#if !(DOT_NET)
            platform = Platform.FromUnityRuntime();
#endif


            _appVersion = appVersion;
            _platform = platform;

            //setup region/country code
            if (Util.GetCurrentCountryCode() == string.Empty)
            {
#if (DOT_NET)
                Util.SetCurrentCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);
#else
                Util.SetCurrentCountryCode(RegionLocale.UsersCountryLocale);
#endif
            }
        }

        #region Deprecated
        /// <summary>A way to get a Singleton instance of brainCloud.</summary>
        [Obsolete("Use of the *singleton* has been deprecated. We recommend that you create your own *variable* to hold an instance of the brainCloudWrapper. Explanation here: http://getbraincloud.com/apidocs/wrappers-clients-and-inconvenient-singletons/")]
        public static BrainCloudClient Get()
        {
            if (!EnableSingletonMode)
#pragma warning disable 162
            {
                throw new Exception(SingletonUseErrorMessage);
            }
#pragma warning restore 162


            // DO NOT USE THIS INTERNALLY WITHIN BRAINCLOUD LIBRARY...
            // THIS IS JUST A CONVENIENCE FOR APP DEVELOPERS TO STORE A SINGLETON!
            if (s_instance == null)
            {
                s_instance = new BrainCloudClient();
            }
            return s_instance;
        }

        [Obsolete("Use of the *singleton* has been deprecated. We recommend that you create your own *variable* to hold an instance of the brainCloudWrapper. Explanation here: http://getbraincloud.com/apidocs/wrappers-clients-and-inconvenient-singletons/")]
        public static BrainCloudClient Instance
        {
            get
            {
                if (!EnableSingletonMode)
#pragma warning disable 162
                {
                    throw new Exception(SingletonUseErrorMessage);
                }
#pragma warning restore 162

                // DO NOT USE THIS INTERNALLY WITHIN BRAINCLOUD LIBRARY...
                // THIS IS JUST A CONVENIENCE FOR APP DEVELOPERS TO STORE A SINGLETON!
                if (s_instance == null)
                {
                    s_instance = new BrainCloudClient();
                }
                return s_instance;
            }
        }
        #endregion
    }
}
