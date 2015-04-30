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
    public class BrainCloudPushNotification
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudPushNotification (BrainCloudClient in_brainCloudClientRef)
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

                // send token to a provider
                // default to iOS
                // TODO: implement other device types
                string deviceType = OperationParam.DeviceRegistrationTypeIos.Value;
                if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android)
                {
                    deviceType = OperationParam.DeviceRegistrationTypeAndroid.Value;
                }

                string hexToken = System.BitConverter.ToString(token).Replace("-","").ToLower();
                return RegisterPushNotificationDeviceToken(deviceType,
                        hexToken,
                        in_success,
                        in_failure,
                        in_cbObject);
            }
            // there was an error
            else
            {
                return false;
            }

        }
#endif
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
        /// <returns> JSON describing the new value of the statistics and any rewards that were triggered:
        /// {
        ///   "status":200,
        ///   "data":null
        /// }
        /// </returns>
        public bool RegisterPushNotificationDeviceToken(
            string in_device,
            string in_token,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            bool bToReturn = false;

            JsonData data = new JsonData();

            data[OperationParam.PushNotificationRegisterParamDeviceType.Value] = in_device;
            data[OperationParam.PushNotificationRegisterParamDeviceToken.Value] = in_token;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Register, data, callback);
            m_brainCloudClientRef.SendRequest(sc);

            bToReturn = true;
            return bToReturn;
        }

        /// <summary>
        /// Create a new push notification for the current user to send to another user.
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
            JsonData data = new JsonData();

            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = in_toPlayerId;
            data[OperationParam.PushNotificationSendParamMessage.Value] = in_message;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendSimple, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Create a new push notification for the current user to send to another user.
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
            JsonData data = new JsonData();

            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = in_toPlayerId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = in_notificationTemplateId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.Create, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}
