using UnityEngine;
using System.Collections;
using BrainCloud;
using LitJson;

public class BrainCloudWrapper : MonoBehaviour
{
    public static string PREFS_PROFILE_ID = "brainCloud.profileId";
    public static string PREFS_ANONYMOUS_ID = "brainCloud.anonymousId";
    public static string PREFS_AUTHENTICATION_TYPE = "brainCloud.authenticationType";
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
        m_client = new BrainCloudClient();
    }

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
    
    public static BrainCloudClient GetBC()
    {
        return GetInstance().m_client;
    }


	public static void Initialize()
	{
		Initialize (
			BrainCloudSettings.Instance.DispatcherURL,
			BrainCloudSettings.Instance.SecretKey,
			BrainCloudSettings.Instance.GameId,
			BrainCloudSettings.Instance.GameVersion);
	}

	public static void Initialize(string in_url, string in_secretKey, string in_gameId, string in_gameVersion)
    {
        BrainCloudWrapper bcw = GetInstance();
		bcw.m_lastUrl = in_url;
		bcw.m_lastSecretKey = in_secretKey;
		bcw.m_lastGameId = in_gameId;
		bcw.m_lastGameVersion = in_gameVersion;
		bcw.m_client.Initialize(in_url, in_secretKey, in_gameId, in_gameVersion);
    }

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

    protected virtual void Reauthenticate()
    {
        BrainCloudWrapper.Initialize(s_instance.m_lastUrl, s_instance.m_lastSecretKey, s_instance.m_lastGameId, s_instance.m_lastGameVersion);
        string authType = GetStoredAuthenticationType();
        if (authType == AUTHENTICATION_ANONYMOUS)
        {
            AuthenticateAnonymous(null, null);
        }
    }

	public void AuthenticateUniversal(string in_username, string in_password, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
	{
		m_client.AuthenticationService.AuthenticateUniversal(in_username, in_password, in_forceCreate, in_successCb, in_failureCb);
	}

	public void AuthenticateEmailPassword(string in_username, string in_password, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
	{
		m_client.AuthenticationService.AuthenticateEmailPassword(in_username, in_password, in_forceCreate, in_successCb, in_failureCb);
	}

	public void AuthenticateFacebook(string in_fbUserId, string in_fbAuthToken, bool in_forceCreate, SuccessCallback in_successCb, FailureCallback in_failureCb)
	{
		m_client.AuthenticationService.AuthenticateFacebook(in_fbUserId, in_fbAuthToken, in_forceCreate, in_successCb, in_failureCb);
	}

    public void AuthenticateAnonymous(SuccessCallback in_successCb, FailureCallback in_failureCb)
    {
        InitializeIdentity();
        SetStoredAuthenticationType(AUTHENTICATION_ANONYMOUS);

        m_client.AuthenticationService.AuthenticateAnonymous(true,
            // success
            (delegate(string json, object cbObject)
            {
                // suck in the profileId and save it in PlayerPrefs
                JsonData jObj = JsonMapper.ToObject(new JsonReader(json));
                string profileId = (string)jObj["data"]["profileId"];
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


    public virtual string GetStoredProfileId()
    {
        return PlayerPrefs.GetString(PREFS_PROFILE_ID);
    }

    public virtual void SetStoredProfileId(string in_profileId)
    {
        PlayerPrefs.SetString(PREFS_PROFILE_ID, in_profileId);
    }

    public virtual void ResetStoredProfileId()
    {
        SetStoredProfileId("");
    }

    public virtual string GetStoredAnonymousId()
    {
        return PlayerPrefs.GetString(PREFS_ANONYMOUS_ID);
    }
    public virtual void SetStoredAnonymousId(string in_profileId)
    {
        PlayerPrefs.SetString(PREFS_ANONYMOUS_ID, in_profileId);
    }

    public virtual void ResetStoredAnonymousId()
    {
        SetStoredAnonymousId("");
    }

    public virtual string GetStoredAuthenticationType()
    {
        return PlayerPrefs.GetString(PREFS_AUTHENTICATION_TYPE);
    }

    public virtual void SetStoredAuthenticationType(string in_authenticationType)
    {
        PlayerPrefs.SetString(PREFS_AUTHENTICATION_TYPE, in_authenticationType);
    }

    public virtual void ResetStoredAuthenticationType()
    {
        SetStoredAuthenticationType("");
    }
}
