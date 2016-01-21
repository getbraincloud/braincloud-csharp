using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System.IO;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFile : TestFixtureBase
    {
        /*[Test]
        public void TestUploadFile()
        {
            TestResult tr = new TestResult();

            FileInfo info = new FileInfo(@"C:\TestFile.filters");

            BrainCloudClient.Get().FileService.UploadFile(
                info.FullName,
                info.Name,
                @"\test",
                false,
                true,
                info.Length,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }*/
    }
}