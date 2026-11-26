// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;

    internal sealed class RelayWebSocketTransport : IRelayTransport
    {
        private readonly IRelayTransportListener m_listener;
        private BrainCloudWebSocket m_webSocket = null;

        public RelayWebSocketTransport(IRelayTransportListener in_listener)
        {
            m_listener = in_listener;
        }

        public bool CanSend
        {
            get { return m_webSocket != null; }
        }

        public void Connect(string in_host, int in_port, bool in_sslEnabled)
        {
            // A full URL (connectData.secureUrl) is used as given; port is ignored.
            string url = in_host.Contains("://") ? in_host : (in_sslEnabled ? "wss://" : "ws://") + in_host + ":" + in_port;
            m_webSocket = new BrainCloudWebSocket(url);
            m_webSocket.OnClose += WebSocket_OnClose;
            m_webSocket.OnOpen += Websocket_OnOpen;
            m_webSocket.OnMessage += WebSocket_OnMessage;
            m_webSocket.OnError += WebSocket_OnError;
        }

        public void Send(byte[] in_data)
        {
            m_webSocket.SendAsync(in_data);
        }

        public void Close()
        {
            BrainCloudWebSocket webSocket = m_webSocket;
            if (webSocket != null)
            {
                // Forget it first: Close() can be called on a socket still connecting, or one whose
                // connect failed, and nothing that goes wrong closing it may leave it in place
                m_webSocket = null;
                webSocket.OnClose -= WebSocket_OnClose;
                webSocket.OnOpen -= Websocket_OnOpen;
                webSocket.OnMessage -= WebSocket_OnMessage;
                webSocket.OnError -= WebSocket_OnError;
                try
                {
                    webSocket.Close();
                }
                catch (Exception e)
                {
                    if (m_listener.Client.LoggingEnabled)
                    {
                        m_listener.Client.Log("Relay: closing the WebSocket failed: " + e.Message);
                    }
                }
            }
        }

        private void WebSocket_OnClose(BrainCloudWebSocket sender, int code, string reason)
        {
            BrainCloudClient client = m_listener.Client;
            if (client.LoggingEnabled)
            {
                if (m_listener.EndMatchRequested)
                {
                    client.Log("Relay: Connection closed by end match");
                }
                else
                {
                    client.Log("Relay: Connection closed: " + reason);
                }

            }
            m_listener.OnTransportError(reason);
        }

        private void Websocket_OnOpen(BrainCloudWebSocket accepted)
        {
            if (m_listener.Client.LoggingEnabled)
            {
                m_listener.Client.Log("Relay: Connection established.");
            }
            // initial connect call, sets connection requests if not connected
            m_listener.OnTransportConnected();
        }

        private void WebSocket_OnMessage(BrainCloudWebSocket sender, byte[] data)
        {
            m_listener.OnTransportData(data, data.Length);
        }

        private void WebSocket_OnError(BrainCloudWebSocket sender, string message)
        {
            if (m_listener.Client.LoggingEnabled)
            {
                m_listener.Client.Log("Relay Error: " + message);
            }
            m_listener.OnTransportError(message);
        }
    }
}
