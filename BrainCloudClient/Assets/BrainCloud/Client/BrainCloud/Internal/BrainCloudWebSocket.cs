
namespace BrainCloud.Internal
{
    using System;
#if DOT_NET
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;
    using System.IO;
    using System.Diagnostics;
#elif UNITY_WEBGL && !UNITY_EDITOR
    using AOT;
    using System.Collections.Generic;
#else
    using BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp;
#endif

    public class BrainCloudWebSocket
    {
#if DOT_NET
        private ClientWebSocket ClientWebSocket;
#elif UNITY_WEBGL && !UNITY_EDITOR
        private NativeWebSocket NativeWebSocket;   
        private static Dictionary<int, BrainCloudWebSocket> webSocketInstances =
        new Dictionary<int, BrainCloudWebSocket>();
#else
        private BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp.WebSocket WebSocket;
#endif

        public BrainCloudWebSocket(string url)
        {
#if DOT_NET
            ClientWebSocket = new ClientWebSocket();
            ConnectClientWebSocketAsync(url);
#elif UNITY_WEBGL && !UNITY_EDITOR
            NativeWebSocket = new NativeWebSocket(url);
            NativeWebSocket.SetOnOpen(NativeSocket_OnOpen);
            NativeWebSocket.SetOnMessage(NativeSocket_OnMessage);
            NativeWebSocket.SetOnError(NativeSocket_OnError);
            NativeWebSocket.SetOnClose(NativeSocket_OnClose);
            webSocketInstances.Add(NativeWebSocket.Id, this);
#else
            WebSocket = new BrainCloud.UnityWebSocketsForWebGL.WebSocketSharp.WebSocket(url);
            WebSocket.ConnectAsync();
            WebSocket.OnOpen += WebSocket_OnOpen;
            WebSocket.OnMessage += WebSocket_OnMessage;
            WebSocket.OnError += WebSocket_OnError;
            WebSocket.OnClose += WebSocket_OnClose;
#endif
        }

#if DOT_NET
        private async void ConnectClientWebSocketAsync(string url)
        {
            bool success = await Task.Run(async () =>
            {
                try
                {
                    await ClientWebSocket.ConnectAsync(new Uri(url), CancellationToken.None);
                }
                catch (Exception e)
                {
                    ClientWebSocket_OnError(e.Message);
                    return false;
                }
                return true;
            });

            if (success && OnOpen != null)
            {
                StartReceivingClientWebSocketAsync();
                ClientWebSocket_OnOpen();
            }
        }

        private async void StartReceivingClientWebSocketAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
                    WebSocketReceiveResult result = null;
                    MemoryStream ms = new MemoryStream();
                    while (ClientWebSocket != null)
                    {
                        do
                        {
                            result = await ClientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage && ClientWebSocket != null);
                        ms.Seek(0, SeekOrigin.Begin);

                        if (!result.EndOfMessage)
                        {
                            // We probably closed the socket
                            break;
                        }

                        ClientWebSocket_OnMessage(ms.ToArray());
                        ms.SetLength(0);
                    }
                }
                catch (ObjectDisposedException e)
                {
                    ClientWebSocket_OnClose(e.Message);
                }
                catch (Exception e)
                {
                    ClientWebSocket_OnError(e.Message);
                }
            });
        }
#endif

        public void Close()
        {
#if DOT_NET
            if (ClientWebSocket == null)
                return;
            ClientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            ClientWebSocket = null;
#elif UNITY_WEBGL && !UNITY_EDITOR
            if (NativeWebSocket == null)
                return;
            webSocketInstances.Remove(NativeWebSocket.Id);
            NativeWebSocket.CloseAsync();
            NativeWebSocket = null;
#else
            if (WebSocket == null)
                return;
            WebSocket.CloseAsync();
            WebSocket.OnOpen -= WebSocket_OnOpen;
            WebSocket.OnMessage -= WebSocket_OnMessage;
            WebSocket.OnError -= WebSocket_OnError;
            WebSocket.OnClose -= WebSocket_OnClose;
            WebSocket = null;
#endif
        }

