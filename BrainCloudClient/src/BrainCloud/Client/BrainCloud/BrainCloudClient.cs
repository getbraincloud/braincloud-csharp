//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.Common;

#if !XAMARIN
using BrainCloud.Entity;
#endif

#if !(DOT_NET)
using UnityEngine;
#else
using System.Globalization;
using System;
#endif

namespace BrainCloud
{
    #region Delegates

    /// <summary>
    /// Success callback for an API method.
    /// </summary>
    /// <param name="jsonResponse">The json response from the server</param>
    /// <param name="cbObject">The user supplied callback object</param>
    public delegate void SuccessCallback(string jsonResponse, object cbObject);

    /// <summary>
    /// Failure callback for an API method.
    /// </summary>
    /// <param name="status">The http status code</param>
    /// <param name="reasonCode">The error reason code</param>
    /// <param name="jsonError">The error json string</param>
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
    /// </summary>
    public delegate void EventCallback(string jsonResponse);

    /// <summary>
    /// Callback method invoked when brainCloud rewards are received.
    /// </summary>
    public delegate void RewardCallback(string jsonResponse);

    /// <summary>
    /// Method called when a file upload has completed.
    /// </summary>
    /// <param name="fileUploadId">The file upload id</param>
    /// <param name="jsonResponse">The json response describing the file details similar to this</param>
    /// <returns> The JSON returned in the callback is as follows: 
    /// { 
    /// 	"status": 200,
    /// 	"data": { 
    /// 		"fileList": [{   
    /// 			"updatedAt": 1452603368201,
    /// 			"uploadedAt": null,
    /// 			"fileSize": 85470,
    /// 			"shareable": true,
    /// 			"createdAt": 1452603368201,
    /// 			"profileId": "bf8a1433-62d2-448e-b396-f3dbffff44",
    /// 			"gameId": "99999",
    /// 			"path": "test2",
    /// 			"filename": "testup.dat",
    /// 			"downloadUrl": "https://sharedprod.braincloudservers.com/s3/bc/g/99999/u/bf8a14-ff44/f/test2/...",
    /// 			"cloudLocation": "bc/g/99999/u/bf8a1433-62d2-448e-b396-f3dbffff44/f/test2/testup.dat"   
    /// 
    ///         }] 
    /// 	}
    /// }
    /// </returns>
    public delegate void FileUploadSuccessCallback(string fileUploadId, string jsonResponse);

    /// <summary>
    /// Method called when a file upload has failed.
    /// </summary>
    /// <param name="fileUploadId">The file upload id</param>
    /// <param name="statusCode">The http status of the operation</param>
    /// <param name="reasonCode">The reason code of the operation</param>
    /// <param name="jsonResponse">The json response describing the failure. This uses the 
    /// usual brainCloud error format similar to this:</param>
    /// <returns> The JSON returned in the callback is as follows:
    /// {
    /// 	"status": 403,
    /// 	"reason_code": 40300,
    /// 	"status_message": "Message describing failure",
    /// 	"severity": "ERROR"
    /// }
    /// </returns>
    public delegate void FileUploadFailedCallback(string fileUploadId, int statusCode, int reasonCode, string jsonResponse);

    #endregion

    public class BrainCloudClient
    {
        #region Private Data

        private string s_defaultServerURL = "https://sharedprod.braincloudservers.com/dispatcherv2";
        private static BrainCloudClient s_instance;

        private string m_gameVersion = "";
        private Platform m_platform;
        private bool m_initialized;
        private bool m_loggingEnabled = false;
        private object m_loggingMutex = new object();

