// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud.Internal
{
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
}
