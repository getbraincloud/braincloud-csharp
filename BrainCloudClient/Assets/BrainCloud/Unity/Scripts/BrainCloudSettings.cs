#if !DOT_NET

#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor;

namespace BrainCloudUnity
{
    namespace BrainCloudSettingsDLL
    {
        /// <summary>
        /// BrainCloud Plugin Data, for those using the Embedded Editor Login
        /// 
        /// When in the Editor, brainCloud | Select Settings 
        /// </summary>
        [InitializeOnLoad]
        public class BrainCloudSettings : BaseBrainCloudSettings
        {
            public static bool IsManualSettingsEnabled()
            {
                return Instance.SettingsState == BrainCloudSettingsState.INTRO ||
                       Instance.SettingsState == BrainCloudSettingsState.DISABLED;
            }

            public override void OnEnable()
            {
                base.OnEnable();
                BrainCloudDebugInfo.Instance.ClearSettingsData();
                BaseBrainCloudSettings.Instance.BrainCloudSettingsUpdated += UpdateSettings;
            }
            
            private void OnDisable()
            {
                BaseBrainCloudSettings.Instance.BrainCloudSettingsUpdated -= UpdateSettings;
            }
            
            private void UpdateSettings()
            {
                if (!IsManualSettingsEnabled())
                {
                    BrainCloudSettingsManual.Instance.ServerURL = Instance.GetServerUrl;   
                    BrainCloudSettingsManual.Instance.SecretKey = GetAppSecret();
                    BrainCloudSettingsManual.Instance.AppId = GetAppId();
                    BrainCloudSettingsManual.Instance.AppVersion = GetAppVersion();

                    var appIdSecrets = GetAppIdSecrets();
                    
                    if(appIdSecrets != null)
                        BrainCloudSettingsManual.Instance.m_appIdSecrets =
                            AppIdSecretPair.FromDictionary(GetAppIdSecrets());
   
                    EditorUtility.SetDirty(Instance);
                
                    EditorUtility.SetDirty(Resources.Load("BrainCloudSettingsManual") as BrainCloudSettingsManual);
                }
            }

            
            public static BrainCloudDebugInfo DebugInstance;

            public new static BaseBrainCloudSettings Instance
            {
                get
                {
                    if (_instance) _instance.ClientVersion = BrainCloud.Version.GetVersion();
                    
                    if (_instance) return _instance;

                    _instance = Resources.Load("BrainCloudSettings") as BrainCloudSettings;

                    // If not found, autocreate the asset object.
                    if (_instance == null)
                    {
                        CreateSettingsAsset();
                    }

                    DebugInstance = (BrainCloudDebugInfo)BrainCloudDebugInfo.Instance;

                    return _instance;
                }
            }

            private static void CreateSettingsAsset()
            {
                _instance = CreateInstance<BrainCloudSettings>();

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


                const string fullPath = "Assets/BrainCloud/Resources/BrainCloudSettings.asset";
                AssetDatabase.CreateAsset(_instance, fullPath);
            }


            public override void Refresh()
            {
                if (_instance != null)
                {
                    _instance.ClearSettingsData();
                }
            }
        }
    }
}

#endif
#endif
