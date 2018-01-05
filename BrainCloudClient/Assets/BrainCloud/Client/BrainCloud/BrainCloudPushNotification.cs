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
        private BrainCloudClient _client;

        public BrainCloudPushNotification(BrainCloudClient client)
        {
            _client = client;
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
        /// Deregisters all device tokens currently registered to the user.
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
            _client.SendRequest(sc);
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
            _client.SendRequest(sc);
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a simple push notification based on the passed in message.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toProfileId">
        /// The braincloud profileId of the user to receive the notification
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
            string toProfileId,
            string message,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toProfileId;
            data[OperationParam.PushNotificationSendParamMessage.Value] = message;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendSimple, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toProfileId">
        /// The braincloud profileId of the user to receive the notification
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
            string toProfileId,
            int notificationTemplateId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRichPushNotification(toProfileId, notificationTemplateId, null, success, failure, cbObject);
        }

        /// <summary>
        /// Sends a notification to a user based on a brainCloud portal configured notification template.
        /// Includes JSON defining the substitution params to use with the template.
        /// See the Portal documentation for more info.
        /// NOTE: It is possible to send a push notification to oneself.
        /// </param>
        /// <param name="toProfileId">
        /// The braincloud profileId of the user to receive the notification
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
            string toProfileId,
            int notificationTemplateId,
            string substitutionJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRichPushNotification(toProfileId, notificationTemplateId, substitutionJson, success, failure, cbObject);
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
            _client.SendRequest(sc);
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
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Schedules raw notifications based on user local time.
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="fcmContent">
        /// Valid Fcm data content
        /// </param>
        /// <param name="iosContent">
        /// Valid ios data content
        /// </param>
        /// <param name="facebookContent">
        /// Facebook template string
        /// </param>
        /// <param name="startTime">
        /// Start time of sending the push notification
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
        public void ScheduleRawPushNotificationUTC(
            string profileId,
            string fcmContent,
            string iosContent,
            string facebookContent,
            int startTime,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfileId.Value] = profileId;

            if (Util.IsOptionalParameterValid(fcmContent))
            {
                data[OperationParam.PushNotificationSendParamFcmContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fcmContent);
            }

            if (Util.IsOptionalParameterValid(iosContent))
            {
                data[OperationParam.PushNotificationSendParamIosContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(iosContent);
            }

            if (Util.IsOptionalParameterValid(facebookContent))
            {
                data[OperationParam.PushNotificationSendParamFacebookContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(facebookContent);
            }

            data[OperationParam.StartDateUTC.Value] = startTime;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleRawNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Schedules raw notifications based on user local time.
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="fcmContent">
        /// Valid Fcm data content
        /// </param>
        /// <param name="iosContent">
        /// Valid ios data content
        /// </param>
        /// <param name="facebookContent">
        /// Facebook template string
        /// </param>
        /// <param name="minutesFromNow">
        /// Minutes from now to send the push notification
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
        public void ScheduleRawPushNotificationMinutes(
            string profileId,
            string fcmContent,
            string iosContent,
            string facebookContent,
            int minutesFromNow,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ProfileId.Value] = profileId;

            if (Util.IsOptionalParameterValid(fcmContent))
            {
                data[OperationParam.PushNotificationSendParamFcmContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fcmContent);
            }

            if (Util.IsOptionalParameterValid(iosContent))
            {
                data[OperationParam.PushNotificationSendParamIosContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(iosContent);
            }

            if (Util.IsOptionalParameterValid(facebookContent))
            {
                data[OperationParam.PushNotificationSendParamFacebookContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(facebookContent);
            }

            data[OperationParam.MinutesFromNow.Value] = minutesFromNow;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleRawNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a raw push notification to a target user.
        /// </param>
        /// <param name="toProfileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="fcmContent">
        /// Valid Fcm data content
        /// </param>
        /// <param name="iosContent">
        /// Valid ios data content
        /// </param>
        /// <param name="facebookContent">
        /// Facebook template string
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
        public void SendRawPushNotification(
            string toProfileId,
            string fcmContent,
            string iosContent,
            string facebookContent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toProfileId;
            
            if (Util.IsOptionalParameterValid(fcmContent))
            {
                data[OperationParam.PushNotificationSendParamFcmContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fcmContent);
            }

            if (Util.IsOptionalParameterValid(iosContent))
            {
                data[OperationParam.PushNotificationSendParamIosContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(iosContent);
            }

            if (Util.IsOptionalParameterValid(facebookContent))
            {
                data[OperationParam.PushNotificationSendParamFacebookContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(facebookContent);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRaw, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a raw push notification to a target list of users.
        /// </param>
        /// <param name="profileIds">
        /// Collection of profile IDs to send the notification to
        /// </param>
        /// <param name="fcmContent">
        /// Valid Fcm data content
        /// </param>
        /// <param name="iosContent">
        /// Valid ios data content
        /// </param>
        /// <param name="facebookContent">
        /// Facebook template string
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
        public void SendRawPushNotificationBatch(
            IList<string> profileIds,
            string fcmContent,
            string iosContent,
            string facebookContent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.PushNotificationSendParamProfileIds.Value] = profileIds;
            
            if (Util.IsOptionalParameterValid(fcmContent))
            {
                data[OperationParam.PushNotificationSendParamFcmContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fcmContent);
            }

            if (Util.IsOptionalParameterValid(iosContent))
            {
                data[OperationParam.PushNotificationSendParamIosContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(iosContent);
            }

            if (Util.IsOptionalParameterValid(facebookContent))
            {
                data[OperationParam.PushNotificationSendParamFacebookContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(facebookContent);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRawBatch, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a raw push notification to a target group.
        /// </param>
        /// <param name="groupId">
        /// Target group
        /// </param>
        /// <param name="fcmContent">
        /// Valid Fcm data content
        /// </param>
        /// <param name="iosContent">
        /// Valid ios data content
        /// </param>
        /// <param name="facebookContent">
        /// Facebook template string
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
        public void SendRawPushNotificationToGroup(
            string groupId,
            string fcmContent,
            string iosContent,
            string facebookContent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            if (Util.IsOptionalParameterValid(fcmContent))
            {
                data[OperationParam.PushNotificationSendParamFcmContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(fcmContent);
            }

            if (Util.IsOptionalParameterValid(iosContent))
            {
                data[OperationParam.PushNotificationSendParamIosContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(iosContent);
            }

            if (Util.IsOptionalParameterValid(facebookContent))
            {
                data[OperationParam.PushNotificationSendParamFacebookContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(facebookContent);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRawToGroup, data, callback);
            _client.SendRequest(sc);
        }


        /// <summary>
        /// Schedules a normalized push notification to a user
        /// 
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="alertContentJson">
        /// Body and title of alert
        /// </param>
        /// <param name="customDataJson">
        /// Optional custom data
        /// </param>
        /// <param name="startTime">
        /// Start time of sending the push notification
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
        public void ScheduleNormalizedPushNotificationUTC(
            string profileId,
            string alertContentJson,
            string customDataJson,
            int startTime,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamProfileId.Value] = profileId;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);

            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            data[OperationParam.StartDateUTC.Value] = startTime;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleNormalizedNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Schedules a normalized push notification to a user
        /// 
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="alertContentJson">
        /// Body and title of alert
        /// </param>
        /// <param name="customDataJson">
        /// Optional custom data
        /// </param>
        /// <param name="minutesFromNow">
        /// Minutes from now to send the push notification
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
        public void ScheduleNormalizedPushNotificationMinutes(
            string profileId,
            string alertContentJson,
            string customDataJson,
            int minutesFromNow,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamProfileId.Value] = profileId;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);

            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            data[OperationParam.MinutesFromNow.Value] = minutesFromNow;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleNormalizedNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Schedules a rich push notification to a user
        /// 
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="notificationTemplateId">
        /// Body and title of alert
        /// </param>
        /// <param name="substitutionsJson">
        /// Optional custom data
        /// </param>
        /// <param name="startTime">
        /// Start time of sending the push notification
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
        public void ScheduleRichPushNotificationUTC(
            string profileId,
            int notificationTemplateId,
            string substitutionsJson,
            int startTime,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamProfileId.Value] = profileId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = notificationTemplateId;

            if (Util.IsOptionalParameterValid(substitutionsJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(substitutionsJson);
            }

            data[OperationParam.StartDateUTC.Value] = startTime;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleRichNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Schedules a rich push notification to a user
        /// 
        /// </param>
        /// <param name="profileId">
        /// The profileId of the user to receive the notification
        /// </param>
        /// <param name="notificationTemplateId">
        /// Body and title of alert
        /// </param>
        /// <param name="substitutionsJson">
        /// Optional custom data
        /// </param>
        /// <param name="minutesFromNow">
        /// Minutes from now to send the push notification
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
        public void ScheduleRichPushNotificationMinutes(
            string profileId,
            int notificationTemplateId,
            string substitutionsJson,
            int minutesFromNow,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamProfileId.Value] = profileId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = notificationTemplateId;

            if (Util.IsOptionalParameterValid(substitutionsJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(substitutionsJson);
            }

            data[OperationParam.MinutesFromNow.Value] = minutesFromNow;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.ScheduleRichNotification, data, callback);
            _client.SendRequest(sc);
        }

        /// <summary>
        /// Sends a notification to a user consisting of alert content and custom data.
        /// </param>
        /// <param name="toProfileId">
        /// The profileId of the user to receive the notification
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
            string toProfileId,
            string alertContentJson,
            string customDataJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toProfileId;
            data[OperationParam.AlertContent.Value] = JsonReader.Deserialize<Dictionary<string, object>>(alertContentJson);
            if (Util.IsOptionalParameterValid(customDataJson))
            {
                data[OperationParam.CustomData.Value] = JsonReader.Deserialize<Dictionary<string, object>>(customDataJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendNormalized, data, callback);
            _client.SendRequest(sc);
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
            _client.SendRequest(sc);
        }

#region Private

        private void SendRichPushNotification(
            string toProfileId,
            int notificationTemplateId,
            string substitutionJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.PushNotificationSendParamToPlayerId.Value] = toProfileId;
            data[OperationParam.PushNotificationSendParamNotificationTemplateId.Value] = notificationTemplateId;

            if (Util.IsOptionalParameterValid(substitutionJson))
            {
                data[OperationParam.PushNotificationSendParamSubstitutions.Value] = JsonReader.Deserialize<Dictionary<string, object>>(substitutionJson);
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.PushNotification, ServiceOperation.SendRich, data, callback);
            _client.SendRequest(sc);
        }

#endregion
    }
}
