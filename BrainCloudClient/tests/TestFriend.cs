using System.Collections.Generic;
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

            tr.RunExpectFail(400, ReasonCodes.INVALID_EXT_AUTH_TYPE);
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
        public void TestFindUserByExactUniversalId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUserByExactUniversalId(
                "RandomUniversalId",
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
        public void TestGetMySocialInfo()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.GetMySocialInfo(
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
        public void TestAddFriendsFromPlatform()
        {
            TestResult tr = new TestResult(_bc);
            
            string [] ids = { };
            
            _bc.FriendService.AddFriendsFromPlatform(
                BrainCloudFriend.FriendPlatform.Facebook, 
                "ADD", 
                ids, 
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
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

        private void testFindUsersByUniversalIdStartingWith()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUsersByUniversalIdStartingWith(
                "completelyRandomUniversalId",
                30,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private void testFindUsersByNameStartingWith()
        {
            TestResult tr = new TestResult(_bc);

            _bc.FriendService.FindUsersByNameStartingWith(
                "completelyRandomName",
                30,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}
