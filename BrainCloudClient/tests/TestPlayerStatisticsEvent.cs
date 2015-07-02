using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlayerStatisticsEvent : TestFixtureBase
    {
        private readonly string _eventId01 = "testEvent01";
        private readonly string _eventId02 = "rewardCredits";

        [Test]
        public void TestTriggerPlayerStatisticsEvent()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStatisticsEventService.TriggerPlayerStatisticsEvent(
                _eventId01,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTriggerPlayerStatisticsEvents()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> event1 = new Dictionary<string, object> { { "eventName", _eventId01 }, { "eventMultiplier", 1 } };
            Dictionary<string, object> event2 = new Dictionary<string, object> { { "eventName", _eventId02 }, { "eventMultiplier", 1 } };

            Dictionary<string, object>[] jsonData = new Dictionary<string, object>[] { event1, event2 };

            BrainCloudClient.Get().PlayerStatisticsEventService.TriggerPlayerStatisticsEvents(
                JsonWriter.Serialize(jsonData),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}