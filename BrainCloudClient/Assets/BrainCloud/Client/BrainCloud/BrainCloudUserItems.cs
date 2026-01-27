// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

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
        /// Awards item(s) to a user without collecting the purchase amount.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - AWARD_USER_ITEM
        /// </remarks>
        /// <param name="defId">The unique id of the item definition to award.</param>
        /// <param name="quantity">The quantity of the item to award.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Drops a quantity of a specified user item without recovering the purchase cost.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - DROP_USER_ITEM
        /// </remarks>
        /// <param name="defId">The unique id of the item definition to drop.</param>
        /// <param name="quantity">The quantity of the item to drop.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Retrieves a page of the user's inventory.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GET_USER_INVENTORY_PAGE
        /// </remarks>
        /// <param name="context">Context string used to filter inventory.</param>
        /// <param name="includeDef">If true, include associated item definitions in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Retrieves a page of the user's inventory with an offset.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GET_USER_INVENTORY_PAGE_OFFSET
        /// </remarks>
        /// <param name="context">Context string used to filter inventory.</param>
        /// <param name="pageOffset">Page offset to retrieve.</param>
        /// <param name="includeDef">If true, include associated item definitions in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Retrieves a specific user item.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GET_USER_ITEM
        /// </remarks>
        /// <param name="itemId">ID of the user item to retrieve.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Gifts an item to another user.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GIVE_USER_ITEM_TO
        /// </remarks>
        /// <param name="profileId">Profile ID of the recipient.</param>
        /// <param name="itemId">ID of the item to gift.</param>
        /// <param name="version">Version of the item being gifted.</param>
        /// <param name="quantity">Quantity of the item to gift.</param>
        /// <param name="immediate">If true, the gift is delivered immediately.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Purchases a user item from a store.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - PURCHASE_USER_ITEM
        /// </remarks>
        /// <param name="defId">The unique id of the item definition to purchase.</param>
        /// <param name="quantity">Quantity of the item to purchase.</param>
        /// <param name="shopId">Store ID for the purchase.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Retrieves and transfers a gift item from another user.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - RECEIVE_USER_ITEM_FROM
        /// </remarks>
        /// <param name="profileId">Profile ID of the sender.</param>
        /// <param name="itemId">ID of the item being received.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Allows a quantity of a specified bundle user item to be opened. Response
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - OPEN_BUNDLE
        /// </remarks>
        /// <param name="itemId">ID of the bundle item to open.</param>
        /// <param name="version">Version of the bundle item (pass -1 for any version).</param>
        /// <param name="quantity">Quantity of the item to open.</param>
        /// <param name="includeDef">Include associated item definitions if true.</param>
        /// <param name="optionsJson">JSON string specifying additional options.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void OpenBundle(
        string itemId,
        int version,
        int quantity,
        bool includeDef,
        Dictionary<string, object> optionsJson = null,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceItemId.Value] = itemId;
            data[OperationParam.UserItemsServiceVersion.Value] = version;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            if (optionsJson != null && optionsJson.Count > 0)
            {
                data[OperationParam.UserItemsServiceOptionsJson.Value] = optionsJson;
            }
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.OpenBundle, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Allows a quantity of a specified user item to be sold.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - SELL_USER_ITEM
        /// </remarks>
        /// <param name="itemId">ID of the user item to sell.</param>
        /// <param name="version">Version of the item being sold.</param>
        /// <param name="quantity">Quantity of the item to sell.</param>
        /// <param name="shopId">Store ID for the sale.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Updates the data of a specific user item.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - UPDATE_USER_ITEM_DATA
        /// </remarks>
        /// <param name="itemId">ID of the user item to update.</param>
        /// <param name="version">Version of the item being updated.</param>
        /// <param name="newItemData">JSON string with updated item data.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Uses a user item, potentially consuming it.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - USE_USER_ITEM
        /// </remarks>
        /// <param name="itemId">ID of the user item to use.</param>
        /// <param name="version">Version of the user item (pass -1 for any version).</param>
        /// <param name="newItemData">Optional JSON string to update item fields.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Publishes a user item to the blockchain.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - PUBLISH_USER_ITEM_TO_BLOCKCHAIN
        /// </remarks>
        /// <param name="itemId">ID of the user item to publish.</param>
        /// <param name="version">Version of the item to publish.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Refreshes blockchain user items.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - REFRESH_BLOCKCHAUSER_ITEMS
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
        /// Removes a user item from the blockchain.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - REMOVE_USER_ITEM_FROM_BLOCKCHAIN
        /// </remarks>
        /// <param name="itemId">ID of the user item to remove.</param>
        /// <param name="version">Version of the user item to remove.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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

        /// <summary>
        /// Awards item(s) to a user with additional options.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - AWARD_USER_ITEM
        /// </remarks>
        /// <param name="defId">The unique id of the item definition to award.</param>
        /// <param name="quantity">The quantity of the item to award.</param>
        /// <param name="includeDef">If true, include associated item definition in the response.</param>
        /// <param name="optionsJson">JSON string specifying additional options (e.g., blockIfExceedItemMaxStackable).</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void AwardUserItemWithOptions(
        string defId,
        int quantity,
        bool includeDef,
        Dictionary<string, object> optionsJson = null,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceDefId.Value] = defId;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;
            if (optionsJson != null && optionsJson.Count > 0)
            {
                data[OperationParam.UserItemsServiceOptionsJson.Value] = optionsJson;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.AwardUserItem, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns a list of items on promotion available to the current user.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GET_ITEMS_ON_PROMOTION
        /// </remarks>
        /// <param name="shopId">Store ID.</param>
        /// <param name="includeDef">Include associated item definition if true.</param>
        /// <param name="includePromotionDetails">Include promotion details if true.</param>
        /// <param name="optionsJson">JSON string specifying additional options (e.g., category).</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetItemsOnPromotion(
        string shopId,
        bool includeDef,
        bool includePromotionDetails,
        Dictionary<string, object> optionsJson = null,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.UserItemsServiceShopId.Value] = shopId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;
            data[OperationParam.UserItemsServiceIncludePromotionDetails.Value] = includePromotionDetails;
            if (optionsJson != null && optionsJson.Count > 0)
            {
                data[OperationParam.UserItemsServiceOptionsJson.Value] = optionsJson;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GetPromotionDetails, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns a list of promotional details for a specified item.
        /// </summary>
        /// <remarks>
        /// Service Name - userItems
        /// Service Operation - GET_ITEM_PROMOTION_DETAILS
        /// </remarks>
        /// <param name="defId">Item definition ID.</param>
        /// <param name="shopId">Store ID.</param>
        /// <param name="includeDef">Include associated item definition if true.</param>
        /// <param name="includePromotionDetails">Include promotion details if true.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetItemPromotionDetails(
        string defId,
        string shopId,
        bool includeDef,
        bool includePromotionDetails,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceDefId.Value] = defId;
            data[OperationParam.UserItemsServiceShopId.Value] = shopId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;
            data[OperationParam.UserItemsServiceIncludePromotionDetails.Value] = includePromotionDetails;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.GetItemPromotionDetails, data, callback);
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
        /// The unique id of the item definition to purchase.
        /// </param>
        /// <param name="quantity">
        /// The quantity of the item to purchase.
        /// </param>
        /// <param name="shopId">
        /// The id identifying the store the item is being purchased from, if applicable.
        /// </param>
        /// <param name="includeDef">
        /// If true, the associated item definition will be included in the response.
        /// </param>
        /// <param name="optionsJson">
        /// Optional support for specifying 'blockIfExceedItemMaxStackable' indicating 
        /// how to process the award if the defId is for a stackable item with a max 
        /// stackable quantity and the specified quantity to award is too high. If 
        /// true and the quantity is too high, the call is blocked and an error is returned.
        /// If false (default) and quantity is too high, the quantity is adjusted 
        /// to the allowed maximum and the quantity not awarded is reported in 
        /// response key 'itemsNotAwarded' - unless the adjusted quantity would be 
        /// 0, in which case the call is blocked and an error is returned.
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
        public void PurchaseUserItemWithOptions(
        String defId,
        int quantity,
        string shopId,
        bool includeDef,
        Dictionary<string, object> optionsJson = null,
        SuccessCallback success = null,
        FailureCallback failure = null,
        object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserItemsServiceDefId.Value] = defId;
            data[OperationParam.UserItemsServiceQuantity.Value] = quantity;
            data[OperationParam.UserItemsServiceShopId.Value] = shopId;
            data[OperationParam.UserItemsServiceIncludeDef.Value] = includeDef;
            if (optionsJson != null && optionsJson.Count > 0)
            {
                data[OperationParam.UserItemsServiceOptionsJson.Value] = optionsJson;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.UserItems, ServiceOperation.PurchaseUserItem, data, callback);
            _client.SendRequest(sc);
        }
    }
}
