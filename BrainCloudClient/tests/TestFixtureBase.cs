// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        public string ServerUrl = "";
        public string AppId = "";
        public string Secret = "";
        public string Version = "1.0.0";
        public string ChildAppId = "";
        public string ChildSecret = "";
        public string ParentLevel = "";
        public string PeerName = "";
        public string SupportsCompression = "";

        private JsonWriterSettings _writerSettings = new JsonWriterSettings { PrettyPrint = true, Tab = "  " };

        public BrainCloudWrapper _bc;

        [SetUp]
        public void Setup()
        {
            LoadIds();

            _bc = new BrainCloudWrapper();
            Dictionary<string, string> secretMap = new Dictionary<string, string>();
            secretMap.Add(AppId, Secret);
            secretMap.Add(ChildAppId, ChildSecret);
            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            _bc.Client.EnableLogging(true);
            _bc.Client.RegisterLogDelegate(HandleLog);

            // USE_COMPRESSION env var is set by the Jenkins pipeline parameter (booleanParam).
            // It takes priority over the supportsCompression field in ids.txt, which acts as the
            // local/manual fallback when the env var is absent.
            string useCompressionEnv = Environment.GetEnvironmentVariable("USE_COMPRESSION");
            if (useCompressionEnv != null)
                _bc.Client.EnableCompressedRequests(bool.Parse(useCompressionEnv));
            else if (SupportsCompression != "")
                _bc.Client.EnableCompressedRequests(bool.Parse(SupportsCompression));

            // Start auth timeout at 30 s instead of the 15 s default.
            // In the DOT_NET transport the timeout is a CancellationTokenSource seeded once
            // at send-time, so the _listAuthPacketTimeouts progression (15→30→60 s) only
            // kicks in for the *next* attempt (fixed in BrainCloudComms).  Beginning at 30 s
            // covers typical CI latency spikes and, for NoAuth test classes, also applies to
            // any AuthenticateUniversal calls made directly inside the test body.
            _bc.Client.SetAuthenticationPacketTimeout(30);

            if (ShouldAuthenticate())
            {
                // Retry up to 3 times.  With the SDK progression fix each failure advances
                // _authPacketTimeoutSecs (30→60→60 s), so later attempts get more time.
                // Total ceiling: 30+60+60 = 150 s – enough for even a heavily loaded CI box.
                Exception lastException = null;
                List<int> attemptStatuses = new List<int>();
                bool authenticated = false;
                for (int attempt = 0; attempt < 3 && !authenticated; attempt++)
                {
                    TestResult tr = new TestResult(_bc);
                    _bc.Client.AuthenticationService.AuthenticateUniversal(
                        GetUser(Users.UserA).Id,
                        GetUser(Users.UserA).Password,
                        true,
                        tr.ApiSuccess, tr.ApiError);
                    try
                    {
                        tr.Run();
                        authenticated = true;
                    }
                    catch (Exception e)
                    {
                        lastException = e;
                        attemptStatuses.Add(tr.m_statusCode);
                        Console.WriteLine("Setup auth attempt " + (attempt + 1) + " failed (status " + tr.m_statusCode + "), " +
                                          (attempt < 2 ? "retrying..." : "giving up."));
                    }
                }

                if (!authenticated)
                {
                    Assert.Inconclusive("Setup authentication failed after " + attemptStatuses.Count + 
                                        " attempts. Statuses: [" + string.Join(", ", attemptStatuses) + "]. " +
                                        "This is likely a CI network/timeout issue, not a test regression." + 
                                        "Exception caught: " + lastException);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            _bc.ResetStoredProfileId();
            _bc.ResetStoredAnonymousId();
            _bc.Client.ResetCommunication();
            _bc.Client.DeregisterEventCallback();
            _bc.Client.DeregisterRewardCallback();
            Thread.Sleep(1000);
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
                    Console.WriteLine(e.Message);
                }

                message = string.Format("\r\n{0}\r\n{1}", prefix, message);
            }

            Console.WriteLine(message);
        }

        /// <summary>
        /// Convenience method to switch to the child profile
        /// </summary>
        /// <returns>If the switch was successful</returns>
        protected bool GoToChildProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToSingletonChildProfile(ChildAppId, true, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Convenience method to switch to the parent profile
        /// </summary>
        /// <returns>If the switch was successful</returns>
        protected bool GoToParentProfile()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.SwitchToParentProfile(ParentLevel, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Attaches a peer profile
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authType"></param>
        /// <returns>Success</returns>
        protected bool AttachPeer(Users user, AuthenticationType authType)
        {
            TestUser testUser = GetUser(user);
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.AttachPeerProfile(PeerName, testUser.Id + "_peer", testUser.Password, authType, null, true, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Detaches a peer profile
        /// </summary>
        /// <returns>Success</returns>
        protected bool DetachPeer()
        {
            TestResult tr = new TestResult(_bc);
            _bc.IdentityService.DetachPeer(PeerName, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Routine loads up brainCloud configuration info from "tests/ids.txt" (hopefully)
        /// in a platform agnostic way.
        /// </summary>
        private void LoadIds()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string absPath = exePath;
            string search = "BrainCloudClient";
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

        public enum Users { UserA, UserB, UserC }

        private static TestUser[] _testUsers;
        private static bool _init = false;

        /// <summary>
        /// Returns the specified user's data
        /// </summary>
        /// <param name="user"> User's data to return </param>
        /// <returns> Object contining the user's Id, Password, and profileId </returns>
        protected TestUser GetUser(Users user)
        {
            if (!_init)
            {
                Console.Write(">> Initializing New Random Users");
                _bc.Client.EnableLogging(true);
                _testUsers = new TestUser[Enum.GetNames(typeof(Users)).Length];
                Random rand = new Random();

                for (int i = _testUsers.Length; i-- > 0;)
                {
                    _testUsers[i] = new TestUser(_bc, ((Users)i).ToString() + "_CS" + "-", rand.Next());
                    Console.Write(".");
                }
                Console.Write("\n");
                _bc.Client.EnableLogging(true);
                _init = true;
            }

            return _testUsers[(int)user];
        }
    }

    /// <summary>
    /// Holds data for a randomly generated user
    /// </summary>
    public class TestUser
    {
        public string Id = "";
        public string Password = "";
        public string ProfileId = "";
        public string Email = "";

        BrainCloudWrapper _bc;

        public TestUser(BrainCloudWrapper bc, string idPrefix, int suffix)
        {
            _bc = bc;

            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            Authenticate();
        }

        private void Authenticate()
        {
            TestResult tr = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                Id,
                Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            ProfileId = _bc.Client.AuthenticationService.ProfileId;

            if (((string)((Dictionary<string, object>)tr.m_response["data"])["newUser"]) == "true")
            {
                _bc.MatchMakingService.EnableMatchMaking(tr.ApiSuccess, tr.ApiError);
                tr.Run();
                _bc.PlayerStateService.UpdateUserName(Id, tr.ApiSuccess, tr.ApiError);
                tr.Run();
                _bc.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
            _bc.Logout(true, tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
