using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayMode
{
    public class TestUser : MonoBehaviour
    {
        public string Id = "";
        public string Password = "";
        public string ProfileId = "";
        public string Email = "";

        BrainCloudWrapper _bc;
        public bool IsRunning;
        private TestContainer _tc;
        
        public TestUser(BrainCloudWrapper bc, string idPrefix, int suffix)
        {
            _bc = bc;
            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            StartCoroutine(Authenticate());
        }

        public IEnumerator SetUp(BrainCloudWrapper bc, string idPrefix, int suffix, TestContainer testContainer)
        {
            _bc = bc;
            _tc = testContainer;
            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            IsRunning = true;
            StartCoroutine(Authenticate());
            while (!IsRunning)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator Authenticate()
        {
            _bc.Client.AuthenticationService.AuthenticateUniversal
            (
                Id,
                Password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            StartCoroutine(_tc.Run());
            while (_tc.IsRunning)
            {
                yield return new WaitForFixedUpdate();
            }
            
            ProfileId = _bc.Client.AuthenticationService.ProfileId;
            
            if (_tc.m_response.Count > 0 && ((string)((Dictionary<string, object>)_tc.m_response["data"])["newUser"]) == "true")
            {
                _bc.MatchMakingService.EnableMatchMaking(_tc.ApiSuccess, _tc.ApiError);
                StartCoroutine(_tc.Run());
                while (_tc.IsRunning)
                {
                    yield return new WaitForFixedUpdate();
                }
                _bc.PlayerStateService.UpdateUserName(Id, _tc.ApiSuccess, _tc.ApiError);
                StartCoroutine(_tc.Run());
                while (_tc.IsRunning)
                {
                    yield return new WaitForFixedUpdate();
                }
                _bc.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", _tc.ApiSuccess, _tc.ApiError);
                StartCoroutine(_tc.Run());
                while (_tc.IsRunning)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                Debug.Log("Got no response from Authentication");
            }
            IsRunning = false;
        }
    }
}

