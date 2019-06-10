
namespace BrainCloudUnity
{

#if !DOT_NET
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
using BrainCloudUnity.BrainCloudSettingsDLL;
#endif

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class BrainCloudSettingsManual : ScriptableObject
    {
        private static BrainCloudSettingsManual s_instance;

        public static BrainCloudSettingsManual Instance
        {
            get
            {
                if (s_instance) return s_instance;

                s_instance = Resources.Load("BrainCloudSettingsManual") as BrainCloudSettingsManual;
                if (s_instance == null)
                {
                    // If not found, auto create the asset object.
                    s_instance = CreateInstance<BrainCloudSettingsManual>();

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

                    /**
                     * Handling name update for 3.11.2 patch. Where the "Plugin" text was removed from BrainCloudSettings.
                     */
                    handlingNameUpdate();

                    string fullPath = "Assets/BrainCloud/Resources/BrainCloudSettingsManual.asset";
                    AssetDatabase.CreateAsset(s_instance, fullPath);
#endif
                }
                s_instance.name = "BrainCloudSettingsManual";

#if UNITY_EDITOR
                BrainCloudDebugInfo.Instance.ClearSettingsData();
#endif
                return s_instance;
            }
        }

        /**
         * Adjust plugin asset name
         */
        private static void handlingNameUpdate()
        {
#if UNITY_EDITOR
            AssetDatabase.DeleteAsset("Assets/BrainCloud/Resources/BrainCloudPluginSettings.asset");
            AssetDatabase.DeleteAsset("Assets/BrainCloud/Resources/BrainCloudSettings.asset");
            AssetDatabase.DeleteAsset("Assets/BrainCloud/Resources/Debug/BrainCloudPluginDebugInfo.asset");
            BaseBrainCloudSettings tempBaseBrainCloudSettings = BrainCloudSettings.Instance;
            BaseBrainCloudDebugInfo tempBaseBrainCloudDebugInfo = BrainCloudDebugInfo.Instance;
#endif
        }

        public string DispatcherURL
        {
            get { return m_serverURL + "/dispatcherv2"; }
        }

        public string PortalURL
        {
            get { return "https://portal.braincloudservers.com"; }
        }

        public string ApiDocsURL
        {
            get { return "https://getbraincloud.com/apidocs"; }
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

        [SerializeField] public AppIdSecretPair[] m_appIdSecrets;

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

#endif
}

