//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using JsonFx.Json;

namespace BrainCloud.Internal
{
    internal sealed class RSComms
    {
        /// <summary>
        /// 
        /// </summary>
        public RSComms(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Enables Real Time event for this session.
        /// Real Time events are disabled by default. Usually events
        /// need to be polled using GET_EVENTS. By enabling this, events will
        /// be received instantly when they happen through a TCP connection to an Event Server.
        ///
        ///This function will first call requestClientConnection, then connect to the address
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(eRSConnectionType in_connectionType = eRSConnectionType.WEBSOCKET, Dictionary<string, object> in_options = null, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            if (!m_bIsConnected)
            {
                m_connectedSuccessCallback = in_success;
                m_connectionFailureCallback = in_failure;
                m_connectedObj = cb_object;

                m_connectionOptions = in_options;
                m_useWebSocket = in_connectionType == eRSConnectionType.WEBSOCKET;
                connectWebSocket();
            }
        }

        /// <summary>
        /// Disables Real Time event for this session.
        /// </summary>
        public void Disconnect()
        {
            addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "disconnect", "Disconnect Called"));
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void RegisterCallback(RSCallback in_callback)
        {
            m_registeredCallbacks = in_callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterCallback()
        {
            m_registeredCallbacks = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Send(string in_message, Dictionary<string, object> in_dict)
        {
            send("RLAY" + in_message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Echo(string in_message, Dictionary<string, object> in_dict)
        {
            send("ECHO" + in_message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Ping()
        {
            m_sentPing = DateTime.Now.Ticks;
            Dictionary<string, object> json = new Dictionary<string, object>();
            string lastPingStr = (LastPing * 0.0001).ToString();
            int indexOf = lastPingStr.IndexOf(".");
            if (LastPing != 0)
            {
                json["ping"] = indexOf > 0 ? lastPingStr.Substring(0,indexOf) : lastPingStr;
            }

            send("PING" + JsonWriter.Serialize(json));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            RSCommandResponse toProcessResponse;
            lock (m_queuedRSCommands)
            {
                for (int i = 0; i < m_queuedRSCommands.Count; ++i)
                {
                    toProcessResponse = m_queuedRSCommands[i];

                    if (m_bIsConnected && m_connectedSuccessCallback != null && toProcessResponse.Operation == "connect")
                    {
                        m_connectedSuccessCallback(toProcessResponse.JsonMessage, m_connectedObj);
                    }
                    else if (m_bIsConnected && m_connectionFailureCallback != null &&
                        toProcessResponse.Operation == "error" || toProcessResponse.Operation == "disconnect")
                    {
                        if (toProcessResponse.Operation == "disconnect")
                            disconnect();

                        // TODO:
                        if (m_connectionFailureCallback != null)
                            m_connectionFailureCallback(400, -1, toProcessResponse.JsonMessage, m_connectedObj);
                    }

                    if (!m_bIsConnected && toProcessResponse.Operation == "connect")
                    {
                        m_bIsConnected = true;
                        send(buildConnectionRequest());
                    }

                    if (m_registeredCallbacks != null)
                        m_registeredCallbacks(toProcessResponse.JsonMessage);
                }

                m_queuedRSCommands.Clear();
            }
        }

        /// <summary>
        /// Call Ping() to get an updated LastPing value
        /// </summary>
        public long LastPing { get; private set; }
        
        #region private
        private string buildConnectionRequest()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["op"] = "CONNECT";
            json["profileId"] = m_clientRef.ProfileId;
            json["lobbyId"] = m_connectionOptions["lobbyId"] as string;
            json["passcode"] = m_connectionOptions["passcode"] as string;

            return "CONN" + JsonWriter.Serialize(json);
        }
        /// <summary>
        /// 
        /// </summary>
        private void connectWebSocket()
        {
            if (!m_bIsConnected)
            {
                startReceivingWebSocket();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void disconnect()
        {
            if (m_webSocket != null) m_webSocket.Close();

            m_webSocket = null;

            m_bIsConnected = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool send(string in_message)
        {
            bool bMessageSent = false;
            // early return
            if ((!m_useWebSocket && m_tcpClient == null) ||
                (m_useWebSocket && m_webSocket == null))
            {
                return bMessageSent;
            }

            try
            {
                //m_clientRef.Log("RS SEND: " + in_message);

                // TCP 
                if (!m_useWebSocket)
                {
                    // Get a stream object for writing. 			
                    NetworkStream stream = m_tcpClient.GetStream();

                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(in_message);

                    // Write byte array to tcpConnection stream.                 
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                    stream.Flush();
                    bMessageSent = true;
                }
                // web socket
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes(in_message);
                    m_webSocket.SendAsync(data);
                }
            }
            catch (SocketException socketException)
            {
                m_clientRef.Log("send exception: " + socketException);
                addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "error", socketException.ToString()));
            }

            return bMessageSent;
        }
        /// <summary>
        /// 
        /// </summary>
        private void startReceivingWebSocket()
        {
            bool sslEnabled = (bool)m_connectionOptions["ssl"];
            string url = (sslEnabled ? "wss://" : "ws://") + m_connectionOptions["host"] as string + ":" + (int)m_connectionOptions["port"];
            setupWebSocket(url);
        }

        private void setupWebSocket(string in_url)
        {
            m_webSocket = new BrainCloudWebSocket(in_url);
            m_webSocket.OnClose += WebSocket_OnClose;
            m_webSocket.OnOpen += Websocket_OnOpen;
            m_webSocket.OnMessage += WebSocket_OnMessage;
            m_webSocket.OnError += WebSocket_OnError;
        }

        private void WebSocket_OnClose(BrainCloudWebSocket sender, int code, string reason)
        {
            m_clientRef.Log("Connection closed: " + reason);
            addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "disconnect", reason));
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            m_clientRef.Log("Connection established.");
            addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "connect", ""));
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            onRecv(message);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            m_clientRef.Log("Error: " + message);
            addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "error", message));
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(string in_message)
        {
            string recvOpp = in_message.Substring(0, 4);
            in_message = in_message.Substring(4);
            if (recvOpp == "RSMG" || recvOpp == "RLAY" || recvOpp == "ECHO") // Room server msg or RLAY
            {
                //m_clientRef.Log("RS RECV: " + in_message);
                addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "onrecv", in_message));
            }
            else if (recvOpp == "PONG")
            {
                LastPing = DateTime.Now.Ticks - m_sentPing;
                //m_clientRef.Log("LastPing: " + (LastPing * 0.0001f).ToString() + "ms");
            }
        }

        private void addRSCommandResponse(RSCommandResponse in_command)
        {
            lock (m_queuedRSCommands)
            {
                m_queuedRSCommands.Add(in_command);
            }
        }

        private TcpClient m_tcpClient;
        private Dictionary<string, object> m_connectionOptions = null;

        private bool m_useWebSocket = false;
        private bool m_bIsConnected = false;
        private BrainCloudWebSocket m_webSocket = null;

        private const int MAX_PACKETSIZE = 1024;// TODO:: based off of some config 

        private BrainCloudClient m_clientRef;

        // success callbacks
        private SuccessCallback m_connectedSuccessCallback = null;
        private FailureCallback m_connectionFailureCallback = null;
        private object m_connectedObj = null;

        private RSCallback m_registeredCallbacks = null;
        private List<RSCommandResponse> m_queuedRSCommands = new List<RSCommandResponse>();

        private long m_sentPing = DateTime.Now.Ticks;

        private struct RSCommandResponse
        {
            public RSCommandResponse(string in_service, string in_op, string in_msg)
            {
                Service = in_service;
                Operation = in_op;
                JsonMessage = in_msg;
            }
            public string Service { get; set; }
            public string Operation { get; set; }
            public string JsonMessage { get; set; }
        }
        #endregion
    }
}

namespace BrainCloud
{
    #region public enums
    public enum eRSConnectionType
    {
        INVALID,
        WEBSOCKET,

        MAX
    }
    #endregion
}

