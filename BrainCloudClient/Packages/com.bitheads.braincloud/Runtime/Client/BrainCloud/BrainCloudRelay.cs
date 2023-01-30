//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System.Collections.Generic;
using BrainCloud.Internal;

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
        /// Disables relay event for this session.
        /// </summary>
        public void Disconnect()
        {
            m_commsLayer.Disconnect();
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
