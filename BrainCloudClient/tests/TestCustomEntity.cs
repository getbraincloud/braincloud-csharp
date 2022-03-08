using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Entity;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

namespace BrainCloudTests
{
    
    [TestFixture]
    public class TestCustomEntity : TestFixtureBase
    {
        [Test]
        public void TestStoreAsync()
        {
            TestResult tr = new TestResult(_bc);
            Player playerEntity = Initialize();

            playerEntity.StoreAsync(tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Cleanup(playerEntity);
        }

        [Test]
        public void TestDeleteAsync()
        {
            TestResult tr = new TestResult(_bc);
            Player playerEntity = Initialize();
            playerEntity.DeleteAsync(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }

        [Test]
        public void TestStoreAsyncShared()
        {
            TestResult tr = new TestResult(_bc);
            _bc.EntityFactory.RegisterEntityClass<Player>(Player.ENTITY_TYPE);
            Player playerEntity = _bc.EntityFactory.NewEntity<Player>(Player.ENTITY_TYPE);

            playerEntity.StoreAsyncShared(GetUser(Users.UserA).ProfileId, tr.ApiSuccess, tr.ApiError);
            tr.Run();

            Cleanup(playerEntity);
        }

        [Test]
        public void TestNewUserEntitiesFromReadPlayerState()
        {
            Initialize();
            TestResult tr = new TestResult(_bc);
            Player playerEntity = null;

            _bc.PlayerStateService.ReadUserState(
                tr.ApiSuccess, tr.ApiError);

            if (tr.Run())
            {
                IList<BCUserEntity> entities = _bc.EntityFactory.NewUserEntitiesFromReadPlayerState(JsonWriter.Serialize(tr.m_response));
                foreach (BCUserEntity e in entities)
                {
                    if (e.EntityType == Player.ENTITY_TYPE)
                    {
                        playerEntity = (Player)e;
                    }
                }
            }

            Cleanup(playerEntity);
        }
        
        [Test]
        public void TestCreateEntityThenUpdateEntityShard()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> entityData = new Dictionary<string, object>
            {
                {
                    "GamesPlayed", 2
                },
                {
                    "Name", "Zoro"
                },
                {
                    "Goals", 7
                }
            };
            Dictionary<string, object> aclData = new Dictionary<string, object>
            {
                {
                    "other", 1
                }
            };
            string jsonEntityData = JsonWriter.Serialize(entityData);
            string jsonAclData = JsonWriter.Serialize(aclData);
            _bc.CustomEntityService.CreateEntity
                    (
                        "athletes",
                        jsonEntityData,
                        jsonAclData,
                        null,
                        true,
                        tr.ApiSuccess,
                        tr.ApiError
                    );
            tr.Run();
            var data = tr.m_response["data"] as Dictionary<string,object>;
            string entityId = data["entityId"] as string;
            string ownerId = data["ownerId"] as string;

            Dictionary<string, object> shardKey = new Dictionary<string, object>
            {
                {
                    "ownerId", ownerId
                }
            };
            Dictionary<string, object> fields = new Dictionary<string, object>
            {
                {
                    "GamesPlayedTotal", 2
                },
                {
                    "Goals", 10
                }
            };
            string jsonShardKey = JsonWriter.Serialize(shardKey);
            string jsonFields = JsonWriter.Serialize(fields);
            _bc.CustomEntityService.UpdateEntityFieldsSharded
                    (
                        "athletes",
                        entityId,
                        1,
                        jsonShardKey,
                        jsonFields,
                        tr.ApiSuccess,
                        tr.ApiError
                    );
            tr.Run();
        }

        private Player Initialize()
        {            
            TestResult tr = new TestResult(_bc);
            _bc.EntityFactory.RegisterEntityClass<Player>(Player.ENTITY_TYPE);
            Player playerEntity = _bc.EntityFactory.NewEntity<Player>(Player.ENTITY_TYPE);
            playerEntity.StoreAsync(tr.ApiSuccess, tr.ApiError);            
            tr.Run();
            return playerEntity;
        }

        private void Cleanup(Player playerEntity)
        {
            if (playerEntity == null) return;
            TestResult tr = new TestResult(_bc);            
            playerEntity.DeleteAsync(tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }

    public class Player : BCUserEntity
    {
        public static string ENTITY_TYPE = "player";
        BrainCloudEntity in_bcEntityService;

        //problem is the need of a parameter for a generic type T in NewEntity call. 

        public Player(BrainCloudEntity in_bcEntityService) : base(in_bcEntityService)
        { 
            // set up some defaults
            m_entityType = "player";
            Name = "";
            Age = 0;
            Hobbies = new List<Hobby>();
        }

        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public int Age
        {
            get { return (int)this["age"]; }
            set { this["age"] = value; }
        }

        public IList<Hobby> Hobbies
        {
            get { return this.Get<IList<Hobby>>("hobbies"); }
            set { this["hobbies"] = value; }
        }
    }

    public class Hobby
    {
        public string Name
        {
            get { return ""; }
        }
    }
}