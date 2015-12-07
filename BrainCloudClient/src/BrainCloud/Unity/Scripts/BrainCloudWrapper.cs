using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using JsonFx.Json;

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
/// ground - especially with anonymous authentications.
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
public class BrainCloudWrapper : MonoBehaviour
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
    
    private static BrainCloudWrapper s_instance = null;
    private static bool s_applicationIsQuitting = false;
    private static object s_lock = new object();
    
    private BrainCloudClient m_client = null;
    
    private string m_lastUrl = "";
    private string m_lastSecretKey = "";
    private string m_lastGameId = "";
    private string m_lastGameVersion = "";
    
    public static string AUTHENTICATION_ANONYMOUS = "anonymous";
    public static string AUTHENTICATION_FACEBOOK = "facebook";
    
    public BrainCloudWrapper()
    {
        m_client = BrainCloudClient.Get ();
    }
    
    /// <summary>
    /// Gets the singleton instance of the BrainCloudWrapper.
    /// The BrainCloudWrapper object is stored in a Unity Game Object.
    /// </summary>
    /// <returns>The instance.</returns>
    public static BrainCloudWrapper GetInstance()
    {
        if (s_applicationIsQuitting)
        {
            return null;
        }
        
        lock (s_lock)
        {
            if (s_instance == null)
            {
                
                s_instance = (BrainCloudWrapper) FindObjectOfType(typeof(BrainCloudWrapper));
                if (s_instance != null)
                {
                    s_instance.Reauthenticate();
                }
                
                if ( FindObjectsOfType(typeof(BrainCloudWrapper)).Length > 1 )
                {
                    Debug.LogError("[Singleton] Something went really wrong " +
                                   " - there should never be more than 1 singleton!" +
                                   " Reopenning the scene might fix it.");
                    return s_instance;
                }
                
                if (s_instance == null)
                {
                    GameObject go = new GameObject(GAMEOBJECT_BRAINCLOUD);
                    s_instance = go.AddComponent<BrainCloudWrapper>();
                    DontDestroyOnLoad(go);
                }
            }
            return s_instance;
        }
    }
    
    void Start()
    {
    }
    
    void Update()
    {
        if (m_client != null)
        {
            m_client.Update();
        }
    }
    
    /// <summary>
    /// Returns an instance of the BrainCloudClient. All brainCloud APIs are
    /// accessible through the client.
    /// </summary>
    /// <returns>The brainCloud client object</returns>
    public static BrainCloudClient GetBC()
    {
        return GetInstance().m_client;
    }
    
    /// <summary>
    /// Initializes the brainCloud client. This method uses the parameters as configured
    /// in the Unity brainCloud Settings window.
    /// </summary>
    public static void Initialize()
    {
        Initialize (
            BrainCloudSettings.Instance.DispatcherURL,
            BrainCloudSettings.Instance.SecretKey,
            BrainCloudSettings.Instance.GameId,
            BrainCloudSettings.Instance.GameVersion);
        
        GetBC().EnableLogging(BrainCloudSettings.Instance.EnableLogging);
    }
    
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
        bcw.m_lastUrl = in_url;
        bcw.m_lastSecretKey = in_secretKey;
        bcw.m_lastGameId = in_gameId;
        bcw.m_lastGameVersion = in_gameVersion;
        bcw.m_client.Initialize(in_url, in_secretKey, in_gameId, in_gameVersion);
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
    protected virtual void InitializeIdentity()
    {
        // retrieve profile and anonymous ids out of the cache
        string profileId = GetStoredProfileId();
        string anonymousId = GetStoredAnonymousId();
        
        if ((anonymousId != "" && profileId == "") || anonymousId == "")
        {
            anonymousId = m_client.AuthenticationService.GenerateGUID();
            SetStoredAnonymousId(anonymousId);
        }
        m_client.InitializeIdentity(profileId, anonymousId);
    }
    
    /// <summary>
    /// Provides a way to reauthenticate with the stored anonymous and profile id.
    /// Only works for Anonymous authentications.
    /// </summary>
    protected virtual void Reauthenticate()
    {
        BrainCloudWrapper.Initialize(s_instance.m_lastUrl, s_instance.m_lastSecretKey, s_instance.m_lastGameId, s_instance.m_lastGameVersion);
        string authType = GetStoredAuthenticationType();
        if (authType == AUTHENTICATION_ANONYMOUS)
        {
            AuthenticateAnonymous(null, null);
        }
    }
    
    /// <summary>
    /// Method authenticates with brainCloud using Universal credentials
    /// </summary>
    /// <param name="in_username">The user name</param>
    /// <param name="in_password">The password</param>
    /// <param name="in_forceCreate">If set to <c>true</c>, attempt to create the user if they do not already exist.</param>
    /// <param name="in_successCb">The success callback</param>
    /// <param name="in_failureCb">The failure callback</param>
    public void AuthenticateUniversal(string in_username, string in_password, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
    {
        m_client.AuthenticationService.AuthenticateUniversal(in_username, in_password, in_forceCreate, in_successCb, in_failureCb);
    }
    
    /// <summary>
    /// Method authenticates with brainCloud using Email/Password credentials
    /// </summary>
    /// <param name="in_email">The email</param>
    /// <param name="in_password">The password</param>
    /// <param name="in_forceCreate">If set to <c>true</c>, attempt to create the user if they do not already exist.</param>
    /// <param name="in_successCb">The success callback</param>
    /// <param name="in_failureCb">The failure callback</param>
    public void AuthenticateEmailPassword(string in_email, string in_password, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
    {
        m_client.AuthenticationService.AuthenticateEmailPassword(in_email, in_password, in_forceCreate, in_successCb, in_failureCb);
    }
    
    /// <summary>
    /// Method authenticates with brainCloud using Facebook credentials
    /// </summary>
    /// <param name="in_fbUserId">The facebook user id</param>
    /// <param name="in_fbAuthToken">The facebook authentication token</param>
    /// <param name="in_forceCreate">If set to <c>true</c>, attempt to create the user if they do not already exist.</param>
    /// <param name="in_successCb">The success callback</param>
    /// <param name="in_failureCb">The failure callback</param>
    public void AuthenticateFacebook(string in_fbUserId, string in_fbAuthToken, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
    {
        m_client.AuthenticationService.AuthenticateFacebook(in_fbUserId, in_fbAuthToken, in_forceCreate, in_successCb, in_failureCb);
    }
    
    /// <summary>
    /// Method authenticates with brainCloud using Anonymous credentials.
    /// 
    /// Note that this method is special in that the anonymous id and profile id
    /// are persisted to the Unity player prefs cache if authentication is successful.
    /// Both pieces of information are required to successfully log into that account
    /// once the player has been created. Failure to store the profile id and anonymous id
    /// once the player has been created results in an inability to log into that account!
    /// For this reason, using other recoverable authentication methods (like email/password, Facebook)
    /// are encouraged.
    /// </summary>
    /// <param name="in_successCb">The success callback</param>
    /// <param name="in_failureCb">The failure callback</param>
    public void AuthenticateAnonymous(SuccessCallback in_successCb, FailureCallback in_failureCb)
    {
        InitializeIdentity();
        SetStoredAuthenticationType(AUTHENTICATION_ANONYMOUS);
        
        m_client.AuthenticationService.AuthenticateAnonymous(true,
                                                             // success
                                                             (delegate(string json, object cbObject)
         {
            // suck in the profileId and save it in PlayerPrefs
            Dictionary<string, object> jsonMessage = (Dictionary<string, object>) JsonReader.Deserialize(json);
            Dictionary<string, object> jsonData = (Dictionary<string, object>) jsonMessage["data"];
            string profileId = "";
            if (jsonData.ContainsKey("profileId"))
            {
                profileId = (string) jsonData["profileId"];   
            }
            if (profileId != "")
            {
                SetStoredProfileId(profileId);
            }
            if (in_successCb != null)
            {
                in_successCb(json, cbObject);
            }
        }),
                                                             // failure
                                                             (delegate(int statusCode, int reasonCode, string statusMessage, object cbObject)
         {
            if (in_failureCb != null)
            {
                in_failureCb(statusCode, reasonCode, statusMessage, cbObject);
            }
        })
                                                             );
    }
    
    
    public void OnDestroy ()
    {
        s_applicationIsQuitting = true;
    }
    
    /// <summary>
    /// Gets the stored profile id from player prefs.
    /// </summary>
    /// <returns>The stored profile id.</returns>
    public virtual string GetStoredProfileId()
    {
        return PlayerPrefs.GetString(PREFS_PROFILE_ID);
    }
    
    /// <summary>
    /// Sets the stored profile id to player prefs.
    /// </summary>
    /// <param name="in_profileId">Profile id.</param>
    public virtual void SetStoredProfileId(string in_profileId)
    {
        PlayerPrefs.SetString(PREFS_PROFILE_ID, in_profileId);
    }
    
    /// <summary>
    /// Resets the stored profile id to empty string.
    /// </summary>
    public virtual void ResetStoredProfileId()
    {
        SetStoredProfileId("");
    }
    
    /// <summary>
    /// Gets the stored anonymous id from player prefs.
    /// </summary>
    /// <returns>The stored anonymous id.</returns>
    public virtual string GetStoredAnonymousId()
    {
        return PlayerPrefs.GetString(PREFS_ANONYMOUS_ID);
    }
    
    /// <summary>
    /// Sets the stored anonymous id to player prefs.
    /// </summary>
    /// <param name="in_anonymousId">Anonymous id</param>
    public virtual void SetStoredAnonymousId(string in_anonymousId)
    {
        PlayerPrefs.SetString(PREFS_ANONYMOUS_ID, in_anonymousId);
    }
    
    /// <summary>
    /// Resets the stored anonymous id to empty string.
    /// </summary>
    public virtual void ResetStoredAnonymousId()
    {
        SetStoredAnonymousId("");
    }
    
    /// <summary>
    /// Gets the type of the stored authentication.
    /// </summary>
    /// <returns>The stored authentication type.</returns>
    public virtual string GetStoredAuthenticationType()
    {
        return PlayerPrefs.GetString(PREFS_AUTHENTICATION_TYPE);
    }
    
    /// <summary>
    /// Sets the type of the stored authentication.
    /// </summary>
    /// <param name="in_authenticationType">Authentication type.</param>
    public virtual void SetStoredAuthenticationType(string in_authenticationType)
    {
        PlayerPrefs.SetString(PREFS_AUTHENTICATION_TYPE, in_authenticationType);
    }
    
    /// <summary>
    /// Resets the type of the stored authentication to empty string
    /// </summary>
    public virtual void ResetStoredAuthenticationType()
    {
        SetStoredAuthenticationType("");
    }
}
