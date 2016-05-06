//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using BrainCloud.Common;
using BrainCloud.Internal;
using JsonFx.Json;
using System.Collections;
using System.Collections.Generic;

namespace BrainCloud
{
    public class BrainCloudGroup
    {
        public enum Role
        {
            OWNER,
            ADMIN,
            MEMBER
        }

        private BrainCloudClient _bcClient;

        public BrainCloudGroup(BrainCloudClient brainCloudClientRef)
        {
            _bcClient = brainCloudClientRef;
        }

        /// <summary>
        /// Method returns the server time in UTC. This is in UNIX millis time format.
        /// For instance 1396378241893 represents 2014-04-01 2:50:41.893 in GMT-4.
        /// </summary>
        /// <remarks>
        /// Service Name - Group
        /// Service Operation - Read
        /// </remarks>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns> A JSON string such as:
        /// 
        /// </returns>
        public void AcceptGroupInvitation(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.AcceptGroupInvitation, success, failure, cbObject, data);
        }

        public void AddGroupMember(
            string groupId,
            string profileId,
            Role role,
            string jsonAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;
            data[OperationParam.GroupRole.Value] = role.ToString();

            if (Util.IsOptionalParameterValid(jsonAttributes))
            {
                Dictionary<string, object> customData = JsonReader.Deserialize<Dictionary<string, object>>(jsonAttributes);
                data[OperationParam.GroupAttributes.Value] = customData;
            }

            SendRequest(ServiceOperation.AddGroupMember, success, failure, cbObject, data);
        }

        public void ApproveGroupJoinRequest(
            string groupId,
            string profileId,
            Role role,
            string jsonAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;
            data[OperationParam.GroupRole.Value] = role.ToString();

            if (Util.IsOptionalParameterValid(jsonAttributes))
            {
                Dictionary<string, object> customData = JsonReader.Deserialize<Dictionary<string, object>>(jsonAttributes);
                data[OperationParam.GroupAttributes.Value] = customData;
            }

            SendRequest(ServiceOperation.ApproveGroupJoinRequest, success, failure, cbObject, data);
        }

        public void CancelGroupInvitation(
            string groupId,
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;

            SendRequest(ServiceOperation.CancelGroupInvitation, success, failure, cbObject, data);
        }

