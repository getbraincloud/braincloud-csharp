using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestPlayerState : TestFixtureBase
    {

        [UnityTest]
        public IEnumerator TestUpdateCountryCode()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));

            var data = _tc.m_response["data"] as Dictionary<string, object>;
            string startingCountryCode = data["countryCode"] as string;
            
            _tc.bcWrapper.PlayerStateService.UpdateCountryCode("CL", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            data = _tc.m_response["data"] as Dictionary<string, object>;
            string updatedCountryCode = data["countryCode"] as string;
            bool result = !startingCountryCode.Equals(updatedCountryCode);
            
            LogResults("Country Code wasn't updated", result);
        }
    }
}