// Offline regression harness. Compile against the SDK and run TestAppCheck.exe.
// No credentials, Firebase, Unity runtime, or server connection required.
using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using BrainCloud;
using BrainCloud.JsonFx.Json;

public static class TestAppCheck
{
    static object Field(object obj, string name)
    {
        return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
    }
    static IList Queue(BrainCloudClient client) { return (IList)Field(Field(client, "_comms"), "_serviceCallsWaiting"); }
    static IDictionary Data(BrainCloudClient client, int index = 0) { return (IDictionary)Field(Queue(client)[index], "m_jsonData"); }
    static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Pump(BrainCloudClient client)
    {
        // Process provider results without allowing the transport to send requests.
        typeof(BrainCloudAuthentication).GetMethod("RunAppCheckCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(client.AuthenticationService, null);
    }
    static void Auth(BrainCloudClient client, FailureCallback failure = null, object context = null)
    {
        client.AuthenticationService.AuthenticateUniversal("user", "password", true, null, failure, context);
    }
    public static void Main()
    {
        var client = new BrainCloudClient();
        var auth = client.AuthenticationService;
        Auth(client);
        string baseline = JsonWriter.Serialize(Data(client));
        Check(!Data(client).Contains("appCheckToken"), "unset token");
        Queue(client).Clear();
        auth.SetAppCheckToken("manual");
        Auth(client);
        Check((string)Data(client)["appCheckToken"] == "manual", "manual token");
        Data(client).Remove("appCheckToken");
        Check(JsonWriter.Serialize(Data(client)) == baseline, "other auth fields unchanged");
        Queue(client).Clear();
        Auth(client);
        auth.SetAppCheckToken("fresh");
        Check((string)Data(client)["appCheckToken"] == "manual", "queued token snapshot");
        Queue(client).Clear();
        Auth(client);
        Check((string)Data(client)["appCheckToken"] == "fresh", "manual refresh");
        foreach (var clear in new[] { "", null })
        {
            Queue(client).Clear();
            auth.SetAppCheckToken(clear);
            Auth(client);
            Check(!Data(client).Contains("appCheckToken"), "clear token");
        }
        Queue(client).Clear();
        auth.SetAppCheckToken("manual");
        auth.getServerVersion();
        Check(!Data(client).Contains("appCheckToken"), "non-auth request unchanged");
        Queue(client).Clear();
        var other = new BrainCloudClient();
        Auth(other);
        Check(!Data(other).Contains("appCheckToken"), "client isolation");

        BrainCloudAuthentication.AppCheckTokenCompletion done = null;
        auth.SetAppCheckTokenProvider(completion => done = completion);
        Auth(client);
        Check(Queue(client).Count == 0, "wait for provider");
        var worker = new Thread(() => { done("async", null); done("duplicate", null); });
        worker.Start(); worker.Join();
        Check(Queue(client).Count == 0, "worker cannot queue request");
        Pump(client);
        Check(Queue(client).Count == 1 && (string)Data(client)["appCheckToken"] == "async", "first completion wins");
        Queue(client).Clear();
        Auth(client);
        auth.SetAppCheckTokenProvider(completion => completion("replacement", null));
        done("original", null);
        Pump(client);
        Check((string)Data(client)["appCheckToken"] == "original", "pending provider preserved");
        Queue(client).Clear();
        Auth(client); Pump(client);
        Check((string)Data(client)["appCheckToken"] == "replacement", "replacement applies to new auth");
        Queue(client).Clear();
        auth.SetAppCheckTokenProvider(null);
        Auth(client);
        Check((string)Data(client)["appCheckToken"] == "manual", "stored token restored");
        Queue(client).Clear();

        int failures = 0;
        object context = new object();
        int sdkThread = Thread.CurrentThread.ManagedThreadId;
        FailureCallback failure = (status, reason, json, obj) => {
            Check(status == 400 && reason == ReasonCodes.CLIENT_APP_CHECK_TOKEN_ERROR, "failure codes");
            Check(ReferenceEquals(obj, context), "callback context");
            Check(Thread.CurrentThread.ManagedThreadId == sdkThread, "failure thread");
            Check(json.Contains("status_message"), "error response JSON");
            ++failures;
        };
        foreach (var provider in new BrainCloudAuthentication.AppCheckTokenProvider[] {
            completion => completion("ignored", "failed"),
            completion => completion(null, null),
            completion => { throw new Exception("private detail"); }
        })
        {
            auth.SetAppCheckTokenProvider(provider);
            Auth(client, failure, context);
            Pump(client);
            Check(Queue(client).Count == 0, "failure must not send stored token");
        }
        Check(failures == 3, "all local failures delivered");
        auth.SetAppCheckTokenProvider(completion => done = completion);
        Auth(client, failure, context);
        client.ResetCommunication(); done("late", null); Pump(client);
        Check(Queue(client).Count == 0 && failures == 3, "reset discards pending");
        Auth(client, failure, context);
        client.ShutDown(); done("late", null); Pump(client);
        Check(Queue(client).Count == 0 && failures == 3, "shutdown discards pending");
        Auth(client, failure, context);
        Console.WriteLine("Waiting for the real 30-second provider timeout...");
        Thread.Sleep(30100);
        client.RunCallbacks(eBrainCloudUpdateType.RTT);
        Check(failures == 3, "RTT does not process provider results");
        client.RunCallbacks(eBrainCloudUpdateType.REST);
        done("too late", null); Pump(client);
        Check(failures == 4 && Queue(client).Count == 0, "timeout fails once without sending");
        Console.WriteLine("All App Check regression checks passed.");
    }
}
