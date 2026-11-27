// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Runtime.CompilerServices;

// Lets the unit tests check ROOM_READY parsing without a server.
[assembly: InternalsVisibleTo("UnitTest")]

namespace BrainCloud
{
    using System;
    using System.Collections.Generic;
    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;

    public struct RelayConnectOptions
    {
        public bool ssl;
        public string host;
        public int port;
        public string passcode;
        public string lobbyId;

        public RelayConnectOptions(bool in_ssl, string in_host, int in_port, string in_passcode, string in_lobbyId)
        {
            ssl = in_ssl;
            host = in_host;
            port = in_port;
            passcode = in_passcode;
            lobbyId = in_lobbyId;
        }
    }

    public class BrainCloudRelay
    {
        public const ulong TO_ALL_PLAYERS = 0x000000FFFFFFFFFF;
        public const int MAX_PLAYERS = 40;
        public const int CHANNEL_HIGH_PRIORITY_1 = 0;
        public const int CHANNEL_HIGH_PRIORITY_2 = 1;
        public const int CHANNEL_NORMAL_PRIORITY = 2;
        public const int CHANNEL_LOW_PRIORITY = 3;

        // Why ConnectFromRoomReady cannot use an event, as reported to the failure callback
        private const string ROOM_READY_ERROR_PREFIX = "Relay: cannot connect from ROOM_READY: ";
        private const string ROOM_READY_NOT_AN_OBJECT = "no ROOM_READY event was given, or it is not a JSON object";
        private const string ROOM_READY_WRONG_EVENT = "expected a ROOM_READY event, not ";
        private const string ROOM_READY_NO_CONNECT_DATA = "the event has no connectData";
        private const string ROOM_READY_NO_ADDRESS = "connectData has no address or ports";
        private const string ROOM_READY_NO_SECURE_WEBSOCKET = "a secure WebSocket was asked for, but this server offers none (secure relay connections are not enabled for this app, or not available from this host)";
        private const string ROOM_READY_BAD_TYPE = "the preferred connection type must be WEBSOCKET, TCP or UDP, not ";
        private const string ROOM_READY_NO_PORT = "the server offers no {0} port (it offers {1})";
        private const string ROOM_READY_NO_PASSCODE = "the event has no passcode";
        private const string ROOM_READY_NO_LOBBY_ID = "the event has no lobbyId";

        /// <summary>
        /// 
        /// </summary>
        internal BrainCloudRelay(RelayComms in_comms, BrainCloudClient in_client)
        {
            m_commsLayer = in_comms;
            m_clientRef = in_client;
        }

        /// <summary>
        /// Use Ping() in order to properly calculate the Last ping received
        /// </summary>
        public long LastPing { get { return m_commsLayer.Ping; } }

        /// <summary>
        /// et the lobby's owner profile Id.
        /// </summary>
        public string OwnerProfileId { get { return m_commsLayer.GetOwnerProfileId(); } }

        /// <summary>
        /// et the lobby's owner profile Id.
        /// </summary>
        public string GetOwnerProfileId()
        {
            return m_commsLayer.GetOwnerProfileId();
        }

        /// <summary>
        /// Returns the profileId associated with a netId.
        /// </summary>
        public string GetProfileIdForNetId(short netId)
        {
            return m_commsLayer.GetProfileIdForNetId(netId);
        }

        /// <summary>
        /// Returns the netId associated with a profileId.
        /// </summary>
        public short GetNetIdForProfileId(string profileId)
        {
            return m_commsLayer.GetNetIdForProfileId(profileId);
        }

        /// <summary>
        /// et the lobby's owner RTT connection Id.
        /// </summary>
        public string OwnerCxId { get { return m_commsLayer.GetOwnerCxId(); } }

        /// <summary>
        /// et the lobby's owner profile Id.
        /// </summary>
        public string GetOwnerCxId()
        {
            return m_commsLayer.GetOwnerCxId();
        }

        /// <summary>
        /// Returns the RTT connection Id associated with a netId.
        /// </summary>
        public string GetCxIdForNetId(short netId)
        {
            return m_commsLayer.GetCxIdForNetId(netId);
        }

