//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

// Uncomment this to have packet log
#define BC_DEBUG_RELAY_LOGS_ENABLED

namespace BrainCloud.Internal
{
    using JsonFx.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using System.Diagnostics;

    internal sealed class RelayComms
    {
        #region public consts
        public const int MAX_PACKETSIZE = 1024;

        public const byte MAX_PLAYERS = 40;
        public const byte INVALID_NET_ID = MAX_PLAYERS;

        public const byte CL2RS_CONNECT = 0;
        public const byte CL2RS_DISCONNECT = 1;
        public const byte CL2RS_RELAY = 2;
        public const byte CL2RS_ACK = 3;
        public const byte CL2RS_PING = 4;
        public const byte CL2RS_RSMG_ACK = 5;

        public const byte RS2CL_RSMG = 0;
        public const byte RS2CL_DISCONNECT = 1;
        public const byte RS2CL_RELAY = 2;
        public const byte RS2CL_ACK = 3;
        public const byte RS2CL_PONG = 4;
        #endregion

        private const int MAX_RSMG_HISTORY = 50;

        /// <summary>
        /// Last Synced Ping
        /// </summary>
        public long Ping { get; private set; }

        /// <summary>
        /// Room Server Comms Constructor
        /// </summary>
        public RelayComms(BrainCloudClient in_client)
        {
            m_clientRef = in_client;
        }

        /// <summary>
        /// Start off a connection, based off connection type to brainClouds Relay Servers.  Connect options come in from "ROOM_ASSIGNED" lobby callback
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_options">
        ///             connectionOptions["ssl"] = false;
        ///             connectionOptions["host"] = "168.0.1.192";
        ///             connectionOptions["port"] = 9000;
        ///             connectionOptions["passcode"] = ""somePasscode"
        ///             connectionOptions["lobbyId"] = "55555:v5v:001";
        ///</param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(RelayConnectionType in_connectionType, RelayConnectOptions in_options, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
#if UNITY_WEBGL
            if (in_connectionType != RelayConnectionType.WEBSOCKET)
            {
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("Non-WebSocket Connection Type Requested, on WEBGL.  Please connect via WebSocket.");
                }

