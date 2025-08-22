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
            LogResults("Unable to switch to child profile", _tc.successCount == 2);
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
            LogResults("Unable to switch to singleton child profile", _tc.successCount == 1);
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
            LogResults("Unable to switch to parent profile", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestDetachParent()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            yield return _tc.StartCoroutine(_tc.GoToChildProfile(ChildAppId));

            _tc.bcWrapper.IdentityService.DetachParent(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to detach parent", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestAttachParentWithIdentity()
        {
            _tc.bcWrapper.AuthenticateUniversal
            (
                username,
                password,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            yield return _tc.StartCoroutine(_tc.GoToChildProfile(ChildAppId));
            
            _tc.bcWrapper.IdentityService.DetachParent(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.IdentityService.AttachParentWithIdentity
            (
                username,
                password,
                AuthenticationType.Universal,
                null,
                forceCreate,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            Debug.Log($"Success Count: {_tc.successCount}");
            LogResults("Unable to attach parent with identity", _tc.successCount == 4);
        }

        [UnityTest]
        public IEnumerator TestGetChildProfiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.GetChildProfiles(true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to get child profile", _tc.successCount == 1);
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
            LogResults("Unable to attach email identity", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestGetIdentites()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.GetIdentities(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to get identities", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestGetIdentityStatus()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.GetIdentityStatus(AuthenticationType.Universal, "",_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to get identity status", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestGetExpiredIdentites()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.IdentityService.GetExpiredIdentities(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to get expired identities", _tc.successCount == 1);
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
            Debug.Log($"expected result: status code BAD_REQUEST ||| reason code UNSUPPORTED_AUTH_TYPE");
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 3);
            
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
            LogResults($"Failure to attach peer profile",_tc.successCount == 1);
            
            //clean up
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
            LogResults($"Failure to either attach or detach peer",_tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestGetPeerProfiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.GetPeerProfiles(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to get peer profiles",_tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestAttachNonLoginUniversalId()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.AttachNonLoginUniversalId(testerEmail, _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.ACCEPTED,ReasonCodes.DUPLICATE_IDENTITY_TYPE));
            
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 3);
            Debug.Log($"expected result: status code ACCEPTED ||| reason code DUPLICATE_IDENTITY_TYPE");
        }

        [UnityTest]
        public IEnumerator TestUpdateUniversalIdLogin()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.IdentityService.UpdateUniversalIdLogin(testerEmail, _tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.BAD_REQUEST,ReasonCodes.NEW_CREDENTIAL_IN_USE));
            LogResults($"Failure expected, results need to be looked over in response",_tc.failCount == 3);
            Debug.Log($"expected result: status code BAD_REQUEST ||| reason code NEW_CREDENTIAL_IN_USE");
        }

        [UnityTest]
        public IEnumerator TestAttachBlockChain()
        {
            TearDown();
            SetUp();
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.IdentityService.AttachBlockChainIdentity
                (
                    blockchainConfig,
                    "schwifty2",
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to attach block chain",_tc.successCount == 1);
            
            //clean up
            _tc.bcWrapper.IdentityService.DetachBlockChainIdentity
                (
                    blockchainConfig,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
            
        }
    }   
}
