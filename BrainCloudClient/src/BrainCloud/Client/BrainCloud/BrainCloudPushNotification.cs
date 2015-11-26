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
using BrainCloud.Common;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudPushNotification
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPushNotification(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

#if !(DOT_NET)
        /// <summary>
        /// Registers the given device token with the server to enable this device
        /// to receive push notifications.
        /// </param>
        /// <param name="in_token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the new value of the statistics and any rewards that were triggered:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public bool RegisterPushNotificationDeviceToken(
            byte[] in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {

            if (in_token != null || in_token.Length < 1)
            {
                byte[] token = in_token;

                Platform platform = Platform.FromUnityRuntime();
                string hexToken = System.BitConverter.ToString(token).Replace("-","").ToLower();
                RegisterPushNotificationDeviceToken(platform,
                        hexToken,
                        in_success,
                        in_failure,
                        in_cbObject);
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
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void DeregisterAllPushNotificationDeviceTokens(
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
           {
            Dictionary<string, object> data = new Dictionary<string, object>();

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.DeregisterAll, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Deregisters the given device token from the server to disable this device
        /// from receiving push notifications.
        /// </param>
        /// <param name="in_platform">
        /// The device platform being registered.
        /// </param>
        /// <param name="in_token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void DeregisterPushNotificationDeviceToken(
            Platform in_platform,
            string in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            string devicePlatform = in_platform.ToString();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationRegisterParamDeviceType.Value] = devicePlatform;
            data[OperationParam.PushNotificationRegisterParamDeviceToken.Value] = in_token;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Deregister, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Registers the given device token with the server to enable this device
        /// to receive push notifications.
        /// </param>
        /// <param name="in_device">
        /// The device platform being registered.
        /// </param>
        /// <param name="in_token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void RegisterPushNotificationDeviceToken(
            Platform in_platform,
            string in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            string devicePlatform = in_platform.ToString();
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationRegisterParamDeviceType.Value] = devicePlatform;
            data[OperationParam.PushNotificationRegisterParamDeviceToken.Value] = in_token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Register, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }


        /// <summary>
        /// Registers the given device token with the server to enable this device
        /// to receive push notifications.
        /// </param>
        /// <param name="in_device">
        /// The device platform being registered.
        /// </param>
        /// <param name="in_token">
        /// The platform-dependant device token needed for push notifications.
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        [Obsolete("Use RegisterPushNotificationDeviceToken with Platform object instead of passing the in_device string directly - removal in 90 days, 2015-12-15")]
        public void RegisterPushNotificationDeviceToken(
            string in_device,
            string in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            RegisterPushNotificationDeviceToken(
                Platform.FromString(in_device),
                in_token,
                in_success,
                in_failure,
                in_cbObject);
        }

        /// <summary>
        /// Sends a simple push notification based on the passed in message.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="in_toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="in_message">
        /// Text of the push notification
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void SendSimplePushNotification(
            string in_toPlayerId,
            string in_message,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = in_toPlayerId;
            data[OperationParam.PushNotificationSendParamMessage.Value] = in_message;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendSimple, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="in_toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="in_notificationTemplateId">
        /// Id of the notification template
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void SendRichPushNotification(
            string in_toPlayerId,
            int in_notificationTemplateId,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            SendRichPushNotification(in_toPlayerId, in_notificationTemplateId, null, in_success, in_failure, in_cbObject);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// Includes JSON defining the substitution params to use with the template.
        /// See the Portal documentation for more info.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="in_toPlayerId">
        /// The braincloud playerId of the user to receive the notification
        /// </param>
        /// <param name="in_notificationTemplateId">
        /// Id of the notification template
        /// </param>
        /// <param name="in_substitutionJson">
        /// JSON defining the substitution params to use with the template
        /// </param>
        /// <param name="in_success">
        /// The success callback
        /// </param>
        /// <param name="in_failure">
        /// The failure callback
        /// </param>
        /// <param name="in_cbObject">
        /// The callback object
        /// </param>
        /// <returns> JSON describing the result
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public void SendRichPushNotificationWithParams(
            string in_toPlayerId,
            int in_notificationTemplateId,
            string in_substitutionJson,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            SendRichPushNotification(in_toPlayerId, in_notificationTemplateId, in_substitutionJson, in_success, in_failure, in_cbObject);
        }

        //Internal
        private void SendRichPushNotification(
            string in_toPlayerId,
            int in_notificationTemplateId,
            string in_substitutionJson,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = in_toPlayerId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = in_notificationTemplateId;

            if (Util.IsOptionalParameterValid(in_substitutionJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(in_substitutionJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRich, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
