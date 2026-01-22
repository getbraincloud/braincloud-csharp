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
        /// Service Name - Presence
        /// Service Operation - ForcePush
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
        /// Gets the presence data for the given <platform>. Can be one of "all",
        /// </summary>

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
        /// Gets the presence data for the given <groupId>. Will not include
        /// </summary>

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
        /// Gets the presence data for the given <profileIds>. Will not include
        /// </summary>

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
        /// Registers the caller for RTT presence updates from friends for the
        /// </summary>

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
        /// Registers the caller for RTT presence updates from the members of
        /// </summary>

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
        /// Registers the caller for RTT presence updates for the given
        /// </summary>

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
        /// Update the presence data visible field for the caller.
        /// </summary>

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
        /// Stops the caller from receiving RTT presence updates. Does not
        /// </summary>

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
        /// Update the presence data activity field for the caller.
        /// </summary>

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
