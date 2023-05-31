//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
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

        public BrainCloudGlobalFile (BrainCloudClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Returns information on a file using fileId.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalFile
        /// Service Operation - GetFileInfo
        /// <param name="fileId">
        /// The Id of the file
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// Returns information on a file using path and name
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalFile
        /// Service Operation - GetFileInfoSimple
        /// <param name="folderPath">
        /// The folderpath
        /// </param>
        /// <param name="filename">
        /// The filename
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// Return CDN url for file for clients that cannot handle redirect.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalFile
        /// Service Operation - GetGlobalCDNUrl
        /// <param name="fileId">
        /// The Id of the file
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
        /// Returns a list of files.
        /// </summary>
        /// <remarks>
        /// Service Name - GlobalFile
        /// Service Operation - GetGlobalFileList
        /// <param name="folderPath">
        /// The folderpath
        /// </param>
        /// <param name="recurse">
        /// do we recurse?
        /// </param>
        /// <param name="success">
        /// The success callback.
        /// </param>
        /// <param name="failure">
        /// The failure callback.
        /// </param>
        /// <param name="cbObject">
        /// The user object sent to the callback.
        /// </param>
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
