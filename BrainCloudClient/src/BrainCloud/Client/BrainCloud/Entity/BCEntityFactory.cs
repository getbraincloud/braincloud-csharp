//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
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

        public delegate BCUserEntity CreateUserEntityFromType(string in_type);


        public BCEntityFactory(BrainCloudEntity in_braincloud)
        {
            m_braincloud = in_braincloud;
            m_registeredClasses  = new Dictionary<string, ConstructorInfo>();
        }

        public T NewEntity<T> (string in_entityType) where T : BCEntity
        {
            T e = (T)CreateRegisteredEntityClass(in_entityType);
            e.BrainCloud = m_braincloud;
            e.EntityType = in_entityType;
            return e;
        }

        public BCUserEntity NewUserEntity(string in_entityType)
        {
            BCUserEntity e = (BCUserEntity) CreateRegisteredEntityClass(in_entityType);
            if (e == null)
            {
                e = new BCUserEntity();
            }
            e.BrainCloud = m_braincloud;
            e.EntityType = in_entityType;
            return e;
        }

        public IList<BCUserEntity> NewUserEntitiesFromReadPlayerState(string in_json)
        {
            JsonData jsonObj = JsonMapper.ToObject(in_json);
            try
            {
                return NewUserEntitiesFromJsonString(in_json, jsonObj["data"]["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromStartMatch(string in_json)
        {
            JsonData jsonObj = JsonMapper.ToObject(in_json);
            try
            {
                return NewUserEntitiesFromJsonString(in_json, jsonObj["data"]["initialSharedData"]["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public void RegisterEntityClass<T>(string in_entityType) where T : BCEntity
        {
            Type type = typeof(T);
            Type[] constructorParams = new Type[] {};

            ConstructorInfo ci = type.GetConstructor(constructorParams);
            if (ci != null)
            {
                m_registeredClasses[in_entityType] = ci;
            }
        }

        private BCEntity CreateRegisteredEntityClass(string in_entityType)
        {
            ConstructorInfo ci = null;
            if (m_registeredClasses.TryGetValue(in_entityType, out ci))
            {
                return (BCEntity)ci.Invoke(null);
            }
            return null;
        }

        // the list of entitiies
        private IList<BCUserEntity> NewUserEntitiesFromJsonString(string in_json, JsonData in_entities)
        {
            List<BCUserEntity> entities = new List<BCUserEntity>();
            /*
            IDictionary d = jsonObj["data"] as IDictionary;
            if (d == null || !d.Contains("entities"))
            {
                return entities;
            }
            */
            try
            {
                //foreach (JsonData child in in_entities)
                JsonData child = null;
                for (int i = 0; i < in_entities.Count; ++i)
                {
                    child = in_entities[i];
                    BCUserEntity entity = null;
                    entity = NewUserEntity((string)child["entityType"]);
                    entity.ReadFromJson(child);
                    entities.Add(entity);
                }
            }
            catch (System.Exception)
            {
                /* do nadda */
            }

            return entities;
        }
    }
}

#endif
