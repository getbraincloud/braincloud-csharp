using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

namespace Tests.PlayMode
{
    public class TestFixtureBase : MonoBehaviour
    {
        public string ServerUrl = "https://internal.braincloudservers.com/dispatcherv2";
        public string AppId = "20001";
        public string Secret = "4e51b45c-030e-4f21-8457-dc53c9a0ed5f";
        public string Version = "1.0.0";
        public string ChildAppId = "20005";
        public string ChildSecret = "f8cec1cf-2f95-4989-910c-8caf598f83db";
        public string ParentLevel = "Master";
        public string PeerName = "peerapp";
        public string SupportsCompression = "false";

        private JsonWriterSettings _writerSettings = new JsonWriterSettings
        {
            PrettyPrint = true,
            Tab = "  "
        };
        public BrainCloudWrapper _bc;
    
        public enum Users { UserA, UserB, UserC }
        protected TestUser _currentUser;
        private static TestUser[] _testUsers;
        private static bool _init = false;
        private bool _isSpinning;
        public bool _isRunning;

        protected GameObject _gameObject;
        protected TestFixtureBase _testingContainer;
        public Server Server;
        public TestFixtureBase(BrainCloudWrapper bc)
        {
            _bc = bc;
        }
    
        // A Test behaves as an ordinary method
        //[Test]
        public void TestFixtureBaseSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        public IEnumerator TestFixtureBaseWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    
        [TearDown]
        public void TearDown()
        {
            _testingContainer._bc.Client.ResetCommunication();
            _testingContainer._bc.Client.DeregisterEventCallback();
            _testingContainer._bc.Client.DeregisterRewardCallback();
        }
    
        [SetUp]
        public void SetUp()
        {
            _gameObject = Instantiate(new GameObject("TestingContainer"), Vector3.zero, Quaternion.identity);
            _testingContainer = _gameObject.AddComponent<TestFixtureBase>();
            _testingContainer._bc = _gameObject.AddComponent<BrainCloudWrapper>();
            _testingContainer._gameObject = _gameObject;
            Dictionary<string, string> secretMap = new Dictionary<string, string>();
            secretMap.Add(AppId, Secret);
            secretMap.Add(ChildAppId, ChildSecret);
            _testingContainer._bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            _testingContainer._bc.Client.EnableLogging(true);
            _testingContainer._bc.Client.RegisterLogDelegate(HandleLog);

            //set to enable compression
            if(SupportsCompression != "")
            {
                _testingContainer._bc.Client.EnableCompression(Boolean.Parse(SupportsCompression));
            }
            Debug.Log("Done Set Up");
            /*
            if (ShouldAuthenticate())
            {
                TestResult tr = new TestResult(_bc);
                _bc.Client.AuthenticationService.AuthenticateUniversal(
                    GetUser(Users.UserA).Id,
                    GetUser(Users.UserA).Password,
                    true,
                    tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
            */
            
            //yield return StartCoroutine(Run());
        }
        
        /// <summary>
        /// Overridable method which if set to true, will cause unit test "SetUp" to
        /// attempt an authentication before calling the test method.
        /// </summary>
        /// <returns><c>true</c>, if authenticate was shoulded, <c>false</c> otherwise.</returns>
        public virtual bool ShouldAuthenticate()
        {
            return true;
        }
    
        public void Reset()
        {
            m_done = false;
            m_result = false;
            m_apiCountExpected = 0;
            m_response = null;
            m_statusCode = 0;
            m_reasonCode = 0;
            m_statusMessage = null;
            m_globalErrorCount = 0;
            m_networkErrorCount = 0;
        }

        public void StartRun()
        {
            m_done = false;
            StartCoroutine(Run());
        }
    
        public IEnumerator Run(int in_apiCount = 1)
        {
            Debug.Log("Running...");
            _isRunning = true;
            Reset();
            m_apiCountExpected = in_apiCount;
            
            var timeBefore = DateTime.Now;
            while (!m_done && (DateTime.Now - timeBefore).TotalSeconds < m_timeToWaitSecs)
            {
                if (_bc)
                {
                    _bc.Update();    
                }
                yield return new WaitForFixedUpdate();
            }
            
            _isRunning = false;
        }

