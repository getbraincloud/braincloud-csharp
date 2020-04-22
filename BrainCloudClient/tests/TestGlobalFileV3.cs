using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    string testfileName = "testGlobalFile.png"
    string testFileId = "ed2d2924-4650-4a88-b095-94b75ce9aa18";
    string testFolderPath = "/fname/";

    [TestFixture]
    public class TestGlobalFileV3 : TestFixtureBase
    {
        [Test]
        public void TestGetFileInfo()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TestGlobalFileV3.GetFileInfo(
                testFileId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetFileInfoSimple()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TestGlobalFileV3.GetFileInfo(
                testFolderPath,
                testfileName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalCDNUrl()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TestGlobalFileV3.GetGlobalCDNUrl(
                testFileId,
                testfileName,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetGlobalFileList()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TestGlobalFileV3.GetGlobalFileList(
                testFolderPath,
                true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}