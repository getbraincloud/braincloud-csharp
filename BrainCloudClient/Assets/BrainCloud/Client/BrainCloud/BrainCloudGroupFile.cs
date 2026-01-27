// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{
    using BrainCloud.Common;
    using BrainCloud.Internal;
    using BrainCloud.JsonFx.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System;

    public class BrainCloudGroupFile
    {
        private BrainCloudClient _bcClient;

        public BrainCloudGroupFile(BrainCloudClient client)
        {
            _bcClient = client;
        }
        /// <summary>
        /// Check if filename exists for provided path and name
        /// </summary>
        /// <param name="groupId">ID of the group.</param>
        /// <param name="folderPath">The path of the file</param>
        /// <param name="filename">The filename of the file</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void CheckFilenameExists(
            string groupId,
            string folderPath,
            string fileName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FolderPath.Value] = folderPath;
            data[OperationParam.FileName.Value] = fileName;

            SendRequest(ServiceOperation.CheckFilenameExists, success, failure, cbObject, data);
        }

        /// <summary>
        /// Check if filename exists for provided full path name
        /// </summary>
        /// <param name="groupId">ID of the group.</param>
        /// <param name="fullPathFilename">The full path of the file</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void CheckFullpathFilenameExists(
            string groupId,
            string fullPathFilename,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FullPathFilename.Value] = fullPathFilename;

            SendRequest(ServiceOperation.CheckFullpathFilenameExists, success, failure, cbObject, data);
        }

        /// <summary>
        /// Copy a file.
        /// </summary>
        /// <param name="groupId">ID of the group</param>
        /// <param name="fileId">ID of the file</param>
        /// <param name="version">Target version of the file</param>
        /// <param name="newTreeId">ID of the destination folder</param>
        /// <param name="treeVersion">Target version of the folder tree</param>
        /// <param name="newFilename">Optional new file name</param>
        /// <param name="overwriteIfPresent">Whether to allow overwrite of an existing file if present</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void CopyFile(
            string groupId,
            string fileId,
            int version,
            string newTreeId,
            int treeVersion,
            string newFilename,
            bool overwriteIfPresent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;
            data[OperationParam.Version.Value] = version;
            data[OperationParam.NewTreeId.Value] = newTreeId;
            data[OperationParam.TreeVersion.Value] = treeVersion;
            data[OperationParam.NewFilename.Value] = newFilename;
            data[OperationParam.OverwriteIfPresent.Value] = overwriteIfPresent;

            SendRequest(ServiceOperation.CopyFile, success, failure, cbObject, data);
        }

        /// <summary>
        /// Delete a file.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="fileId">the fileId</param>
        /// <param name="version">the version</param>
        /// <param name="newFilename">the newFilename</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void DeleteFile(
            string groupId,
            string fileId,
            int version,
            string filename,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;
            data[OperationParam.Version.Value] = version;
            data[OperationParam.FileName.Value] = filename;

            SendRequest(ServiceOperation.DeleteFile, success, failure, cbObject, data);
        }

        /// <summary>
        /// Return CDN url for file for clients that cannot handle redirect.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="fileId">the fileId</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void GetCDNUrl(
            string groupId,
            string fileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;

            SendRequest(ServiceOperation.GetCdnUrl, success, failure, cbObject, data);
        }

        /// <summary>
        /// Returns information on a file using fileId.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="fileId">the fileId</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void GetFileInfo(
            string groupId,
            string fileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;

            SendRequest(ServiceOperation.GetFileInfo, success, failure, cbObject, data);
        }

        /// <summary>
        /// Returns information on a file using path and name.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="folderPath">the folderPath</param>
        /// <param name="fileName">the fileName</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void GetFileInfoSimple(
            string groupId,
            string folderPath,
            string filename,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FolderPath.Value] = folderPath;
            data[OperationParam.FileName.Value] = filename;

            SendRequest(ServiceOperation.GetFileInfoSimple, success, failure, cbObject, data);
        }

        /// <summary>
        /// Returns a list of files.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="folderPath">the folderPath</param>
        /// <param name="recurse">true to recurse</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void GetFileList(
            string groupId,
            string folderPath,
            bool recurse,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FolderPath.Value] = folderPath;
            data[OperationParam.Recurse.Value] = recurse;

            SendRequest(ServiceOperation.GetFileList, success, failure, cbObject, data);
        }

        /// <summary>
        /// Move a file.
        /// </summary>
        /// <param name="groupId">the groupId</param>
        /// <param name="fileId">the fileId</param>
        /// <param name="version">the version</param>
        /// <param name="newTreeId">the newTreeId</param>
        /// <param name="newFilename">the newFilename</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void MoveFile(
            string groupId,
            string fileId,
            int version,
            string newTreeId,
            int treeVersion,
            string newFilename,
            bool overwriteIfPresent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;
            data[OperationParam.Version.Value] = version;
            data[OperationParam.NewTreeId.Value] = newTreeId;
            data[OperationParam.TreeVersion.Value] = treeVersion;
            data[OperationParam.NewFilename.Value] = newFilename;
            data[OperationParam.OverwriteIfPresent.Value] = overwriteIfPresent;

            SendRequest(ServiceOperation.MoveFile, success, failure, cbObject, data);
        }

        /// <summary>
        /// Move a file from user space to group space.
        /// </summary>
        /// <param name="userCloudPath">User file folder</param>
        /// <param name="userCloudFilename">User file name</param>
        /// <param name="groupId">ID of the group</param>
        /// <param name="groupTreeId">ID of the destination folder</param>
        /// <param name="groupFileName">Group file name</param>
        /// <param name="groupFileAcl">Acl of the new group file</param>
        /// <param name="overwriteIfPresent">Whether to allow overwrite of an existing file if present</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void MoveUserToGroupFile(
            string userCloudPath,
            string userCloudFilename,
            string groupId,
            string groupTreeId,
            string groupFilename,
            Dictionary<string, object> groupFileAcl,
            bool overwriteIfPresent,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UserCloudPath.Value] = userCloudPath;
            data[OperationParam.UserCloudFilename.Value] = userCloudFilename;
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.GroupTreeId.Value] = groupTreeId;
            data[OperationParam.GroupFilename.Value] = groupFilename;
            data[OperationParam.GroupFileACL.Value] = groupFileAcl;
            data[OperationParam.OverwriteIfPresent.Value] = overwriteIfPresent;

            SendRequest(ServiceOperation.MoveUserToGroupFile, success, failure, cbObject, data);
        }

        /// <summary>
        /// Rename or edit permissions of an uploaded file. Does not change the contents of the file.
        /// </summary>
        /// <param name="groupId">ID of the group</param>
        /// <param name="fileId">ID of the file</param>
        /// <param name="version">Target version of the file</param>
        /// <param name="newFilename">Optional new file name</param>
        /// <param name="newACL">Optional new acl</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>



        public void UpdateFileInfo(
            string groupId,
            string fileId,
            int version,
            string newFilename,
            Dictionary<string, object> newACL,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GroupId.Value] = groupId;
            data[OperationParam.FileId.Value] = fileId;
            data[OperationParam.Version.Value] = version;
            data[OperationParam.NewFilename.Value] = newFilename;
            data[OperationParam.NewACL.Value] = newACL;

            SendRequest(ServiceOperation.UpdateFileInfo, success, failure, cbObject, data);
        }

        private void SendRequest(ServiceOperation operation, SuccessCallback success, FailureCallback failure, object cbObject, IDictionary data)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GroupFile, operation, data, callback);
            _bcClient.SendRequest(sc);
        }
    }
}