using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestGroupFile : TestFixtureBase
    {

        [UnityTest]
        public IEnumerator TestCheckFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.GroupFileService.CheckFilenameExists("123", "", "test.txt", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Something went wrong..", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestCheckFullpathFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.GroupFileService.CheckFullpathFilenameExists("123", "test.txt", _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Something went wrong..", _tc.successCount == 1);
        }
    }    
}

