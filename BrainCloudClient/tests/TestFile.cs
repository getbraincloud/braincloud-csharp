using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;
using System.Threading;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFile : TestFixtureBase
    {
        private const string _cloudPath = "";
        private bool _isDone = false;
        private bool _isError = true;


        [Test]
        public void TestListUserFiles()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().FileService.ListUserFiles(tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSimpleUpload()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            FileInfo info = new FileInfo(CreateFile(4024));

            BrainCloudClient.Get().FileService.UploadFile(
                _cloudPath,
                info.Name,
                true,
                true,
                info.FullName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            WaitForReturn(GetUploadId(tr.m_response));

            Assert.IsFalse(_isError);

            CleanupUploadTest();
        }

        [Test]
        public void TestUploadSimpleFileAndCancel()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            FileInfo info = new FileInfo(CreateFile(4024));

            BrainCloudClient.Get().FileService.UploadFile(
                _cloudPath,
                info.Name,
                true,
                true,
                info.FullName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            WaitForReturn(GetUploadId(tr.m_response), 500);

            Assert.IsTrue(_isError);

            Thread.Sleep(2000);

            BrainCloudClient.Get().FileService.ListUserFiles(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var fileList = ((object[])((Dictionary<string, object>)tr.m_response["data"])["fileList"]);
            Assert.IsFalse(fileList.Length > 0);

            CleanupUploadTest();
        }

        [Test]
        public void TestDeleteUserFile()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            FileInfo info = new FileInfo(CreateFile(4024));

            BrainCloudClient.Get().FileService.UploadFile(
                _cloudPath,
                info.Name,
                true,
                true,
                info.FullName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            WaitForReturn(GetUploadId(tr.m_response));

            Assert.IsFalse(_isError);

            BrainCloudClient.Get().FileService.DeleteUserFile(
                GetCloudPath(tr.m_response),
                info.Name,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupUploadTest();
        }

        [Test]
        public void TestDeleteUserFiles()
        {
            TestResult tr = new TestResult();
            BrainCloudClient.Get().RegisterFileUploadCallbacks(FileCallbackSuccess, FileCallbackFail);

            FileInfo info = new FileInfo(CreateFile(4024));

            BrainCloudClient.Get().FileService.UploadFile(
                _cloudPath,
                info.Name,
                true,
                true,
                info.FullName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();            

            WaitForReturn(GetUploadId(tr.m_response));

            Assert.IsFalse(_isError);

            BrainCloudClient.Get().FileService.DeleteUserFiles(
                GetCloudPath(tr.m_response),
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            CleanupUploadTest();
        }

        private void FileCallbackSuccess(string uploadId, string jsonData)
        {
            _isDone = true;
            _isError = false;
        }

        private void FileCallbackFail(string uploadId, int statusCode, int reasonCode, string jsonData)
        {
            _isDone = true;
            _isError = true;
        }


        private void WaitForReturn(string uploadId, int cancelTime = -1)
        {
            int count = 0;
            bool sw = true;

            BrainCloudClient _client = BrainCloudClient.Get();

            _client.Update();
            while (!_isDone && count < 1000 * 15)
            {
                if (sw)
                {
                    string logStr = "Progress: " +
                        _client.FileService.GetUploadProgress(uploadId) + " | " +
                        _client.FileService.GetUploadBytesTransferred(uploadId) + "/" +
                        _client.FileService.GetUploadTotalBytesToTransfer(uploadId);

                    Console.WriteLine(logStr);
                }

                _client.Update();
                sw = !sw;

                if (cancelTime > 0 && count > cancelTime)
                {
                    _client.FileService.CancelUpload(uploadId);
                }

                Thread.Sleep(150);
                count += 150;
            }
        }

        private string GetUploadId(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            return (string)(fileDetails["uploadId"]);
        }

        private string GetCloudPath(Dictionary<string, object> response)
        {
            var fileDetails = ((Dictionary<string, object>)((Dictionary<string, object>)response["data"])["fileDetails"]);
            return (string)(fileDetails["cloudPath"]);
        }

        private void CleanupUploadTest()
        {
            BrainCloudClient.Get().DeregisterFileUploadCallbacks();
            TestResult tr = new TestResult();
            _isDone = false;
            BrainCloudClient.Get().FileService.DeleteUserFiles("", true, tr.ApiSuccess, tr.ApiError);
        }

        /// <summary>
        /// Creates a test file filled with garbage
        /// </summary>
        /// <param name="size">Size of the file in KB</param>
        /// <returns>Full path to the file</returns>
        private string CreateFile(int size)
        {
            string path = Path.Combine(Path.GetTempPath() + "testFile.dat");

            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                fileStream.SetLength(1024 * size);
            }

            return path;
        }
    }
}