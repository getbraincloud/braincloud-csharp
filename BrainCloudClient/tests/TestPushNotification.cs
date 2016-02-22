using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPushNotification : TestFixtureBase
    {
        [Test]
        public void DeregisterAllPushNotificationDeviceTokens()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Instance.PushNotificationService.DeregisterAllPushNotificationDeviceTokens(
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void DeregisterPushNotificationDeviceToken()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Instance.PushNotificationService.RegisterPushNotificationDeviceToken(
                Platform.iOS,
                "GARBAGE_TOKEN",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();

            tr.Reset ();
            BrainCloudClient.Instance.PushNotificationService.DeregisterPushNotificationDeviceToken(
                Platform.iOS,
                "GARBAGE_TOKEN",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void RegisterPushNotificationDeviceToken()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PushNotificationService.RegisterPushNotificationDeviceToken(
                Platform.iOS,
                "GARBAGE_TOKEN",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void SendSimplePushNotification()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PushNotificationService.SendSimplePushNotification(
                GetUser(Users.UserA).ProfileId,
                "Test message",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void SendRichPushNotification()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PushNotificationService.SendRichPushNotification(
                GetUser(Users.UserA).ProfileId,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void SendRichPushNotificationWithParams()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.PushNotificationService.SendRichPushNotificationWithParams(
                GetUser(Users.UserA).ProfileId,
                1,
                Helpers.CreateJsonPair("1", GetUser(Users.UserA).Id),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}