//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Common;
using BrainCloud.Internal;

#if !(DOT_NET)
using System;
#endif

namespace BrainCloud
{
    public class BrainCloudPushNotification
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPushNotification(BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
        }

#if !(DOT_NET)
        /// <summary>
        /// Registers the given device token with the server to enable this device
        /// to receive push notifications.
        /// </param>
        /// <param name="token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public bool RegisterPushNotificationDeviceToken(
            byte[] token,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {

            if (token != null || token.Length < 1)
            {
                byte[] tokenData = token;

                Platform platform = Platform.FromUnityRuntime();
                string hexToken = BitConverter.ToString(tokenData).Replace("-","").ToLower();
                RegisterPushNotificationDeviceToken(platform,
                        hexToken,
                        success,
                        failure,
                        cbObject);
                return true;
            }
            // there was an error
            else
            {
                return false;
            }

        }
#endif

        /// <summary>
        /// Deregisters all device tokens currently registered to the player.
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void DeregisterAllPushNotificationDeviceTokens(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.DeregisterAll, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Deregisters the given device token from the server to disable this device
        /// from receiving push notifications.
        /// </param>
        /// <param name="platform">
        /// The device platform being registered.
        /// </param>
        /// <param name="token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void DeregisterPushNotificationDeviceToken(
            Platform platform,
            string token,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            string devicePlatform = platform.ToString();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationRegisterParamDeviceType.Value] = devicePlatform;
            data[OperationParam.PushNotificationRegisterParamDeviceToken.Value] = token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Deregister, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Registers the given device token with the server to enable this device
        /// to receive push notifications.
        /// </param>
        /// <param name="device">
        /// The device platform being registered.
        /// </param>
        /// <param name="token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void RegisterPushNotificationDeviceToken(
            Platform platform,
            string token,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            string devicePlatform = platform.ToString();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationRegisterParamDeviceType.Value] = devicePlatform;
            data[OperationParam.PushNotificationRegisterParamDeviceToken.Value] = token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Register, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a simple push notification based on the passed in message.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="message">
        /// Text of the push notification
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendSimplePushNotification(
            string toPlayerId,
            string message,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toPlayerId;
            data[OperationParam.PushNotificationSendParamMessage.Value] = message;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendSimple, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="notificationTemplateId">
        /// Id of the notification template
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendRichPushNotification(
            string toPlayerId,
            int notificationTemplateId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRichPushNotification(toPlayerId, notificationTemplateId, null, success, failure, cbObject);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// Includes JSON defining the substitution params to use with the template.
        /// See the Portal documentation for more info.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="notificationTemplateId">
        /// Id of the notification template
        /// </param>
        /// <param name="substitutionJson">
        /// JSON defining the substitution params to use with the template
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendRichPushNotificationWithParams(
            string toPlayerId,
            int notificationTemplateId,
            string substitutionJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRichPushNotification(toPlayerId, notificationTemplateId, substitutionJson, success, failure, cbObject);
        }

        /// <summary>
        /// Sends a notification to a "group" of user based on a brainCloud portal configured notification template.
        /// Includes JSON defining the substitution params to use with the template.
        /// See the Portal documentation for more info.
        /// </param>
        /// <param name="groupId">
        /// Target group
        /// </param>
        /// <param name="notificationTemplateId">
        /// Id of the notification template
        /// </param>
        /// <param name="substitutionsJson">
        /// JSON defining the substitution params to use with the template
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendTemplatedPushNotificationToGroup(
            string groupId,
            int notificationTemplateId,
            string substitutionsJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = notificationTemplateId;

            if (Util.IsOptionalParameterValid(substitutionsJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(substitutionsJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendTemplatedToGroup, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a "group" of user based on a brainCloud portal configured notification template.
        /// Includes JSON defining the substitution params to use with the template.
        /// See the Portal documentation for more info.
        /// </param>
        /// <param name="groupId">
        /// Target group
        /// </param>
        /// <param name="alertContentJson">
        /// Body and title of alert
        /// </param>
        /// <param name="customDataJson">
        /// Optional custom data
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendNormalizedPushNotificationToGroup(
            string groupId,
            string alertContentJson,
            string customDataJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);
            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendNormalizedToGroup, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a user consisting of alert content and custom data.
        /// </param>
        /// <param name="toPlayerId">
        /// The playerId of the user to receive the notification
        /// </param>
        /// <param name="alertContentJson">
        /// Body and title of alert
        /// </param>
        /// <param name="customDataJson">
        /// Optional custom data
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendNormalizedPushNotification(
            string toPlayerId,
            string alertContentJson,
            string customDataJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toPlayerId;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);
            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendNormalized, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to multiple users consisting of alert content and custom data.
        /// </param>
        /// <param name="profileIds">
        /// Collection of profile IDs to send the notification to
        /// </param>
        /// <param name="alertContentJson">
        /// Body and title of alert
        /// </param>
        /// <param name="customDataJson">
        /// Optional custom data
        /// </param>
        /// <param name="success">
        /// The success callback
        /// </param>
        /// <param name="failure">
        /// The failure callback
        /// </param>
        /// <param name="cbObject">
        /// The callback object
        /// </param>
        public void SendNormalizedPushNotificationBatch(
            IList<string> profileIds,
            string alertContentJson,
            string customDataJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamProfileIds.Value] = profileIds;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);
            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendNormalizedBatch, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

#region Private

        private void SendRichPushNotification(
            string toPlayerId,
            int notificationTemplateId,
            string substitutionJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toPlayerId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = notificationTemplateId;

            if (Util.IsOptionalParameterValid(substitutionJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(substitutionJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRich, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

#endregion
    }
}
