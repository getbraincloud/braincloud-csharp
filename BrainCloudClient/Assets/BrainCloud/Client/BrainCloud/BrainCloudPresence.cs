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
    public class BrainCloudPresence
    {
        private BrainCloudClient _client;

        public BrainCloudPresence(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Force an RTT presence update to all listeners of the caller.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - FORCE_PUSH
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void ForcePush(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.ForcePush, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the presence data for friends on the specified platform.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - GET_PRESENCE_OF_FRIENDS
        /// </remarks>
        /// <param name="platform">One of "all", "brainCloud", or "facebook".</param>
        /// <param name="includeOffline">If true, includes offline profiles.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetPresenceOfFriends(
            string platform,
            bool includeOffline,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServicePlatform.Value] = platform;
            data[OperationParam.PresenceServiceIncludeOffline.Value] = includeOffline;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.GetPresenceOfFriends, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the presence data for members of a given group.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - GET_PRESENCE_OF_GROUP
        /// </remarks>
        /// <param name="groupId">Group ID to query.</param>
        /// <param name="includeOffline">If true, includes offline profiles.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetPresenceOfGroup(
            string groupId,
            bool includeOffline,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceGroupId.Value] = groupId;
            data[OperationParam.PresenceServiceIncludeOffline.Value] = includeOffline;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.GetPresenceOfGroup, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves the presence data for the specified users.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - GET_PRESENCE_OF_USERS
        /// </remarks>
        /// <param name="profileIds">Vector of profile IDs to query.</param>
        /// <param name="includeOffline">If true, includes offline profiles.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void GetPresenceOfUsers(
            List<string> profileIds,
            bool includeOffline,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceProfileIds.Value] = profileIds;
            data[OperationParam.PresenceServiceIncludeOffline.Value] = includeOffline;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.GetPresenceOfUsers, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Registers the caller for RTT presence updates from friends on a given platform.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - REGISTER_LISTENERS_FOR_FRIENDS
        /// </remarks>
        /// <param name="platform">One of "all", "brainCloud", or "facebook".</param>
        /// <param name="bidirectional">If true, also registers targeted users for updates from the caller.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void RegisterListenersForFriends(
            string platform,
            bool bidirectional,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServicePlatform.Value] = platform;
            data[OperationParam.PresenceServiceBidirectional.Value] = bidirectional;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.RegisterListenersForFriends, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Registers the caller for RTT presence updates from members of a given group.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - REGISTER_LISTENERS_FOR_GROUP
        /// </remarks>
        /// <param name="groupId">Group ID to listen to. Caller must be a member.</param>
        /// <param name="bidirectional">If true, also registers targeted users for updates from the caller.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void RegisterListenersForGroup(
            string groupId,
            bool bidirectional,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceGroupId.Value] = groupId;
            data[OperationParam.PresenceServiceBidirectional.Value] = bidirectional;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.RegisterListenersForGroup, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Registers the caller for RTT presence updates from specific profiles.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - REGISTER_LISTENERS_FOR_PROFILES
        /// </remarks>
        /// <param name="profileIds">Vector of profile IDs to listen to.</param>
        /// <param name="bidirectional">If true, also registers targeted users for updates from the caller.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void RegisterListenersForProfiles(
            List<string> profileIds,
            bool bidirectional,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceProfileIds.Value] = profileIds;
            data[OperationParam.PresenceServiceBidirectional.Value] = bidirectional;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.RegisterListenersForProfiles, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates the visibility field of the caller's presence data.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - SET_VISIBILITY
        /// </remarks>
        /// <param name="visible">True to make the caller visible, false to hide.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void SetVisibility(
            bool visible,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceVisibile.Value] = visible;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.SetVisibility, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Stops the caller from receiving RTT presence updates.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - STOP_LISTENING
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void StopListening(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.StopListening, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates the activity field of the caller's presence data.
        /// </summary>
        /// <remarks>
        /// Service Name - presence
        /// Service Operation - UPDATE_ACTIVITY
        /// </remarks>
        /// <param name="jsonActivity">JSON string representing activity information.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>


        public void UpdateActivity(
            string jsonActivity,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            var jsonActivityString = JsonReader.Deserialize<Dictionary<string, object>>(jsonActivity);
            data[OperationParam.PresenceServiceActivity.Value] = jsonActivityString;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.UpdateActivity, data, callback);
            _client.SendRequest(sc);
        }
    }
}