        private LogCallback m_logDelegate;

#if !XAMARIN
        private BCEntityFactory m_entityFactory;
#endif
        private BrainCloudComms m_comms;
        private BrainCloudEntity m_entityService;
        private BrainCloudGlobalEntity m_globalEntityService;
        private BrainCloudGlobalApp m_globalAppService;
        private BrainCloudProduct m_productService;
        private BrainCloudPlayerStatistics m_playerStatisticsService;
        private BrainCloudGlobalStatistics m_globalStatisticsService;
        private BrainCloudIdentity m_identityService;
        private BrainCloudScript m_scriptService;
        private BrainCloudMatchMaking m_matchMakingService;
        private BrainCloudOneWayMatch m_oneWayMatchService;
        private BrainCloudPlaybackStream m_playbackStreamService;
        private BrainCloudGamification m_gamificationService;
        private BrainCloudPlayerState m_playerStateService;
        private BrainCloudFriend m_friendService;
        private BrainCloudEvent m_eventService;
        private BrainCloudSocialLeaderboard m_leaderboardService;
        private BrainCloudAsyncMatch m_asyncMatchService;
        private BrainCloudTime m_timeService;
        private BrainCloudAuthentication m_authenticationService;
        private BrainCloudTwitter m_twitterService;
        private BrainCloudPushNotification m_pushNotificationService;
        private BrainCloudPlayerStatisticsEvent m_playerStatisticsEventService;
        private BrainCloudS3Handling m_s3HandlingService;
        private BrainCloudRedemptionCode m_redemptionCodeService;
        private BrainCloudDataStream m_dataStreamService;
        private BrainCloudProfanity m_profanityService;
        private BrainCloudFile m_fileService;

        #endregion Private Data

        #region Public Static
        /// <summary>A way to get a Singleton instance of brainCloud.</summary>
        public static BrainCloudClient Get()
        {
            // DO NOT USE THIS INTERNALLY WITHIN BRAINCLOUD LIBRARY...
            // THIS IS JUST A CONVENIENCE FOR APP DEVELOPERS TO STORE A SINGLETON!
            if (s_instance == null)
            {
                s_instance = new BrainCloudClient();
            }
            return s_instance;
        }

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
            m_comms = new BrainCloudComms(this);
            m_entityService = new BrainCloudEntity(this);
#if !XAMARIN
            m_entityFactory = new BCEntityFactory(m_entityService);
#endif
            m_globalEntityService = new BrainCloudGlobalEntity(this);

            m_globalAppService = new BrainCloudGlobalApp(this);
            m_productService = new BrainCloudProduct(this);
            m_playerStatisticsService = new BrainCloudPlayerStatistics(this);
            m_globalStatisticsService = new BrainCloudGlobalStatistics(this);

            m_identityService = new BrainCloudIdentity(this);
            m_scriptService = new BrainCloudScript(this);
            m_matchMakingService = new BrainCloudMatchMaking(this);
            m_oneWayMatchService = new BrainCloudOneWayMatch(this);

            m_playbackStreamService = new BrainCloudPlaybackStream(this);
            m_gamificationService = new BrainCloudGamification(this);
            m_playerStateService = new BrainCloudPlayerState(this);
            m_friendService = new BrainCloudFriend(this);

            m_eventService = new BrainCloudEvent(this);
            m_leaderboardService = new BrainCloudSocialLeaderboard(this);
            m_asyncMatchService = new BrainCloudAsyncMatch(this);
            m_timeService = new BrainCloudTime(this);

            m_authenticationService = new BrainCloudAuthentication(this);
            m_twitterService = new BrainCloudTwitter(this);
            m_pushNotificationService = new BrainCloudPushNotification(this);
            m_playerStatisticsEventService = new BrainCloudPlayerStatisticsEvent(this);

            m_s3HandlingService = new BrainCloudS3Handling(this);
            m_redemptionCodeService = new BrainCloudRedemptionCode(this);
            m_dataStreamService = new BrainCloudDataStream(this);
            m_profanityService = new BrainCloudProfanity(this);
            m_fileService = new BrainCloudFile(this);
        }

        //---------------------------------------------------------------

        #endregion

        #region Properties

        public static BrainCloudClient Instance
        {
            get
            {
                // DO NOT USE THIS INTERNALLY WITHIN BRAINCLOUD LIBRARY...
                // THIS IS JUST A CONVENIENCE FOR APP DEVELOPERS TO STORE A SINGLETON!
                if (s_instance == null)
                {
                    s_instance = new BrainCloudClient();
                }
                return s_instance;
            }
        }

        public bool Authenticated
        {
            get { return m_comms.Authenticated; }
        }

        public bool Initialized
        {
            get { return m_initialized; }
        }

        /// <summary>Returns the sessionId or empty string if no session present.</summary>
        /// <returns>The sessionId or empty string if no session present.</returns>
        public string SessionID
        {
            get { return m_comms != null ? m_comms.SessionID : ""; }
        }

        public string GameId
        {
            get { return m_comms != null ? m_comms.GameId : ""; }
        }

