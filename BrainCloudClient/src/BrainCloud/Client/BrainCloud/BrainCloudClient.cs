//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using BrainCloud.Internal;
using BrainCloud.Common;
using BrainCloud.Entity;


#if !(DOT_NET)
using UnityEngine;
#endif

namespace BrainCloud
{
    public delegate void SuccessCallback(string responseData, object cbObject);
    public delegate void FailureCallback(string errorData, object cbObject);
    public delegate void LogCallback(string log);
    public delegate void NetworkErrorHandler(string error);

//[Serializable]
    public class BrainCloudClient
    {
        #region Public Static

        private static BrainCloudClient s_instance;

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

        public static ServerCallback CreateServerCallback(SuccessCallback in_success, FailureCallback in_failure, object in_cbObject = null)
        {
            ServerCallback newCallback = null;

            if (in_success != null || in_failure != null)
            {
                newCallback = new ServerCallback(in_success, in_failure, in_cbObject);
            }

            return newCallback;
        }

        #endregion

        #region Constructors

        public BrainCloudClient()
        {
            m_bc = new BrainCloudComms(this);
            m_entityService = new BrainCloudEntity(this);
            m_entityFactory = new BCEntityFactory(m_entityService);
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
            m_socialLeaderboardService = new BrainCloudSocialLeaderboard(this);
            m_asyncMatchService = new BrainCloudAsyncMatch(this);
            m_timeService = new BrainCloudTime(this);

            m_authenticationService = new BrainCloudAuthentication(this);
            m_twitterService = new BrainCloudTwitter(this);
            m_pushNotificationService = new BrainCloudPushNotification(this);
            m_playerStatisticsEventService = new BrainCloudPlayerStatisticsEvent(this);
        }

        //---------------------------------------------------------------

        #endregion

        #region Private Data

        private BCEntityFactory m_entityFactory;

        private BrainCloudComms m_bc;
        private bool m_initialized;
        private bool m_loggingEnabled = false;
        private object m_loggingMutex = new object();
        private LogCallback m_logDelegate;

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
        private BrainCloudSocialLeaderboard m_socialLeaderboardService;
        private BrainCloudAsyncMatch m_asyncMatchService;
        private BrainCloudTime m_timeService;
        private BrainCloudAuthentication m_authenticationService;
        private BrainCloudTwitter m_twitterService;
        private BrainCloudPushNotification m_pushNotificationService;
        private BrainCloudPlayerStatisticsEvent m_playerStatisticsEventService;

        #endregion Private Data

        #region Properties

        public BrainCloudEntity EntityService
        {
            get
            {
                return m_entityService;
            }
        }

        public BrainCloudEntity GetEntityService()
        {
            return this.EntityService;
        }

        public BCEntityFactory EntityFactory
        {
            get
            {
                return m_entityFactory;
            }
        }

        public BCEntityFactory GetEntityFactory()
        {
            return EntityFactory;
        }

        public BrainCloudGlobalEntity GlobalEntityService
        {
            get
            {
                return m_globalEntityService;
            }
        }

        public BrainCloudGlobalApp GlobalAppService
        {
            get
            {
                return m_globalAppService;
            }
        }

        public BrainCloudGlobalApp GetGlobalAppService()
        {
            return this.GlobalAppService;
        }

        public BrainCloudGlobalEntity GetGlobalEntityService()
        {
            return this.GlobalEntityService;
        }

        public BrainCloudProduct ProductService
        {
            get
            {
                return m_productService;
            }
        }

        public BrainCloudProduct GetProductService()
        {
            return this.ProductService;
        }

        public BrainCloudPlayerStatistics PlayerStatisticsService
        {
            get
            {
                return m_playerStatisticsService;
            }
        }

        public BrainCloudPlayerStatistics GetPlayerStatisticsService()
        {
            return this.PlayerStatisticsService;
        }

        public BrainCloudGlobalStatistics GlobalStatisticsService
        {
            get
            {
                return m_globalStatisticsService;
            }
        }

        public BrainCloudGlobalStatistics GetGlobalStatisticsService()
        {
            return this.GlobalStatisticsService;
        }

        public BrainCloudIdentity IdentityService
        {
            get
            {
                return m_identityService;
            }
        }
        public BrainCloudIdentity GetIdentityService()
        {
            return this.IdentityService;
        }

        public BrainCloudScript ScriptService
        {
            get
            {
                return m_scriptService;
            }
        }

        public BrainCloudScript GetScriptService()
        {
            return this.ScriptService;
        }

