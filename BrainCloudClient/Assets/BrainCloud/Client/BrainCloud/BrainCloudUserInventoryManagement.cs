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


    public class BrainCloudUserInventoryManagement
    {
        private BrainCloudClient _client;

        public BrainCloudUserInventoryManagement(BrainCloudClient client)
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
            data[OperationParam.UserInventoryManagementServiceDefId.Value] = defId;
            data[OperationParam.UserInventoryManagementServiceQuantity.Value] = quantity;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.AwardUserItem, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceQuantity.Value] = quantity;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.DropUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the user's inventory from the server (or inventory specified by criteria). 
        //If includeDef is true, response includes associated itemDef with each user item, with
        // language fields limited to the current or default language.
        /// </summary>
        /// <remarks>
        /// Service Name - UserInventoryManagement
        /// Service Operation - GetUserInventory
        /// </remarks>
        /// <param name="criteria">
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
        public void GetUserInventory(
        string criteria,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            var criteriaData = JsonReader.Deserialize<Dictionary<string, object>>(criteria);
            data[OperationParam.UserInventoryManagementServiceCriteria.Value] = criteriaData;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.GetUserInventory, data, callback);
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
        public void GetUserInventoryPage(
        string context,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            var contextData = JsonReader.Deserialize<Dictionary<string, object>>(context);
            data[OperationParam.UserInventoryManagementServiceContext.Value] = contextData;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.GetUserInventoryPage, data, callback);
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
        public void GetUserInventoryPageOffset(
        string context,
        int pageOffset,
        bool includeDef,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserInventoryManagementServiceContext.Value] = context;
            data[OperationParam.UserInventoryManagementServicePageOffset.Value] = pageOffset;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.GetUserInventoryPageOffset, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.GetUserItem, data, callback);
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
        bool immediate,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserInventoryManagementServiceProfileId.Value] = profileId;
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceVersion.Value] = version;
            data[OperationParam.UserInventoryManagementServiceImmediate.Value] = immediate;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.GiveUserItemTo, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceDefId.Value] = defId;
            data[OperationParam.UserInventoryManagementServiceQuantity.Value] = quantity;
            data[OperationParam.UserInventoryManagementServiceShopId.Value] = shopId;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.PurchaseUserItem, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceProfileId.Value] = profileId;
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.ReceiveUserItemFrom, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceVersion.Value] = version;
            data[OperationParam.UserInventoryManagementServiceQuantity.Value] = quantity;
            data[OperationParam.UserInventoryManagementServiceShopId.Value] = shopId;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.SellUserItem, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceVersion.Value] = version;
            var newItemDataDict = JsonReader.Deserialize<Dictionary<string, object>>(newItemData);
            data[OperationParam.UserInventoryManagementServiceNewItemData.Value] = newItemDataDict;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.UpdateUserItemData, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceVersion.Value] = version;
            data[OperationParam.UserInventoryManagementServiceIncludeDef.Value] = includeDef;

            var newItemDataDict = JsonReader.Deserialize<Dictionary<string, object>>(newItemData);
            data[OperationParam.UserInventoryManagementServiceNewItemData.Value] = newItemDataDict;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.UseUserItem, data, callback);
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
            data[OperationParam.UserInventoryManagementServiceItemId.Value] = itemId;
            data[OperationParam.UserInventoryManagementServiceVersion.Value] = version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.PublishUserItemToBlockchain, data, callback);
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
            ServerCall sc = new ServerCall(ServiceName.UserInventoryManagement, ServiceOperation.RefreshBlockchainUserItems, data, callback);
            _client.SendRequest(sc);
        }
    }
}