        public string GameVersion
        {
            get { return m_gameVersion; }
        }
        public string BrainCloudClientVersion
        {
            get { return Version.GetVersion(); }
        }

        public Platform ReleasePlatform
        {
            get { return m_platform; }
        }

        #endregion

        #region Service Properties

        internal BrainCloudComms Comms
        {
            get { return m_comms; }
        }

        public BrainCloudEntity EntityService
        {
            get { return m_entityService; }
        }

#if !XAMARIN
        public BCEntityFactory EntityFactory
        {
            get { return m_entityFactory; }
        }
#endif

        public BrainCloudGlobalEntity GlobalEntityService
        {
            get { return m_globalEntityService; }
        }

        public BrainCloudGlobalApp GlobalAppService
        {
            get
            {
                return m_globalAppService;
            }
        }

        public BrainCloudProduct ProductService
        {
            get { return m_productService; }
        }

        public BrainCloudPlayerStatistics PlayerStatisticsService
        {
            get { return m_playerStatisticsService; }
        }

        public BrainCloudGlobalStatistics GlobalStatisticsService
        {
            get { return m_globalStatisticsService; }
        }

        public BrainCloudIdentity IdentityService
        {
            get { return m_identityService; }
        }

        public BrainCloudScript ScriptService
        {
            get { return m_scriptService; }
        }

        public BrainCloudMatchMaking MatchMakingService
        {
            get { return m_matchMakingService; }
        }

        public BrainCloudOneWayMatch OneWayMatchService
        {
            get { return m_oneWayMatchService; }
        }

        public BrainCloudPlaybackStream PlaybackStreamService
        {
            get { return m_playbackStreamService; }
        }

        public BrainCloudGamification GamificationService
        {
            get { return m_gamificationService; }
        }

        public BrainCloudPlayerState PlayerStateService
        {
            get { return m_playerStateService; }
        }

        public BrainCloudFriend FriendService
        {
            get { return m_friendService; }
        }

        public BrainCloudEvent EventService
        {
            get { return m_eventService; }
        }

        public BrainCloudSocialLeaderboard SocialLeaderboardService
        {
            get { return m_leaderboardService; }
        }

        public BrainCloudAsyncMatch AsyncMatchService
        {
            get { return m_asyncMatchService; }
        }

        public BrainCloudTime TimeService
        {
            get { return m_timeService; }
        }

        public BrainCloudAuthentication AuthenticationService
        {
            get { return m_authenticationService; }
        }

        public BrainCloudTwitter TwitterService
        {
            get { return m_twitterService; }
        }

        public BrainCloudPushNotification PushNotificationService
        {
            get { return m_pushNotificationService; }
        }

        public BrainCloudPlayerStatisticsEvent PlayerStatisticsEventService
        {
            get { return m_playerStatisticsEventService; }
        }

        public BrainCloudS3Handling S3HandlingService
        {
            get { return m_s3HandlingService; }
        }

        public BrainCloudRedemptionCode RedemptionCodeService
        {
            get { return m_redemptionCodeService; }
        }

        public BrainCloudDataStream DataStreamService
        {
            get { return m_dataStreamService; }
        }

        public BrainCloudProfanity ProfanityService
        {
            get { return m_profanityService; }
        }

        public BrainCloudFile FileService
        {
            get { return m_fileService; }
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
            return m_playerStateService;
        }

        public BrainCloudAsyncMatch GetAsyncMatchService()
        {
            return m_asyncMatchService;
        }

        public BrainCloudFriend GetFriendService()
        {
            return m_friendService;
        }

        public BrainCloudEvent GetEventService()
        {
            return m_eventService;
        }

        public BrainCloudSocialLeaderboard GetSocialLeaderboardService()
        {
            return m_leaderboardService;
        }

        public BrainCloudTime GetTimeService()
        {
            return m_timeService;
        }

        public BrainCloudAuthentication GetAuthenticationService()
        {
            return m_authenticationService;
        }

        public BrainCloudTwitter GetTwitterService()
        {
            return m_twitterService;
        }

        public BrainCloudPushNotification GetPushNotificationService()
        {
            return m_pushNotificationService;
        }

        public BrainCloudPlayerStatisticsEvent GetPlayerStatisticsEventService()
        {
            return m_playerStatisticsEventService;
        }

