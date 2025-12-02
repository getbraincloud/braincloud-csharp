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
            
            string optionsJson = "{\"blockIfExceedItemMaxStackable\": False}";
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
            
            string optionsJson = "{\"blockIfExceedItemMaxStackable\": True}";
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
            
            string shopId = "";
            bool includeDef = true;
            bool includePromotionDetails = true;
            _tc.bcWrapper.UserItemsService.GetItemsOnPromotion
            (
                shopId,
                includeDef,
                includePromotionDetails,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults($"Failure to award user items",_tc.successCount == 1);
        }
    }
}