#if DOT_NET
        private void ClientWebSocket_OnError(string message)
        {
            if (OnError != null)
            {
                OnError(this, message);
            }
        }

        private void ClientWebSocket_OnOpen()
        {
            if (OnOpen != null)
            {
                OnOpen(this);
            }
        }

        private void ClientWebSocket_OnMessage(byte[] data)
        {
            if (OnMessage != null)
            {
                OnMessage(this, data);
            }
        }

        private void ClientWebSocket_OnClose(string message)
        {
            if (OnClose != null)
            {
                OnClose(this, 0, message);
            }
        }

#elif UNITY_WEBGL && !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void NativeSocket_OnOpen(int id)
        {
            if (webSocketInstances.ContainsKey(id) && webSocketInstances[id].OnOpen != null)
            {
                webSocketInstances[id].OnOpen(webSocketInstances[id]);
            }
        }
        
        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void NativeSocket_OnMessage(int id)
        {
            if (webSocketInstances.ContainsKey(id))
            {
                byte[] data = webSocketInstances[id].NativeWebSocket.Receive();
                if (webSocketInstances[id].OnMessage != null)
                {
                    webSocketInstances[id].OnMessage(webSocketInstances[id], data);
                }
            }
        }

        [MonoPInvokeCallback(typeof(Action<int>))]
        public static void NativeSocket_OnError(int id)
        {    
            if (webSocketInstances.ContainsKey(id) && webSocketInstances[id].OnError != null)
            {
                webSocketInstances[id].OnError(webSocketInstances[id], webSocketInstances[id].NativeWebSocket.Error);
            }
        }

        [MonoPInvokeCallback(typeof(Action<int, int>))]
        public static void NativeSocket_OnClose(int code, int id)
        {
            CloseError errorInfo = CloseError.Get(code);
            if (webSocketInstances.ContainsKey(id) && webSocketInstances[id].OnClose != null)
            {
                webSocketInstances[id].OnClose(webSocketInstances[id], errorInfo.Code, errorInfo.Message);
            }
        }
#else
        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            WebSocket.TCPClient.NoDelay = true;
            WebSocket.TCPClient.Client.NoDelay = true;
            if (OnOpen != null)
            {
                OnOpen(this);
            }
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            if (OnMessage != null)
            {
                OnMessage(this, e.RawData);
            }
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            if (OnError != null)
            {
                OnError(this, e.Message);
            }
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            if (OnClose != null)
            {
                OnClose(this, e.Code, e.Reason);
            }
        }
#endif

        public void SendAsync(byte[] packet)
        {
#if DOT_NET
            ClientWebSocket.SendAsync(new ArraySegment<byte>(packet), WebSocketMessageType.Binary, false, CancellationToken.None);
#elif UNITY_WEBGL && !UNITY_EDITOR
            NativeWebSocket.SendAsync(packet);
#else
            WebSocket.SendAsync(packet, null);
#endif
        }

        public void Send(byte[] packet)
        {
#if DOT_NET
            SendAsync(packet);
#elif UNITY_WEBGL && !UNITY_EDITOR
            SendAsync(packet);
#else
            WebSocket.Send(packet);
#endif
        }



        public delegate void OnOpenHandler(BrainCloudWebSocket accepted);

        public delegate void OnMessageHandler(BrainCloudWebSocket sender, byte[] data);

        public delegate void OnErrorHandler(BrainCloudWebSocket sender, string message);

        public delegate void OnCloseHandler(BrainCloudWebSocket sender, int code, string reason);

        public event OnOpenHandler OnOpen;
        public event OnMessageHandler OnMessage;
        public event OnErrorHandler OnError;
        public event OnCloseHandler OnClose;
    }
}