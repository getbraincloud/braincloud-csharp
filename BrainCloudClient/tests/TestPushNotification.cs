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
        public void RegisterPushNotificationDeviceToken()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Get().PushNotificationService.RegisterPushNotificationDeviceToken(
                Platform.iOS,
                "GARBAGE_TOKEN",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }
    }
}