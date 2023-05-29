using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
            
            if(_tc.m_response == null ||
               _tc.m_response.Count == 0)
            {
                Debug.Log("Got no response from Authentication");
            }
            IsRunning = false;
        }
        
        public IEnumerator SetUp(BrainCloudWrapper bc, TestContainer testContainer)
        {
            _bc = bc;
            _tc = testContainer;
            Random rand = new Random();
            Id = "unity_tester" + rand;
            Password = Id;
            Email = Id + "@bctestuser.com";
            IsRunning = true;
            
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
            
            if(_tc.m_response == null ||
               _tc.m_response.Count == 0)
            {
                Debug.Log("Got no response from Authentication");
            }
            IsRunning = false;
        }
    }
}

