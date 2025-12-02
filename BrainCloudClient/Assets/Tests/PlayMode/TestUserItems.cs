using System.Collections;
using System.Collections.Generic;
using BrainCloud;
using BrainCloud.Common;
using NUnit.Framework;
using UnityEngine;
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
    }
}