                if (in_failure != null)
                    in_failure(403, ReasonCodes.CLIENT_NETWORK_ERROR_TIMEOUT, buildRSRequestError("Non-WebSocket Connection Type Requested, on WEBGL.  Please connect via WebSocket."), cb_object);
                return;
            }
#endif
            Ping = 999;
            if (!IsConnected())
            {
                // the callback
                m_connectOptions = in_options;
                m_connectedSuccessCallback = in_success;
                m_connectionFailureCallback = in_failure;
                m_connectedObj = cb_object;
                // connection type
                m_connectionType = in_connectionType;
                // now connect
                startReceivingRSConnectionAsync();
            }
        }

        /// <summary>
        /// Disables relay event for this session.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected()) send(buildDisconnectRequest());
            disconnect();
        }

        /// <summary>
        /// IsConnected
        /// </summary>
        public bool IsConnected()
        {
            return m_bIsConnected;
        }

        public void RegisterRelayCallback(RelayCallback in_callback)
        {
            m_registeredRelayCallback = in_callback;
        }

        public void DeregisterRelayCallback()
        {
            m_registeredRelayCallback = null;
        }

        public void RegisterSystemCallback(RelaySystemCallback in_callback)
        {
            m_registeredSystemCallback = in_callback;
        }

        public void DeregisterSystemCallback()
        {
            m_registeredSystemCallback = null;
        }

        public void QueueError(string message)
        {
            queueErrorEvent(message);
        }

        public void Send(byte[] in_data, ulong in_playerMask, bool in_reliable, bool in_ordered, int in_channel)
        {
            if (!IsConnected()) return;
            if (in_data.Length > MAX_PACKETSIZE)
            {
                disconnect();
                queueErrorEvent("Packet too big: " + in_data.Length.ToString() + " > max " + MAX_PACKETSIZE.ToString());
                return;
            }

            // NetId
            byte[] controlByteHeader = { CL2RS_RELAY };

            // Reliable header
            ushort rh = 0;
            if (in_reliable) rh |= RELIABLE_BIT;
            if (in_ordered) rh |= ORDERED_BIT;
            rh |= (ushort)((in_channel << 12) & 0x3000);

            // Store inverted player mask
            ulong playerMask = 0;
            for (int i = 0, len = (int)MAX_PLAYERS; i < len; ++i)
            {
                playerMask |= ((in_playerMask >> (MAX_PLAYERS - i - 1)) & 1) << i;
            }
            playerMask = (playerMask << 8) & 0x0000FFFFFFFFFF00;

            // AckId without packet id
            ulong ackIdWithoutPacketId = (((ulong)rh << 48) & 0xFFFF000000000000) | playerMask;

            // Packet Id
            int packetId = 0;
            if (m_sendPacketId.ContainsKey(ackIdWithoutPacketId))
            {
                packetId = m_sendPacketId[ackIdWithoutPacketId];
            }
            m_sendPacketId[ackIdWithoutPacketId] = (packetId + 1) & MAX_PACKET_ID;

            // Add packet id to the header, then encode
            rh |= (ushort)packetId;

            ushort playerMask0 = (ushort)((playerMask >> 32) & 0xFFFF);
            ushort playerMask1 = (ushort)((playerMask >> 16) & 0xFFFF);
            ushort playerMask2 = (ushort)((playerMask) & 0xFFFF);

            byte header0, header1, header2, header3, header4, header5, header6, header7;
            fromShortBE(rh, out header0, out header1);
            fromShortBE(playerMask0, out header2, out header3);
            fromShortBE(playerMask1, out header4, out header5);
            fromShortBE(playerMask2, out header6, out header7);

            byte[] ackIdData = { header0, header1, header2, header3, header4, header5, header6, header7 };

            // Rest of data
            byte[] header = concatenateByteArrays(controlByteHeader, ackIdData);
            byte[] packetData = concatenateByteArrays(header, in_data);

            send(packetData);

            // UDP, store reliable in send map
            if (in_reliable && m_connectionType == RelayConnectionType.UDP)
            {
                UDPPacket packet = new UDPPacket(packetData, in_channel, packetId, 0);

                ulong ackId = BitConverter.ToUInt64(ackIdData, 0);
                m_reliables[ackId] = packet;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPingInterval(float in_interval)
        {
            m_timeSinceLastPingRequest = 0;
            m_pingInterval = (int)(in_interval * 1000);
        }

        public string GetOwnerProfileId()
        {
            string[] splits = m_ownerCxId.Split(':');
            if (splits.Length != 3) return "";
            return splits[1];
        }

        public string GetProfileIdForNetId(short netId)
        {
            if (m_netIdToCxId.ContainsKey((int)netId))
            {
                string cxId = m_netIdToCxId[(int)netId];
                string[] splits = cxId.Split(':');
                if (splits.Length != 3) return null;
                return splits[1];
            }
            return null;
        }

        public short GetNetIdForProfileId(string profileId)
        {
            foreach (KeyValuePair<string, int> entry in m_cxIdToNetId)
            {
                string[] splits = entry.Key.Split(':');
                if (splits.Length != 3) continue;
                if (profileId == splits[1]) return (short)entry.Value;
            }
            return (short)INVALID_NET_ID;
        }

        public string GetOwnerCxId()
        {
            return m_ownerCxId;
        }

        public string GetCxIdForNetId(short netId)
        {
            return m_netIdToCxId[(int)netId];
        }

        public short GetNetIdForCxId(string cxId)
        {
            if (m_cxIdToNetId.ContainsKey(cxId))
            {
                return (short)m_cxIdToNetId[cxId];
            }
            return (short)INVALID_NET_ID;
        }

        /// <summary>
        /// Callbacks responded to on the main thread
        /// </summary>
        public void Update()
        {
            // ** Resend connect request **
            // A UDP client needs to resend that until a confirmation is received that they are connected.
            // A subsequent connection request will just be ignored if already connected.
            // Re-transmission doesn't need to be high frequency. A suitable interval could be 500ms.
            if (m_connectionType == RelayConnectionType.UDP && m_resendConnectRequest)
            {
                if ((DateTime.Now - m_lastConnectResendTime).TotalSeconds > 0.5)
                {
                    send(buildConnectionRequest());
                    m_lastConnectResendTime = DateTime.Now;
                }
            }

            // Ping
            DateTime nowMS = DateTime.Now;
            if (IsConnected())
            {
                m_timeSinceLastPingRequest += (nowMS - m_lastNowMS).Milliseconds;
                m_lastNowMS = nowMS;

                if (m_timeSinceLastPingRequest >= m_pingInterval)
                {
                    m_timeSinceLastPingRequest = 0;
                    ping();
                }

                // Process reliable resends
                if (m_connectionType == RelayConnectionType.UDP)
                {
                    foreach (var kv in m_reliables)
                    {
                        UDPPacket packet = kv.Value;
                        if ((packet.TimeSinceFirstSend - nowMS).Milliseconds > 10000)
                        {
                            disconnect();
                            queueErrorEvent("Relay disconnected, too many packet lost");
                            break;
                        }
                        if ((packet.LastTimeSent - nowMS).Milliseconds > packet.TimeInterval)
                        {
                            packet.UpdateTimeIntervalSent();
                            send(packet.RawData);
                        }
                    }
                }
            }

            // Check if we timeout
            if (m_connectionType == RelayConnectionType.UDP &&
                (nowMS - m_lastRecvTime).Milliseconds > 10000)
            {
                disconnect();
                queueErrorEvent("Relay Socket Timeout");
            }

            // Perform event callbacks
            for (int i = 0; i < 10 && m_events.Count > 0; ++i) // Events can trigger other events, we want to consume as fast as possible. Add loop cap in case get stuck
            {
                List<Event> eventsCopy;
                lock (m_events)
                {
                    eventsCopy = m_events;
                    m_events = new List<Event>();
                }
                
                for (int j = 0; j < eventsCopy.Count; ++j)
                {
                    Event evt = eventsCopy[j];
                    switch (evt.type)
                    {
                        case EventType.SocketData:
                            m_lastRecvTime = DateTime.Now;
                            onRecv(evt.data);
                            break;
                        case EventType.SocketError:
                            disconnect();
                            queueErrorEvent(evt.message);
                            break;
                        case EventType.SocketConnected:
                            {
                                m_lastNowMS = DateTime.Now;
                                m_lastRecvTime = DateTime.Now;
                                send(buildConnectionRequest());

                                if (m_connectionType == RelayConnectionType.UDP)
                                {
                                    m_resendConnectRequest = true;
                                    m_lastConnectResendTime = DateTime.Now;
                                }
                                break;
                            }
                        case EventType.ConnectSuccess:
                            if (m_connectedSuccessCallback != null)
                            {
                                m_connectedSuccessCallback(evt.message, m_connectedObj);
                            }
                            break;
                        case EventType.ConnectFailure:
                            if (m_connectionFailureCallback != null)
                            {
                                eventsCopy.Clear();
                                lock (m_events)
                                {
                                    m_events.Clear();
                                }
                                var callback = m_connectionFailureCallback;
                                var callbackObj = m_connectedObj;
                                m_connectionFailureCallback = null;
                                m_connectedObj = null;
                                callback(400, -1, buildRSRequestError(evt.message), callbackObj);
                            }
                            break;
                        case EventType.System:
                            if (m_registeredSystemCallback != null)
                            {
                                m_registeredSystemCallback(evt.message);
                            }
                            break;
                        case EventType.Relay:
                            if (m_registeredRelayCallback != null)
                            {
                                // Callback data without headers
                                byte[] data = new byte[evt.data.Length - 11];
                                Buffer.BlockCopy(evt.data, 11, data, 0, data.Length);
                                m_registeredRelayCallback(evt.netId, data);
                            }
                            break;
                    }
                }
            }
        }

        #region private
        /// <summary>
        /// 
        /// </summary>
        public void ping()
        {
            m_sentPing = DateTime.Now.Ticks;
            short lastPingShort = Convert.ToInt16(Ping * 0.0001);
            byte data1, data2;
            fromShortBE(lastPingShort, out data1, out data2);

            byte[] dataArr = { data1, data2 };
            byte target = Convert.ToByte(CL2RS_PING);
            byte[] header = { target };

            byte[] destination = concatenateByteArrays(header, dataArr);
            send(destination);
        }

        private byte[] buildConnectionRequest()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["cxId"] = m_clientRef.RTTConnectionID;
            json["lobbyId"] = m_connectOptions.lobbyId;
            json["passcode"] = m_connectOptions.passcode;
            json["version"] = m_clientRef.BrainCloudClientVersion;

            byte[] array = concatenateByteArrays(CONNECT_ARR, Encoding.ASCII.GetBytes(JsonWriter.Serialize(json)));
            return array;
        }

        private string buildRSRequestError(string in_statusMessage)
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["status"] = 403;
            json["reason_code"] = ReasonCodes.RS_CLIENT_ERROR;
            json["status_message"] = in_statusMessage;
            json["severity"] = "ERROR";

            return JsonWriter.Serialize(json);
        }

        private byte[] buildDisconnectRequest()
        {
            return DISCONNECT_ARR;
        }


        /// <summary>
        /// 
        /// </summary>
        private void disconnect()
        {
            m_bIsConnected = false;
            m_connectedSuccessCallback = null;
            m_connectedObj = null;
            m_resendConnectRequest = false;

            m_connectionType = RelayConnectionType.INVALID;

            m_cxIdToNetId.Clear();
            m_netIdToCxId.Clear();
            m_ownerCxId = "";
            m_netId = INVALID_NET_ID;

            if (m_webSocket != null) m_webSocket.Close();
            m_webSocket = null;

            if (m_tcpStream != null)
            {
                m_tcpStream.Dispose();
            }
            m_tcpStream = null;

            if (m_tcpClient != null)
            {
                m_tcpClient.Client.Close(0);
                m_tcpClient.Close();
                fToSend.Clear();
            }
            m_tcpClient = null;

            if (m_udpClient != null) m_udpClient.Close();
            m_udpClient = null;

            // cleanup UDP stuff
            m_sendPacketId.Clear();
            m_recvPacketId.Clear();
            m_reliables.Clear();
            m_orderedReliablePackets.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private byte[] appendSizeBytes(byte[] in_data)
        {
            byte data1, data2;
            // size of data is the incoming data, plus the two that we're adding
            short sizeOfData = Convert.ToInt16(in_data.Length + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY);
            fromShortBE(sizeOfData, out data1, out data2);
            // append length prefixed, before sending off
            byte[] dataArr = { data1, data2 };
            return concatenateByteArrays(dataArr, in_data);
        }

        /// <summary>
        /// Parse Header bits associated by two bytes, 
        /// 0|reliable, 1|ordered, 2-3|channel, 4-16|packetId
        /// </summary>
        private void parseHeaderData(byte[] in_data, out bool out_reliable, out bool out_ordered, out int out_channel, out int out_packetId)
        {
            ushort reliableHeader = toShortBE(in_data);

            // read in big endian
            out_reliable = (reliableHeader & (1 << 15)/*0b1000_0000_0000_0000*/) == (1 << 15)/*0b1000_0000_0000_0000*/;
            out_ordered = (reliableHeader & (1 << 14) /*0b0100_0000_0000_0000*/) == (1 << 14)/*0b0100_0000_0000_0000*/;
            out_channel = (reliableHeader >> 12) & 0x3;
            out_packetId = reliableHeader & 0xFFF;
        }

        /// <summary>
        /// raw send of byte[]
        /// </summary>
        private bool send(byte[] in_data)
        {
            bool bMessageSent = false;
            // early return, based on type
            switch (m_connectionType)
            {
                case RelayConnectionType.WEBSOCKET:
                    {
                        if (m_webSocket == null)
                            return bMessageSent;
                    }
                    break;
                case RelayConnectionType.TCP:
                    {
                        if (m_tcpClient == null)
                            return bMessageSent;
                    }
                    break;
                case RelayConnectionType.UDP:
                    {
                        if (m_udpClient == null)
                            return bMessageSent;
                    }
                    break;
                default: break;
            }
            // actually do the send
            try
            {
                byte[] data = appendSizeBytes(in_data);
                switch (m_connectionType)
                {
                    case RelayConnectionType.WEBSOCKET:
                        {
                            m_webSocket.SendAsync(data);
                            bMessageSent = true;
                        }
                        break;
                    case RelayConnectionType.TCP:
                        {
                            tcpWrite(data);
                            bMessageSent = true;
                        }
                        break;
                    case RelayConnectionType.UDP:
                        {
                            m_udpClient.SendAsync(data, data.Length);
                            bMessageSent = true;
                        }
                        break;
                    default: break;
                }
            }
            catch (Exception socketException)
            {
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("send exception: " + socketException);
                }
                queueSocketErrorEvent(socketException.ToString());
            }

            return bMessageSent;
        }

        /// <summary>
        /// 
        /// </summary>
        private void startReceivingRSConnectionAsync()
        {
            bool sslEnabled = m_connectOptions.ssl;
            string host = m_connectOptions.host;
            int port = m_connectOptions.port;
            switch (m_connectionType)
            {
                case RelayConnectionType.WEBSOCKET:
                    {
                        connectWebSocket(host, port, sslEnabled);
                    }
                    break;
                case RelayConnectionType.TCP:
                    {
                        connectTCPAsync(host, port);
                    }
                    break;
                case RelayConnectionType.UDP:
                    {
                        connectUDPAsync(host, port);
                    }
                    break;
                default: break;
            }
        }

        private void WebSocket_OnClose(BrainCloudWebSocket sender, int code, string reason)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("Relay: Connection closed: " + reason);
            }
            queueErrorEvent(reason);
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("Relay: Connection established.");
            }
            // initial connect call, sets connection requests if not connected
            queueSocketConnectedEvent();
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            queueSocketDataEvent(data, data.Length);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("Relay Error: " + message);
            }
            queueErrorEvent(message);
        }

        private void sendRSMGAck(int rsmgPacketId)
        {
            byte[] data = new byte[3];
            data[0] = CL2RS_RSMG_ACK;

            byte data1, data2;
            fromShortBE((short)rsmgPacketId, out data1, out data2);
            data[1] = data1;
            data[2] = data2;

            send(data);
        }

        private void sendAck(byte[] in_data)
        {
            byte[] data = new byte[9];
            data[0] = CL2RS_ACK;
            Buffer.BlockCopy(in_data, 3, data, 1, 8);
            send(data);
        }

        private void onRSMG(byte[] in_data, int in_lengthOfData)
        {
            int rsmgPacketId = BitConverter.ToUInt16(new byte[2]{
                                                        BitConverter.IsLittleEndian ? in_data[4] : in_data[3],
                                                        BitConverter.IsLittleEndian ? in_data[3] : in_data[4]}, 0);

            if (m_connectionType == RelayConnectionType.UDP)
            {
                // Send ack, always. Even if we already received it
                sendRSMGAck(rsmgPacketId);

                // If already received, we ignore
                for (int i = 0; i < m_rsmgHistory.Count; ++i)
                {
                    if (m_rsmgHistory[i] == rsmgPacketId)
                    {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                        if (m_clientRef.LoggingEnabled)
                        {
                            m_clientRef.Log("Duplicated System Msg: " + rsmgPacketId.ToString());
                        }
#endif
                        return;
                    }
                }

                // Add to history
                m_rsmgHistory.Add(rsmgPacketId);

                // Crop to max history
                while (m_rsmgHistory.Count > MAX_RSMG_HISTORY)
                {
                    m_rsmgHistory.RemoveAt(0);
                }
            }

            int stringOffset = SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY + CONTROL_BYTE_HEADER_LENGTH + 2; // +2 for packet id
            int stringLen = in_lengthOfData - stringOffset;
            if (stringLen == 0)
            {
                queueErrorEvent("RSMG cannot be empty");
                return;
            }

            string jsonMessage = Encoding.ASCII.GetString(in_data, stringOffset, stringLen);
#if BC_DEBUG_RELAY_LOGS_ENABLED
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("Relay System Msg: " + jsonMessage);
            }
