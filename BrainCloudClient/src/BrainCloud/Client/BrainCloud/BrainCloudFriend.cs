//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using LitJson;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudFriend
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudFriend (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
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
            JsonData data = new JsonData();
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
        /// <param name="in_entitySubtype">
        /// Subtypes of entities to retrieve.
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
            string in_entitySubtype,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.FriendServiceEntityType.Value] = in_entityType;
            data[OperationParam.FriendServiceEntitySubtype.Value] = in_entitySubtype;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendsEntities, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns list of friends with optional summary data.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriendsWithApplication
        /// </remarks>
        /// <param name="in_includeSummaryData">
        /// Whether to include summary data
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
        public void ReadFriendsWithApplication(
            bool in_includeSummaryData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
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
            JsonData data = new JsonData();
            data[OperationParam.FriendServiceReadPlayerStateFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriendPlayerState, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Returns the "friend summary data" associated with the logged in player.
        /// Some operations will return this summary data. For instance the social
        /// leaderboards will return the player's score in the leaderboard along
        /// with the player's friend summary data. Generally this data is used to
        /// provide a quick overview of the player without requiring a separate API
        /// call to read their public stats or entity data.
        ///
        /// Note this API call pre-dates the shared player data api (public entity/stats).
        /// The shared player data api may be more suitable for sharing of data.
        /// </summary>
        /// <remarks>
        /// Service Name - Friend
        /// Service Operation - ReadFriends
        /// </remarks>
        /// <param name="in_jsonSummaryData">
        /// A JSON string defining the summary data.
        /// For example:
        /// {
        ///   "status":200
        ///   "data":{
        ///     "friendSummaryData": {
        ///       "xp":123,
        ///       "level":12,
        ///       "highScore":45123
        ///     }
        ///   }
        /// }
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
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        [Obsolete("Deprecated method")]
        public void ReadSummaryFriendData(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriends, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Updates the "friend summary data" associated with the logged in player.
        /// Some operations will return this summary data. For instance the social
        /// leaderboards will return the player's score in the leaderboard along
        /// with the friend summary data. Generally this data is used to provide
        /// a quick overview of the player without requiring a separate API call
        /// to read their public stats or entity data.
        ///
        /// Note this API call pre-dates the shared player data api (public entity/stats)
        /// and thus usage for anything outside of social leaderboards should be
        /// deprecated.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UpdateSummary
        /// </remarks>
        /// <param name="in_jsonSummaryData">
        /// A JSON string defining the summary data.
        /// For example:
        /// {
        ///   "xp":123,
        ///   "level":12,
        ///   "highScore":45123
        /// }
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        ///
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        [Obsolete("Deprecated method")]
        public void UpdateSummaryFriendData(
            string in_jsonSummaryData,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            if (Util.IsOptionalParameterValid(in_jsonSummaryData))
            {
                JsonData jsonData = JsonMapper.ToObject(in_jsonSummaryData);
                data[OperationParam.PlayerStateServiceUpdateFriendSummaryData.Value] = jsonData;
            }
            else data = null;
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateSummary, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Deprecated method")]
        public void ReadFriendPlayerState(
            long friendId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            JsonData data = new JsonData();
            data[OperationParam.FriendServiceReadPlayerStateFriendId.Value] = friendId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriends, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        [Obsolete("Deprecated method")]
        public void ReadFriendData(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.Friend, ServiceOperation.ReadFriends, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

    }
}

