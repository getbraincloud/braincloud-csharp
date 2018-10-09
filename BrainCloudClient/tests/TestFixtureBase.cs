using System;
using System.IO;
using JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System.Text;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        protected string ServerUrl = "";
        protected string AppId = "";
        protected string Secret = "";
        protected string Version = "1.0.0";
        protected string ChildAppId = "";
        protected string ParentLevel = "";
        protected string PeerName = "";

        private JsonWriterSettings _writerSettings = new JsonWriterSettings { PrettyPrint = true, Tab = "  " };

        public BrainCloudWrapper _bc;

        [SetUp]
        public void Setup()
        {
            LoadIds();

            _bc = new BrainCloudWrapper();
            _bc.Init(ServerUrl, Secret, AppId, Version);
            _bc.Client.EnableLogging(true);
            _bc.Client.RegisterLogDelegate(HandleLog);

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
        }

        [TearDown]
        public void TearDown()
        {
            _bc.Client.ResetCommunication();
            _bc.Client.DeregisterEventCallback();
            _bc.Client.DeregisterRewardCallback();
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
                _bc.Client.EnableLogging(false);
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

            _bc.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}