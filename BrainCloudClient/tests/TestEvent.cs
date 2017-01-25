using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestEvent : TestFixtureBase
    {
        private readonly string _eventType = "test";
        private readonly string _eventDataKey = "testData";

        private bool _callbackRan = false;

        [Test]
        public void TestSendEvent()
        {
            TestResult tr = new TestResult();
            string eventId = "";

            BrainCloudClient.Instance.RegisterEventCallback(EventCallback);

            BrainCloudClient.Instance.EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = (((Dictionary<string, object>)(tr.m_response["data"]))["evId"]) as string;
            }

            Assert.IsTrue(_callbackRan);

            CleanupIncomingEvent(eventId);
            BrainCloudClient.Instance.DeregisterEventCallback();
        }

        //[Test]
        //public void TestSendEvent()
        //{
        //    TestResult tr = new TestResult();
        //    ulong eventId = 0;

        //    BrainCloudClient.Instance.EventService.SendEvent(
        //        GetUser(Users.UserB).ProfileId,
        //        _eventType,
        //        Helpers.CreateJsonPair(_eventDataKey, 117),
        //        true,
        //        tr.ApiSuccess, tr.ApiError);

        //    if (tr.Run())
        //    {
        //        eventId = System.Convert.ToUInt64(((Dictionary<string, object>)(tr.m_response["data"]))["eventId"]);
        //    }

        //    BrainCloudClient.Instance.PlayerStateService.Logout(tr.ApiSuccess, tr.ApiError);
        //    tr.Run();
        //    BrainCloudClient.Instance.AuthenticationService.ClearSavedProfileID();

        //    BrainCloudClient.Instance.RegisterEventCallback(EventCallback);

        //    BrainCloudClient.Instance.AuthenticationService.AuthenticateUniversal(GetUser(Users.UserB).Id, GetUser(Users.UserB).Password, false, tr.ApiSuccess, tr.ApiError);
        //    tr.Run();

        //    CleanupIncomingEvent(eventId);
        //}

        public void EventCallback(string jsonResponse)
        {
            Console.WriteLine("Events received: " + jsonResponse);

            var response = JsonReader.Deserialize<Dictionary<string, object>>(jsonResponse);
            var events = (object[])(response["events"]);

            Assert.Greater(events.Length, 0);

            _callbackRan = true;
        }
        
        [Test]
        public void TestDeleteIncomingEvent()
        {
            TestResult tr = new TestResult();

            string eventId = SendDefaultMessage();

            BrainCloudClient.Instance.EventService.DeleteIncomingEvent(
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateIncomingEventData()
        {
            TestResult tr = new TestResult();

            string eventId = SendDefaultMessage();

            BrainCloudClient.Instance.EventService.UpdateIncomingEventData(
                eventId,
                Helpers.CreateJsonPair(_eventDataKey, 343),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        [Test]
        public void TestGetEvents()
        {
            TestResult tr = new TestResult();

            string eventId = SendDefaultMessage();

            BrainCloudClient.Instance.EventService.GetEvents(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupIncomingEvent(eventId);
        }

        #region Helpers

        private string SendDefaultMessage()
        {
            TestResult tr = new TestResult();
            string eventId = "";

            BrainCloudClient.Instance.EventService.SendEvent(
                GetUser(Users.UserA).ProfileId,
                _eventType,
                Helpers.CreateJsonPair(_eventDataKey, 117),
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                eventId = (((Dictionary<string, object>)(tr.m_response["data"]))["evId"]) as string;
            }

            return eventId;
        }

        private void CleanupIncomingEvent(string eventId)
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.EventService.DeleteIncomingEvent(
                eventId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion
    }
}