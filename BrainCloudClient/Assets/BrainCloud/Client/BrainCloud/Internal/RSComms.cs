//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
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
                m_currentConnectionType = in_connectionType;
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
            m_registeredCallback = in_callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterCallback()
        {
            m_registeredCallback = null;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public void RegisterDataCallback(RSDataCallback in_callback)
        {
            m_registeredDataCallback = in_callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterDataCallback()
        {
            m_registeredDataCallback = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Send(string in_message)
        {
            send("RLAY" + in_message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Send(byte[] in_data, string in_header = "RLAY")
        {
            // appened RLAY to the beginning
            byte[] destination = concatenateByteArrays(Encoding.ASCII.GetBytes(in_header), in_data);
            send(destination);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Echo(string in_message)
        {
            send("ECHO" + in_message);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Ping()
        {
            m_sentPing = DateTime.Now.Ticks;
            short lastPingShort = Convert.ToInt16(LastPing * 0.0001);
            byte data1, data2;
            fromShort(lastPingShort, out data1, out data2);

            byte[] dataArr = { data1, data2 }; 

            Send(dataArr, "PING");
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

                    if (m_registeredCallback != null)
                        m_registeredCallback(toProcessResponse.JsonMessage);

                    if (m_registeredDataCallback != null && toProcessResponse.RawData != null)
                        m_registeredDataCallback(toProcessResponse.RawData);
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
            m_clientRef.Log("RS SEND: " + in_message);
            byte[] data = Encoding.ASCII.GetBytes(in_message);
            return send(data);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool send(byte[] in_data)
        {
            bool bMessageSent = false;
            bool isSocket = m_currentConnectionType == eRSConnectionType.WEBSOCKET;
            // early return
            if ((isSocket && m_webSocket == null))
            {
                return bMessageSent;
            }

            try
            {
                // WEBSOCKET 
                if (isSocket)
                {
                    //m_clientRef.Log("RS SEND Bytes : " + in_data.Length);
                    m_webSocket.SendAsync(in_data);
                }
                bMessageSent = true;
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
            onRecv(data);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            m_clientRef.Log("Error: " + message);
            addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "error", message));
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(byte[] in_data)
        {
            if (in_data.Length >= 4)
            {
                string in_message = Encoding.ASCII.GetString(in_data);
                string recvOpp = in_message.Substring(0, 4);
                in_message = in_message.Substring(4);

                if (recvOpp == "RSMG" || recvOpp == "RLAY" || recvOpp == "ECHO") // Room server msg or RLAY
                {
                    // bytes after the headers removed
                    in_data = Encoding.ASCII.GetBytes(in_message);

                    //m_clientRef.Log("RS RECV: " + in_message);
                    addRSCommandResponse(new RSCommandResponse(ServiceName.RoomServer.Value, "onrecv", in_message, in_message[0] != '{'  && in_message[0] != 'o' ? in_data : null));
                }
                else if (recvOpp == "PONG")
                {
                    LastPing = DateTime.Now.Ticks - m_sentPing;
                    //m_clientRef.Log("LastPing: " + (LastPing * 0.0001f).ToString() + "ms");
                }
            }
        }

        private void addRSCommandResponse(RSCommandResponse in_command)
        {
            lock (m_queuedRSCommands)
            {
                m_queuedRSCommands.Add(in_command);
            }
        }
        
        private byte[] concatenateByteArrays(byte[] a, byte[] b)
        {
            byte[] rv = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, rv, 0, a.Length);
            Buffer.BlockCopy(b, 0, rv, a.Length, b.Length);
            return rv;
        }

        private void fromShort(short number, out byte byte1, out byte byte2)
        {
            byte2 = (byte)(number >> 8);
            byte1 = (byte)(number >> 0);
        }

        private Dictionary<string, object> m_connectionOptions = null;

        private eRSConnectionType m_currentConnectionType = eRSConnectionType.INVALID;
        private bool m_bIsConnected = false;
        private BrainCloudWebSocket m_webSocket = null;

        private const int MAX_PACKETSIZE = 1024;// TODO:: based off of some config 

        private BrainCloudClient m_clientRef;

        // success callbacks
        private SuccessCallback m_connectedSuccessCallback = null;
        private FailureCallback m_connectionFailureCallback = null;
        private object m_connectedObj = null;

        private RSCallback m_registeredCallback = null;
        private RSDataCallback m_registeredDataCallback = null;

        private List<RSCommandResponse> m_queuedRSCommands = new List<RSCommandResponse>();

        private long m_sentPing = DateTime.Now.Ticks;

        private struct RSCommandResponse
        {
            public RSCommandResponse(string in_service, string in_op, string in_msg, byte[] in_data = null)
            {
                Service = in_service;
                Operation = in_op;
                JsonMessage = in_msg;
                RawData = in_data;
            }
            public string Service { get; set; }
            public string Operation { get; set; }
            public string JsonMessage { get; set; }
            public byte[] RawData { get; set; }
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

