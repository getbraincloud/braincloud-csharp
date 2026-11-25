// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>
    /// What a relay transport reports back to RelayComms. Calls can come from any thread;
    /// RelayComms queues them and handles them on the main thread in Update().
    /// </summary>
    internal interface IRelayTransportListener
    {
        BrainCloudClient Client { get; }

        /// True once the relay has accepted this client. Socket errors are not reported before then.
        bool IsRelayConnected { get; }

        bool EndMatchRequested { get; }

        void OnTransportConnected();

        /// The data includes the 2-byte length prefix the relay protocol puts on every packet.
        void OnTransportData(byte[] data, int length);

        void OnTransportError(string message);
    }

    /// <summary>
    /// Where the bytes of a relay connection go: a WebSocket, TCP or UDP connection to a relay server.
    /// </summary>
    internal interface IRelayTransport
    {
        void Connect(string host, int port, bool sslEnabled);

        /// False until the socket exists.
        bool CanSend { get; }

        /// Sends one packet, already length-prefixed. May throw.
        void Send(byte[] data);

        void Close();
    }

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
            if (m_webSocket != null)
            {
                m_webSocket.OnClose -= WebSocket_OnClose;
                m_webSocket.OnOpen -= Websocket_OnOpen;
                m_webSocket.OnMessage -= WebSocket_OnMessage;
                m_webSocket.OnError -= WebSocket_OnError;
                m_webSocket.Close();
                m_webSocket = null;
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

    internal sealed class RelayTcpTransport : IRelayTransport
    {
        const int SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY = 2;

        private readonly IRelayTransportListener m_listener;
        private TcpClient m_tcpClient = null;
        private NetworkStream m_tcpStream = null;
        private byte[] m_tcpReadBuffer = new byte[ushort.MaxValue + 1];

        // ASync TCP Reads
        private int m_tcpBytesRead = 0; // the ones already processed
        private int m_tcpBytesToRead = 0; // the number to finish reading
        private byte[] m_tcpHeaderReadBuffer = new byte[SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY];

        // ASync TCP Writes
        private object fLock = new object();
        private Queue<byte[]> fToSend = new Queue<byte[]>();

        public RelayTcpTransport(IRelayTransportListener in_listener)
        {
            m_listener = in_listener;
        }

        public bool CanSend
        {
            get { return m_tcpClient != null; }
        }

        public void Connect(string in_host, int in_port, bool in_sslEnabled)
        {
            connectTCPAsync(in_host, in_port);
        }

        public void Send(byte[] in_data)
        {
            tcpWrite(in_data);
        }

        public void Close()
        {
            m_tcpStream?.Dispose();
            m_tcpStream = null;

            if (m_tcpClient != null)
            {
                m_tcpClient.Client.Close(0);
                m_tcpClient.Close();
                fToSend.Clear();
            }
            m_tcpClient = null;
        }

        /// <summary>
        /// Writes the specified message to the stream. Messages are written one at a time, in order.
        /// </summary>
        /// <param name="message"></param>
        private void tcpWrite(byte[] message)
        {
            try
            {
                lock (fLock)
                {
                    fToSend.Enqueue(message);
                    // Start writing only if no write is in progress; otherwise the one in progress
                    // carries on to this message when it finishes.
                    if (fToSend.Count == 1)
                    {
                        writeQueued();
                    }
                }
            }
            catch (Exception e)
            {
                m_listener.OnTransportError(e.ToString());
            }
        }

        /// <summary>
        /// Writes the message at the head of the queue, and the ones after it for as long as writes
        /// complete straight away. A message stays at the head until its write has finished.
        /// Call with fLock held.
        /// </summary>
        private void writeQueued()
        {
            while (fToSend.Count > 0 && m_tcpStream != null)
            {
                byte[] next = fToSend.Peek();
                IAsyncResult result = m_tcpStream.BeginWrite(next, 0, next.Length, tcpFinishWrite, null);
                if (!result.CompletedSynchronously)
                {
                    // tcpFinishWrite carries on when it finishes
                    return;
                }
                // Finished already. Carry on here rather than in tcpFinishWrite, which would go
                // one call deeper for every message queued.
                m_tcpStream.EndWrite(result);
                fToSend.Dequeue();
            }
        }

        private void tcpFinishWrite(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                // writeQueued is handling it
                return;
            }
            try
            {
                if (m_tcpStream == null) return;
                m_tcpStream.EndWrite(result);
                lock (fLock)
                {
                    // Pop the message just written, then write the next
                    if (fToSend.Count > 0)
                    {
                        fToSend.Dequeue();
                    }
                    writeQueued();
                }
            }
            catch (Exception e)
            {
                // Suppress errors from a socket we intentionally closed (disconnect/end-match).
                if (!m_listener.IsRelayConnected) return;
                m_listener.OnTransportError(e.ToString());
            }
        }

        private void onTCPReadHeader(IAsyncResult ar)
        {
            try
            {
                // Read precisely SIZE_OF_HEADER for the length of the following message
                int read = -1;
                if (ar != null && m_tcpStream != null)
                {
                    read = m_tcpStream.EndRead(ar);
                }
                if (read == 0)
                {
                    // Suppress "server closed" if we already disconnected (end-match / voluntary close).
                    if (!m_listener.IsRelayConnected) return;
                    m_listener.OnTransportError("Server Closed Connection");
                    return;
                }

                if (m_tcpStream != null && read == SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY)
                {
                    m_tcpBytesRead = 0;
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(m_tcpHeaderReadBuffer);

                    // from the header that was read, how much should we read after this until the next message ?
                    m_tcpBytesToRead = BitConverter.ToUInt16(m_tcpHeaderReadBuffer, 0);
                    m_tcpBytesToRead -= SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY;

                    m_tcpStream.BeginRead(m_tcpReadBuffer, 2, m_tcpBytesToRead, onTCPFinishRead, null);
                }
            }
            catch (Exception e)
            {
                // Suppress errors from a socket we intentionally closed (disconnect/end-match).
                if (!m_listener.IsRelayConnected) return;
                m_listener.OnTransportError(e.ToString());
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
                    // Suppress "server closed" if we already disconnected (end-match / voluntary close).
                    if (!m_listener.IsRelayConnected) return;
                    m_listener.OnTransportError("Server Closed Connection");
                    return;
                }

                // Increment the number of bytes we've read. If there's still more to get, get them
                m_tcpBytesRead += read;
                if (m_tcpBytesRead < m_tcpBytesToRead)
                {
                    m_tcpStream.BeginRead(m_tcpReadBuffer, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY + m_tcpBytesRead, m_tcpBytesToRead - m_tcpBytesRead, onTCPFinishRead, null);
                    return;
                }

                // Should be exactly the right number read now.
                if (m_tcpBytesRead != m_tcpBytesToRead)
                {
                    m_listener.OnTransportError("Incorrect Bytes Read " + m_tcpBytesRead + " " + m_tcpBytesToRead);
                    return;
                }

                // Handle the message
                m_listener.OnTransportData(m_tcpReadBuffer, m_tcpBytesToRead + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY);

                // read the next header
                m_tcpBytesToRead = 0;

                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);
            }
            catch (Exception e)
            {
                m_listener.OnTransportError(e.ToString());
            }
        }

        private async void connectTCPAsync(string host, int port)
        {
            BrainCloudClient client = m_listener.Client;
            bool success = await Task.Run(async () =>
            {
                try
                {
                    m_tcpClient = new TcpClient();
                    m_tcpClient.NoDelay = true;
                    m_tcpClient.Client.NoDelay = true;
                    if (client.LoggingEnabled)
                    {
                        client.Log("Starting TCP connect ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
                    }
                    await m_tcpClient.ConnectAsync(host, port);
                }
                catch (Exception e)
                {
                    m_listener.OnTransportError(e.ToString());
                    return false;
                }
                return true;
            });

            if (success)
            {
                m_tcpStream = m_tcpClient.GetStream();
                m_listener.OnTransportConnected();
                // read the next header
                m_tcpBytesToRead = 0;
                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);

                if (client.LoggingEnabled)
                {
                    client.Log("Connected! ASYNC " + m_tcpClient.Connected + " s:" + m_tcpClient.Client.Connected);
                }
            }
        }
    }

    internal sealed class RelayUdpTransport : IRelayTransport
    {
        private readonly IRelayTransportListener m_listener;
        private UdpClient m_udpClient = null;

        public RelayUdpTransport(IRelayTransportListener in_listener)
        {
            m_listener = in_listener;
        }

        public bool CanSend
        {
            get { return m_udpClient != null; }
        }

        public void Connect(string host, int port, bool in_sslEnabled)
        {
            Task.Run(() =>
            {
                try
                {
                    m_udpClient = new UdpClient();
                    m_udpClient.Client.Connect(host, port);
                    return true;
                }
                catch (Exception e)
                {
                    m_listener.OnTransportError(e.ToString());
                    return false;
                }
            }).ContinueWith(task =>
            {
                if (task.Result)
                {
                    m_udpClient.BeginReceive(new AsyncCallback(onUDPRecv), m_udpClient);
                    m_listener.OnTransportConnected();
                }
            });
        }

        public void Send(byte[] in_data)
        {
            m_udpClient.SendAsync(in_data, in_data.Length);
        }

        public void Close()
        {
            m_udpClient?.Close();
            m_udpClient = null;
        }

        private void onUDPRecv(IAsyncResult result)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            try
            {
                if (result != null)
                {
                    UdpClient udpClient = result.AsyncState as UdpClient;
                    IPEndPoint source = new IPEndPoint(IPAddress.Any, 0);

                    if (udpClient != null)
                    {
                        // get the actual message and fill out the source:
                        byte[] data = udpClient.EndReceive(result, ref source);
                        m_listener.OnTransportData(data, data.Length);
                        // schedule the next receive operation once reading is done:
                        udpClient.BeginReceive(new AsyncCallback(onUDPRecv), udpClient);
                    }
                }
            }
            catch (Exception e)
            {
                // Suppress errors from a socket we intentionally closed (disconnect/end-match).
                if (!m_listener.IsRelayConnected) return;
                m_listener.OnTransportError(e.ToString());
            }
        }
    }
}