        private IEnumerator Spin()
        {
            _isSpinning = false;
            yield return null;
        }
        
        public void RunAuth()
        {
            StartCoroutine(SetUpAuth());
        }

        public IEnumerator SetUpAuth()
        {
            Debug.Log("Set Up Authentication Started...");

            StartCoroutine(SetUpNewUser(Users.UserA));
            while (!_init) 
                yield return new WaitForFixedUpdate();
        }

        /// <summary>
        /// Routine loads up brainCloud configuration info from "tests/ids.txt" (hopefully)
        /// in a platform agnostic way.
        /// </summary>
        /// ToDo FL : Doesn't have access to read ids.txt. Lame
        private void LoadIds()
        {
            string exePath = Application.dataPath;
            string absPath = exePath;
            string search = "tests";
            if (absPath.Contains(search))
            {
                absPath = absPath.Substring(0, absPath.LastIndexOf(search));
                absPath += search + Path.DirectorySeparatorChar + "tests" + Path.DirectorySeparatorChar + "ids.txt";
            }
            //Console.Out.WriteLine(absPath);
            //Console.Out.WriteLine(search);

            using (var reader = new StreamReader(absPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("serverUrl="))
                    {
                        ServerUrl = line.Substring(("serverUrl=").Length);
                        ServerUrl.Trim();
                    }
                    else if (line.StartsWith("appId="))
                    {
                        AppId = line.Substring(("appId=").Length);
                        AppId.Trim();
                    }
                    else if (line.StartsWith("secret="))
                    {
                        Secret = line.Substring(("secret=").Length);
                        Secret.Trim();
                    }
                    else if (line.StartsWith("version="))
                    {
                        Version = line.Substring(("version=").Length);
                        Version.Trim();
                    }
                    else if (line.StartsWith("childAppId="))
                    {
                        ChildAppId = line.Substring(("childAppId=").Length);
                        ChildAppId.Trim();
                    }
                    else if (line.StartsWith("childSecret="))
                    {
                        ChildSecret = line.Substring(("childSecret=").Length);
                        ChildSecret.Trim();
                    }
                    else if (line.StartsWith("parentLevelName="))
                    {
                        ParentLevel = line.Substring(("parentLevelName=").Length);
                        ParentLevel.Trim();
                    }
                    else if (line.StartsWith("peerName="))
                    {
                        PeerName = line.Substring(("peerName=").Length);
                        PeerName.Trim();
                    }
                    else if (line.StartsWith("supportsCompression="))
                    {
                        SupportsCompression = line.Substring(("supportsCompression=").Length);
                        SupportsCompression.Trim();
                    }
                }
            }
        }
    
        /// <summary>
        /// Pretty prints outgoing and incoming log messages
        /// </summary>
        /// <param name="message">Log message</param>
        private void HandleLog(string message)
        {
            if (message.StartsWith("#BCC"))
            {
                string outPrefix = "#BCC OUTGOING: ";
                string inPrefix = "#BCC INCOMING: ";

                string prefix = "";

                if (message.StartsWith(outPrefix))
                {
                    prefix = outPrefix;
                    message = message.Substring(outPrefix.Length);
                }
                else if (message.StartsWith(inPrefix))
                {
                    prefix = inPrefix;
                    message = message.Substring(inPrefix.Length);
                }

                try
                {
                    var data = JsonReader.Deserialize(message);
                    var sb = new StringBuilder();
                    var writer = new JsonWriter(sb, _writerSettings);
                    writer.Write(data);
                    message = sb.ToString();
                }
                catch (JsonDeserializationException e)
                {
                    Debug.Log(e.Message);
                }

                message = string.Format("\r\n{0}\r\n{1}", prefix, message);
            }
            Debug.Log(message);
        }
        
        public IEnumerator SetUpNewUser(Users user)
        {
            if (!_init)
            {
                Debug.Log(">> Initializing New Random Users");
                _bc.Client.EnableLogging(true);
                
                Random rand = new Random();

                _currentUser = _gameObject.AddComponent<TestUser>();
                IEnumerator routine = _currentUser.SetUp
                (
                    _bc,
                    Users.UserA + "_CS" + "-",
                    rand.Next(),
                    this
                );
                
                StartCoroutine(routine);
                while (_currentUser.IsRunning) 
                    yield return new WaitForFixedUpdate();
                _init = true;
            }
            //_currentUser = _testUsers[(int)user];
            yield return null;
        }
    
