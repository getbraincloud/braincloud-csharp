// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
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

    internal sealed class RelayComms : IRelayTransportListener
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
        public const byte CL2RS_ENDMATCH = 6;

        public const byte RS2CL_RSMG = 0;
        public const byte RS2CL_DISCONNECT = 1;
        public const byte RS2CL_RELAY = 2;
        public const byte RS2CL_ACK = 3;
        public const byte RS2CL_PONG = 4;
        #endregion

        private const int MAX_RSMG_HISTORY = 50;

        private const int CONNECT_RESEND_INTERVAL_S = 2;

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
            m_channel = new RelayReliableChannel(() => m_clientRef.LoggingEnabled, message => m_clientRef.Log(message));
            // Made once: a method group passed as a delegate allocates each time, and these run every frame
            m_sendDelegate = send;
            m_queueRelayEventDelegate = queueRelayEvent;
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
            if (!IsConnected() && m_clientRef.IsAuthenticated())
            {
                m_lastRecvTime = m_clientRef.NowMs;
                m_lastNowMS = m_clientRef.NowMs;
                m_endMatchRequested = false;
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
            else if (!m_clientRef.IsAuthenticated())
            {
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log("Relay: Connect called before calling authentication request. Disabling Relay.");
                }

                if (in_failure != null)
                {
                    in_failure(StatusCodes.FORBIDDEN, ReasonCodes.RS_NO_API_SESSION_ERROR, buildRSRequestError("Relay: Connect called before calling authentication request. Disabling Relay."), cb_object);
                }
            }
        }

        /// <summary>
        /// Disables relay event for this session.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected())
            {
                send(buildDisconnectRequest());

                m_channel.ClearTrackedPacketIds();

                // disconnect from the client after the client gets to send it out
                m_queuedForDisconnect = true;
            }
        }

        /// <summary>
        /// Terminate the match instance by the owner.
        /// </summary>
        /// <param name="in_jsonPayload">payload data sent in JSON format. It will be relayed to other connnected players.</param>
        public void EndMatch(Dictionary<string, object> in_jsonPayload)
        {
            if (IsConnected())
            {
                send(buildEndMatchRequest(in_jsonPayload));

                m_endMatchRequested = true;
            }
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
            int packetId = m_channel.NextPacketId(ackIdWithoutPacketId);

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
                ulong ackId = BitConverter.ToUInt64(ackIdData, 0);
                m_channel.TrackReliable(ackId, packetData, in_channel, packetId, m_clientRef.NowMs);
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
            m_netIdToCxId.TryGetValue((int)netId, out string cxId);
            return cxId;
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
                if (m_clientRef.NowMs - m_lastConnectResendTime > CONNECT_RESEND_INTERVAL_S * 1000L)
                {
                    send(buildConnectionRequest());
                    m_lastConnectResendTime = m_clientRef.NowMs;
                }
            }

            // Ping
            long nowMS = m_clientRef.NowMs;
            if (IsConnected())
            {
                if (m_queuedForDisconnect)
                {
                    m_queuedForDisconnect = false;
                    disconnect();
                }
                m_timeSinceLastPingRequest += (int)(nowMS - m_lastNowMS);
                m_lastNowMS = nowMS;

                if (m_timeSinceLastPingRequest >= m_pingInterval)
                {
                    m_timeSinceLastPingRequest = 0;
                    ping();
                }

                // Process reliable resends
                if (m_connectionType == RelayConnectionType.UDP)
                {
                    if (!m_channel.ProcessResends(nowMS, m_sendDelegate))
                    {
                        disconnect();
                        queueErrorEvent("Relay disconnected, too many packet lost");
                    }
                }
            }

            // Check if we timeout
            if (m_connectionType == RelayConnectionType.UDP &&
                nowMS - m_lastRecvTime > 10000)
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
                            m_lastRecvTime = m_clientRef.NowMs;
                            onRecv(evt.data);
                            break;
                        case EventType.SocketError:
                            disconnect();
                            queueErrorEvent(evt.message);
                            break;
                        case EventType.SocketConnected:
                            {
                                m_lastNowMS = m_clientRef.NowMs;
                                m_lastRecvTime = m_clientRef.NowMs;
                                send(buildConnectionRequest());

                                if (m_connectionType == RelayConnectionType.UDP)
                                {
                                    m_resendConnectRequest = true;
                                    m_lastConnectResendTime = m_clientRef.NowMs;
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
                            //When End Match is requested, then the server will close the connection
                            if (m_connectionFailureCallback != null &&
                                !m_endMatchRequested)
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
                                callback(200, ReasonCodes.RS_ENDMATCH_REQUESTED, buildRSRequestError(evt.message), callbackObj);
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
            m_sentPingMs = m_clientRef.NowMs;
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

            byte[] array = concatenateByteArrays(CONNECT_ARR, Encoding.ASCII.GetBytes(m_clientRef.SerializeJson(json)));
            return array;
        }

        private byte[] buildEndMatchRequest(Dictionary<string, object> in_jsonPayload)
        {
            byte[] array = concatenateByteArrays(ENDMATCH_ARR, Encoding.ASCII.GetBytes(m_clientRef.SerializeJson(in_jsonPayload)));
            return array;
        }

        private string buildRSRequestError(string in_statusMessage)
        {
            Dictionary<string, object> json = new Dictionary<string, object>();
            json["status"] = 403;
            json["reason_code"] = ReasonCodes.RS_CLIENT_ERROR;
            json["status_message"] = in_statusMessage;
            json["severity"] = "ERROR";

            return m_clientRef.SerializeJson(json);
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

            m_queuedForDisconnect = false;

            m_transport?.Close();
            m_transport = null;

            // cleanup UDP stuff
            m_channel.Reset();
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
            // early return when there is no socket yet
            if (m_transport == null || !m_transport.CanSend)
                return bMessageSent;
            // actually do the send
            try
            {
                byte[] data = appendSizeBytes(in_data);
                m_transport.Send(data);
                bMessageSent = true;
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
                    m_transport = new RelayWebSocketTransport(this);
                    break;
                case RelayConnectionType.TCP:
                    m_transport = new RelayTcpTransport(this);
                    break;
                case RelayConnectionType.UDP:
                    initUDPConnection();
                    m_transport = new RelayUdpTransport(this);
                    break;
                default:
                    return;
            }
            m_transport.Connect(host, port, sslEnabled);
        }

        #region IRelayTransportListener
        BrainCloudClient IRelayTransportListener.Client
        {
            get { return m_clientRef; }
        }

        bool IRelayTransportListener.IsRelayConnected
        {
            get { return m_bIsConnected; }
        }

        bool IRelayTransportListener.EndMatchRequested
        {
            get { return m_endMatchRequested; }
        }

        void IRelayTransportListener.OnTransportConnected()
        {
            queueSocketConnectedEvent();
        }

        void IRelayTransportListener.OnTransportData(byte[] data, int length)
        {
            queueSocketDataEvent(data, length);
        }

        void IRelayTransportListener.OnTransportError(string message)
        {
            queueErrorEvent(message);
        }
        #endregion

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
                        m_channel.PeerConnected(netId, cxId);
                        m_cxIdToNetId[cxId] = netId;
                        m_netIdToCxId[netId] = cxId;
                        if (cxId == m_clientRef.RTTConnectionID && !m_bIsConnected)
                        {
                            m_netId = netId;
                            m_ownerCxId = parsedDict["ownerCxId"] as string;
                            m_bIsConnected = true;
                            m_lastNowMS = m_clientRef.NowMs;
                            m_resendConnectRequest = false;
                            queueConnectSuccessEvent(jsonMessage);
                        }
                        break;
                    }
                case "NET_ID":
                    {
                        int netId = (int)parsedDict["netId"];
                        string cxId = parsedDict["cxId"] as string;
                        m_channel.PeerConnected(netId, cxId);
                        m_cxIdToNetId[cxId] = netId;
                        m_netIdToCxId[netId] = cxId;

                        int[] packetIdArray = null;

                        if (parsedDict.ContainsKey("orderedPacketIds"))
                        {
                            packetIdArray = parsedDict["orderedPacketIds"] as int[];
                        }

                        if (packetIdArray != null)
                        {
                            for (int channelID = 0; channelID < packetIdArray.Length; channelID++)
                            {
                                int packetID = packetIdArray[channelID];
                                if (packetID != 0)
                                {
                                    m_channel.SetTrackedPacketId(channelID, netId, packetID);
                                    if (m_clientRef.LoggingEnabled)
                                    {
                                        m_clientRef.Log($"Added tracked packetId {packetID} for netID {netId} at channel {channelID}");
                                    }
                                }
                            }
                        }
                        break;
                    }
                case "MIGRATE_OWNER":
                    {
                        m_ownerCxId = parsedDict["cxId"] as string;
                        break;
                    }
                case "DISCONNECT":
                    {
                        string cxId = parsedDict["cxId"] as string;
                        if (cxId == m_clientRef.RTTService.getRTTConnectionID())
                        {
                            // We are the one that got disconnected!
                            disconnect();
                            queueErrorEvent("Disconnected by server");
                            return;
                        }
                        break;
                    }
                case "END_MATCH":
                    {
                        m_endMatchRequested = true;
                        disconnect();
                        break;
                    }
            }

            queueSystemEvent(jsonMessage);
        }

        private void onPong()
        {
            Ping = (m_clientRef.NowMs - m_sentPingMs) * TimeSpan.TicksPerMillisecond;
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

        private void onRelay(byte[] in_data)
        {
            if (m_connectionType == RelayConnectionType.UDP)
            {
                // Ack reliables, always. An ack might have been previously dropped.
                ushort rh = (ushort)((in_data[3] << 8) | in_data[4]);
                if ((rh & RELIABLE_BIT) != 0)
                {
                    sendAck(in_data);
                }

                if (m_channel.Receive(in_data, m_queueRelayEventDelegate) == RelayReliableChannel.ReceiveOutcome.TooManyOutOfOrder)
                {
                    disconnect();
                    queueErrorEvent("Relay disconnected, too many queued out of order packets.");
                }
                return;
            }

            queueRelayEvent((short)(in_data[10] & 0xFF), in_data);
        }

        private void onUDPAcknowledge(byte[] in_data)
        {
            ulong ackId = BitConverter.ToUInt64(in_data, 3);
            m_channel.Acknowledge(ackId);

#if BC_DEBUG_RELAY_LOGS_ENABLED
            if (m_clientRef.LoggingEnabled)
            {
                m_clientRef.Log("RELAY RECV ACK: " + ackId.ToString());
            }
#endif
        }

        private void initUDPConnection()
        {
            // init packet id list
            m_channel.Reset();
            m_rsmgHistory.Clear();
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
        private long m_lastNowMS;
        private int m_timeSinceLastPingRequest = 0;
        private int m_pingInterval = 1000; // one second
        private long m_lastRecvTime;

        private string m_ownerCxId = "";
        private Dictionary<string, int> m_cxIdToNetId = new Dictionary<string, int>();
        private Dictionary<int, string> m_netIdToCxId = new Dictionary<int, string>();
        private int m_netId = INVALID_NET_ID;

        // The connection to the relay server, and the reliability layer over it
        private IRelayTransport m_transport = null;
        private RelayReliableChannel m_channel;
        private Func<byte[], bool> m_sendDelegate;
        private Action<short, byte[]> m_queueRelayEventDelegate;

        const int SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY = 2;

        // Packet history
        private List<int> m_rsmgHistory = new List<int>();

        private bool m_queuedForDisconnect = false;
        private bool m_resendConnectRequest = false;
        private bool m_endMatchRequested = false;
        private long m_lastConnectResendTime;

        private const int CONTROL_BYTE_HEADER_LENGTH = 1;
        private const int SIZE_OF_RELIABLE_FLAGS = 2;

        public const ushort RELIABLE_BIT = 0x8000;
        public const ushort ORDERED_BIT = 0x4000;

        private BrainCloudClient m_clientRef;
        private long m_sentPingMs;
        private byte[] DISCONNECT_ARR = { CL2RS_DISCONNECT };
        private byte[] CONNECT_ARR = { CL2RS_CONNECT };
        private byte[] ENDMATCH_ARR = { CL2RS_ENDMATCH };

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
