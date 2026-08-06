// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using BrainCloud.JsonFx.Json;

    internal sealed class RTTComms
    {
        /// <summary>
        /// 
        /// </summary>
        public RTTComms(BrainCloudClient in_client)
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
        public void EnableRTT(SuccessCallback in_success, FailureCallback in_failure, RTTConnectionType in_connectionType = RTTConnectionType.WEBSOCKET, object cb_object = null)
        {
            m_disconnectedWithReason = false;

            if(IsRTTEnabled() || m_rttConnectionStatus == RTTConnectionStatus.CONNECTING)
            {
                return;
            }
            if(!m_clientRef.Authenticated || m_clientRef.Comms.KillSwitchEngaged)
            {
                if(m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("RTT: EnableRTT called before calling authentication request. Disabling RTT.");
                }
                if(in_failure != null)
                {
                    in_failure(StatusCodes.FORBIDDEN, ReasonCodes.RTT_NO_API_SESSION_ERROR, "RTT: EnableRTT called before calling authentication request. Disabling RTT.", cb_object);
                }
            }
            else
            {
                m_connectedSuccessCallback = in_success;
                m_connectionFailureCallback = in_failure;
                m_connectedObj = cb_object;

                m_currentConnectionType = in_connectionType;
                m_clientRef.RTTService.RequestClientConnection(rttConnectionServerSuccess, rttConnectionServerError, cb_object);
            }
        }

        /// <summary>
        /// Disables Real Time event for this session.
        /// </summary>
        public void DisableRTT()
        {
            if (!IsRTTEnabled() || m_rttConnectionStatus == RTTConnectionStatus.DISCONNECTING)
            {
                return;
            }
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "disconnect", "DisableRTT Called"));
        }

        /// <summary>
        /// Returns true if RTT is enabled
        /// </summary>
        public bool IsRTTEnabled()
        {
            return m_rttConnectionStatus == RTTConnectionStatus.CONNECTED;
        }

        ///<summary>
        ///Returns the status of the connection
        ///</summary>
        public RTTConnectionStatus GetConnectionStatus()
        {
            return m_rttConnectionStatus;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTCallback(ServiceName in_serviceName, RTTCallback in_callback)
        {
            m_registeredCallbacks[in_serviceName.Value.ToLower()] = in_callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTCallback(ServiceName in_serviceName)
        {
            string toCheck = in_serviceName.Value.ToLower();
            if (m_registeredCallbacks.ContainsKey(toCheck))
            {
                m_registeredCallbacks.Remove(toCheck);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterAllRTTCallbacks()
        {
            m_registeredCallbacks.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetRTTHeartBeatSeconds(int in_value)
        {
            m_heartBeatTime = TimeSpan.FromMilliseconds(in_value * 1000);
        }

        public string RTTConnectionID { get; private set; }
        public string RTTEventServer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            RTTCommandResponse toProcessResponse;
            lock (m_queuedRTTCommands)
            {
                for (int i = 0; i < m_queuedRTTCommands.Count; ++i)
                {
                    toProcessResponse = m_queuedRTTCommands[i];
 
                    // Socket closed unexpectedly — tear down and report failure.
                    // Covers WebSocket (CLOSED status) and TCP (m_tcpDisconnected flag).
                    if (m_webSocketStatus == WebsocketStatus.CLOSED || m_tcpDisconnected)
                    {
                        m_rttConnectionStatus = RTTConnectionStatus.DISCONNECTING;
                        if (m_connectionFailureCallback != null)
                            m_connectionFailureCallback(400, -1, toProcessResponse.JsonMessage, m_connectedObj);
                        disconnect();
                        break;
                    }

                    // does this go to one of our registered service listeners? 
                    if (m_registeredCallbacks.ContainsKey(toProcessResponse.Service))
                    {
                        m_registeredCallbacks[toProcessResponse.Service](toProcessResponse.JsonMessage);
                    }

                    // are we actually connected? only pump this back, when the server says we've connected
                    else if (m_rttConnectionStatus == RTTConnectionStatus.CONNECTING && m_connectedSuccessCallback != null && toProcessResponse.Operation == "connect")
                    {
                        m_sinceLastHeartbeat = DateTime.Now.TimeOfDay;
                        m_rttConnectionStatus = RTTConnectionStatus.CONNECTED;
                        m_connectedSuccessCallback(toProcessResponse.JsonMessage, m_connectedObj);
                    }

                    //if we're connected and we get a disconnect - we disconnect the comms... 
                    else if (m_rttConnectionStatus == RTTConnectionStatus.CONNECTED && toProcessResponse.Operation == "disconnect")
                    {
                        m_rttConnectionStatus = RTTConnectionStatus.DISCONNECTING;
                        disconnect();
                    }

                    //If there's an error, we send back the error
                    else if (m_connectionFailureCallback != null && toProcessResponse.Operation == "error")
                    {
                        if(toProcessResponse.JsonMessage != null)
                        {   
                            Dictionary<string, object> messageData = (Dictionary<string, object>)JsonReader.Deserialize(toProcessResponse.JsonMessage);
                            if(messageData.ContainsKey("status") && messageData.ContainsKey("reason_code"))
                            {
                                m_connectionFailureCallback((int)messageData["status"], (int)messageData["reason_code"], toProcessResponse.JsonMessage, m_connectedObj);
                            }
                            else
                            {
                                //in the rare case the message is differently structured. 
                                m_connectionFailureCallback(400, -1, toProcessResponse.JsonMessage, m_connectedObj);
                            }
                        }
                        else
                        {
                            m_connectionFailureCallback(400, -1, "Error - No Response from Server", m_connectedObj);
                        }
                    }

                    //if we're not connected and we're trying to connect, then start the connection
                    else if (m_rttConnectionStatus == RTTConnectionStatus.DISCONNECTED && toProcessResponse.Operation == "connect")
                    {
                        // Socket is open — transition to CONNECTING and send the CONNECT handshake.
                        // Protocol must match what was actually connected ("ws" or "tcp").
                        m_rttConnectionStatus = RTTConnectionStatus.CONNECTING;
                        string protocol = m_currentConnectionType == RTTConnectionType.TCP ? "tcp" : "ws";
                        send(buildConnectionRequest(protocol));
                    }
                    else
                    {
                        if (m_clientRef.LoggingEnabled)
                        {
                            m_clientRef.Log("WARNING no handler registered for RTT callbacks ");
                        }
                    }

                }

                m_queuedRTTCommands.Clear();
            }

            if (m_rttConnectionStatus == RTTConnectionStatus.CONNECTED)
            {
                if ((DateTime.Now.TimeOfDay - m_sinceLastHeartbeat) >= m_heartBeatTime)
                {
                    m_sinceLastHeartbeat = DateTime.Now.TimeOfDay;
                    send(buildHeartbeatRequest(), true);
                }
            }
        }

        #region private
        /// <summary>
        /// 
        /// </summary>
        private void connectWebSocket()
        {
            if (m_rttConnectionStatus == RTTConnectionStatus.DISCONNECTED)
            {
                startReceivingWebSocket();
            }
        }

        private void connectTCP()
        {
            if (m_rttConnectionStatus != RTTConnectionStatus.DISCONNECTED) return;

            m_tcpIsDisconnecting = false;

            string host = m_endpoint["host"] as string;
            int port = (int)m_endpoint["port"];

            m_tcpReceiveThread = new Thread(() =>
            {
                try
                {
                    if (m_clientRef.LoggingEnabled)
                        m_clientRef.Log("RTT TCP: Connecting to " + host + ":" + port + "...");

                    m_tcpClient = new TcpClient();
                    m_tcpClient.NoDelay = true;
                    m_tcpClient.Connect(host, port);
                    m_tcpStream = m_tcpClient.GetStream();

                    if (m_clientRef.LoggingEnabled)
                        m_clientRef.Log("RTT TCP: Connected.");

                    // Signal socket open — Update() will transition DISCONNECTED→CONNECTING
                    // and send the CONNECT handshake (same two-step as WebSocket OnOpen).
                    addRTTCommandResponse(new RTTCommandResponse(
                        ServiceName.RTTRegistration.Value.ToLower(), "connect", ""));

                    // Receive loop: 4-byte big-endian length prefix + UTF-8 payload.
                    byte[] lenBuf = new byte[4];
                    while (m_tcpClient != null && m_tcpStream != null)
                    {
                        if (!readFully(m_tcpStream, lenBuf, 4)) break;
                        if (BitConverter.IsLittleEndian) Array.Reverse(lenBuf);
                        int msgLen = BitConverter.ToInt32(lenBuf, 0);

                        byte[] msgBuf = new byte[msgLen];
                        if (!readFully(m_tcpStream, msgBuf, msgLen)) break;

                        onRecv(Encoding.UTF8.GetString(msgBuf));
                    }
                }
                catch (Exception e)
                {
                    // Suppress the IOException/SocketException (WSACancelBlockingCall / WSAEINTR)
                    // that fires when disconnect() closes the socket from the main thread while
                    // we are blocking in NetworkStream.Read() — that is expected clean shutdown.
                    if (!m_tcpIsDisconnecting && m_clientRef.LoggingEnabled)
                        m_clientRef.Log("RTT TCP error: " + e);
                }

                // Only signal Update() when the socket closed unexpectedly; disconnect() already
                // handles intentional teardown and resets m_tcpDisconnected itself.
                if (!m_tcpIsDisconnecting)
                {
                    m_tcpDisconnected = true;
                    addRTTCommandResponse(new RTTCommandResponse(
                        ServiceName.RTTRegistration.Value.ToLower(), "disconnect", "RTT TCP connection closed"));
                }
            });
            m_tcpReceiveThread.IsBackground = true;
            m_tcpReceiveThread.Start();
        }

        // Reads exactly `count` bytes from `stream` into `buf`, blocking until done.
        // Returns false if the stream closes before all bytes are read.
        private static bool readFully(NetworkStream stream, byte[] buf, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int n = stream.Read(buf, offset, count - offset);
                if (n == 0) return false;
                offset += n;
            }
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        private void disconnect()
        {
            if (m_webSocket != null) m_webSocket.Close();

            // TCP cleanup — set the flag first so the receive thread's catch block knows
            // that the IOException is intentional and should not be logged as an error.
            m_tcpIsDisconnecting = true;
            m_tcpStream?.Dispose();
            m_tcpStream = null;
            if (m_tcpClient != null)
            {
                m_tcpClient.Close();
                m_tcpClient = null;
            }
            m_tcpDisconnected = false;
            // m_tcpIsDisconnecting is NOT reset here — the background receive thread may
            // still be in its catch block. It is reset at the top of connectTCP() instead,
            // before any new connection is started.

            RTTConnectionID = "";
            RTTEventServer = "";

            m_webSocket = null;

            if (m_disconnectedWithReason == true)
            {
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("RTT: Disconnect: " + m_clientRef.SerializeJson(m_disconnectJson));
                }
                if (m_connectionFailureCallback != null)
                {
                    m_connectionFailureCallback(400, (int)m_disconnectJson["reason_code"], (string)m_disconnectJson["reason"], m_connectedObj);
                }
            }
            m_rttConnectionStatus = RTTConnectionStatus.DISCONNECTED;
        }

        private string buildConnectionRequest(string protocol = "ws")
        {
            Dictionary<string, object> system = new Dictionary<string, object>();
            system["platform"] = m_clientRef.ReleasePlatform.ToString();
            system["protocol"] = protocol;

            Dictionary<string, object> jsonData = new Dictionary<string, object>();
            jsonData["appId"] = m_clientRef.AppId;
            jsonData["sessionId"] = m_clientRef.SessionID;
            jsonData["profileId"] = m_clientRef.ProfileId;
            jsonData["system"] = system;

            jsonData["auth"] = m_rttHeaders;

            Dictionary<string, object> json = new Dictionary<string, object>();
            json["service"] = ServiceName.RTT.Value;
            json["operation"] = "CONNECT";
            json["data"] = jsonData;

            return m_clientRef.SerializeJson(json);
        }

        private string buildHeartbeatRequest()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["service"] = ServiceName.RTT.Value;
            json["operation"] = "HEARTBEAT";
            json["data"] = null;

            return m_clientRef.SerializeJson(json);
        }

        /// <summary>
        /// 
        /// </summary>
        private bool send(string in_message, bool in_bLogMessage = true)
        {
            bool bMessageSent = false;
            bool useWebSocket = m_currentConnectionType == RTTConnectionType.WEBSOCKET;

            if (useWebSocket && m_webSocket == null) return bMessageSent;
            if (!useWebSocket && m_tcpStream == null) return bMessageSent;

            try
            {
                if (in_bLogMessage && m_clientRef.LoggingEnabled)
                    m_clientRef.Log("RTT SEND: " + in_message);

                if (useWebSocket)
                {
                    byte[] data = Encoding.ASCII.GetBytes(in_message);
                    m_webSocket.SendAsync(data);
                    bMessageSent = true;
                }
                else // TCP: 4-byte big-endian length prefix + UTF-8 payload
                {
                    byte[] msgBytes = Encoding.UTF8.GetBytes(in_message);
                    byte[] lenBytes = BitConverter.GetBytes(msgBytes.Length);
                    if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
                    lock (m_tcpSendLock)
                    {
                        m_tcpStream.Write(lenBytes, 0, 4);
                        m_tcpStream.Write(msgBytes, 0, msgBytes.Length);
                        m_tcpStream.Flush();
                    }
                    bMessageSent = true;
                }
            }
            catch (Exception socketException)
            {
                if (m_clientRef.LoggingEnabled)
                    m_clientRef.Log("send exception: " + socketException);
                addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "error", buildRTTRequestError(socketException.ToString())));
            }

            return bMessageSent;
        }

        /// <summary>
        /// 
        /// </summary>
        private void startReceivingWebSocket()
        {
            bool sslEnabled = (bool)m_endpoint["ssl"];
            string url = (sslEnabled ? "wss://" : "ws://") + (m_endpoint["host"] as string) + ":" + (int)m_endpoint["port"] + getUrlQueryParameters();
            setupWebSocket(url);
        }

        private string getUrlQueryParameters()
        {
            string sToReturn = "?";
            int count = 0;
            foreach (KeyValuePair<string, object> item in m_rttHeaders)
            {
                if (count > 0) sToReturn += "&";
                sToReturn += item.Key + "=" + item.Value;
                ++count;
            }

            return sToReturn;
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
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RTT: Connection closed: " + reason);
            }
            m_webSocketStatus = WebsocketStatus.CLOSED;
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "disconnect", reason));
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RTT: Connection established.");
            }
            m_webSocketStatus = WebsocketStatus.OPEN;
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "connect", ""));
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            if (data.Length == 0) return;
            m_webSocketStatus = WebsocketStatus.MESSAGE;
            string message = Encoding.UTF8.GetString(data);
            onRecv(message);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RTT Error: " + message);
            }
            m_webSocketStatus = WebsocketStatus.ERROR;
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "error", buildRTTRequestError(message)));
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(string in_message)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RTT RECV: " + in_message);
            }

            Dictionary<string, object> response = (Dictionary<string, object>)JsonReader.Deserialize(in_message);

            string service = (string)response["service"];
            string operation = (string)response["operation"];

            Dictionary<string, object> data = null;
            if (response.ContainsKey("data"))
                data = (Dictionary<string, object>)response["data"];
            if (operation == "CONNECT")
            {
                int heartBeat = m_heartBeatTime.Milliseconds / 1000;
                try
                {
                    heartBeat = (int)data["heartbeatSeconds"];
                }
                catch (Exception)
                {
                    heartBeat = (int)data["wsHeartbeatSecs"];
                }

                SetRTTHeartBeatSeconds(heartBeat);
            }
            else if (operation == "DISCONNECT")
            {
                m_disconnectedWithReason = true;
                m_disconnectJson["reason_code"] = (int)data["reasonCode"];
                m_disconnectJson["reason"] = (string)data["reason"];
                m_disconnectJson["severity"] = "ERROR";
            }

            if (data != null)
            {
                if (data.ContainsKey("cxId")) RTTConnectionID = (string)data["cxId"];
                if (data.ContainsKey("evs")) RTTEventServer = (string)data["evs"];
            }

            if (operation != "HEARTBEAT")
            {
                addRTTCommandResponse(new RTTCommandResponse(service.ToLower(), operation.ToLower(), in_message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void rttConnectionServerSuccess(string jsonResponse, object cbObject)
        {
            Dictionary<string, object> jsonMessage = (Dictionary<string, object>)JsonReader.Deserialize(jsonResponse);
            Dictionary<string, object> jsonData = (Dictionary<string, object>)jsonMessage["data"];
            Array endpoints = (Array)jsonData["endpoints"];
            m_rttHeaders = (Dictionary<string, object>)jsonData["auth"];

            if (m_currentConnectionType == RTTConnectionType.WEBSOCKET)
            {
                //   1st choice: websocket + ssl
                //   2nd: websocket
                m_endpoint = getEndpointForType(endpoints, "ws", true);
                if (m_endpoint == null)
                    m_endpoint = getEndpointForType(endpoints, "ws", false);

                connectWebSocket();
            }
            else if (m_currentConnectionType == RTTConnectionType.TCP)
            {
                //   1st choice: tcp (no ssl)
                //   2nd: tcp + ssl (not yet fully implemented server-side)
                m_endpoint = getEndpointForType(endpoints, "tcp", false);
                if (m_endpoint == null)
                    m_endpoint = getEndpointForType(endpoints, "tcp", true);

                if (m_endpoint == null)
                {
                    rttConnectionServerError(400, ReasonCodes.RTT_CLIENT_ERROR,
                        buildRTTRequestError("No TCP endpoint available"), cbObject);
                    return;
                }

                connectTCP();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, object> getEndpointForType(Array endpoints, string type, bool in_bWantSsl)
        {
            Dictionary<string, object> toReturn = null;
            Dictionary<string, object> tempToReturn = null;
            for (int i = 0; i < endpoints.Length; ++i)
            {
                tempToReturn = endpoints.GetValue(i) as Dictionary<string, object>;
                if (tempToReturn["protocol"] as string == type)
                {
                    if (in_bWantSsl)
                    {
                        if ((bool)tempToReturn["ssl"])
                        {
                            toReturn = tempToReturn;
                            break;
                        }
                    }
                    else
                    {
                        toReturn = tempToReturn;
                        break;
                    }
                }
            }

            return toReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        private void rttConnectionServerError(int status, int reasonCode, string jsonError, object cbObject)
        {
            m_rttConnectionStatus = RTTConnectionStatus.DISCONNECTED;
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RTT Connection Server Error: \n" + jsonError);
            }
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value.ToLower(), "error", jsonError));
        }

        private void addRTTCommandResponse(RTTCommandResponse in_command)
        {
            lock (m_queuedRTTCommands)
            {
                m_queuedRTTCommands.Add(in_command);
            }
        }

        private string buildRTTRequestError(string in_statusMessage)
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["status"] = 403;
            json["reason_code"] = ReasonCodes.RTT_CLIENT_ERROR;
            json["status_message"] = in_statusMessage;
            json["severity"] = "ERROR";

            return m_clientRef.SerializeJson(json);
        }

        private bool m_disconnectedWithReason = false;
        private Dictionary<string, object> m_disconnectJson = new Dictionary<string, object>();

        private Dictionary<string, object> m_endpoint = null;
        private RTTConnectionType m_currentConnectionType = RTTConnectionType.INVALID;
        private BrainCloudWebSocket m_webSocket = null;

        // TCP connection state
        private TcpClient m_tcpClient = null;
        private NetworkStream m_tcpStream = null;
        private Thread m_tcpReceiveThread = null;
        private volatile bool m_tcpDisconnected = false;
        private volatile bool m_tcpIsDisconnecting = false;
        private readonly object m_tcpSendLock = new object();

        private TimeSpan m_sinceLastHeartbeat;
        private const int MAX_PACKETSIZE = 1024;
        private TimeSpan m_heartBeatTime = TimeSpan.FromMilliseconds(10 * 1000);

        private BrainCloudClient m_clientRef;

        // success callbacks
        private SuccessCallback m_connectedSuccessCallback = null;
        private FailureCallback m_connectionFailureCallback = null;
        private object m_connectedObj = null;

        private Dictionary<string, object> m_rttHeaders = new Dictionary<string, object>();
        private Dictionary<string, RTTCallback> m_registeredCallbacks = new Dictionary<string, RTTCallback>();
        private List<RTTCommandResponse> m_queuedRTTCommands = new List<RTTCommandResponse>();

        private struct RTTCommandResponse
        {
            public RTTCommandResponse(string in_service, string in_op, string in_msg)
            {
                Service = in_service;
                Operation = in_op;
                JsonMessage = in_msg;
            }
            public string Service { get; set; }
            public string Operation { get; set; }
            public string JsonMessage { get; set; }
        }

        private WebsocketStatus m_webSocketStatus = WebsocketStatus.NONE;

        private RTTConnectionStatus m_rttConnectionStatus = RTTConnectionStatus.DISCONNECTED;

        #endregion
    }
}

namespace BrainCloud
{
    #region public enums
    public enum WebsocketStatus
    {
        OPEN,
        CLOSED,
        MESSAGE, 
        ERROR,
        NONE
    }

    public enum RTTConnectionStatus
    {
        CONNECTED,
        DISCONNECTED,
        CONNECTING,
        DISCONNECTING
    }

    public enum RTTConnectionType
    {
        INVALID,
        WEBSOCKET,
        TCP,
        MAX
    }
    #endregion
}

