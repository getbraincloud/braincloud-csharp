using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using NUnit.Framework;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestComms : TestFixtureBase
    {
        private BrainCloudWrapper tempWrapper;
        private int _globalErrorCount;
        [UnityTest]
        public IEnumerator TestNoSession()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.PlayerStateService.Logout(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION));
            Assert.IsTrue(_tc.failCount == 2);
        }

        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator TestSessionStaysAlive()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            Debug.Log("Waiting for session if it does expire...");

            yield return new WaitForSeconds(61 * 5);    
            
            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }
        
        [UnityTest]
        [Timeout(100000000)]
        public IEnumerator TestSessionTimeout()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.bcWrapper.Client.EnableCommunications(true);
            _tc.bcWrapper.Client.BlockSendingHeartbeats(true);

            var timeElapsed = 0;
            var heartBeatInterval = 0;
            Debug.Log("Waiting for session to expire...");
            while (timeElapsed < (61 * 5))
            {
                timeElapsed += 10;
                Debug.Log($"Time Elapsed: {timeElapsed}");
                //_tc.bcWrapper.Update();
                yield return new WaitForSeconds(10);
            }
            
            _tc.bcWrapper.Client.BlockSendingHeartbeats(false);
            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.PLAYER_SESSION_EXPIRED));
            Debug.Log($"Status: {_tc.m_statusCode}");
            Debug.Log($"Reason: {_tc.m_reasonCode}");
            Assert.IsTrue(_tc.failCount == 2);
        }

        [UnityTest]
        public IEnumerator TestBadUrl()
        {
            CreateWrapper(ServerUrl + "unitTestFail");

            tempWrapper.Client.AuthenticationService.AuthenticateUniversal("abc", "abc", true, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Spin(tempWrapper));
            
            var failCount = 0;
            if (StatusCodes.CLIENT_NETWORK_ERROR == _tc.m_statusCode)
            {
                failCount++;
            }
            if (ReasonCodes.JSON_PARSING_ERROR == _tc.m_reasonCode)
            {
                failCount++;
            }
            
            Assert.IsTrue(failCount == 2);
        }

        [UnityTest]
        public IEnumerator TestErrorCallback()
        {
            CreateWrapper(ServerUrl);
            
            tempWrapper.EntityService.CreateEntity("type", "", "", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Spin(tempWrapper));
            Assert.False(_tc.m_result);
            Debug.Log(_tc.m_statusMessage);
            Assert.True(_tc.m_statusMessage.StartsWith("{"));
            
            tempWrapper.Client.SetOldStyleStatusMessageErrorCallback(true);
            _tc.Reset();
            
            tempWrapper.EntityService.CreateEntity("type", "", "", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Spin(tempWrapper));
            Assert.False(_tc.m_result);
            Debug.Log(_tc.m_statusMessage);
            Assert.False(_tc.m_statusMessage.StartsWith("{"));
            
            //try now using 900 client timeout
            tempWrapper.Init("http://localhost:5432", Secret, AppId, Version);
            
            _tc.Reset();
            tempWrapper.Client.SetOldStyleStatusMessageErrorCallback(false);
            
            tempWrapper.EntityService.CreateEntity("type", "", "", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Spin(tempWrapper));
            Assert.False(_tc.m_result);
            Debug.Log(_tc.m_statusMessage);
            Assert.True(_tc.m_statusMessage.StartsWith("{"));
            
            _tc.Reset();
            tempWrapper.Client.SetOldStyleStatusMessageErrorCallback(true);
            
            tempWrapper.EntityService.CreateEntity("type", "", "", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Spin(tempWrapper));
            Assert.False(_tc.m_result);
            Debug.Log(_tc.m_statusMessage);
            Assert.False(_tc.m_statusMessage.StartsWith("{"));
            
            tempWrapper.Client.SetOldStyleStatusMessageErrorCallback(false);
            tempWrapper.Client.ResetCommunication();
        }

        [UnityTest]
        public IEnumerator TestGlobalErrorCallback()
        {
            _tc.bcWrapper.Client.RegisterGlobalErrorCallback(GlobalErrorHandler);

            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.NO_SESSION));
            Assert.AreEqual(_tc.failCount,2);
            Assert.AreEqual(_globalErrorCount, 1);
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.TimeService.ReadServerTime(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.EntityService.UpdateEntity
                (
                    "fakeId",
                    "type",
                    Helpers.CreateJsonPair("test", 2),
                    null,
                    -1,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            _tc.failCount = 0;
            yield return _tc.StartCoroutine(_tc.RunExpectFail(404, 40332));
            Assert.AreEqual(_tc.failCount,2);
            Assert.AreEqual(_globalErrorCount, 2);
        }

        [UnityTest]
        public IEnumerator TestGlobalErrorCallbackUsingWrapper()
        {
            _tc.bcWrapper.Client.RegisterGlobalErrorCallback(GlobalErrorHandler);
            _tc.bcWrapper.AuthenticateUniversal("", "zzz", true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.RunExpectFail(StatusCodes.FORBIDDEN, ReasonCodes.TOKEN_DOES_NOT_MATCH_USER));
            Assert.AreEqual(_tc.failCount,2);
            Assert.AreEqual(_globalErrorCount, 1);
        }

        private void CreateWrapper(string url)
        {
            tempWrapper = _tc.gameObject.AddComponent<BrainCloudWrapper>();
            tempWrapper.Init(url, Secret, AppId, Version);
            tempWrapper.Client.EnableLogging(true);
        }
        
        private void GlobalErrorHandler(int status, int reasonCode, string jsonError, object cbObject)
        {
            if (cbObject != null)
            {
                if (cbObject.GetType().ToString() == "BrainCloud.Internal.WrapperAuthCallbackObject")
                {
                    Debug.Log("GlobalErrorHandler received internal WrapperAuthCallbackObject object: " + cbObject.GetType());
                    throw new Exception("GlobalErrorHandler received internal WrapperAuthCallbackObject object");
                }
            }
            _globalErrorCount++;
            Debug.Log("Global error: " + jsonError);
            Debug.Log("Callback object: " + cbObject);
        }
    }
}