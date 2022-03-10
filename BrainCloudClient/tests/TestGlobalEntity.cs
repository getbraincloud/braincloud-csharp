using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGlobalEntity : TestFixtureBase
    {
        private readonly string _defaultEntityType = "testGlobalEntity";
        private readonly string _defaultEntityValueName = "globalTestName";
        private readonly string _defaultEntityValue = "Test Name 01";

        [Test]
        public void TestCreateEntity()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.CreateEntity(
                _defaultEntityType,
                3434343,
                null,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCreateEntityWithIndexedId()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
                "indexedIdTest",
                3434343,
                null,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDeleteEntity()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.DeleteEntity(
                entityId,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetListByIndexedId()
        {
            TestResult tr = new TestResult(_bc);

            string indexedId = "testIndexedId";

            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);
            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);

            _bc.GlobalEntityService.GetListByIndexedId(
                indexedId,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetList()
        {
            TestResult tr = new TestResult(_bc);

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.GetList(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                Helpers.CreateJsonPair("data.name", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetListCount()
        {
            TestResult tr = new TestResult(_bc);

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.GetListCount(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadEntity()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.ReadEntity(
                entityId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntity()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntity(
                entityId,
                1,
                Helpers.CreateJsonPair(_defaultEntityValueName, "Test Name 02 Changed"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntitySummary()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntity(
                entityId,
                1,
                Helpers.CreateJsonPair(_defaultEntityValueName, "Test Name 02 Changed"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityAcl()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntityAcl(
                entityId,
                1,
                new ACL { Other = ACL.Access.ReadWrite }.ToJsonString(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityAclSummary()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntityAcl(
                entityId,
                1,
                new ACL { Other = ACL.Access.ReadWrite }.ToJsonString(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityTimeToLive()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntityTimeToLive(
                entityId,
                1,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityTimeToLiveSummary()
        {
            TestResult tr = new TestResult(_bc);

            string entityId = CreateDefaultGlobalEntity();

            _bc.GlobalEntityService.UpdateEntityTimeToLive(
                entityId,
                1,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPage()
        {
            TestResult tr = new TestResult(_bc);

            GenerateDefaultEntitites(200);

            _bc.GlobalEntityService.GetPage(
                CreateContext(125, 1, _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPageOffset()
        {
            TestResult tr = new TestResult(_bc);
            GenerateDefaultEntitites(200);

            _bc.GlobalEntityService.GetPage(
                CreateContext(50, 1, _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            int page = 0;
            page = (int)((Dictionary<string, object>)(((Dictionary<string, object>)tr.m_response["data"])["results"]))["page"];

            string context = "";
            context = (string)((Dictionary<string, object>)tr.m_response["data"])["context"];

            _bc.GlobalEntityService.GetPageOffset(
                context,
                page,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestIncrementGlobalEntityData()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.CreateEntity(
                _defaultEntityType,
                3434343,
                null,
                Helpers.CreateJsonPair("test", 1234),
                tr.ApiSuccess,
                tr.ApiError);
            tr.Run();

            string id = GetEntityId(tr.m_response);

            _bc.GlobalEntityService.IncrementGlobalEntityData(
                id,
                Helpers.CreateJsonPair("test", 1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestIncrementGlobalEntityDataSummary()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.CreateEntity(
                _defaultEntityType,
                3434343,
                null,
                Helpers.CreateJsonPair("test", 1234),
                tr.ApiSuccess,
                tr.ApiError);
            tr.Run();

            string id = GetEntityId(tr.m_response);

            _bc.GlobalEntityService.IncrementGlobalEntityData(
                id,
                Helpers.CreateJsonPair("test", 1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityIndexedId()
        {
            string entityId = CreateDefaultGlobalEntity(ACL.Access.None, "indexedId");

            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.UpdateEntityIndexedId(
                entityId,
                -1,
                "indexedId",
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityOwnerAndAcl()
        {
            string entityId = CreateDefaultGlobalEntity();

            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.UpdateEntityOwnerAndAcl(
                entityId,
                -1,
                GetUser(Users.UserB).ProfileId,
                new ACL(ACL.Access.ReadWrite),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestMakeSystemEntity()
        {
            string entityId = CreateDefaultGlobalEntity();

            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.MakeSystemEntity(
                entityId,
                -1,
                new ACL(ACL.Access.ReadWrite),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        #region Helper Functions

        /// <summary>
        /// Returns the entityId from a raw json response
        /// </summary>
        /// <param name="json"> Json to parse for ID </param>
        /// <returns> entityId from data </returns>
        private static string GetEntityId(Dictionary<string, object> json)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)(json["data"]);
            return (string)data["entityId"];
        }

        /// <summary>
        /// Creates a default entity on the server
        /// </summary>
        /// <param name="accessLevel"> accessLevel for entity </param>
        /// <returns> The ID of the entity </returns>
        private string CreateDefaultGlobalEntity(ACL.Access accessLevel = ACL.Access.None, string indexedId = null)
        {
            TestResult tr = new TestResult(_bc);

            ACL access = new ACL() { Other = accessLevel };
            string entityId = "";

            //Create entity
            if (string.IsNullOrEmpty(indexedId))
            {
                _bc.GlobalEntityService.CreateEntity(
                 _defaultEntityType,
                 3434343,
                 access.ToJsonString(),
                 Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                 tr.ApiSuccess,
                 tr.ApiError);
            }
            else
            {
                _bc.GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
               indexedId,
                3434343,
                access.ToJsonString(),
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);
            }

            if (tr.Run())
            {
                entityId = GetEntityId(tr.m_response);
            }

            return entityId;
        }

        private string CreateContext(int numberOfEntitiesPerPage, int startPage, string entityType)
        {
            Dictionary<string, object> context = new Dictionary<string, object>();

            Dictionary<string, object> pagination = new Dictionary<string, object>();
            pagination.Add("rowsPerPage", numberOfEntitiesPerPage);
            pagination.Add("pageNumber", startPage);
            context.Add("pagination", pagination);

            Dictionary<string, object> searchCriteria = new Dictionary<string, object>();
            searchCriteria.Add("entityType", entityType);
            context.Add("searchCriteria", searchCriteria);

            return JsonWriter.Serialize(context);
        }

        private void GenerateDefaultEntitites(int numberOfEntites)
        {
            TestResult tr = new TestResult(_bc);

            _bc.GlobalEntityService.GetListCount(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            int existing = (int)(((Dictionary<string, object>)tr.m_response["data"])["entityListCount"]);

            numberOfEntites -= existing;
            if (numberOfEntites <= 0) return;

            for (int i = 0; i < numberOfEntites; i++)
            {
                CreateDefaultGlobalEntity(ACL.Access.ReadWrite);
            }
        }

        #endregion
    }
}