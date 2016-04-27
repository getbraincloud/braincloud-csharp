using System.Collections.Generic;
using BrainCloud;
using JsonFx.Json;

#if !DOT_NET
using UnityEngine;
using BrainCloudUnity;
#else
using System.IO;
using System.IO.IsolatedStorage;
#endif

/// <summary>
/// The BrainCloudWrapper class provides some glue between the Unity environment and the
/// brainCloud C# library. Specifically the BrainCloudWrapper does the following:
/// 
/// 1) Creates and uses a global singleton GameObject to manage it's lifetime across the game
/// 2) Provides an Initialize method which uses the game id, secret, version, and server url
///    defined in the brainCloud Settings Unity window.
/// 3) Provides a few of the authentication types that are supported by brainCloud.
/// 4) For Anonymous authentication, stores the anonymous id and profile id to the Unity player prefs
///    upon successful authentication. This is important as once an anonymous account is created,
///    both the anonymous id and profile id of the account are required to authenticate.
/// 
/// Note that this class is *not* required to use brainCloud - you are free to reimplement the
/// functionality as you see fit. It is simply used as a starting point to get developers off the
/// ground - especially with authentications.
/// 
/// The meat of the BrainCloud api is available by using
/// 
/// BrainCloudWrapper.GetBC()
/// 
/// to grab an instance of the BrainCloudClient. From here you have access to all of the brainCloud
/// API service. So for instance, to execute a read player statistics API you would do the following:
/// 
/// BrainCloudWrapper.GetBC().PlayerStatisticsService.ReadAllPlayerStats()
/// 
/// Similar services exist for other APIs.
/// 
/// See http://getbraincloud.com/apidocs/ for the full list of brainCloud APIs.
/// </summary>
#if !DOT_NET
public class BrainCloudWrapper : MonoBehaviour
#else
public class BrainCloudWrapper
#endif
{
    /// <summary>
    /// The key for the player prefs profile id
    /// </summary>
    public static string PREFS_PROFILE_ID = "brainCloud.profileId";

    /// <summary>
    /// The key for the player prefs anonymous id
    /// </summary>
    public static string PREFS_ANONYMOUS_ID = "brainCloud.anonymousId";

    /// <summary>
    /// The key for the player prefs authentication type
    /// </summary>
    public static string PREFS_AUTHENTICATION_TYPE = "brainCloud.authenticationType";

    /// <summary>
    /// The name of the singleton brainCloud game object
    /// </summary>
    public static string GAMEOBJECT_BRAINCLOUD = "BrainCloudWrapper";

    private static BrainCloudWrapper _instance = null;
    private static bool _applicationIsQuitting = false;

    private BrainCloudClient _client = null;

    private string _lastUrl = "";
    private string _lastSecretKey = "";
    private string _lastGameId = "";
    private string _lastGameVersion = "";

    private WrapperData _wrapperData = new WrapperData();

    public bool AlwaysAllowProfileSwitch { get; set; }

    public static string AUTHENTICATION_ANONYMOUS = "anonymous";

    // class handles bundling user-defined cb objects and callback methods
    private class AuthCallbackObject
    {
        public object _cbObject;
        public SuccessCallback _successCallback;
        public FailureCallback _failureCallback;
    }

    public BrainCloudWrapper()
    {
        _client = BrainCloudClient.Get();
    }

    public static BrainCloudWrapper Instance { get { return GetInstance(); } }

    public BrainCloudClient Client { get { return Instance._client; } }

    /// <summary>
    /// Gets the singleton instance of the BrainCloudWrapper.
    /// The BrainCloudWrapper object is stored in a Unity Game Object.
    /// </summary>
    /// <returns>The instance</returns>
    public static BrainCloudWrapper GetInstance()
    {
        if (_applicationIsQuitting)
        {
            return null;
        }
        if (_instance == null)
        {
#if !DOT_NET
            _instance = (BrainCloudWrapper)FindObjectOfType(typeof(BrainCloudWrapper));
            if (_instance != null)
            {
                _instance.Reauthenticate();
            }

            if (FindObjectsOfType(typeof(BrainCloudWrapper)).Length > 1)
            {
                Debug.LogError("[Singleton] Something went really wrong " +
                               " - there should never be more than 1 singleton!" +
                               " Reopenning the scene might fix it.");
                return _instance;
            }

            if (_instance == null)
            {
                GameObject go = new GameObject(GAMEOBJECT_BRAINCLOUD);
                _instance = go.AddComponent<BrainCloudWrapper>();
                DontDestroyOnLoad(go);
            }
#else
            _instance = new BrainCloudWrapper();
            _instance.LoadData();
#endif
        }
        return _instance;
    }

    public void Update()
    {
        if (_client != null)
        {
            _client.Update();
        }
    }

#if !DOT_NET
    public void OnDestroy()
    {
        _applicationIsQuitting = true;
    }
#endif

    /// <summary>
    /// Returns an instance of the BrainCloudClient. All brainCloud APIs are
    /// accessible through the client.
    /// </summary>
    /// <returns>The brainCloud client object</returns>
    public static BrainCloudClient GetBC()
    {
        return GetInstance()._client;
    }

#if !DOT_NET
    /// <summary>
    /// Initializes the brainCloud client. This method uses the parameters as configured
    /// in the Unity brainCloud Settings window.
    /// </summary>
    public static void Initialize()
    {
        Initialize(
            BrainCloudSettings.Instance.DispatcherURL,
            BrainCloudSettings.Instance.SecretKey,
            BrainCloudSettings.Instance.GameId,
            BrainCloudSettings.Instance.GameVersion);

        GetBC().EnableLogging(BrainCloudSettings.Instance.EnableLogging);
    }
#endif

    /// <summary>
    /// Initialize the brainCloud client with the passed in parameters. This version of Initialize
    /// overrides the parameters configured in the Unity brainCloud Settings window.
    /// </summary>
    /// <param name="in_url">The brainCloud server url</param>
    /// <param name="in_secretKey">The game's secret</param>
    /// <param name="in_gameId">The game's id</param>
    /// <param name="in_gameVersion">The game's version</param>
    public static void Initialize(string in_url, string in_secretKey, string in_gameId, string in_gameVersion)
    {
        BrainCloudWrapper bcw = GetInstance();
        bcw._lastUrl = in_url;
        bcw._lastSecretKey = in_secretKey;
        bcw._lastGameId = in_gameId;
        bcw._lastGameVersion = in_gameVersion;
        bcw._client.Initialize(in_url, in_secretKey, in_gameId, in_gameVersion);

        _instance.LoadData();
    }

    /// <summary>
    /// If set to true, profile id is never sent along with non-anonymous authenticates
    /// thereby ensuring that valid credentials always work but potentially cause a profile switch.
    /// If set to false, profile id is passed to the server (if it has been stored) and a profile id
    /// to non-anonymous credential mismatch will cause an error.
    /// </summary>
    /// <param name="in_enabled">True if we always allow profile switch</param>
    public void SetAlwaysAllowProfileSwitch(bool in_enabled)
    {
        AlwaysAllowProfileSwitch = in_enabled;
    }

    #region Authenticate Methods

    /// <summary>
    /// Authenticate a user anonymously with brainCloud - used for apps that don't want to bother
    /// the user to login, or for users who are sensitive to their privacy
    /// 
    /// Note that this method is special in that the anonymous id and profile id
    /// are persisted to the Unity player prefs cache if authentication is successful.
    /// Both pieces of information are required to successfully log into that account
    /// once the player has been created. Failure to store the profile id and anonymous id
    /// once the player has been created results in an inability to log into that account!
    /// For this reason, using other recoverable authentication methods (like email/password, Facebook)
    /// are encouraged.
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateAnonymous(
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity(true);

        _client.AuthenticationService.AuthenticateAnonymous(
            true, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user with a custom Email and Password.  Note that the client app
    /// is responsible for collecting (and storing) the e-mail and potentially password
    /// (for convenience) in the client data.  For the greatest security,
    /// force the user to re-enter their password at each login.
    /// (Or at least give them that option).
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    ///
    /// Note that the password sent from the client to the server is protected via SSL.
    /// </remarks>
    /// <param name="in_email">
    /// The e-mail address of the user
    /// </param>
    /// <param name="in_password">
    /// The password of the user
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateEmailPassword(
        string in_email,
        string in_password,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateEmailPassword(
            in_email, in_password, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user via cloud code (which in turn validates the supplied credentials against an external system).
    /// This allows the developer to extend brainCloud authentication to support other backend authentication systems.
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_userid">
    /// The user id
    /// </param>
    /// <param name="in_token">
    /// The user token (password etc)
    /// </param>
    /// /// <param name="in_externalAuthName">
    /// The name of the cloud script to call for external authentication
    /// </param>
    /// <param name="forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateExternal(
        string in_userid,
        string in_token,
        string in_externalAuthName,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateExternal(
            in_userid, in_token, in_externalAuthName, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user with brainCloud using their Facebook Credentials
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="externalId">
    /// The facebook id of the user
    /// </param>
    /// <param name="authenticationToken">
    /// The validated token from the Facebook SDK (that will be further
    /// validated when sent to the bC service)
    /// </param>
    /// <param name="forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateFacebook(
        string in_fbUserId,
        string in_fbAuthToken,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateFacebook(
            in_fbUserId, in_fbAuthToken, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user using their Game Center id
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_gameCenterId">
    /// The player's game center id  (use the playerID property from the local GKPlayer object)
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateGameCenter(
        string in_gameCenterId,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateGameCenter(
            in_gameCenterId, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user using a google userid(email address) and google authentication token.
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_userid">
    /// String representation of google+ userid (email)
    /// </param>
    /// <param name="in_token">
    /// The authentication token derived via the google apis.
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateGoogle(
        string in_userid,
        string in_token,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateGoogle(
            in_userid, in_token, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user using a steam userid and session ticket (without any validation on the userid).
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_userid">
    /// String representation of 64 bit steam id
    /// </param>
    /// <param name="in_sessionticket">
    /// The session ticket of the user (hex encoded)
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateSteam(
        string in_userid,
        string in_sessionticket,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateSteam(
            in_userid, in_sessionticket, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user using a Twitter userid, authentication token, and secret from twitter.
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_userid">
    /// String representation of a Twitter user ID
    /// </param>
    /// <param name="in_token">
    /// The authentication token derived via the Twitter apis
    /// </param>
    /// <param name="in_secret">
    /// The secret given when attempting to link with Twitter
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param>
    public void AuthenticateTwitter(
        string in_userid,
        string in_token,
        string in_secret,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateTwitter(
            in_userid, in_token, in_secret, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }

    /// <summary>
    /// Authenticate the user using a userid and password (without any validation on the userid).
    /// Similar to AuthenticateEmailPassword - except that that method has additional features to
    /// allow for e-mail validation, password resets, etc.
    /// </summary>
    /// <remarks>
    /// Service Name - Authenticate
    /// Service Operation - Authenticate
    /// </remarks>
    /// <param name="in_email">
    /// The e-mail address of the user
    /// </param>
    /// <param name="in_password">
    /// The password of the user
    /// </param>
    /// <param name="in_forceCreate">
    /// Should a new profile be created for this user if the account does not exist?
    /// </param>
    /// <param name="in_success">
    /// The method to call in event of successful login
    /// </param>
    /// <param name="in_failure">
    /// The method to call in the event of an error during authentication
    /// </param>
    /// <param name="in_cbObject">
    /// The user supplied callback object
    /// </param> 
    public void AuthenticateUniversal(
        string in_username,
        string in_password,
        bool in_forceCreate,
        SuccessCallback in_success = null,
        FailureCallback in_failure = null,
        object in_cbObject = null)
    {
        AuthCallbackObject aco = new AuthCallbackObject();
        aco._successCallback = in_success;
        aco._failureCallback = in_failure;
        aco._cbObject = in_cbObject;

        InitializeIdentity();

        _client.AuthenticationService.AuthenticateUniversal(
            in_username, in_password, in_forceCreate, AuthSuccessCallback, AuthFailureCallback, aco);
    }


    /// <summary>
    /// Method initializes the identity information from the Unity player prefs cache.
    /// This is specifically useful for an Anonymous authentication as Anonymous authentications
    /// require both the anonymous id *and* the profile id. By using the BrainCloudWrapper
    /// AuthenticateAnonymous method, a success callback handler hook will be installed
    /// that will trap the return from the brainCloud server and persist the anonymous id
    /// and profile id.
    /// 
    /// Note that clients are free to implement this logic on their own as well if they 
    /// wish to store the information in another location and/or change the behaviour.
    /// </summary>
    protected virtual void InitializeIdentity(bool in_isAnonymousAuth = false)
    {
        // retrieve profile and anonymous ids out of the cache
        string profileId = GetStoredProfileId();
        string anonymousId = GetStoredAnonymousId();

        if ((anonymousId != "" && profileId == "") || anonymousId == "")
        {
            anonymousId = _client.AuthenticationService.GenerateGUID();
            profileId = "";
            SetStoredAnonymousId(anonymousId);
            SetStoredProfileId(profileId);
        }
        string profileIdToAuthenticateWith = profileId;
        if (!in_isAnonymousAuth && AlwaysAllowProfileSwitch)
        {
            profileIdToAuthenticateWith = "";
        }
        SetStoredAuthenticationType(in_isAnonymousAuth ? AUTHENTICATION_ANONYMOUS : "");
        _client.InitializeIdentity(profileIdToAuthenticateWith, anonymousId);
    }

    #endregion

    /// <summary>
    /// Gets the stored profile id from player prefs.
    /// </summary>
    /// <returns>The stored profile id.</returns>
    public virtual string GetStoredProfileId()
    {
        return _wrapperData.ProfileId;
    }

    /// <summary>
    /// Sets the stored profile id to player prefs.
    /// </summary>
    /// <param name="in_profileId">Profile id.</param>
    public virtual void SetStoredProfileId(string in_profileId)
    {
        _wrapperData.ProfileId = in_profileId;
        SaveData();
    }

    /// <summary>
    /// Resets the stored profile id to empty string.
    /// </summary>
    public virtual void ResetStoredProfileId()
    {
        _wrapperData.ProfileId = "";
        SaveData();
    }

    /// <summary>
    /// Gets the stored anonymous id from player prefs.
    /// </summary>
    /// <returns>The stored anonymous id.</returns>
    public virtual string GetStoredAnonymousId()
    {
        return _wrapperData.AnonymousId;
    }

    /// <summary>
    /// Sets the stored anonymous id to player prefs.
    /// </summary>
    /// <param name="in_anonymousId">Anonymous id</param>
    public virtual void SetStoredAnonymousId(string in_anonymousId)
    {
        _wrapperData.AnonymousId = in_anonymousId;
        SaveData();
    }

    /// <summary>
    /// Resets the stored anonymous id to empty string.
    /// </summary>
    public virtual void ResetStoredAnonymousId()
    {
        _wrapperData.AnonymousId = "";
        SaveData();
    }

    /// <summary>
    /// Gets the type of the stored authentication.
    /// </summary>
    /// <returns>The stored authentication type.</returns>
    public virtual string GetStoredAuthenticationType()
    {
        return _wrapperData.AuthenticationType;
    }

    /// <summary>
    /// Sets the type of the stored authentication.
    /// </summary>
    /// <param name="in_authenticationType">Authentication type.</param>
    public virtual void SetStoredAuthenticationType(string in_authenticationType)
    {
        _wrapperData.AuthenticationType = in_authenticationType;
        SaveData();
    }

    /// <summary>
    /// Resets the type of the stored authentication to empty string
    /// </summary>
    public virtual void ResetStoredAuthenticationType()
    {
        _wrapperData.AuthenticationType = "";
        SaveData();
    }

    /// <summary>
    /// Provides a way to reauthenticate with the stored anonymous and profile id.
    /// Only works for Anonymous authentications.
    /// </summary>
    protected virtual void Reauthenticate()
    {
        Initialize(_instance._lastUrl, _instance._lastSecretKey, _instance._lastGameId, _instance._lastGameVersion);
        string authType = GetStoredAuthenticationType();
        if (authType == AUTHENTICATION_ANONYMOUS)
        {
            AuthenticateAnonymous(null, null);
        }
    }

    /// <summary>
    /// Callback for authentication success using the BrainCloudWrapper class.
    /// </summary>
    /// <param name="json">The returned json</param>
    /// <param name="cbObject">The returned callback object</param>
    protected virtual void AuthSuccessCallback(string json, object cbObject)
    {
        // grab the profileId and save it in PlayerPrefs
        Dictionary<string, object> jsonMessage = (Dictionary<string, object>)JsonReader.Deserialize(json);
        Dictionary<string, object> jsonData = (Dictionary<string, object>)jsonMessage["data"];
        string profileId = "";
        if (jsonData.ContainsKey("profileId"))
        {
            profileId = (string)jsonData["profileId"];
        }
        if (profileId != "")
        {
            SetStoredProfileId(profileId);
        }
        if (cbObject != null)
        {
            AuthCallbackObject aco = (AuthCallbackObject)cbObject;
            if (aco._successCallback != null)
            {
                aco._successCallback(json, aco._cbObject);
            }
        }
    }

    /// <summary>
    /// Callback for authentication failure using the BrainCloudWrapper class.
    /// </summary>
    /// <param name="statusCode">The status code</param>
    /// <param name="reasonCode">The reason code</param>
    /// <param name="errorJson">The error json</param>
    /// <param name="cbObject">The returned callback object</param>
    protected virtual void AuthFailureCallback(int statusCode, int reasonCode, string errorJson, object cbObject)
    {
        if (cbObject != null)
        {
            AuthCallbackObject aco = (AuthCallbackObject)cbObject;
            if (aco._failureCallback != null)
            {
                aco._failureCallback(statusCode, reasonCode, errorJson, aco._cbObject);
            }
        }
    }

    private void SaveData()
    {
#if DOT_NET
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(WrapperData.FileName, FileMode.OpenOrCreate, isoStore))
        {
            using (StreamWriter writer = new StreamWriter(isoStream))
            {
                string file = JsonWriter.Serialize(_wrapperData);
                writer.WriteLine(file);
            }
        }
#else
        PlayerPrefs.SetString(PREFS_PROFILE_ID, _wrapperData.ProfileId);
        PlayerPrefs.SetString(PREFS_ANONYMOUS_ID, _wrapperData.AnonymousId);
        PlayerPrefs.SetString(PREFS_AUTHENTICATION_TYPE, _wrapperData.AuthenticationType);
#endif
    }

    private void LoadData()
    {
#if DOT_NET
        IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        if (isoStore.FileExists(WrapperData.FileName))
        {
            string file;

            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(WrapperData.FileName, FileMode.Open, isoStore))
            {
                using (StreamReader reader = new StreamReader(isoStream))
                {
                    file = reader.ReadToEnd();
                }
            }

            //parse
            _wrapperData = JsonReader.Deserialize<WrapperData>(file);
        }
#else
        _wrapperData.ProfileId = PlayerPrefs.GetString(PREFS_PROFILE_ID);
        _wrapperData.AnonymousId = PlayerPrefs.GetString(PREFS_ANONYMOUS_ID);
        _wrapperData.AuthenticationType = PlayerPrefs.GetString(PREFS_AUTHENTICATION_TYPE);
#endif
    }

    private class WrapperData
    {
        public string ProfileId = "";
        public string AnonymousId = "";
        public string AuthenticationType = "";

        public static readonly string FileName = "BrainCloudWrapper.json";
    }
}
