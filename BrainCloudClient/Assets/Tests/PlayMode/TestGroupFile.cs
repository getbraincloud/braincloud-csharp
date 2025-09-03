using System.Collections;
using System.Collections.Generic;
using System.IO;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestGroupFile : TestFixtureBase
    {
        //Information grabbed from internal servers -> Unit Test Master
        private string _folderPath = "";
        private string _groupFileId;
        private string _groupID = "a7ff751c-3251-407a-b2fd-2bd1e9bca64a";
        private bool _recurse = true;
        //Making version a negative value to tell the server to use the latest version
        private int _version = -1;
        private bool _shareable = true;
        private bool _replaceIfExists = true;
        private string _cloudPath = "";
        private int _returnCount;
        private string _filename = "testingGroupFile.dat";
        private string _tempFilename = "deleteThisFileAfter.dat";
        private string _updatedName = "UpdatedGroupFile.dat";
        private Dictionary<string, object> _acl = new Dictionary<string, object>() {{"other", 0}, {"member", 2}};

        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [OneTimeTearDown]
        public override void TearDown()
        {
            //Due to the nature of this test suite relying on the newly uploaded file,
            //I've taken out tear down from here so that I can manually tear down in TestZ()
            //This way the Test Container can be persisted in all GroupFile tests
        }

        //We want this to run first before all other tests
        [UnityTest]
        public IEnumerator TestA()
        {
            Debug.Log("Setting up GroupFile Tests....");
            //Create new User
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            //Add user to group
            _tc.bcWrapper.GroupService.JoinGroup(_groupID, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Upload new file
            _tc.bcWrapper.Client.RegisterFileUploadCallback(FileCallbackSuccess, FileCallbackFail);
            FileInfo info = new FileInfo(CreateFile(4024));
            _cloudPath = "TestFolder";
            _tc.bcWrapper.FileService.UploadFileFromMemory
            (
                _cloudPath,
                _filename,
                _shareable,
                _replaceIfExists,
                ConvertFileToBytes(info),
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            yield return null;
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) });
            
            //Moving new file to group file
            _tc.bcWrapper.GroupFileService.MoveUserToGroupFile
            (
                "TestFolder/",
                _filename,
                _groupID,
                "",
                _filename,
                _acl,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Now perform the micro tests
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)_tc.m_response["data"])["fileDetails"]);
            _groupFileId = (string) fileDetails["fileId"];
            LogResults("Couldn't upload file before test", _groupFileId.Length > 0);
        }

        [UnityTest]
        public IEnumerator TestZ()
        {
            Debug.Log("Group File Tearing down...");
            //Delete new file
            _tc.bcWrapper.GroupFileService.DeleteFile
            (
                _groupID,
                _groupFileId,
                _version,
                _filename,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            _tc.bcWrapper.GroupService.LeaveGroup(_groupID, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            base.TearDown();
        }

        [UnityTest]
        public IEnumerator TestCheckFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.GroupFileService.CheckFilenameExists(_groupID, _folderPath, _filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            bool doesFileExist = ((bool)((Dictionary<string, object>)_tc.m_response["data"])["exists"]);
            LogResults("File doesn't exist anymore..", doesFileExist);
        }

        [UnityTest]
        public IEnumerator TestCheckFullpathFilenameExists()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.GroupFileService.CheckFullpathFilenameExists(_groupID, _filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            bool doesFileExist = ((bool)((Dictionary<string, object>)_tc.m_response["data"])["exists"]);
            LogResults("File doesn't exist anymore..", doesFileExist);
        }

        [UnityTest]
        public IEnumerator TestGetFileInfo()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.GetFileInfo(_groupID, _groupFileId, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetFileInfoSimple()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.GetFileInfoSimple(_groupID, "", _filename, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }
        
        [UnityTest]
        public IEnumerator TestGetFileList()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.GetFileList(_groupID, "", _recurse, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());

            LogResults("Something went wrong..", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestGetCDNUrl()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.GetCDNUrl(_groupID, _groupFileId, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Something went wrong..", _tc.successCount == 1);
        }

        [UnityTest]
        public IEnumerator TestMoveFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.MoveFile
            (
                _groupID,
                _groupFileId,
                _version,
                "",
                -1,
                _updatedName,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Reverting file name for other tests to use
            _tc.bcWrapper.GroupFileService.MoveFile
            (
                _groupID,
                _groupFileId,
                _version,
                "",
                -1,
                _filename,
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("Couldn't move file..", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestCopyFile()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            string newFileName = "testCopiedFile.dat";
            _tc.bcWrapper.GroupFileService.CopyFile
            (
                _groupID,
                _groupFileId,
                _version,
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
                _groupID,
                newFileId,
                _version,
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
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            _tc.successCount = 0;
            _tc.bcWrapper.GroupFileService.UpdateFileInfo
            (
                _groupID,
                _groupFileId,
                _version,
                _updatedName,
                _acl,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Revert the update... This file is used in other tests so its better to keep it consistent.
            _tc.bcWrapper.GroupFileService.UpdateFileInfo
            (
                _groupID,
                _groupFileId,
                _version,
                _filename,
                _acl,
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
            string path = Path.Combine(Path.GetTempPath() + _filename);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(1024 * size);
            }

            return path;
        }
    }    
}

