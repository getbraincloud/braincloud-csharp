using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BrainCloud;
using BrainCloud.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestFriend : TestFixtureBase
    {        
        [UnityTest]
        public IEnumerator TestGetProfileInfoForCredential()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
    
            _tc.bcWrapper.FriendService.GetProfileInfoForCredential(_tc.TestUserA.Id, AuthenticationType.Universal, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetProfileInfoForCredentialIfExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.FriendService.GetProfileInfoForCredentialIfExists(_tc.TestUserA.Id, AuthenticationType.Universal, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

    }
}