using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using BrainCloud.Entity;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestCustomEntity : TestFixtureBase
    {
        [Test]
        public void TestStoreAsync()
        {
            TestResult tr = new TestResult(_bc);
            _bc.EntityFactory.RegisterEntityClass<Player>(Player.ENTITY_TYPE);
            Player playerEntity = _bc.EntityFactory.NewEntity<Player>(Player.ENTITY_TYPE);

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

        public Player(BrainCloudWrapper bc)
            : base(bc.EntityService)
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