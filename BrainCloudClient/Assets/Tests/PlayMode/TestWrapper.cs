using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestWrapper : TestFixtureBase
    {
        [UnityTest]
        public IEnumerator TestAuthenticateAnonymous()
        {
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();

            _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess, _tc.ApiError);
            
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            string profileId = _tc.bcWrapper.Client.AuthenticationService.ProfileId;
            string anonId = _tc.bcWrapper.Client.AuthenticationService.AnonymousId;
            
            Assert.AreEqual(profileId, _tc.bcWrapper.GetStoredProfileId());
            Assert.AreEqual(anonId, _tc.bcWrapper.GetStoredAnonymousId());
            
            _tc.bcWrapper.Client.PlayerStateService.Logout(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            Assert.AreEqual(profileId, _tc.bcWrapper.GetStoredProfileId());
            Assert.AreEqual(anonId, _tc.bcWrapper.GetStoredAnonymousId());
        }
        
        [UnityTest]
        public IEnumerator TestAuthenticateUniversal()
        {
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();

            _tc.bcWrapper.AuthenticateUniversal
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            Assert.AreEqual(0,_tc.m_apiCountExpected);
        }

        [UnityTest]
        public IEnumerator TestSmartSwitchAuthenticateEmailFromANonAuth()
        {
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();
            
            _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess,_tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.SmartSwitchAuthenticateEmail
                (
                    username,
                    password,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            Assert.AreEqual(0,_tc.m_apiCountExpected);
        }

        [UnityTest]
        public IEnumerator TestSmartSwitchAuthenticateEmailFromAuth()
        {
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();
            
            _tc.bcWrapper.AuthenticateUniversal
            (
                username + "WW",
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.SmartSwitchAuthenticateEmail
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());

            Assert.AreEqual(0,_tc.m_apiCountExpected);
        }

        [UnityTest]
        public IEnumerator TestSmartSwitchAuthenticateEmailFromNoAuth()
        {
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();
            
            _tc.bcWrapper.SmartSwitchAuthenticateEmail
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestWrapperResetEmailPassword()
        {
            _tc.bcWrapper.ResetEmailPassword("ryanr@bitheads.com", _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            Assert.AreEqual(0,_tc.m_apiCountExpected);
        }

        [UnityTest]
        public IEnumerator TestWrapperResetEmailPasswordAdvanced()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            string content = "{\"fromAddress\": \"fromAddress\",\"fromName\": \"fromName\",\"replyToAddress\": \"replyToAddress\",\"replyToName\": \"replyToName\", \"templateId\": \"8f14c77d-61f4-4966-ab6d-0bee8b13d090\",\"subject\": \"subject\",\"body\": \"Body goes here\", \"substitutions\": { \":name\": \"John Doe\",\":resetLink\": \"www.dummuyLink.io\"}, \"categories\": [\"category1\",\"category2\" ]}";
            
            _tc.bcWrapper.ResetEmailPasswordAdvanced
            (
                "ryanr@bitheads.com",
                content,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            //Expect To Fail
            yield return _tc.StartCoroutine(_tc.Run(1, false));
            //Assert.False(_tc.m_result);
            Assert.AreEqual(StatusCodes.BAD_REQUEST, _tc.m_statusCode);
            Assert.AreEqual(ReasonCodes.INVALID_FROM_ADDRESS, _tc.m_reasonCode);
        }

        [UnityTest]
        public IEnumerator TestReInit()
        {
            Dictionary<string, string> secretMap = new Dictionary<string, string>();
            secretMap.Add(AppId, Secret);
            secretMap.Add(ChildAppId, ChildSecret);
            
            //CASE 1

            //testing muliple attempts at Initializing in a row 
            int initCounter = 1;
            //try to init several times and see if everything works as intended
            _tc.bcWrapper.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 1); //init called once
            initCounter++;

            _tc.bcWrapper.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 2); //init called twice
            initCounter++;

            _tc.bcWrapper.InitWithApps(ServerUrl, AppId, secretMap, Version);
            Debug.Assert(initCounter == 3); //inti called a third time

            _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess, _tc.ApiError);
            
            //Run
            _tc.m_done = false;
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.Reset();
            
            GameObject gameObject2 = Instantiate(new GameObject("TestingContainer2"), Vector3.zero, Quaternion.identity);
            TestContainer _tc2 = gameObject2.AddComponent<TestContainer>();
            
            //DO A CALL
            _tc.bcWrapper.TimeService.ReadServerTime(_tc2.ApiSuccess,_tc2.ApiError);
            
            //Run
            _tc2.m_done = false;
            yield return _tc2.StartCoroutine(_tc2.Run());
            _tc2.Reset();
            
            //Re Init
            _tc.bcWrapper.InitWithApps(ServerUrl, AppId, secretMap, Version);
            
            GameObject gameObject3 = Instantiate(new GameObject("TestingContainer3"), Vector3.zero, Quaternion.identity);
            TestContainer _tc3 = gameObject3.AddComponent<TestContainer>();
            
            _tc.bcWrapper.TimeService.ReadServerTime(_tc3.ApiSuccess,_tc3.ApiError);
            
            //Run
            _tc3.m_done = false;
            yield return _tc3.StartCoroutine(_tc3.Run());
            _tc3.Reset();
            
            Assert.False(_tc3.m_result);
        }
    }
}