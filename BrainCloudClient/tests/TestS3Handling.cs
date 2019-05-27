using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestS3Handling : TestFixtureBase
    {
        private string _category = "test";

        [Test]
        public void TestGetUpdatedFiles()
        {
            TestResult tr = new TestResult(_bc);

            _bc.S3HandlingService.GetUpdatedFiles(
                _category,
                GetModifiedFileDetails(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetFileList()
        {
            TestResult tr = new TestResult(_bc);

            _bc.S3HandlingService.GetFileList(
                _category,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetCdnUrl()
        {
            TestResult tr = new TestResult(_bc);

            _bc.S3HandlingService.GetFileList(
                _category,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            object[] files = (object[])((Dictionary<string, object>)(tr.m_response["data"]))["fileDetails"];
            Dictionary<string, object> file = (Dictionary<string, object>)files[0];
            string fileId = file["fileId"] as string;

            _bc.S3HandlingService.GetCDNUrl(
                fileId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private string GetModifiedFileDetails()
        {
            TestResult tr = new TestResult(_bc);
            string fileDetails = "";

            _bc.S3HandlingService.GetFileList(
                _category,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                object[] files = (object[])((Dictionary<string, object>)(tr.m_response["data"]))["fileDetails"];

                if (files.Length <= 0) return "";

                Dictionary<string, object> file = (Dictionary<string, object>)files[0];
                file["md5Hash"] = "d41d8cd98f00b204e9800998ecf8427e";
                fileDetails = JsonWriter.Serialize(new object[] { file });
            }
            return fileDetails;
        }
    }
}