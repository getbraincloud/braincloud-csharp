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

        private BrainCloudClient _bcClient;

        public BrainCloudGroup(BrainCloudClient brainCloudClientRef)
        {
            _bcClient = brainCloudClientRef;
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// Attributes for the group owner (current player).
        /// </param>
        /// <param name="jsonDefaultMemberAttributes">
        /// Default attributes for group members.
        /// </param>
        /// <param name="jsonData">
        /// Custom application data.
        /// </param>
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "gameId": "20595",
        ///     "groupId": "211559ef-234a-4aef-a643-48a90a6036c2",
        ///     "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///     "name": "my-group-name",
        ///     "groupType": "TestGroup",
        ///     "createdAt": 1461613090251,
        ///     "updatedAt": 1461613090251,
        ///     "members": {
        ///         "ee8cad26-16f2-4ef8-9045-3aab84ce6362": {
        ///             "role": "OWNER",
        ///             "attributes": {}
        ///         }
        ///     },
        ///     "pendingMembers": {},
        ///     "version": 1,
        ///     "data": {},
        ///     "isOpenGroup": false,
        ///     "defaultMemberAttributes": {},
        ///     "memberCount": 1,
        ///     "invitedPendingMemberCount": 0,
        ///     "requestingPendingMemberCount": 0,
        ///     "acl": {
        ///         "member": 2,
        ///         "other": 1
        ///     }
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "gameId": "20595",
        ///     "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///     "entityId": "91cfece7-debb-4698-ba6b-cd2cb432458d",
        ///     "ownerId": null,
        ///     "entityType": "BLUE",
        ///     "createdAt": 1462812680359,
        ///     "updatedAt": 1462812680359,
        ///     "version": 1,
        ///     "data": {},
        ///     "acl": {
        ///         "member": 2,
        ///         "other": 1
        ///     }
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// Read information on groups to which the current player belongs.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - GET_MY_GROUPS
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "groups": [{
        ///         "gameId": "20595",
        ///         "groupId": "4f176781-e65e-42ce-b24f-9d39449380d5",
        ///         "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///         "name": "temp-group-name",
        ///         "groupType": "test2",
        ///         "createdAt": 1462222320554,
        ///         "updatedAt": 1462222320554,
        ///         "members": {
        ///             "ee8cad26-16f2-4ef8-9045-3aab84ce6362": {
        ///                 "role": "OWNER",
        ///                 "attributes": {}
        ///             }
        ///         },
        ///         "pendingMembers": {},
        ///         "version": 1,
        ///         "data": {},
        ///         "isOpenGroup": false,
        ///         "defaultMemberAttributes": {},
        ///         "memberCount": 1,
        ///         "invitedPendingMemberCount": 0,
        ///         "requestingPendingMemberCount": 0,
        ///         "acl": {
        ///             "other": 1
        ///         }
        ///     }]
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// Leave a group in which the player is a member.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LEAVE_GROUP
        /// </remarks>
        /// <param name="groupId">
        /// ID of the group.
        /// </param>
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// Read a page of group information.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LIST_GROUPS_PAGE
        /// </remarks>
        /// <param name="context">
        /// Query context.
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "results": {
        ///         "moreBefore": false,
        ///         "count": 4,
        ///         "items": [{
        ///             "groupType": "test2",
        ///             "groupId": "4f176781-e65e-42ce-b24f-9d39449380d5",
        ///             "isOpenGroup": false,
        ///             "requestingPendingMemberCount": 0,
        ///             "invitedPendingMemberCount": 0,
        ///             "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///             "name": "temp-group-name",
        ///             "memberCount": 1
        ///         }, {
        ///             "groupType": "test2",
        ///             "groupId": "bcdaa4b3-f26e-47ad-813f-6506a1f4949d",
        ///             "isOpenGroup": false,
        ///             "requestingPendingMemberCount": 0,
        ///             "invitedPendingMemberCount": 0,
        ///             "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///             "name": "group-1",
        ///             "memberCount": 1
        ///         }],
        ///         "page": 1,
        ///         "moreAfter": true
        ///     },
        ///     "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJnYW1lSWQiOiIyMDU5NSJ9LCJz"
        /// }
        /// </returns>
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
        /// Read a page of group information.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - LIST_GROUPS_PAGE_BY_OFFSET
        /// </remarks>
        /// <param name="encodedContext">
        /// Encoded reference query context.
        /// </param>
        /// <param name="offset">
        /// Number of pages by which to offset the query.
        /// </param>
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "results": {
        ///         "moreBefore": true,
        ///         "count": 4,
        ///         "items": [{
        ///             "groupType": "test2",
        ///             "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///             "isOpenGroup": false,
        ///             "requestingPendingMemberCount": 0,
        ///             "invitedPendingMemberCount": 0,
        ///             "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///             "name": "group-1",
        ///             "memberCount": 2
        ///         }],
        ///         "page": 2,
        ///         "moreAfter": false
        ///     },
        ///     "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJnYW1lSWQiOiIyMDU5NSJ9LCJzb3J0Q3J"
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "groups": [{
        ///         "groupType": "test2",
        ///         "groupId": "4f176781-e65e-42ce-b24f-9d39449380d5",
        ///         "isOpenGroup": false,
        ///         "requestingPendingMemberCount": 0,
        ///         "invitedPendingMemberCount": 0,
        ///         "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///         "name": "temp-group-name",
        ///         "memberCount": 1
        ///     }, {
        ///         "groupType": "test2",
        ///         "groupId": "b2c49711-4e7f-4c18-92e1-cdf9634c2f89",
        ///         "isOpenGroup": false,
        ///         "requestingPendingMemberCount": 0,
        ///         "invitedPendingMemberCount": 0,
        ///         "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///         "name": "my-group-name",
        ///         "memberCount": 1
        ///     }]
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "gameId": "20595",
        ///     "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///     "ownerId": "ee8cad26-16f2-4ef8-9045-3aab84ce6362",
        ///     "name": "group-1",
        ///     "groupType": "test2",
        ///     "createdAt": 1462223553243,
        ///     "updatedAt": 1462223553243,
        ///     "members": {
        ///         "ee8cad26-16f2-4ef8-9045-3aab84ce6362": {
        ///             "role": "OWNER",
        ///             "attributes": {}
        ///         },
        ///         "295c510f-507f-4bcf-80e1-ebc73708ec3c": {
        ///             "role": "MEMBER",
        ///             "attributes": {}
        ///         }
        ///     },
        ///     "pendingMembers": {},
        ///     "version": 1,
        ///     "data": {},
        ///     "isOpenGroup": false,
        ///     "defaultMemberAttributes": {},
        ///     "memberCount": 2,
        ///     "invitedPendingMemberCount": 0,
        ///     "requestingPendingMemberCount": 0,
        ///     "acl": {
        ///         "member": 2,
        ///         "other": 1
        ///     }
        /// }
        /// </returns>
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
        /// Read a page of group entity information.
        /// </summary>
        /// <remarks>
        /// Service Name - group
        /// Service Operation - READ_GROUP_ENTITIES_PAGE
        /// </remarks>
        /// <param name="context">
        /// Query context.
        /// </param>
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        ///
        /// @return Object encapsulating the page of group entity information.
        ///
        /// {
        ///     "results": {
        ///         "moreBefore": false,
        ///         "count": 3,
        ///         "items": [{
        ///             "gameId": "20595",
        ///             "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///             "entityId": "91cfece7-debb-4698-ba6b-cd2cb432458d",
        ///             "ownerId": null,
        ///             "entityType": "BLUE",
        ///             "createdAt": 1462812680359,
        ///             "updatedAt": 1462812680359,
        ///             "version": 1,
        ///             "data": {},
        ///             "acl": {
        ///                 "member": 2,
        ///                 "other": 1
        ///             }
        ///         }],
        ///         "page": 1,
        ///         "moreAfter": true
        ///     },
        ///     "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJncm91cElkIjoiZmVlNTVhMzct"
        /// }
        /// </returns>
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
        /// <param name="offset">
        /// Number of pages by which to offset the query.
        /// </param>
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        ///
        /// @return Object encapsulating the page of group entity information.
        /// {
        ///     "results": {
        ///         "moreBefore": true,
        ///         "count": 3,
        ///         "items": [{
        ///             "gameId": "20595",
        ///             "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///             "entityId": "ccbf996c-9e96-4935-b570-eebaab81c75a",
        ///             "ownerId": null,
        ///             "entityType": "RED",
        ///             "createdAt": 1462812845384,
        ///             "updatedAt": 1462812845384,
        ///             "version": 1,
        ///             "data": {
        ///                 "third": true
        ///             },
        ///             "acl": {
        ///                 "member": 2,
        ///                 "other": 1
        ///             }
        ///         }],
        ///         "page": 2,
        ///         "moreAfter": false
        ///     },
        ///     "context": "eyJzZWFyY2hDcml0ZXJpYSI6eyJncm91cElkIjoiZmVlNTVhMzctNWU4Ni00M2U4LTk"
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "gameId": "20595",
        ///     "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///     "entityId": "91cfece7-debb-4698-ba6b-cd2cb432458d",
        ///     "ownerId": null,
        ///     "entityType": "BLUE",
        ///     "createdAt": 1462812680359,
        ///     "updatedAt": 1462812680359,
        ///     "version": 1,
        ///     "data": {},
        ///     "acl": {
        ///         "member": 2,
        ///         "other": 1
        ///     }
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "ee8cad26-16f2-4ef8-9045-3aab84ce6362": {
        ///         "role": "OWNER",
        ///         "attributes": {},
        ///         "playerName": "Peter",
        ///         "emailAddress": "klug@bitheads.com"
        ///     },
        ///     "295c510f-507f-4bcf-80e1-ebc73708ec3c": {
        ///         "role": "MEMBER",
        ///         "attributes": {},
        ///         "playerName": "Billy",
        ///         "emailAddress": "billy@bitheads.com"
        ///     }
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "gameId": "20595",
        ///     "groupId": "fee55a37-5e86-43e8-942e-06bcbe1b701e",
        ///     "entityId": "84b36406-6cc5-4417-9143-3ee136e0586f",
        ///     "ownerId": null,
        ///     "entityType": "BLUE",
        ///     "createdAt": 1462812827651,
        ///     "updatedAt": 1462812827651,
        ///     "version": 2,
        ///     "data": {
        ///         "second": true,
        ///         "plus": "and then some"
        ///     },
        ///     "acl": {
        ///         "member": 2,
        ///         "other": 1
        ///     }
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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
        /// <param name="callback">
        /// The method to be invoked when the server response is received
        /// </param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": null
        /// }
        /// </returns>
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