        public BrainCloudMatchMaking MatchMakingService
        {
            get
            {
                return m_matchMakingService;
            }
        }

        public BrainCloudMatchMaking GetMatchMakingService()
        {
            return this.MatchMakingService;
        }

        public BrainCloudOneWayMatch OneWayMatchService
        {
            get
            {
                return m_oneWayMatchService;
            }
        }

        public BrainCloudOneWayMatch GetOneWayMatchService()
        {
            return this.OneWayMatchService;
        }

        public BrainCloudPlaybackStream PlaybackStreamService
        {
            get
            {
                return m_playbackStreamService;
            }
        }

        public BrainCloudPlaybackStream GetPlaybackStreamService()
        {
            return this.PlaybackStreamService;
        }

        public BrainCloudGamification GamificationService
        {
            get
            {
                return m_gamificationService;
            }
        }

        public BrainCloudGamification GetGamificationService()
        {
            return this.GamificationService;
        }

        public BrainCloudPlayerState PlayerStateService
        {
            get
            {
                return m_playerStateService;
            }
        }

        public BrainCloudPlayerState GetPlayerStateService()
        {
            return this.m_playerStateService;
        }

        public BrainCloudFriend FriendService
        {
            get
            {
                return m_friendService;
            }
        }

        public BrainCloudFriend GetFriendService()
        {
            return this.m_friendService;
        }

        public BrainCloudEvent EventService
        {
            get
            {
                return m_eventService;
            }
        }

        public BrainCloudEvent GetEventService()
        {
            return this.m_eventService;
        }

        public BrainCloudSocialLeaderboard SocialLeaderboardService
        {
            get
            {
                return m_socialLeaderboardService;
            }
        }

        public BrainCloudSocialLeaderboard GetSocialLeaderboardService()
        {
            return this.m_socialLeaderboardService;
        }

        public BrainCloudAsyncMatch AsyncMatchService
        {
            get
            {
                return this.m_asyncMatchService;
            }
        }

        public BrainCloudAsyncMatch GetAsyncMatchService()
        {
            return m_asyncMatchService;
        }

        public BrainCloudTime TimeService
        {
            get
            {
                return m_timeService;
            }
        }

        public BrainCloudTime GetTimeService()
        {
            return this.m_timeService;
        }

        public BrainCloudAuthentication AuthenticationService
        {
            get
            {
                return m_authenticationService;
            }
        }

        public BrainCloudAuthentication GetAuthenticationService()
        {
            return this.m_authenticationService;
        }

        public BrainCloudTwitter TwitterService
        {
            get
            {
                return m_twitterService;
            }
        }

        public BrainCloudTwitter GetTwitterService()
        {
            return this.m_twitterService;
        }

        public BrainCloudPushNotification PushNotificationService
        {
            get
            {
                return m_pushNotificationService;
            }
        }

        public BrainCloudPushNotification GetPushNotificationService()
        {
            return this.m_pushNotificationService;
        }

        public BrainCloudPlayerStatisticsEvent PlayerStatisticsEventService
        {
            get
            {
                return m_playerStatisticsEventService;
            }
        }

        public BrainCloudPlayerStatisticsEvent GetPlayerStatisticsEventService()
        {
            return this.m_playerStatisticsEventService;
        }

        public bool Authenticated
        {
            get
            {
                return m_bc.Authenticated;    //no public "set"
            }
        }

        public bool Initialized
        {
            get
            {
                return m_initialized;
            }
        }

        public string SessionID
        {
            get
            {
                if (m_bc != null)
                {
                    return m_bc.SessionID;
                }
                else
                {
                    return "";
                }
            } //no public "set"
        }
        public string GetSessionId()
        {
            return this.SessionID;
        }

        private string m_gameId = "";
        public string GameId
        {
            get
            {
                return m_gameId;    //no public "set"
            }
        }

        private string m_gameVersion = "";
        public string GameVersion
        {
            get
            {
                return m_gameVersion;    //no public "set"
            }
        }
        public string BrainCloudClientVersion
        {
            get
            {
                return Version.GetVersion();    //no public "set"
            }
        }

        private string m_releasePlatform = "";
        public string ReleasePlatform
        {
            get
            {
                return m_releasePlatform;    //no public "set"
            }
        }

        #endregion

        // InitializeClient
        // OnHeartBeat
        // ResetCommunication
        #region Miscellaneous

