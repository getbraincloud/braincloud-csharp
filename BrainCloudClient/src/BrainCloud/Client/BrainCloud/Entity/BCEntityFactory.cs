//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Reflection;

namespace BrainCloud.Entity
{
    public class BCEntityFactory
    {
        private BrainCloudEntity m_braincloud;
        private IDictionary<string, ConstructorInfo> m_registeredClasses;

        public delegate BCUserEntity CreateUserEntityFromType(string type);


        public BCEntityFactory(BrainCloudEntity braincloud)
        {
            m_braincloud = braincloud;
            m_registeredClasses  = new Dictionary<string, ConstructorInfo>();
        }

        public T NewEntity<T> (string entityType) where T : BCEntity
        {
            T e = (T)CreateRegisteredEntityClass(entityType);
            e.BrainCloud = m_braincloud;
            e.EntityType = entityType;
            return e;
        }

        public BCUserEntity NewUserEntity(string entityType)
        {
            BCUserEntity e = (BCUserEntity) CreateRegisteredEntityClass(entityType);
            if (e == null)
            {
                e = new BCUserEntity();
            }
            e.BrainCloud = m_braincloud;
            e.EntityType = entityType;
            return e;
        }

        public IList<BCUserEntity> NewUserEntitiesFromGetList(string json)
        {
            JsonData jsonObj = JsonMapper.ToObject(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, jsonObj["data"]["entityList"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromReadPlayerState(string json)
        {
            JsonData jsonObj = JsonMapper.ToObject(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, jsonObj["data"]["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromStartMatch(string json)
        {
            JsonData jsonObj = JsonMapper.ToObject(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, jsonObj["data"]["initialSharedData"]["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public void RegisterEntityClass<T>(string entityType) where T : BCEntity
        {
            Type type = typeof(T);
            Type[] constructorParams = new Type[] {};

            ConstructorInfo ci = type.GetConstructor(constructorParams);
            if (ci != null)
            {
                m_registeredClasses[entityType] = ci;
            }
        }

        private BCEntity CreateRegisteredEntityClass(string entityType)
        {
            ConstructorInfo ci = null;
            if (m_registeredClasses.TryGetValue(entityType, out ci))
            {
                return (BCEntity)ci.Invoke(null);
            }
            return null;
        }

        // the list of entitiies
        private IList<BCUserEntity> NewUserEntitiesFromJsonString(string json, JsonData entitiesJson)
        {
            List<BCUserEntity> entities = new List<BCUserEntity>();
            JsonData child = null;
            for (int i = 0; i < entities.Count; ++i)
            {
                try
                {
                    child = entitiesJson[i];
                    BCUserEntity entity = null;
                    entity = NewUserEntity((string)child["entityType"]);
                    entity.ReadFromJson(child);
                    entities.Add(entity);
                }
                catch (System.Exception)
                {
                    /* do nadda */
                }
            }
            return entities;
        }
    }
}

#endif
