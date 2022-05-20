using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.Common;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestAuthenticate : TestFixtureBase
    {
        private bool _init;
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

            Debug.Log($"***************** NEW AUTH *****************************");
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
            
            Debug.Log($"***************** NEW AUTH *****************************");
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
            Debug.Log($"***************** NEW AUTH *****************************");
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
            Debug.Log($"***************** NEW AUTH *****************************");
            //yield return new WaitForSeconds(35);
            
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
            Debug.Log($"***************** NEW AUTH *****************************");
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

            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT));
            
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
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.CLIENT_NETWORK_ERROR, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT));
            if (_tc.bcWrapper.Client.Authenticated)
            {
                successfulAuths++;
            }
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
                AdditionalApiSuccess,
                AdditionalApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Failed to append additional calls", additionalCalls == 3);
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
            LogResults("Stuff Happened", !_tc.bcWrapper.Client.Authenticated);
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
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            Assert.True(_tc.bcWrapper.Client.Authenticated);
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
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 2);
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
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 2);
            Debug.Log($"expected result: status code BAD_REQUEST ||| reason code INVALID_FROM_ADDRESS");
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithKoreanLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "kn";
            _tc.bcWrapper.Client.CountryCode = "kn";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            LogResults("Didn't receive enough successful calls while authenticating", _tc.successCount >= 4);
            
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithJapaneseLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "ja";
            _tc.bcWrapper.Client.CountryCode = "ja";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            LogResults("Didn't receive enough successful calls while authenticating", _tc.successCount >= 4);
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

