using System.Collections;
using System.Collections.Generic;
using BrainCloud;
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
        }

        [UnityTest]
        public IEnumerator TestAuthenticateAnonymous()
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
            Debug.Log($"Failed Count {_tc.failCount}");
            Assert.True(_tc.failCount == 2);
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
            Assert.True(_tc.successCount == 1);
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
            Assert.True(_tc.successCount == 1);
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
            Assert.True(_tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestResetEmailPasswordAdvanced()
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
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithKoreanLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "kn";
            _tc.bcWrapper.Client.CountryCode = "kn";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            Assert.True(_tc.successCount >= 4);
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateWithJapaneseLang()
        {
            _tc.bcWrapper.Client.LanguageCode = "ja";
            _tc.bcWrapper.Client.CountryCode = "ja";
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA, false));
            Debug.Log($"Success Count: {_tc.successCount}");
            Assert.True(_tc.successCount >= 4);
        }
    }    
}

