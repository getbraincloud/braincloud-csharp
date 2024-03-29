using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using BrainCloud;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class TestFile : TestFixtureBase
    {        
        private string _cloudPath = "";
        private bool _shouldDeleteFiles = true;

        private int _returnCount = 0;
        private bool _shareable = true;
        private bool _replaceIfExists = true;
        
        [TearDown]
        public override void TearDown()
        {
            _tc.bcWrapper.Client.DeregisterFileUploadCallbacks();
            if (_shouldDeleteFiles)
            {
                _tc.StartCoroutine(DeleteAllFiles());
                while (!_tc.m_done)
                {
                    _tc.bcWrapper.RunCallbacks();
                }
            }
            base.TearDown();
            _returnCount = 0;
            _shouldDeleteFiles = true;
        }
        
        [UnityTest]
        public IEnumerator TestListUserFiles()
        {
            _tc.bcWrapper.AuthenticateUniversal(username, password, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            _tc.bcWrapper.FileService.ListUserFiles(_tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
            
            LogResults("failed to list user files",_tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestSimpleUpload()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
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
            yield return new WaitForSeconds(5);
            //Checking if cloud path is correct according to response
            var data = _tc.m_response["data"] as Dictionary<string, object>;
            var fileDetails = data["fileDetails"] as Dictionary<string, object>;
            string responseCloudPath = (string)fileDetails["cloudPath"];
            Assert.True(_cloudPath.Equals(responseCloudPath));
            
            LogResults("failed to upload file", _tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestUploadFromMemory()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            string fileName = "testFile.txt";
            string fileContent = "Hello, I'm a file";
            byte[] fileData = ConvertByteFromFile(fileContent);
            
            _tc.bcWrapper.FileService.UploadFileFromMemory
                (
                    _cloudPath,
                    fileName,
                    _shareable,
                    _replaceIfExists,
                    fileData,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) });
            yield return new WaitForSeconds(2);
            LogResults("failed to upload file",_tc.successCount == 2);
        }

        [UnityTest]
        public IEnumerator TestUploadFromMemoryMultiFiles()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            string fileName1 = "testFile1.txt";
            string fileContent1 = "Hello, I'm a file !";
            byte[] fileData1 = ConvertByteFromFile(fileContent1);

            _tc.bcWrapper.FileService.UploadFileFromMemory
                (
                    _cloudPath,
                    fileName1,
                    _shareable,
                    _replaceIfExists,
                    fileData1,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            string fileName2 = "testFile2.txt";
            string fileContent2 = "Hey look ! Another file !";
            byte[] fileData2 = ConvertByteFromFile(fileContent2);

            _tc.bcWrapper.FileService.UploadFileFromMemory
                (
                    _cloudPath,
                    fileName2,
                    _shareable,
                    _replaceIfExists,
                    fileData2,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) });
            yield return new WaitForSeconds(2);
            LogResults("Couldn't upload all files",_tc.successCount == 4);
        }

        [UnityTest]
        public IEnumerator TestSimpleUploadThenDownloadFromPrivacySettings()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info = new FileInfo(CreateFile(4024));
            _shareable = false;
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
            yield return new WaitForSeconds(2);

            UnityWebRequest request = new UnityWebRequest("https://api.internal.braincloudservers.com/downloader/bc/g/20001/u/fabfacf0-3dc6-4cdd-8a48-6f7021407169/f/testFile1.txt");
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                _tc.successCount++;
            }
            
            LogResults("failed, it could be upload or download that failed.",_tc.successCount >= 2);
        }

        [UnityTest]
        public IEnumerator TestUploadSimpleFileAndCancel()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info = new FileInfo(CreateFile(4024));

            _tc.bcWrapper.FileService.UploadFile
                (
                    _cloudPath,
                    "testFileCANCEL.dat",
                    _shareable,
                    _replaceIfExists,
                    info.FullName,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            
            yield return WaitForReturn(new[] { GetUploadId(_tc.m_response) }, 500);

            yield return new WaitForSeconds(5);
            
            _tc.bcWrapper.FileService.ListUserFiles(_tc.ApiSuccess, _tc.ApiError);
            
            yield return _tc.StartCoroutine(_tc.Run());

            var fileList = ((object[])((Dictionary<string, object>)_tc.m_response["data"])["fileList"]);

            if (_tc.failCount > 0)
            {
                Debug.Log("Number of failed callbacks: " + _tc.failCount);
                LogResults("",true);
            }
            else if (fileList.Length > 0)
            {
                LogResults("Response indicates the file did not get cancelled and successfully uploaded.",false);
            }
        }

        [UnityTest]
        public IEnumerator TestDeleteUserFile()
        {
            _shouldDeleteFiles = false;
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info = new FileInfo(CreateFile(1024));

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

            yield return new WaitForSeconds(5);
            
            Assert.IsFalse(_tc.failCount > 0);
            
            _tc.bcWrapper.FileService.DeleteUserFile
                (
                    GetCloudPath(_tc.m_response),
                    info.Name,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());
            LogResults("failed to delete user files, check json response",_tc.successCount == 3);
        }

        [UnityTest]
        public IEnumerator TestDeleteUserFiles()
        {
            _shouldDeleteFiles = false;
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info = new FileInfo(CreateFile(4024));

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
            
            yield return _tc.StartCoroutine(WaitForReturn(new[] { GetUploadId(_tc.m_response) }));
            
            yield return new WaitForSeconds(5);
            
            FileInfo info2 = new FileInfo(CreateFile(4024));

            _tc.bcWrapper.FileService.UploadFile
            (
                _cloudPath,
                info2.Name,
                _shareable,
                _replaceIfExists,
                info2.FullName,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            yield return _tc.StartCoroutine(_tc.Run());
            
            yield return _tc.StartCoroutine(WaitForReturn(new[] { GetUploadId(_tc.m_response) }));
            
            yield return new WaitForSeconds(5);
            
            Assert.IsFalse(_tc.failCount > 0);

            _tc.bcWrapper.FileService.DeleteUserFiles
            (
                GetCloudPath(_tc.m_response),
                true,
                _tc.ApiSuccess,
                _tc.ApiError
            );
            
            yield return _tc.StartCoroutine(_tc.Run());

            LogResults("failed to delete all files OR uploading files failed.", _tc.successCount == 5);
        }

        [UnityTest]
        public IEnumerator TestGetCdnUrl()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info = new FileInfo(CreateFile(1024));

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
            
            yield return _tc.StartCoroutine(WaitForReturn(new[] { GetUploadId(_tc.m_response) }));
            
            Assert.IsFalse(_tc.failCount > 0);

            yield return new WaitForSeconds(5);
            
            _tc.bcWrapper.Client.FileService.GetCDNUrl
                (
                    GetCloudPath(_tc.m_response),
                    info.Name,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            
            yield return _tc.StartCoroutine(_tc.Run());

            LogResults("Files failed to upload, check logs for response", _tc.successCount == 3);
        }

        [UnityTest]
        public IEnumerator TestUploadMultiple()
        {
            yield return _tc.StartCoroutine(_tc.SetUpNewUser(Users.UserA));
            
            _tc.bcWrapper.Client.RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);
            
            FileInfo info1 = new FileInfo(CreateFile(4096, "multiUpload1.dat"));
            FileInfo info2 = new FileInfo(CreateFile(4096, "multiUpload2.dat"));

            _tc.bcWrapper.FileService.UploadFile
                (
                    _cloudPath,
                    info1.Name,
                    _shareable,
                    _replaceIfExists,
                    info1.FullName,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            
            //Getting Id from first upload
            string id1 = GetUploadId(_tc.m_response);

            _tc.bcWrapper.FileService.UploadFile
                (
                    _cloudPath,
                    info2.Name,
                    _shareable,
                    _replaceIfExists,
                    info2.FullName,
                    _tc.ApiSuccess,
                    _tc.ApiError
                );
            yield return _tc.StartCoroutine(_tc.Run());
            var ids = new[] {id1, GetUploadId(_tc.m_response)};
            yield return WaitForReturn(ids);

            LogResults("Files failed to upload, check logs for response", _tc.successCount >= 2);
        }

        private IEnumerator DeleteAllFiles()
        {
            _tc.bcWrapper.FileService.DeleteUserFiles(_cloudPath, true, _tc.ApiSuccess, _tc.ApiError);
            yield return _tc.StartCoroutine(_tc.Run());
        }

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
        
        private void FileCallbackSuccess(string uploadId, string jsonData)
        {
            _returnCount++;
            _tc.successCount++;
        }

        private void FileCallbackFail(string uploadId, int statusCode, int reasonCode, string jsonData)
        {
            _returnCount++;
            _tc.failCount++;
            _tc.m_statusMessage = jsonData;
        }
        
        private string GetCloudPath(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            return (string)(fileDetails["cloudPath"]);
        }
        
        /// <summary>
        /// Creates a test file filled with garbage
        /// </summary>
        /// <param name="size">Size of the file in KB</param>
        /// <returns>Full path to the file</returns>
        private string CreateFile(int size, string name = "testFile.dat")
        {
            string path = Path.Combine(Path.GetTempPath() + name);

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(1024 * size);
            }

            return path;
        }
        
        private string GetUploadId(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            if (fileDetails == null) return "";
            
            return (string)(fileDetails["uploadId"]);
        }
        
        private byte[] ConvertByteFromFile(string fileContent)
        {
            if (Uri.IsWellFormedUriString(fileContent, UriKind.Absolute))
            {
                Stream info = new FileStream(fileContent, FileMode.Open);
                byte[] fileData = new Byte[(int) info.Length];
                info.Seek(0, SeekOrigin.Begin);
                info.Read(fileData, 0, (int) info.Length);
                info.Close();
                return fileData;
            }

            return Encoding.ASCII.GetBytes(fileContent);
        }
        
        private string GetFullPath(string cloudPath, string cloudFileName)
        {
            string serverUrl = ServerUrl.Replace("/dispatcherv2", "");
            return serverUrl +
                   "/downloader/bc/g/" +
                   _tc.bcWrapper.Client.AppId +
                   "/u/" +
                   _tc.bcWrapper.Client.AuthenticationService.ProfileId +
                   "/f/" +
                   cloudPath +
                   "/" +
                   cloudFileName;
        }
    }
}