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

    
    internal sealed class RelayComms
    {
        #region public consts
        public const int MAX_PACKETSIZE = 1024;

        public const byte MAX_PLAYERS = 128;
        public const byte INVALID_NET_ID = MAX_PLAYERS;

        public const byte CL2RS_CONNECTION = 129;
        public const byte CL2RS_DISCONNECT = 130;
        public const byte CL2RS_RELAY = 131;
        public const byte CL2RS_PING = 133;
        public const byte CL2RS_RSMG_ACKNOWLEDGE = 134;
        public const byte CL2RS_ACKNOWLEDGE = 135;

        public const byte RS2CL_RSMG = 129;
        public const byte RS2CL_PONG = CL2RS_PING;
        public const byte RS2CL_ACKNOWLEDGE = CL2RS_ACKNOWLEDGE;

        public const short TO_ALL_PLAYERS = CL2RS_RELAY;
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
                m_clientRef.Log("Non-WebSocket Connection Type Requested, on WEBGL.  Please connect via WebSocket.");

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
            addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "Disconnect Called"));
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

        /// <summary>
        /// send byte array representation of data
        /// </summary>
        /// <param in_message="message to be sent"></param>
        /// <param to_netId="the net id to send to, RelayComms.TO_ALL_PLAYERS to relay to all"></param>
        /// <param in_reliable="send this reliably or not"></param>
        /// <param in_ordered="received this ordered or not"></param>
        /// <param in_channel="0,1,2,3 (max of four channels)"></param>
        public void Send(byte[] in_data, short in_target, bool in_reliable = true, bool in_ordered = true, int in_channel = 0)
        {
            // appened the target (netId, or all) to the beginning
            byte target = Convert.ToByte(in_target);
            if (!(target < MAX_PLAYERS || target == TO_ALL_PLAYERS))
            {
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError("Invalid NetId: " + target.ToString())));
                return;
            }

            if (in_data.Length > MAX_PACKETSIZE)
            {
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError("Packet too big: " + in_data.Length.ToString() + " > max " + MAX_PACKETSIZE.ToString())));
                return;
            }
            
            byte[] destination = appendHeaderData(in_data, target, in_reliable, in_ordered, in_channel);
            send(destination, in_reliable, in_ordered, in_channel);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPingInterval(float in_interval)
        {
            m_timeSinceLastPingRequest = 0;
            m_pingInterval = (int)(in_interval * 1000);
        }

        /// <summary>
        /// Callbacks responded to on the main thread
        /// </summary>
        public void Update()
        {
            RSCommandResponse toProcessResponse;
            bool isConnected = IsConnected();
            lock (m_queuedRSCommands)
            {
                for (int i = 0; i < m_queuedRSCommands.Count; ++i)
                {
                    toProcessResponse = m_queuedRSCommands[i];

                    if (toProcessResponse.Operation == "connect") // Socket connected
                    {
                        m_lastNowMS = DateTime.Now;
                        send(buildConnectionRequest(), true, true, 0);

                        if (m_connectionType == RelayConnectionType.UDP)
                        {
                            m_resendConnectRequest = true;
                            m_lastConnectResendTime = DateTime.Now;
                        }
                    }
                    else if (toProcessResponse.Operation == "rsmg") // System message
                    {
                        Dictionary<string, object> parsedDict = (Dictionary<string, object>)JsonReader.Deserialize(toProcessResponse.JsonMessage);
                        switch (parsedDict["op"] as string)
                        {
                            case "CONNECT":
                            {
                                int netId = (int)parsedDict["netId"];
                                string profileId = parsedDict["profileId"] as string;
                                m_profileIdToNetId[profileId] = netId;
                                m_netIdToProfileId[netId] = profileId;
                                if (profileId == m_clientRef.AuthenticationService.ProfileId && !m_bIsConnected)
                                {
                                    m_resendConnectRequest = false;
                                    m_netId = netId;
                                    m_bIsConnected = true;
                                    m_lastNowMS = DateTime.Now;
                                    if (m_connectedSuccessCallback != null)
                                    {
                                        m_connectedSuccessCallback(toProcessResponse.JsonMessage, m_connectedObj);
                                    }
                                }
                                break;
                            }
                            case "DISCONNECT":
                            {
                                string profileId = parsedDict["profileId"] as string;
                                if (profileId == m_clientRef.AuthenticationService.ProfileId)
                                {
                                    // This will only really happen on UDP
                                    if (m_connectionFailureCallback != null)
                                        m_connectionFailureCallback(400, -1, toProcessResponse.JsonMessage, m_connectedObj);
                                    disconnect();
                                    m_queuedRSCommands.Clear();
                                    return;
                                }
                                break;
                            }
                            case "NET_ID":
                            {
                                int netId = (int)parsedDict["netId"];
                                string profileId = parsedDict["profileId"] as string;
                                m_profileIdToNetId[profileId] = netId;
                                m_netIdToProfileId[netId] = profileId;
                                break;
                            }
                            case "MIGRATE_OWNER":
                            {
                                m_ownerId = parsedDict["profileId"] as string;
                                break;
                            }
                        }

                        if (m_registeredSystemCallback != null)
                        {
                            m_registeredSystemCallback(toProcessResponse.JsonMessage);
                        }
                    }
                    else if ((toProcessResponse.Operation == "error" || toProcessResponse.Operation == "disconnect")) // error/disconnect
                    {
                        if (m_connectionFailureCallback != null)
                            m_connectionFailureCallback(400, -1, toProcessResponse.JsonMessage, m_connectedObj);

                        if (toProcessResponse.Operation == "disconnect")
                        {
                            disconnect();
                            m_queuedRSCommands.Clear();
                            return;
                        }
                    }
                    else if (toProcessResponse.Operation == "relay") // Relay
                    {
                        if (m_registeredRelayCallback != null && toProcessResponse.RawData != null)
                        {
                            m_registeredRelayCallback(toProcessResponse.RawData);
                        }
                    }
                }

                m_queuedRSCommands.Clear();
            }

            // ** Resend connect request **
            // A UDP client needs to resend that until a confirmation is received that they are connected.
            // A subsequent connection request will just be ignored if already connected.
            // Re-transmission doesn't need to be high frequency. A suitable interval could be 500ms.
            if (m_connectionType == RelayConnectionType.UDP && m_resendConnectRequest)
            {
                if ((DateTime.Now - m_lastConnectResendTime).TotalSeconds > 0.5)
                {
                    send(buildConnectionRequest(), true, true, 0);
                    m_lastConnectResendTime = DateTime.Now;
                }
            }

            // Ping
            if (isConnected)
            {
                DateTime nowMS = DateTime.Now;
                m_timeSinceLastPingRequest += (nowMS - m_lastNowMS).Milliseconds;
                m_lastNowMS = nowMS;

                if (m_timeSinceLastPingRequest >= m_pingInterval)
                {
                    m_timeSinceLastPingRequest = 0;
                    ping();
                }

                processReliableQueue();
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
            Send(dataArr, CL2RS_PING);
        }

        private byte[] buildConnectionRequest()
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["profileId"] = m_clientRef.ProfileId;
            json["lobbyId"] = m_connectOptions.lobbyId;
            json["passcode"] = m_connectOptions.passcode;

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
            if (IsConnected()) send(buildDisconnectRequest(), true, false, 0);

            m_bIsConnected = false;
            m_connectedSuccessCallback = null;
            m_connectionFailureCallback = null;
            m_connectedObj = null;
            m_resendConnectRequest = false;

            m_connectionType = RelayConnectionType.INVALID;

            m_profileIdToNetId.Clear();
            m_netIdToProfileId.Clear();
            m_ownerId = "";
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
            m_currentReceivedPacketId.Clear();
            m_reliableSentMap.Clear();
            m_orderedReceivedMap.Clear();
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

        private void constructReliableHeader(out byte out_data1, out byte out_data2, bool in_reliable, bool in_ordered, int in_channel)
        {
            ushort reliableHeader = 0;
            // r = reliable bit
            // o = ordered bit
            // ch = channel bits (up to four channels)
            // pack_etid_rest = packet id, 12 bits (MAX_PACKET_ID, 4096)
            //                                   roch_pack_etid_rest
            if (in_reliable) reliableHeader |= 1 << 15;// 0b1000_0000_0000_0000;
            if (in_ordered) reliableHeader |= 1 << 14;// 0b0100_0000_0000_0000;
            if (in_channel > 0)
            {
                switch (in_channel)
                {
                    case 1:
                        {
                            reliableHeader |= 1 << 12; //0b0001_0000_0000_0000;
                        }
                        break;
                    case 2:
                        {
                            reliableHeader |= 1 << 13; //0b0010_0000_0000_0000;
                        }
                        break;
                    case 3:
                        {
                            reliableHeader |= 1 << 12;
                            reliableHeader |= 1 << 13; //reliableHeader |= 0b0011_0000_0000_0000;
                        }
                        break;
                    default:
                        break;
                }
            }

            // append to reliable header
            var packetId = m_sendPacketId[in_channel][in_reliable] & 0xFFF;
            reliableHeader |= (ushort)packetId;

            // increment packet id
            packetId = (packetId + 1) % MAX_PACKET_ID;
            m_sendPacketId[in_channel][in_reliable] = packetId;

            // switch to Big Endian
            fromShortBE(reliableHeader, out out_data1, out out_data2);
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
        /// Reliable bits associated by two bytes, 
        /// 0|reliable, 1|ordered, 2-3|channel, 4-16|packetId
        /// </summary>
        private byte[] appendHeaderData(byte[] in_data, byte in_header, bool in_reliable, bool in_ordered, int in_channel)
        {
            byte[] destination = null;
            byte[] header = { in_header };
            if (in_header == TO_ALL_PLAYERS || in_header < MAX_PLAYERS)
            {
                byte data1 = 0;
                byte data2 = 0;
                if (m_connectionType == RelayConnectionType.UDP)
                {
                    constructReliableHeader(out data1, out data2, in_reliable, in_ordered, in_channel);
                }
                header = new byte[] { in_header, data1, data2 };
            }

            destination = concatenateByteArrays(header, in_data);
            return destination;
        }

        /// <summary>
        /// resend 
        /// </summary>
        private void resendPacket(UDPPacket in_packet)
        {
            // force resend! reliable, ordered and channel do not matter in this case
            send(in_packet.RawData, true, true, 0, true);
            in_packet.UpdateTimeIntervalSent();
        }

        /// <summary>
        /// raw send of byte[]
        /// </summary>
        private bool send(byte[] in_data, bool in_reliable, bool in_ordered, int in_channel, bool in_bResend = false)
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
#if BC_DEBUG_RELAY_LOGS_ENABLED
                m_clientRef.Log("RELAY SEND: " + in_data.Length + " bytes, msg: " + Encoding.ASCII.GetString(in_data, 0, in_data.Length));// + ", Stack: " + new System.Diagnostics.StackTrace().ToString());
#endif
                if (!in_bResend) in_data = appendSizeBytes(in_data);
                switch (m_connectionType)
                {
                    case RelayConnectionType.WEBSOCKET:
                        {
                            m_webSocket.SendAsync(in_data);
                            bMessageSent = true;
                        }
                        break;
                    case RelayConnectionType.TCP:
                        {
                            tcpWrite(in_data);
                            bMessageSent = true;
                        }
                        break;
                    case RelayConnectionType.UDP:
                        {
                            if (!in_bResend && in_reliable)
                            {
                                byte controlHeader = in_data[SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY];
                                if (controlHeader == CL2RS_RELAY || controlHeader < MAX_PLAYERS)
                                {
                                    // add to the reliable queue
                                    int packetId, channel;
                                    bool reliable, ordered;
                                    parseHeaderData(in_data, out reliable, out ordered, out channel, out packetId);
                                    lock (m_reliableSentMap)
                                    {
                                        m_reliableSentMap[in_channel][packetId] = new UDPPacket(in_data, in_channel, packetId, m_netId, in_reliable);
                                    }
                                }
                            }
                            m_udpClient.SendAsync(in_data, in_data.Length);
                            bMessageSent = true;
                        }
                        break;
                    default: break;
                }
            }
            catch (Exception socketException)
            {
                m_clientRef.Log("send exception: " + socketException);
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(socketException.ToString())));
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
            m_clientRef.Log("Relay: Connection closed: " + reason);
            addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", reason));
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            m_clientRef.Log("Relay: Connection established.");
            // initial connect call, sets connection requests if not connected
            addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "connect", ""));
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            onRecv(data, data.Length);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            m_clientRef.Log("Relay Error: " + message);
            addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(message)));
        }

        private bool isOppRSMG(byte controlByte)
        {
            return controlByte < MAX_PLAYERS ||
                controlByte == CL2RS_CONNECTION ||
                controlByte == CL2RS_RELAY;
        }

        private bool isReceivedServerAck(byte controlByte)
        {
            return (m_connectionType == RelayConnectionType.UDP && controlByte == CL2RS_ACKNOWLEDGE);
        }

        /// <summary>
        /// 
        /// </summary>
        private void onRecv(byte[] in_data, int in_lengthOfData)
        {
            if (in_lengthOfData < 3) // Any packet is at least 3 bytes
            {
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "packet cannot be smaller than 3 bytes"));
                return;
            }

            // Read control byte
            byte controlByte = in_data[SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY];

            // acknowledge we got it right away.
            if (m_connectionType == RelayConnectionType.UDP)
            {
                if (!onUDPAcknowledge(controlByte, in_data, in_lengthOfData))
                {
                    return;
                }
            }

            // Take action depending on the control byte
            if (controlByte == RS2CL_RSMG)
            {
                if (in_lengthOfData < 5)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "RSMG cannot be smaller than 5 bytes"));
                    return;
                }

                if (m_connectionType == RelayConnectionType.UDP)
                {
                    // Check if we already received it
                    int packetId = BitConverter.ToUInt16(in_data, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY + CONTROL_BYTE_HEADER_LENGTH);
                    for (int i = 0; i < m_rsmgHistory.Count; ++i)
                    {
                        if (m_rsmgHistory[i] == packetId)
                        {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                            m_clientRef.Log("Duplicated System Msg: " + packetId.ToString());
#endif
                            return;
                        }
                    }

                    // Add to history
                    m_rsmgHistory.Add(packetId);

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
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "RSMG cannot be empty"));
                    return;
                }

                string jsonMessage = Encoding.ASCII.GetString(in_data, stringOffset, stringLen);
