#if !DOT_NET

using UnityEditor;
using UnityEngine;

namespace BrainCloudUnity
{
    namespace BrainCloudSettingsDLL
    {
        [CustomEditor(typeof(BrainCloudSettings))]
        public class BrainCloudSettingsInspector : BaseBrainCloudSettingsInspector {
            
        }
    }
}


#endif