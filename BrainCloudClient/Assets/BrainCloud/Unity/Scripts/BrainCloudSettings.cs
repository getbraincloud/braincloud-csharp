#if !DOT_NET

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace BrainCloudUnity
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    
    public class BrainCloudSettings : ScriptableObject
    {
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
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_dispatchUrl = m_serverURL + "/dispatcherv2";
                }
                else
                {
                    m_dispatchUrl = BrainCloudPlugin.BrainCloudPluginSettings.Instance.GetServerUrl + "/dispatcherv2";
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
#if UNITY_EDITOR
                if (!BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_serverURL = BrainCloudPlugin.BrainCloudPluginSettings.Instance.GetServerUrl;
                }
#endif

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
#if UNITY_EDITOR
                if (!BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_secretKey = BrainCloudPlugin.BaseBrainCloudPluginSettings.GetAppSecret();
                }                                
#endif

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
                
#if UNITY_EDITOR
                if (!BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_appId = BrainCloudPlugin.BaseBrainCloudPluginSettings.GetAppId();
                }
#endif 
                
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
#if UNITY_EDITOR
                if (!BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    m_appVersion = BrainCloudPlugin.BaseBrainCloudPluginSettings.GetAppVersion();
                }
#endif                    
                
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
#if UNITY_EDITOR
                if (!BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {

                    m_appIdSecrets =
                        AppIdSecretPair.FromDictionary(BrainCloudPlugin.BaseBrainCloudPluginSettings.GetAppIdSecrets());
                }
#endif

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