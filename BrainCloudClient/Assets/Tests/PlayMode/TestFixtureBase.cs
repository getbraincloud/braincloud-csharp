using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;

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
        
        protected int _successCount = 0;
        protected GameObject _gameObject;
        protected TestContainer _testingContainer;
        
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
            //LoadIds();
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
        /// ToDo FL : Getting error for not having access to read ids.txt & unity doesnt like 'new StreamReader', need to come back later once a test is set up
        private void LoadIds()
        {
            string exePath = Application.dataPath;
            string absPath = exePath;
            string search = "tests";
            if (absPath.Contains(search))
            {
                absPath = absPath.Substring(0, absPath.LastIndexOf(search));
                absPath += search + Path.DirectorySeparatorChar + "Tests" + Path.DirectorySeparatorChar + "ids.txt";
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
    }
}