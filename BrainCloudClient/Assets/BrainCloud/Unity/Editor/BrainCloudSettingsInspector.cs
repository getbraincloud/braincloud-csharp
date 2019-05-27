#if !DOT_NET

using UnityEditor;

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