using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestEvent : TestFixtureBase
    {        
        [UnityTest]
        public IEnumerator TestSendEvent()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string eventData = "{\"someMapAttribute\": \"someValue\"}";

            _tc.bcWrapper.EventService.SendEvent(_tc.bcWrapper.GetStoredProfileId(), "eventType1", eventData, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestSendEventToProfiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserB));

            string profileIdB = _tc.bcWrapper.GetStoredProfileId();

            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            string profileIdA = _tc.bcWrapper.GetStoredProfileId();

            string[] toIds = new string[]{
                profileIdA, profileIdB};
            string eventData = "{'someMapAttribute': 'someValue'}";

            _tc.bcWrapper.EventService.SendEventToProfiles(toIds, "eventType1", eventData, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

    }
}