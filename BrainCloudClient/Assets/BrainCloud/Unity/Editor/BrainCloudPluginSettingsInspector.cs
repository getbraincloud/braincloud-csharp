#if !DOT_NET

using UnityEditor;
using UnityEngine;

namespace BrainCloudUnity
{
    namespace BrainCloudPlugin
    {
        [CustomEditor(typeof(BrainCloudPluginSettings))]
        public class BrainCloudPluginSettingsInspector : BaseBrainCloudPluginSettingsInspector {
            
        }
    }
}


#endif