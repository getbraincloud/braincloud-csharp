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
        private int m_rewardCallbackHitCount = 0;

        [Test]
        public void TestTriggerPlayerStatisticsEvent()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStatisticsEventService.TriggerStatsEvent(
                _eventId01,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTriggerPlayerStatisticsEvents()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> event1 = new Dictionary<string, object> { { "eventName", _eventId01 }, { "eventMultiplier", 1 } };
            Dictionary<string, object> event2 = new Dictionary<string, object> { { "eventName", _eventId02 }, { "eventMultiplier", 1 } };

            Dictionary<string, object>[] jsonData = new Dictionary<string, object>[] { event1, event2 };

            _bc.PlayerStatisticsEventService.TriggerStatsEvents(
                JsonWriter.Serialize(jsonData),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRewardHandlerTriggerStatisticsEvents()
        {
            m_rewardCallbackHitCount = 0;

            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.ResetUser(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Dictionary<string, object> event1 = new Dictionary<string, object> { { "eventName", "incQuest1Stat" }, { "eventMultiplier", 1 } };
            Dictionary<string, object>[] jsonData = new Dictionary<string, object>[] { event1 };

            BrainCloudClient.Get ().RegisterRewardCallback(rewardCallback);
            _bc.PlayerStatisticsEventService.TriggerStatsEvents(
                JsonWriter.Serialize(jsonData),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get ().DeregisterRewardCallback();

            Assert.AreEqual (m_rewardCallbackHitCount, 1);
        }

        [Test]
        public void TestRewardHandlerMultipleApiCallsInBundle()
        {
            m_rewardCallbackHitCount = 0;
            
            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.ResetUser(tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            Dictionary<string, object> event1 = new Dictionary<string, object> { { "eventName", "incQuest1Stat" }, { "eventMultiplier", 1 } };
            Dictionary<string, object>[] jsonData1 = new Dictionary<string, object>[] { event1 };
            Dictionary<string, object> event2 = new Dictionary<string, object> { { "eventName", "incQuest2Stat" }, { "eventMultiplier", 1 } };
            Dictionary<string, object>[] jsonData2 = new Dictionary<string, object>[] { event2 };
            
            BrainCloudClient.Get ().RegisterRewardCallback(rewardCallback);
            _bc.PlayerStatisticsEventService.TriggerStatsEvents(
                JsonWriter.Serialize(jsonData1),
                tr.ApiSuccess, tr.ApiError);
            _bc.PlayerStatisticsEventService.TriggerStatsEvents(
                JsonWriter.Serialize(jsonData2),
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectCount(2);
            
            BrainCloudClient.Get ().DeregisterRewardCallback();
            
            Assert.AreEqual (m_rewardCallbackHitCount, 2);
        }

        public void rewardCallback(string jsonData)
        {
            System.Console.WriteLine(jsonData);
            Dictionary<string, object> data = (Dictionary<string, object>) JsonReader.Deserialize(jsonData);
            Dictionary<string, object>[] apiRewards = (Dictionary<string, object>[]) data["apiRewards"];
            m_rewardCallbackHitCount += apiRewards.Length;
        }
    }
}