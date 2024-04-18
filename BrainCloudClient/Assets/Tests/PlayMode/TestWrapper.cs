using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BrainCloud.JsonFx.Json;
using System.Text;
using BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp;

namespace Tests.PlayMode
{
    public class TestWrapper : TestFixtureBase
    {
        private string emailToReset = "braincloudunittest@gmail.com";

        [OneTimeTearDown]
        public override void TearDown()
        {
            base.TearDown();
            Application.Quit();
        }

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

        string handoffID;
        string handoffToken;
        [UnityTest]
        public IEnumerator TestAuthenticateHandoff()
        {
            
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.ScriptService.RunScript("createHandoffId","{}", ApiHandoffSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.AuthenticateHandoff(handoffID, handoffToken, _tc.ApiSuccess, _tc.ApiError);
            
            //_tc.bcWrapper.Client.AuthenticationService.AuthenticateHandoff(handoffID, handoffToken, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Stuff happened", _tc.successCount == 1);
        }

        public void ApiHandoffSuccess(string json, object cb)
        {
            _tc.m_response = JsonReader.Deserialize<Dictionary<string, object>>(json);
            var data = _tc.m_response["data"] as Dictionary<string, object>;
            var response = data["response"] as Dictionary<string, object>;
            handoffID = response["handoffId"] as string;
            handoffToken = response["securityToken"] as string;
            Debug.Log($"HandoffID: {handoffID}");
            Debug.Log($"Token: {handoffToken}");
            _tc.m_done = true;
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
                    email,
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
                email,
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
                email,
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
            _tc.bcWrapper.ResetEmailPassword(emailToReset, _tc.ApiSuccess, _tc.ApiError);
            
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
                emailToReset,
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

            //DO A CALL
            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess,_tc.ApiError);
            
            //Run
            _tc.m_done = false;
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.Reset();
            
            //Re Init
            _tc.bcWrapper.InitWithApps(ServerUrl, AppId, secretMap, Version);
            yield return null;
            _tc.bcWrapper.AuthenticateAnonymous(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess,_tc.ApiError);
            //Run
            _tc.m_done = false;
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.Reset();
            
            //Assert.False(_tc3.m_result);
            LogResults("Failed to re initialize", _tc.successCount == 4);
        }

        [UnityTest]
        public IEnumerator TestDeepJsonPayloadRequestError()
        {
            LogAssert.ignoreFailingMessages = true;
            _tc.bcWrapper.Client.AuthenticationService.ClearSavedProfileID();
            _tc.bcWrapper.ResetStoredAnonymousId();
            _tc.bcWrapper.ResetStoredProfileId();
            
            AuthenticationIds ids = new AuthenticationIds();
            ids.externalId = username;
            ids.authenticationToken = password;
            
            //Setting Max Depth to it's default value / a number not large enough to accomodate the payload.
            _tc.bcWrapper.Client.MaxDepth = 25;
            
            Debug.Log("Max Depth:" + _tc.bcWrapper.Client.MaxDepth);
            Dictionary<string, object> extraJson = MakeJsonOfDepth(10);

            FailureCallback failureCallback = (status, reasoncode, errormessage, cbObject) =>
            {
                _tc.m_done = true;
                Debug.Log("failure callback reached");
            };
            
            _tc.bcWrapper.AuthenticateAdvanced
                (
                    BrainCloud.Common.AuthenticationType.Universal,
                    ids,
                    forceCreate,
                    extraJson,
                    _tc.ApiSuccess,
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

            _tc.bcWrapper.Client.MaxDepth = 1; //63 will make this operation have a successCallback rather than failureCallback
            Debug.Log("Max Depth:" + _tc.bcWrapper.Client.MaxDepth);
            FailureCallback failureCallback = (status, reasoncode, errormessage, cbObject) =>
            {
                _tc.m_done = true;
                Debug.Log("failure callback reached");
                _tc.failCount++;
            };
            _tc.bcWrapper.GlobalEntityService.ReadEntity(entityID, _tc.ApiSuccess, failureCallback);

            yield return _tc.StartCoroutine(_tc.Run());
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
        
        [UnityTest]
        public IEnumerator TestLogoutAndClearProfileID()
        {
            _tc.bcWrapper.AuthenticateUniversal
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            string profileId = _tc.bcWrapper.GetStoredProfileId();
            if(!profileId.IsNullOrEmpty())
            {
                _tc.successCount++;
            }
            _tc.bcWrapper.Logout(true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            profileId = _tc.bcWrapper.GetStoredProfileId();
            if(profileId.IsNullOrEmpty())
            {
                _tc.successCount++;
                Debug.Log("profile Id in wrapper is empty");
            }
            else
            {
                Debug.Log("profile Id in wrapper is still stored");
            }

            LogResults("Couldn't wipe profile ID when logging out.", _tc.successCount == 4);

        }
        
        [UnityTest]
        public IEnumerator TestLogoutAndSavedProfileID()
        {
            _tc.bcWrapper.AuthenticateUniversal
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            string profileId = _tc.bcWrapper.GetStoredProfileId();
            if(!profileId.IsNullOrEmpty())
            {
                _tc.successCount++;
            }
            _tc.bcWrapper.Logout(false, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            profileId = _tc.bcWrapper.GetStoredProfileId();
            if(!profileId.IsNullOrEmpty())
            {
                _tc.successCount++;
            }

            LogResults("ProfileID got wiped when logging out.", _tc.successCount == 4);
        }
        
        [UnityTest]
        public IEnumerator TestCanReconnectAfterLogout()
        {
            _tc.bcWrapper.AuthenticateUniversal
            (
                username,
                password,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.Logout(false, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            if(_tc.bcWrapper.CanReconnect())
            {
                _tc.successCount++;
            }
            LogResults("ProfileID got wiped when logging out.", _tc.successCount == 3);
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