using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestEntity : TestFixtureBase
    {
        private readonly string _defaultEntityType = "address";
        private readonly string _defaultEntityValueName = "street";
        private readonly string _defaultEntityValue = "1309 Carling";

        [Test]
        public void TestCreateEntity()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.EntityService.CreateEntity(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                new ACL(ACL.Access.None).ToJsonString(),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestDeleteEntity()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.None);

            //Delete entity
            BrainCloudClient.Instance.EntityService.DeleteEntity(entityId, 1, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateEntity()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.None);

            //Update entity
            string updatedAddress = "1609 Bank St";

            BrainCloudClient.Instance.EntityService.UpdateEntity(
                entityId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                null,
                1,
                true,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateEntitySummary()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.None);

            //Update entity
            string updatedAddress = "1609 Bank St";

            BrainCloudClient.Instance.EntityService.UpdateEntity(
                entityId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                null,
                1,
                true,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetEntity()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.None);

            //GetEntity
            BrainCloudClient.Instance.EntityService.GetEntity(entityId, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetSharedEntitiesForPlayerId()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            //GetEntity
            BrainCloudClient.Instance.EntityService.GetSharedEntitiesForPlayerId(GetUser(Users.UserA).ProfileId, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetSharedEntityForPlayerId()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            //GetEntity
            BrainCloudClient.Instance.EntityService.GetSharedEntityForPlayerId(
                GetUser(Users.UserA).ProfileId,
                entityId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetEntitiesByType()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.None);

            //GetEntity
            BrainCloudClient.Instance.EntityService.GetEntitiesByType(_defaultEntityType, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateSharedEntity()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            string updatedAddress = "1609 Bank St";

            //UpdateSharedEntity
            BrainCloudClient.Instance.EntityService.UpdateSharedEntity(
                entityId,
                BrainCloudClient.Instance.AuthenticationService.ProfileId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                1,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateSharedEntitySummary()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            string updatedAddress = "1609 Bank St";

            //UpdateSharedEntity
            BrainCloudClient.Instance.EntityService.UpdateSharedEntity(
                entityId,
                BrainCloudClient.Instance.AuthenticationService.ProfileId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                1,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateSingleton()
        {
            TestResult tr = new TestResult();
            //CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            string updatedAddress = "1609 Bank St";

            //UpdateSharedEntity          
            BrainCloudClient.Instance.EntityService.UpdateSingleton(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                new ACL() { Other = ACL.Access.ReadWrite }.ToJsonString(),
                1,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestUpdateSingletonSummary()
        {
            TestResult tr = new TestResult();
            //CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            string updatedAddress = "1609 Bank St";

            //UpdateSharedEntity          
            BrainCloudClient.Instance.EntityService.UpdateSingleton(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                new ACL() { Other = ACL.Access.ReadWrite }.ToJsonString(),
                1,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetSingleton()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.None);

            //UpdateSharedEntity          
            BrainCloudClient.Instance.EntityService.GetSingleton(
                _defaultEntityType,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestDeleteSingleton()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            //UpdateSharedEntity
            BrainCloudClient.Instance.EntityService.DeleteSingleton(
                _defaultEntityType,
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetList()
        {
            TestResult tr = new TestResult();

            CreateDefaultAddressEntity(ACL.Access.ReadWrite);
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            BrainCloudClient.Instance.EntityService.GetList(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                Helpers.CreateJsonPair("createdAt", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetListCount()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.EntityService.GetListCount(
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPage()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.EntityService.GetPage(
                CreateContext(125, 1, _defaultEntityType),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetPageOffset()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.EntityService.GetPage(
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

        [Test]
        public void TestIncrementSharedEntityData()
        {
            TestResult tr = new TestResult();

            string id = CreateEntity(_defaultEntityType, "test", 10);

            BrainCloudClient.Instance.EntityService.IncrementSharedUserEntityData(
                id,
                GetUser(Users.UserA).ProfileId,
                Helpers.CreateJsonPair("test", 1),
                true,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestIncrementEntityData()
        {
            TestResult tr = new TestResult();

            string id = CreateEntity(_defaultEntityType, "test", 10);

            BrainCloudClient.Instance.EntityService.IncrementUserEntityData(
                id,
                Helpers.CreateJsonPair("test", 1),
                true,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetSharedEntitiesListForPlayerId()
        {
            TestResult tr = new TestResult();

            CreateDefaultAddressEntity(ACL.Access.ReadWrite);
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            BrainCloudClient.Instance.EntityService.GetSharedEntitiesListForPlayerId(
                GetUser(Users.UserA).ProfileId,
                Helpers.CreateJsonPair("entityType", _defaultEntityType),
                Helpers.CreateJsonPair("createdAt", 1),
                1000,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        #region Helper Functions

        /// <summary>
        /// Returns the entityId from a raw json response
        /// </summary>
        /// <param name="json"> Json to parse for ID </param>
        /// <returns> entityId from data </returns>
        private string GetEntityId(Dictionary<string, object> json)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)(json["data"]);
            return (string)data["entityId"];
        }

        /// <summary>
        /// Creates a default entity on the server
        /// </summary>
        /// <param name="accessLevel"> accessLevel for entity </param>
        /// <returns> The ID of the entity </returns>
        private string CreateDefaultAddressEntity(ACL.Access accessLevel)
        {
            TestResult tr = new TestResult();

            ACL access = new ACL() { Other = accessLevel };
            string entityId = "";

            //Create entity
            BrainCloudClient.Instance.EntityService.CreateEntity(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                access.ToJsonString(),
                tr.ApiSuccess,
                tr.ApiError);

            if (tr.Run()) entityId = GetEntityId(tr.m_response);

            return entityId;
        }

        private string CreateEntity(string type, string key, int value)
        {
            TestResult tr = new TestResult();
            string entityId = "";
            //Create entity
            BrainCloudClient.Instance.EntityService.CreateEntity(
                _defaultEntityType,
                Helpers.CreateJsonPair(key, value),
                null,
                tr.ApiSuccess,
                tr.ApiError);

            if (tr.Run()) entityId = GetEntityId(tr.m_response);
            return entityId;
        }

        /// <summary>
        /// Deletes all defualt entities
        /// </summary>
        private void DeleteAllDefaultEntities()
        {
            TestResult tr = new TestResult();

            List<string> entityIds = new List<string>(0);

            //get all entities
            BrainCloudClient.Instance.EntityService.GetEntitiesByType(_defaultEntityType, tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                Dictionary<string, object> data = (Dictionary<string, object>)tr.m_response["data"];
                object[] temp = (object[])data["entities"];

                if (temp.Length <= 0) return;

                Dictionary<string, object>[] entities = (Dictionary<string, object>[])data["entities"];

                for (int i = 0; i < entities.Length; i++)
                {
                    entityIds.Add((string)entities[i]["entityId"]);
                }
            }

            for (int i = 0; i < entityIds.Count; i++)
            {
                tr.Reset();
                BrainCloudClient.Instance.EntityService.DeleteEntity(entityIds[i], -1, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberOfEntitiesPerPage"></param>
        /// <param name="startPage"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
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

        #endregion
    }
}