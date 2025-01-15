using System;
using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestPlaybackStream : TestFixtureBase
    {
        private string _playbackStreamId;
        private string _userAProfileId;

        [UnityTest]
        public IEnumerator TestStartStream()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _userAProfileId = _tc.bcWrapper.GetStoredProfileId();

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestEndStream()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string playbackStreamId = string.Empty;

            SuccessCallback success = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("playbackStreamId"))
                {
                    playbackStreamId = dataObj["playbackStreamId"] as string;
                }

                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, success, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.PlaybackStreamService.EndStream(playbackStreamId, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestReadStream()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string playbackStreamId = string.Empty;

            SuccessCallback success = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("playbackStreamId"))
                {
                    playbackStreamId = dataObj["playbackStreamId"] as string;
                }

                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, success, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.PlaybackStreamService.ReadStream(playbackStreamId, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestAddEvent()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string playbackStreamId = string.Empty;

            SuccessCallback success = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("playbackStreamId"))
                {
                    playbackStreamId = dataObj["playbackStreamId"] as string;
                }

                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, success, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            Dictionary<string, object> eventData = new Dictionary<string, object>();
            eventData["value"] = 1;
            Dictionary<string, object> summaryData = new Dictionary<string, object>();
            summaryData["total"] = 5;

            string eventDataStr = _tc.bcWrapper.Client.SerializeJson(eventData);
            string summaryDataStr = _tc.bcWrapper.Client.SerializeJson(summaryData);

            _tc.bcWrapper.PlaybackStreamService.AddEvent(playbackStreamId, eventDataStr, summaryDataStr, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetRecentStreamsForInitiatingPlayer()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            _tc.bcWrapper.PlaybackStreamService.GetRecentStreamsForInitiatingPlayer(_tc.bcWrapper.GetStoredProfileId(), 5, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetRecentStreamsForTargetPlayer()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string profileIdA = _tc.bcWrapper.GetStoredProfileId();

            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserB));

            _tc.bcWrapper.PlaybackStreamService.GetRecentStreamsForInitiatingPlayer(profileIdA, 5, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestProtectStreamUntil()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string playbackStreamId = string.Empty;

            SuccessCallback success = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("playbackStreamId"))
                {
                    playbackStreamId = dataObj["playbackStreamId"] as string;
                }

                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, success, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.PlaybackStreamService.ProtectStreamUntil(playbackStreamId, 365, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestDeleteStream()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string playbackStreamId = string.Empty;

            SuccessCallback success = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("playbackStreamId"))
                {
                    playbackStreamId = dataObj["playbackStreamId"] as string;
                }

                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.PlaybackStreamService.StartStream(_tc.bcWrapper.GetStoredProfileId(), false, success, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.PlaybackStreamService.DeleteStream(playbackStreamId, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }
    }
}