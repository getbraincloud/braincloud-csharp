//----------------------------------------------------
// brainCloud client source code
// Copyright 2018 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using System;

namespace BrainCloud
{

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
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// Force an RTT presence update to all listeners of the caller.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - GetPresenceOfFriends
        /// </remarks>
        /// <param name="platform">
        /// The store platform. Valid stores are:
        /// - all
        /// - brainCloud
        /// - facebook
        /// </param>
        /// <param name="includeOffline">
        /// Will not include offline profiles unless includeOffline is set to true.
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
        ///	Gets the presence data for the given <groupId>. Will not include
	    /// offline profiles unless<includeOffline> is set to true.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - GetPresenceOfGroup
        /// </remarks>
        /// <param name="groupId">
        /// The id for the group
        /// </param>
        /// <param name="includeOffline">
        /// Will not include offline profiles unless includeOffline is set to true.
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
        ///Gets the presence data for the given<profileIds>.Will not include
	    /// offline profiles unless<includeOffline> is set to true.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - GetPresenceOfUsers
        /// </remarks>
        /// <param name="profileIds">
        /// List of profile Ids
        /// </param>
        /// <param name="includeOffline">
        /// Will not include offline profiles unless includeOffline is set to true.
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
	    /// given platform. Can be one of "all", "brainCloud", or "facebook".
	    /// If bidirectional is set to true, then also registers the targeted
	    /// users for presence updates from the caller.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - RegisterListenersForFriends
        /// </remarks>
        /// <param name="platform">
        /// The store platform. Valid stores are:
        /// - all
        /// - brainCloud
        /// - facebook
        /// </param>
        /// <param name="bidirectional">
        /// Allows registration of target user for presence update
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
	    /// Registers the caller for RTT presence updates from the members of the given groupId.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - RegisterListenersForGroup
        /// </remarks>
        /// <param name="groupId">
        /// The Id of the group
        /// </param>
        /// <param name="bidirectional">
        /// Allows registration of target user for presence update
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
	    /// profileIds. If bidirectional is set to true, then also registers
	    /// the targeted users for presence updates from the caller.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - RegisterListenersForGroup
        /// </remarks>
        /// <param name="profileIds">
        /// List of profile Ids
        /// </param>
        /// <param name="bidirectional">
        /// Allows registration of target user for presence update
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
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - SetVisibility
        /// </remarks>
        /// <param name="visible">
        /// Determines if the user is visible
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
		/// affect the broadcasting of *their* presence updates to other
		/// listeners.
        /// </summary>
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - StopListening
        /// </remarks>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// <remarks>
        /// Service Name - Presence
        /// Service Operation - UpdateActivity
        /// </remarks>
        /// <param name="jsonActivity">
        /// the Json data
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
        public void UpdateActivity(
            string jsonActivity,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PresenceServiceActivity.Value] = jsonActivity;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Presence, ServiceOperation.UpdateActivity, data, callback);
            _client.SendRequest(sc);
        }
    }
}
