//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fullPathFilename"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="version"></param>
        /// <param name="newTreeId"></param>
        /// <param name="treeVersion"></param>
        /// <param name="newFilename"></param>
        /// <param name="overwriteIfPresent"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="version"></param>
        /// <param name="filename"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="folderPath"></param>
        /// <param name="filename"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="folderPath"></param>
        /// <param name="recurse"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="version"></param>
        /// <param name="newTreeId"></param>
        /// <param name="treeVersion"></param>
        /// <param name="newFilename"></param>
        /// <param name="overwriteIfPresent"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="userCloudPath"></param>
        /// <param name="userCloudFilename"></param>
        /// <param name="groupId"></param>
        /// <param name="groupTreeId"> Folder ID within the folder structure of the group </param>
        /// <param name="groupFilename"></param>
        /// <param name="groupFileAcl"></param>
        /// <param name="overwriteIfPresent"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="fileId"></param>
        /// <param name="version"></param>
        /// <param name="newFilename"></param>
        /// <param name="newACL"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
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