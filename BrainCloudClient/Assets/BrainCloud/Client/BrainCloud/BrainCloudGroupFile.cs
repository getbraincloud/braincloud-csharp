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

        public void MoveUserToGroupFile(
            string userCloudPath,
            string userCloudFilename,
            string groupId,
            string groupTreeId,
            string groupFilename,
            GroupACL groupFileAcl,
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

        public void UpdateFileInfo(
            string groupId,
            string fileId,
            int version,
            string newFilename,
            GroupACL newACL,
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
            
            SendRequest(ServiceOperation.MoveFile, success, failure, cbObject, data);
        }
        
        private void SendRequest(ServiceOperation operation, SuccessCallback success, FailureCallback failure, object cbObject, IDictionary data)
        {
            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.GroupFile, operation, data, callback);
            _bcClient.SendRequest(sc);
        }
    }
}