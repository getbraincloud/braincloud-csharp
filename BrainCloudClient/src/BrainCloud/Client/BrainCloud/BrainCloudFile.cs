//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using BrainCloud.Internal;
using System.Collections.Generic;
using System.IO;
using System;

namespace BrainCloud
{
    public class BrainCloudFile
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudFile(BrainCloudClient brainCloudClientRef)
        {
            m_brainCloudClientRef = brainCloudClientRef;
        }

        /// <summary>
        /// Prepares a user file upload. On success the file will begin uploading
        /// to the brainCloud server.To be informed of success/failure of the upload
        /// register an IFileUploadCallback with the BrainCloudClient class.
        /// </summary>
        /// <param name="cloudPath">The desired cloud path of the file</param>
        /// <param name="cloudFilename">The desired cloud fileName of the file</param>
        /// <param name="shareable">True if the file is shareable</param>
        /// <param name="replaceIfExists">Whether to replace file if it exists</param>
        /// <param name="localPath">The path and fileName of the local file</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        /// <returns> A bool which is false if the file cannot be found, or file size cannot be determind.
        /// Otherwise the JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "fileDetails": {
        ///             "updatedAt": 1452616408147,
        ///             "fileSize": 100,
        ///             "fileType": "User",
        ///             "expiresAt": 1452702808146,
        ///             "shareable": true,
        ///             "uploadId": "cf9a075c-587e-4bd1-af0b-eab1a79b958f",
        ///             "createdAt": 1452616408147,
        ///             "profileId": "bf8a8733-62d2-448e-b396-f3dbffff44",
        ///             "gameId": "99999",
        ///             "path": "dir1/dir2",
        ///             "fileName": "fileName",
        ///             "replaceIfExists": true,
        ///             "cloudPath": "bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/dir1/dir2/fileName"
        ///         }
        ///     }
        /// }
        /// </returns>
        public bool UploadFile(
            string cloudPath,
            string cloudFilename,
            bool shareable,
            bool replaceIfExists,
            string localPath,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
#if UNITY_WEBPLAYER || UNITY_WEBGL
            throw new Exception("File upload API is not supported on Web builds");
#else
            FileInfo info = new FileInfo(localPath);

            if (!info.Exists)
            {
                m_brainCloudClientRef.Log("File at " + localPath + " does not exist");
                return false;
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.UploadLocalPath.Value] = localPath;
            data[OperationParam.UploadCloudFilename.Value] = cloudFilename;
            data[OperationParam.UploadCloudPath.Value] = cloudPath;
            data[OperationParam.UploadShareable.Value] = shareable;
            data[OperationParam.UploadReplaceIfExists.Value] = replaceIfExists;
            data[OperationParam.UploadFileSize.Value] = info.Length;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.File, ServiceOperation.PrepareUserUpload, data, callback);
            m_brainCloudClientRef.SendRequest(sc);

