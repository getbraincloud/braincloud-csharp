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
        private string _email = "UnityTester@bctestuser.com";
        private string _password = "12345";
        
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
    }    
}

