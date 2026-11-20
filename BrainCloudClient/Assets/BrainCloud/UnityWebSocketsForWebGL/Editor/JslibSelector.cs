using UnityEditor;

[InitializeOnLoad]
static class JslibSelector
{
    private const bool USE_MAKEDYNCALL
#if UNITY_2021_2_OR_NEWER
        = true;
#else
        = false;
#endif

    static JslibSelector()
    {
        foreach (var importer in PluginImporter.GetAllImporters())
        {
            if (importer.assetPath.EndsWith("websocket-native-legacy.jslib"))
            {
                importer.SetIncludeInBuildDelegate(_ => !USE_MAKEDYNCALL);
            }
            else if (importer.assetPath.EndsWith("websocket-native.jslib"))
            {
                importer.SetIncludeInBuildDelegate(_ => USE_MAKEDYNCALL);
            }
        }
    }
}
