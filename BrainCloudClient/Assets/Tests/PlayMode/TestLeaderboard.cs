using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestLeaderboard : TestFixtureBase
    {
        [UnityTest]
        public IEnumerator TestPostScoreToDynamicGroupLeaderboardUsingConfig()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            GroupACL acl = new GroupACL();
            acl.Member = GroupACL.Access.ReadWrite;
            acl.Other = GroupACL.Access.None;

            string jsonData = "{}";
            string ownerAttributes = "{}";
            string defaultMemberAttributes = "{}";

            string myGroupId = "";

            SuccessCallback successCallback = (response, cbObject) =>
            {
                Debug.Log(string.Format("Success | {0}", response));

                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("groupId"))
                {
                    myGroupId = dataObj["groupId"] as string;
                }
                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.GroupService.CreateGroup("myGroup", "test", true, acl, jsonData, ownerAttributes, defaultMemberAttributes, successCallback, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            string leaderboardId = "groupLeaderboardConfig";
            int score = 10;
            string scoreData = "{\"nickname\": \"batman\"}";
            string configJson = "{\"leaderboardType\": \"HIGH_VALUE\", \"rotationType\": \"DAYS\", \"numDaysToRotate\": 4, \"resetAt\": \"[[#ts+60000]]\", \"retainedCount\": 2, \"expireInMins\": \"None\"}";

            _tc.bcWrapper.LeaderboardService.PostScoreToDynamicGroupLeaderboardUsingConfig(leaderboardId, myGroupId, score, scoreData, configJson, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.GroupService.DeleteGroup(myGroupId, 1, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

    }
}