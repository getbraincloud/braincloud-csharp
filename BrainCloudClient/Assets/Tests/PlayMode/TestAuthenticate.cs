using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestAuthenticate : TestFixtureBase
    {
        private string _email = "UnityTestee@bctestuser.com";
        private string _universalID = "UserA_CS-1730261329";
        private string _password = "12345";
        private int additionalCalls = 0;
        
        [UnityTest]
        public IEnumerator TestAuthenticateSpam()
        {
            string anonId = _tc.bcWrapper.Client.AuthenticationService.GenerateAnonymousId();
            _tc.bcWrapper.Client.AuthenticationService.Initialize("randomProfileId", anonId);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));

            GameObject go2 = Instantiate(new GameObject("TestingContainer2"), Vector3.zero, Quaternion.identity);
            TestContainer tc2 = go2.AddComponent<TestContainer>();
            tc2.bcWrapper = _tc.bcWrapper;
            tc2.bcWrapper.Client.EnableLogging(true);
            tc2.bcWrapper.Client.RegisterLogDelegate(HandleLog);
            tc2.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                tc2.ApiSuccess,
                tc2.ApiError
            );
            yield return tc2.StartCoroutine(tc2.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));
            
            GameObject go3 = Instantiate(new GameObject("TestingContainer3"), Vector3.zero, Quaternion.identity);
            TestContainer tc3 = go3.AddComponent<TestContainer>();
            tc3.bcWrapper = _tc.bcWrapper;
            tc3.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                tc3.ApiSuccess,
                tc3.ApiError
            );
            yield return tc3.StartCoroutine(tc3.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));
            
            GameObject go4 = Instantiate(new GameObject("TestingContainer4"), Vector3.zero, Quaternion.identity);
            TestContainer tc4 = go4.AddComponent<TestContainer>();
            tc4.bcWrapper = _tc.bcWrapper;
            tc4.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                tc4.ApiSuccess,
                tc4.ApiError
            );
            yield return tc4.StartCoroutine(tc4.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));

            GameObject go5 = Instantiate(new GameObject("TestingContainer5"), Vector3.zero, Quaternion.identity);
            TestContainer tc5 = go5.AddComponent<TestContainer>();
            tc5.bcWrapper = _tc.bcWrapper;
            tc5.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                tc5.ApiSuccess,
                tc5.ApiError
            );
            yield return tc5.StartCoroutine(tc5.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.CLIENT_DISABLED_FAILED_AUTH));
            
            GameObject go6 = Instantiate(new GameObject("TestingContainer6"), Vector3.zero, Quaternion.identity);
            TestContainer tc6 = go6.AddComponent<TestContainer>();
            tc6.bcWrapper = _tc.bcWrapper;
            tc6.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                tc6.ApiSuccess,
                tc6.ApiError
            );
            yield return tc6.StartCoroutine(tc6.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));
        }

        [UnityTest]
        public IEnumerator TestAuthenticateUniversalTimeout()
        {
            _tc.m_timeToWaitSecs = 99999999;
            var successfulAuths = 0;
            //Using the version parameter for init as our sleep timer which will be caught with SleepPostAuth cloud code script
            //1000 = 1 second
            var timeoutDuration = 62000.ToString();
            _tc.bcWrapper.Init(ServerUrl, Secret, AppId, timeoutDuration);
            _tc.bcWrapper.Client.EnableLogging(true);
            _tc.bcWrapper.Client.RegisterLogDelegate(HandleLog);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return new WaitForFixedUpdate();
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return new WaitForFixedUpdate();
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return new WaitForFixedUpdate();
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            if (_tc.bcWrapper.Client.Authenticated)
            {
                successfulAuths++;
            }
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            if (_tc.bcWrapper.Client.Authenticated)
            {
                successfulAuths++;
            }

            Debug.Log(_tc.bcWrapper.Client.Authenticated);
            LogResults("Test was able to authenticate when we shouldn't" , successfulAuths == 0);
        }

        [UnityTest]
        public IEnumerator TestAdditionalAuthCalls()
        {
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            _tc.bcWrapper.Client.PlayerStateService.ReadUserState();
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                AdditionalApiSuccess,
                AdditionalApiError
            );
            _tc.bcWrapper.Client.EntityService.GetEntitiesByType("athletes");
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                AdditionalApiSuccess,
                AdditionalApiError
            );
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                AdditionalApiError
            );
            yield return _tc.StartCoroutine(_tc.Run(2));
            LogResults("Failed to append additional calls", additionalCalls == 2);
        }
        
        [UnityTest]
        public IEnumerator TestMixedAuthAndOtherCalls()
        {
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password + 1,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            _tc.bcWrapper.Client.PlayerStateService.ReadUserState(AdditionalApiSuccess, AdditionalApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.EntityService.GetEntitiesByType("athletes", AdditionalApiSuccess, AdditionalApiError);
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                "noobie",
                _password,
                false,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                "loopy",
                _password,
                false,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT));
            
            LogResults("Test was able to authenticate user when I shouldn't. Something went wrong with timeouts", !_tc.bcWrapper.Client.Authenticated);
        }

        [UnityTest]
        public IEnumerator TestURLUnavailableService()
        {
            _tc.m_timeToWaitSecs = 99999999;
            var url = "https://portal-swap.internal.braincloudservers.com/";
            _tc.bcWrapper.Init(url, Secret, AppId, Version);
            _tc.bcWrapper.Client.EnableLogging(true);
            _tc.bcWrapper.Client.RegisterLogDelegate(HandleLog);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            string anonId = _tc.bcWrapper.Client.AuthenticationService.GenerateAnonymousId();
            _tc.bcWrapper.Client.AuthenticationService.Initialize("randomProfileId", anonId);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Test was able to be authenticated to a bad url...", !_tc.bcWrapper.Client.Authenticated);
        }

        private TestContainer _testContainer;
        [UnityTest]
        public IEnumerator TestReauthenticateWithSpecificCallbacks()
        {
            /*
             * Note: Reauthenticate is a private method but it just calls AuthenticateAnoymous
             * User A
             * - Authenticate a session
             * - Wait somehow for the session to expire
             * - Call into Entity.UpdateSingleton, get session expiry error
             *      - Response from this should be to call AnnoymousAuthenticate request
             * User B
             * - Authenticate a session
             * - Wait the same way as User A for session to expire
             * - Call into PlayerStatistics.IncrementUserStats, get a session expiry error
             *      - Response from this should be to call Annoymouse Authenticate Request
             *
             * Goal:
             * - Reauthenticate once, NOT TWICE
             * - Call both Entity.UpdateSingleton & PlayerStatistics.IncrementUserStats when authenticated
             */
            _tc.bcWrapper.AuthenticateUniversal(_universalID, _password, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _testContainer = _gameObject.AddComponent<TestContainer>();
            _testContainer.bcWrapper = _tc.bcWrapper;
            
            
            Debug.Log("Make the session expire....");
            //Making the session expire
            _tc.bcWrapper.PlayerStateService.Logout(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            Dictionary<string, object> stats = new Dictionary<string, object> { { "highestScore", "RESET" } };
            _tc.bcWrapper.PlayerStatisticsService.IncrementUserStats
            (
                JsonWriter.Serialize(stats),
                _tc.ApiSuccess,
                ReauthSpecificErrorCallback,
                _tc
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            string updatedAddress = "1609 Bank St";
            string _entityType = "address";
            string _entityValueName = "street";
            var entityData = Helpers.CreateJsonPair(_entityValueName, updatedAddress);
            var entityAcl = new ACL() {Other = ACL.Access.ReadWrite}.ToJsonString(); 
            _testContainer.bcWrapper.EntityService.UpdateSingleton
            (
                _entityType,
                entityData,
                entityAcl,
                -1,
                _testContainer.ApiSuccess, 
                ReauthSpecificErrorCallback,
                _testContainer
            );
            yield return _testContainer.StartCoroutine(_testContainer.Run());
            
            //Both testing containers have ran a specific function and should have gotten a session expire(from the log out request specifically)
            if (_tc.failCount == 1 && _testContainer.failCount == 1)
            {
                _tc.successCount = 0;
                _testContainer.successCount = 0;
                //Responses has returned as failures, so we re authenticate as though each testing container is a single component
                _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess, _tc.ApiError);
                _testContainer.bcWrapper.AuthenticateAnonymous(_testContainer.ApiSuccess, _testContainer.ApiError);
                _testContainer.StartCoroutine(_testContainer.Run());
                _tc.StartCoroutine(_tc.Run());
                yield return new WaitUntil(() => _tc.m_done);
                yield return new WaitUntil(() => _testContainer.m_done);

                //Callbacks from re-authenticating will flip these booleans true
                //Now call the functions again with a valid session
                _tc.bcWrapper.PlayerStatisticsService.IncrementUserStats
                (
                    JsonWriter.Serialize(stats),
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
                _testContainer.bcWrapper.EntityService.UpdateSingleton
                (
                    _entityType,
                    entityData,
                    entityAcl,
                    -1,
                    _testContainer.ApiSuccess, 
                    _testContainer.ApiError
                );
                _tc.StartCoroutine(_tc.Run());
                _testContainer.StartCoroutine(_testContainer.Run());
                yield return new WaitUntil(() => _tc.m_done);
                yield return new WaitUntil(() => _testContainer.m_done);
            }
            LogResults("Not enough successful callbacks reached, something went wrong", _tc.successCount == 2 && _testContainer.successCount == 2);
        }

        private void ReauthSpecificErrorCallback(int statusCode, int reasonCode, string jsonError, object cb)
        {
            TestContainer objectContainer = cb as TestContainer;
            objectContainer.m_done = true;
            objectContainer.failCount++;
        }

        [UnityTest]
        public IEnumerator TestAuthenticateAnnyUpdateNameSpam()
        {
            string anonId = _tc.bcWrapper.Client.AuthenticationService.GenerateAnonymousId();
            _tc.bcWrapper.Client.AuthenticationService.Initialize("randomProfileId", anonId);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            _tc.bcWrapper.Client.PlayerStateService.UpdateName("UserA", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateEmailPassword
            (
                _email,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            _tc.bcWrapper.PlayerStateService.ReadUserState(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            _tc.bcWrapper.VirtualCurrencyService.GetCurrency("gems", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            _tc.bcWrapper.Client.PlayerStateService.UpdateName("UserB", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
             LogResults($"Failed to authenticate", _tc.bcWrapper.Client.Authenticated);
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateUniversal()
        {
            _tc.bcWrapper.AuthenticateUniversal
            (
                _universalID,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults($"Failed to authenticate", _tc.bcWrapper.Client.Authenticated);
        }

        [UnityTest]
        public IEnumerator TestFailAuthenticateAnonymous()
        {
            string anonId = _tc.bcWrapper.Client.AuthenticationService.GenerateAnonymousId();
            _tc.bcWrapper.Client.AuthenticationService.Initialize("randomProfileId", anonId);
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateAnonymous
            (
                "",
                false,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.ACCEPTED, ReasonCodes.SWITCHING_PROFILES));
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 3);
            Debug.Log($"expected result: status code ACCEPTED ||| reason code SWITCHING_PROFILES");
        }

        [UnityTest]
        public IEnumerator TestAuthenticateEmailPassword()
        {
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateEmailPassword
            (
                _email,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Failed to authenticate", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestResetEmailPassword()
        {
            _tc.bcWrapper.Client.AuthenticationService.ResetEmailPassword
            (
                _email,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to reset email password", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestResetUniversalIDPassword()
        {
            _tc.bcWrapper.AuthenticateUniversal(_universalID, _password, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.IdentityService.AttachEmailIdentity("thisIsAnEmail@domain.com", _password, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.successCount = 0;
            _tc.bcWrapper.Client.AuthenticationService.ResetUniversalIdPassword
            (
                _universalID,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to reset Universal ID password", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestResetEmailPasswordWithExpiry()
        {
            _tc.bcWrapper.Client.AuthenticationService.AuthenticateEmailPassword
            (
                _email,
                _password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.Client.AuthenticationService.ResetEmailPasswordWithExpiry
            (
                _email,
                1,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults(" Didn't receive enough successful calls while authenticating", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestFailResetEmailPasswordAdvanced()
        {
            string content = "{\"fromAddress\": \"fromAddress\",\"fromName\": \"fromName\",\"replyToAddress\": \"replyToAddress\",\"replyToName\": \"replyToName\", \"templateId\": \"8f14c77d-61f4-4966-ab6d-0bee8b13d090\",\"subject\": \"subject\",\"body\": \"Body goes here\", \"substitutions\": { \":name\": \"John Doe\",\":resetLink\": \"www.dummuyLink.io\"}, \"categories\": [\"category1\",\"category2\" ]}";

            _tc.bcWrapper.Client.AuthenticationService.ResetEmailPasswordAdvanced
            (
                _email,
                content,
                _tc.ApiSuccess,
                _tc.ApiError
            );

            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_FROM_ADDRESS));
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 3);
            Debug.Log($"expected result: status code BAD_REQUEST ||| reason code INVALID_FROM_ADDRESS");
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithKoreanLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "kn";
            _tc.bcWrapper.Client.CountryCode = "kn";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            LogResults("Didn't receive enough successful calls while authenticating", _tc.successCount == 1);
            
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithJapaneseLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "ja";
            _tc.bcWrapper.Client.CountryCode = "ja";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            LogResults("Didn't receive enough successful calls while authenticating", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestAuthenticateAdvanced()
        {
            AuthenticationIds ids;
            ids.externalId = "authAdvancedUser";
            ids.authenticationToken = "authAdvancedPass";
            ids.authenticationSubType = "";
            Dictionary<string, object> extraJson = new Dictionary<string, object>();
            extraJson["AnswerToEverything"] = 42;

            _tc.bcWrapper.AuthenticateAdvanced(AuthenticationType.Universal, ids, true, 
                extraJson, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to authenticate advanced", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateAnonymousAdvanced()
        {
            AuthenticationIds ids;
            ids.externalId = "advancedAnonymousUser";
            ids.authenticationToken = "specialToken";
            ids.authenticationSubType = "";
            Dictionary<string, object> extraJson = new Dictionary<string, object>();

            _tc.bcWrapper.AuthenticateAdvanced(AuthenticationType.Anonymous, ids, true,
                extraJson, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to authenticate advanced with anonymous as the type", _tc.bcWrapper.Client.Authenticated);
        }
        
        //Testing to see if profile & anonymous id's get saved properly
        [UnityTest]
        public IEnumerator TestReauthenticateAnonymousAdvanced()
        {
            AuthenticationIds ids;
            ids.externalId = "superNewUser";
            ids.authenticationToken = "newToken";
            ids.authenticationSubType = "";
            Dictionary<string, object> extraJson = new Dictionary<string, object>();

            _tc.bcWrapper.AuthenticateAdvanced(AuthenticationType.Anonymous, ids, true,
                extraJson, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.PlayerStateService.Logout(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.AuthenticateAdvanced(AuthenticationType.Anonymous, ids, false,
                extraJson, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Failed to reauthenticate", _tc.bcWrapper.Client.Authenticated);
            
            _tc.bcWrapper.PlayerStateService.DeleteUser(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestAuthenticateUltra()
        {
            if (!ServerUrl.Contains("api-internal.braincloudservers.com") &&
                !ServerUrl.Contains("internala.braincloudservers.com") &&
                !ServerUrl.Contains("api.internalg.braincloudservers.com"))
            {
                Debug.Log("This env doesn't support Ultra authentication type");
                Assert.True(true);
            }
            else
            {
                yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
                _tc.bcWrapper.ScriptService.RunScript("getUltraToken", "{}", _tc.ApiSuccess, _tc.ApiError);
                yield return _tc.StartCoroutine(_tc.Run());
            
                var data = _tc.m_response["data"] as Dictionary<string, object>;
                var response = data["response"] as Dictionary<string, object>;
                var data2 = response["data"] as Dictionary<string, object>;
                var json = data2["json"] as Dictionary<string, object>;
                string idToken = json["id_token"] as string;
            
                _tc.bcWrapper.PlayerStateService.Logout();
                yield return _tc.StartCoroutine(_tc.Spin());
            
                _tc.bcWrapper.AuthenticateUltra("braincloud1", idToken, true, _tc.ApiSuccess, _tc.ApiError);
                yield return _tc.StartCoroutine(_tc.Run());
            
                LogResults("Failed to authenticate ultra", _tc.successCount == 2);   
            }
        }

        private void AdditionalApiSuccess(string jsonResponse, object cbObject)
        {
            Debug.Log("Additional Callback successful");
            additionalCalls++;
        }

        private void AdditionalApiError(int statusCode, int reasonCode, string jsonError, object cb)
        {
            Debug.Log("Additional Callback failed");
            additionalCalls++;
        }
    }    
}

