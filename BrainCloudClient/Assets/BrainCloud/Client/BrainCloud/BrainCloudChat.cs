//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;
using System;

namespace BrainCloud
{
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
        /// Registers a listener for incoming events from <channelId>. Also returns a list of <maxReturn> recent messages from history.
        /// </summary>
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
        /// Gets the channelId for the given <channelType> and <channelSubId>. Channel type must be one of "gl"(GlobalChannelType) or "gr"(GroupChannelType).
        /// </summary>
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
        /// Gets description info and activity stats for channel <channelId>. Note that numMsgs and listeners only returned for non-global groups. Only callable for channels the user is a member of.
        /// </summary>
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
        /// Get a list of <maxReturn> messages from history of channel <channelId>
        /// </summary>
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
        /// Gets a list of the channels of type <channelType> that the user has access to. Channel type must be one of "gl"(GlobalChannelType), "gr"(GroupChannelType) or "all"(AllChannelType).
        /// </summary>
        public void GetSubscribedChannels(string in_channelType, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.ChatChannelType.Value] = in_channelType;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Chat, ServiceOperation.GetSubscribedChannels, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// Sends a potentially richer member chat message. By convention, content should contain a field named text for plain-text content. Returns the id of the message created.
        /// </summary>
        /// 
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
        /// Sends a plain-text chat message.
        /// </summary>
        /// 
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
        /// Update the specified chat message. Message must have been from this user. Version provided must match (or pass -1 to bypass version enforcement).
        /// </summary>
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