        public BrainCloudS3Handling GetS3HandlingService()
        {
            return m_s3HandlingService;
        }

        public BrainCloudRedemptionCode GetRedemptionCodeService
        {
            get { return m_redemptionCodeService; }
        }

        public BrainCloudDataStream GetDataStreamService
        {
            get { return m_dataStreamService; }
        }

        public BrainCloudProfanity GetProfanityService
        {
            get { return m_profanityService; }
        }

        public BrainCloudFile GetFileService
        {
            get { return m_fileService; }
        }

        #endregion

        #region Getters

        /// <summary>Returns the sessionId or empty string if no session present.</summary>
        /// <returns>The sessionId or empty string if no session present.</returns>
        public string GetSessionId()
        {
            return SessionID;
        }

        /// <summary>
        /// Returns true if the user is currently authenticated.
        /// If a session time out or session invalidation is returned from executing a
        /// sever api call, this flag will reset back to false.
        /// </summary>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        public bool IsAuthenticated()
        {
            return Authenticated;
        }

        /// <summary>
        /// Returns true if brainCloud has been initialized.
        /// </summary>
        /// <returns><c>true</c> if brainCloud is initialized; otherwise, <c>false</c>.</returns>
        public bool IsInitialized()
        {
            return Initialized;
        }

        #endregion

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="secretKey">The secret key for your game
        /// <param name="gameId ">The game id</param>
        /// <param name="gameVersion The game version</param>
        /// <param name="cachedProfileId The profile Id</param>
        /// <param name="anonymousId The anonymous Id</param>
        public void Initialize(string secretKey, string gameId, string gameVersion)
        {
            Initialize(s_defaultServerURL, secretKey, gameId, gameVersion);
        }

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="serverURL">The url to the brainCloud server</param>
        /// <param name="secretKey">The secret key for your game
        /// <param name="gameId ">The game id</param>
        /// <param name="gameVersion The game version</param>
        /// <param name="cachedProfileId The profile Id</param>
        /// <param name="anonymousId The anonymous Id</param>
        public void Initialize(string serverURL, string secretKey, string gameId, string gameVersion)
        {
            // TODO: what is our default c# platform?
            Platform platform = Platform.Windows;
#if !(DOT_NET)
            platform = Platform.FromUnityRuntime();
#endif

            // set up braincloud which does the message handling
            m_comms.Initialize(serverURL, gameId, secretKey);

            m_gameVersion = gameVersion;
            m_platform = platform;

            //setup region/country code
            if (Util.GetCurrentCountryCode() == string.Empty)
            {
#if (DOT_NET)
                Util.SetCurrentCountryCode(RegionInfo.CurrentRegion.TwoLetterISORegionName);
#else
                Util.SetCurrentCountryCode(RegionLocale.UsersCountryLocale);
#endif
            }

            m_initialized = true;
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
        /// Should be used at the end of the app, and opposite of Initiatilize Client
        /// </summary>
        public void ShutDown()
        {
            m_comms.ShutDown();
        }

        /// <summary>Update method needs to be called regularly in order
        /// to process incoming and outgoing messages.
        /// </summary>
        public void Update()
        {
            if (m_comms != null) m_comms.Update();
        }

        /// <summary>
        /// Sets a callback handler for any out of band event messages that come from
        /// brainCloud.
        /// </summary>
        /// <param name="cb">eventCallback A function which takes a json string as it's only parameter.
        ///  The json format looks like the following:
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
            m_comms.RegisterEventCallback(cb);
        }

        /// <summary>
        /// Deregisters the event callback.
        /// </summary>
        public void DeregisterEventCallback()
        {
            m_comms.DeregisterEventCallback();
        }

        /// <summary>
        /// Sets a reward handler for any api call results that return rewards.
        /// </summary>
        /// <param name="cb">The reward callback handler.</param>
        /// <see cref="http://getbraincloud.com/apidocs">The brainCloud apidocs site for more information on the return JSON</see>
        public void RegisterRewardCallback(RewardCallback cb)
        {
            m_comms.RegisterRewardCallback(cb);
        }

        /// <summary>
        /// Deregisters the reward callback.
        /// </summary>
        public void DeregisterRewardCallback()
        {
            m_comms.DeregisterRewardCallback();
        }

