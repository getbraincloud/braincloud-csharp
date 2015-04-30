using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class BrainCloudSettings : ScriptableObject
{
	private static BrainCloudSettings s_instance;
	public static BrainCloudSettings Instance
	{
		get
		{
			if (s_instance) return s_instance;

			s_instance = Resources.Load("BrainCloudSettings") as BrainCloudSettings;
			if (s_instance == null)
			{
				// If not found, autocreate the asset object.
				s_instance = CreateInstance<BrainCloudSettings>();

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
				string fullPath = "Assets/BrainCloud/Resources/BrainCloudSettings.asset";
				AssetDatabase.CreateAsset(s_instance, fullPath);
#endif
			}
			s_instance.name = "BrainCloud Settings";
			return s_instance;
		}
	}
	
#if UNITY_EDITOR
	// Menu Bar
	[MenuItem ("brainCloud/Settings", false, 0)]
	public static void  GoSettings () 
	{
		Selection.activeObject = BrainCloudSettings.Instance;
	}

	[MenuItem ("brainCloud/Portal...", false, 100)]
	public static void  GoPortal () 
	{
		Help.BrowseURL(Instance.PortalURL);
	}

	[MenuItem ("brainCloud/API Documentation...", false, 101)]
	public static void  GoAPIDoc () 
	{
		Help.BrowseURL(Instance.ApiDocsURL);
	}
	
	[MenuItem ("brainCloud/Tutorials...", false, 200)]
	public static void  GoTutorials () 
	{
		Help.BrowseURL(Instance.ApiDocsURL + "/api/tutorials");
	}
#endif

	
	public string DispatcherURL
	{
		get {return m_serverURL + "/dispatcher";}
	}
	public string PortalURL
	{
		get {return m_serverURL;}
	}
	public string ApiDocsURL
	{
		get {return "http://apidocs.braincloudservers.com";}
	}

	// Settings
	public const string DEFAULT_BRAINCLOUD_URL = "https://sharedprod.braincloudservers.com";
	[SerializeField]
	private string m_serverURL = DEFAULT_BRAINCLOUD_URL;
	public string ServerURL
	{
		get {return m_serverURL;}
		set
		{
			if (m_serverURL != value)
			{
				m_serverURL = value;
				#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
				#endif
			}
		}
	}

	[SerializeField]
	private string m_secretKey = "";
	public string SecretKey
	{
		get {return m_secretKey;}
		set
		{
			if (m_secretKey != value)
			{
				m_secretKey = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
	}

	[SerializeField]
	private string m_gameId = "";
	public string GameId
	{
		get {return m_gameId;}
		set
		{
			if (m_gameId != value)
			{
				m_gameId = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
	}

	[SerializeField]
	private string m_gameVersion = "1.0";
	public string GameVersion
	{
		get {return m_gameVersion;}
		set
		{
			if (m_gameVersion != value)
			{
				m_gameVersion = value;
#if UNITY_EDITOR
				EditorUtility.SetDirty(this);
#endif
			}
		}
	}
}