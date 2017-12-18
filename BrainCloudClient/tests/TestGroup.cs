using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Common;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGroup : TestFixtureNoAuth
    {
        private readonly string _groupType = "test";
        private readonly string _entityType = "test";

        private string _groupId = "";

        [TearDown]
        public void Cleanup()
        {
            if (!string.IsNullOrEmpty(_groupId))
            {
                DeleteGroupAsUserA();
            }

            if (_bc.Client.IsAuthenticated())
            {
                Logout();
            }
        }

        [Test]
        public void TestAcceptGroupInvitation()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.InviteGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.ADMIN,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            Authenticate(Users.UserB);

            _bc.GroupService.AcceptGroupInvitation(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            DeleteGroupAsUserA();
        }

        [Test]
        public void TestAddGroupMember()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.AddGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.ADMIN,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }

        [Test]
        public void TestApproveGroupJoinRequest()
        {
            CreateGroupAsUserA();
            Authenticate(Users.UserB);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.JoinGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            Authenticate(Users.UserA);

            _bc.GroupService.ApproveGroupJoinRequest(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.MEMBER,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }


        [Test]
        public void TestAutoJoinGroup()
        {
            CreateGroupAsUserA(true);
            Authenticate(Users.UserB);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.AutoJoinGroup(
                _groupType,
                BrainCloudGroup.AutoJoinStrategy.JoinFirstGroup,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            DeleteGroupAsUserA();
        }

        [Test]
        public void TestCancelGroupInvitation()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.InviteGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.ADMIN,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.GroupService.CancelGroupInvitation(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }

        [Test]
        public void TestCreateGroup()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestCreateGroupEntity()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            CreateGroupEntity();
            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestDeleteGroup()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            DeleteGroup();
            Logout();
        }


        [Test]
        public void TestDeleteGroupEntity()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            string entityId = CreateGroupEntity();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.DeleteGroupEntity(
                _groupId,
                entityId,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestGetMyGroups()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.GetMyGroups(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestIncrementGroupData()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.IncrementGroupData(
                _groupId,
                Helpers.CreateJsonPair("testInc", 1),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestIncrementGroupEntityData()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            string id = CreateGroupEntity();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.IncrementGroupEntityData(
                _groupId,
                id,
                Helpers.CreateJsonPair("testInc", 1),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestIncrementGroupEntityDataSummary()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            string id = CreateGroupEntity();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.IncrementGroupEntityData(
                _groupId,
                id,
                Helpers.CreateJsonPair("testInc", 1),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestInviteGroupMember()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.InviteGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.MEMBER,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestJoinGroup()
        {
            CreateGroupAsUserA();
            Authenticate(Users.UserB);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.JoinGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            DeleteGroupAsUserA();
        }

        [Test]
        public void TestLeaveGroup()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.LeaveGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            _bc.GroupService.ReadGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.RunExpectFail(400, 40345);

            _groupId = null;
            Logout();
        }

        [Test]
        public void TestListGroupsPage()
        {
            Authenticate(Users.UserA);

            string context = CreateContext(10, 1, "groupType", _groupType);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ListGroupsPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
        }

        [Test]
        public void TestListGroupsPageByOffset()
        {
            Authenticate(Users.UserA);

            string context = CreateContext(10, 1, "groupType", _groupType);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ListGroupsPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            context = (string)((Dictionary<string, object>)tr.m_response["data"])["context"];

            _bc.GroupService.ListGroupsPageByOffset(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);

            Logout();
        }

        [Test]
        public void TestListGroupsWithMember()
        {
            Authenticate(Users.UserA);
            //CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ListGroupsWithMember(
                GetUser(Users.UserA).ProfileId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
        }

        [Test]
        public void TestReadGroup()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestReadGroupData()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroupData(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestReadGroupEntitiesPage()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            string context = CreateContext(10, 1, "groupId", _groupId);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroupEntitiesPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestReadGroupEntitiesPageByOffset()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            string context = CreateContext(10, 1, "groupId", _groupId);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroupEntitiesPage(
                context,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            context = (string)((Dictionary<string, object>)tr.m_response["data"])["context"];

            _bc.GroupService.ReadGroupEntitiesPageByOffset(
                context,
                1,
                tr.ApiSuccess, tr.ApiError);

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestReadGroupEntity()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            string id = CreateGroupEntity();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroupEntity(
                _groupId,
                id,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestReadGroupMembers()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.ReadGroupMembers(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestRejectGroupInvitation()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.InviteGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                BrainCloudGroup.Role.ADMIN,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            Authenticate(Users.UserB);

            _bc.GroupService.RejectGroupInvitation(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            DeleteGroupAsUserA();
        }

        [Test]
        public void TestRejectGroupJoinRequest()
        {
            CreateGroupAsUserA();
            Authenticate(Users.UserB);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.JoinGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            Authenticate(Users.UserA);

            _bc.GroupService.RejectGroupJoinRequest(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }

        [Test]
        public void TestRemoveGroupMember()
        {
            CreateGroupAsUserA(true);
            Authenticate(Users.UserB);

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.JoinGroup(
                _groupId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Logout();
            Authenticate(Users.UserA);

            _bc.GroupService.RemoveGroupMember(
                _groupId,
                GetUser(Users.UserB).ProfileId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
        }

        [Test]
        public void TestUpdateGroupData()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.UpdateGroupData(
                _groupId,
                1,
                Helpers.CreateJsonPair("testUpdate", 1),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestUpdateGroupEntity()
        {
            Authenticate(Users.UserA);
            CreateGroup();
            string id = CreateGroupEntity();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.UpdateGroupEntityData(
                _groupId,
                id,
                1,
                Helpers.CreateJsonPair("testUpdate", 1),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestUpdateGroupMember()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.UpdateGroupMember(
                _groupId,
                GetUser(Users.UserA).ProfileId,
                null,
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        [Test]
        public void TestUpdateGroupName()
        {
            Authenticate(Users.UserA);
            CreateGroup();

            TestResult tr = new TestResult(_bc);
            _bc.GroupService.UpdateGroupName(
                _groupId,
                "testName",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            DeleteGroup();
            Logout();
        }

        #region Helpers

        private void CreateGroupAsUserA(bool isOpen = false)
        {
            Authenticate(Users.UserA);
            CreateGroup(isOpen);
            Logout();
        }

        private void DeleteGroupAsUserA()
        {
            Authenticate(Users.UserA);
            DeleteGroup();
            Logout();
        }

        private void Authenticate(Users user)
        {
            TestResult tr = new TestResult(_bc);
            _bc.Client.AuthenticationService.AuthenticateUniversal(
                GetUser(user).Id,
                GetUser(user).Password,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        private void Logout()
        {
            TestResult tr = new TestResult(_bc);
            _bc.PlayerStateService.Logout(
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            _bc.Client.AuthenticationService.ClearSavedProfileID();
        }

        private void CreateGroup(bool isOpen = false)
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroup(
                "testGroup",
                _groupType,
                isOpen,
                new GroupACL(GroupACL.Access.ReadWrite, GroupACL.Access.ReadWrite),
                Helpers.CreateJsonPair("testInc", 123),
                Helpers.CreateJsonPair("test", "test"),
                Helpers.CreateJsonPair("test", "test"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            _groupId = (string)data["groupId"];
        }

        private string CreateGroupEntity()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GroupService.CreateGroupEntity(
                _groupId,
                _entityType,
                false,
                new GroupACL(GroupACL.Access.ReadWrite, GroupACL.Access.ReadWrite),
                Helpers.CreateJsonPair("testInc", 123),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            return (string)Helpers.GetDataFromJsonResponse(tr.m_response)["entityId"];
        }

        private void DeleteGroup()
        {
            TestResult tr = new TestResult(_bc);
            _bc.GroupService.DeleteGroup(
                _groupId,
                -1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
            _groupId = null;
        }

        private string CreateContext(int numItemsPerPage, int startPage, string searchKey, string searchValue)
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            Dictionary<string, object> pagination = new Dictionary<string, object>();
            pagination.Add("rowsPerPage", startPage);
            pagination.Add("pageNumber", startPage);
            context.Add("pagination", pagination);

            Dictionary<string, object> searchCriteria = new Dictionary<string, object>();
            searchCriteria.Add(searchKey, searchValue);
            context.Add("searchCriteria", searchCriteria);

            return JsonWriter.Serialize(context);
        }

        #endregion
    }
}