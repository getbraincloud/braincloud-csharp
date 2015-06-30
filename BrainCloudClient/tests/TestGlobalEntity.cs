using NUnit;
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
        private readonly string _defaultEntityName = "globalTestName";
        private readonly string _defaultEntityValue = "Test Name 01";

        [Test]
        public void TestCreateEntity()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().GlobalEntityService.CreateEntity(
                _defaultEntityType,
                0,
                null,
                CreateJsonPair(_defaultEntityName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestCreateEntityWithIndexedId()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
                "indexIdTest",
                0,
                null,
                CreateJsonPair(_defaultEntityName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestDeleteEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.DeleteEntity(
                entityId,
                -1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestGetListByIndexedId()
        {
            TestResult tr = new TestResult();

            string indexedId = "testIndexedId";

            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);
            CreateDefaultGlobalEntity(ACL.Access.None, indexedId);

            BrainCloudClient.Get().GlobalEntityService.GetListByIndexedId(
                indexedId,
                100,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestGetList()
        {
            TestResult tr = new TestResult();

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.GetList(
                CreateJsonPair("entityType", "testGlobalEntity"),
                CreateJsonPair("data.name", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestGetListCount()
        {
            TestResult tr = new TestResult();

            CreateDefaultGlobalEntity();
            CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.GetListCount(
                CreateJsonPair("entityType", "testGlobalEntity"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestReadEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.ReadEntity(
                entityId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestUpdateEntity()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.UpdateEntity(
                entityId,
                -1,
                CreateJsonPair(_defaultEntityName, "Test Name 02 Changed"),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestUpdateEntityAcl()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.UpdateEntityAcl(
                entityId,
                -1,
                new ACL { Other = ACL.Access.ReadWrite }.ToJsonString(),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
        }

        [Test]
        public void TestUpdateEntityTimeToLive()
        {
            TestResult tr = new TestResult();

            string entityId = CreateDefaultGlobalEntity();

            BrainCloudClient.Get().GlobalEntityService.UpdateEntityTimeToLive(
                entityId,
                -1,
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteDefaultEntities();
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
                BrainCloudClient.Get().GlobalEntityService.CreateEntity(
                 _defaultEntityType,
                 0,
                 null,
                 CreateJsonPair(_defaultEntityName, _defaultEntityValue),
                 tr.ApiSuccess,
                 tr.ApiError);
            }
            else
            {
                BrainCloudClient.Get().GlobalEntityService.CreateEntityWithIndexedId(
                _defaultEntityType,
               indexedId,
                0,
                new ACL() { Other = ACL.Access.None }.ToJsonString(),
                CreateJsonPair(_defaultEntityName, _defaultEntityValue),
                tr.ApiSuccess,
                tr.ApiError);
            }

            if (tr.Run())
            {
                entityId = GetEntityId(tr.m_response);
            }

            return entityId;
        }

        /// <summary>
        /// Deletes all defualt entities
        /// </summary>
        private void DeleteDefaultEntities()
        {
            TestResult tr = new TestResult();

            List<string> entityIds = new List<string>(0);

            //get all entities
            BrainCloudClient.Get().GlobalEntityService.GetList(
                CreateJsonPair("entityType", "testGlobalEntity"),
                CreateJsonPair("data.name", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                Dictionary<string, object> data = (Dictionary<string, object>)tr.m_response["data"];
                object[] temp = (object[])data["entityList"];

                if (temp.Length <= 0) return;

                Dictionary<string, object>[] entities = (Dictionary<string, object>[])data["entityList"];

                for (int i = 0; i < entities.Length; i++)
                {
                    entityIds.Add((string)entities[i]["entityId"]);
                }
            }

            for (int i = 0; i < entityIds.Count; i++)
            {
                tr.Reset();
                BrainCloudClient.Get().GlobalEntityService.DeleteEntity(entityIds[i], -1, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
        }

        /// <summary>
        /// Creates a properly formatted key/value json pair
        /// </summary>
        /// <param name="key"> Key </param>
        /// <param name="value"> Value </param>
        /// <returns> Formatted Json pair </returns>
        public string CreateJsonPair(string key, string value)
        {
            return "{ \"" + key + "\" : \"" + value + "\"}";
        }

        public string CreateJsonPair(string key, int value)
        {
            return "{ \"" + key + "\" : " + value + "}";
        }

        #endregion
    }
}