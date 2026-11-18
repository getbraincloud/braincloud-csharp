// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Tests.PlayMode
{
    /*
     * TestFixtureBase is mainly used
     *  - Hold braincloud init parameters
     *  - Set up UnityTests
     *  - Tear down UnityTests
     *  - Handle console logging
     *  - Init from the brainCloud Settings window's config (restored by bccm from data/)
     */
    public class TestFixtureBase : MonoBehaviour
    {
        // From the settings config; the rest are fixed values the test app expects.
        public string ServerUrl;
        public string AppId;
        public string Version;
        public string ChildAppId;
        public string ParentLevel = "Master";
        public string PeerName = "peerapp";
        
        protected GameObject _gameObject;
        protected TestContainer _tc;
        protected bool _isRunning;
        
        protected string username = "UnityTestUser";
        protected string email = "UnityTestUser@fakedomain.com";
        protected string password = "testPass";
        protected bool forceCreate = true;
        private bool isInitialized;
        
        private JsonWriterSettings _writerSettings = new JsonWriterSettings
        {
            PrettyPrint = true,
            Tab = "  "
        };
        
        [TearDown]
        public virtual void TearDown()
        {
            Debug.Log("Tearing Down....");
            if(_tc != null && _tc.bcWrapper != null && _tc.bcWrapper.Client != null)
            {
                _tc.bcWrapper.Client.FlushCachedMessages(false);
                _tc.bcWrapper.Client.ResetCommunication();
                _tc.bcWrapper.Client.DeregisterEventCallback();
                _tc.bcWrapper.Client.DeregisterRewardCallback();
                _tc.bcWrapper.Client.DeregisterFileUploadCallback();
                _tc.bcWrapper.Client.DeregisterFileUploadCallback();
                _tc.bcWrapper.Client.DeregisterGlobalErrorCallback();
                _tc.bcWrapper.Client.DeregisterNetworkErrorCallback();
                _tc.CleanUp();
                Destroy(_tc.bcWrapper);
            }
            
            var listOfContainers = FindObjectsOfType<Transform>();
            foreach (Transform container in listOfContainers)
            {
                if (container.name.Contains("TestingContainer"))
                {
                    Destroy(container.gameObject);
                }
            }

            isInitialized = false;
            _tc = null;
        }
    
        [SetUp]
        public virtual void SetUp()
        {
            if (isInitialized) return;
            isInitialized = true;
            Debug.Log("Setting Up......");
            
            _gameObject = Instantiate(new GameObject("TestingContainer"), Vector3.zero, Quaternion.identity);
            _tc = _gameObject.AddComponent<TestContainer>();

            _tc.bcWrapper = _gameObject.AddComponent<BrainCloudWrapper>();
            _tc.bcWrapper.Init();
            Assert.That(_tc.bcWrapper.Client.Initialized, "No readable brainCloud settings. Run `node bccm test csharp_unity` to restore them from data/.");
            ServerUrl = _tc.bcWrapper.Client.GetUrl();
            AppId = _tc.bcWrapper.Client.AppId;
            Version = _tc.bcWrapper.Client.AppVersion;
            var children = _tc.bcWrapper.GetChildAppIdList();
            ChildAppId = children.Count > 0 ? children[0] : "";
            _tc.bcWrapper.ResetStoredProfileId();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.Client.EnableLogging(true);
            _tc.bcWrapper.Client.RegisterLogDelegate(HandleLog);
        }

        // Re-inits on another url or version with the settings' main app.
        protected void InitAt(string url, string version)
        {
            _tc.bcWrapper.Init(url, AppProfile(), AppId, version);
        }

        // The settings' signing profile, read back from the wrapper's own client.
        private System.Func<byte[], string> AppProfile()
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            var client = _tc.bcWrapper.Client;
            var comms = client.GetType().GetProperty("Comms", flags).GetValue(client);
            return (System.Func<byte[], string>)comms.GetType().GetMethod("GetAppProfile", flags).Invoke(comms, null);
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
        
        protected byte[] ConvertFileToBytes(FileInfo fileInfo)
        {
            return System.IO.File.ReadAllBytes(fileInfo.FullName);
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