        /// <summary>
        /// Returns the netId associated with an RTT connection Id.
        /// </summary>
        public short GetNetIdForCxId(string cxId)
        {
            return m_commsLayer.GetNetIdForCxId(cxId);
        }

        /// <summary>
        /// Start off a connection, based off connection type to brainClouds Relay Servers.  Connect options come in from "ROOM_ASSIGNED" lobby callback
        ///
        /// With RelayConnectionType.DIRECT (ROOM_READY connectData type "direct"), only in_options.lobbyId
        /// is used: players connect over the sockets the direct connect attempt established.
        /// </summary>
        /// <param name="in_connectionType"></param>
        /// <param name="in_options"></param>
        /// <param name="in_success"></param>
        /// <param name="in_failure"></param>
        /// <param name="cb_object"></param>
        public void Connect(RelayConnectionType in_connectionType, RelayConnectOptions in_options, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            m_commsLayer.Connect(in_connectionType, in_options, in_success, in_failure, cb_object);
        }

        /// <summary>
        /// True while connecting or connected peer to peer, without a relay server.
        /// </summary>
        public bool IsDirect { get { return m_commsLayer.ConnectionType == RelayConnectionType.DIRECT; } }

        /// <summary>
        /// The current transport: WEBSOCKET, TCP or UDP via a relay server, DIRECT peer to peer, or
        /// INVALID when not connecting or connected.
        /// </summary>
        public RelayConnectionType ConnectionType { get { return m_commsLayer.ConnectionType; } }

        /// <summary>
        /// Connects using a lobby's ROOM_READY event, reading the address, port, passcode and lobby id
        /// from it, for whichever kind of server the lobby launched:
        /// <list type="bullet">
        /// <item>Direct (connectData type "direct", see EnableDirectConnect): peer to peer as DIRECT.
        /// in_preferredType and in_useSsl do not apply; traffic is protected with the lobby's cipher.</item>
        /// <item>brainCloud relay and Edgegap: the port for in_preferredType, with no fallback to
        /// another type. With in_useSsl and WEBSOCKET, the server's secureUrl.</item>
        /// <item>GameLift and i3D: their one port, over WebSocket, whatever type was preferred.</item>
        /// </list>
        /// in_useSsl applies only to WebSocket and never falls back to insecure: the connect fails if
        /// the server offers no secure WebSocket (Edgegap, GameLift, i3D, or relay without it enabled).
        /// An unusable event is reported to in_failure (403, RS_CLIENT_ERROR) before this returns.
        /// </summary>
        /// <param name="in_preferredType">WEBSOCKET, TCP or UDP, where the server offers a choice.</param>
        /// <param name="in_useSsl">Use a secure WebSocket (wss) when connecting over WebSocket.</param>
        /// <param name="in_roomReadyJson">The ROOM_READY event as the lobby callback received it.</param>
        /// <param name="in_success">As for Connect.</param>
        /// <param name="in_failure">As for Connect; also called when the event is unusable.</param>
        /// <param name="cb_object">As for Connect.</param>
        public void ConnectFromRoomReady(RelayConnectionType in_preferredType, bool in_useSsl, string in_roomReadyJson, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            ConnectFromRoomReady(in_preferredType, in_useSsl, parseEvent(in_roomReadyJson), in_success, in_failure, cb_object);
        }

