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
            MEMBER,
            OTHER
        }

        public enum AutoJoinStrategy
        {
            JoinFirstGroup,
            JoinRandomGroup
        }

        private BrainCloudClient _bcClient;

        public BrainCloudGroup(BrainCloudClient client)
        {
            _bcClient = client;
        }

        /// <summary>
        /// Accept an outstanding invitation to join the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - ACCEPT_GROUP_INVITATION
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Add a member to the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - ADD_GROUP_MEMBER
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the member being added.
        /// </param>
        /// <param name="role">
        /// Role of the member being added.
        /// </param>
        /// <param name="jsonAttributes">
        /// Attributes of the member being added.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Approve an outstanding request to join the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - APPROVE_GROUP_JOIN_REQUEST
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the invitation being deleted.
        /// </param>
        /// <param name="role">
        /// Role of the member being invited.
        /// <param name="jsonAttributes">
        /// Attributes of the member being invited.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Automatically join an open group that matches the search criteria and has space available.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - AUTO_JOIN_GROUP
        /// </remarks>
        /// <param name="groupType">
        /// Name of the associated group type.
        /// </param>
        /// <param name="autoJoinStrategy">
        /// Selection strategy to employ when there are multiple matches
        /// </param>
        /// <param name="dataQueryJson">
        /// Query parameters (optional)
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void AutoJoinGroup(
            string groupType,
            AutoJoinStrategy autoJoinStrategy,
            string dataQueryJson,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupType.Value] = groupType;
            data[OperationParam.GroupAutoJoinStrategy.Value] = autoJoinStrategy.ToString();

            if (Util.IsOptionalParameterValid(dataQueryJson))
                data[OperationParam.GroupWhere.Value] = dataQueryJson;

            SendRequest(ServiceOperation.AutoJoinGroup, success, failure, cbObject, data);
        }

        /// <summary>
        /// Cancel an outstanding invitation to the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - CANCEL_GROUP_INVITATION
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the invitation being deleted.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Create a group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - CREATE_GROUP
        /// </remarks>
        /// <param name="name">
        /// Name of the group.
        /// </param>
        /// <param name="groupType">
        /// Name of the type of group.
        /// </param>
        /// <param name="isOpenGroup">
        /// true if group is open; false if closed.
        /// </param>
        /// <param name="acl">
        /// The group's access control list. A null ACL implies default.
        /// </param>
        /// <param name="jsonOwnerAttributes">
        /// Attributes for the group owner (current user).
        /// </param>
        /// <param name="jsonDefaultMemberAttributes">
        /// Default attributes for group members.
        /// </param>
        /// <param name="jsonData">
        /// Custom application data.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void CreateGroup(
            string name,
            string groupType,
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
            data[OperationParam.GroupType.Value] = groupType;
            if (isOpenGroup.HasValue) data[OperationParam.GroupIsOpenGroup.Value] = isOpenGroup.Value;
            if (acl != null) data[OperationParam.GroupAcl.Value] = JsonReader.Deserialize(acl.ToJsonString());
            if (!string.IsNullOrEmpty(jsonData)) data[OperationParam.GroupData.Value] = JsonReader.Deserialize(jsonData);
            if (!string.IsNullOrEmpty(jsonOwnerAttributes))
                data[OperationParam.GroupOwnerAttributes.Value] = JsonReader.Deserialize(jsonOwnerAttributes);
            if (!string.IsNullOrEmpty(jsonDefaultMemberAttributes))
                data[OperationParam.GroupDefaultMemberAttributes.Value] = JsonReader.Deserialize(jsonDefaultMemberAttributes);

            SendRequest(ServiceOperation.CreateGroup, success, failure, cbObject, data);
        }

        /// <summary>
        /// Create a group entity.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - CREATE_GROUP_ENTITY
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="isOwnedByGroupMember">
        /// true if entity is owned by a member; false if owned by the entire group.
        /// </param>
        /// <param name="type">
        /// Type of the group entity.
        /// </param>
        /// <param name="acl">
        /// Access control list for the group entity.
        /// </param>
        /// <param name="jsonData">
        /// Custom application data.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Delete a group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - DELETE_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="version">
        /// Current version of the group
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void DeleteGroup(
            string groupId,
            long version,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupVersion.Value] = version;

            SendRequest(ServiceOperation.DeleteGroup, success, failure, cbObject, data);
        }

        /// <summary>
        /// Delete a group entity.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - DELETE_GROUP_ENTITY
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="entityId">
        /// ID of the entity.
        /// </param>
        /// <param name="version">
        /// The current version of the group entity (for concurrency checking).
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read information on groups to which the current user belongs.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - GET_MY_GROUPS
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void GetMyGroups(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            SendRequest(ServiceOperation.GetMyGroups, success, failure, cbObject, null);
        }

        /// <summary>
        /// Increment elements for the group's data field.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - INCREMENT_GROUP_DATA
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="jsonData">
        /// Partial data map with incremental values.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Increment elements for the group entity's data field.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - INCREMENT_GROUP_ENTITY_DATA
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="entityId">
        /// ID of the entity.
        /// </param>
        /// <param name="jsonData">
        /// Partial data map with incremental values.
        /// </param> 
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Invite a member to the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - INVITE_GROUP_MEMBER
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the member being invited.
        /// </param>
        /// <param name="role">
        /// Role of the member being invited.
        /// </param>
        /// <param name="jsonAttributes">
        /// Attributes of the member being invited.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Join an open group or request to join a closed group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - JOIN_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Leave a group in which the user is a member.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LEAVE_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Retrieve a page of group summary information based on the specified context.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LIST_GROUPS_PAGE
        /// </remarks>
        /// <param name="jsonContext">
        /// Query context.
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Retrieve a page of group summary information based on the encoded context 
        /// and specified page offset.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LIST_GROUPS_PAGE_BY_OFFSET
        /// </remarks>
        /// <param name="context">
        /// Encoded reference query context.
        /// </param>
        /// <param name="pageOffset">
        /// Number of pages by which to offset the query.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read information on groups to which the specified user belongs.  Access is subject to restrictions.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LIST_GROUPS_WITH_MEMBER
        /// </remarks>
        /// <param name="profileId">
        /// User to read groups for
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read the specified group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read the data of the specified group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP_DATA
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void ReadGroupData(
            string groupId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;

            SendRequest(ServiceOperation.ReadGroupData, success, failure, cbObject, data);
        }

        /// <summary>
        /// Read a page of group entity information.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP_ENTITIES_PAGE
        /// </remarks>
        /// <param name="jsonContext">
        /// Query context.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read a page of group entity information.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP_ENTITIES_PAGE_BY_OFFSET
        /// </remarks>
        /// <param name="encodedContext">
        /// Encoded reference query context.
        /// </param>
        /// <param name="pageOffset">
        /// Number of pages by which to offset the query.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void ReadGroupEntitiesPageByOffset(
            string encodedContext,
            int pageOffset,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupContext.Value] = encodedContext;
            data[OperationParam.GroupPageOffset.Value] = pageOffset;

            SendRequest(ServiceOperation.ReadGroupEntitiesPageByOffset, success, failure, cbObject, data);
        }

        /// <summary>
        /// Read the specified group entity.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP_ENTITY
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="entityId">
        /// ID of the entity.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Read the members of the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_MEMBERS_OF_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Reject an outstanding invitation to join the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - REJECT_GROUP_INVITATION
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Reject an outstanding request to join the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - REJECT_GROUP_JOIN_REQUEST
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the invitation being deleted.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Remove a member from the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - REMOVE_GROUP_MEMBER
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the member being deleted.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Updates a group's data.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - UPDATE_GROUP_DATA
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="version">
        /// Version to verify.
        /// </param>
        /// <param name="jsonData">
        /// Data to apply.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Update a group entity.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - UPDATE_GROUP_ENTITY_DATA
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="entityId">
        /// ID of the entity.
        /// </param>
        /// <param name="version">
        /// The current version of the group entity (for concurrency checking).
        /// </param>
        /// <param name="jsonData">
        /// Custom application data.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void UpdateGroupEntityData(
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

        /// <summary>
        /// Update a member of the group.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - UPDATE_GROUP_MEMBER
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="profileId">
        /// Profile ID of the member being updated.
        /// </param>
        /// <param name="role">
        /// Role of the member being updated (optional).
        /// </param>
        /// <param name="jsonAttributes">
        /// Attributes of the member being updated (optional).
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// Updates a group's name.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - UPDATE_GROUP_NAME
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="name">
        /// Name to apply.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
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

        /// <summary>
        /// set a group to be open true or false
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - SET_GROUP_OPEN
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="isOpenGroup">
        /// true or false if a group is open.
        /// </param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        public void SetGroupOpen(
            string groupId,
            bool isOpenGroup,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupIsOpenGroup.Value] = isOpenGroup;

            SendRequest(ServiceOperation.SetGroupOpen, success, failure, cbObject, data);
        }

        private void SendRequest(ServiceOperation operation, SuccessCallback success, FailureCallback failure, object cbObject, IDictionary data)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.Group, operation, data, callback);
            _bcClient.SendRequest(sc);
        }
    }
}