using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestEvent : TestFixtureBase
    {
        private readonly string _eventType = "test";
        private readonly string _eventDataKey = "testData";


        [Test]
        public void TestSendEvent()
        {
            TestResult tr = new TestResult();
            ulong eventId = 0;

            BrainCloudClient.Get().EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                true,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = System.Convert.ToUInt64(((Dictionary<string, object>)(tr.m_response["data"]))["eventId"]);
            }

            CleanupIncomingEvent(eventId);
        }

        [Test]
        public void TestDeleteSentEvent()
        {
            TestResult tr = new TestResult();

            ulong eventId = SendDefaultMessage(true);

            BrainCloudClient.Get().EventService.DeleteSentEvent(
                GetUser(Users.UserA).ProfileId,
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        [Test]
        public void TestDeleteIncomingEvent()
        {
            TestResult tr = new TestResult();

            ulong eventId = SendDefaultMessage(false);

            BrainCloudClient.Get().EventService.DeleteIncomingEvent(
                GetUser(Users.UserA).ProfileId,
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateIncomingEventData()
        {
            TestResult tr = new TestResult();

            ulong eventId = SendDefaultMessage(false);

            BrainCloudClient.Get().EventService.UpdateIncomingEventData(
                GetUser(Users.UserA).ProfileId,
                eventId,
                Helpers.CreateJsonPair(_eventDataKey, 343),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        #region Helpers

        private ulong SendDefaultMessage(bool recordLocally = false)
        {
            TestResult tr = new TestResult();
            ulong eventId = 0;

            BrainCloudClient.Get().EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                recordLocally,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = System.Convert.ToUInt64(((Dictionary<string, object>)(tr.m_response["data"]))["eventId"]);
            }

            return eventId;
        }

        private void CleanupIncomingEvent(ulong eventId)
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().EventService.DeleteIncomingEvent(
                GetUser(Users.UserA).ProfileId,
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion
    }
}