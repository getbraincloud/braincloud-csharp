using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestS3Handling : TestFixtureBase
    {
        private string _category = "test";

        [Test]
        public void TestGetUpdatedFiles()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.S3HandlingService.GetUpdatedFiles(
                _category,
                GetModifiedFileDetails(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetFileList()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.S3HandlingService.GetFileList(
                _category,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        private string GetModifiedFileDetails()
        {
            TestResult tr = new TestResult();
            string fileDetails = "";

            BrainCloudClient.Instance.S3HandlingService.GetFileList(
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