using System.Collections;
using System.Collections.Generic;
using System.IO;
using BrainCloud;
using BrainCloud.Common;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestGroup : TestFixtureBase
    {
        [UnityTest]
        public IEnumerator TestUpdateGroupACL()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));

            GroupACL acl = new GroupACL();
            acl.Member = GroupACL.Access.ReadWrite;
            acl.Other = GroupACL.Access.None;

            string jsonData = "{}";
            string ownerAttributes = "{}";
            string defaultMemberAttributes = "{}";

            string myGroupId = "";

            int groupVersion = 0;

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

            SuccessCallback updateCallback = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("version"))
                {
                    groupVersion = (int)dataObj["version"];
                }
                _tc.ApiSuccess(response, cbObject);
            };

            _tc.bcWrapper.GroupService.UpdateGroupAcl(myGroupId, acl, updateCallback, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());

            _tc.bcWrapper.GroupService.DeleteGroup(myGroupId, groupVersion, _tc.ApiSuccess, _tc.ApiError);

            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestUpdateGroupEntityACL()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            //create group
            GroupACL acl = new GroupACL();
            acl.Member = GroupACL.Access.ReadWrite;
            acl.Other = GroupACL.Access.None;

            string jsonData = "{}";
            string ownerAttributes = "{}";
            string defaultMemberAttributes = "{}";

            string myGroupId = string.Empty;

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

            string myEntityId = string.Empty;
            //create group entity
            SuccessCallback successCallbackEntity = (response, cbObject) =>
            {
                Debug.Log(string.Format("Success | {0}", response));

                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("entityId"))
                {
                    myEntityId = dataObj["entityId"] as string;
                }
                _tc.ApiSuccess(response, cbObject);
            };
            _tc.bcWrapper.GroupService.CreateGroupEntity(myGroupId, "test", true, acl, jsonData, successCallbackEntity, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            //update group entity acl
            int entityVersion = 0;
            SuccessCallback updateEntityCallback = (response, cbObject) =>
            {
                var jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(response);
                var dataObj = jsonObj["data"] as Dictionary<string, object>;
                if (dataObj.ContainsKey("version"))
                {
                    entityVersion = (int)dataObj["version"];
                }
                _tc.ApiSuccess(response, cbObject);
            };


            _tc.bcWrapper.GroupService.UpdateGroupEntityAcl(myGroupId, myEntityId, acl, updateEntityCallback, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            //cleanup the entity and the group
            _tc.bcWrapper.GroupService.DeleteGroupEntity(myGroupId, myEntityId, entityVersion, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.GroupService.DeleteGroup(myGroupId, 1, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }
    }    
}

