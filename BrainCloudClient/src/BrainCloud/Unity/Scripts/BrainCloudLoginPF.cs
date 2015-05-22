using UnityEngine;
using System.Collections;
using BrainCloudUnity.HUD;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BrainCloudUnity
{
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif

	public class BrainCloudLoginPF : MonoBehaviour
	{
		private string m_username = "";
		private string m_password = "";
		
		private Vector2 m_scrollPosition;
		private string m_authStatus = "Welcome to brainCloud";
		
		void Start()
		{
			///////////////////////////////////////////////////////////////////
			// brainCloud game configuration
			///////////////////////////////////////////////////////////////////
			
			BrainCloudWrapper.Initialize();
			
			///////////////////////////////////////////////////////////////////
			
			m_username = PlayerPrefs.GetString("username");
			
			// Stores the password in plain text directly in the unity store.
			// This is obviously not secure but speeds up debugging/testing.
			m_password = PlayerPrefs.GetString("password");
		}
		
		void Update()
		{
		}
		
		void OnGUI()
		{
			if (!BrainCloudWrapper.GetBC ().IsAuthenticated())
			{
				int width = Screen.width / 2 - 125;
				if (width < 500) width = 500;
				if (width > Screen.width) width = Screen.width;
				
				int height = Screen.height / 2 - 200;
				if (height < 400) height = 400;
				if (height > Screen.height) height = Screen.height;
				
				GUILayout.Window(0, new Rect(Screen.width / 2 - (width / 2), Screen.height / 2 - (height / 2), width, height), OnWindow, "brainCloud Login");
			}
		}
		
		void OnWindow(int windowId)
		{
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.BeginVertical ();
			
			GUILayout.Label ("Username");
			m_username = GUILayout.TextField (m_username, GUILayout.MinWidth (200));
			
			GUILayout.Label ("Password");
			m_password = GUILayout.PasswordField (m_password, '*', GUILayout.MinWidth (100));
			
			GUILayout.Space (10);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button ("Authenticate", GUILayout.MinHeight (30), GUILayout.MinWidth (100))) 
			{
				if( m_username.Length == 0 || m_password.Length == 0 )
				{
					AppendLog("Username/password can't be empty");
				}
				else 
				{
					AppendLog("Attempting to authenticate...");
					PlayerPrefs.SetString("username", m_username);
					PlayerPrefs.SetString("password", m_password);
					
					///////////////////////////////////////////////////////////////////
					// brainCloud authentication
					///////////////////////////////////////////////////////////////////
					
					BrainCloudWrapper.GetInstance().AuthenticateUniversal(m_username, m_password, true, OnSuccess_Authenticate, OnError_Authenticate);
					
					///////////////////////////////////////////////////////////////////
				}
			}
			
			GUILayout.EndHorizontal ();
			GUILayout.Space (20);
			
			m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			GUILayout.TextArea(m_authStatus);
			GUILayout.EndScrollView();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Clear Log", GUILayout.MinHeight(30), GUILayout.MinWidth(100)))
			{
				m_authStatus = "";
			}
			GUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();
		}
		
		private void AppendLog(string log)
		{
			string oldStatus = m_authStatus;
			m_authStatus = "\n" + log + "\n" + oldStatus;
			Debug.Log (log);
		}
		
		public void OnSuccess_Authenticate(string responseData, object cbObject)
		{
			AppendLog("Authenticate successful!");

			// enable the HUD if it's kicking around
			Object o = FindObjectOfType(typeof(BrainCloudHUD));
			if (o)
			{
				BrainCloudHUD hud = (BrainCloudHUD)o;
				hud.EnableUI = true;
			}
			if (m_authSuccessLevel != null && m_authSuccessLevel.Length > 0)
			{
				Application.LoadLevel(m_authSuccessLevel);
			}
		}
		
		public void OnError_Authenticate(int statusCode, int reasonCode, string statusMessage, object cbObject)
		{
			AppendLog("Authenticate failed, statusCode: " + statusCode + " reasonCode: " + reasonCode + " statusMessage: " + statusMessage);
		}
		
		[SerializeField]
		private string m_authSuccessLevel = "";
		public string AuthSuccessLevel
		{
			get {return m_authSuccessLevel;}
			set
			{
				if (m_authSuccessLevel != value)
				{
					m_authSuccessLevel = value;
					#if UNITY_EDITOR
					EditorUtility.SetDirty(this);
					#endif
				}
			}
		}
	}
}