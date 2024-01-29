using NUnit.Framework;
using BrainCloud;
using System.Diagnostics;
using System.Collections.Generic;


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
        public void TestWrapperLogoutAndClearProfileID()
        {
            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateUniversal("braincloudTester", "12345", true, tr.ApiSuccess, tr.ApiError);
            tr.Run();            
            _bc.Logout(true, tr.ApiSuccess, tr.ApiError);
            string profileID = _bc.GetStoredProfileId();
            if(profileID.Length == 0)
            {
                Assert.True(true);
            }
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

        
        [Test]
        public void TestReInit()
        {
            Dictionary<string, string> secretMap = new Dictionary<string, string>();
            secretMap.Add(AppId, Secret);
            secretMap.Add(ChildAppId, ChildSecret);

            //CASE 1

            //testing muliple attempts at Initializing in a row 
            int initCounter = 1;
            //try to init several times and see if everything works as intended
            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 1); //init called once
            initCounter++;

            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 2); //init called twice
            initCounter++;

            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 3); //inti called a third time

            //CASE 2
            //if we manage to get an authenticate our re-initializing was successful - now to test out users case 
            //AUTH
            TestResult tr = new TestResult(_bc);
            _bc.AuthenticateAnonymous(tr.ApiSuccess, tr.ApiError);
            tr.Run();
            TestResult tr2 = new TestResult(_bc);
            
            //DO A CALL
            _bc.TimeService.ReadServerTime(
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
            
            //RE-INIT
            _bc.InitWithApps(ServerUrl, AppId, secretMap, Version);
            TestResult tr3 = new TestResult(_bc);
            
            //Call WITHOUT AUTH - should fail because we have re-initialized and will need to authenticate again
            _bc.TimeService.ReadServerTime(
                tr3.ApiSuccess, tr3.ApiError);
            tr3.RunExpectFail();

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


    }
}
