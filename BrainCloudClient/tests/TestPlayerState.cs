using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlayerState : TestFixtureBase
    {
        [Test]
        public void TestDeletePlayer()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().AuthenticationService.ClearSavedProfileID();

            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserC).Id,
                GetUser(Users.UserC).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().PlayerStateService.DeletePlayer(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            BrainCloudClient.Get().AuthenticationService.ClearSavedProfileID();

            BrainCloudClient.Get().AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            //GetUser(Users.UserA).ProfileId = BrainCloudClient.Get().AuthenticationService.ProfileId;
        }

        [Test]
        public void TestGetAttributes()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.GetAttributes(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestLogout()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.Logout(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerState()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.ReadPlayerState(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRemoveAttributes()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.RemoveAttributes(
                new string[] { "testAttrib1", "testAttrib2" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetPlayer()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.ResetPlayer(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateAttributes()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> stats = new Dictionary<string, object> { { "testAttrib1", "value1" }, { "testAttrib2", "value2" } };

            BrainCloudClient.Get().PlayerStateService.UpdateAttributes(
                JsonWriter.Serialize(stats),
                false,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdatePlayerName()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.UpdatePlayerName(
                "ABC",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        
        [Test]
        public void TestUpdateSummaryFriendData()
        {
            TestResult tr = new TestResult();
            
            BrainCloudClient.Get().PlayerStateService.UpdateSummaryFriendData(
                "{\"field\":\"value\"}",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestUpdatePlayerPictureUrl()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().PlayerStateService.UpdatePlayerPictureUrl(
                @"https://some.domain.com/mypicture.jpg",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}