        public void CreateGroup(
            string name,
            string type,
            bool? isOpenGroup,
            GroupACL acl,
            string jsonData,
            string jsonOwnerAttributes,
            string jsonDefaultMemberAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(name)) data[OperationParam.GroupName.Value] = name;
            data[OperationParam.GroupType.Value] = type;
            if (isOpenGroup.HasValue) data[OperationParam.GroupIsOpenGroup.Value] = isOpenGroup.Value;
            if (acl != null) data[OperationParam.GroupAcl.Value] = JsonReader.Deserialize(acl.ToJsonString());
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);
            if (!string.IsNullOrEmpty(jsonOwnerAttributes))
                data[OperationParam.GroupOwnerAttributes.Value] = JsonReader.Deserialize(jsonOwnerAttributes);
            if (!string.IsNullOrEmpty(jsonDefaultMemberAttributes))
                data[OperationParam.GroupDefaultMemberAttributes.Value] = JsonReader.Deserialize(jsonDefaultMemberAttributes);

            SendRequest(ServiceOperation.CreateGroup, success, failure, cbObject, data);
        }

        public void CreateGroupEntity(
            string groupId,
            string entityType,
            bool? isOwnedByGroupMember,
            GroupACL acl,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            if (!string.IsNullOrEmpty(entityType)) data[OperationParam.GroupEntityType.Value] = entityType;
            if (isOwnedByGroupMember.HasValue) data[OperationParam.GroupIsOwnedByGroupMember.Value] = isOwnedByGroupMember.Value;
            if (acl != null) data[OperationParam.GroupAcl.Value] = JsonReader.Deserialize(acl.ToJsonString());
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);

            SendRequest(ServiceOperation.CreateGroupEntity, success, failure, cbObject, data);
        }

        public void DeleteGroup(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.DeleteGroup, success, failure, cbObject, data);
        }

        public void DeleteGroupEntity(
            string groupId,
            string entityId,
            long version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupEntityId.Value] = entityId;
            data[OperationParam.GroupVersion.Value] = version;

            SendRequest(ServiceOperation.DeleteGroupEntity, success, failure, cbObject, data);
        }

        public void GetMyGroups(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRequest(ServiceOperation.GetMyGroups, success, failure, cbObject, null);
        }

        public void IncrementGroupData(
            string groupId,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);

            SendRequest(ServiceOperation.IncrementGroupData, success, failure, cbObject, data);
        }

        public void IncrementGroupEntityData(
            string groupId,
            string entityId,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupEntityId.Value] = entityId;
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);

            SendRequest(ServiceOperation.IncrementGroupEntityData, success, failure, cbObject, data);
        }

        public void InviteGroupMember(
            string groupId,
            string profileId,
            Role role,
            string jsonAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;
            data[OperationParam.GroupRole.Value] = role.ToString();
            if (!string.IsNullOrEmpty(jsonAttributes)) data[OperationParam.GroupAttributes.Value] = JsonReader.Deserialize(jsonAttributes);

            SendRequest(ServiceOperation.InviteGroupMember, success, failure, cbObject, data);
        }

        public void JoinGroup(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.JoinGroup, success, failure, cbObject, data);
        }

        public void LeaveGroup(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.LeaveGroup, success, failure, cbObject, data);
        }

        public void ListGroupsPage(
            string jsonContext,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupContext.Value] = JsonReader.Deserialize(jsonContext);

            SendRequest(ServiceOperation.ListGroupsPage, success, failure, cbObject, data);
        }

        public void ListGroupsPageByOffset(
            string context,
            int pageOffset,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupContext.Value] = context;
            data[OperationParam.GroupPageOffset.Value] = pageOffset;

            SendRequest(ServiceOperation.ListGroupsPageByOffset, success, failure, cbObject, data);
        }

        public void ListGroupsWithMember(
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupProfileId.Value] = profileId;

            SendRequest(ServiceOperation.ListGroupsWithMember, success, failure, cbObject, data);
        }

        public void ReadGroup(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.ReadGroup, success, failure, cbObject, data);
        }

        public void ReadGroupEntitiesPage(
            string jsonContext,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupContext.Value] = JsonReader.Deserialize(jsonContext);

            SendRequest(ServiceOperation.ReadGroupEntitiesPage, success, failure, cbObject, data);
        }

        public void ReadGroupEntitiesPageByOffset(
            string context,
            int pageOffset,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupContext.Value] = context;
            data[OperationParam.GroupPageOffset.Value] = pageOffset;

            SendRequest(ServiceOperation.ReadGroupEntitiesPageByOffset, success, failure, cbObject, data);
        }

        public void ReadGroupEntity(
            string groupId,
            string entityId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupEntityId.Value] = entityId;

            SendRequest(ServiceOperation.ReadGroupEntity, success, failure, cbObject, data);
        }

        public void ReadGroupMembers(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.ReadGroupMembers, success, failure, cbObject, data);
        }

        public void RejectGroupInvitation(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.RejectGroupInvitation, success, failure, cbObject, data);
        }

        public void RejectGroupJoinRequest(
            string groupId,
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;

            SendRequest(ServiceOperation.RejectGroupJoinRequest, success, failure, cbObject, data);
        }

        public void RemoveGroupMember(
            string groupId,
            string profileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;

            SendRequest(ServiceOperation.RemoveGroupMember, success, failure, cbObject, data);
        }

        public void UpdateGroupData(
            string groupId,
            long version,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupVersion.Value] = version;
            data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);

            SendRequest(ServiceOperation.UpdateGroupData, success, failure, cbObject, data);
        }

        public void UpdateGroupEntity(
            string groupId,
            string entityId,
            long version,
            string jsonData,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupEntityId.Value] = entityId;
            data[OperationParam.GroupVersion.Value] = version;
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);

            SendRequest(ServiceOperation.UpdateGroupEntity, success, failure, cbObject, data);
        }

        public void UpdateGroupMember(
            string groupId,
            string profileId,
            Role? role,
            string jsonAttributes,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupProfileId.Value] = profileId;
            if (role.HasValue) data[OperationParam.GroupRole.Value] = role.Value.ToString();
            if (!string.IsNullOrEmpty(jsonAttributes)) data[OperationParam.GroupAttributes.Value] = JsonReader.Deserialize(jsonAttributes);

            SendRequest(ServiceOperation.UpdateGroupMember, success, failure, cbObject, data);
        }

        public void UpdateGroupName(
            string groupId,
            string name,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupName.Value] = name;

            SendRequest(ServiceOperation.UpdateGroupName, success, failure, cbObject, data);
        }


        private void SendRequest(ServiceOperation operation, SuccessCallback success, FailureCallback failure, object cbObject, IDictionary data)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Group, operation, data, callback);
            _bcClient.SendRequest(sc);
        }
    }
}