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

    public class BrainCloudChat
    {
        /// <summary>
        /// 
        /// </summary>
        public BrainCloudChat(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
/// Registers a listener for incoming events from <channelId>.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - ChannelConnect
/// </remarks>
/// <param name="channelId">The id of the chat channel to return history from.</param>
/// <param name="maxReturn">Maximum number of messages to return.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void ChannelConnect(string in_channelId, int in_maxToReturn, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMaxReturn.Value] = in_maxToReturn;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.ChannelConnect, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Unregisters a listener for incoming events from <channelId>.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - ChannelDisconnect
/// </remarks>
/// <param name="channelId">The id of the chat channel to unsubscribed from.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void ChannelDisconnect(string in_channelId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.ChannelDisconnect, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Delete a chat message. <version> must match the latest or pass -1 to bypass version check.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - DeleteChatMessage
/// </remarks>
/// <param name="channelId">The id of the chat channel that contains the message to delete.</param>
/// <param name="msgId">The message id to delete.</param>
/// <param name="version">Version of the message to delete. Must match latest or pass -1 to bypass version check.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void DeleteChatMessage(string in_channelId, string in_messageId, int in_version, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMessageId.Value] = in_messageId;
            data[OperationParam.ChatVersion.Value] = in_version;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.DeleteChatMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Gets the channelId for the given <channelType> and <channelSubId>. Channel type must be one of "gl" or "gr".
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - GetChannelId
/// </remarks>
/// <param name="channelType">Channel type must be one of "gl" or "gr". For (global) or (group) respectively.</param>
/// <param name="channelSubId">The sub id of the channel.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void GetChannelId(string in_channelType, string in_channelSubId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelType.Value] = in_channelType;
            data[OperationParam.ChatChannelSubId.Value] = in_channelSubId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetChannelId, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Gets description info and activity stats for channel <channelId>.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - GetChannelInfo
/// </remarks>
/// <param name="channelId">Id of the channel to receive the info from.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void GetChannelInfo(string in_channelId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetChannelInfo, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Gets a populated chat object (normally for editing).
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - GetChatMessage
/// </remarks>
/// <param name="channelId">Id of the channel to receive the message from.</param>
/// <param name="msgId">Id of the message to read.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void GetChatMessage(string in_channelId, string in_messageId, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMessageId.Value] = in_messageId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetChatMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Get a list of <maxReturn> messages from history of channel <channelId>.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - GetRecentChatMessages
/// </remarks>
/// <param name="channelId">Id of the channel to receive the info from.</param>
/// <param name="maxReturn">Maximum message count to return.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void GetRecentChatMessages(string in_channelId, int in_maxToReturn, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMaxReturn.Value] = in_maxToReturn;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetRecentChatMessages, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Gets a list of the channels of type <channelType> that the user has access to.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - GetSubscribedChannels
/// </remarks>
/// <param name="channelType">Type of channels to get back. "gl" for global, "gr" for group or "all" for both.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void GetSubscribedChannels(string in_channelType, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelType.Value] = in_channelType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetSubscribedChannels, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Send a potentially rich chat message.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - PostChatMessage
/// </remarks>
/// <param name="channelId">Channel id to post message to.</param>
/// <param name="content">Object containing "text" for the text message. Can also has rich content for custom data.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>
 
        public void PostChatMessage(string in_channelId, string in_contentJson, bool in_recordInHistory = true, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatContent.Value] = JsonReader.Deserialize<Dictionary<string, object>> (in_contentJson);
            data[OperationParam.ChatRecordInHistory.Value] = in_recordInHistory;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.PostChatMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /// <summary>
        /// Send a potentially rich chat message. <content> must contain at least a "plain" field for plain-text messaging.
        /// </summary>
        /// 
        public void PostChatMessage(string in_channelId, string in_plain, string in_jsonRich, bool in_recordInHistory = true, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Build message content
            Dictionary<string, object> content = new Dictionary<string, object>();
            content[OperationParam.ChatText.Value] = in_plain;
            if (Util.IsOptionalParameterValid(in_jsonRich))
            {
                Dictionary<string, object> jsonRich = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonRich);
                content[OperationParam.ChatRich.Value] = jsonRich;
            }
            else
            {
                Dictionary<string, object> jsonRich = JsonReader.Deserialize<Dictionary<string, object>> ("{}");
                content[OperationParam.ChatRich.Value] = jsonRich;
            }

            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatContent.Value] = content;
            data[OperationParam.ChatRecordInHistory.Value] = in_recordInHistory;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.PostChatMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /// <summary>
/// Send a chat message with text only
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - PostChatMessage
/// </remarks>
/// <param name="channelId">Channel id to post message to.</param>
/// <param name="text">The text message.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>
 
        public void PostChatMessageSimple(string in_channelId, string in_plain, bool in_recordInHistory = true, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatText.Value] = in_plain;
            data[OperationParam.ChatRecordInHistory.Value] = in_recordInHistory;
            
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.PostChatMessageSimple, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
/// Update a chat message.
/// </summary>
/// <remarks>
/// Service Name - Chat
/// Service Operation - UpdateChatMessage
/// </remarks>
/// <param name="channelId">Channel id where the message to update is.</param>
/// <param name="msgId">Message id to update.</param>
/// <param name="version">Version of the message to update. Must match latest or pass -1 to bypass version check.</param>
/// <param name="content">Data to update. Object containing "text" for the text message. Can also has rich content for custom data.</param>
/// <param name="success">The success callback.</param>
/// <param name="failure">The failure callback.</param>
/// <param name="cbObject">The user object sent to the callback.</param>

        public void UpdateChatMessage(string in_channelId, string in_messageId, int in_version, string in_contentJson, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMessageId.Value] = in_messageId;
            data[OperationParam.ChatVersion.Value] = in_version;
            data[OperationParam.ChatContent.Value] = JsonReader.Deserialize<Dictionary<string, object>> (in_contentJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.UpdateChatMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /// <summary>
        /// Update a chat message. <content> must contain at least a "plain" field for plain-text messaging. <version> must match the latest or pass -1 to bypass version check.
        /// </summary>
        public void UpdateChatMessage(string in_channelId, string in_messageId, int in_version, string in_plain, string in_jsonRich, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> content = new Dictionary<string, object>();
            content[OperationParam.ChatText.Value] = in_plain;
            if (Util.IsOptionalParameterValid(in_jsonRich))
            {
                Dictionary<string, object> jsonRich = JsonReader.Deserialize<Dictionary<string, object>> (in_jsonRich);
                content[OperationParam.ChatRich.Value] = jsonRich;
            }
            else
            {
                Dictionary<string, object> jsonRich = JsonReader.Deserialize<Dictionary<string, object>> ("{}");
                content[OperationParam.ChatRich.Value] = jsonRich;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelId.Value] = in_channelId;
            data[OperationParam.ChatMessageId.Value] = in_messageId;
            data[OperationParam.ChatVersion.Value] = in_version;
            data[OperationParam.ChatContent.Value] = content;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.UpdateChatMessage, data, callback);
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
