using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureNoAuth
    {
        [TearDown]
        public void Cleanup()
        {
            _bc.Client.OverrideCountryCode(null);
            _bc.Client.OverrideLanguageCode(null);
        }

        [Test]
        public void TestAuthenticateSpam()
        {
            // //our problem is that users who find they can't log in, will retry over and over until they have success. They do not change their credentials while doing this.
            // //This threatens our servers, because huge numbers of errors related to the profileId not matching the anonymousId show up, as the user continues to have this retry. 
            // //Our goal is to stop this by checking to see if the call being made was an authentication call, then seeing if the attempted parameters for the authenticate were the
            // //same. If they were, we know they're simply retrying, and retrying, and we can send a client error saying that the credentials have already been retried. 

            // //start test by initializing an anonymous Id and profileID
            // string anonId = _bc.Client.AuthenticationService.GenerateAnonymousId();
            // _bc.Client.AuthenticationService.Initialize("randomProfileId", anonId);

            // //in this test case I am going to spam the anonymous authentication 4 times, I expect failure to happen with these calls. When the failure calls exceed 
            // //the max that we allow, the client becomes disabled due to repeated errors from the authentication call.  
            // TestResult tr = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr.ApiSuccess, tr.ApiError
            //  ); 
            // tr.RunExpectFail(202, 40207);

            // TestResult tr2 = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr2.ApiSuccess, tr2.ApiError
            //  ); 
            // tr2.RunExpectFail(202, 40207);

            // TestResult tr3 = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr3.ApiSuccess, tr3.ApiError
            //  ); 
            // tr3.RunExpectFail(202, 40207);

            // TestResult tr4 = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr4.ApiSuccess, tr4.ApiError
            //  ); 
            // tr4.RunExpectFail(202, 40207);

            // //At this point, I expect we the max error count and the client is disabled 
            // //currently testing it as disabled for 0.01 seconds so it should skip this call.
            // TestResult tr5 = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr5.ApiSuccess, tr5.ApiError
            //  ); 
            // tr5.RunExpectFail(900, 90200);

            // //The timer should finish by this call, and you will see that they can call authenticate again. 
            // TestResult tr6 = new TestResult(_bc);
            // _bc.Client.AuthenticationService.AuthenticateAnonymous(
            //     "",
            //    true,
            //    tr6.ApiSuccess, tr6.ApiError
            //  );  
            //  tr6.RunExpectFail(202, 40207);


                        //our problem is that users who find they can't log in, will retry over and over until they have success. They do not change their credentials while doing this.
            //This threatens our servers, because huge numbers of errors related to the profileId not matching the anonymousId show up, as the user continues to have this retry. 
            //Our goal is to stop this by checking to see if the call being made was an authentication call, then seeing if the attempted parameters for the authenticate were the
            //same. If they were, we know they're simply retrying, and retrying, and we can send a client error saying that the credentials have already been retried. 

            //start test by initializing an anonymous Id and profileID
            string anonId = _bc.Client.AuthenticationService.GenerateAnonymousId();
            _bc.Client.AuthenticationService.Initialize("randomProfileId", anonId);

            //in this test case I am going to spam the anonymous authentication 4 times, I expect failure to happen with these calls. When the failure calls exceed 
            //the max that we allow, the client becomes disabled due to repeated errors from the authentication call.  
            TestResult tr = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr.ApiSuccess, tr.ApiError
             ); 
            tr.RunExpectFail(202, 40207);

            TestResult tr2 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr2.ApiSuccess, tr2.ApiError
             ); 
            tr2.RunExpectFail(202, 40207);

            TestResult tr3 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr3.ApiSuccess, tr3.ApiError
             ); 
            tr3.RunExpectFail(202, 40207);

            TestResult tr4 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr4.ApiSuccess, tr4.ApiError
             ); 
            tr4.RunExpectFail(202, 40207);

            TestResult tr5 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr5.ApiSuccess, tr5.ApiError
             ); 
            tr5.RunExpectFail(900, 90200);

            TestResult tr6 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr6.ApiSuccess, tr6.ApiError
             );  
             tr6.RunExpectFail(900, 90200);

            TestResult tr7 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr7.ApiSuccess, tr7.ApiError
             );  
             tr7.RunExpectFail(202, 40207);

            TestResult tr8 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr8.ApiSuccess, tr8.ApiError
             );  
             tr8.RunExpectFail(202, 40207);
        }

        [Test]
        public void TestAuthenticateUniversal()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestAuthenticateHandoff()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateHandoff(
                "invalid_handOffId",
                "invalid_securityToken",
                tr.ApiSuccess, tr.ApiError);

            //expect token to not match user
            tr.RunExpectFail(403, ReasonCodes.TOKEN_DOES_NOT_MATCH_USER);
        }   

        [Test]
        public void TestAuthenticateAnonymous()
        {
            string anonId = _bc.Client.AuthenticationService.GenerateAnonymousId();
            _bc.Client.AuthenticationService.Initialize("randomProfileId", anonId);

            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(202, 40207);
        }

        [Test]
        public void TestAuthenticateEmailPassword()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetEmailPassword()
        {
            TestResult tr = new TestResult(_bc);

            string email = "braincloudunittest@gmail.com";

            _bc.Client.AuthenticationService.ResetEmailPassword(
                email,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
        
        [Test]
        public void TestResetEmailPasswordAdvanced()
        {
            TestResult tr = new TestResult(_bc);
        
            string email = "braincloudunittest@gmail.com";
            string content = "{\"fromAddress\": \"fromAddress\",\"fromName\": \"fromName\",\"replyToAddress\": \"replyToAddress\",\"replyToName\": \"replyToName\", \"templateId\": \"8f14c77d-61f4-4966-ab6d-0bee8b13d090\",\"subject\": \"subject\",\"body\": \"Body goes here\", \"substitutions\": { \":name\": \"John Doe\",\":resetLink\": \"www.dummuyLink.io\"}, \"categories\": [\"category1\",\"category2\" ]}";

            _bc.Client.AuthenticationService.ResetEmailPasswordAdvanced(
                email,
                content,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_FROM_ADDRESS);
        }

        [Test]
        public void TestAuthenticateWithHeartbeat()
        {
            TestResult tr = new TestResult(_bc);

            // Insert heartbeat as first packet. This would normally cause the
            // server to reject the second authenticate packet but with the
            // new comms change, this should result in the heartbeat being
            // removed from the message bundle.
            _bc.Client.SendHeartbeat();

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetCountryCode()
        {
            TestResult tr = new TestResult(_bc);

            string countryCode = "RU";

            _bc.Client.OverrideCountryCode(countryCode);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string code = data["countryCode"] as string;
            Assert.AreEqual(countryCode, code);
        }

        [Test]
        public void TestSetLanguageCode()
        {
            TestResult tr = new TestResult(_bc);

            string languageCode = "ru";

            _bc.Client.OverrideLanguageCode(languageCode);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string code = data["languageCode"] as string;
            Assert.AreEqual(languageCode, code);
        }

        [Test]
        public void TestAuthFirst()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.ReadUserState(tr.ApiSuccess, tr.ApiError);

            _bc.Client.AuthenticationService.AuthenticateEmailPassword(
                GetUser(Users.UserA).Email,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            tr.Run();
        }

    }
}