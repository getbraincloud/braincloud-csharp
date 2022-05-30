using System.IO;
using System.Text;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    /*
     * TestFixtureBase is mainly used
     *  - Hold braincloud init parameters
     *  - Set up UnityTests
     *  - Tear down UnityTests
     *  - Handle console logging
     *  - Load Id's from file with a given path with "pathToIds" string variable
     */
    public class TestFixtureBase : MonoBehaviour
    {
        public string ServerUrl;
        public string AppId;
        public string Secret;
        public string Version;
        public string ChildAppId;
        public string ChildSecret;
        public string ParentLevel;
        public string PeerName;
        public string SupportsCompression;
        
        protected int _successCount = 0;
        protected GameObject _gameObject;
        protected TestContainer _tc;
        protected bool _isRunning;
        private string pathToIds;
        
        protected string username = "UnityTestUser";
        protected string password = "testPass";
        protected bool forceCreate = true;
        
        private JsonWriterSettings _writerSettings = new JsonWriterSettings
        {
            PrettyPrint = true,
            Tab = "  "
        };
        
        [TearDown]
        public virtual void TearDown()
        {
            Debug.Log("Tearing Down....");
            _tc.bcWrapper.Client.FlushCachedMessages(false);
            
            _tc.bcWrapper.Client.ResetCommunication();
            _tc.bcWrapper.Client.DeregisterEventCallback();
            _tc.bcWrapper.Client.DeregisterRewardCallback();
            _tc.bcWrapper.Client.DeregisterFileUploadCallback();
            _tc.bcWrapper.Client.DeregisterFileUploadCallbacks();
            _tc.bcWrapper.Client.DeregisterGlobalErrorCallback();
            _tc.bcWrapper.Client.DeregisterNetworkErrorCallback();
            _tc.bcWrapper.Client.ShutDown();
            _tc.CleanUp();
            
            _successCount = 0;
            Destroy(_tc.bcWrapper);
            var listOfContainers = FindObjectsOfType<Transform>();
            foreach (Transform container in listOfContainers)
            {
                if (container.name.Contains("TestingContainer"))
                {
                    Destroy(container.gameObject);
                }
            }
            
            _tc = null;
        }
    
        [SetUp]
        public void SetUp()
        {
            Debug.Log("Setting Up......");
            
            LoadIds();
            _gameObject = Instantiate(new GameObject("TestingContainer"), Vector3.zero, Quaternion.identity);
            _tc = _gameObject.AddComponent<TestContainer>();
            
            _tc.bcWrapper = _gameObject.AddComponent<BrainCloudWrapper>();
            _tc.bcWrapper.Init(ServerUrl, Secret, AppId, Version);
            
            _tc.bcWrapper.Client.EnableLogging(true);
            _tc.bcWrapper.Client.RegisterLogDelegate(HandleLog);
        }

        /// <summary>
        /// Routine loads up brainCloud configuration info from "tests/ids.txt" (hopefully)
        /// in a platform agnostic way.
        /// </summary>
        private void LoadIds()
        {
            pathToIds = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "\\ids.txt";
            using (var reader = new StreamReader(pathToIds))
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
        protected void HandleLog(string message)
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

        protected void LogResults(string errorMessage,bool testPassed)
        {
            if (testPassed)
            {
                Debug.Log($"Test passed");
                Assert.True(true);
            }
            else
            {
                Debug.Log($"ERROR: {errorMessage}");
                if (_tc.m_statusMessage != null && _tc.m_statusMessage.Contains("{"))
                {
                    Debug.Log($"Json Error: {_tc.m_statusMessage}");    
                }
                
                Assert.True(false);
            }
        }
    }
}