using System.IO;
using System.Text;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;

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
        protected TestContainer _testingContainer;

        private string pathToIds = "C:/ids.txt";
        
        private JsonWriterSettings _writerSettings = new JsonWriterSettings
        {
            PrettyPrint = true,
            Tab = "  "
        };
        
        [TearDown]
        public void TearDown()
        {
            _testingContainer.bcWrapper.Client.ResetCommunication();
            _testingContainer.bcWrapper.Client.DeregisterEventCallback();
            _testingContainer.bcWrapper.Client.DeregisterRewardCallback();
            _testingContainer.CleanUp();
            Destroy(_gameObject);
            _testingContainer = null;
            _successCount = 0;
            Debug.Log("Tearing Down....");
        }
    
        [SetUp]
        public void SetUp()
        {
            LoadIds();
            _gameObject = Instantiate(new GameObject("TestingContainer"), Vector3.zero, Quaternion.identity);
            _testingContainer = _gameObject.AddComponent<TestContainer>();
            
            _testingContainer.bcWrapper = _gameObject.AddComponent<BrainCloudWrapper>();
            _testingContainer.bcWrapper.Init(ServerUrl, Secret, AppId, Version);
            
            _testingContainer.bcWrapper.Client.EnableLogging(true);
            _testingContainer.bcWrapper.Client.RegisterLogDelegate(HandleLog);
        }

        /// <summary>
        /// Routine loads up brainCloud configuration info from "tests/ids.txt" (hopefully)
        /// in a platform agnostic way.
        /// </summary>
        private void LoadIds()
        {
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
    }
}