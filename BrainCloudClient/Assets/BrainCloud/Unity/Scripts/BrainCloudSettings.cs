#if !DOT_NET

using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using BrainCloudUnity.BrainCloudPlugin;
#endif

namespace BrainCloudUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    
    public class BrainCloudSettings : ScriptableObject
    {
#if UNITY_EDITOR
        static BrainCloudSettings()
        {
            
        }
        

        
        public void OnEnable()
        {
            BrainCloudDebugInfo.Instance.ClearPluginData();

            
            BaseBrainCloudPluginSettings.Instance.BrainCloudPluginUpdated += UpdatePlugin;

        }

        private void UpdatePlugin()
        {
            if (!BrainCloudPluginSettings.IsLegacyPluginEnabled())
            {
                Instance.m_dispatchUrl = BrainCloudPluginSettings.Instance.GetServerUrl + "/dispatcherv2";
                Instance.m_serverURL = BrainCloudPluginSettings.Instance.GetServerUrl;   
                Instance.m_secretKey = BaseBrainCloudPluginSettings.GetAppSecret();
                Instance.m_appId = BaseBrainCloudPluginSettings.GetAppId();
                Instance.m_appVersion = BaseBrainCloudPluginSettings.GetAppVersion();

                var appIdSecrets = BaseBrainCloudPluginSettings.GetAppIdSecrets();
                    
                if(appIdSecrets != null)
                    Instance.m_appIdSecrets =
                        AppIdSecretPair.FromDictionary(BaseBrainCloudPluginSettings.GetAppIdSecrets());
   
                EditorUtility.SetDirty(Instance);
                
                EditorUtility.SetDirty(Resources.Load("BrainCloudSettings") as BrainCloudSettings);
            }
        }

        private void OnDisable()
        {
            BaseBrainCloudPluginSettings.Instance.BrainCloudPluginUpdated -=  UpdatePlugin;
        }
#endif
        
        private static BrainCloudSettings s_instance;

        public static BrainCloudSettings Instance
        {
            get
            {
                if (s_instance) return s_instance;

                s_instance = Resources.Load("BrainCloudSettings") as BrainCloudSettings;
                if (s_instance == null)
                {
                    // If not found, autocreate the asset object.
                    s_instance = CreateInstance<BrainCloudSettings>();

#if UNITY_EDITOR
                    string properPath = Path.Combine(Application.dataPath, "BrainCloud");
                    if (!Directory.Exists(properPath))
                    {
                        AssetDatabase.CreateFolder("Assets", "BrainCloud");
                    }
                    properPath = Path.Combine(Application.dataPath, "BrainCloud/Resources");
                    if (!Directory.Exists(properPath))
                    {
                        AssetDatabase.CreateFolder("Assets/BrainCloud", "Resources");
                    }
                    string fullPath = "Assets/BrainCloud/Resources/BrainCloudSettings.asset";
                    AssetDatabase.CreateAsset(s_instance, fullPath);
#endif
                }
                s_instance.name = "BrainCloudSettings";
                return s_instance;
            }
        }

        [SerializeField] private string m_dispatchUrl = "";
        public string DispatcherURL
        {
            get
            {  
#if UNITY_EDITOR
                if (BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_dispatchUrl = m_serverURL + "/dispatcherv2";
                }
#endif

                return m_dispatchUrl;
            }
        }

        public string PortalURL
        {
            get { return "https://portal.braincloudservers.com"; }
        }

        public string ApiDocsURL
        {
            get { return "http://getbraincloud.com/apidocs"; }
        }

        // Settings
        public const string DEFAULT_BRAINCLOUD_URL = "https://sharedprod.braincloudservers.com";

        [SerializeField] private string m_serverURL = DEFAULT_BRAINCLOUD_URL;

        public string ServerURL
        {
            get
            {
                return m_serverURL;
            }
            set
            {
                if (m_serverURL != value)
                {
                    m_serverURL = value;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }

        [SerializeField] private string m_secretKey = "";

        public string SecretKey
        {
            get
            {
                return m_secretKey;
            }
            set
            {
                if (m_secretKey != value)
                {
                    m_secretKey = value;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }

        [FormerlySerializedAs("m_gameId")] [SerializeField] private string m_appId = "";
        
        public string AppId
        {
            get
            {
                return m_appId;
            }
            set
            {
                if (m_appId != value)
                {
                    m_appId = value;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }
        
        public string GameId
        {
            get { return AppId; }
            set { AppId = value; }
        }

        [FormerlySerializedAs("m_gameVersion")] [SerializeField] private string m_appVersion = "1.0.0";

        public string AppVersion
        {
            get
            { 
                return m_appVersion;
            }
            set
            {
                if (m_appVersion != value)
                {
                    m_appVersion = value;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }

        [SerializeField] private AppIdSecretPair[] m_appIdSecrets;

        public Dictionary<string, string> AppIdSecrets
        {
            get
            {
                Dictionary<string, string> appIdSecretsDict = AppIdSecretPair.ToDictionary(m_appIdSecrets);
                    
                if (!appIdSecretsDict.ContainsKey(AppId))
                {
                    appIdSecretsDict.Add(AppId, SecretKey);
                }
 
                return appIdSecretsDict;
            }
            set
            {
                    m_appIdSecrets = AppIdSecretPair.FromDictionary(value);
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                
            }
        }
        
        public string GameVersion
        {
            get { return AppVersion; }
            set { AppVersion = value; }
        }

        [SerializeField] private bool m_enableLogging = false;

        public bool EnableLogging
        {
            get { return m_enableLogging; }
            set
            {
                if (m_enableLogging != value)
                {
                    m_enableLogging = value;
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
            }
        }
    }
}

#endif