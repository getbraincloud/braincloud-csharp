using System;
using System.IO;
using System.Reflection;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        public readonly string ChildAppId = "10326";
        public readonly string ParentLevel = "Master";

        protected string _serverUrl = "";
        protected string _appId = "";
        protected string _secret = "";
        protected string _version = "1.0.0";

        [SetUp]
        public void Setup()
        {
            LoadIds();

            BrainCloudClient.Get().Initialize(_serverUrl, _secret, _appId, _version);
            BrainCloudClient.Get().EnableLogging(true);

            if (ShouldAuthenticate())
            {
                TestResult tr = new TestResult();
                BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(GetUser(Users.UserA).Id, GetUser(Users.UserA).Password, true, tr.ApiSuccess, tr.ApiError);
                if (!tr.Run())
                {
                    // what do we do on error?
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            BrainCloudClient.Get().ResetCommunication();
            BrainCloudClient.Get().DeregisterEventCallback();
            BrainCloudClient.Get().DeregisterRewardCallback();
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
        /// Convenience method to switch to the child profile
        /// </summary>
        /// <returns>If the swtich was successful</returns>
        protected bool GoToChildProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToSingletonChildProfile(ChildAppId, true, tr.ApiSuccess, tr.ApiError);
            return tr.Run();
        }

        /// <summary>
        /// Convenience method to switch to the parent profile
        /// </summary>
        /// <returns>If the swtich was successful</returns>
        protected bool GoToParentProfile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().IdentityService.SwitchToParentProfile(ParentLevel, tr.ApiSuccess, tr.ApiError);
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
                        _serverUrl = line.Substring(("serverUrl=").Length);
                        _serverUrl.Trim();
                    }
                    else if (line.StartsWith("appId="))
                    {
                        _appId = line.Substring(("appId=").Length);
                        _appId.Trim();
                    }
                    else if (line.StartsWith("secret="))
                    {
                        _secret = line.Substring(("secret=").Length);
                        _secret.Trim();
                    }
                    else if (line.StartsWith("version="))
                    {
                        _version = line.Substring(("version=").Length);
                        _version.Trim();
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
                BrainCloudClient.Get().EnableLogging(false);
                _testUsers = new TestUser[Enum.GetNames(typeof(Users)).Length];
                Random rand = new Random();

                for (int i = _testUsers.Length; i-- > 0;)
                {
                    _testUsers[i] = new TestUser(((Users)i).ToString() + "_CS" + "-", rand.Next());
                    Console.Write(".");
                }
                Console.Write("\n");
                BrainCloudClient.Get().EnableLogging(true);
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
            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                Id,
                Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            ProfileId = BrainCloudClient.Get().AuthenticationService.ProfileId;

            if (((string)((Dictionary<string, object>)tr.m_response["data"])["newUser"]) == "true")
            {
                BrainCloudClient.Get().MatchMakingService.EnableMatchMaking(tr.ApiSuccess, tr.ApiError);
                tr.Run();
                BrainCloudClient.Get().PlayerStateService.UpdatePlayerName(Id, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            BrainCloudClient.Get().PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}