        /// <summary>
        /// ConnectFromRoomReady for an already-parsed event.
        /// </summary>
        /// <param name="in_preferredType">WEBSOCKET, TCP or UDP, where the server offers a choice.</param>
        /// <param name="in_useSsl">Use a secure WebSocket (wss) when connecting over WebSocket.</param>
        /// <param name="in_roomReady">The parsed ROOM_READY event, or its "data" object.</param>
        /// <param name="in_success">As for Connect.</param>
        /// <param name="in_failure">As for Connect; also called when the event is unusable.</param>
        /// <param name="cb_object">As for Connect.</param>
        public void ConnectFromRoomReady(RelayConnectionType in_preferredType, bool in_useSsl, Dictionary<string, object> in_roomReady, SuccessCallback in_success = null, FailureCallback in_failure = null, object cb_object = null)
        {
            RelayConnectionType connectionType;
            RelayConnectOptions options;
            string error;
            if (!TryGetConnectOptionsFromRoomReady(in_preferredType, in_useSsl, in_roomReady, out connectionType, out options, out error))
            {
                if (m_clientRef.LoggingEnabled)
                {
                    m_clientRef.Log(ROOM_READY_ERROR_PREFIX + error);
                }
                if (in_failure != null)
                {
                    // Same status, reason and shape as the relay's own client-side errors
                    var json = new Dictionary<string, object>();
                    json["status"] = StatusCodes.FORBIDDEN;
                    json["reason_code"] = ReasonCodes.RS_CLIENT_ERROR;
                    json["status_message"] = ROOM_READY_ERROR_PREFIX + error;
                    json["severity"] = "ERROR";
                    in_failure(StatusCodes.FORBIDDEN, ReasonCodes.RS_CLIENT_ERROR, m_clientRef.SerializeJson(json), cb_object);
                }
                return;
            }
            Connect(connectionType, options, in_success, in_failure, cb_object);
        }

        /// <summary>
        /// What ConnectFromRoomReady would connect with, without connecting, e.g. to log or adjust it
        /// before calling Connect. Same rules as ConnectFromRoomReady.
        /// </summary>
        /// <param name="in_preferredType">WEBSOCKET, TCP or UDP, where the server offers a choice.</param>
        /// <param name="in_useSsl">Use a secure WebSocket (wss) when connecting over WebSocket.</param>
        /// <param name="in_roomReadyJson">The ROOM_READY event as the lobby callback received it.</param>
        /// <param name="out_connectionType">The transport to pass to Connect.</param>
        /// <param name="out_options">The options to pass to Connect.</param>
        /// <param name="out_error">Why the event is unusable, or null.</param>
        /// <returns>True when the event is usable.</returns>
        public bool TryGetConnectOptionsFromRoomReady(RelayConnectionType in_preferredType, bool in_useSsl, string in_roomReadyJson,
            out RelayConnectionType out_connectionType, out RelayConnectOptions out_options, out string out_error)
        {
            return TryGetConnectOptionsFromRoomReady(in_preferredType, in_useSsl, parseEvent(in_roomReadyJson),
                out out_connectionType, out out_options, out out_error);
        }

        /// <summary>
        /// TryGetConnectOptionsFromRoomReady for an already-parsed event.
        /// </summary>
        /// <param name="in_preferredType">WEBSOCKET, TCP or UDP, where the server offers a choice.</param>
        /// <param name="in_useSsl">Use a secure WebSocket (wss) when connecting over WebSocket.</param>
        /// <param name="in_roomReady">The parsed ROOM_READY event, or its "data" object.</param>
        /// <param name="out_connectionType">The transport to pass to Connect.</param>
        /// <param name="out_options">The options to pass to Connect.</param>
        /// <param name="out_error">Why the event is unusable, or null.</param>
        /// <returns>True when the event is usable.</returns>
        public bool TryGetConnectOptionsFromRoomReady(RelayConnectionType in_preferredType, bool in_useSsl, Dictionary<string, object> in_roomReady,
            out RelayConnectionType out_connectionType, out RelayConnectOptions out_options, out string out_error)
        {
            out_error = ResolveRoomReady(in_preferredType, in_useSsl, in_roomReady, out out_connectionType, out out_options);
            return out_error == null;
        }

        /// The event as a JSON object, or null when it is not one.
        private static Dictionary<string, object> parseEvent(string in_json)
        {
            try
            {
                return JsonReader.Deserialize(in_json) as Dictionary<string, object>;
            }
            catch (Exception)
            {
                // The caller reports null as an unusable event
                return null;
            }
        }

