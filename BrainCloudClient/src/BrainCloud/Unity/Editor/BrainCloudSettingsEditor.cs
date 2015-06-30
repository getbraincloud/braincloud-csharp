using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(BrainCloudSettings))]
public class BrainCloudSettingsEditor : Editor
{
	// Draw the content of the inspector GUI
	#if UNITY_EDITOR
	public override void OnInspectorGUI()
	{
		BrainCloudSettings instance = (BrainCloudSettings)target;
		
		// Game Config
		EditorGUILayout.HelpBox("The game configuration parameters can be found on the brainCloud portal.", MessageType.None);

		instance.GameId = EditorGUILayout.TextField("Game Id:", instance.GameId);
		instance.SecretKey = EditorGUILayout.TextField("Game Secret:", instance.SecretKey);
		instance.GameVersion = EditorGUILayout.TextField("Game Version:", instance.GameVersion);

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("The brainCloud server to use. Most users should not have to change this value.", MessageType.None);
		instance.ServerURL = EditorGUILayout.TextField("Server URL:", instance.ServerURL);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Reset to Default Server URL", GUILayout.ExpandWidth (false)))
		{
			instance.ServerURL = BrainCloudSettings.DEFAULT_BRAINCLOUD_URL;
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.HelpBox("Additional development options for the brainCloud library.", MessageType.None);
		instance.EnableLogging = EditorGUILayout.Toggle ("Enable Logging:", instance.EnableLogging);
	}
	#endif
}
