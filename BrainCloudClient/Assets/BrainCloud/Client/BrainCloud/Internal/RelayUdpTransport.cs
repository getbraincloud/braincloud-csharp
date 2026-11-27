// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal sealed class RelayUdpTransport : IRelayTransport
    {
        private readonly IRelayTransportListener m_listener;
        private UdpClient m_udpClient = null;
        private readonly object m_lock = new object();

        // Set by Close(): in-flight callbacks then report nothing, as RelayComms may already be
        // connecting a new transport.
        private volatile bool m_closed = false;

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
                UdpClient udpClient = null;
                try
                {
                    udpClient = new UdpClient();
                    udpClient.Client.Connect(host, port);
                    return udpClient;
                }
                catch (Exception e)
                {
                    udpClient?.Close();
                    reportError(e.ToString());
                    return null;
                }
            }).ContinueWith(task =>
            {
                UdpClient udpClient = task.Result;
                if (udpClient == null) return;
                lock (m_lock)
                {
                    if (m_closed)
                    {
                        // Closed while connecting
                        udpClient.Close();
                        return;
                    }
                    m_udpClient = udpClient;
                }
                try
                {
                    udpClient.BeginReceive(new AsyncCallback(onUDPRecv), udpClient);
                }
                catch (Exception e)
                {
                    reportError(e.ToString());
                    return;
                }
                if (!m_closed) m_listener.OnTransportConnected();
            });
        }

        public void Send(byte[] in_data)
        {
            m_udpClient.SendAsync(in_data, in_data.Length);
        }

        public void Close()
        {
            m_closed = true;
            lock (m_lock)
            {
                m_udpClient?.Close();
                m_udpClient = null;
            }
        }

        private void reportError(string in_message)
        {
            if (!m_closed) m_listener.OnTransportError(in_message);
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
                        if (m_closed) return;
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
                reportError(e.ToString());
            }
        }
    }
}