        /// <summary>
        /// How to connect from a ROOM_READY event (whole, or its "data" object). Returns null and sets
        /// the outs, or returns why the event is unusable.
        /// </summary>
        internal static string ResolveRoomReady(RelayConnectionType in_preferredType, bool in_useSsl, Dictionary<string, object> in_roomReady,
            out RelayConnectionType out_connectionType, out RelayConnectOptions out_options)
        {
            out_connectionType = in_preferredType;
            out_options = new RelayConnectOptions();
            if (in_roomReady == null) return ROOM_READY_NOT_AN_OBJECT;

            string operation = valueOf(in_roomReady, "operation") as string;
            if (operation != null && operation != "ROOM_READY") return ROOM_READY_WRONG_EVENT + operation;
            Dictionary<string, object> data = valueOf(in_roomReady, "data") as Dictionary<string, object> ?? in_roomReady;

            var connectData = valueOf(data, "connectData") as Dictionary<string, object>;
            if (connectData == null) return ROOM_READY_NO_CONNECT_DATA;
            if (valueOf(connectData, "type") as string == "direct")
            {
                // Peer to peer: no server, address or passcode, only the lobby id
                out_connectionType = RelayConnectionType.DIRECT;
                out_options.lobbyId = valueOf(data, "lobbyId") as string;
                if (string.IsNullOrEmpty(out_options.lobbyId)) return ROOM_READY_NO_LOBBY_ID;
                return null;
            }

            string address = valueOf(connectData, "address") as string;
            var ports = valueOf(connectData, "ports") as Dictionary<string, object>;
            if (string.IsNullOrEmpty(address) || ports == null) return ROOM_READY_NO_ADDRESS;

            int port;
            if (tryPort(ports, "gamelift", out port) || tryPort(ports, "i3d", out port))
            {
                // These hosts offer the relay on one port, over WebSocket without SSL
                if (in_useSsl) return ROOM_READY_NO_SECURE_WEBSOCKET;
                out_connectionType = RelayConnectionType.WEBSOCKET;
                out_options.host = address;
                out_options.port = port;
            }
            else if (in_useSsl && in_preferredType == RelayConnectionType.WEBSOCKET)
            {
                // A complete wss:// URL, used as given. Only brainCloud relays send one: a wss port on
                // a bare IP address (Edgegap) would not match its certificate.
                string secureUrl = valueOf(connectData, "secureUrl") as string;
                if (string.IsNullOrEmpty(secureUrl)) return ROOM_READY_NO_SECURE_WEBSOCKET;
                out_options.host = secureUrl;
                out_options.port = 0;
                out_options.ssl = true;
            }
            else
            {
                string key;
                switch (in_preferredType)
                {
                    case RelayConnectionType.WEBSOCKET: key = "ws"; break;
                    case RelayConnectionType.TCP: key = "tcp"; break;
                    case RelayConnectionType.UDP: key = "udp"; break;
                    default: return ROOM_READY_BAD_TYPE + in_preferredType;
                }
                if (!tryPort(ports, key, out port))
                {
                    return string.Format(ROOM_READY_NO_PORT, key, string.Join(", ", new List<string>(ports.Keys).ToArray()));
                }
                out_options.host = address;
                out_options.port = port;
            }

            out_options.passcode = valueOf(data, "passcode") as string;
            out_options.lobbyId = valueOf(data, "lobbyId") as string;
            // The relay checks both when it accepts the connection
            if (string.IsNullOrEmpty(out_options.passcode)) return ROOM_READY_NO_PASSCODE;
            if (string.IsNullOrEmpty(out_options.lobbyId)) return ROOM_READY_NO_LOBBY_ID;
            return null;
        }

        private static object valueOf(Dictionary<string, object> in_map, string in_key)
        {
            object value;
            return in_map.TryGetValue(in_key, out value) ? value : null;
        }

        private static bool tryPort(Dictionary<string, object> in_ports, string in_key, out int out_port)
        {
            out_port = 0;
            object value = valueOf(in_ports, in_key);
            if (value == null) return false;
            try
            {
                out_port = Convert.ToInt32(value);
            }
            catch (Exception)
            {
                return false;
            }
            return out_port > 0 && out_port <= 65535;
        }

        /// <summary>
        /// Disables relay event for this session.
        /// </summary>
        public void Disconnect()
        {
            m_commsLayer.Disconnect();
        }