            return true;
#endif
        }

        /// <summary>
        /// Method cancels an upload. If an IFileUploadCallback has been registered with the BrainCloudClient class,
        /// the fileUploadFailed callback method will be called once the upload has been canceled.
        /// NOTE: The upload will still continue in the background on versions of Unity before 5.3
        /// and on Unity mobile platforms.
        /// </summary>
        /// <param name="uploadId">Upload ID of the file to cancel</param>
        public void CancelUpload(string uploadId)
        {
            m_brainCloudClientRef.Comms.CancelUpload(uploadId);
        }

        /// <summary>
        /// Returns the progress of the given upload from 0.0 to 1.0 or -1 if upload not found.
        /// NOTE: This will always return 1 on Unity mobile platforms.
        /// </summary>
        /// <param name="uploadId">The id of the upload</param>
        /// <returns></returns>
        public double GetUploadProgress(string uploadId)
        {
            return m_brainCloudClientRef.Comms.GetUploadProgress(uploadId);
        }

        /// <summary>
        /// Returns the number of bytes uploaded or -1 if upload not found.
        /// NOTE: This will always return the total bytes to transfer on Unity mobile platforms.
        /// </summary>
        /// <param name="uploadId">The id of the upload</param>
        /// <returns>Total bytes transfered</returns>
        public long GetUploadBytesTransferred(string uploadId)
        {
            return m_brainCloudClientRef.Comms.GetUploadBytesTransferred(uploadId);
        }

        /// <summary>
        /// Returns the total number of bytes that will be uploaded or -1 if upload not found.
        /// </summary>
        /// <param name="uploadId">The id of the upload</param>
        /// <returns>Total bytes to transfer</returns>
        public long GetUploadTotalBytesToTransfer(string uploadId)
        {
            return m_brainCloudClientRef.Comms.GetUploadTotalBytesToTransfer(uploadId);
        }

        /// <summary>
        /// List all user files
        /// </summary>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "fileList": [{
        ///             "updatedAt": 1452603368201,
        ///             "uploadedAt": null,
        ///             "fileSize": 85470,
        ///             "shareable": true,
        ///             "createdAt": 1452603368201,
        ///             "profileId": "bf8a8733-62d2-448e-b396-f3dbffff44",
        ///             "gameId": "99999",
        ///             "path": "test2",
        ///             "fileName": "testup.dat",
        ///             "downloadUrl": "https://sharedprod.braincloudservers.com/s3/bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat",
        ///             "cloudLocation": "bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat"
        ///
        ///         }]
        ///     }
        /// }
        /// </returns>
        public void ListUserFiles(
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            ListUserFiles(null, null, success, failure, cbObject);
        }

        /// <summary>
        /// List user files from the given cloud path
        /// </summary>
        /// <param name="cloudPath">File path</param>
        /// <param name="recurse">Whether to recurse down the path</param>
        /// <param name="success">The success callback</param>
        /// <param name="failure">The failure callback</param>
        /// <param name="cbObject">The callback object</param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "fileList": [{
        ///             "updatedAt": 1452603368201,
        ///             "uploadedAt": null,
        ///             "fileSize": 85470,
        ///             "shareable": true,
        ///             "createdAt": 1452603368201,
        ///             "profileId": "bf8a8733-62d2-448e-b396-f3dbffff44",
        ///             "gameId": "99999",
        ///             "path": "test2",
        ///             "fileName": "testup.dat",
        ///             "downloadUrl": "https://sharedprod.braincloudservers.com/s3/bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat",
        ///             "cloudLocation": "bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat"
        ///
        ///         }]
        ///     }
        /// }
        /// </returns>
        public void ListUserFiles(
            string cloudPath,
            bool? recurse,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(cloudPath))
                data[OperationParam.UploadPath.Value] = cloudPath;
            if (recurse.HasValue)
                data[OperationParam.UploadRecurse.Value] = recurse.Value;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.File, ServiceOperation.ListUserFiles, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Deletes a single user file.
        /// </summary>
        /// <param name="cloudPath">File path</param>
        /// <param name="cloudFileName"></param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "fileDetails": {
        ///             "updatedAt": 1452603368201,
        ///             "uploadedAt": null,
        ///             "fileSize": 85470,
        ///             "shareable": true,
        ///             "createdAt": 1452603368201,
        ///             "profileId": "bf8a8733-62d2-448e-b396-f3dbffff44",
        ///             "gameId": "99999",
        ///             "path": "test2",
        ///             "fileName": "testup.dat",
        ///             "downloadUrl": "https://sharedprod.braincloudservers.com/s3/bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat",
        ///             "cloudLocation": "bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat"
        ///         }
        ///     }
        /// }
        /// </returns>
        public void DeleteUserFile(
            string cloudPath,
            string cloudFileName,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.UploadCloudPath.Value] = cloudPath;
            data[OperationParam.UploadCloudFilename.Value] = cloudFileName;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.File, ServiceOperation.DeleteUserFile, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Delete multiple user files
        /// </summary>
        /// <param name="cloudPath">File path</param>
        /// <param name="recurse">Whether to recurse down the path</param>
        /// <param name="success"></param>
        /// <param name="failure"></param>
        /// <param name="cbObject"></param>
        /// <returns> The JSON returned in the callback is as follows:
        /// {
        ///     "status": 200,
        ///     "data": {
        ///         "fileList": [{
        ///             "updatedAt": 1452603368201,
        ///             "uploadedAt": null,
        ///             "fileSize": 85470,
        ///             "shareable": true,
        ///             "createdAt": 1452603368201,
        ///             "profileId": "bf8a8733-62d2-448e-b396-f3dbffff44",
        ///             "gameId": "99999",
        ///             "path": "test2",
        ///             "fileName": "testup.dat",
        ///             "downloadUrl": "https://sharedprod.braincloudservers.com/s3/bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat",
        ///             "cloudLocation": "bc/g/99999/u/bf8a8733-62d2-448e-b396-f3dbffff44/f/test2/testup.dat"
        ///         }]
        ///     }
        /// }
        /// </returns>
        public void DeleteUserFiles(
            string cloudPath,
            bool recurse,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data[OperationParam.UploadCloudPath.Value] = cloudPath;
            data[OperationParam.UploadRecurse.Value] = recurse;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall sc = new ServerCall(ServiceName.File, ServiceOperation.DeleteUserFiles, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}

