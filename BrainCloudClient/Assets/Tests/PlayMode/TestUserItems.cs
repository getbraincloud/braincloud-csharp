// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestUserItems : TestFixtureBase
    {
        [UnityTest]
        public IEnumerator TestAwardUserItemWithOptionsFalse()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("blockIfExceedItemMaxStackable", false);
            string defId = "sword001";
            int quanitity = 6;
            bool includeDef = true;
            _tc.bcWrapper.UserItemsService.AwardUserItemWithOptions
            (
                defId,
                quanitity,
                includeDef,
                optionsJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to award user items",_tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestAwardUserItemWithOptionsTrue()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("blockIfExceedItemMaxStackable", true);
            string defId = "sword001";
            int quanitity = 6;
            bool includeDef = true;
            _tc.bcWrapper.UserItemsService.AwardUserItemWithOptions
            (
                defId,
                quanitity,
                includeDef,
                optionsJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to award user items",_tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetItemsOnPromotion()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));

            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("category", "Equipment");
            string shopId = "";
            bool includeDef = true;
            bool includePromotionDetails = true;
            _tc.bcWrapper.UserItemsService.GetItemsOnPromotion
            (
                shopId,
                includeDef,
                includePromotionDetails,
                optionsJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
        }

        [UnityTest]
        public IEnumerator TestGetPromotionDetails()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            string defId = "sword001";
            string shopId = "";
            bool includeDef = true;
            bool includePromotionDetails = true;
            _tc.bcWrapper.UserItemsService.GetItemPromotionDetails
            (
                defId,
                shopId,
                includeDef,
                includePromotionDetails,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to get promotion details",_tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestPurchaseUserItemWithOptionsFalse()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("blockIfExceedItemMaxStackable", false);
            string defId = "sword001";
            int quanitity = 6;
            bool includeDef = true;
            _tc.bcWrapper.UserItemsService.PurchaseUserItemWithOptions
            (
                defId,
                quanitity,
                null,
                includeDef,
                optionsJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to purchase item with options false",_tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestPurchaseUserItemWithOptionsTrue()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("blockIfExceedItemMaxStackable", true);
            string defId = "sword001";
            int quanitity = 6;
            bool includeDef = true;
            _tc.bcWrapper.UserItemsService.PurchaseUserItemWithOptions
            (
                defId,
                quanitity,
                null,
                includeDef,
                optionsJson,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to purchase item with options true",_tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestOpenBundle()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            Dictionary<string, object> optionsJson = new Dictionary<string, object>();
            optionsJson.Add("blockIfExceedItemMaxStackable", true);
            string bundleId = "equipmentBundle";
            int quanitity = 1;
            bool includeDef = true;
            _tc.bcWrapper.UserItemsService.AwardUserItem(bundleId, quanitity, includeDef, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            Dictionary<string, object>[] listOfItems = null;
            string itemId = "";
            string key = "";
            var data = _tc.m_response["data"] as Dictionary<string, object>;
            var items = data["items"] as Dictionary<string, object>;
            
            Dictionary<string, object> value = new Dictionary<string, object>();
            for (int index = 0; index < items.Count; index++)
            {
                var item = items.ElementAt(index);
                value[item.Key] = ((Dictionary<string, object>) item.Value)["itemId"];
                key = item.Key;
                break;
            }
            itemId = value[key] as string;
            _tc.bcWrapper.UserItemsService.OpenBundle(itemId, -1, quanitity, true, optionsJson, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            
            LogResults($"Failure to purchase item with options true", _tc.successCount == 2);
        }
    }
}