        /// <summary>
        /// Opts this player in to direct connect: players in a small relay lobby connect peer to peer
        /// instead of through a relay server. Off by default; always off on WebGL.
        ///
        /// Call once after initializing brainCloud. It lasts through RTT reconnects and logins (even of
        /// another player) until a wrapper Init(). While on, each RTT connect tests the network and
        /// reports it; a lobby tries direct connect only if every member is reachable. Direct connect
        /// shares IP addresses within the lobby, so a game may tie it to a player setting (then set it
        /// again after each login). Turning it off withdraws the report, so the relay is used.
        /// </summary>
        public void EnableDirectConnect(bool in_enable)
        {
#if UNITY_WEBGL
            // No UDP in a browser
            in_enable = false;
#endif
            m_clientRef.DirectConnectProbe.Enabled = in_enable;
        }

        /// <summary>
        /// Returns true if this player takes part in direct connect
        /// </summary>
        public bool IsDirectConnectEnabled()
        {
            return m_clientRef.DirectConnectProbe.Enabled;
        }

        /// <summary>
        /// Whether direct connect offers this player's local-network addresses, so two players behind
        /// the same router can connect; without them they usually use the relay. On by default except
        /// on iOS, where it may prompt for local network access: enable it there only if the game
        /// already has that permission (NSLocalNetworkUsageDescription in Info.plist).
        /// </summary>
        public void SetDirectConnectLocalAddresses(bool in_include)
        {
            m_clientRef.DirectConnector.IncludeLocalAddresses = in_include;
        }

        /// <summary>
        /// Returns true if direct connect offers this player's local-network addresses
        /// </summary>
        public bool GetDirectConnectLocalAddresses()
        {
            return m_clientRef.DirectConnector.IncludeLocalAddresses;
        }
        
        /// <summary>
        /// Terminate the match instance by the owner.
        /// </summary>
        /// <param name="json">payload data sent in JSON format. It will be relayed to other connnected players.</param>
        public void EndMatch(Dictionary<string, object> json)
        {
            m_commsLayer.EndMatch(json);
        }

        /// <summary>
        /// Is Connected
        /// </summary>
        public bool IsConnected()
        {
            return m_commsLayer.IsConnected();
        }

        /// <summary>
        /// Register callback for relay messages coming from peers on the main thread
        /// </summary>
        public void RegisterRelayCallback(RelayCallback in_callback)
        {
            m_commsLayer.RegisterRelayCallback(in_callback);
        }

        /// <summary>
        /// Deregister the relay callback
        /// </summary>
        public void DeregisterRelayCallback()
        {
            m_commsLayer.DeregisterRelayCallback();
        }

        /// <summary>
        /// Register callback for RelayServer system messages.
        /// 
        /// # CONNECT
        /// Received when a new member connects to the server.
        /// {
        ///   op: "CONNECT",
        ///   profileId: "...",
        ///   ownerId: "...",
        ///   netId: #
        /// }
        /// 
        /// # NET_ID
        /// Receive the Net Id assossiated with a profile Id. This is
        /// sent for each already connected members once you
        /// successfully connected.
        /// {
        ///   op: "NET_ID",
        ///   profileId: "...",
        ///   netId: #
        /// }
        /// 
        /// # DISCONNECT
        /// Received when a member disconnects from the server.
        /// {
        ///   op: "DISCONNECT",
        ///   profileId: "..."
        /// }
        /// 
        /// # MIGRATE_OWNER
        /// If the owner left or never connected in a timely manner,
        /// the relay-server will migrate the role to the next member
        /// with the best ping. If no one else is currently connected
        /// yet, it will be transferred to the next member in the
        /// lobby members' list. This last scenario can only occur if
        /// the owner connected first, then quickly disconnected.
        /// Leaving only unconnected lobby members.
        /// {
        ///   op: "MIGRATE_OWNER",
        ///   profileId: "..."
        /// }
        /// </summary>
        public void RegisterSystemCallback(RelaySystemCallback in_callback)
        {
            m_commsLayer.RegisterSystemCallback(in_callback);
        }

        /// <summary>
        /// Deregister the relay callback
        /// </summary>
        public void DeregisterSystemCallback()
        {
            m_commsLayer.DeregisterSystemCallback();
        }

