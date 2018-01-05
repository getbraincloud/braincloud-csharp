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
        /// <inheritdoc />
        /// <summary>
        /// Contains the debug data for the newer brainCloud Plugin - BrainCloudPluginSettings
        /// When in the Editor, brainCloud | Select Settings 
        /// </summary>
#if UNITY_EDITOR
        [InitializeOnLoad]
#endif
        public class BrainCloudDebugInfo : BaseBrainCloudDebugInfo
        {
            public new static BaseBrainCloudDebugInfo Instance
            {
                get
                {
                    if (_instance) return _instance;

                    _instance = Resources.Load("Debug/BrainCloudPluginDebugInfo") as BrainCloudDebugInfo;

                    // If not found, autocreate the asset object.
                    if (_instance == null)
                    {
                        CreatePluginAsset();
                    }

                    _instance.name = "BrainCloudPluginDebugInfo";

                    return _instance;
                }
            }

            private static void CreatePluginAsset()
            {
                _instance = CreateInstance<BrainCloudDebugInfo>();

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
                properPath = Path.Combine(Application.dataPath, "BrainCloud/Resources/Debug");
                if (!Directory.Exists(properPath))
                {
                    AssetDatabase.CreateFolder("Assets/BrainCloud/Resources", "Debug");
                }


                const string fullPath = "Assets/BrainCloud/Resources/Debug/BrainCloudPluginDebugInfo.asset";
                AssetDatabase.CreateAsset(_instance, fullPath);
#endif
            }


            public void Refresh()
            {
                _instance = Resources.Load("Debug/BrainCloudPluginDebugInfo") as BrainCloudDebugInfo;

                if (_instance != null)
                {
                    _instance.ClearPluginData();

                    Resources.UnloadAsset(_instance);

                    _instance = null;
                }

                CreatePluginAsset();
            }
        }
    }
}

#endif