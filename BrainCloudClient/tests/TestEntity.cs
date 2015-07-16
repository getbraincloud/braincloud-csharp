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

            BrainCloudClient.Get().EntityService.CreateEntity(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                null,
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
            BrainCloudClient.Get().EntityService.DeleteEntity(entityId, 1, tr.ApiSuccess, tr.ApiError);

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

            BrainCloudClient.Get().EntityService.UpdateEntity(
                entityId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                null,
                1,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities(2);
        }

        [Test]
        public void TestGetEntity()
        {
            TestResult tr = new TestResult();
            string entityId = CreateDefaultAddressEntity(ACL.Access.None);

            //GetEntity
            BrainCloudClient.Get().EntityService.GetEntity(entityId, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }

        [Test]
        public void TestGetSharedEntitiesForPlayerId()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.None);

            //GetEntity
            BrainCloudClient.Get().EntityService.GetSharedEntitiesForPlayerId(GetUser(Users.UserA).ProfileId, tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities();
        }
        [Test]
        public void TestGetEntitiesByType()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.None);

            //GetEntity
            BrainCloudClient.Get().EntityService.GetEntitiesByType(_defaultEntityType, tr.ApiSuccess, tr.ApiError);

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
            BrainCloudClient.Get().EntityService.UpdateSharedEntity(
                entityId,
                BrainCloudClient.Get().AuthenticationService.ProfileId,
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities(2);
        }

        [Test]
        public void TestUpdateSingleton()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            string updatedAddress = "1609 Bank St";

            //UpdateSharedEntity          
            BrainCloudClient.Get().EntityService.UpdateSingleton(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, updatedAddress),
                new ACL() { Other = ACL.Access.ReadWrite }.ToJsonString(),
                1,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
            DeleteAllDefaultEntities(2);
        }

        [Test]
        public void TestDeleteSingleton()
        {
            TestResult tr = new TestResult();
            CreateDefaultAddressEntity(ACL.Access.ReadWrite);

            //UpdateSharedEntity
            BrainCloudClient.Get().EntityService.DeleteSingleton(
                _defaultEntityType,
                1,
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
            BrainCloudClient.Get().EntityService.CreateEntity(
                _defaultEntityType,
                Helpers.CreateJsonPair(_defaultEntityValueName, _defaultEntityValue),
                access.ToJsonString(),
                tr.ApiSuccess,
                tr.ApiError);

            if (tr.Run()) entityId = GetEntityId(tr.m_response);

            return entityId;
        }

        /// <summary>
        /// Deletes all defualt entities
        /// </summary>
        private void DeleteAllDefaultEntities(int version = 1)
        {
            TestResult tr = new TestResult();

            List<string> entityIds = new List<string>(0);

            //get all entities
            BrainCloudClient.Get().EntityService.GetEntitiesByType(_defaultEntityType, tr.ApiSuccess, tr.ApiError);

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
                BrainCloudClient.Get().EntityService.DeleteEntity(entityIds[i], version, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
        }

        #endregion
    }
}