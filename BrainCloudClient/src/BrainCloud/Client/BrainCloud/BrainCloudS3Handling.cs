//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using JsonFx.Json;
using BrainCloud.Internal;

namespace BrainCloud
{
    public class BrainCloudS3Handling
    {
        private BrainCloudClient m_brainCloudClientRef;

        public BrainCloudS3Handling(BrainCloudClient in_brainCloudClientRef)
        {
            m_brainCloudClientRef = in_brainCloudClientRef;
        }

        /// <summary>
        /// Sends an array of file details and returns 
        /// the details of any of those files that have changed
        /// </summary>
        /// <remarks>
        /// Service Name - S3Handling
        /// Service Operation - GetUpdatedFiles
        /// </remarks>
        /// <param name="in_category">  
        /// Category of files on server to compare against
        /// </param>
        /// <param name="in_fileDetailsJson">  
        /// An array of file details
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>  The JSON returned in the callback is as follows. 
        /// {
        ///     "status": 200,
        ///     "fileDetails": [
        ///         {
        ///             "gameId": "12311331",
        ///             "fileId": "3780516b-14f8-4055-8899-8eaab6ac7e82",
        ///             "shortName": "Test Doc",
        ///             "fileName": "testDoc.txt",
        ///             "type": "g",
        ///             "subType": "cust",
        ///             "category": null,
        ///             "fileSize": 4,
        ///             "dateUploaded": 1437154770000,
        ///             "relativeUrl": "/cust/testDoc.txt",
        ///             "absoluteUrl": "http://internal.braincloudservers.com/s3/portal/g/12311331/cust/testDoc.txt",
        ///             "md5Hash": "d41d8cd98f00b204e9800998ecf8427e"
        ///         }
        ///     ]
        /// }
        /// </returns>
        public void GetUpdatedFiles(
            string in_category,
            string in_fileDetailsJson,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_category))
            {
                data[OperationParam.S3HandlingServiceFileCategory.Value] = in_category;
            }

            data[OperationParam.S3HandlingServiceFileDetails.Value] = JsonReader.Deserialize<object[]>(in_fileDetailsJson);

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.S3Handling, ServiceOperation.GetUpdatedFiles, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }

        /// <summary>
        /// Retreives the detailds of custom files stored on the server
        /// </summary>
        /// <remarks>
        /// Service Name - S3Handling
        /// Service Operation - GetFileList
        /// </remarks>
        /// <param name="in_category">  
        /// Category of files to retrieve
        /// </param>
        /// <param name="in_success">
        /// The success callback.
        /// </param>
        /// <param name="in_failure">
        /// The failure callback.
        /// </param>
        /// <param name="in_cbObject">
        /// The user object sent to the callback.
        /// </param>
        /// <returns>  The JSON returned in the callback is as follows. 
        /// {
        ///     "status": 200,
        ///     "fileDetails": [
        ///         {
        ///             "gameId": "12311331",
        ///             "fileId": "3780516b-14f8-4055-8899-8eaab6ac7e82",
        ///             "shortName": "Test Doc",
        ///             "fileName": "testDoc.txt",
        ///             "type": "g",
        ///             "subType": "cust",
        ///             "category": null,
        ///             "fileSize": 4,
        ///             "dateUploaded": 1437154770000,
        ///             "relativeUrl": "/cust/testDoc.txt",
        ///             "absoluteUrl": "http://internal.braincloudservers.com/s3/portal/g/12311331/cust/testDoc.txt",
        ///             "md5Hash": "d41d8cd98f00b204e9800998ecf8427e"
        ///         }
        ///     ]
        /// }
        /// </returns>
        public void GetFileList(
            string in_category,
            SuccessCallback in_success = null,
            FailureCallback in_failure = null,
            object in_cbObject = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (Util.IsOptionalParameterValid(in_category))
            {
                data[OperationParam.S3HandlingServiceFileCategory.Value] = in_category;
            }

            ServerCallback callback = BrainCloudClient.CreateServerCallback(in_success, in_failure, in_cbObject);
            ServerCall sc = new ServerCall(ServiceName.S3Handling, ServiceOperation.GetFileList, data, callback);
            m_brainCloudClientRef.SendRequest(sc);
        }
    }
}