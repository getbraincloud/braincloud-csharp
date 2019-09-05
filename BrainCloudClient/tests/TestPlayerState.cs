using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestPlayerState : TestFixtureBase
    {
        [Test]
        public void TestDeletePlayer()
        {
            TestResult tr = new TestResult(_bc);

            _bc.Client.AuthenticationService.ClearSavedProfileID();

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserC).Id,
                GetUser(Users.UserC).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.PlayerStateService.DeleteUser(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.Client.AuthenticationService.ClearSavedProfileID();

            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(Users.UserA).Id,
                GetUser(Users.UserA).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            //GetUser(Users.UserA).ProfileId = _bc.AuthenticationService.ProfileId;
        }

        [Test]
        public void TestGetAttributes()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.GetAttributes(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestLogout()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.Logout(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadPlayerState()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.ReadUserState(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRemoveAttributes()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.RemoveAttributes(
                new string[] { "testAttrib1", "testAttrib2" },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetPlayer()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.ResetUser(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateAttributes()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> stats = new Dictionary<string, object> { { "testAttrib1", "value1" }, { "testAttrib2", "value2" } };

            _bc.PlayerStateService.UpdateAttributes(
                JsonWriter.Serialize(stats),
                false,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdatePlayerName()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.UpdateUserName(
                "ABC",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        
        [Test]
        public void TestUpdateSummaryFriendData()
        {
            TestResult tr = new TestResult(_bc);
            
            _bc.PlayerStateService.UpdateSummaryFriendData(
                "{\"field\":\"value\"}",
                tr.ApiSuccess, tr.ApiError);
            
            tr.Run();
        }

        [Test]
        public void TestUpdatePlayerPictureUrl()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.UpdateUserPictureUrl(
                @"https://some.domain.com/mypicture.jpg",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateContactEmail()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.UpdateContactEmail(
                GetUser(Users.UserA).Email,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestClearUserStatus()
        {
            TestResult tr = new TestResult(_bc);

            _bc.PlayerStateService.ClearUserStatus(
                "status",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestExtendUserStatus()
        {
            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.ExtendUserStatus(
                "status", 5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetUserStatus()
        {
            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.GetUserStatus(
                "status",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetUserStatus()
        {
            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.SetUserStatus(
                "status", 5,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}