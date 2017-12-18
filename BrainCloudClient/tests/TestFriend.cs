using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFriend : TestFixtureBase
    {
        [Test]
        public void FindUsersByExactName()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUsersByExactName(
                "search",
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindUsersBySubstrName()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUsersBySubstrName(
                "search",
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetExternalIdForProfileId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.GetExternalIdForProfileId(
                GetUser(Users.UserA).ProfileId,
                "Facebook",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetProfileInfoForCredential()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.GetProfileInfoForCredential(
                GetUser(Users.UserA).Id,
                AuthenticationType.Universal,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetProfileInfoForExternalAuthId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.GetProfileInfoForExternalAuthId(
                GetUser(Users.UserA).Id,
                "test",
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(400, ReasonCodes.INVALID_CREDENTIAL);
        }

        [Test]
        public void TestGetSummaryDataForProfileId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.GetSummaryDataForProfileId(
                GetUser(Users.UserA).ProfileId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindUserByUniversalId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUserByUniversalId(
                "search",
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestListFriends()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.ListFriends(
                BrainCloudFriend.FriendPlatform.All,
                false,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestAddFriends()
        {
            AddFriends();
        }

        [Test]
        public void TestRemoveFriends()
        {
            AddFriends();

            TestResult tr = new TestResult(_bc);

            string[] friends = { GetUser(Users.UserB).ProfileId };

            _bc.FriendService.RemoveFriends(
                friends,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetUsersOnlineStatus()
        {
            TestResult tr = new TestResult(_bc);

            string[] friends = { GetUser(Users.UserB).ProfileId };

            _bc.FriendService.GetUsersOnlineStatus(
                friends,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        //Helpers
        private void AddFriends()
        {
            TestResult tr = new TestResult(_bc);

            string[] friends = { GetUser(Users.UserB).ProfileId };

            _bc.FriendService.AddFriends(
                friends,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}