#endif

            Dictionary<string, object> parsedDict = (Dictionary<string, object>)JsonReader.Deserialize(jsonMessage);
            switch (parsedDict["op"] as string)
            {
                case "CONNECT":
                    {
                        int netId = (int)parsedDict["netId"];
                        string cxId = parsedDict["cxId"] as string;
                        m_cxIdToNetId[cxId] = netId;
                        m_netIdToCxId[netId] = cxId;
                        if (cxId == m_clientRef.RTTConnectionID && !m_bIsConnected)
                        {
                            m_netId = netId;
                            m_ownerCxId = parsedDict["ownerCxId"] as string;
                            m_bIsConnected = true;
                            m_lastNowMS = DateTime.Now;
                            m_resendConnectRequest = false;
                            queueConnectSuccessEvent(jsonMessage);
                        }
                        break;
                    }
                case "NET_ID":
                    {
                        int netId = (int)parsedDict["netId"];
                        string cxId = parsedDict["cxId"] as string;

                        m_cxIdToNetId[cxId] = netId;
                        m_netIdToCxId[netId] = cxId;
                        break;
                    }
                case "MIGRATE_OWNER":
                    {
                        m_ownerCxId = parsedDict["cxId"] as string;
                        break;
                    }
                case "DISCONNECT":
                    {
                        string profileId = parsedDict["profileId"] as string;
                        if (profileId == m_clientRef.AuthenticationService.ProfileId)
                        {
                            // We are the one that got disconnected!
                            disconnect();
                            queueErrorEvent("Disconnected by server");
                            return;
                        }
                        break;
                    }
            }

            queueSystemEvent(jsonMessage);
        }

        private void onPong()
        {
            Ping = DateTime.Now.Ticks - m_sentPing;
#if BC_DEBUG_RELAY_LOGS_ENABLED
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("Relay LastPing: " + (Ping * 0.0001f).ToString() + "ms");
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(byte[] in_data)
        {
            if (in_data.Length < 3) // Any packet is at least 3 bytes
            {
                queueErrorEvent("packet cannot be smaller than 3 bytes");
                return;
            }

            // Read control byte
            byte controlByte = in_data[SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY];

            // Take action depending on the control byte
            if (controlByte == RS2CL_RSMG)
            {
                if (in_data.Length < 5)
                {
                    queueErrorEvent("packet cannot be smaller than 5 bytes");
                    return;
                }
                onRSMG(in_data, in_data.Length);
            }
            else if (controlByte == RS2CL_DISCONNECT)
            {
                disconnect();
                queueErrorEvent("Relay: Disconnected by server");
            }
            else if (controlByte == RS2CL_PONG)
            {
                onPong();
            }
            else if (controlByte == RS2CL_ACK)
            {
                if (in_data.Length < 11)
                {
                    queueErrorEvent("ack packet cannot be smaller than 11 bytes");
                    return;
                }
                if (m_connectionType == RelayConnectionType.UDP)
                {
                    onUDPAcknowledge(in_data);
                }
            }
            else if (controlByte == RS2CL_RELAY)
            {
                if (in_data.Length < 11)
                {
                    queueErrorEvent("Relay packets cannot be smaller than 11 bytes");
                    return;
                }
#if BC_DEBUG_RELAY_LOGS_ENABLED
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("RELAY RECV: " + in_data.Length + " bytes, msg: " + Encoding.ASCII.GetString(in_data, 11, in_data.Length - 11));
                }
#endif
                onRelay(in_data);
            }
            else
            {
                // Invalid packet, throw error
                disconnect();
                queueErrorEvent("Relay Recv Error: Unknown control byte: " + controlByte.ToString());
            }
        }

        private bool packetLE(int a, int b)
        {
            if (a > PACKET_HIGHER_THRESHOLD && b <= PACKET_LOWER_THRESHOLD)
            {
                return true;
            }
            if (b > PACKET_HIGHER_THRESHOLD && a <= PACKET_LOWER_THRESHOLD)
            {
                return false;
            }
            return a <= b;
        }

        private void onRelay(byte[] in_data)
        {
            ushort rh = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[4] : in_data[3],
                BitConverter.IsLittleEndian ? in_data[3] : in_data[4]}, 0);
            ushort playerMask0 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[6] : in_data[5],
                BitConverter.IsLittleEndian ? in_data[5] : in_data[6]}, 0);
            ushort playerMask1 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[8] : in_data[7],
                BitConverter.IsLittleEndian ? in_data[7] : in_data[8]}, 0);
            ushort playerMask2 = BitConverter.ToUInt16(new byte[2]{
                BitConverter.IsLittleEndian ? in_data[10] : in_data[9],
                BitConverter.IsLittleEndian ? in_data[9] : in_data[10]}, 0);

            ulong ackId =
                (((ulong)rh << 48) & 0xFFFF000000000000) |
                (((ulong)playerMask0 << 32) & 0x0000FFFF00000000) |
                (((ulong)playerMask1 << 16) & 0x00000000FFFF0000) |
                (((ulong)playerMask2) & 0x000000000000FFFF);
            ulong ackIdWithoutPacketId = ackId & 0xF000FFFFFFFFFFFF;
            bool reliable = ((rh & RELIABLE_BIT) != 0) ? true : false;
            bool ordered = ((rh & ORDERED_BIT) != 0) ? true : false;
            int channel = (rh >> 12) & 0x3;
            int packetId = rh & 0xFFF;
            byte netId = (byte)(playerMask2 & 0x00FF);

            // Reconstruct ack id without packet id
            if (m_connectionType == RelayConnectionType.UDP)
            {
                // Ack reliables, always. An ack might have been previously dropped.
                if (reliable)
                {
                    sendAck(in_data);
                }

                if (ordered)
                {
                    int prevPacketId = MAX_PACKET_ID;
                    if (m_recvPacketId.ContainsKey(ackIdWithoutPacketId))
                    {
                        prevPacketId = m_recvPacketId[ackIdWithoutPacketId];
                    }

                    if (reliable)
                    {
                        if (packetLE(packetId, prevPacketId))
                        {
                            // We already received that packet if it's lower than the last confirmed
                            // packetId. This must be a duplicate
#if BC_DEBUG_RELAY_LOGS_ENABLED
                            if (m_clientRef.LoggingEnabled)
                            {
                                m_clientRef.Log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
                            }
#endif
                            return;
                        }

                        // Check if it's out of order, then save it for later
                        if (!m_orderedReliablePackets.ContainsKey(ackIdWithoutPacketId))
                        {
                            m_orderedReliablePackets[ackIdWithoutPacketId] = new List<UDPPacket>();
                        }
                        List<UDPPacket> orderedReliablePackets = m_orderedReliablePackets[ackIdWithoutPacketId];
                        if (packetId != ((prevPacketId + 1) & MAX_PACKET_ID))
                        {
                            if ((int)orderedReliablePackets.Count > MAX_PACKET_ID_HISTORY)
                            {
                                disconnect();
                                queueErrorEvent("Relay disconnected, too many queued out of order packets.");
                                return;
                            }

                            int insertIdx = 0;
                            for (; insertIdx < (int)orderedReliablePackets.Count; ++insertIdx)
                            {
                                var packet = orderedReliablePackets[insertIdx];
                                if (packet.Id == packetId)
                                {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                                    if (m_clientRef.LoggingEnabled)
                                    {
                                        m_clientRef.Log("Duplicated packet from " + netId.ToString() + ". got " + packetId.ToString());
                                    }
#endif
                                    return;
                                }
                                if (packetLE(packetId, packet.Id)) break;
                            }
                            var newPacket = new UDPPacket(in_data, channel, packetId, netId);
                            orderedReliablePackets.Insert(insertIdx, newPacket);
#if BC_DEBUG_RELAY_LOGS_ENABLED
                            if (m_clientRef.LoggingEnabled)
                            {
                                m_clientRef.Log("Queuing out of order reliable from " + netId.ToString() + ". got " + packetId.ToString());
                            }
#endif
                            return;
                        }

                        // It's in order, queue event
                        m_recvPacketId[ackIdWithoutPacketId] = packetId;
                        queueRelayEvent(netId, in_data);

                        // Empty previously queued packets if they follow this one
                        while (orderedReliablePackets.Count > 0)
                        {
                            var packet = orderedReliablePackets[0];
                            if (packet.Id == ((packetId + 1) & MAX_PACKET_ID))
                            {
                                queueRelayEvent(packet.NetId, packet.RawData);
                                orderedReliablePackets.RemoveAt(0);
                                packetId = packet.Id;
                                m_recvPacketId[ackIdWithoutPacketId] = packetId;
                                continue;
                            }
                            break; // Out of order
                        }
                        return;
                    }
                    else
                    {
                        if (packetLE(packetId, prevPacketId))
                        {
                            // Just drop out of order packets for unreliables
#if BC_DEBUG_RELAY_LOGS_ENABLED
                            if (m_clientRef.LoggingEnabled)
                            {
                                m_clientRef.Log("Out of order packet from " + netId.ToString() + ". Expecting " + ((prevPacketId + 1) & MAX_PACKET_ID).ToString() + ", got " + packetId.ToString());
                            }
#endif
                            return;
                        }
                        m_recvPacketId[ackIdWithoutPacketId] = packetId;
                    }
                }
            }

            queueRelayEvent(netId, in_data);
        }

        private void onUDPAcknowledge(byte[] in_data)
        {
            ulong ackId = BitConverter.ToUInt64(in_data, 3);
            m_reliables.Remove(ackId);

#if BC_DEBUG_RELAY_LOGS_ENABLED
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RELAY RECV ACK: " + ackId.ToString());
            }
