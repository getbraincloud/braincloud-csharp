// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.Common;
    using System;

    public class BrainCloudFriend
    {
        public enum FriendPlatform
        {
            All,
            brainCloud,
            Facebook,
            PlaystationNetwork,
            Steam
        }

        private BrainCloudClient _client;

        public BrainCloudFriend(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Retrieves profile information for the specified user.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_PROFILE_INFO_FOR_CREDENTIAL
        /// </remarks>
        /// <param name="externalId">The users's external ID</param>
        /// <param name="authenticationType">The authentication type of the user ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetProfileInfoForCredential(
            string externalId,
            AuthenticationType authenticationType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = externalId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = authenticationType.ToString();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetProfileInfoForCredential, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the specified user.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_PROFILE_INFO_FOR_CREDENTIAL_IF_EXISTS
        /// </remarks>
        /// <param name="externalId">The users's external ID</param>
        /// <param name="authenticationType">The authentication type of the user ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetProfileInfoForCredentialIfExists(
            string externalId,
            AuthenticationType authenticationType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = externalId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = authenticationType.ToString();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetProfileInfoForCredentialIfExists, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the specified external auth user.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID
        /// </remarks>
        /// <param name="externalId">External ID of the friend to find</param>
        /// <param name="externalAuthType">The external authentication type used for this friend's external ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetProfileInfoForExternalAuthId(
            string externalId,
            string externalAuthType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = externalId;
            data[OperationParam.ExternalAuthType.Value] = externalAuthType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetProfileInfoForExternalAuthId, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the specified user. Silently fails, if profile does not exist, just returns null and success, instead of an error.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID_IF_EXISTS
        /// </remarks>
        /// <param name="externalId">External ID of the friend to find</param>
        /// <param name="externalAuthType">The external authentication type used for this friend's external ID</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetProfileInfoForExternalAuthIdIfExists(
            string externalId,
            string externalAuthType,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = externalId;
            data[OperationParam.ExternalAuthType.Value] = externalAuthType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.GetProfileInfoForExternalAuthIdIfExists, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the external ID for the specified user profile ID on the specified social platform.
        /// </summary>
        /// <param name="profileId">Profile (user) ID.</param>
        /// <param name="authenticationType">Associated authentication type.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns a particular entity of a particular friend.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - READ_FRIEND_ENTITY
        /// </remarks>
        /// <param name="entityId">Id of entity to retrieve.</param>
        /// <param name="friendId">Profile Id of friend who owns entity.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns entities of all friends optionally based on type.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - READ_FRIENDS_ENTITIES
        /// </remarks>
        /// <param name="entityType">Types of entities to retrieve.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Read a friend's user state.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - READ_FRIEND_PLAYER_STATE
        /// </remarks>
        /// <param name="friendId">Target friend</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void ReadFriendUserState(
            string friendId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceReadPlayerStateFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendPlayerState, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Returns user state of a particular user.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_SUMMARY_DATA_FOR_PROFILE_ID
        /// </remarks>
        /// <param name="profileId">Profile Id of user to retrieve user state for.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finds a list of users matching the search text by performing an exact match search
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - FIND_USERS_BY_EXACT_NAME
        /// </remarks>
        /// <param name="searchText">The string to search for.</param>
        /// <param name="maxResults">Maximum number of results to return.</param>
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information of the specified universal Id.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - FIND_USER_BY_EXACT_UNIVERSAL_ID
        /// </remarks>
        /// <param name="searchText">Universal ID text on which to search.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void FindUserByExactUniversalId(
            string searchText,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceSearchText.Value] = searchText;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindUserByExactUniversalId, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Finds a list of users matching the search text by performing a substring
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - FIND_USERS_BY_SUBSTR_NAME
        /// </remarks>
        /// <param name="searchText">The substring to search for. Minimum length of 3 characters.</param>
        /// <param name="maxResults">Maximum number of results to return. If there are more the message</param>
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves a list of user and friend platform information for all friends of the current user.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - LIST_FRIENDS
        /// </remarks>
        /// <param name="friendPlatform">Friend platform to query.</param>
        /// <param name="includeSummaryData">True if including summary data; false otherwise.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the social information associated with the logged in user. Includes summary data if includeSummaryData is true.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - GET_MY_SOCIAL_INFO
        /// </remarks>
        /// <param name="friendPlatform">Friend platform to query.</param>
        /// <param name="includeSummaryData">True if including summary data; false otherwise.</param>
        /// <param name="success"> The success callback. </param>
        /// <param name="failure"> The failure callback. </param>
        /// <param name="cbObject"> The user object sent to the callback. </param>
        public void GetMySocialInfo(
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Links the current user and the specified users as brainCloud friends.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - ADD_FRIENDS
        /// </remarks>
        /// <param name="profileIds">Collection of profile IDs.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Links the profiles for the specified externalIds for the given friend platform as internal friends.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - ADD_FRIENDS_FROM_PLATFORM
        /// </remarks>
        /// <param name="friendPlatform">Platform to add from (i.e: FriendPlatform::Facebook)</param>
        /// <param name="mode">ADD or SYNC</param>
        /// <param name="externalIds">Collection of external IDs from the friend platform.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void AddFriendsFromPlatform(
            FriendPlatform friendPlatform,
            string mode,
            IList<string> externalIds,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceFriendPlatform.Value] = friendPlatform.ToString();
            data[OperationParam.FriendServiceMode.Value] = mode;
            data[OperationParam.FriendServiceExternalIds.Value] = externalIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.Friend, ServiceOperation.AddFriendsFromPlatform, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Unlinks the current user and the specified users as brainCloud friends.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - REMOVE_FRIENDS
        /// </remarks>
        /// <param name="profileIds">Collection of profile IDs.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Get users online status
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - GET_USERS_ONLINE_STATUS
        /// </remarks>
        /// <param name="profileIds">Collection of profile IDs.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the users whos names start with search text.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - FIND_USERS_BY_NAME_STARTING_WITH
        /// </remarks>
        /// <param name="searchText">Name text on which to search.</param>
        /// <param name="maxResults">Maximum number of results to return.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void FindUsersByNameStartingWith(
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
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindUsersByNameStartingWith, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves profile information for the users whos UniversalId start with search text.
        /// </summary>
        /// <remarks>
        /// Service Name - friend
        /// Service Operation - FIND_USERS_BY_UNIVERSAL_ID_STARTING_WITH
        /// </remarks>
        /// <param name="searchText">Universal ID text on which to search.</param>
        /// <param name="maxResults">Maximum number of results to return.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void FindUsersByUniversalIdStartingWith(
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
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindUsersByUniversalIdStartingWith, data, callback);
            _client.SendRequest(sc);
        }
    }
}

