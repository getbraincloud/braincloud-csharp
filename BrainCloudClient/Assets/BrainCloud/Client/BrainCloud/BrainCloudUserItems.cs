//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System.Collections.Generic;
using BrainCloud.Internal;
using BrainCloud.JsonFx.Json;
using System;


    public class BrainCloudUserItems
    {
        private BrainCloudClient _client;

        public BrainCloudUserItems(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Allows item(s) to be awarded to a user without collecting
        ///the purchase amount. If includeDef is true, response 
        ///includes associated itemDef with language fields limited
        ///to the current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - AwardUserItem
        /// </remarks>
        /// <param name="defId">
        /// </param>
        /// <param name="quantity">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void AwardUserItem(
        string defId,
        int quantity,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceDefId.Value] = defId;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.AwardUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows a quantity of a specified user item to be dropped, 
        ///without any recovery of the money paid for the item. If any 
        ///quantity of the user item remains, it will be returned, potentially 
        ///with the associated itemDef (with language fields limited to the 
        ///current or default language).
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - DropUserItem
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="quantity">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void DropUserItem(
        string itemId,
        int quantity,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.DropUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the page of user's inventory from the server 
        ///based on the context. If includeDef is true, response includes
        /// associated itemDef with each user item, with language fields 
        ///limited to the current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserInventoryPage
        /// </remarks>
        /// <param name="context">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetUserItemsPage(
        string context,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            var contextData = JsonReader.Deserialize<Dictionary<string, object>>(context);
            data[OperationParam.UserItemsServiceContext.Value] = contextData;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GetUserItemsPage, data, callback);
            _client.SendRequest(sc);
        }

        
        /// <summary>
        /// Retrieves the page of user's inventory 
        ///from the server based on the encoded context. 
        ///If includeDef is true, response includes associated 
        ///itemDef with each user item, with language fields limited 
        ///to the current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserInventoryPageOffset
        /// </remarks>
        /// <param name="context">
        /// </param>
        /// <param name="pageOffset">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetUserItemsPageOffset(
        string context,
        int pageOffset,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceContext.Value] = context;
            data[OperationParam.UserItemsServicePageOffset.Value] = pageOffset;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GetUserItemsPageOffset, data, callback);
            _client.SendRequest(sc);
        }

        
        
        /// <summary>
        /// Retrieves the identified user item from the server.
        /// If includeDef is true, response includes associated
        /// itemDef with language fields limited to the current 
        ///or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserItem
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GetUserItem(
        String itemId,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GetUserItem, data, callback);
            _client.SendRequest(sc);
        }

        
        
        
        /// <summary>
        /// Gifts item to the specified player.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserItem
        /// </remarks>
        /// <param name="profileId">
        /// </param>
        /// <param name="itemId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="quantity">
        /// </param>
        /// <param name="immediate">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void GiveUserItemTo(
        String profileId,
        String itemId,
        int version,
        int quantity,
        bool immediate,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceProfileId.Value] = profileId;
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceImmediate.Value] = immediate;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GiveUserItemTo, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Purchases a quantity of an item from the specified store, 
        ///if the user has enough funds. If includeDef is true, 
        ///response includes associated itemDef with language fields
        /// limited to the current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserItem
        /// </remarks>
        /// <param name="defId">
        /// </param>
        /// <param name="quatity">
        /// </param>
        /// <param name="shopId">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void PurchaseUserItem(
        String defId,
        int quantity,
        string shopId,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceDefId.Value] = defId;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceShopId.Value] = shopId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.PurchaseUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves and transfers the gift item from the specified player, 
        //who must have previously called giveUserItemTo.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserItem
        /// </remarks>
        /// <param name="defId">
        /// </param>
        /// <param name="quatity">
        /// </param>
        /// <param name="shopId">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void ReceiveUserItemFrom(
        string profileId,
        string itemId,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceProfileId.Value] = profileId;
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.ReceiveUserItemFrom, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows a quantity of a specified user item to be sold. 
        ///If any quantity of the user item remains, 
        ///it will be returned, potentially with the associated 
        ///itemDef (with language fields limited to the current 
        ///or default language), along with the currency refunded 
        ///and currency balances.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - SellUserItem
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="quantity">
        /// </param>
        /// <param name="shopId">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void SellUserItem(
        string itemId,
        int version, 
        int quantity,
        string shopId, 
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceShopId.Value] = shopId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.SellUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates the item data on the specified user item.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - UpdateUserItemData
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="newItemData">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void UpdateUserItemData(
        string itemId,
        int version, 
        string newItemData,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;
            var newItemDataDict = JsonReader.Deserialize<Dictionary<string, object>>(newItemData);
            data[OperationParam.UserItemsServiceNewItemData.Value] = newItemDataDict;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.UpdateUserItemData, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Uses the specified item, potentially consuming it.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - UseUserItem
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="newItemData">
        /// </param>
        /// <param name="includeDef">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void UseUserItem(
        string itemId,
        int version, 
        string newItemData,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            var newItemDataDict = JsonReader.Deserialize<Dictionary<string, object>>(newItemData);
            data[OperationParam.UserItemsServiceNewItemData.Value] = newItemDataDict;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.UseUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Publishes the specified item to the item management attached blockchain. Results are reported asynchronously via an RTT event.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - PublishUserItemToBlockchain
        /// </remarks>
        /// <param name="itemId">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void PublishUserItemToBlockchain(
        string itemId,
        int version, 
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.PublishUserItemToBlockchain, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Syncs the caller's user items with the item management attached blockchain. Results are reported asynchronously via an RTT event.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - RefreshBlockchainUserItems
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void RefreshBlockchainUserItems(
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.RefreshBlockchainUserItems, data, callback);
            _client.SendRequest(sc);
        }

        
        /// <summary>
        /// removes item from a blockchain.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - RemoveUserItemFromBlockchain
        /// 
        /// </param>
        /// <param name="itemId">
        /// 
        /// </param>
        /// <param name="version">
        /// 
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
        public void RemoveUserItemFromBlockchain(
        string itemId,
        int version,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.RemoveUserItemFromBlockchain, data, callback);
            _client.SendRequest(sc);
        }
    }
}