#if BC_DEBUG_RELAY_LOGS_ENABLED
                m_clientRef.Log("Relay System Msg: " + jsonMessage);
#endif
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "rsmg", jsonMessage, null));
            }
            else if (controlByte == RS2CL_PONG)
            {
                Ping = DateTime.Now.Ticks - m_sentPing;
#if BC_DEBUG_RELAY_LOGS_ENABLED
                m_clientRef.Log("Relay LastPing: " + (Ping * 0.0001f).ToString() + "ms");
#endif
            }
            else if (controlByte == RS2CL_ACKNOWLEDGE)
            {
                if (in_lengthOfData < 5)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "ack packet cannot be smaller than 5 bytes"));
                    return;
                }
                if (m_connectionType == RelayConnectionType.UDP)
                {
                    removeReliableQueueData(in_data);
                }
            }
            else if (controlByte < MAX_PLAYERS)
            {
                if (in_lengthOfData < 5)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "Relay packets cannot be smaller than 5 bytes"));
                    return;
                }
#if BC_DEBUG_RELAY_LOGS_ENABLED
                m_clientRef.Log("RELAY RECV: " + in_lengthOfData + " bytes, msg: " + Encoding.ASCII.GetString(in_data, 5, in_lengthOfData - 5));
#endif
                if (confirmOrderedReceive(controlByte, in_data, in_lengthOfData))
                {
                    int offset = SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY + CONTROL_BYTE_HEADER_LENGTH + SIZE_OF_RELIABLE_FLAGS;
                    int cutOffLen = in_lengthOfData - offset;
                    byte[] cutOffData = new byte[cutOffLen];
                    Buffer.BlockCopy(in_data, offset, 
                                     cutOffData, 0, cutOffLen);
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "relay", "", cutOffData));
                }
            }
            else
            {
                // Invalid packet, throw error
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "Relay Rcv Error: Unknown control byte: " + controlByte));
            }

            processOrderedReceivedMap();
        }

        private void removeReliableQueueData(byte[] in_data)
        {
            bool reliable, ordered;
            int channel, packetId;
            parseHeaderData(in_data, out reliable, out ordered, out channel, out packetId);

#if BC_DEBUG_RELAY_LOGS_ENABLED
            m_clientRef.Log("RELAY RECV ACK: " + packetId.ToString());
#endif
            lock (m_reliableSentMap)
            {
                if (m_reliableSentMap[channel].ContainsKey(packetId))
                {
                    m_reliableSentMap[channel].Remove(packetId);
                }
            }
        }

        private void processReliableQueue()
        {
            lock (m_reliableSentMap)
            {
                DateTime nowMS = DateTime.Now;
                // for each channel
                foreach (var reliables in m_reliableSentMap)
                {
                    // we always resend reliably ordered
                    var ordered = reliables.OrderBy(x => x.Key);
                    // and every packet that is in the queue
                    foreach (var udpPacket in ordered)
                    {
                        // if the delta of the next time interval < the delta of now and last time we tried sending
                        if (udpPacket.Value.TimeInterval < (nowMS - udpPacket.Value.LastTimeSent).Milliseconds)
                        {
                            // RESEND!
                            resendPacket(udpPacket.Value);
                        }
                    }
                }
            }
        }

        private bool confirmOrderedReceive(byte in_incomingNetId, byte[] in_data, int in_lengthOfData)
        {
            bool bProcessReceived = true;

            if (m_connectionType == RelayConnectionType.UDP)
            {
                // only do this for messages that are reliable
                // read the incoming datas flags based off its header
                bool reliable, ordered;
                int channel, incomingPacketId;
                parseHeaderData(in_data, out reliable, out ordered, out channel, out incomingPacketId);
                if (ordered && in_incomingNetId < MAX_PLAYERS)
                {
                    // update to the newer packet id on the channel
                    // if we haven't seen this packet before
                    // non reliable, we just care if its less then the incoming one
                    // reliable, we need it to be the correct one
                    int currentPacketId = (m_currentReceivedPacketId[channel][reliable].ContainsKey(in_incomingNetId) ? m_currentReceivedPacketId[channel][reliable][in_incomingNetId] : 0);

                    bool receivedNewerPacketId = !m_currentReceivedPacketId[channel][reliable].ContainsKey(in_incomingNetId) ||
                                                 ((!reliable && incomingPacketId > currentPacketId) ||      // all others dropped
                                                  (reliable && incomingPacketId == currentPacketId + 1));   // consecutive one

                    // check cyclical packets
                    if (!receivedNewerPacketId)
                    {
                        if (reliable && currentPacketId == MAX_PACKET_ID - 1 && incomingPacketId == 0)
                            receivedNewerPacketId = true;
                        // give a 25% buffer to receive a new one 
                        else if (!reliable && currentPacketId >= MAX_PACKET_ID * 0.75 && incomingPacketId >= 0)
                            receivedNewerPacketId = true;
                    }

                    if (receivedNewerPacketId)
                    {
                        m_currentReceivedPacketId[channel][reliable][in_incomingNetId] = incomingPacketId;
                    }
                    else if (reliable && incomingPacketId > currentPacketId)
                    {
                        lock (m_orderedReceivedMap)
                        {
                            m_orderedReceivedMap[channel].Add(new KeyValuePair<int, int>(in_incomingNetId, incomingPacketId), new UDPPacket(in_data, channel, incomingPacketId, in_incomingNetId, reliable));
                        }
                    }

                    // by default we are going to process
                    // these are the conditions that we will not process the received response again
                    if (!receivedNewerPacketId ||                                // dont process older ones
                        m_orderedReceivedMap[channel].ContainsKey(
                            new KeyValuePair<int, int>(in_incomingNetId, incomingPacketId))   // already has seen this control byte + packet id combo
                       )
                    {
                        bProcessReceived = false;
                    }
                }
            }

            return bProcessReceived;
        }

        private void processOrderedReceivedMap()
        {
            if (m_connectionType == RelayConnectionType.UDP)
            {
                List<UDPPacket> toProcess = new List<UDPPacket>();
                lock (m_orderedReceivedMap)
                {
                    DateTime nowMS = DateTime.Now;
                    // for each channel
                    int currentPacketId = 0;
                    for (int channel = 0; channel < MAX_CHANNELS; ++channel)
                    {
                        // we always process in order
                        var ordered = m_orderedReceivedMap[channel].OrderBy(x => x.Key.Value);
                        // and every packet that is in the queue
                        foreach (var udpPacket in ordered)
                        {
                            currentPacketId = m_currentReceivedPacketId[channel][udpPacket.Value.Reliable][udpPacket.Key.Key];
                            //m_clientRef.Log("processOrderedReceivedMap k:" + channel + " p:" + udpPacket.Value.PacketId + " cp:" + m_currentReceivedPacketId[channel][udpPacket.Key.Key]);
                            if (udpPacket.Value.Reliable && udpPacket.Value.PacketId == currentPacketId + 1)
                            {
                                //m_clientRef.Log(" processOrderedReceivedMap k:" + channel + " removing p:" + udpPacket.Value.PacketId + " cp:" + currentPacketId);
                                toProcess.Add(udpPacket.Value);
                                // remove the item
                                m_orderedReceivedMap[channel].Remove(udpPacket.Key);
                            }
                            // packet id is less then the current one we are on
                            else if (!udpPacket.Value.Reliable && udpPacket.Value.PacketId <= currentPacketId)
                            {
                                //m_clientRef.Log("**REMOVING processOrderedReceivedMap k:" + channel + " removing p:" + udpPacket.Value.PacketId + " cp:" + currentPacketId);
                                m_orderedReceivedMap[channel].Remove(udpPacket.Key);
                            }
                        }
                    }
                }

                // and now process them 
                foreach (var packet in toProcess)
                {
                    onRecv(packet.RawData, packet.RawData.Length);
                }
            }
        }

        private bool onUDPAcknowledge(byte controlByte, byte[] in_data, int in_lengthOfData)
        {
            // we always want to ack anything we get back 
            byte[] cutOffData = null;

            if (controlByte == RS2CL_RSMG)
            {
                if (in_lengthOfData < 5)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "RSMG packet cannot be smaller than 5 bytes"));
                    return false;
                }

