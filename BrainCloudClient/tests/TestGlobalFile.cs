using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalFile : TestFixtureBase
    {   
        string testfileName = "testGlobalFile.png";
        string testFileId = "ed2d2924-4650-4a88-b095-94b75ce9aa18";
        string testFolderPath = "/fname/";

        [Test]
        public void TestGetFileInfo()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalFileService.GetFileInfo(
                testFileId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetFileInfoSimple()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalFileService.GetFileInfoSimple(
                testFolderPath,
                testfileName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalCDNUrl()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalFileService.GetGlobalCDNUrl(
                testFileId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalFileList()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalFileService.GetGlobalFileList(
                testFolderPath,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}