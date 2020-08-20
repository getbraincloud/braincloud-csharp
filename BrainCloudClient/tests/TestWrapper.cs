using NUnit.Framework;
using BrainCloud;


namespace BrainCloudTests
{
    [TestFixture]
    public class TestWrapper : TestFixtureNoAuth
    {
        [Test]
        public void TestAuthenticateAnonymous()
        {
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            string profileId = _bc.Client.AuthenticationService.ProfileId;
            string anonId = _bc.Client.AuthenticationService.AnonymousId;

            Assert.AreEqual(profileId, _bc.GetStoredProfileId());
            Assert.AreEqual(anonId, _bc.GetStoredAnonymousId());

            _bc.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Assert.AreEqual(profileId, _bc.GetStoredProfileId());
            Assert.AreEqual(anonId, _bc.GetStoredAnonymousId());
        }

        [Test]
        public void TestAuthenticateUniversal()
        {
            _bc.Client.AuthenticationService.ClearSavedProfileID();
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);

            _bc.AuthenticateUniversal(
                GetUser(Users.UserA).Id + "W",
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSmartSwitchAuthenticateEmailFromAnonAuth()
        {
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();
            

            tr = new TestResult(_bc);

            
            _bc.SmartSwitchAuthenticateEmail(
               "testAuth",
               "testPass",
               true,
               tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSmartSwitchAuthenticateEmailFromAuth()
        {
            _bc.Client.AuthenticationService.ClearSavedProfileID();
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);

            _bc.AuthenticateUniversal(
                GetUser(Users.UserA).Id + "WW",
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            _bc.SmartSwitchAuthenticateEmail(
               "testAuth",
               "testPass",
               true,
               tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSmartSwitchAuthenticateEmailFromNoAuth()
        {
            _bc.Client.AuthenticationService.ClearSavedProfileID();
            _bc.ResetStoredAnonymousId();
            _bc.ResetStoredProfileId();

            TestResult tr = new TestResult(_bc);

            _bc.SmartSwitchAuthenticateEmail(
                "testAuth",
                "testPass",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestWrapperResetEmailPassword()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ResetEmailPassword(
                "ryanr@bitheads.com",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        
        [Test]
        public void TestWrapperResetEmailPasswordAdvanced()
        {
            string content = "{\"fromAddress\": \"fromAddress\",\"fromName\": \"fromName\",\"replyToAddress\": \"replyToAddress\",\"replyToName\": \"replyToName\", \"templateId\": \"8f14c77d-61f4-4966-ab6d-0bee8b13d090\",\"subject\": \"subject\",\"body\": \"Body goes here\", \"substitutions\": { \":name\": \"John Doe\",\":resetLink\": \"www.dummuyLink.io\"}, \"categories\": [\"category1\",\"category2\" ]}";
            
            TestResult tr = new TestResult(_bc);

            _bc.ResetEmailPasswordAdvanced(
                "ryanr@bitheads.com",
                content,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_FROM_ADDRESS);
        }

        //[Test] //TODO Jon
        public void TestReconnect()
        {
            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Reconnect(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.TimeService.ReadServerTime(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        public void TestReInit()
        {
            // string ServerUrl = "https://internal.braincloudservers.com/dispatcherv2";
            // string AppId = "20001";
            // string Secret = "4e51b45c-030e-4f21-8457-dc53c9a0ed5f";
            // string ChildAppId = "20005";
            // string childSecret = "f8cec1cf-2f95-4989-910c-8caf598f83db"
            // string Version = "1.0.0";
            // Dictionary<string, string> secretMap = new Dictionary<string, string>();
            // secretMap.Add(AppId, Secret);
            // secretMap.Add(ChildAppId, ChildSecret);

            int initCounter = 1;
            //try to init several times and see if everything works as intended
            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 1); //init called once
            Console.WriteLine("Init test passed: " + initCounter + "/3");
            initCounter++;

            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 2); //init called twice
            Console.WriteLine("Init test passed: " + initCounter + "/3");
            initCounter++;

            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 3); //inti called a third time
            Console.WriteLine("Init test passed: " + initCounter + "/3");
            initTestPass = true;

            //if we manage to get an authenticate our re-initializing was successful
            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

    }
}