
namespace BrainCloudUnity
{
    

#if !DOT_NET

#if UNITY_EDITOR

    using System.IO;
    using UnityEngine;
    using UnityEditor;

    

    namespace BrainCloudSettingsDLL
    {
        
        
        /// <inheritdoc />
        /// <summary>
        /// Contains the debug data for the brainCloud Settings - BrainCloudSettings
        /// When in the Editor, brainCloud | Select Settings 
        /// </summary>

        [InitializeOnLoad]
        public class BrainCloudDebugInfo : BaseBrainCloudDebugInfo
        {
            public new static BaseBrainCloudDebugInfo Instance
            {
                get
                {
                    if (_instance) return _instance;

                    _instance = Resources.Load("Debug/BrainCloudDebugInfo") as BrainCloudDebugInfo;

                    // If not found, autocreate the asset object.
                    if (_instance == null)
                    {
                        CreateSettingsAsset();
                    }

                    _instance.name = "BrainCloudDebugInfo";

                    return _instance;
                }
            }

            private static void CreateSettingsAsset()
            {
                _instance = CreateInstance<BrainCloudDebugInfo>();

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
                properPath = Path.Combine(Application.dataPath, "BrainCloud/Resources/Debug");
                if (!Directory.Exists(properPath))
                {
                    AssetDatabase.CreateFolder("Assets/BrainCloud/Resources", "Debug");
                }

                const string fullPath = "Assets/BrainCloud/Resources/Debug/BrainCloudDebugInfo.asset";
                AssetDatabase.CreateAsset(_instance, fullPath);
           }


            public void Refresh()
            {
                _instance = Resources.Load("Debug/BrainCloudDebugInfo") as BrainCloudDebugInfo;

                if (_instance != null)
                {
                    _instance.ClearSettingsData();

                    Resources.UnloadAsset(_instance);

                    _instance = null;
                }

                CreateSettingsAsset();
            }
        }
    }
    
#endif

#endif
}
