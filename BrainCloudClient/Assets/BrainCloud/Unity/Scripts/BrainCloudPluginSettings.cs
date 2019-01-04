#if !DOT_NET

using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace BrainCloudUnity
{
    namespace BrainCloudPlugin
    {
        /// <summary>
        /// BrainCloud Plugin Data, for those using the Embedded Editor Login
        /// 
        /// When in the Editor, brainCloud | Select Settings 
        /// </summary>
#if UNITY_EDITOR
        [InitializeOnLoad]
#endif
        public class BrainCloudPluginSettings : BaseBrainCloudPluginSettings
        {
            public static bool IsLegacyPluginEnabled()
            {
                return Instance.PluginState == BrainCloudPluginState.INTRO ||
                       Instance.PluginState == BrainCloudPluginState.DISABLED;
            }

            public static BrainCloudDebugInfo DebugInstance;

            public new static BaseBrainCloudPluginSettings Instance
            {
                get
                {
                    if (_instance) _instance.ClientVersion = BrainCloud.Version.GetVersion();
                    
                    if (_instance) return _instance;

                    _instance = Resources.Load("BrainCloudPluginSettings") as BrainCloudPluginSettings;

                    // If not found, autocreate the asset object.
                    if (_instance == null)
                    {
                        CreatePluginAsset();
                    }

                    DebugInstance = (BrainCloudDebugInfo)BrainCloudDebugInfo.Instance;

                    return _instance;
                }
            }

            private static void CreatePluginAsset()
            {
                _instance = CreateInstance<BrainCloudPluginSettings>();

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


                const string fullPath = "Assets/BrainCloud/Resources/BrainCloudPluginSettings.asset";
                AssetDatabase.CreateAsset(_instance, fullPath);
#endif
            }


            public override void Refresh()
            {
                if (_instance != null)
                {
                    _instance.ClearPluginData();
                }
            }
        }
    }
}

#endif