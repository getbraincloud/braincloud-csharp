using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BrainCloud.JsonFx.Json;
using System.Text;

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

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadErrorBasic()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            const int JSON_DEPTH = 25;

            Dictionary<string, object> jsonPayload = MakeJsonOfDepth(JSON_DEPTH);

            bool failedTest = false;

            try { string dictionaryJson = _tc.bcWrapper.Client.SerializeJson(jsonPayload); }
            catch (JsonSerializationException e)
            {
                failedTest = true;
            }

            LogResults("Failed to catch json serialization exception", failedTest);
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadRequestError()
        {
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();

            AuthenticationIds ids = new AuthenticationIds();
            ids.externalId = username;
            ids.authenticationToken = password;

            Dictionary<string, object> extraJson = MakeJsonOfDepth(25);

            //Setting Max Depth to it's default value / a number not large enough to accomodate the payload.
            _tc.bcWrapper.Client.MaxDepth = 25;

            SuccessCallback successCallback = (response, cbObject) => {};

            FailureCallback failureCallback = (status, reasoncode, errormessage, cbObject) =>
            {
                LogAssert.Expect(LogType.Exception, "JsonSerializationException: You have exceeded the max json depth, increase the MaxDepth using the MaxDepth variable in BrainCloudClient.cs");
            };
            
            _tc.bcWrapper.AuthenticateAdvanced
                (
                    BrainCloud.Common.AuthenticationType.Universal,
                    ids,
                    forceCreate,
                    extraJson,
                    successCallback,
                    failureCallback
                );

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadResponseError()
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

            string entityID = "9c7685a2-5f3e-4a95-9be9-946456b93f63";

            _tc.bcWrapper.Client.MaxDepth = 25;

            SuccessCallback successCallback = (response, cbObject) => { };

            FailureCallback failureCallback = (status, reasoncode, errormessage, cbObject) =>
            {
                LogAssert.Expect(LogType.Exception, "JsonSerializationException: You have exceeded the max json depth, increase the MaxDepth using the MaxDepth variable in BrainCloudClient.cs");
            };

            _tc.bcWrapper.GlobalEntityService.ReadEntity(entityID, successCallback, failureCallback);
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadBasicMaxDepthAdjustment()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            const int JSON_DEPTH = 25;

            Dictionary<string, object> jsonPayload = MakeJsonOfDepth(JSON_DEPTH);

            _tc.bcWrapper.Client.MaxDepth = 50;

            string dictionaryJson = _tc.bcWrapper.Client.SerializeJson(jsonPayload);
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadRequestAdjustment()
        {
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();

            AuthenticationIds ids = new AuthenticationIds();
            ids.externalId = username;
            ids.authenticationToken = password;

            Dictionary<string, object> extraJson = MakeJsonOfDepth(25);

            //Increasing max depth to accomodate larger request payloads. 
            _tc.bcWrapper.Client.MaxDepth = 75;

            _tc.bcWrapper.AuthenticateAdvanced
            (
                BrainCloud.Common.AuthenticationType.Universal,
                ids,
                forceCreate,
                extraJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadResponseAdjustment()
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

            string entityID = "9c7685a2-5f3e-4a95-9be9-946456b93f63";

            _tc.bcWrapper.Client.MaxDepth = 75;

            _tc.bcWrapper.GlobalEntityService.ReadEntity(entityID, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        #region Helper Methods
        private Dictionary<string, object> MakeJsonOfDepth(int depth)
        {
            int JSON_DEPTH = depth;

            //Creating a Json payload to send that will exceed the default max depth of JsonWriter.
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();

            //Creaing the first child
            Dictionary<string, object> lastChild = new Dictionary<string, object>();
            lastChild.Add("child", "lastchild");
            dictionaryList.Add(lastChild);

            for (int i = 1; i < JSON_DEPTH; i++)
            {
                int targetChildIndex = i - 1;

                Dictionary<string, object> nextParent = new Dictionary<string, object>();
                nextParent.Add("child", dictionaryList[targetChildIndex]);
                dictionaryList.Add(nextParent);
            }

            return dictionaryList[JSON_DEPTH - 1];
        }

        #endregion
    }

}