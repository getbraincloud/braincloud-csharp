using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.Common;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace Tests.PlayMode
{
    public class TestIdentity : TestFixtureBase
    {
        private string testerEmail = "braincloudtest@gmail.com";
        private string blockchainConfig = "config";
        
        [UnityTest]
        public IEnumerator TestSwitchToChildProfile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.SwitchToChildProfile
                (
                    null,
                    ChildAppId,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.PlayerStateService.DeleteUser(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestSwitchToSingletonChildProfile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.SwitchToSingletonChildProfile
                (
                    ChildAppId,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestSwitchToParentProfile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.SwitchToSingletonChildProfile
                (
                    ChildAppId,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.IdentityService.SwitchToParentProfile
                (
                    ParentLevel,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestDetachParent()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            yield return _tc.StartCoroutine(_tc.GoToChildProfile(ChildAppId));

            _tc.bcWrapper.IdentityService.DetachParent(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestAttachParentWithIdentity()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            yield return _tc.StartCoroutine(_tc.GoToChildProfile(ChildAppId));
            
            _tc.bcWrapper.IdentityService.DetachParent(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.IdentityService.AttachParentWithIdentity
                (
                    _tc.TestUserA.Id,
                    _tc.TestUserA.Password,
                    AuthenticationType.Universal, 
                    null,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetChildProfiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.GetChildProfiles(true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }
        
        [UnityTest]
        public IEnumerator TestAttachEmailIdentity()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.AttachEmailIdentity
                (
                    "id_" + _tc.TestUserA.Email,
                    _tc.TestUserA.Password,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetIdentites()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.GetIdentities(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetExpiredIdentites()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.GetExpiredIdentities(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestRefreshIdentity()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.RefreshIdentity
                (
                    _tc.TestUserA.Id,
                    _tc.TestUserA.Password,
                    AuthenticationType.Universal,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.RunExpectFail(400,40464));
        }

        [UnityTest]
        public IEnumerator TestAttachPeerProfile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.AttachPeerProfile
                (
                    PeerName,
                    _tc.TestUserA.Id+"_peer",
                    _tc.TestUserA.Password,
                    AuthenticationType.Universal, 
                    null,
                    forceCreate,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());

            yield return _tc.StartCoroutine(_tc.DetachPeer(PeerName));
        }

        [UnityTest]
        public IEnumerator TestDetachPeer()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            yield return _tc.StartCoroutine(_tc.AttachPeer(PeerName, AuthenticationType.Universal));
            
            _tc.bcWrapper.IdentityService.DetachPeer
                (
                    PeerName,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetPeerProfiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.GetPeerProfiles(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestAttachNonLoginUniversalId()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.AttachNonLoginUniversalId(testerEmail, _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.RunExpectFail(202,ReasonCodes.DUPLICATE_IDENTITY_TYPE));
        }

        [UnityTest]
        public IEnumerator TestUpdateUniversalIdLogin()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.UpdateUniversalIdLogin(testerEmail, _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.RunExpectFail(400,ReasonCodes.NEW_CREDENTIAL_IN_USE));
        }

        [UnityTest]
        public IEnumerator TestAttachBlockChain()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.AttachBlockChain
                (
                    blockchainConfig,
                    "schwifty",
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.IdentityService.DetachBlockChain
                (
                    blockchainConfig,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
        }
    }   
}
