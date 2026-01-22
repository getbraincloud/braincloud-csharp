// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;

    public class BrainCloudMessaging
    {
        /// <summary>
        /// 
        /// </summary>
        public BrainCloudMessaging(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Deletes specified user messages on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - DeleteMessages
        /// </remarks>
        /// <param name="msgIds">Arrays of message ids to delete.</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void DeleteMessages(string in_msgBox, string[] in_msgsIds, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingMessageBox.Value] = in_msgBox;
            data[OperationParam.MessagingMessageIds.Value] = in_msgsIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.DeleteMessages, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve user's message boxes, including 'inbox', 'sent', etc.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - GetMessageboxes
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMessageboxes(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.GetMessageBoxes, null, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieve user's message boxes, including 'inbox', 'sent', etc.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - GetMessageCounts
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMessageCounts(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.GetMessageCounts, null, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves list of specified messages.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - GetMessages
        /// </remarks>
        /// <param name="msgIds">Arrays of message ids to get.</param>
        /// <param name="markAsRead">mark messages that are read</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMessages(string in_msgBox, string[] in_msgsIds, bool markAsRead, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingMessageBox.Value] = in_msgBox;
            data[OperationParam.MessagingMessageIds.Value] = in_msgsIds;
            data[OperationParam.MessagingMarkAsRead.Value] = markAsRead;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.GetMessages, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retrieves a page of messages.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - GetMessagesPage
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMessagesPage(string in_context, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            var context = JsonReader.Deserialize<Dictionary<string, object>>(in_context);
            data[OperationParam.MessagingContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.GetMessagesPage, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Gets the page of messages from the server based on the encoded context and specified page offset.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - GetMessagesPageOffset
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetMessagesPageOffset(string in_context, int pageOffset, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.MessagingContext.Value] = in_context;
            data[OperationParam.MessagingPageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.GetMessagesPageOffset, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Marks list of user messages as read on the server.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - MarkMessagesRead
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void MarkMessagesRead(string in_msgBox, string[] in_msgsIds, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingMessageBox.Value] = in_msgBox;
            data[OperationParam.MessagingMessageIds.Value] = in_msgsIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.MarkMessagesRead, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a message with specified 'subject' and 'text' to list of users.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - SendMessage
        /// </remarks>
        /// <param name="contentJson">the message you are sending</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void SendMessage(string[] in_toProfileIds, string in_contentJson, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingToProfileIds.Value] = in_toProfileIds;

            var content = JsonReader.Deserialize<Dictionary<string, object>>(in_contentJson);
            data[OperationParam.MessagingContent.Value] = content;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.SendMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a simple message to specified list of users.
        /// </summary>
        /// <remarks>
        /// Service Name - Messaging
        /// Service Operation - SendMessageSimple
        /// </remarks>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void SendMessageSimple(string[] in_toProfileIds, string in_messageText, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingToProfileIds.Value] = in_toProfileIds;
            data[OperationParam.MessagingText.Value] = in_messageText;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Messaging, ServiceOperation.SendMessageSimple, data, callback);
            m_clientRef.SendRequest(sc);
        }

        #region private
        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion
    }
}
