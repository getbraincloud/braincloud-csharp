// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using BrainCloud.JsonFx.Json;
    using BrainCloud.Internal;

    public class BrainCloudGlobalFile
    {
        private BrainCloudClient _client;

        public BrainCloudGlobalFile(BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns the complete info for the specified file given it’s fileId
        /// </summary>
        /// <remarks>
        /// Service Name - globalFileV3
        /// Service Operation - GET_FILE_INFO
        /// </remarks>
        /// <param name="fileId">The fileId of the global file</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetFileInfo(
            string fileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalFileServiceFileId.Value] = fileId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalFile, ServiceOperation.GetFileInfo, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Returns the complete info for the specified file, without having to look up the fileId first.
        /// </summary>
        /// <remarks>
        /// Service Name - globalFileV3
        /// Service Operation - GET_FILE_INFO_SIMPLE
        /// </remarks>
        /// <param name="folderPath">The folder path of the file</param>
        /// <param name="filename">The name of the file</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetFileInfoSimple(
            string folderPath,
            string filename,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalFileServiceFolderPath.Value] = folderPath;
            data[OperationParam.GlobalFileServiceFileName.Value] = filename;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalFile, ServiceOperation.GetFileInfoSimple, data, callback);
            _client.SendRequest(serverCall);
        }
        /// <summary>
        /// Returns the CDN of the specified file.
        /// </summary>
        /// <remarks>
        /// Service Name - globalFileV3
        /// Service Operation - GET_GLOBAL_CDN_URL
        /// </remarks>
        /// <param name="fileId">The fileId of the global file</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetGlobalCDNUrl(
            string fileId,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalFileServiceFileId.Value] = fileId;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalFile, ServiceOperation.GetGlobalCDNUrl, data, callback);
            _client.SendRequest(serverCall);
        }

        /// <summary>
        /// Returns files at the current path.
        /// </summary>
        /// <remarks>
        /// Service Name - globalFileV3
        /// Service Operation - GET_GLOBAL_FILE_LIST
        /// </remarks>
        /// <param name="folderPath">The folder path to list files from</param>
        /// <param name="recurse">Whether to recurse into subfolders</param>
        /// <param name="success">The success callback.</param>
        /// <param name="failure">The failure callback.</param>
        /// <param name="cbObject">The user object sent to the callback.</param>

        public void GetGlobalFileList(
            string folderPath,
            bool recurse,
            SuccessCallback success = null,
            FailureCallback failure = null,
            object cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data[OperationParam.GlobalFileServiceFolderPath.Value] = folderPath;
            data[OperationParam.GlobalFileServiceRecurse.Value] = recurse;

            ServerCallback callback = BrainCloudClient.CreateServerCallback(success, failure, cbObject);
            ServerCall serverCall = new ServerCall(ServiceName.GlobalFile, ServiceOperation.GetGlobalFileList, data, callback);
            _client.SendRequest(serverCall);
        }

    }
}
