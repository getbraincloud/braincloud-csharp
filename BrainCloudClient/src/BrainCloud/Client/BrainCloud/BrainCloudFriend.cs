//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
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

        public BrainCloudFriend(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
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
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// {
        /// 	"status": 200,
        /// 	"data": {
        /// 		"matchedCount": 20,
        /// 		"matches": [{
        /// 			"profileId": "17c7ee96-1b73-43d0-8817-cba1953bbf57",
        /// 			"profileName": "Donald Trump",
        /// 			"playerSummaryData": {}
        /// }, {
        /// 			"profileId": "19d7ee96-2x73-43d0-8817-cba1953bbf57",
        /// 			"profileName": "Donald Duck",
        /// 			"playerSummaryData": {}
        /// 		}]
        /// 	}
        /// }
        /// 
        /// Alternatively, if there are too many results: 
        /// {
        /// 	"status": 200,
        /// 	"data": {
        /// 		"matchedCount": 2059,
        /// 		"message": "Too many results to return."
        /// 	}
        /// }
        /// </returns>
        public void FindPlayerByUniversalId(
            string searchText,
            int maxResults,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceSearchText.Value] = searchText;
            data[OperationParam.FriendServiceMaxResults.Value] = maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_externalId">
        /// External id of the friend to find
        /// </param>
        /// <param name="in_authenticationType">
        /// The authentication type used for this friend's external id e.g. Facebook
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// {
        ///   "status":200,
        ///   "data": {
        ///     "playerId" : "17c7ee96-1b73-43d0-8817-cba1953bbf57",
        ///     "playerName" : "Donald Trump",
        ///     "email" : "donald@trumpcastle.com",
        ///     "playerSummaryData" : {},
        ///   }
        /// }
        /// </returns>
        public void GetFriendProfileInfoForExternalId(
            string in_externalId,
            string in_authenticationType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceExternalId.Value] = in_externalId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = in_authenticationType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_profileId">
        /// Profile (player) ID.
        /// </param>
        /// <param name="in_authenticationType">
        /// Associated authentication type.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// {
        /// 	"status": 200,
        /// 	"data": {
        /// 		"externalId": "19e1c0cf-9a2d-4d5c-9a71-1b0f6b309b4b"
        /// 	}
        /// }
        /// </returns>
        public void GetExternalIdForProfileId(
            string in_profileId,
            string in_authenticationType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceProfileId.Value] = in_profileId;
            data[OperationParam.FriendServiceAuthenticationType.Value] = in_authenticationType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_entityId">
        /// Id of entity to retrieve.
        /// </param>
        /// <param name="in_friendId">
        /// Profile Id of friend who owns entity.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// </returns>
        public void ReadFriendEntity(
            string in_entityId,
            string in_friendId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceEntityId.Value] = in_entityId;
            data[OperationParam.FriendServiceFriendId.Value] = in_friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_entityType">
        /// Types of entities to retrieve.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// </returns>
        public void ReadFriendsEntities(
            string in_entityType,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceEntityType.Value] = in_entityType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendsEntities, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Use ListFriends method instead - removal after June 21 2016")]
        public void ReadFriendsWithApplication(
            bool in_includeSummaryData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceIncludeSummaryData.Value] = in_includeSummaryData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendsWithApplication, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns player state of a particular friend.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriendPlayerState
        /// </remarks>
        /// <param name="in_friendId">
        /// Profile Id of friend to retrieve player state for.
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> The JSON returned in the callback
        /// </returns>
        public void ReadFriendPlayerState(
            string friendId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.FriendServiceReadPlayerStateFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendPlayerState, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Finds a list of players matching the search text by performing a substring
        /// search of all player names.
        /// If the number of results exceeds maxResults the message
        /// "Too many results to return." is received and no players are returned
        /// </summary>
        /// 
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - FindPlayerByName
        /// </remarks>
        /// 
        /// <param name="in_searchText"> 
        /// The substring to search for. Minimum length of 3 characters.
        /// </param>
        /// <param name="in_maxResults"> 
        /// Maximum number of results to return. If there are more the message 
        /// "Too many results to return." is sent back instead of the players.
        /// </param>
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// 
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "matches": [
        ///             {
        ///                 "profileId": "63d1fdbd-2971-4791-a248-f8cda1a79bba",
        ///                 "playerSummaryData": null,
        ///                 "profileName": "ABC"
        ///             }
        ///         ],
        ///         "matchedCount": 1
        ///     }
        /// }
        /// </returns>
        public void FindPlayerByName(
            string in_searchText,
            int in_maxResults,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceSearchText.Value] = in_searchText;
            data[OperationParam.FriendServiceMaxResults.Value] = in_maxResults;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.FindPlayerByName, data, callback);
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
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// Example 1: for friendPlatform = All
        /// {
        /// 	"status": 200,
        /// 	"data": {
        /// 		"friends": [{
        /// 			"externalData": {
        /// 				"Facebook": {
        /// 					"pictureUrl": "https://scontent.xx.fbcdn.net/hprofile-xfp1/v/t1.0-1/p50x50/XXX.jpg?oh=YYY&oe=ZZZ",
        /// 					"name": "scientist at large",
        /// 					"externalId": "100003668521730"
        /// 
        ///                 },
        /// 				"brainCloud": {}
        /// 			},
        /// 			"playerId": "1aa3428c-5877-4624-a909-f2b1af931f00",
        /// 			"name": "Mr. Peabody",
        /// 			"summaryFriendData": {
        /// 				"LEVEL": -4
        /// 			}
        /// 		}, {
        /// 			"externalData": {
        /// 				"Facebook": {
        /// 					"pictureUrl": "https://scontent.xx.fbcdn.net/hprofile-xfa1/v/t1.0-1/c0.11.50.50/p50x50/3AAA.jpg?oh=BBBa&oe=CCC",
        /// 					"name": "Aquaman",
        /// 					"externalId": "100003509724516"
        /// 				}
        /// 			},
        /// 			"playerId": "1598c5b6-1b09-431b-96bc-9c2c928cad3b",
        /// 			"name": null,
        /// 			"summaryFriendData": {
        /// 				"LEVEL": 1
        /// 			}
        /// 		}],
        /// 		"server_time": 1458224807855
        /// 	}
        /// }
        /// 
        /// Example 2: for friendPlatform = Facebook
        /// {
        /// 	"status": 200,
        /// 	"data": {
        /// 		"friends": [{
        /// 			"externalData": {
        /// 				"Facebook": {
        /// 					"pictureUrl": "https://scontent.xx.fbcdn.net/hprofile-xfp1/v/t1.0-1/p50x50/XXX.jpg?oh=YYY&oe=ZZZ",
        /// 					"name": "scientist at large",
        /// 					"externalId": "100003668521730"
        /// 				}
        /// 			},
        /// 			"playerId": "1aa3428c-5877-4624-a909-f2b1af931f00",
        /// 			"name": "Mr. Peabody",
        /// 			"summaryFriendData": {
        /// 				"LEVEL": -4
        /// 			}
        /// 		}, {
        /// 			"externalData": {
        /// 				"Facebook": {
        /// 					"pictureUrl": "https://scontent.xx.fbcdn.net/hprofile-xfa1/v/t1.0-1/c0.11.50.50/p50x50/3AAA.jpg?oh=BBBa&oe=CCC",
        /// 					"name": "Aquaman",
        /// 					"externalId": "100003509724516"
        /// 				}
        /// 			},
        /// 			"playerId": "1598c5b6-1b09-431b-96bc-9c2c928cad3b",
        /// 			"name": null,
        /// 			"summaryFriendData": {
        /// 				"LEVEL": 1
        /// 			}
        /// 		}],
        /// 		"server_time": 1458224807855
        /// 	}
        /// }
        /// </returns>
        public void ListFriends(
            FriendPlatform friendPlatform,
            bool includeSummaryData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceFriendPlatform.Value] = friendPlatform.ToString();
            data[OperationParam.FriendServiceIncludeSummaryData.Value] = includeSummaryData;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        /// 	"status": 200,
        /// 	"data": null
        /// }
        /// </returns>
        public void AddFriends(
            IList<string> profileIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceProfileIds.Value] = profileIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
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
        /// <param name="in_success"> The success callback. </param>
        /// <param name="in_failure"> The failure callback. </param>
        /// <param name="in_cbObject"> The user object sent to the callback. </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        /// 	"status": 200,
        /// 	"data": null
        /// }
        /// </returns>
        public void RemoveFriends(
            IList<string> profileIds,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.FriendServiceProfileIds.Value] = profileIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.RemoveFriends, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

