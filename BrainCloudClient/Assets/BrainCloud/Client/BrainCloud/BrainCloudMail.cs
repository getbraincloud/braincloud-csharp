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
    public class BrainCloudMail
    {
        private BrainCloudClient _clientRef;

        public BrainCloudMail(BrainCloudClient client)
        {
            _clientRef = client;
        }

        /// <summary>
        /// Sends a simple text email to the specified user
        /// </summary>
        /// <remarks>
        /// Service Name - mail
        /// Service Operation - SEND_BASIC_EMAIL
        /// </remarks>
        /// <param name="toProfileId">
        /// The user to send the email to
        /// </param>
        /// <param name="subject">
        /// The email subject
        /// </param>
        /// <param name="body">
        /// The email body
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
        public void SendBasicEmail(
            string profileId,
            string subject,
            string body,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.ProfileId.Value] = profileId;
            data[OperationParam.Subject.Value] = subject;
            data[OperationParam.Body.Value] = body;

            SendMessage(ServiceOperation.SendBasicEmail, data, success, failure, cbObject);
        }

        /// <summary>
        /// Sends an advanced email to the specified user
        /// </summary>
        /// <remarks>
        /// Service Name - mail
        /// Service Operation - SEND_ADVANCED_EMAIL
        /// </remarks>
        /// <param name="toProfileId">
        /// The user to send the email to
        /// </param>
        /// <param name="jsonServiceParams">
        /// Parameters to send to the email service. See the documentation for
        /// a full list. http://getbraincloud.com/apidocs/apiref/#capi-mail
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
        public void SendAdvancedEmail(
            string profileId,
            string jsonServiceParams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.ProfileId.Value] = profileId;
            data[OperationParam.ServiceParams.Value] = JsonReader.Deserialize<Dictionary<string, object>>(jsonServiceParams);

            SendMessage(ServiceOperation.SendAdvancedEmail, data, success, failure, cbObject);
        }

        /// <summary>
        /// Sends an advanced email to the specified email address
        /// </summary>
        /// <remarks>
        /// Service Name - mail
        /// Service Operation - SEND_ADVANCED_EMAIL_BY_EMAIL
        /// </remarks>
        /// <param name="emailAddress">
        /// The address to send the email to
        /// </param>
        /// <param name="jsonServiceParams">
        /// Parameters to send to the email service. See the documentation for
        /// a full list. http://getbraincloud.com/apidocs/apiref/#capi-mail
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
        public void SendAdvancedEmailByAddress(
            string emailAddress,
            string jsonServiceParams,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.EmailAddress.Value] = emailAddress;
            data[OperationParam.ServiceParams.Value] = JsonReader.Deserialize<Dictionary<string, object>>(jsonServiceParams);

            SendMessage(ServiceOperation.SendAdvancedEmailByAddress, data, success, failure, cbObject);
        }

        // Private
        private void SendMessage(
            ServiceOperation operation,
            Dictionary<string, object> data,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            _clientRef.SendRequest(new ServerCall(ServiceName.Mail, operation, data, callback));
        }
    }
}