#endif
        }

        private void onUDPRecv(IAsyncResult result)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            try
            {
                UdpClient udpClient = result.AsyncState as UdpClient;
                string host = m_connectOptions.host;
                int port = m_connectOptions.port;
                IPEndPoint source = new IPEndPoint(IPAddress.Parse(host), port);

                if (udpClient != null)
                {
                    // get the actual message and fill out the source:
                    byte[] data = udpClient.EndReceive(result, ref source);
                    queueSocketDataEvent(data, data.Length);
                    // schedule the next receive operation once reading is done:
                    udpClient.BeginReceive(new AsyncCallback(onUDPRecv), udpClient);
                }
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }

        /// <summary>
        /// Writes the specified message to the stream.
        /// </summary>
        /// <param name="message"></param>
        private void tcpWrite(byte[] message)
        {
            // Add this message to the list of message to send. If it's the only one in the
            // queue, fire up the async events to send it.
            try
            {
                lock (fLock)
                {
                    fToSend.Enqueue(message);
                    if (1 == fToSend.Count)
                    {
                        m_tcpStream.BeginWrite(message, 0, message.Length, tcpFinishWrite, null);
                    }
                }
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }

        private void tcpFinishWrite(IAsyncResult result)
        {
            try
            {
                m_tcpStream.EndWrite(result);
                lock (fLock)
                {
                    // Pop the message we just sent out of the queue
                    fToSend.Dequeue();

                    // See if there's anything else to send. Note, do not pop the message yet because
                    // that would indicate its safe to start writing a new message when its not.
                    if (fToSend.Count > 0)
                    {
                        byte[] final = fToSend.Peek();
                        m_tcpStream.BeginWrite(final, 0, final.Length, tcpFinishWrite, null);
                    }
                }
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }
        private void onTCPReadHeader(IAsyncResult ar)
        {
            try
            {
                // Read precisely SIZE_OF_HEADER for the length of the following message
                int read = m_tcpStream.EndRead(ar);
                if (read == 0)
                {
                    queueErrorEvent("Server Closed Connection");
                    return;
                }

                if (m_tcpStream != null && read == SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY)
                {
                    m_tcpBytesRead = 0;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(m_tcpHeaderReadBuffer);

                    // from the header that was read, how much should we read after this until the next message ? 
                    m_tcpBytesToRead = BitConverter.ToInt16(m_tcpHeaderReadBuffer, 0);
                    m_tcpBytesToRead -= SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY;

                    m_tcpStream.BeginRead(m_tcpReadBuffer, 2, m_tcpBytesToRead, onTCPFinishRead, null);
                }
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }
        private void onTCPFinishRead(IAsyncResult result)
        {
            try
            {
                if (m_tcpStream == null) return;
                // Finish reading from our stream. 0 bytes read means stream was closed
                int read = m_tcpStream.EndRead(result);
                if (read == 0)
                {
                    queueErrorEvent("Server Closed Connection");
                    return;
                }

                // Increment the number of bytes we've read. If there's still more to get, get them
                m_tcpBytesRead += read;
                if (m_tcpBytesRead < m_tcpBytesToRead)
                {
                    //if (m_clientRef.LoggingEnabled)
                    //{
                        //m_clientRef.Log("m_tcpBytesRead < m_tcpBuffer.Length " + m_tcpBytesRead + " " + m_tcpBytesToRead);
                    //}
                    m_tcpStream.BeginRead(m_tcpReadBuffer, m_tcpBytesRead, m_tcpBytesToRead - m_tcpBytesRead, onTCPFinishRead, null);
                    return;
                }

                // Should be exactly the right number read now.
                if (m_tcpBytesRead != m_tcpBytesToRead)
                {
                    queueErrorEvent("Incorrect Bytes Read " + m_tcpBytesRead + " " + m_tcpBytesToRead);
                    return;
                }

                // Handle the message
                queueSocketDataEvent(m_tcpReadBuffer, m_tcpBytesToRead + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY);

                // read the next header
                m_tcpBytesToRead = 0;

                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }

        private void connectWebSocket(string in_host, int in_port, bool in_sslEnabled)
        {
            string url = (in_sslEnabled ? "wss://" : "ws://") + in_host + ":" + in_port;
            m_webSocket = new BrainCloudWebSocket(url);
            m_webSocket.OnClose += WebSocket_OnClose;
            m_webSocket.OnOpen += Websocket_OnOpen;
            m_webSocket.OnMessage += WebSocket_OnMessage;
            m_webSocket.OnError += WebSocket_OnError;
        }

        private async void connectTCPAsync(string host, int port)
        {
            bool success = await Task.Run(async () =>
            {
                try
                {
                    m_tcpClient = new TcpClient();
                    m_tcpClient.NoDelay = true;
                    m_tcpClient.Client.NoDelay = true;
                    if (m_clientRef.LoggingEnabled)
                    {
                        m_clientRef.Log("Starting TCP connect ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
                    }
                    await m_tcpClient.ConnectAsync(host, port);
                }
                catch (Exception e)
                {
                    queueErrorEvent(e.ToString());
                    return false;
                }
                return true;
            });

            if (success)
            {
                m_tcpStream = m_tcpClient.GetStream();
                queueSocketConnectedEvent();
                // read the next header
                m_tcpBytesToRead = 0;
                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);

                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("Connected! ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
                }
            }
        }

        private void initUDPConnection()
        {
#if !DOT_NET
            m_udpClient = new UdpClient();
#endif

            // init packet id list
            m_sendPacketId.Clear();
            m_recvPacketId.Clear();
            m_reliables.Clear();
            m_rsmgHistory.Clear();
            m_orderedReliablePackets.Clear();
        }

        private void connectUDPAsync(string host, int port)
        {
            try
            {
#if DOT_NET
                m_udpClient = new UdpClient(host, port);
                initUDPConnection();
                OnUDPConnected(null, null);
#else
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnUDPConnected);
                args.RemoteEndPoint = new DnsEndPoint(host, port);

                initUDPConnection();
                m_udpClient.Client.ConnectAsync(args);
#endif
            }
            catch (Exception e)
            {
                queueErrorEvent(e.ToString());
            }
        }

        private void OnUDPConnected(object sender, SocketAsyncEventArgs args)
        {
            queueSocketConnectedEvent();
            m_udpClient.BeginReceive(new AsyncCallback(onUDPRecv), m_udpClient);
        }

        private void queueConnectSuccessEvent(string jsonString)
        {
            var evt = new Event();
            evt.type = EventType.ConnectSuccess;
            evt.message = jsonString;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueSocketErrorEvent(string message)
        {
            var evt = new Event();
            evt.type = EventType.SocketError;
            evt.message = message;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueSocketConnectedEvent()
        {
            var evt = new Event();
            evt.type = EventType.SocketConnected;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueSocketDataEvent(byte[] in_data, int length)
        {
            var evt = new Event();
            evt.type = EventType.SocketData;
            evt.data = new byte[length];
            Buffer.BlockCopy(in_data, 0, evt.data, 0, length);
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueErrorEvent(string message)
        {
            var evt = new Event();
            evt.type = EventType.ConnectFailure;
            evt.message = message;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueSystemEvent(string jsonString)
        {
            var evt = new Event();
            evt.type = EventType.System;
            evt.message = jsonString;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private void queueRelayEvent(short netId, byte[] data)
        {
            var evt = new Event();
            evt.type = EventType.Relay;
            evt.netId = netId;
            evt.data = data;
            lock (m_events)
            {
                m_events.Add(evt);
            }
        }

        private byte[] concatenateByteArrays(byte[] a, byte[] b)
        {
            byte[] rv = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, rv, 0, a.Length);
            Buffer.BlockCopy(b, 0, rv, a.Length, b.Length);
            return rv;
        }

        private void fromShortBE(short number, out byte byte1, out byte byte2)
        {
            byte1 = (byte)(number >> 8);
            byte2 = (byte)(number >> 0);
        }
        private void fromShortBE(ushort number, out byte byte1, out byte byte2)
        {
            byte1 = (byte)(number >> 8);
            byte2 = (byte)(number >> 0);
        }

        private ushort toShortBE(byte[] byteArr)
        {
            var offset = SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY + CONTROL_BYTE_HEADER_LENGTH;
            bool isLittleEndian = BitConverter.IsLittleEndian;
            ushort toReturn = BitConverter.ToUInt16(new byte[2] {
                                                        isLittleEndian ? byteArr[offset + 1] : byteArr[offset + 0],
                                                        isLittleEndian ? byteArr[offset + 0] : byteArr[offset + 1] }, 0);
            return toReturn;
        }

        private RelayConnectOptions m_connectOptions;

        private RelayConnectionType m_connectionType = RelayConnectionType.INVALID;
        private bool m_bIsConnected = false;
        private DateTime m_lastNowMS;
        private int m_timeSinceLastPingRequest = 0;
        private int m_pingInterval = 1000; // one second
        private DateTime m_lastRecvTime;

        private const int MAX_PACKET_ID_HISTORY = 60 * 10; // So we last 10 seconds at 60 fps
        private const int MAX_RELIABLE_RESEND_INTERVAL = 500;
        private const int MAX_PACKET_ID = 0xFFF;
        private const int MAX_CHANNELS = 4;
        private const int PACKET_LOWER_THRESHOLD = MAX_PACKET_ID * 25 / 100;
        private const int PACKET_HIGHER_THRESHOLD = MAX_PACKET_ID * 75 / 100;

        private string m_ownerCxId = "";
        private Dictionary<string, int> m_cxIdToNetId = new Dictionary<string, int>();
        private Dictionary<int, string> m_netIdToCxId = new Dictionary<int, string>();
        private int m_netId = INVALID_NET_ID;

        // start
        // different connection types
        // WebSocket
        private BrainCloudWebSocket m_webSocket = null;
        // TCP
        private TcpClient m_tcpClient = null;
        private NetworkStream m_tcpStream = null;
        private byte[] m_tcpReadBuffer = new byte[MAX_PACKETSIZE];

        // ASync TCP Reads
        const int SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY = 2;
        private int m_tcpBytesRead = 0; // the ones already processed
        private int m_tcpBytesToRead = 0; // the number to finish reading
        private byte[] m_tcpHeaderReadBuffer = new byte[SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY];

        // ASync TCP Writes
        private object fLock = new object();
        private Queue<byte[]> fToSend = new Queue<byte[]>();

        // UDP
        private UdpClient m_udpClient = null;

        // Packet history
        private List<int> m_rsmgHistory = new List<int>();
        private Dictionary<ulong, int> m_sendPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, int> m_recvPacketId = new Dictionary<ulong, int>();
        private Dictionary<ulong, UDPPacket> m_reliables = new Dictionary<ulong, UDPPacket>();
        private Dictionary<ulong, List<UDPPacket>> m_orderedReliablePackets = new Dictionary<ulong, List<UDPPacket>>();
        // end 

        private bool m_resendConnectRequest = false;
        private DateTime m_lastConnectResendTime = DateTime.Now;

        private const int CONTROL_BYTE_HEADER_LENGTH = 1;
        private const int SIZE_OF_RELIABLE_FLAGS = 2;

        public const ushort RELIABLE_BIT = 0x8000;
        public const ushort ORDERED_BIT = 0x4000;

        private BrainCloudClient m_clientRef;
        private long m_sentPing = DateTime.Now.Ticks;
        private byte[] DISCONNECT_ARR = { CL2RS_DISCONNECT };
        private byte[] CONNECT_ARR = { CL2RS_CONNECT };

        // success callbacks
        private SuccessCallback m_connectedSuccessCallback = null;
        private FailureCallback m_connectionFailureCallback = null;
        private object m_connectedObj = null;

        private RelayCallback m_registeredRelayCallback = null;
        private RelaySystemCallback m_registeredSystemCallback = null;

        private enum EventType
        {
            SocketError,
            SocketConnected,
            SocketData,
            ConnectSuccess,
            ConnectFailure,
            Relay,
            System
        }

        private class Event
        {
            public EventType type;
            public string message;
            public short netId;
            public byte[] data;
        }

        private List<Event> m_events = new List<Event>();

        private class UDPPacket
        {
            public UDPPacket(byte[] in_data, int in_channel, int in_packetId, byte in_netId)
            {
                LastTimeSent = DateTime.Now;
                TimeSinceFirstSend = DateTime.Now;
                TimeInterval = in_channel <= 1 ? 50 : in_channel == 2 ? 150 : 250;// ms
                RawData = in_data;
                Id = in_packetId;
                NetId = in_netId;
            }

            public void UpdateTimeIntervalSent()
            {
                LastTimeSent = DateTime.Now;
                TimeInterval = Math.Min((int)(TimeInterval * 1.25f), MAX_RELIABLE_RESEND_INTERVAL);
            }
            public DateTime TimeSinceFirstSend { get; private set; }
            public DateTime LastTimeSent { get; private set; }
            public int TimeInterval { get; private set; }
            public byte[] RawData { get; private set; }
            public int Id { get; private set; }
            public byte NetId { get; private set; }
        }
#endregion
    }
}

namespace BrainCloud
{
#region public enums
    public enum RelayConnectionType
    {
        INVALID,
        WEBSOCKET,
        TCP,
        UDP,

        MAX
    }
#endregion
}