using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
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
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalEntityService.CreateEntity(
                _defaultEntityType,
                0,
                null,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCreateEntityWithIndexedId()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
                "indexedIdTest",
                0,
                null,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDeleteEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.DeleteEntity(
                entityId,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetListByIndexedId()
        {
            TestResult tr = new TestResult();

            string indexedId = "testIndexedId";

            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);
            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);

            BrainCloudClient.Instance.GlobalEntityService.GetListByIndexedId(
                indexedId,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetList()
        {
            TestResult tr = new TestResult();

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.GetList(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                Helpers.CreateJsonPair("data.name", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetListCount()
        {
            TestResult tr = new TestResult();

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.GetListCount(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.ReadEntity(
                entityId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.UpdateEntity(
                entityId,
                1,
                Helpers.CreateJsonPair(_defaultEntityValueName, "Test Name 02 Changed"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityAcl()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.UpdateEntityAcl(
                entityId,
                1,
                new ACL { Other = ACL.Access.ReadWrite }.ToJsonString(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestUpdateEntityTimeToLive()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Instance.GlobalEntityService.UpdateEntityTimeToLive(
                entityId,
                1,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPage()
        {
            TestResult tr = new TestResult();

            GenerateDefaultEntitites(200);

            BrainCloudClient.Instance.GlobalEntityService.GetPage(
                CreateContext(125, 1, _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPageOffset()
        {
            TestResult tr = new TestResult();
            GenerateDefaultEntitites(200);

            BrainCloudClient.Instance.GlobalEntityService.GetPage(
                CreateContext(50, 1, _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            int page = 0;
            page = (int)((Dictionary<string, object>)(((Dictionary<string, object>)tr.m_response["data"])["results"]))["page"];

            string context = "";
            context = (string)((Dictionary<string, object>)tr.m_response["data"])["context"];

            BrainCloudClient.Instance.GlobalEntityService.GetPageOffset(
                context,
                page,
                tr.ApiSuccess, tr.ApiError);

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
        private string CreateDefaultGlobalEntity(ACL.Access accessLevel = ACL.Access.None, string indexedId = "")
        {
            TestResult tr = new TestResult();

            ACL access = new ACL() { Other = accessLevel };
            string entityId = "";

            //Create entity
            if (indexedId.Length <= 0)
            {
                BrainCloudClient.Instance.GlobalEntityService.CreateEntity(
                 _defaultEntityType,
                 0,
                 access.ToJsonString(),
                 Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                 tr.ApiSuccess,
                 tr.ApiError);
            }
            else
            {
                BrainCloudClient.Instance.GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
               indexedId,
                0,
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
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.GlobalEntityService.GetListCount(
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