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
        public void EnableRTT(eRTTConnectionType in_connectionType = eRTTConnectionType.WEBSOCKET, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            if (!m_bIsConnected)
            {
                m_connectedSuccessCallback = in_success;
                m_connectionFailureCallback = in_failure;
                m_connectedObj = cb_object;

                m_useWebSocket = in_connectionType == eRTTConnectionType.WEBSOCKET;
                m_clientRef.RTTService.RequestClientConnection(rttConnectionServerSuccess, rttConnectionServerError, cb_object);
            }
        }

        /// <summary>
        /// Disables Real Time event for this session.
        /// </summary>
        public void DisableRTT()
        {
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "disconnect", "DisableRTT Called"));
            //disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterRTTCallback(ServiceName in_serviceName, RTTCallback in_callback)
        {
            m_registeredCallbacks[in_serviceName.Value] = in_callback;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DeregisterRTTCallback(ServiceName in_serviceName)
        {
            if (m_registeredCallbacks.ContainsKey(in_serviceName.Value))
            {
                m_registeredCallbacks.Remove(in_serviceName.Value);
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
            m_heartBeatTime = in_value * 1000;
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

                    // does this go to one of our registered service listeners? 
                    if (m_registeredCallbacks.ContainsKey(toProcessResponse.Service))
                    {
                        m_registeredCallbacks[toProcessResponse.Service](toProcessResponse.JsonMessage);
                    }
                    // are we actually connected? only pump this back, when the server says we've connected
                    else if (m_bIsConnected && m_connectedSuccessCallback != null && toProcessResponse.Operation == "connect")
                    {
                        m_lastNowMS = DateTime.Now;
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

                    // first time connecting? send the server connection call
                    if (!m_bIsConnected && toProcessResponse.Operation == "connect")
                    {
                        m_bIsConnected = true;
                        send(buildConnectionRequest());
                    }
                }

                m_queuedRTTCommands.Clear();
            }

            //////
            if (m_bIsConnected)
            {
                DateTime nowMS = DateTime.Now;
                // the heart beat
                m_timeSinceLastRequest += (nowMS - m_lastNowMS).Milliseconds;
                m_lastNowMS = nowMS;

                if (m_timeSinceLastRequest >= m_heartBeatTime)
                {
                    m_timeSinceLastRequest = 0;
                    send(buildHeartbeatRequest());
                }
            }
            //////
        }

        #region private
        /// <summary>
        /// 
        /// </summary>
        private void connect()
        {
            if (!m_bIsConnected)
            {
                try
                {
                    m_tcpClient = new TcpClient(m_endpoint["host"] as string, (int)m_endpoint["port"]);
                    // Start an asynchronous read invoking DoRead to avoid lagging the user
                    // interface.
                    m_tcpClient.GetStream().BeginRead(m_readBuffer, 0, MAX_PACKETSIZE, new AsyncCallback(DoRead), null);
                    m_bIsConnected = true;
                    send(buildConnectionRequest());
                }
                catch (Exception e)
                {
                    m_clientRef.Log("On client connect exception " + e);
                    addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "error", e.ToString()));
                }
            }
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
            if (m_tcpClient != null) m_tcpClient.Close();
            if (m_webSocket != null) m_webSocket.Close();

            RTTConnectionID = "";
            RTTEventServer = "";

            m_tcpClient = null;
            m_webSocket = null;

            m_bIsConnected = false;
        }

        private string buildConnectionRequest()
        {
            Dictionary<string, object> system = new Dictionary<string, object>();
            system["platform"] = m_clientRef.ReleasePlatform;
            system["protocol"] = m_useWebSocket ? "ws" : "tcp";

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

            return JsonWriter.Serialize(json);
        }

        private string buildHeartbeatRequest()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["service"] = ServiceName.RTT.Value;
            json["operation"] = "HEARTBEAT";
            json["data"] = null;

            return JsonWriter.Serialize(json);
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

            m_timeSinceLastRequest = 0;
            try
            {
                m_clientRef.Log("RTT SEND: " + in_message);

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
                addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "error", socketException.ToString()));
            }

            return bMessageSent;
        }

        private void DoRead(IAsyncResult ar)
        {
            int BytesRead;
            try
            {
                // Finish asynchronous read into readBuffer and return number of bytes read.
                BytesRead = m_tcpClient.GetStream().EndRead(ar);
                if (BytesRead < 1)
                {
                    // if no bytes were read server has close. 
                    addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "disconnect", "no bytes from server"));
                    return;
                }
                onRecv(Encoding.ASCII.GetString(m_readBuffer, 0, BytesRead - 2));
                // Start a new asynchronous read into readBuffer.
                m_tcpClient.GetStream().BeginRead(m_readBuffer, 0, MAX_PACKETSIZE, new AsyncCallback(DoRead), null);

            }
            catch (Exception e)
            {
                addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "disconnect", e.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void startReceiving()
        {
            try
            {
                byte[] sizeBuffer = new byte[MAX_PACKETSIZE];
                int size;
                byte[] buffer;
                while (true)
                {
                    if (m_tcpClient == null) continue;
                    if (m_tcpClient.Client.Connected && !m_bIsConnected)
                    {
                        m_bIsConnected = true;
                        send(buildConnectionRequest());
                    }
                    // Get a stream object for reading 				
                    NetworkStream stream = m_tcpClient.GetStream();
                    if (stream.DataAvailable)
                    {
                        // get the message size 
                        stream.Read(sizeBuffer, 0, MAX_PACKETSIZE);
                        size = BitConverter.ToInt32(sizeBuffer, 0);
                        // give the buffer its correct size.
                        buffer = new byte[size];
                        stream.Read(buffer, 0, size);

                        string serverMessage = Encoding.ASCII.GetString(buffer);
                        onRecv(serverMessage);
                    }
                    Thread.Sleep(1);
                }
            }
            catch (SocketException socketException)
            {
                m_clientRef.Log("Socket exception: " + socketException);
                addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "error", socketException.ToString()));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void startReceivingWebSocket()
        {
            bool sslEnabled = (bool)m_endpoint["ssl"];
            string url = (sslEnabled ? "wss://" : "ws://") + m_endpoint["host"] as string + ":" + (int)m_endpoint["port"] + getUrlQueryParameters();
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
            m_clientRef.Log("Connection closed: " + reason);
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "disconnect", reason));
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            m_clientRef.Log("Connection established.");
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "connect", ""));
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            string message = Encoding.UTF8.GetString(data);
            onRecv(message);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            m_clientRef.Log("Error: " + message);
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "error", message));
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(string in_message)
        {
            m_timeSinceLastRequest = 0;
            m_clientRef.Log("RTT RECV: " + in_message);
            Dictionary<string, object> response = (Dictionary<string, object>)JsonReader.Deserialize(in_message);

            string service = (string)response["service"];
            string operation = (string)response["operation"];

            Dictionary<string, object> data = (Dictionary<string, object>)response["data"];
            if (operation == "CONNECT")
            {
                int heartBeat = m_heartBeatTime / 1000;
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

            if (data != null)
            {
                if (data.ContainsKey("cxId")) RTTConnectionID = (string)data["cxId"];
                if (data.ContainsKey("evs")) RTTEventServer = (string)data["evs"];
            }

            if (operation != "HEARTBEAT")
                addRTTCommandResponse(new RTTCommandResponse(service.ToLower(), operation.ToLower(), in_message));
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

            if (m_useWebSocket)
            {
                //   1st choice: websocket + ssl
                //   2nd: websocket
                m_endpoint = getEndpointForType(endpoints, "ws", true);
                if (m_endpoint == null)
                {
                    m_endpoint = getEndpointForType(endpoints, "ws", false);
                }

                connectWebSocket();
            }
            else
            {
                //   1st choice: tcp
                //   2nd: tcp + ssl (not implemented yet)
                m_endpoint = getEndpointForType(endpoints, "tcp", false);
                if (m_endpoint == null)
                {
                    m_endpoint = getEndpointForType(endpoints, "tcp", true);
                }

                connect();
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
            // TODO::
            m_bIsConnected = false;
            m_clientRef.Log("RTT Connection Server Error: \n" + jsonError);
            addRTTCommandResponse(new RTTCommandResponse(ServiceName.RTTRegistration.Value, "error", jsonError));
        }

        private void addRTTCommandResponse(RTTCommandResponse in_command)
        {
            lock (m_queuedRTTCommands)
            {
                m_queuedRTTCommands.Add(in_command);
            }
        }

        private TcpClient m_tcpClient;
        private Dictionary<string, object> m_endpoint = null;

        private byte[] m_readBuffer = new byte[MAX_PACKETSIZE];

        private bool m_useWebSocket = false;
        private bool m_bIsConnected = false;
        private BrainCloudWebSocket m_webSocket = null;

        private DateTime m_lastNowMS;

        private int m_timeSinceLastRequest = 0;
        private const int MAX_PACKETSIZE = 1024;// TODO:: based off of some config 
        private int m_heartBeatTime = 10 * 1000;

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
        #endregion
    }
}

namespace BrainCloud
{
    #region public enums
    public enum eRTTConnectionType
    {
        INVALID,
        WEBSOCKET,
        TCP,

        MAX
    }
    #endregion
}

