//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;
using BrainCloud.Internal;
using JsonFx.Json;

namespace BrainCloud
{
    public class BrainCloudLobby
    {
        /// <summary>
        /// 
        /// </summary>
        public BrainCloudLobby(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// 
        /// </summary>
        public void FindLobby(string[] in_msgsIds, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingMessageIds.Value] = in_msgsIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.DeleteMessages, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateLobby(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetMessageBoxes, null, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void FindOrCreateLobby(SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetMessageCounts, null, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void UpdateReady(string in_context, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            var context = JsonReader.Deserialize<Dictionary<string, object>>(in_context);
            data[OperationParam.MessagingContext.Value] = context;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetMessagesPage, data, callback);
            m_clientRef.SendRequest(sc);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void UpdateLobbyConfig( string in_context, int pageOffset, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            var data = new Dictionary<string, object>();
            data[OperationParam.MessagingContext.Value] = in_context;
            data[OperationParam.MessagingPageOffset.Value] = pageOffset;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.GetMessagesPageOffset, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SwitchTeam(string[] in_msgsIds, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingMessageIds.Value] = in_msgsIds;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.MarkMessagesRead, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendSignal(string in_fromName, string[] in_toProfileIds, string in_messageText, string in_messageSubject, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> content = new Dictionary<string, object>();
            content[OperationParam.MessagingText.Value] = in_messageText;
            content[OperationParam.MessagingSubject.Value] = in_messageSubject;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingFromName.Value] = in_fromName;
            data[OperationParam.MessagingToProfileIds.Value] = in_toProfileIds;
            data[OperationParam.MessagingContent.Value] = content;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.SendMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void LeaveLobby(string in_fromName, string[] in_toProfileIds, string in_messageText, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingFromName.Value] = in_fromName;
            data[OperationParam.MessagingToProfileIds.Value] = in_toProfileIds;
            data[OperationParam.MessagingText.Value] = in_messageText;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.SendSimpleMessage, data, callback);
            m_clientRef.SendRequest(sc);
        }


        /// <summary>
        /// 
        /// </summary>
        /// 
        public void removeMember(string in_fromName, string[] in_toProfileIds, string in_messageText, SuccessCallback success = null, FailureCallback failure = null, object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.MessagingFromName.Value] = in_fromName;
            data[OperationParam.MessagingToProfileIds.Value] = in_toProfileIds;
            data[OperationParam.MessagingText.Value] = in_messageText;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Lobby, ServiceOperation.SendSimpleMessage, data, callback);
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