        /// <summary>
        /// Registers the file upload callbacks.
        /// </summary>
        public void RegisterFileUploadCallbacks(FileUploadSuccessCallback success, FileUploadFailedCallback failure)
        {
            m_comms.RegisterFileUploadCallbacks(success, failure);
        }

        /// <summary>
        /// Deregisters the file upload callbacks.
        /// </summary>
        public void DeregisterFileUploadCallbacks()
        {
            m_comms.DeregisterFileUploadCallbacks();
        }

        /// <summary>
        /// Failure callback invoked for all errors generated
        /// </summary>
        public void RegisterGlobalErrorCallback(FailureCallback callback)
        {
            m_comms.RegisterGlobalErrorCallback(callback);
        }

        /// <summary>
        /// Deregisters the global error callback.
        /// </summary>
        public void DeregisterGlobalErrorCallback()
        {
            m_comms.DeregisterGlobalErrorCallback();
        }

        /// <summary>
        /// Registers a callback that is invloked for network errors.
        /// Note this is only called if EnableNetworkErrorMessageCaching
        /// has been set to true.
        /// </summary>
        public void RegisterNetworkErrorCallback(NetworkErrorCallback callback)
        {
            m_comms.RegisterNetworkErrorCallback(callback);
        }

        /// <summary>
        /// Deregisters the network error callback.
        /// </summary>
        public void DeregisterNetworkErrorCallback()
        {
            m_comms.DeregisterNetworkErrorCallback();
        }

        /// <summary> Enable logging of braincloud transactions (comms etc)</summary>
        /// <param name="enable">True if logging is to be enabled</param>
        public void EnableLogging(bool enable)
        {
            m_loggingEnabled = enable;
        }

        /// <summary>Allow developers to register their own log handling routine</summary>
        /// <param name="logDelegate">The log delegate</param>
        public void RegisterLogDelegate(LogCallback logDelegate)
        {
            m_logDelegate = logDelegate;
        }

        /// <summary>Get the Server URL</summary>
        public string GetUrl()
        {
            return m_comms.ServerURL;
        }

        /// <summary>Resets all messages and calls to the server</summary>
        public void ResetCommunication()
        {
            m_comms.ResetCommunication();
            AuthenticationService.ClearSavedProfileID();
        }

        /// <summary>Enable Communications with the server. By default this is true</summary>
        /// <param name="value">True to enable comms, false otherwise.</param>
        public void EnableCommunications(bool value)
        {
            m_comms.EnableComms(value);
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
            m_comms.PacketTimeouts = timeouts;
        }

        /// <summary>
        /// Sets the packet timeouts back to default.
        /// </summary>
        public void SetPacketTimeoutsToDefault()
        {
            m_comms.SetPacketTimeoutsToDefault();
        }

        /// <summary>
        /// Returns the list of packet timeouts.
        /// </summary>
        /// <returns>The packet timeouts.</returns>
        public List<int> GetPacketTimeouts()
        {
            return m_comms.PacketTimeouts;
        }

        /// <summary>
        /// Sets the authentication packet timeout which is tracked separately
        /// from all other packets. Note that authentication packets are never
        /// retried and so this value represents the total time a client would
        /// wait to receive a reply to an authentication api call. By default
        /// this timeout is set to 15 seconds.
        /// </summary>
        /// <param name="valueSecs">The timeout in seconds.</param>
        public void SetAuthenticationPacketTimeout(int timeoutSecs)
        {
            m_comms.AuthenticationPacketTimeoutSecs = timeoutSecs;
        }

        /// <summary>
        /// Gets the authentication packet timeout which is tracked separately
        /// from all other packets. Note that authentication packets are never
        /// retried and so this value represents the total time a client would
        /// wait to receive a reply to an authentication api call. By default
        /// this timeout is set to 15 seconds.
        /// </summary>
        /// <returns>The authentication packet timeoutin seconds.</returns>
        public int GetAuthenticationPacketTimeout()
        {
            return m_comms.AuthenticationPacketTimeoutSecs;
        }

        /// <summary>
        /// Sets the error callback to return the status message instead of the
        /// error json string. This flag is used to conform to pre-2.17 client
        /// behaviour.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c>, enable.</param>
        public void SetOldStyleStatusMessageErrorCallback(bool enabled)
        {
            m_comms.OldStyleStatusResponseInErrorCallback = enabled;
        }

