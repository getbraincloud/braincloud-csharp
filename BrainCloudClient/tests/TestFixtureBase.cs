using System;
using System.IO;
using JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System.Text;

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

        [SetUp]
        public void Setup()
        {
            LoadIds();

            BrainCloudClient.Instance.Initialize(ServerUrl, Secret, AppId, Version);
            BrainCloudClient.Instance.EnableLogging(true);
            BrainCloudClient.Instance.RegisterLogDelegate(HandleLog);

            if (ShouldAuthenticate())
            {
                TestResult tr = new TestResult();
                BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
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
            BrainCloudClient.Instance.ResetCommunication();
            BrainCloudClient.Instance.DeregisterEventCallback();
            BrainCloudClient.Instance.DeregisterRewardCallback();
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
        /// <returns>If the swtich was successful</returns>
        protected bool GoToChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.SwitchToSingletonChildProfile(ChildAppId, true, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Convenience method to switch to the parent profile
        /// </summary>
        /// <returns>If the swtich was successful</returns>
        protected bool GoToParentProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.IdentityService.SwitchToParentProfile(ParentLevel, tr.ApiSuccess, tr.ApiError);
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
            string search = "Unity-Csharp" + Path.DirectorySeparatorChar + "BrainCloudClient";
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
                BrainCloudClient.Instance.EnableLogging(false);
                _testUsers = new TestUser[Enum.GetNames(typeof(Users)).Length];
                Random rand = new Random();

                for (int i = _testUsers.Length; i-- > 0;)
                {
                    _testUsers[i] = new TestUser(((Users)i).ToString() + "_CS" + "-", rand.Next());
                    Console.Write(".");
                }
                Console.Write("\n");
                BrainCloudClient.Instance.EnableLogging(true);
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

        public TestUser(string idPrefix, int suffix)
        {
            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            Authenticate();
        }

        private void Authenticate()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(
                Id,
                Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            ProfileId = BrainCloudClient.Instance.AuthenticationService.ProfileId;

            if (((string)((Dictionary<string, object>)tr.m_response["data"])["newUser"]) == "true")
            {
                BrainCloudClient.Instance.MatchMakingService.EnableMatchMaking(tr.ApiSuccess, tr.ApiError);
                tr.Run();
                BrainCloudClient.Instance.PlayerStateService.UpdatePlayerName(Id, tr.ApiSuccess, tr.ApiError);
                tr.Run();
                BrainCloudClient.Instance.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            BrainCloudClient.Instance.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}