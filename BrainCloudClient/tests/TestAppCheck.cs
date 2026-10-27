// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Reflection;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestAppCheck
    {
        // These SDK-only tests inspect queued requests without sending them to a server.
        // Do not inherit TestFixtureBase: it loads credentials and authenticates in Setup.
        private BrainCloudClient client;
        private BrainCloudAuthentication auth;
        private int _failures;
        private object _context;
        private int _sdkThread;

        [SetUp]
        public void Setup()
        {
            client = new BrainCloudClient();
            auth = client.AuthenticationService;
            _failures = 0;
            _context = new object();
            _sdkThread = Thread.CurrentThread.ManagedThreadId;
        }

        [TearDown]
        public void TearDown()
        {
            client.ShutDown();
        }

        [Test]
        public void TestAppCheckStoredToken()
        {
            Authenticate(client);
            string baseline = JsonWriter.Serialize(GetRequestData(client));
            Assert.That(!GetRequestData(client).Contains("appCheckToken"), "unset token");
            GetQueuedRequests(client).Clear();
            auth.SetAppCheckToken("manual");
            Authenticate(client);
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "manual", "manual token");
            GetRequestData(client).Remove("appCheckToken");
            Assert.That(JsonWriter.Serialize(GetRequestData(client)) == baseline, "other auth fields unchanged");
        }

        [Test]
        public void TestAppCheckStoredTokenRefresh()
        {
            auth.SetAppCheckToken("manual");
            GetQueuedRequests(client).Clear();
            Authenticate(client);
            auth.SetAppCheckToken("fresh");
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "manual", "queued token snapshot");
            GetQueuedRequests(client).Clear();
            Authenticate(client);
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "fresh", "manual refresh");
        }

        [TestCase("")]
        [TestCase(null)]
        public void TestAppCheckClearStoredToken(string token)
        {
            auth.SetAppCheckToken("manual");
            auth.SetAppCheckToken(token);
            Authenticate(client);
            Assert.That(GetRequestData(client).Contains("appCheckToken"), Is.False);
        }

        [Test]
        public void TestAppCheckNonAuthenticationRequest()
        {
            GetQueuedRequests(client).Clear();
            auth.SetAppCheckToken("manual");
            auth.getServerVersion();
            Assert.That(!GetRequestData(client).Contains("appCheckToken"), "non-auth request unchanged");
            GetQueuedRequests(client).Clear();
        }

        [Test]
        public void TestAppCheckClientIsolation()
        {
            auth.SetAppCheckToken("manual");
            var other = new BrainCloudClient();
            try
            {
                Authenticate(other);
                Assert.That(!GetRequestData(other).Contains("appCheckToken"), "client isolation");
            }
            finally
            {
                other.ShutDown();
            }
        }

        [Test]
        public void TestAppCheckAsyncProviderFirstCompletionWins()
        {
            BrainCloudAuthentication.AppCheckTokenCompletion done = null;
            auth.SetAppCheckTokenProvider(completion => done = completion);
            Authenticate(client);
            Assert.That(GetQueuedRequests(client).Count == 0, "wait for provider");
            var worker = new Thread(() => { done("async", null); done("duplicate", null); });
            worker.Start();
            worker.Join();
            Assert.That(GetQueuedRequests(client).Count == 0, "worker cannot queue request");
            RunAppCheckCallbacks(client);
            Assert.That(GetQueuedRequests(client).Count == 1 && (string)GetRequestData(client)["appCheckToken"] == "async", "first completion wins");
        }

        [Test]
        public void TestAppCheckReplaceAndClearProvider()
        {
            auth.SetAppCheckToken("manual");
            BrainCloudAuthentication.AppCheckTokenCompletion done = null;
            auth.SetAppCheckTokenProvider(completion => done = completion);
            GetQueuedRequests(client).Clear();
            Authenticate(client);
            auth.SetAppCheckTokenProvider(completion => completion("replacement", null));
            done("original", null);
            RunAppCheckCallbacks(client);
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "original", "pending provider preserved");
            GetQueuedRequests(client).Clear();
            Authenticate(client);
            RunAppCheckCallbacks(client);
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "replacement", "replacement applies to new auth");
            GetQueuedRequests(client).Clear();
            auth.SetAppCheckTokenProvider(null);
            Authenticate(client);
            Assert.That((string)GetRequestData(client)["appCheckToken"] == "manual", "stored token restored");
            GetQueuedRequests(client).Clear();
        }

        [Test]
        public void TestAppCheckProviderErrors()
        {
            auth.SetAppCheckToken("manual");
            foreach (var provider in new BrainCloudAuthentication.AppCheckTokenProvider[] {
                completion => completion("ignored", "failed"),
                completion => completion(null, null),
                completion => { throw new Exception("private detail"); }
            })
            {
                auth.SetAppCheckTokenProvider(provider);
                Authenticate(client, Failure, _context);
                RunAppCheckCallbacks(client);
                Assert.That(GetQueuedRequests(client).Count == 0, "failure must not send stored token");
            }
            Assert.That(_failures == 3, "all local failures delivered");
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestAppCheckCancelPendingProvider(bool shutdown)
        {
            BrainCloudAuthentication.AppCheckTokenCompletion done = null;
            auth.SetAppCheckTokenProvider(completion => done = completion);
            Authenticate(client, Failure, _context);
            if (shutdown)
                client.ShutDown();
            else
                client.ResetCommunication();
            done("late", null);
            RunAppCheckCallbacks(client);
            Assert.That(GetQueuedRequests(client).Count, Is.EqualTo(0));
            Assert.That(_failures, Is.EqualTo(0));
        }

        [Test]
        public void TestAppCheckProviderTimeout()
        {
            BrainCloudAuthentication.AppCheckTokenCompletion done = null;
            auth.SetAppCheckTokenProvider(completion => done = completion);
            Authenticate(client, Failure, _context);
            // Exercise the real provider deadline; RTT updates must not process REST callbacks.
            Thread.Sleep(30100);
            client.RunCallbacks(eBrainCloudUpdateType.RTT);
            Assert.That(_failures, Is.EqualTo(0));
            client.RunCallbacks(eBrainCloudUpdateType.REST);
            done("too late", null);
            RunAppCheckCallbacks(client);
            Assert.That(_failures, Is.EqualTo(1));
            Assert.That(GetQueuedRequests(client).Count, Is.EqualTo(0));
        }

        private void Failure(int status, int reason, string json, object context)
        {
            Assert.That(status, Is.EqualTo(400));
            Assert.That(reason, Is.EqualTo(ReasonCodes.CLIENT_APP_CHECK_TOKEN_ERROR));
            Assert.That(context, Is.SameAs(_context));
            Assert.That(Thread.CurrentThread.ManagedThreadId, Is.EqualTo(_sdkThread));
            Assert.That(json.Contains("status_message"));
            ++_failures;
        }

        private static object GetPrivateField(object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        private static IList GetQueuedRequests(BrainCloudClient client)
        {
            return (IList)GetPrivateField(GetPrivateField(client, "_comms"), "_serviceCallsWaiting");
        }

        private static IDictionary GetRequestData(BrainCloudClient client, int index = 0)
        {
            return (IDictionary)GetPrivateField(GetQueuedRequests(client)[index], "m_jsonData");
        }

        private static void RunAppCheckCallbacks(BrainCloudClient client)
        {
            // Process provider results without allowing the transport to send requests.
            typeof(BrainCloudAuthentication).GetMethod("RunAppCheckCallbacks", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(client.AuthenticationService, null);
        }
        private static void Authenticate(BrainCloudClient client, FailureCallback failure = null, object context = null)
        {
            client.AuthenticationService.AuthenticateUniversal("user", "password", true, null, failure, context);
        }
    }
}