        /// <summary>
        /// Returns the low transfer rate timeout in secs
        /// </summary>
        /// <returns>The low transfer rate timeout in secs</returns>
        public int GetUploadLowTransferRateTimeout()
        {
            return m_comms.UploadLowTransferRateTimeout;
        }

        /// <summary>
        /// Sets the timeout in seconds of a low speed upload
        /// (ie transfer rate which is underneath the low transfer rate threshold).
        /// By default this is set to 120 secs.Setting this value to 0 will
        /// turn off the timeout. Note that this timeout method
        /// does not work on Unity mobile platforms.
        /// </summary>
        /// <param name="timeoutSecs"></param>
        public void SetUploadLowTransferRateTimeout(int timeoutSecs)
        {
            m_comms.UploadLowTransferRateTimeout = timeoutSecs;
        }

        /// <summary>
        /// Returns the low transfer rate threshold in bytes/sec
        /// </summary>
        /// <returns>The low transfer rate threshold in bytes/sec</returns>
        public int GetUploadLowTransferRateThreshold()
        {
            return m_comms.UploadLowTransferRateThreshold;
        }

        /// <summary>
        /// Sets the low transfer rate threshold of an upload in bytes/sec.
        /// If the transfer rate dips below the given threshold longer
        /// than the specified timeout, the transfer will fail.
        /// By default this is set to 50 bytes/sec. Note that this timeout method
        /// does not work on Unity mobile platforms.
        /// </summary>
        /// <param name="in_bytesPerSec">The low transfer rate threshold in bytes/sec</param>
        public void SetUploadLowTransferRateThreshold(int in_bytesPerSec)
        {
            m_comms.UploadLowTransferRateThreshold = in_bytesPerSec;
        }

        /// <summary>
        /// Enables the timeout message caching which is disabled by default.
        /// Once enabled, if a client side timeout is encountered 
        /// (i.e. brainCloud server is unreachable presumably due to the client
        /// network being down) the sdk will do the following:
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
        /// WARNING - the brainCloud sdk will cache *all* api calls sent
        /// when a timeout is encountered if this mechanism is enabled.
        /// This effectively freezes all communication with brainCloud.
        /// Apps must call either RetryCachedMessages() or FlushCachedMessages() 
        /// for the brainCloud SDK to resume sending messages.
        /// ResetCommunication() will also clear the message cache.
        /// </summary>
        /// <param name="in_enabled">True if message should be cached on timeout</param>
        public void EnableNetworkErrorMessageCaching(bool in_enabled)
        {
            m_comms.EnableNetworkErrorMessageCaching(in_enabled);
        }

        /// <summary>
        /// Attempts to resend any cached messages. If no messages are in the cache,
        /// this method does nothing.
        /// </summary>
        public void RetryCachedMessages()
        {
            m_comms.RetryCachedMessages();
        }

        /// <summary>
        /// Flushes the cached messages to resume api call processing. This will dump
        /// all of the cached messages in the queue.
        /// </summary>
        /// <param name="in_sendApiErrorCallbacks">If set to <c>true</c> API error callbacks will
        /// be called for every cached message with statusCode CLIENT_NETWORK_ERROR and reasonCode CLIENT_NETWORK_ERROR_TIMEOUT.
        /// </param>
        public void FlushCachedMessages(bool in_sendApiErrorCallbacks)
        {
            m_comms.FlushCachedMessages(in_sendApiErrorCallbacks);
        }

        /// <summary>
        /// Normally not needed as the brainCloud SDK sends heartbeats automatically.
        /// Regardless, this is a manual way to send a heartbeat.
        /// </summary>
        public void SendHeartbeat(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCall sc = new ServerCall(ServiceName.HeartBeat, ServiceOperation.Read, null,
                new ServerCallback(in_success, in_failure, in_cbObject));
            m_comms.AddToQueue(sc);
        }

        /// <summary>Method writes log if logging is enabled</summary>
        internal void Log(string log)
        {
            if (m_loggingEnabled)
            {
                string formattedLog = "#BCC " + (log.Length < 14000 ? log : log.Substring(0, 14000) + " << (LOG TRUNCATED)");
                lock (m_loggingMutex)
                {
                    if (m_logDelegate != null)
                    {
                        m_logDelegate(formattedLog);
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
            m_comms.AddToQueue(serviceMessage);
        }
    }
}