#if BC_DEBUG_RELAY_LOGS_ENABLED
                m_clientRef.Log("ACKING RSMG: " + BitConverter.ToUInt16(in_data, 3).ToString());
#endif

                cutOffData = new byte[3];
                cutOffData[0] = CL2RS_RSMG_ACKNOWLEDGE;
                Buffer.BlockCopy(in_data, 3, cutOffData, 1, 2);
                send(cutOffData, false, false, 0, false);
            }
            else if (controlByte < MAX_PLAYERS)
            {
                if (in_lengthOfData < 5)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "disconnect", "Relay packet cannot be smaller than 5 bytes"));
                    return false;
                }

                // only do this for messages that are reliable
                // read the incoming datas flags based off its header
                bool reliable, ordered;
                int channel, packetId;
                parseHeaderData(in_data, out reliable, out ordered, out channel, out packetId);
                if (reliable)
                {
#if BC_DEBUG_RELAY_LOGS_ENABLED
                    m_clientRef.Log("ACKING RELAY: " + packetId.ToString());
#endif
                    cutOffData = new byte[4];
                    cutOffData[0] = CL2RS_ACKNOWLEDGE;
                    Buffer.BlockCopy(in_data, 3, cutOffData, 1, 2);
                    cutOffData[3] = controlByte;
                    send(cutOffData, false, false, 0, false);
                }
            }

            return true;
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
                    onRecv(data, data.Length);
                    // schedule the next receive operation once reading is done:
                    udpClient.BeginReceive(new AsyncCallback(onUDPRecv), udpClient);
                }
            }
            catch (Exception e)
            {
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
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
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
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
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
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
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError("Server Closed Connection")));
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
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
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
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError("Server Closed Connection")));
                    return;
                }

                // Increment the number of bytes we've read. If there's still more to get, get them
                m_tcpBytesRead += read;
                if (m_tcpBytesRead < m_tcpBytesToRead)
                {
                    //m_clientRef.Log("m_tcpBytesRead < m_tcpBuffer.Length " + m_tcpBytesRead + " " + m_tcpBytesToRead);
                    m_tcpStream.BeginRead(m_tcpReadBuffer, m_tcpBytesRead, m_tcpBytesToRead - m_tcpBytesRead, onTCPFinishRead, null);
                    return;
                }

                // Should be exactly the right number read now.
                if (m_tcpBytesRead != m_tcpBytesToRead)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError("Incorrect Bytes Read " + m_tcpBytesRead + " " + m_tcpBytesToRead)));
                    return;
                }

                // Handle the message
                onRecv(m_tcpReadBuffer, m_tcpBytesToRead + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY);

                // read the next header
                m_tcpBytesToRead = 0;

                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);
            }
            catch (Exception e)
            {
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
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
                    m_clientRef.Log("Starting TCP connect ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
                    await m_tcpClient.ConnectAsync(host, port);
                }
                catch (Exception e)
                {
                    addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
                    return false;
                }
                return true;
            });

            if (success)
            {
                m_tcpStream = m_tcpClient.GetStream();
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "connect", ""));
                // read the next header
                m_tcpBytesToRead = 0;
                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);

                m_clientRef.Log("Connected! ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
            }
        }

        private void initUDPConnection()
        {
#if !DOT_NET
            m_udpClient = new UdpClient();
#endif

            // init packet id list
            m_sendPacketId.Clear();
            m_currentReceivedPacketId.Clear();
            lock (m_reliableSentMap)
            {
                m_reliableSentMap.Clear();
                for (int i = 0; i < MAX_CHANNELS; ++i)
                {
                    m_sendPacketId.Add(new Dictionary<bool, int>());
                    m_sendPacketId[i][true] = 0;
                    m_sendPacketId[i][false] = 0;

                    m_currentReceivedPacketId.Add(new Dictionary<bool, Dictionary<int, int>>());
                    m_currentReceivedPacketId[i][true] = new Dictionary<int, int>();
                    m_currentReceivedPacketId[i][false] = new Dictionary<int, int>();

                    m_reliableSentMap.Add(new Dictionary<int, UDPPacket>());
                    m_orderedReceivedMap.Add(new Dictionary<KeyValuePair<int, int>, UDPPacket>());
                }
            }
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
                addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "error", buildRSRequestError(e.ToString())));
            }
        }

        private void OnUDPConnected(object sender, SocketAsyncEventArgs args)
        {
            m_udpClient.BeginReceive(new AsyncCallback(onUDPRecv), m_udpClient);
            addRSCommandResponse(new RSCommandResponse(ServiceName.Relay.Value, "connect", ""));
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

        private const int MAX_RELIABLE_RESEND_INTERVAL = 500;
        private const short MAX_PACKET_ID = 4096;
        private const short MAX_CHANNELS = 4;

        private string m_ownerId = "";
        private Dictionary<string, int> m_profileIdToNetId = new Dictionary<string, int>();
        private Dictionary<int, string> m_netIdToProfileId = new Dictionary<int, string>();
        private int m_netId = INVALID_NET_ID;

        public string GetOwnerProfileId()
        {
            return m_ownerId;
        }

        public string GetProfileIdForNetId(short netId)
        {
            return m_netIdToProfileId[(int)netId];
        }

        public short GetNetIdForProfileId(string profileId)
        {
            if (m_profileIdToNetId.ContainsKey(profileId))
            {
                return (short)m_profileIdToNetId[profileId];
            }
            return (short)INVALID_NET_ID;
        }

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
        // packetIds per channel, and reliableness
        private List<Dictionary<bool, int>> m_sendPacketId = new List<Dictionary<bool, int>>();

        // current index of received, packetIds per channel, per reliable, per net id,  
        private List<Dictionary<bool, Dictionary<int, int>>> m_currentReceivedPacketId = new List<Dictionary<bool, Dictionary<int, int>>>();

        // reliable send queue.. per channel, per packet id look up map, items are removed thus keeping a thing lookup
        private List<Dictionary<int, UDPPacket>> m_reliableSentMap = new List<Dictionary<int, UDPPacket>>();

        // ordered received queue... per channel, per net id, per packet id, 
        private List<Dictionary<KeyValuePair<int, int>, UDPPacket>> m_orderedReceivedMap = new List<Dictionary<KeyValuePair<int, int>, UDPPacket>>();

        // History of received RSMG so we don't duplicate events
        private List<int> m_rsmgHistory = new List<int>();
        // end 

        private bool m_resendConnectRequest = false;
        private DateTime m_lastConnectResendTime = DateTime.Now;

        private const int CONTROL_BYTE_HEADER_LENGTH = 1;
        private const int SIZE_OF_RELIABLE_FLAGS = 2;

        private BrainCloudClient m_clientRef;
        private long m_sentPing = DateTime.Now.Ticks;
        private byte[] DISCONNECT_ARR = { CL2RS_DISCONNECT };
        private byte[] CONNECT_ARR = { CL2RS_CONNECTION };

        // success callbacks
        private SuccessCallback m_connectedSuccessCallback = null;
        private FailureCallback m_connectionFailureCallback = null;
        private object m_connectedObj = null;

        private RelayCallback m_registeredRelayCallback = null;
        private RelaySystemCallback m_registeredSystemCallback = null;
        private List<RSCommandResponse> m_queuedRSCommands = new List<RSCommandResponse>();
        private struct RSCommandResponse
        {
            public RSCommandResponse(string in_service, string in_op, string in_msg, byte[] in_data = null, bool in_isRSMG = false)
            {
                Service = in_service;
                Operation = in_op;
                JsonMessage = in_msg;
                RawData = in_data;
                IsRSMG = in_isRSMG;
            }
            public string Service { get; set; }
            public string Operation { get; set; }
            public string JsonMessage { get; set; }
            public byte[] RawData { get; set; }
            public bool IsRSMG { get; set; }
        }

        private class UDPPacket
        {
            public UDPPacket(byte[] in_data, int in_channel, int in_packetId, int in_netId, bool in_reliable)
            {
                LastTimeSent = DateTime.Now;
                TimeInterval = in_channel <= 1 ? 50 : in_channel == 2 ? 150 : 250;// ms
                RawData = in_data;
                ChannelId = in_channel;
                PacketId = in_packetId;
                NetId = in_netId;
                Reliable = in_reliable;
            }

            public void UpdateTimeIntervalSent()
            {
                LastTimeSent = DateTime.Now;
                TimeInterval = Math.Min((int)(TimeInterval * 1.25f), MAX_RELIABLE_RESEND_INTERVAL);
            }
            public DateTime LastTimeSent { get; private set; }
            public int TimeInterval { get; private set; }
            public byte[] RawData { get; private set; }
            public int ChannelId { get; private set; }
            public int PacketId { get; private set; }
            public int NetId { get; private set; }
            public bool Reliable { get; private set; }
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