// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
    /// <summary>
    /// Transport callbacks to RelayComms, from any thread; RelayComms queues them for Update().
    /// </summary>
    internal interface IRelayTransportListener
    {
        BrainCloudClient Client { get; }

        /// True once the relay has accepted this client. Socket errors are not reported before then.
        bool IsRelayConnected { get; }

        bool EndMatchRequested { get; }

        void OnTransportConnected();

        /// data includes the relay's 2-byte length prefix.
        void OnTransportData(byte[] data, int length);

        void OnTransportError(string message);
    }

    /// <summary>
    /// A WebSocket, TCP or UDP connection to a relay server.
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
}
