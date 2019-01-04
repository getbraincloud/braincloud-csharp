#if !DOT_NET

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
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

        public string DispatcherURL
        {
            get
            {
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    return m_serverURL + "/dispatcherv2";
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.Instance.GetServerUrl + "/dispatcherv2";
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
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    return m_serverURL;
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.Instance.GetServerUrl;
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
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    return m_secretKey;
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.GetAppSecret();
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

        [SerializeField] private string m_gameId = "";

        
        public string AppId
        {
            get
            {
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    return m_gameId;
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.GetAppId();
            }
            set
            {
                if (m_gameId != value)
                {
                    m_gameId = value;
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

        [SerializeField] private string m_gameVersion = "1.0.0";

        public string AppVersion
        {
            get
            {
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    return m_gameVersion;
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.GetAppVersion();
            }
            set
            {
                if (m_gameVersion != value)
                {
                    m_gameVersion = value;
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
                if (BrainCloudPlugin.BrainCloudPluginSettings.IsLegacyPluginEnabled())
                {
                    Dictionary<string, string> appIdSecretsDict = AppIdSecretPair.ToDictionary(m_appIdSecrets);

                    if (!appIdSecretsDict.ContainsKey(AppId))
                    {
                        appIdSecretsDict.Add(AppId, SecretKey);
                    }
                    
                    return appIdSecretsDict;
                }

                return BrainCloudPlugin.BrainCloudPluginSettings.GetAppIdSecrets();
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