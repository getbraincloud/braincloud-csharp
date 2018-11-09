//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using System;
using System.Collections.Generic;
using JsonFx.Json;
using System.Reflection;

namespace BrainCloud.Entity
{
    public class BCEntityFactory
    {
        private BrainCloudEntity m_bcEntityService;
        private IDictionary<string, ConstructorInfo> m_registeredClasses;

        public delegate BCUserEntity CreateUserEntityFromType(string type);

        public BCEntityFactory(BrainCloudEntity in_bcEntityService)
        {
            m_bcEntityService = in_bcEntityService;
            m_registeredClasses = new Dictionary<string, ConstructorInfo>();
        }

        public T NewEntity<T>(string entityType) where T : BCEntity
        {
            T e = (T)CreateRegisteredEntityClass(entityType);
            
            //we're never creating the instance before as suspected. 
            if (e == null)
            {
                //added so new entity would actually create an instance THIS WORKS! Creates the exact kind of instance we needed!
                e = (T)Activator.CreateInstance(typeof(T), new Object[] { m_bcEntityService });
            }
            e.BrainCloudEntityService = m_bcEntityService;
            e.EntityType = entityType;
            return e;
        }

        public BCUserEntity NewUserEntity(string entityType)
        {
            BCUserEntity e = (BCUserEntity)CreateRegisteredEntityClass(entityType);
            if (e == null)
            {
                e = new BCUserEntity(m_bcEntityService);
            }
            e.EntityType = entityType;
            return e;
        }

        public IList<BCUserEntity> NewUserEntitiesFromGetList(string json)
        {
            Dictionary<string, object> jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, (Array)((Dictionary<string, object>)jsonObj["data"])["entityList"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromReadPlayerState(string json)
        {
            Dictionary<string, object> jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, (Array)((Dictionary<string, object>)jsonObj["data"])["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromStartMatch(string json)
        {
            Dictionary<string, object> jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, (Array)((Dictionary<string, object>)((Dictionary<string, object>)jsonObj["data"])["initialSharedData"])["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public IList<BCUserEntity> NewUserEntitiesFromDataResponse(string json)
        {
            Dictionary<string, object> jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(json);
            try
            {
                return NewUserEntitiesFromJsonString(json, (Array)((Dictionary<string, object>)((Dictionary<string, object>)jsonObj["data"])["response"])["entities"]);
            }
            catch (KeyNotFoundException)
            {
                return new List<BCUserEntity>();
            }
        }

        public void RegisterEntityClass<T>(string entityType) where T : BCEntity
        {
            Type type = typeof(T);
            Type[] constructorParams = new Type[] { };

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

        public BCUserEntity NewUserFromDictionary(Dictionary<string, object> in_dict)
        {
            BCUserEntity toReturn = null;
            if (in_dict != null)
            {
                try
                {
                    toReturn = NewUserEntity((string)in_dict["entityType"]);
                    toReturn.ReadFromJson(in_dict);
                }
                catch (Exception)
                {
                    /* do nadda */
                }
            }

            return toReturn;
        }

        // the list of entitiies
        public IList<BCUserEntity> NewUserEntitiesFromJsonString(string json, Array entitiesJson)
        {
            List<BCUserEntity> entities = new List<BCUserEntity>();
            Dictionary<string, object> child = null;
            for (int i = 0; i < entitiesJson.Length; ++i)
            {
                try
                {
                    child = entitiesJson.GetValue(i) as Dictionary<string, object>;
                    BCUserEntity entity = NewUserFromDictionary(child);
                    entities.Add(entity);
                }
                catch (Exception)
                {
                    /* do nadda */
                }
            }
            return entities;
        }
    }
}

#endif
