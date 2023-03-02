using System.Collections;
using System.Collections.Generic;
using System.IO;
using BrainCloud;
using BrainCloud.Common;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestGroupFile : TestFixtureBase
    {
        /*
         * Testing Note: I'm using a specific user called unity_tester
         * For other languages, they should use this same group but a different user. I think...
         */
        
        //Information grabbed from internal servers -> Unit Test Master
        private string folderPath = "";
        //id for testingGroupFile.dat
        private string groupFileId = "f92a7231-2257-4e2d-899c-79c47a8a5215";
        private string groupID = "a7ff751c-3251-407a-b2fd-2bd1e9bca64a";
        private bool recurse = true;
        //Making version a negative value to tell the server to use the latest version
        private int version = -1;
        private bool _shareable = true;
        private bool _replaceIfExists = true;
        private string _cloudPath = "";
        private int _returnCount;
        private string filename = "testingGroupFile.dat";
        private string tempFilename = "deleteThisFileAfter.dat";
        private string updatedName = "UpdatedGroupFile.dat";
        private Dictionary<string, object> acl = new Dictionary<string, object> {{"other", 0}, {"member", 2}};


        [UnityTest]
        public IEnumerator TestCheckFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.CheckFilenameExists(groupID, folderPath, filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            bool doesFileExist = ((bool)((Dictionary<string, object>)_tc.m_response["data"])["exists"]);
            LogResults("File doesn't exist anymore..", doesFileExist);
        }

        [UnityTest]
        public IEnumerator TestCheckFullpathFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.CheckFullpathFilenameExists(groupID, filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            bool doesFileExist = ((bool)((Dictionary<string, object>)_tc.m_response["data"])["exists"]);
            LogResults("File doesn't exist anymore..", doesFileExist);
        }

        [UnityTest]
        public IEnumerator TestGetFileInfo()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.GetFileInfo(groupID, groupFileId, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetFileInfoSimple()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.GetFileInfoSimple(groupID, "", filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetFileList()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.GetFileList(groupID, "", recurse, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            LogResults("Something went wrong..", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestGetCDNUrl()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            _tc.bcWrapper.GroupFileService.GetCDNUrl(groupID, groupFileId, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestMoveFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));

            _tc.bcWrapper.GroupFileService.MoveFile
            (
                groupID,
                groupFileId,
                version,
                "",
                -1,
                updatedName,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Reverting file name for other tests to use
            _tc.bcWrapper.GroupFileService.MoveFile
            (
                groupID,
                groupFileId,
                version,
                "",
                -1,
                filename,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Couldn't move file..", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestMoveUserToGroupFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            
            //Upload a fresh file
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            FileInfo info = new FileInfo(CreateFile(4024));
            _cloudPath = "TestFolder";
            _tc.bcWrapper.FileService.UploadFile
            (
                _cloudPath,
                info.Name,
                _shareable,
                _replaceIfExists,
                info.FullName,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) });
            
            //Moving new file to group file
            _tc.bcWrapper.GroupFileService.MoveUserToGroupFile
            (
                "TestFolder/",
                tempFilename,
                groupID,
                "",
                tempFilename,
                acl,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)_tc.m_response["data"])["fileDetails"]);
            string newFileId = (string) fileDetails["fileId"];
            
            //Delete new file
            _tc.bcWrapper.GroupFileService.DeleteFile
            (
                groupID,
                newFileId,
                version,
                tempFilename,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to update file info", _tc.successCount == 4);
        }

        [UnityTest]
        public IEnumerator TestCopyFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            string newFileName = "testCopiedFile.dat";
            _tc.bcWrapper.GroupFileService.CopyFile
            (
                groupID,
                groupFileId,
                version,
                "",
                0,
                newFileName,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)_tc.m_response["data"])["fileDetails"]);
            string newFileId = (string) fileDetails["fileId"];
            
            //Delete new file
            _tc.bcWrapper.GroupFileService.DeleteFile
            (
                groupID,
                newFileId,
                version,
                newFileName,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Couldn't copy file", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestUpdateFileInfo()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(_tc.bcWrapper));
            _tc.bcWrapper.GroupFileService.UpdateFileInfo
            (
                groupID,
                groupFileId,
                version,
                updatedName,
                acl,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Revert the update... This file is used in other tests so its better to keep it consistent.
            _tc.bcWrapper.GroupFileService.UpdateFileInfo
            (
                groupID,
                groupFileId,
                version,
                filename,
                acl,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("Unable to update file info", _tc.successCount == 2);
        }

        //File upload specific functions taken from TestFile.cs
        private IEnumerator WaitForReturn(string[] uploadIds, int cancelTime = -1)
        {
            int count = 0;
            bool sw = true;
            Debug.Log("Waiting for file to upload...");
            BrainCloudClient client = _tc.bcWrapper.Client;
            
            client.Update();
            while (_returnCount < uploadIds.Length)
            {
                for (int i = 0; i < uploadIds.Length; i++)
                {
                    double progress = client.FileService.GetUploadProgress(uploadIds[i]);

                    if (progress > -1 && sw)
                    {
                        string logStr = "File " + (i + 1) + " Progress: " +
                                        progress + " | " +
                                        client.FileService.GetUploadBytesTransferred(uploadIds[i]) + "/" +
                                        client.FileService.GetUploadTotalBytesToTransfer(uploadIds[i]);
                        Debug.Log(logStr);
                    }

                    if (cancelTime > 0 && progress > 0.05)
                    {
                        client.FileService.CancelUpload(uploadIds[i]);
                        Debug.LogWarning("Canceling Upload...");
                    }
                }
                client.Update();
                yield return new WaitForFixedUpdate();
                sw = !sw;
                count += 150;
            }

            _returnCount = 0;
        }
        
        private string GetUploadId(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            if (fileDetails == null) return "";
            
            return (string)(fileDetails["uploadId"]);
        }
        
        private void FileCallbackSuccess(string uploadId, string jsonData)
        {
            _returnCount++;
            _tc.successCount++;
            _tc.m_done = true;
        }

        private void FileCallbackFail(string uploadId, int statusCode, int reasonCode, string jsonData)
        {
            _returnCount++;
            _tc.m_done = true;
            _tc.failCount++;
            _tc.m_statusMessage = jsonData;
        }
        
        /// <summary>
        /// Creates a test file filled with garbage
        /// </summary>
        /// <param name="size">Size of the file in KB</param>
        /// <returns>Full path to the file</returns>
        private string CreateFile(int size)
        {
            string path = Path.Combine(Path.GetTempPath() + tempFilename);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(1024 * size);
            }

            return path;
        }
    }    
}

