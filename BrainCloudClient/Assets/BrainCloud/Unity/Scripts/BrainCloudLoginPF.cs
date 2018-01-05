using UnityEngine;
using BrainCloudUnity.HUD;

#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

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
        private string _username = "";
        private string _password = "";

        private Vector2 _scrollPosition;
        private string _authStatus = "Welcome to brainCloud";

        public static BrainCloudWrapper BrainCloud;
        
        void Start()
        {
            ///////////////////////////////////////////////////////////////////
            // brainCloud game configuration
            ///////////////////////////////////////////////////////////////////
            
            
            BrainCloud = gameObject.AddComponent<BrainCloudWrapper>();
            BrainCloud.Init();

            ///////////////////////////////////////////////////////////////////

            _username = PlayerPrefs.GetString("username");

            // Stores the password in plain text directly in the unity store.
            // This is obviously not secure but speeds up debugging/testing.
            _password = PlayerPrefs.GetString("password");
            
            // Clearing current profile
            BrainCloud.ResetStoredAnonymousId();
            BrainCloud.ResetStoredProfileId();
        }

        void OnGUI()
        {
            if (!BrainCloud.Client.IsAuthenticated())
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
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            GUILayout.Label("Username");
            _username = GUILayout.TextField(_username, GUILayout.MinWidth(200));

            GUILayout.Label("Password");
            _password = GUILayout.PasswordField(_password, '*', GUILayout.MinWidth(100));

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Authenticate", GUILayout.MinHeight(30), GUILayout.MinWidth(100)))
            {
                if (_username.Length == 0 || _password.Length == 0)
                {
                    AppendLog("Username/password can't be empty");
                }
                else
                {
                    AppendLog("Attempting to authenticate...");
                    PlayerPrefs.SetString("username", _username);
                    PlayerPrefs.SetString("password", _password);

                    ///////////////////////////////////////////////////////////////////
                    // brainCloud authentication
                    ///////////////////////////////////////////////////////////////////

                    BrainCloud.AuthenticateUniversal(_username, _password, true, OnSuccess_Authenticate, OnError_Authenticate);

                    ///////////////////////////////////////////////////////////////////
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUILayout.TextArea(_authStatus);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear Log", GUILayout.MinHeight(30), GUILayout.MinWidth(100)))
            {
                _authStatus = "";
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void AppendLog(string log)
        {
            string oldStatus = _authStatus;
            _authStatus = "\n" + log + "\n" + oldStatus;
            Debug.Log(log);
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
                hud.Minimized = false;
            }
            if (m_authSuccessLevel != null && m_authSuccessLevel.Length > 0)
            {
#if UNITY_5_3_OR_NEWER
                SceneManager.LoadScene(m_authSuccessLevel);
#else
                Application.LoadLevel(m_authSuccessLevel);
#endif
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
            get { return m_authSuccessLevel; }
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
