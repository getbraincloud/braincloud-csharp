//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudFriend
    {
        public enum FriendPlatform
        {
            All,
            brainCloud,
            Facebook
        }

        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudFriend(BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
        }

        /// <summary>
        /// Retrieves profile information for the partial matches of the specified text.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - FIND_PLAYER_BY_UNIVERSAL_ID
        /// </remarks>
        /// <param name="searchText">
        /// Universal ID text on which to search.
        /// </param>
        /// <param name="maxResults">
        /// Maximum number of results to return.
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
        public void FindPlayerByUniversalId(
            string searchText,
            int maxResults,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceSearchText.Value] = searchText;
            data[OperationParam.FriendServiceMaxResults.Value] = maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindPlayerByUniversalId, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the specified user.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - GetFriendProfileInfoForExternalId
        /// </remarks>
        /// <param name="externalId">
        /// External id of the friend to find
        /// </param>
        /// <param name="authenticationType">
        /// The authentication type used for this friend's external id e.g. Facebook
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
        public void GetFriendProfileInfoForExternalId(
            string externalId,
            string authenticationType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = externalId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = authenticationType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetFriendProfileInfoForExternalId, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the external ID for the specified user profile ID on the specified social platform.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - GET_EXTERNAL_ID_FOR_PROFILE_ID
        /// </remarks>
        /// <param name="profileId">
        /// Profile (player) ID.
        /// </param>
        /// <param name="authenticationType">
        /// Associated authentication type.
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
        public void GetExternalIdForProfileId(
            string profileId,
            string authenticationType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceProfileId.Value] = profileId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = authenticationType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetExternalIdForProfileId, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns a particular entity of a particular friend.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriendEntity
        /// </remarks>
        /// <param name="entityId">
        /// Id of entity to retrieve.
        /// </param>
        /// <param name="friendId">
        /// Profile Id of friend who owns entity.
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
        public void ReadFriendEntity(
            string entityId,
            string friendId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceEntityId.Value] = entityId;
            data[OperationParam.FriendServiceFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendEntity, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns entities of all friends based on type and/or subtype.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriendsEntities
        /// </remarks>
        /// <param name="entityType">
        /// Types of entities to retrieve.
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
        public void ReadFriendsEntities(
            string entityType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceEntityType.Value] = entityType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendsEntities, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns player state of a particular friend.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriendPlayerState
        /// </remarks>
        /// <param name="friendId">
        /// Profile Id of friend to retrieve player state for.
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
        public void ReadFriendPlayerState(
            string friendId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceReadPlayerStateFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendPlayerState, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns player state of a particular user.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - GET_SUMMARY_DATA_FOR_PROFILE_ID
        /// </remarks>
        /// <param name="profileId">
        /// Profile Id of player to retrieve player state for.
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
        public void GetSummaryDataForProfileId(
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceProfileId.Value] = profileId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetSummaryDataForProfileId, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Finds a list of players matching the search text by performing an exact
        /// search of all player names.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - FIND_USERS_BY_EXACT_NAME
        /// </remarks>
        /// <param name="searchText"> 
        /// The string to search for.
        /// </param>
        /// <param name="maxResults"> 
        /// Maximum number of results to return.
        /// </param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void FindUsersByExactName(
            string searchText,
            int maxResults,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceSearchText.Value] = searchText;
            data[OperationParam.FriendServiceMaxResults.Value] = maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindUsersByExactName, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Finds a list of players matching the search text by performing a substring
        /// search of all player names.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - FIND_USERS_BY_EXACT_NAME
        /// </remarks>
        /// <param name="searchText"> 
        /// The substring to search for. Minimum length of 3 characters.
        /// </param>
        /// <param name="maxResults"> 
        /// Maximum number of results to return.
        /// </param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>
        public void FindUsersBySubstrName(
            string searchText,
            int maxResults,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceSearchText.Value] = searchText;
            data[OperationParam.FriendServiceMaxResults.Value] = maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindUsersBySubstrName, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves a list of player and friend platform information for all friends of the current player.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - LIST_FRIENDS
        /// </remarks>
        /// <param name="friendPlatform">Friend platform to query.</param>
        /// <param name="includeSummaryData">True if including summary data; false otherwise.</param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void ListFriends(
            FriendPlatform friendPlatform,
            bool includeSummaryData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceFriendPlatform.Value] = friendPlatform.ToString();
            data[OperationParam.FriendServiceIncludeSummaryData.Value] = includeSummaryData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ListFriends, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Links the current player and the specified players as brainCloud friends.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ADD_FRIENDS
        /// </remarks>
        /// <param name="profileIds">Collection of player IDs.</param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void AddFriends(
            IList<string> profileIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceProfileIds.Value] = profileIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.AddFriends, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Unlinks the current player and the specified players as brainCloud friends.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - REMOVE_FRIENDS
        /// </remarks>
        /// <param name="profileIds">Collection of player IDs.</param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void RemoveFriends(
            IList<string> profileIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceProfileIds.Value] = profileIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.RemoveFriends, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Get users online status
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - GET_USERS_ONLINE_STATUS
        /// </remarks>
        /// <param name="profileIds">Collection of profile IDs.</param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void GetUsersOnlineStatus(
            IList<string> profileIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceProfileIds.Value] = profileIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetUsersOnlineStatus, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

