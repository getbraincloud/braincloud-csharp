using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCustomEntityService : TestFixtureBase
    {
        string entityId;
        int version;

        [Test]
        public void TestCreateCustomEntity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.CreateCustomEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            entityId= (string)((Dictionary<string, object>)tr.m_response["data"])["entityId"];
            version= (int)((Dictionary<string, object>)tr.m_response["data"])["version"];
        }

        [Test]
        public void TestGetCustomEntityPage()
        {
            //string context = "{\"test\": \"Testing\"}";
            //pass in context
            TestResult tr = new TestResult(_bc);

            _bc.CustomEntityService.GetCustomEntityPage(
                "athletes", 20, "{\"data.position\": \"defense\"}","{\"createdAt\": 1 }", false,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetCustomEntityPageOffset()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJkYXRhLnBvc2l0aW9uIjoiZGVmZW5zZSIsIiRvciI6W3sib3duZXJJZCI6IjJhYmYwODNhLTc1Y2QtNGE4My05YTQyLWIzNTIwNzI5ZWY4YiJ9LHsiYWNsLm90aGVyIjp7IiRuZSI6MH19XX0sInNvcnRDcml0ZXJpYSI6eyJjcmVhdGVkQXQiOjF9LCJwYWdpbmF0aW9uIjp7InJvd3NQZXJQYWdlIjoyMCwicGFnZU51bWJlciI6MSwiZG9Db3VudCI6ZmFsc2V9LCJvcHRpb25zIjpudWxsfQ";
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.GetCustomEntityPageOffset(
                "athletes",
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestReadCustomEntity()
        {
            TestCreateCustomEntity();
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.ReadCustomEntity(
                "athletes",
                entityId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateCustomEntity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateCustomEntity(
                "athletes",
                entityId,
                1,
                "{\"test\": \"Testing\"}",
                "{\"test\": \"Testing\"}",
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateCustomEntityFields()
        {
            TestCreateCustomEntity();
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateCustomEntityFields(
                "athletes",
                entityId,
                version,
                "{\"test\": \"Testing\"}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
