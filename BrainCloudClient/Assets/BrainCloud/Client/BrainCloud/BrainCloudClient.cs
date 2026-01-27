// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.Common;
    using BrainCloud.JsonFx.Json;
#if !XAMARIN
    using BrainCloud.Entity;
    using System;
#endif

#if !(DOT_NET || GODOT)
    using UnityEngine;
    using UnityEngine.Assertions;
    using System.Text;
#else
using System.Globalization;
#endif

    #region Enums
    public enum eBrainCloudUpdateType
    {
        ALL,
        REST,
        RTT,
        RS,
        PING,

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
    /// Relay callback.
    /// </summary>
    /// <param name="data">The data send from netId user</param>
    public delegate void RelayCallback(short netId, byte[] data);

    /// <summary>
    /// Relay system callback.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the server</param>
    public delegate void RelaySystemCallback(string jsonResponse);

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

    public delegate void JsonSerializationSuccessCallback(string jsonResponse);
    public delegate void JsonSerializationFailureCallback(int statusCode, int reasonCode, string errorMessage);

    #endregion

    public class BrainCloudClient
    {
        #region Private Data

        private string s_defaultServerURL = "https://api.braincloudservers.com/dispatcherv2";


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
        private RTTComms _rttComms;
        private RelayComms _rsComms;

        private BrainCloudEntity _entityService;
        private BrainCloudGlobalEntity _globalEntityService;
        private BrainCloudGlobalApp _globalAppService;
        private BrainCloudPresence _presenceService;
        private BrainCloudVirtualCurrency _virtualCurrencyService;
        private BrainCloudAppStore _appStore;
        private BrainCloudPlayerStatistics _playerStatisticsService;
        private BrainCloudGlobalStatistics _globalStatisticsService;
        private BrainCloudIdentity _identityService;
        private BrainCloudItemCatalog _itemCatalogService;
        private BrainCloudUserItems _userItemsService;
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
        private BrainCloudGlobalFile _globalFileService;
        private BrainCloudCustomEntity _customEntityService;
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
        private BrainCloudBlockchain _blockchain;
        private BrainCloudGroupFile _groupFileService;

        // RTT service
        private BrainCloudLobby _lobbyService;
        private BrainCloudChat _chatService;
        private BrainCloudRTT _rttService;
        private BrainCloudRelay _rsService;

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
            init();
        }

        public BrainCloudClient(BrainCloudWrapper in_wrapper)
        {
            Wrapper = in_wrapper;
            init();
        }

        private void init()
        {
            _comms = new BrainCloudComms(this);
            _rttComms = new RTTComms(this);
            _rsComms = new RelayComms(this);

            _entityService = new BrainCloudEntity(this);
#if !XAMARIN
            _entityFactory = new BCEntityFactory(_entityService);
#endif
            _globalEntityService = new BrainCloudGlobalEntity(this);

            _globalAppService = new BrainCloudGlobalApp(this);
            _presenceService = new BrainCloudPresence(this);
            _virtualCurrencyService = new BrainCloudVirtualCurrency(this);
            _appStore = new BrainCloudAppStore(this);

            _playerStatisticsService = new BrainCloudPlayerStatistics(this);
            _globalStatisticsService = new BrainCloudGlobalStatistics(this);

            _identityService = new BrainCloudIdentity(this);
            _itemCatalogService = new BrainCloudItemCatalog(this);
            _userItemsService = new BrainCloudUserItems(this);
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
            _globalFileService = new BrainCloudGlobalFile(this);
            _customEntityService = new BrainCloudCustomEntity(this);

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
            _groupFileService = new BrainCloudGroupFile(this);

            // RTT 
            _lobbyService = new BrainCloudLobby(this);
            _chatService = new BrainCloudChat(this);
            _rttService = new BrainCloudRTT(_rttComms, this);
            _rsService = new BrainCloudRelay(_rsComms, this);

            _blockchain = new BrainCloudBlockchain(this);
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

        public void EnableCompressedRequests(bool isEnabled)
        {
            _comms.EnableCompression(isEnabled);
        }

        public void EnableCompressedResponses(bool isEnabled)
        {
            _authenticationService.CompressResponses = isEnabled;
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

        public string GetAppId() { return AppId; }

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

        public string GetAppVersion() { return AppVersion; }

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
            set { _languageCode = value; }
        }

        public string CountryCode
        {
            get { return string.IsNullOrEmpty(_countryCode) ? Util.GetCurrentCountryCode() : _countryCode; }
            set { _countryCode = value; }
        }

        #endregion

        #region Service Properties

        public BrainCloudWrapper Wrapper
        {
            get;
            set;
        }

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

        public BrainCloudItemCatalog ItemCatalogService
        {
            get { return _itemCatalogService; }
        }

        public BrainCloudUserItems UserItemsService
        {
            get { return _userItemsService; }
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

        public BrainCloudGlobalFile GlobalFileService
        {
            get { return _globalFileService; }
        }

        public BrainCloudCustomEntity CustomEntityService
        {
            get { return _customEntityService; }
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

        public BrainCloudRelay RelayService
        {
            get { return _rsService; }
        }

        public BrainCloudBlockchain Blockchain
        {
            get { return _blockchain; }
        }

        public BrainCloudGroupFile GroupFileService
        {
            get { return _groupFileService; }
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

        public BrainCloudItemCatalog GetItemCatalogService()
        {
            return ItemCatalogService;
        }
        public BrainCloudUserItems GetUserItemsService()
        {
            return UserItemsService;
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

        public BrainCloudGlobalFile GetGlobalFileService()
        {
            return _globalFileService;
        }

        public BrainCloudCustomEntity GetCustomEntityService()
        {
            return _customEntityService;
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
        /// Returns whether the client is authenticated with the brainCloud server.
        /// </summary>
        /// <returns>True if authenticated, false otherwise.</returns>



        public bool IsAuthenticated()
        {
            return Authenticated;
        }

        public long GetReceivedPacketId()
        {
            return _comms.GetReceivedPacketId();
        }

        /// <summary>
        /// Returns whether the client is initialized.
        /// </summary>
        /// <returns>True if initialized, false otherwise.</returns>



        public bool IsInitialized()
        {
            return Initialized;
        }

        public int MaxDepth
        {
            get { return _comms.MaxDepth; }
            set
            {
                _comms.MaxDepth = value;
            }
        }
        #endregion



        /// <summary>
        /// Method initializes the BrainCloudClient. Automatically passes in current serverURL
        /// </summary>
        /// <param name="secretKey">The secret key for your game</param>
        /// <param name="appId">The app id</param>
        /// <param name="appVersion">The version</param>












        public void Initialize(string secretKey, string appId, string appVersion)
        {
            Initialize(s_defaultServerURL, secretKey, appId, appVersion);
        }

        /// <summary>
        /// Method initializes the BrainCloudClient with multiple app/secret.
        /// </summary>
        /// <param name="defaultAppId">The default app id that we start with</param>
        /// <param name="secretMap">A map of <appId, secretKey></param>
        /// <param name="appVersion">The version</param>












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

        /// <summary>
        /// Initialize - initializes the identity service with the saved
        /// </summary>
        /// <param name="profileId">The id of the profile id that was most recently used by the app (on this device)</param>
        /// <param name="anonymousId">The anonymous installation id that was generated for this device</param>



        public void InitializeIdentity(string profileId, string anonymousId)
        {
            AuthenticationService.Initialize(profileId, anonymousId);
        }

        /// <summary>
        /// Shuts the brainCloud client down.
        /// </summary>

        public void ShutDown()
        {
            _comms.ShutDown();
        }

        /// <summary>
        /// Run callbacks, to be called once per frame from your main thread
        /// </summary>

        public void RunCallbacks(eBrainCloudUpdateType in_updateType = eBrainCloudUpdateType.ALL)
        {
            Update(in_updateType);
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

                case eBrainCloudUpdateType.RS:
                    {
                        if (_rsComms != null) _rsComms.Update();
                    }
                    break;

                case eBrainCloudUpdateType.PING:
                    {
                        if (_lobbyService != null) _lobbyService.Update();
                    }
                    break;

                default:
                case eBrainCloudUpdateType.ALL:
                    {
                        if (_rttComms != null) _rttComms.Update();
                        if (_comms != null) _comms.Update();
                        if (_rsComms != null) _rsComms.Update();
                        if (_lobbyService != null) _lobbyService.Update();
                    }
                    break;
            }
        }

        /// <summary>
        /// Sets a callback handler for any out of band event messages that come from
        /// </summary>
        /// <param name="eventCallback">A function which takes a json string as it's only parameter. The json format looks like the following: { "events": [{ "fromPlayerId": "178ed06a-d575-4591-8970-e23a5d35f9df", "eventId": 3967, "createdAt": 1441742105908, "gameId": "123", "toPlayerId": "178ed06a-d575-4591-8970-e23a5d35f9df", "eventType": "test", "eventData": {"testData": 117} }], ] }</param>



        public void RegisterEventCallback(EventCallback cb)
        {
            _comms.RegisterEventCallback(cb);
        }

        /// <summary>
        /// Deregisters the event callback
        /// </summary>

        public void DeregisterEventCallback()
        {
            _comms.DeregisterEventCallback();
        }

        /// <summary>
        /// Sets a reward handler for any api call results that return rewards.
        /// </summary>
        /// <param name="rewardCallback">The reward callback handler. @see The brainCloud apidocs site for more information on the return JSON</param>



        public void RegisterRewardCallback(RewardCallback cb)
        {
            _comms.RegisterRewardCallback(cb);
        }

        /// <summary>
        /// Deregisters the reward callback
        /// </summary>

        public void DeregisterRewardCallback()
        {
            _comms.DeregisterRewardCallback();
        }

        [Obsolete("This has been deprecated, use RegisterFileUploadCallback instead")]
        public void RegisterFileUploadCallbacks(FileUploadSuccessCallback success, FileUploadFailedCallback failure)
        {
            _comms.RegisterFileUploadCallbacks(success, failure);
        }

        [Obsolete("This has been deprecated, use DeregisterFileUploadCallback instead")]
        public void DeregisterFileUploadCallbacks()
        {
            _comms.DeregisterFileUploadCallbacks();
        }

        /// <summary>
        /// Registers a file upload callback handler to listen for status updates on uploads
        /// </summary>
        /// <param name="fileUploadCallback">The file upload callback handler.</param>


        public void RegisterFileUploadCallback(FileUploadSuccessCallback success, FileUploadFailedCallback failure)
        {
            _comms.RegisterFileUploadCallbacks(success, failure);
        }

        /// <summary>
        /// Deregisters the file upload callback
        /// </summary>

        public void DeregisterFileUploadCallback()
        {
            _comms.DeregisterFileUploadCallbacks();
        }

        /// <summary>
        /// Registers a callback that is invoked for all errors generated
        /// </summary>
        /// <param name="globalErrorCallback">The global error callback handler.</param>


        public void RegisterGlobalErrorCallback(FailureCallback callback)
        {
            _comms.RegisterGlobalErrorCallback(callback);
        }

        /// <summary>
        /// Deregisters the global error callback
        /// </summary>

        public void DeregisterGlobalErrorCallback()
        {
            _comms.DeregisterGlobalErrorCallback();
        }

        /// <summary>
        /// Registers a callback that is invoked for network errors.
        /// </summary>
        /// <param name="networkErrorCallback">The network error callback handler.</param>



        public void RegisterNetworkErrorCallback(NetworkErrorCallback callback)
        {
            _comms.RegisterNetworkErrorCallback(callback);
        }

        /// <summary>
        /// Deregisters the network error callback
        /// </summary>

        public void DeregisterNetworkErrorCallback()
        {
            _comms.DeregisterNetworkErrorCallback();
        }

        /// <summary>
        /// Set to true to enable logging packets to std::out
        /// </summary>

        public void EnableLogging(bool enable)
        {
            _loggingEnabled = enable;
        }

        /// <summary> Check if logging of brainCloud transactions is enabled</summary>
        public bool LoggingEnabled { get { return _loggingEnabled; } }

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

        /// <summary>
        /// Clears any pending messages from communication library.
        /// </summary>

        public void ResetCommunication()
        {
            _comms.ResetCommunication();
            _rttComms.DisableRTT();
            _rsComms.Disconnect();
            Update();
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
        /// </summary>
        /// <param name="timeouts">A vector of packet timeouts.</param>



        public void SetPacketTimeouts(List<int> timeouts)
        {
            _comms.PacketTimeouts = timeouts;
        }

        /// <summary>
        /// Sets the packet timeouts back to the default ie {10, 10, 10}
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
        /// </summary>
        /// <param name="timeoutSecs">The timeout in seconds</param>



        public void SetAuthenticationPacketTimeout(int timeoutSecs)
        {
            _comms.AuthenticationPacketTimeoutSecs = timeoutSecs;
        }

        /// <summary>
        /// Gets the authentication packet timeout which is tracked separately
        /// </summary>
        /// <returns>The timeout in seconds</returns>



        public int GetAuthenticationPacketTimeout()
        {
            return _comms.AuthenticationPacketTimeoutSecs;
        }

        /// <summary>
        /// Sets the error callback to return the status message instead of the
        /// </summary>
        /// <param name="enabled">If set to true, enable</param>



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
        /// </summary>
        /// <param name="timeoutSecs">The timeout in secs</param>



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
        /// </summary>
        /// <param name="bytesPerSec">The low transfer rate threshold in bytes/sec</param>



        public void SetUploadLowTransferRateThreshold(int bytesPerSec)
        {
            _comms.UploadLowTransferRateThreshold = bytesPerSec;
        }

        /// <summary>
        /// Enables the message caching upon network error, which is disabled by default.
        /// </summary>
        /// <param name="enabled">True if message should be cached on timeout</param>



        public void EnableNetworkErrorMessageCaching(bool enabled)
        {
            _comms.EnableNetworkErrorMessageCaching(enabled);
        }

        /// <summary>
        /// Attempts to resend any cached messages. If no messages are in the cache,
        /// </summary>



        public void RetryCachedMessages()
        {
            _comms.RetryCachedMessages();
        }

        /// <summary>
        /// Flushes the cached messages to resume api call processing. This will dump
        /// </summary>
        /// <param name="sendApiErrorCallbacks">If set to true API error callbacks will be called for every cached message with statusCode CLIENT_NETWORK_ERROR and reasonCode CLIENT_NETWORK_ERROR_TIMEOUT.</param>



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
        /// </summary>
        /// <param name="countryCode">ISO 3166-1 two-letter country code</param>



        public void OverrideCountryCode(string countryCode)
        {
            _countryCode = countryCode;
        }

        /// <summary>
        /// Sets the language code sent to brainCloud when a user authenticates.
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
        /// 
        internal void Log(string log, bool bypassLogEnabled = false)
        {
            if (_loggingEnabled || bypassLogEnabled)
            {
                string formattedLog = DateTime.Now.ToString("HH:mm:ss.fff") + " #BCC " + (log.Length < 14000 ? log : log.Substring(0, 14000) + " << (LOG TRUNCATED)");
                lock (_loggingMutex)
                {
                    if (_logDelegate != null)
                    {
                        _logDelegate(formattedLog);
                    }
                    else
                    {
#if GODOT
                        Godot.GD.Print(formattedLog);
#elif !DOT_NET
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

        public string SerializeJson(object payLoad)
        {
            return _comms.SerializeJson(payLoad);
        }

        public Dictionary<string, object> DeserializeJson(string jsonData)
        {
            return _comms.DeserializeJson(jsonData);
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
#if GODOT
                Godot.GD.Print("ERROR | Failed to initialize brainCloud - " + error);
#elif !DOT_NET
                Debug.LogError("ERROR | Failed to initialize brainCloud - " + error);
#elif !XAMARIN
                Console.WriteLine("ERROR | Failed to initialize brainCloud - " + error);
#endif
                return;
            }

            // TODO: what is our default c# platform?
            Platform platform = Platform.Windows;
#if !(DOT_NET || GODOT)
            platform = Platform.FromUnityRuntime();
#endif

            _appVersion = appVersion;
            _platform = platform;

            //setup region/country code
            if (Util.GetCurrentCountryCode() == string.Empty)
            {
#if (DOT_NET || GODOT)
                Util.SetCurrentCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);
#else
                Util.SetCurrentCountryCode(RegionLocale.UsersCountryLocale);
#endif
            }
        }
    }
}
