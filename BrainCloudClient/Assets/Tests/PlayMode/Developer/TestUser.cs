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
        }

        public IEnumerator SetUp(BrainCloudWrapper bc, string idPrefix, int suffix, TestContainer testContainer)
        {
            _bc = bc;
            _tc = testContainer;
            Id = idPrefix + suffix;
            Password = Id;
            Email = Id + "@bctestuser.com";
            IsRunning = true;
            
            yield return StartCoroutine(Authenticate());
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

            yield return StartCoroutine(_tc.Run());
            
            ProfileId = _bc.Client.AuthenticationService.ProfileId;
            
            if (_tc.m_response.Count > 0 && ((string)((Dictionary<string, object>)_tc.m_response["data"])["newUser"]) == "true")
            {
                _bc.MatchMakingService.EnableMatchMaking(_tc.ApiSuccess, _tc.ApiError);
                yield return StartCoroutine(_tc.Run());
                
                _bc.PlayerStateService.UpdateUserName(Id, _tc.ApiSuccess, _tc.ApiError);
                yield return StartCoroutine(_tc.Run());
                
                _bc.PlayerStateService.UpdateContactEmail("braincloudunittest@gmail.com", _tc.ApiSuccess, _tc.ApiError);
                yield return StartCoroutine(_tc.Run());
            }
            else
            {
                Debug.Log("Got no response from Authentication");
            }
            IsRunning = false;
        }
    }
}

