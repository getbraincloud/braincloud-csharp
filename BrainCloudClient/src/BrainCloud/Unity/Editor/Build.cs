using UnityEditor;
using System.Collections;
using System.Linq;

public class Build {
	static string OUTPUT_FOLDER = "../autobuild_prefabs/artifacts/generated_build";

	static string[] GetScenes()
	{
		string[] scenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
		UnityEngine.Debug.Log ("Dumping scenes...");
		foreach (string scene in scenes)
		{
			UnityEngine.Debug.Log (scene);
		}
		return scenes;
	}

	static void PerformBuildIOS()
	{
		string[] scenes = GetScenes();
		BuildPipeline.BuildPlayer(scenes, System.IO.Path.GetFullPath(OUTPUT_FOLDER), BuildTarget.iPhone, BuildOptions.None);
	}

	static void PerformBuildWeb()
	{
		string[] scenes = GetScenes();
		BuildPipeline.BuildPlayer(scenes, System.IO.Path.GetFullPath(OUTPUT_FOLDER), BuildTarget.WebPlayer, BuildOptions.None);
	}
}
