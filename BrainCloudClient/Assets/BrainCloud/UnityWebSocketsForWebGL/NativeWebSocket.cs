#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;

public class NativeWebSocket
{
    public int Id { get; private set; }

    public string Error
    {
        get
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int result = SocketError(buffer, bufferSize, Id);
            if (result == 0)
                return null;
            return Encoding.UTF8.GetString(buffer);
        }
    }

    public int State { get { return SocketState(Id); } }

    [DllImport("__Internal")]
    private static extern void SocketCreate(string url, int id);

    [DllImport("__Internal")]
    private static extern void SocketClose(int id);

    [DllImport("__Internal")]
    private static extern void SocketSend(byte[] data, int length, int id);

    [DllImport("__Internal")]
    private static extern int SocketState(int id);

    [DllImport("__Internal")]
    private static extern void SocketOnOpen(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void SocketOnMessage(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void SocketOnError(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void SocketOnClose(Action<int, int> action, int id);

    [DllImport("__Internal")]
    private static extern void SocketReceive(byte[] ptr, int length, int id);

    [DllImport("__Internal")]
    private static extern int SocketReceiveLength(int id);

    [DllImport("__Internal")]
    private static extern int SocketError(byte[] ptr, int length, int id);


    public NativeWebSocket(string url)
    {
        Id = UnityEngine.Random.Range(0, int.MaxValue);
        SocketCreate(url, Id);
    }

    public void SendAsync(byte[] packet)
    {
        SocketSend(packet, packet.Length, Id);
    }

    public void CloseAsync()
    {
        SocketClose(Id);
    }

    public void SetOnOpen(Action<int> action)
    {
        SocketOnOpen(action, Id);
    }

    public void SetOnMessage(Action<int> action)
    {
        SocketOnMessage(action, Id);
    }

    public void SetOnError(Action<int> action)
    {
        SocketOnError(action, Id);
    }

    public void SetOnClose(Action<int, int> action)
    {
        SocketOnClose(action, Id);
    }

    public byte[] Receive()
    {
        int length = SocketReceiveLength(Id);
        if (length == 0)
            return null;
        byte[] buffer = new byte[length];
        SocketReceive(buffer, length, Id);
        return buffer;
    }
}

public class EReadyState
{

    public const int Connecting = 0;
    public const int Open = 0;
    public const int Closing = 2;
    public const int Closed = 3;
}

public class CloseError
{

    public int Code { get { return code; } }
    public string Message { get { return message; } }

    private int code;
    private string message;

    private CloseError(int c, string m)
    {
        code = c;
        message = m;
    }

    public static readonly CloseError EndpointGoingAway = new CloseError(1001, "Endpoint going away.");
    public static readonly CloseError ProtocolError = new CloseError(1002, "Protocol error.");
    public static readonly CloseError UnsupportedMessage = new CloseError(1003, "Unsupported message.");
    public static readonly CloseError NoStatus = new CloseError(1005, "No status.");
    public static readonly CloseError AbnormalDisconnection = new CloseError(1006, "Abnormal disconnection.");
    public static readonly CloseError DataFrameTooLarge = new CloseError(1009, "Data frame too large.");
    public static readonly CloseError Unknown = new CloseError(0, "Unknown.");

    public static CloseError Get(int c)
    {
        switch (c)
        {
            case 1001:
                return EndpointGoingAway;
            case 1002:
                return ProtocolError;
            case 1003:
                return UnsupportedMessage;
            case 1005:
                return NoStatus;
            case 1006:
                return AbnormalDisconnection;
            case 1009:
                return DataFrameTooLarge;
            default:
                return Unknown;
        }
    }
}
#endif
