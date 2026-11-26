// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Threading.Tasks;

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
        private int m_tcpHeaderBytesRead = 0;

        // ASync TCP Writes. fLock also guards m_tcpStream against Close() on another thread.
        private object fLock = new object();
        private Queue<byte[]> fToSend = new Queue<byte[]>();

        // Set by Close(). A connect or read still in flight on this socket then reports nothing,
        // because RelayComms may already have a new transport connecting.
        private volatile bool m_closed = false;

        public RelayTcpTransport(IRelayTransportListener in_listener)
        {
            m_listener = in_listener;
        }

        public bool CanSend
        {
            get { return m_tcpStream != null; }
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
            m_closed = true;
            lock (fLock)
            {
                m_tcpStream?.Dispose();
                m_tcpStream = null;
                fToSend.Clear();
            }

            if (m_tcpClient != null)
            {
                m_tcpClient.Client.Close(0);
                m_tcpClient.Close();
            }
            m_tcpClient = null;
        }

        private void reportError(string in_message)
        {
            if (!m_closed) m_listener.OnTransportError(in_message);
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
                reportError(e.ToString());
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
                lock (fLock)
                {
                    if (m_tcpStream == null) return;
                    m_tcpStream.EndWrite(result);
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
                reportError(e.ToString());
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
                    reportError("Server Closed Connection");
                    return;
                }

                if (m_tcpStream != null && read > 0)
                {
                    // The length prefix can itself arrive in pieces
                    m_tcpHeaderBytesRead += read;
                    if (m_tcpHeaderBytesRead < SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY)
                    {
                        m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, m_tcpHeaderBytesRead, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY - m_tcpHeaderBytesRead, new AsyncCallback(onTCPReadHeader), null);
                        return;
                    }
                    m_tcpHeaderBytesRead = 0;

                    m_tcpBytesRead = 0;

                    // from the header that was read (big-endian), how much should we read after this until the next message ?
                    m_tcpBytesToRead = (m_tcpHeaderReadBuffer[0] << 8) | m_tcpHeaderReadBuffer[1];
                    m_tcpBytesToRead -= SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY;
                    if (m_tcpBytesToRead <= 0)
                    {
                        // Every relay packet has at least its control byte after the length. A
                        // shorter length means the stream is out of step: a read of 0 bytes would
                        // look like the server closing, and a negative one throws.
                        reportError("Bad packet length " + (m_tcpBytesToRead + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY));
                        return;
                    }

                    m_tcpStream.BeginRead(m_tcpReadBuffer, 2, m_tcpBytesToRead, onTCPFinishRead, null);
                }
            }
            catch (Exception e)
            {
                // Suppress errors from a socket we intentionally closed (disconnect/end-match).
                if (!m_listener.IsRelayConnected) return;
                reportError(e.ToString());
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
                    reportError("Server Closed Connection");
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
                    reportError("Incorrect Bytes Read " + m_tcpBytesRead + " " + m_tcpBytesToRead);
                    return;
                }

                // Handle the message
                if (m_closed) return;
                m_listener.OnTransportData(m_tcpReadBuffer, m_tcpBytesToRead + SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY);

                // read the next header
                m_tcpBytesToRead = 0;
                m_tcpHeaderBytesRead = 0;

                m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);
            }
            catch (Exception e)
            {
                reportError(e.ToString());
            }
        }

        private async void connectTCPAsync(string host, int port)
        {
            BrainCloudClient client = m_listener.Client;
            // Set here, not on the worker thread, so a Close() from now on closes this socket
            TcpClient tcpClient;
            try
            {
                tcpClient = new TcpClient();
            }
            catch (Exception e)
            {
                reportError(e.ToString());
                return;
            }
            m_tcpClient = tcpClient;
            bool success = await Task.Run(async () =>
            {
                try
                {
                    tcpClient.NoDelay = true;
                    tcpClient.Client.NoDelay = true;
                    if (client.LoggingEnabled)
                    {
                        client.Log("Starting TCP connect ASYNC " + tcpClient.Connected + " s:" + tcpClient.Client.Connected);
                    }
                    await tcpClient.ConnectAsync(host, port);
                }
                catch (Exception e)
                {
                    reportError(e.ToString());
                    return false;
                }
                return true;
            });

            if (success)
            {
                try
                {
                    lock (fLock)
                    {
                        if (m_closed) return;
                        m_tcpStream = tcpClient.GetStream();
                    }
                    if (m_closed) return;
                    m_listener.OnTransportConnected();
                    // read the next header
                    m_tcpBytesToRead = 0;
                    m_tcpHeaderBytesRead = 0;
                    m_tcpStream.BeginRead(m_tcpHeaderReadBuffer, 0, SIZE_OF_LENGTH_PREFIX_BYTE_ARRAY, new AsyncCallback(onTCPReadHeader), null);

                    if (client.LoggingEnabled)
                    {
                        client.Log("Connected! ASYNC " + tcpClient.Connected + " s:" + tcpClient.Client.Connected);
                    }
                }
                catch (Exception e)
                {
                    // Closed between connecting and here
                    reportError(e.ToString());
                }
            }
        }
    }
}