        public bool m_done;
        public bool m_result;
        public int m_apiCountExpected;

        // if success
        public Dictionary<string, object> m_response =  new Dictionary<string, object>();

        // if error
        public int m_statusCode;
        public int m_reasonCode;
        public string m_statusMessage;
        public int m_timeToWaitSecs = 120;
        public int m_globalErrorCount;
        public int m_networkErrorCount;
        protected TestFixtureBase()
        {
            
        }


        public void ApiSuccess(string json, object cb)
        {
            m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            m_result = true;
            --m_apiCountExpected;
            
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
        }

        public void ApiError(int statusCode, int reasonCode, string jsonError, object cb)
        {
            m_statusCode = statusCode;
            m_reasonCode = reasonCode;
            m_statusMessage = jsonError;
            m_result = false;
            --m_apiCountExpected;
            if (m_apiCountExpected <= 0)
            {
                m_done = true;
            }
        }
    }

    public class TestUser : MonoBehaviour
    {
        public string Id = "";
        public string Password = "";
        public string ProfileId = "";
        public string Email = "";

        BrainCloudWrapper _bc;
        public bool IsRunning;
        private TestFixtureBase _tf;
        public TestUser(BrainCloudWrapper bc, string idPrefix, int suffix)
        {
            _bc = bc;

            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            StartCoroutine(Authenticate());
        }

        public IEnumerator SetUp(BrainCloudWrapper bc, string idPrefix, int suffix, TestFixtureBase testFixtureBase)
        {
            _bc = bc;
            _tf = testFixtureBase;
            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            IsRunning = true;
            StartCoroutine(Authenticate());
            while (!IsRunning) 
                yield return new WaitForFixedUpdate();
        }

        private IEnumerator Authenticate()
        {
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                Id,
                Password,
                true,
                _tf.ApiSuccess, _tf.ApiError);
            StartCoroutine(_tf.Run());
            while (_tf._isRunning) 
                yield return new WaitForFixedUpdate();
            
            ProfileId = _bc.Client.AuthenticationService.ProfileId;
            
            if (_tf.m_response.Count > 0 && ((string)((Dictionary<string, object>)_tf.m_response["data"])["newUser"]) == "true")
            {
                _bc.MatchMakingService.EnableMatchMaking(_tf.ApiSuccess, _tf.ApiError);
                StartCoroutine(_tf.Run());
                while (_tf._isRunning) 
                    yield return new WaitForFixedUpdate();
                _bc.PlayerStateService.UpdateUserName(Id, _tf.ApiSuccess, _tf.ApiError);
                StartCoroutine(_tf.Run());
                while (_tf._isRunning) 
                    yield return new WaitForFixedUpdate();
                _bc.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", _tf.ApiSuccess, _tf.ApiError);
                StartCoroutine(_tf.Run());
                while (_tf._isRunning) 
                    yield return new WaitForFixedUpdate();
            }
            else
            {
                Debug.Log("Got no response from Authentication");
            }

            /*_bc.PlayerStateService.Logout(_tf.ApiSuccess, _tf.ApiError);
            StartCoroutine(_tf.Run());
            while (_tf._isRunning) 
                yield return new WaitForFixedUpdate();*/
            IsRunning = false;
        }
    }
}


public class Server
{
    public string Host;
    public int WsPort = -1;
    public int TcpPort = -1;
    public int UdpPort = -1;
    public string Passcode;
    public string LobbyId;

    public Server(Dictionary<string, object> serverJson)
    {
        var connectData = serverJson["connectData"] as Dictionary<string, object>;
        var ports = connectData["ports"] as Dictionary<string, object>;

        Host = connectData["address"] as string;
        WsPort = (int)ports["ws"];
        TcpPort = (int)ports["tcp"];
        UdpPort = (int)ports["udp"];
        Passcode = serverJson["passcode"] as string;
        LobbyId = serverJson["lobbyId"] as string;
    }
}