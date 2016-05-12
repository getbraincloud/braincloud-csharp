using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using BrainCloudUnity;

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

		instance.GameId = EditorGUILayout.TextField("Game Id", instance.GameId);
		instance.SecretKey = EditorGUILayout.TextField("Game Secret", instance.SecretKey);
		instance.GameVersion = EditorGUILayout.TextField("Game Version", instance.GameVersion);

		EditorGUILayout.Space();

		GUILayout.Space (20);
		EditorGUILayout.HelpBox("The brainCloud server to use. Most users should not have to change this value.", MessageType.None);
		instance.ServerURL = EditorGUILayout.TextField("Server URL", instance.ServerURL);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Reset to Default Server URL", GUILayout.ExpandWidth (false)))
		{
			instance.ServerURL = BrainCloudSettings.DEFAULT_BRAINCLOUD_URL;
		}
		GUILayout.EndHorizontal();

		GUILayout.Space (20);
		EditorGUILayout.HelpBox("Additional development options for the brainCloud library.", MessageType.None);
		instance.EnableLogging = EditorGUILayout.Toggle ("Enable Logging", instance.EnableLogging);

		GUILayout.Space (20);

		GUIStyle buttonStyle = new GUIStyle (GUI.skin.button);
		buttonStyle.padding.left = 20;
		buttonStyle.padding.right = 20;

		EditorGUILayout.HelpBox("Links to brainCloud webpages.", MessageType.None);
		GUILayout.Space (10);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Launch Portal", buttonStyle))
		{
			BrainCloudSettings.GoPortal();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		//GUIStyle linkStyle = new GUIStyle(GUI.skin.button);
		//linkStyle.richText = true;
		//if (GUILayout.Button("<color=#ffda48ff>Open brainCloud API Docs</color>", linkStyle))
		if (GUILayout.Button("View API Docs", buttonStyle))
		{
			BrainCloudSettings.GoAPIDoc();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("View Tutorials", buttonStyle))
		{
			BrainCloudSettings.GoTutorials();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

	}
	#endif
}
