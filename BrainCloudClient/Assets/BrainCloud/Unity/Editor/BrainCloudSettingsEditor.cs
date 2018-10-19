using BrainCloudUnity;
using BrainCloudUnity.BrainCloudPlugin;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BrainCloudSettings))]
public class BrainCloudSettingsEditor : Editor
{
    // Menu Bar
    [MenuItem("brainCloud/Select Settings", false, 001)]
    public static void OpenSettings()
    {
        Selection.activeObject = BrainCloudPluginSettings.Instance;
    }

    [MenuItem("brainCloud/Select Legacy Settings", false, 201)]
    public static void SelectLegacyPlugin()
    {
        Selection.activeObject = BrainCloudSettings.Instance;
    }

    [MenuItem("brainCloud/Launch Portal...", false, 100)]
    public static void GoPortal()
    {
        Help.BrowseURL(BrainCloudSettings.Instance.PortalURL);
    }

    [MenuItem("brainCloud/View API Documentation...", false, 101)]
    public static void GoAPIDoc()
    {
        Help.BrowseURL(BrainCloudSettings.Instance.ApiDocsURL + "/apiref/index.html");
    }

    [MenuItem("brainCloud/View Tutorials...", false, 102)]
    public static void GoTutorials()
    {
        Help.BrowseURL(BrainCloudSettings.Instance.ApiDocsURL + "/tutorials/unity-tutorials/");
    }

    [MenuItem("brainCloud/View Code Examples...", false, 103)]
    public static void GoCodeExamples()
    {
        Help.BrowseURL("https://github.com/getbraincloud/UnityExamples");
    }


    // Draw the content of the inspector GUI
#if UNITY_EDITOR
    
    
    public static void Show (SerializedProperty list) {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel += 1;
        if (list.isExpanded) {
            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < list.arraySize; i++) {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), true);
            }
        }
        EditorGUI.indentLevel -= 1;
    }
    
    public override void OnInspectorGUI()
    {
        BrainCloudSettings instance = (BrainCloudSettings) target;

        if (BrainCloudPluginSettings.IsLegacyPluginEnabled())
        {
            // Game Config
            EditorGUILayout.HelpBox("The game configuration parameters can be found on the brainCloud portal.",
                MessageType.None);

            serializedObject.Update();
            Show(serializedObject.FindProperty("m_appIdSecrets"));
            serializedObject.ApplyModifiedProperties();
            
            
            
            instance.AppId = EditorGUILayout.TextField("App Id", instance.AppId);
            instance.SecretKey = EditorGUILayout.TextField("App Secret", instance.SecretKey);
            instance.GameVersion = EditorGUILayout.TextField("App Version", instance.GameVersion);
            
            

            EditorGUILayout.Space();

            GUILayout.Space(20);
            EditorGUILayout.HelpBox("The brainCloud server to use. Most users should not have to change this value.",
                MessageType.None);
            instance.ServerURL = EditorGUILayout.TextField("Server URL", instance.ServerURL);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset to Default Server URL", GUILayout.ExpandWidth(false)))
            {
                instance.ServerURL = BrainCloudSettings.DEFAULT_BRAINCLOUD_URL;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            EditorGUILayout.HelpBox("Additional development options for the brainCloud library.", MessageType.None);
            instance.EnableLogging = EditorGUILayout.Toggle("Enable Logging", instance.EnableLogging);

            GUILayout.Space(20);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                padding =
                {
                    left = 20,
                    right = 20
                }
            };

            EditorGUILayout.HelpBox("Links to brainCloud webpages.", MessageType.None);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Launch Portal", buttonStyle))
            {
                GoPortal();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("View API Docs", buttonStyle))
            {
                GoAPIDoc();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("View Tutorials", buttonStyle))
            {
                GoTutorials();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox(
                "These are the settings for the Manual brainCloud Settings. Please use the automatic settings asset.",
                MessageType.None);

            if (GUILayout.Button("Select Settings", GUILayout.ExpandWidth(false)))
            {
                Selection.activeObject = BrainCloudPluginSettings.Instance;
            }
        }
    }
#endif
}