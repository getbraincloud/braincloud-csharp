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
    public class BrainCloudPlayerState
    {
        private BrainCloudClient _client;

        public BrainCloudPlayerState(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Read the state of the currently logged in user.
        /// This method returns a JSON object describing most of the
        /// player's data: entities, statistics, level, currency.
        /// Apps will typically call this method after authenticating to get an
        /// up-to-date view of the user's data.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - Read
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
        public void ReadUserState(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Read, null, callback);
            _client.SendRequest(sc);
        }

        [Obsolete("This has been deprecated. Use DeleteUser instead - removal after September 1 2017")]
        public void DeletePlayer(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.FullReset, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Completely deletes the user record and all data fully owned
        /// by the user. After calling this method, the user will need
        /// to re-authenticate and create a new profile.
        /// This is mostly used for debugging/qa.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - FullReset
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
        public void DeleteUser(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.FullReset, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// This method will delete *most* data for the currently logged in user.
        /// Data which is not deleted includes: currency, credentials, and
        /// purchase transactions. ResetUser is different from DeleteUser in that
        /// the player record will continue to exist after the reset (so the user
        /// does not need to re-authenticate).
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - DataReset
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
        public void ResetUser(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.DataReset, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Logs user out of server.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - Logout
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
        public void Logout(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.Logout, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sets the user name.
        /// </summary>
        /// <remarks>
        /// Service Name - playerState
        /// Service Operation - UPDATE_NAME
        /// </remarks>
        /// <param name="userName">
        /// The name of the user
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
        public void UpdateUserName(
            string userName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceUpdateNameData.Value] = userName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateName, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates the "friend summary data" associated with the logged in user.
        /// Some operations will return this summary data. For instance the social
        /// leaderboards will return the player's score in the leaderboard along
        /// with the friend summary data. Generally this data is used to provide
        /// a quick overview of the player without requiring a separate API call
        /// to read their public stats or entity data.
        ///
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UpdateSummary
        /// </remarks>
        /// <param name="jsonSummaryData">
        /// A JSON string defining the summary data.
        /// For example:
        /// {
        ///   "xp":123,
        ///   "level":12,
        ///   "highScore":45123
        /// }
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        ///
        /// </param>
        public void UpdateSummaryFriendData(
            string jsonSummaryData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (Util.IsOptionalParameterValid(jsonSummaryData))
            {
                Dictionary<string, object> summaryData = JsonReader.Deserialize<Dictionary<string, object>>(jsonSummaryData);
                data[OperationParam.PlayerStateServiceUpdateSummaryFriendData.Value] = summaryData;
            }
            else data = null;
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateSummary, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve the user's attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - GetAttributes
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
        public void GetAttributes(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.GetAttributes, null, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Update user's attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UpdateAttributes
        /// </remarks>
        /// <param name="jsonAttributes">
        /// Single layer json string that is a set of key-value pairs
        /// </param>
        /// <param name="wipeExisting">
        /// Whether to wipe existing attributes prior to update.
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
        public void UpdateAttributes(
            string jsonAttributes,
            bool wipeExisting,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            Dictionary<string, object> attributes = JsonReader.Deserialize<Dictionary<string, object>>(jsonAttributes);
            data[OperationParam.PlayerStateServiceAttributes.Value] = attributes;
            data[OperationParam.PlayerStateServiceWipeExisting.Value] = wipeExisting;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateAttributes, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Remove user's attributes.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - RemoveAttributes
        /// </remarks>
        /// <param name="attributeNames">
        /// List of attribute names.
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
        public void RemoveAttributes(
            IList<string> attributeNames,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceAttributes.Value] = attributeNames;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.RemoveAttributes, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Updates player's picture URL.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UPDATE_PICTURE_URL
        /// </remarks>
        /// <param name="pictureUrl">
        /// URL to apply.
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
        public void UpdateUserPictureUrl(
            string pictureUrl,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServicePlayerPictureUrl.Value] = pictureUrl;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdatePictureUrl, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Update the user's contact email. 
        /// Note this is unrelated to email authentication.
        /// </summary>
        /// <remarks>
        /// Service Name - PlayerState
        /// Service Operation - UPDATE_CONTACT_EMAIL
        /// </remarks>
        /// <param name="contactEmail">
        /// Updated email
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
        public void UpdateContactEmail(
            string contactEmail,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PlayerStateServiceContactEmail.Value] = contactEmail;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PlayerState, ServiceOperation.UpdateContactEmail, data, callback);
            _client.SendRequest(sc);
        }
    }
}