        /// <summary>
        /// Send a packet to peer(s)
        /// </summary>
        /// <param in_data="message to be sent"></param>
        /// <param to_netId="the net id to send to, BrainCloudRelay.TO_ALL_PLAYERS to relay to all"></param>
        /// <param in_reliable="send this reliably or not"></param>
        /// <param in_ordered="received this ordered or not"></param>
        /// <param in_channel="0,1,2,3 (max of four channels)">
        /// CHANNEL_HIGH_PRIORITY_1 = 0;
        /// CHANNEL_HIGH_PRIORITY_2 = 1;
        /// CHANNEL_NORMAL_PRIORITY = 2;
        /// CHANNEL_LOW_PRIORITY = 3;
        /// </param>
        public void Send(byte[] in_data, ulong to_netId, bool in_reliable = true, bool in_ordered = true, int in_channel = 0)
        {
            if (to_netId == TO_ALL_PLAYERS)
            {
                SendToAll(in_data, in_reliable, in_ordered, in_channel);
            }
            else if (to_netId >= MAX_PLAYERS)
            {
                // Error. Invalid net id
                string error = "Invalid NetId: " + to_netId.ToString();
                m_commsLayer.QueueError(error);
            }
            else
            {
                ulong playerMask = 1ul << (int)to_netId;
                m_commsLayer.Send(in_data, playerMask, in_reliable, in_ordered, in_channel);
            }
        }

        /// <summary>
        /// Send a packet to any players by using a mask
        /// </summary>
        /// <param in_data="message to be sent"></param>
        /// <param in_playerMask="Mask of the players to send to. 0001 = netId 0, 0010 = netId 1, etc. If you pass ALL_PLAYER_MASK you will be included and you will get an echo for your message. Use sendToAll instead, you will be filtered out. You can manually filter out by : ALL_PLAYER_MASK &= ~(1 << myNetId)"></param>
        /// <param in_reliable="send this reliably or not"></param>
        /// <param in_ordered="received this ordered or not"></param>
        /// <param in_channel="0,1,2,3 (max of four channels)">
        /// CHANNEL_HIGH_PRIORITY_1 = 0;
        /// CHANNEL_HIGH_PRIORITY_2 = 1;
        /// CHANNEL_NORMAL_PRIORITY = 2;
        /// CHANNEL_LOW_PRIORITY = 3;
        /// </param>
        public void SendToPlayers(byte[] in_data, ulong in_playerMask, bool in_reliable = true, bool in_ordered = true, int in_channel = 0)
        {
            m_commsLayer.Send(in_data, in_playerMask, in_reliable, in_ordered, in_channel);
        }

        /// <summary>
        /// Send a packet to all except yourself
        /// </summary>
        /// <param in_data="message to be sent"></param>
        /// <param in_reliable="send this reliably or not"></param>
        /// <param in_ordered="received this ordered or not"></param>
        /// <param in_channel="0,1,2,3 (max of four channels)">
        /// CHANNEL_HIGH_PRIORITY_1 = 0;
        /// CHANNEL_HIGH_PRIORITY_2 = 1;
        /// CHANNEL_NORMAL_PRIORITY = 2;
        /// CHANNEL_LOW_PRIORITY = 3;
        /// </param>
        public void SendToAll(byte[] in_data, bool in_reliable = true, bool in_ordered = true, int in_channel = 0)
        {
            var myProfileId = m_clientRef.AuthenticationService.ProfileId;
            var myNetId = GetNetIdForProfileId(myProfileId);

            ulong myBit = 1ul << (int)myNetId;
            ulong myInvertedBits = ~myBit;
            ulong playerMask = TO_ALL_PLAYERS & myInvertedBits;
            m_commsLayer.Send(in_data, playerMask, in_reliable, in_ordered, in_channel);
        }

        /// <summary>
        /// Set the ping interval.
        /// </summary>
        public void SetPingInterval(float in_interval)
        {
            m_commsLayer.SetPingInterval(in_interval);
        }

        #region private
        /// <summary>
        /// Reference to the Relay Comms
        /// </summary>
        private RelayComms m_commsLayer;

        /// <summary>
        /// Reference to the brainCloud client object
        /// </summary>
        private BrainCloudClient m_clientRef;
        #endregion
    }
}
