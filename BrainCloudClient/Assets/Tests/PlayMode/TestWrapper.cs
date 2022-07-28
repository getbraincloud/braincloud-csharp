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

            bool testPassed = profileId == _tc.bcWrapper.GetStoredProfileId() &&
                          anonId == _tc.bcWrapper.GetStoredAnonymousId();
            LogResults("Profile Id or anonymous id did not match and failed the test", testPassed);
            if (profileId != _tc.bcWrapper.GetStoredProfileId())
            {
                Debug.Log("ERROR: Profile id did not match");
            }
            else if (anonId != _tc.bcWrapper.GetStoredAnonymousId())
            {
                Debug.Log("ERROR: Anonymous Id did not match");
            }
        }

        [UnityTest]
        public IEnumerator TestSleepCloudCodeScript()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.ScriptService.RunScript("SleepScript","", _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Stuff happened", _tc.m_done);
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
            
            LogResults("Authentication failed",_tc.successCount == 1);
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
            
            LogResults("Switching Authenticate email from anonymous auth failed",_tc.successCount == 2);
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
            
            LogResults("Failed to smart switch authenticat email from universal authentication", _tc.successCount == 2);
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
            LogResults("Failed to smart switch authenticat email from anonymous authentication", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestWrapperResetEmailPassword()
        {
            _tc.bcWrapper.ResetEmailPassword("ryanr@bitheads.com", _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Failed to reset email password", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestFailWrapperResetEmailPasswordAdvanced()
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
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_FROM_ADDRESS));
            LogResults("Failed to reset advanced email password", _tc.failCount == 3);
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
            
            //Assert.False(_tc3.m_result);
            LogResults("Failed to re initialize", _tc.successCount == 1);
        }
    }
}