using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;
using System.Threading;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCustomEntityService : TestFixtureBase
    {
        [Test]
        public void TestCreateEntity()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetEntityPage()
        {
            TestResult tr = new TestResult(_bc);

            _bc.CustomEntityService.GetEntityPage(
                "athletes", CreateContext(125, 1, "athletes"),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetEntityPageByOffset()
        {
            string context = "eyJzZWFyY2hDcml0ZXJpYSI6eyJkYXRhLnBvc2l0aW9uIjoiZGVmZW5zZSIsIiRvciI6W3sib3duZXJJZCI6IjJhYmYwODNhLTc1Y2QtNGE4My05YTQyLWIzNTIwNzI5ZWY4YiJ9LHsiYWNsLm90aGVyIjp7IiRuZSI6MH19XX0sInNvcnRDcml0ZXJpYSI6eyJjcmVhdGVkQXQiOjF9LCJwYWdpbmF0aW9uIjp7InJvd3NQZXJQYWdlIjoyMCwicGFnZU51bWJlciI6MSwiZG9Db3VudCI6ZmFsc2V9LCJvcHRpb25zIjpudWxsfQ";
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.GetEntityPageOffset(
                "athletes",
                context,
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestReadEntity()
        {

            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            string entityId;
            entityId= (string)((Dictionary<string, object>)tr1.m_response["data"])["entityId"];

            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.ReadEntity(
                "athletes",
                entityId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateEntity()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            string entityId;

            entityId= (string)((Dictionary<string, object>)tr1.m_response["data"])["entityId"];

            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateEntity(
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
        public void TestDeleteEntity()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            string entityId;
            int version;

            entityId= (string)((Dictionary<string, object>)tr1.m_response["data"])["entityId"];
            version= (int)((Dictionary<string, object>)tr1.m_response["data"])["version"];

            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.DeleteEntity(
                "athletes",
                entityId,
                version,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            var datadict = tr.m_response["data"] as Dictionary<string, object>;
            Assert.True(datadict == null);
            Assert.True((int)tr.m_response["status"]==200);
        }

        [Test]
        public void TestUpdateEntityFields()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            string entityId;
            int version;

            entityId= (string)((Dictionary<string, object>)tr1.m_response["data"])["entityId"];
            version= (int)((Dictionary<string, object>)tr1.m_response["data"])["version"];

            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateEntityFields(
                "athletes",
                entityId,
                version,
                "{\"test\": \"Testing\"}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestIncrementData()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            tr1.Run();

            string entityId;
            int version;

            entityId= (string)((Dictionary<string, object>)tr1.m_response["data"])["entityId"];
            version= (int)((Dictionary<string, object>)tr1.m_response["data"])["version"];

            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.IncrementData(
                "athletes",
                entityId,
                //"{\"test\": \"8\"}",
                "{\"goals\": 3, \"assists\": 5}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
        
        [Test]
        public void TestIncrementSingletonData()
        {
            TestResult tr1 = new TestResult(_bc);
            _bc.CustomEntityService.CreateEntity(
                "athletes", "{\"test\": \"Testing\"}", "{\"test\": \"Testing\"}", null, true,
                tr1.ApiSuccess, tr1.ApiError);
            
            tr1.Run();
            
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.IncrementSingletonData(
                "athletes",
                "{\"goals\": 3, \"assists\": 5}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDeleteEntities()
        {
            TestResult tr = new TestResult(_bc);

            //string context = "";
            //context = (string)((Dictionary<string, object>)tr.m_response["data"])["context"];

            _bc.CustomEntityService.DeleteEntities(
                "athletes",
                "{\"entityId\": \"Testing\"}",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetCount()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.GetCount(
                "athletes",
                "{\"data.position\": \"defense\"}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestGetRandomEntitiesMatching()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.GetRandomEntitiesMatching(
                "athletes",
                "{\"data.position\": \"defense\"}",
                2,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestDeleteSingleton()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.DeleteSingleton(
                "athletes",
                1,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateSingleton()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateSingleton(
                "athletes",
                1,
                "{\"data.position\": \"defense\"}",
                "{\"test\": \"Testing\"}", 
                null,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestUpdateSingletonFields()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.UpdateSingletonFields(
                "athletes",
                -1,
                "{\"data.position\": \"defense\"}",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

                [Test]
        public void TestReadSingleton()
        {
            TestResult tr = new TestResult(_bc);
            _bc.CustomEntityService.ReadSingleton(
                "athletes",
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        private string CreateContext(int numberOfEntitiesPerPage, int startPage, string entityType)
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            Dictionary<string, object> pagination = new Dictionary<string, object>();
            pagination.Add("rowsPerPage", numberOfEntitiesPerPage);
            pagination.Add("pageNumber", startPage);
            context.Add("pagination", pagination);

            // Dictionary<string, object> searchCriteria = new Dictionary<string, object>();
            // searchCriteria.Add("entityType", entityType);
            // context.Add("searchCriteria", searchCriteria);

            return JsonWriter.Serialize(context);
        }
    }
}
