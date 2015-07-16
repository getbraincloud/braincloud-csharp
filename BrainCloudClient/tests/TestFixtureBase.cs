using System;
using System.IO;
using System.Reflection;
using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFixtureBase
    {
        protected string m_serverUrl = "";
        protected string m_appId = "";
        protected string m_secret = "";
        protected string m_version = "1.0.0";

        [SetUp]
        public void Setup()
        {
            LoadIds();

            BrainCloudClient.Get().Initialize(m_serverUrl, m_secret, m_appId, m_version);
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
        /// Routine loads up brainCloud configuration info from "tests/ids.txt" (hopefully)
        /// in a platform agnostic way.
        /// </summary>
        private void LoadIds()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
                        m_serverUrl = line.Substring(("serverUrl=").Length);
                        m_serverUrl.Trim();
                    }
                    else if (line.StartsWith("appId="))
                    {
                        m_appId = line.Substring(("appId=").Length);
                        m_appId.Trim();
                    }
                    else if (line.StartsWith("secret="))
                    {
                        m_secret = line.Substring(("secret=").Length);
                        m_secret.Trim();
                    }
                    else if (line.StartsWith("version="))
                    {
                        m_version = line.Substring(("version=").Length);
                        m_version.Trim();
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
                _testUsers = new TestUser[Enum.GetNames(typeof(Users)).Length];
                Random rand = new Random();

                for (int i = _testUsers.Length; i-- > 0; )
                {
                    _testUsers[i] = new TestUser(((Users)i).ToString() + "-", rand.Next());
                }

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
            tr.Reset();
            BrainCloudClient.Get().PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}