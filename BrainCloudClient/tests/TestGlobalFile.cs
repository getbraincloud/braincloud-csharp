using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalFile : TestFixtureBase
    {   
        string testfileName = "png1.png";
        string testFileId = "34802251-0da0-419e-91b5-59d91790af15";
        string testFolderPath = "/existingfolder/";

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