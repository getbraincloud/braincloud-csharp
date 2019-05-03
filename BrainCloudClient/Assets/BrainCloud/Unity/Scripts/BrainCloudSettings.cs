#if !DOT_NET

#if UNITY_EDITOR

using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
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
