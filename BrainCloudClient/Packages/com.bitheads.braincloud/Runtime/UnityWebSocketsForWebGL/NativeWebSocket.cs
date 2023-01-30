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
            int result = BrainCloudSocketError(buffer, bufferSize, Id);
            if (result == 0)
                return null;
            return Encoding.UTF8.GetString(buffer);
        }
    }

    public int State { get { return BrainCloudSocketState(Id); } }

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketCreate(string url, int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketClose(int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketSend(byte[] data, int length, int id);

    [DllImport("__Internal")]
    private static extern int BrainCloudSocketState(int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketOnOpen(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketOnMessage(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketOnError(Action<int> action, int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketOnClose(Action<int, int> action, int id);

    [DllImport("__Internal")]
    private static extern void BrainCloudSocketReceive(byte[] ptr, int length, int id);

    [DllImport("__Internal")]
    private static extern int BrainCloudSocketReceiveLength(int id);

    [DllImport("__Internal")]
    private static extern int BrainCloudSocketError(byte[] ptr, int length, int id);


    public NativeWebSocket(string url)
    {
        Id = UnityEngine.Random.Range(0, int.MaxValue);
        BrainCloudSocketCreate(url, Id);
    }

    public void SendAsync(byte[] packet)
    {
        BrainCloudSocketSend(packet, packet.Length, Id);
    }

    public void CloseAsync()
    {
        BrainCloudSocketClose(Id);
    }

    public void SetOnOpen(Action<int> action)
    {
        BrainCloudSocketOnOpen(action, Id);
    }

    public void SetOnMessage(Action<int> action)
    {
        BrainCloudSocketOnMessage(action, Id);
    }

    public void SetOnError(Action<int> action)
    {
        BrainCloudSocketOnError(action, Id);
    }

    public void SetOnClose(Action<int, int> action)
    {
        BrainCloudSocketOnClose(action, Id);
    }

    public byte[] Receive()
    {
        int length = BrainCloudSocketReceiveLength(Id);
        if (length == 0)
            return null;
        byte[] buffer = new byte[length];
        BrainCloudSocketReceive(buffer, length, Id);
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
