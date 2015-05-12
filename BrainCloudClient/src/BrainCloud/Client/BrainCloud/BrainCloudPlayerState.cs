//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudPlayerState
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPlayerState (BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Read the state of the currently logged in player.
        /// This method returns a JSON object describing most of the
        /// player's data: entities, statistics, level, currency.
        /// Apps will typically call this method after authenticating to get an
        /// up-to-date view of the player's data.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - Read
        /// </remarks>
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
        ///   "status": 200,
        ///   "data": {
        ///     "vcPurchased": null,
        ///     "id": "210ee817-d555-40c3-b109-c24a84c84dc7",
        ///     "experiencePoints": 10,
        ///     "sent_events": [],
        ///     "vcClaimed": null,
        ///     "server_time": 1395950294285,
        ///     "experienceLevel": 1,
        ///     "incoming_events": [],
        ///     "currency": {
        ///       "gems": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 0,
        ///         "awarded": 0
        ///       },
        ///       "gold": {
        ///         "purchased": 0,
        ///         "balance": 0,
        ///         "consumed": 0,
        ///         "awarded": 0
        ///       }
        ///     },
        ///     "statistics": {
        ///       "minions": 0,
        ///       "wood": 50,
        ///       "pantelons": 3,
        ///       "iron": 0
        ///     },
        ///     "abTestingId": 60
        ///   }
        /// }
        /// </returns>
        public void ReadPlayerState(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Read, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Completely deletes the player record and all data fully owned
        /// by the player. After calling this method, the player will need
        /// to re-authenticate and create a new profile.
        /// This is mostly used for debugging/qa.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - FullReset
        /// </remarks>
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
        public void DeletePlayer(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.FullReset, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// This method will delete *most* data for the currently logged in player.
        /// Data which is not deleted includes: currency, credentials, and
        /// purchase transactions. ResetPlayer is different from DeletePlayer in that
        /// the player record will continue to exist after the reset (so the user
        /// does not need to re-authenticate).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - DataReset
        /// </remarks>
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
        public void ResetPlayer(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.DataReset, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Logs player out of server.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - Logout
        /// </remarks>
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
        public void Logout(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Logout, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sets the players name.
        /// </summary>
        /// <remarks>
        /// Service Name - playerState
        /// Service Operation - UPDATE_NAME
        /// </remarks>
        /// <param name="in_playerName">
        /// The name of the player
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
        ///   "data":
        ///   {
        ///     "playerName": "someName"
        ///   }
        /// }
        /// </returns>
        public void UpdatePlayerName(
            string in_playerName,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceUpdateNameData.Value] = in_playerName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateName, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the player attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - GetAttributes
        /// </remarks>
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
        ///   "status": 200,
        ///   "data": {
        ///     "attributes": {
        ///          "key1": "value1",
        ///          "key2": "value2"
        ///     }
        ///   }
        /// }
        /// </returns>
        public void GetAttributes(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.GetAttributes, null, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Update player attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UpdateAttributes
        /// </remarks>
        /// <param name="in_jsonAttributes">
        /// Single layer json string that is a set of key-value pairs
        /// </param>
        /// <param name="in_wipeExisting">
        /// Whether to wipe existing attributes prior to update.
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
        ///   "status": 200,
        /// }
        /// </returns>
        public void UpdateAttributes(
            string in_jsonAttributes,
            bool in_wipeExisting,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Dictionary<string, object> attributes = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonAttributes);
            data[OperationParam.PlayerStateServiceAttributes.Value] = attributes;
            data[OperationParam.PlayerStateServiceWipeExisting.Value] = in_wipeExisting;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateAttributes, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Remove player attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - RemoveAttributes
        /// </remarks>
        /// <param name="in_jsonAttributeNameList">
        /// Json array of attribute names.
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
        ///   "status": 200,
        /// }
        /// </returns>
        public void RemoveAttributes(
            string in_jsonAttributeNameList,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            List<string> attributeNames = JsonReader.Deserialize<List<string>> (in_jsonAttributeNameList);
            data[OperationParam.PlayerStateServiceAttributes.Value] = attributeNames;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.RemoveAttributes, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }

}