        /// <summary>Method initializes the BrainCloudClient.</summary>
        /// <param name="serverURL">The url to the brainCloud server</param>
        /// <param name="secretKey">The secret key for your game
        /// <param name="gameId ">The game id</param>
        /// <param name="gameVersion The game version</param>
        /// <param name="cachedProfileId The profile Id</param>
        /// <param name="anonymousId The anonymous Id</param>
        public void Initialize(string serverURL, string secretKey, string gameId, string gameVersion)
        {
            string platform = "";
#if !(DOT_NET)
            switch ( UnityEngine.Application.platform )
            {
            // web browser
            case UnityEngine.RuntimePlatform.WindowsWebPlayer:
            case UnityEngine.RuntimePlatform.OSXWebPlayer:
            {
                // this is going to change to "WEB" at some point (when we remove "FB")
                platform = Constants.PlatformFacebook;
                break;
            }

            // android
            case UnityEngine.RuntimePlatform.Android:
            {
                platform = Constants.PlatformGooglePlayAndroid;
                break;
            }

            // windows phone 8
            case UnityEngine.RuntimePlatform.WP8Player:
            {
                platform = Constants.PlatformWindowsPhone;
                break;
            }

            // mac osx
            case UnityEngine.RuntimePlatform.OSXEditor:
            case UnityEngine.RuntimePlatform.OSXPlayer:
            case UnityEngine.RuntimePlatform.OSXDashboardPlayer:
            {
                platform = Constants.PlatformOSX;
                break;
            }

            // windows desktop
            case UnityEngine.RuntimePlatform.WindowsEditor:
            case UnityEngine.RuntimePlatform.WindowsPlayer:
            {
                platform = Constants.PlatformWindows;
                break;
            }

            // ios and default
            case UnityEngine.RuntimePlatform.IPhonePlayer:
            default:
            {
                platform = Constants.PlatformIOS;
                break;
            }
            }
#else
            // TODO: what is our default c# platform?
            platform = Constants.PlatformIOS;
#endif

            // set up braincloud which does the message handling
            m_bc.Initialize(serverURL, secretKey);

            m_gameId = gameId;
            m_gameVersion = gameVersion;
            m_releasePlatform = platform;

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
            m_bc.ShutDown();
        }

        /// <summary>Update method needs to be called regularly in order
        /// to process incoming and outgoing messages.
        /// </summary>
        public void Update()
        {
            if (m_bc != null) m_bc.Update();
        }


        /// <summary> Enable logging of braincloud transactions (comms etc)</summary>
        /// <param name="in_enable">True if logging is to be enabled</param>
        public void EnableLogging(bool in_enable)
        {
            m_loggingEnabled = in_enable;
        }

        /// <summary>Allow developers to register their own log handling routine</summary>
        /// <param name="in_logDelegate">The log delegate</param>
        public void RegisterLogDelegate(LogCallback in_logDelegate)
        {
            m_logDelegate = in_logDelegate;
        }

        /// <summary>Method writes log if logging is enabled</summary>
        internal void Log(string log)
        {
            if (m_loggingEnabled)
            {
                string formattedLog = "#BCC " + log;
                lock(m_loggingMutex)
                {
                    if (m_logDelegate != null)
                    {
                        m_logDelegate(formattedLog);
                    }
                    else
                    {
#if !(DOT_NET)
                        Debug.Log(formattedLog);
#else
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
            m_bc.AddToQueue(serviceMessage);
        }

        /// <summary>Get the Server URL</summary>
        public string GetUrl()
        {
            return m_bc.ServerURL;
        }

        /// <summary>Resets all messages and calls to the server</summary>
        public void ResetCommunication()
        {
            m_bc.ResetCommunication();
        }

        /// <summary>Enable Communications with the server</summary>
        /// <param name="in_value">True to enable comms, false otherwise.</param>
        public void EnableCommunications(bool in_value)
        {
            m_bc.EnableComms(in_value);
        }

        public void SetNetworkErrorHandler(NetworkErrorHandler in_handler)
        {
            m_bc.NetworkErrorHandler = in_handler;
        }
        #endregion

        #region Authentication

        // This isn't the best confirmation of authenticated,
        // Once authenticated this is NOT removed, unless the user
        // is reset completely.
        public bool IsAuthenticated()
        {
            return this.Authenticated;
        }

        /// <summary>
        /// Returns true if brainCloud has been initialized.
        /// </summary>
        /// <returns><c>true</c> if brainCloud is initialized; otherwise, <c>false</c>.</returns>
        public bool IsInitialized()
        {
            return this.Initialized;
        }

        #endregion Authentication
    }
}
