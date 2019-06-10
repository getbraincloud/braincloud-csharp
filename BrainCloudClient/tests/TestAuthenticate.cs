using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAuthenticate : TestFixtureNoAuth
    {
        //putting in arbitrary value to tell if it's the first time updating these or not. 
        long mostRecentPacket = -1000000;
        long secondMostRecentPacket = -1000000;

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

            //start test by initializing an anonymous Id and profileID
            string anonId = _bc.Client.AuthenticationService.GenerateAnonymousId();
            _bc.Client.AuthenticationService.Initialize("randomProfileId", anonId);

            //in this test I purposefully fail 4 times so that 3 identical call will have been made after the first
            //I then allow a fifth call to show that whenever a call is made it will simply hit the client with a fake response.
            //Then I freeze the test in a while loop for 30 seconds to wait out the comms timer.
            //then I call authenticate again and you will notice that a call will be made to the server and everything reset.
            TestResult tr = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr.ApiSuccess, tr.ApiError
             );
            tr.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);

            TestResult tr2 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr2.ApiSuccess, tr2.ApiError
             );
            tr2.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);

            TestResult tr3 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr3.ApiSuccess, tr3.ApiError
             );
            tr3.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);

            TestResult tr4 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr4.ApiSuccess, tr4.ApiError
             );
            tr4.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);

            DateTime _testPauseStart = DateTime.Now;
            TimeSpan _testPauseDuration = TimeSpan.FromSeconds(35);

            //now that we've had out tests exceed the max failed tests, we will make a timer for the test to wait out the timer in comms.
            while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
            {
                //putting the test into a while loop until it passes this condition
            }

            //based on the order of logic in comms, this test will get a fake response before the timer is finished so we expect the fake response.
            TestResult tr5 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr5.ApiSuccess, tr5.ApiError
             );
            tr5.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_DISABLED_FAILED_AUTH);

            //Now that the timer has refreshed in comms after waiting out the time, we should now be able to call another authenticate call.
            TestResult tr6 = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateAnonymous(
                "",
               true,
               tr6.ApiSuccess, tr6.ApiError
             );
             tr6.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);
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

            tr.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES);
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
        public void TestResetUniversalIdPassword()
        {
            TestResult tr2 = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr2.ApiSuccess, tr2.ApiError);

            tr2.Run();

            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.ResetUniversalIdPassword(
                GetUser(Users.UserA).Id,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetUniversalIdPasswordAdvanced()
        {
            TestResult tr2 = new TestResult(_bc);

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr2.ApiSuccess, tr2.ApiError);

            tr2.Run();

            TestResult tr = new TestResult(_bc);

            string content = "{\"templateId\": \"d-template-id-guid\", \"substitutions\": { \":name\": \"John Doe\",\":resetLink\": \"www.dummuyLink.io\"}, \"categories\": [\"category1\",\"category2\" ]}";
            _bc.Client.AuthenticationService.ResetUniversalIdPasswordAdvanced(
                GetUser(Users.UserA).Id,
                content,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
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

        [Test]
        public void TestBadSignature()
        {
            //our problem is that users who refresh their app secret via the portal, the client would fail to read the response, and would retry infinitely.
            //This threatens our servers, because huge numbers of errors related to bad signature show up, and infinitely retry to get out of this error.
            //Instead of updating the signature via the portal, we will mimic a bad signature from the client.

            Dictionary<string, string> originalAppSecretMap = new Dictionary<string, string>();
            originalAppSecretMap.Add(AppId, Secret);
            originalAppSecretMap.Add(ChildAppId, ChildSecret);
            int numRepeatBadSigFailures = 0;


            // mess up the app
            Dictionary<string, string> updatedAppSecretMap = new Dictionary<string,string>(originalAppSecretMap);
            foreach (var kvPair in originalAppSecretMap)
            {
                updatedAppSecretMap[kvPair.Key] = kvPair.Value + "123";
            }


            // authenticate
            TestResult tr = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            if(tr.Run())
            {
                //Check the packet coming in and compare it to the last recevied packet. if they're both -1, we may be in a repeating scenario.
                if(mostRecentPacket == -1000000 && secondMostRecentPacket == -1000000)
                {
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + _bc.Client.GetReceivedPacketId());
                }
                else
                {
                    secondMostRecentPacket = mostRecentPacket;
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + mostRecentPacket);
                    Console.WriteLine("SECOND MOST RECENT PACKET " + secondMostRecentPacket);
                }

                //Is there the sign of a repeat?
                if(mostRecentPacket == -1 && secondMostRecentPacket == -1)
                {
                    numRepeatBadSigFailures++;
                }

                //we shouldnt expect more than 2 times that most recent and second most recent are both bad sig errors for this test, else its repeating itself. 
                if(numRepeatBadSigFailures > 2)
                {
                    Assert.Fail("Repeating bad sig error");
                }
            }


            //check state
            TestResult tr2 = new TestResult(_bc);
            _bc.PlayerStateService.ReadUserState(tr2.ApiSuccess, tr2.ApiError);
            if(tr2.Run())
            {
                //Check the packet coming in and compare it to the last recevied packet. if they're both -1, we may be in a repeating scenario.
                if(mostRecentPacket == -1000000 && secondMostRecentPacket == -1000000)
                {
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + _bc.Client.GetReceivedPacketId());
                }
                else
                {
                    secondMostRecentPacket = mostRecentPacket;
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + mostRecentPacket);
                    Console.WriteLine("SECOND MOST RECENT PACKET " + secondMostRecentPacket);
                }

                //Is there the sign of a repeat?
                if(mostRecentPacket == -1 && secondMostRecentPacket == -1)
                {
                    numRepeatBadSigFailures++;
                }

                //we shouldnt expect more than 2 times that most recent and second most recent are both bad sig errors for this test, else its repeating itself. 
                if(numRepeatBadSigFailures > 2)
                {
                    Assert.Fail("Repeating bad sig error");
                }
            }


            //init with bad secret
            TestResult tr3 = new TestResult(_bc);
            _bc.InitWithApps(ServerUrl, AppId, updatedAppSecretMap, Version);
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr3.ApiSuccess, tr3.ApiError);
            Console.WriteLine("AUTHENTICATION RESPONSE");
            if(tr3.RunExpectFail())
            {
                if(mostRecentPacket == -1000000 && secondMostRecentPacket == -1000000)
                {
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + _bc.Client.GetReceivedPacketId());
                }
                else
                {
                    secondMostRecentPacket = mostRecentPacket;
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + mostRecentPacket);
                    Console.WriteLine("SECOND MOST RECENT PACKET " + secondMostRecentPacket);
                }

                //Is there the sign of a repeat?
                if(mostRecentPacket == -1 && secondMostRecentPacket == -1)
                {
                    numRepeatBadSigFailures++;
                }
                
                //we shouldnt expect more than 2 times that most recent and second most recent are both bad sig errors for this test, else its repeating itself. 
                if(numRepeatBadSigFailures > 2)
                {
                    Assert.Fail("Repeating bad sig error");
                }
            }


            //check state
            TestResult tr4 = new TestResult(_bc);
            _bc.PlayerStateService.ReadUserState(tr4.ApiSuccess, tr4.ApiError);
            //going to have a bad sig because of bad initialize.
            tr4.RunExpectFail();


            // wait some time
            DateTime _testPauseStart = DateTime.Now;
            TimeSpan _testPauseDuration = TimeSpan.FromSeconds(5 * 0.05);

            //make a timer for the test.
            while (!(DateTime.Now.Subtract(_testPauseStart) >= _testPauseDuration))
            {
                //putting the test into a while loop until it passes this condition
                //float time = (float)DateTime.Now.Subtract(_testPauseStart).TotalSeconds;
                //Console.WriteLine(time);
            }


            //Try things with the proper secret.
            //Do a normal test.
            TestResult tr5 = new TestResult(_bc);
            // back to normal
            _bc.InitWithApps(ServerUrl, AppId, originalAppSecretMap, _bc.Client.AppVersion);
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr5.ApiSuccess, tr5.ApiError);
            //the packetId upon re-initializinf is going to change 
            if(tr5.RunExpectFail())
            {
                if(mostRecentPacket == -1000000 && secondMostRecentPacket == -1000000)
                {
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + _bc.Client.GetReceivedPacketId());
                }
                else
                {
                    secondMostRecentPacket = mostRecentPacket;
                    mostRecentPacket = _bc.Client.GetReceivedPacketId();
                    Console.WriteLine("MOST RECENT PACKET " + mostRecentPacket);
                    Console.WriteLine("SECOND MOST RECENT PACKET " + secondMostRecentPacket);
                }

                //Is there the sign of a repeat?
                if(mostRecentPacket == -1 && secondMostRecentPacket == -1)
                {
                    numRepeatBadSigFailures++;
                    
                }
                //we shouldnt expect more than 2 times that most recent and second most recent are both bad sig errors for this test, else its repeating itself. 
                if(numRepeatBadSigFailures > 2)
                {
                    Assert.Fail("Repeating bad sig error");
                }
            }

            TestResult tr6 = new TestResult(_bc);
            _bc.PlayerStateService.ReadUserState(tr6.ApiSuccess, tr6.ApiError);
            tr6.RunExpectFail();
        }
    }
}