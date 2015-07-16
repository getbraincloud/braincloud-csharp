//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BrainCloud.Common;
using LitJson;
using BrainCloud.Entity.Internal;

namespace BrainCloud.Entity
{
    public abstract class BCEntity
    {
        protected string m_entityId;
        protected string m_entityType;
        protected ACL m_acl;
        protected int m_version = -1; // skip version checking for now...

        protected IDictionary<string, object> m_data = new Dictionary<string, object>();

        // one way to support delta updates...
        //protected IDictionary<string, object> m_cachedServerData = new Dictionary<string, object>();


        protected enum EntityState
        {
            // New client-side entity - hasn't been fetched from or created on the server
            New = 0,

            // In process of creating the entity on the server
            Creating = 1,

            // Entity created on server and potentially in the process of pushing updates to the server
            Ready = 2,

            // Removed... for now
            //Updating = 3,

            // Entity in the process of being deleted
            Deleting = 4,

            // Entity has been deleted on the server
            Deleted = 5
        }
        private EntityState m_state = EntityState.New;

        // members used if we send a create to the server but in the meantime the client has updated the entity
        protected bool m_updateWhenCreated = false;
        SuccessCallback m_updateWhenCreatedSuccessCb = null;
        FailureCallback m_updateWhenCreatedFailureCb = null;

        // properties managed by the server
        protected DateTime m_createdAt;
        protected DateTime m_updatedAt;

        protected BrainCloudEntity m_braincloud;

        //protected JsonData m_cachedServerObject;


        #region properties
        public string EntityId
        {
            get
            {
                return m_entityId;
            }
        }
        public string EntityType
        {
            get
            {
                return m_entityType;
            }
            set
            {
                m_entityType = value;
            }
        }
        public ACL ACL
        {
            get
            {
                return m_acl;
            }
            set
            {
                m_acl = value;
            }
        }
        public DateTime CreatedAt
        {
            get
            {
                return m_createdAt;
            }
        }
        public DateTime UpdatedAt
        {
            get
            {
                return m_updatedAt;
            }
        }
        protected EntityState State
        {
            get
            {
                return m_state;
            }
            set
            {
                if (value < m_state)
                {
                    throw new ArgumentException("Can't transition to a lower state");
                }
                m_state = value;
            }
        }
        public BrainCloudEntity BrainCloud
        {
            get
            {
                return m_braincloud;
            }
            set
            {
                m_braincloud = value;
            }
        }
        #endregion


        #region abstractMethods
        protected abstract void CreateEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure);
        protected abstract void UpdateEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure);
        protected abstract void UpdateSharedEntity(string in_targetPlayerId, SuccessCallback in_cbSuccess, FailureCallback in_cbFailure);
        protected abstract void DeleteEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure);
        #endregion


        public BCEntity()
        {
        }

        public BCEntity(BrainCloudEntity in_braincloud)
        {
            m_braincloud = in_braincloud;
        }

        /// <summary>
        /// Store the Entity object to the braincloud server. This will result in one of the following operations:
        ///
        /// 1) CreateEntity
        /// 2) UpdateEntity
        /// 3) DeleteEntity
        ///
        /// Certain caveats must be observed:
        /// a) Store operation will be ignored if an entity has been deleted or is in the process of being deleted.
        /// b) TODO: remove this caveat!: Store operation will queue an update if an entity is in the process of being created on the server.
        /// If the entity fails to be created, the update failure callback will not be run.
        ///
        /// </param>
        /// <param name="in_cbSuccess">
        /// A callback to run when store operation is completed successfully.
        /// </param>
        /// <param name="in_cbFailure">
        /// A callback to run when store operation fails.
        /// </param>
        public void StoreAsync(SuccessCallback in_cbSuccess = null, FailureCallback in_cbFailure = null)
        {
            if (m_state == EntityState.Deleting || m_state == EntityState.Deleted)
            {
                return;
            }

            if (m_state == EntityState.Creating)
            {
                // a store async call came in while we are waiting for the server to create the object... queue an update
                m_updateWhenCreated = true;
                m_updateWhenCreatedSuccessCb = in_cbSuccess;
                m_updateWhenCreatedFailureCb = in_cbFailure;
                return;
            }

            if (m_state == EntityState.New)
            {
                CreateEntity(in_cbSuccess, in_cbFailure);

                m_state = EntityState.Creating;
            }
            else
            {
                UpdateEntity(in_cbSuccess, in_cbFailure);
                // we don't currently need a state to say an update is in progress... and if we add this state we
                // need to keep track of how many updates are queued in order to set the state back to ready when *all*
                // updates have completed. So just removing the state for now... an update queued should not have any impact
                // on whether the user can transition to the delete state.
                //m_state = EntityState.Updating;
            }
        }

        /// <summary>
        /// Store the Entity object to the braincloud server. This will result in one of the following operations:
        ///
        /// 1) CreateEntity
        /// 2) UpdateSharedEntity
        /// 3) DeleteEntity
        ///
        /// Certain caveats must be observed:
        /// a) Store operation will be ignored if an entity has been deleted or is in the process of being deleted.
        /// b) TODO: remove this caveat!: Store operation will queue an update if an entity is in the process of being created on the server.
        /// If the entity fails to be created, the update failure callback will not be run.
        ///
        /// </param>
        /// <param name="in_cbSuccess">
        /// A callback to run when store operation is completed successfully.
        /// </param>
        /// <param name="in_cbFailure">
        /// A callback to run when store operation fails.
        /// </param>
        public void StoreAsyncShared(string in_targetPlayerId, SuccessCallback in_cbSuccess = null, FailureCallback in_cbFailure = null)
        {
            if (m_state == EntityState.Deleting || m_state == EntityState.Deleted)
            {
                return;
            }

            if (m_state == EntityState.Creating)
            {
                // a store async call came in while we are waiting for the server to create the object... queue an update
                m_updateWhenCreated = true;
                m_updateWhenCreatedSuccessCb = in_cbSuccess;
                m_updateWhenCreatedFailureCb = in_cbFailure;
                return;
            }

            if (m_state == EntityState.New)
            {
                CreateEntity(in_cbSuccess, in_cbFailure);

                m_state = EntityState.Creating;
            }
            else
            {
                UpdateSharedEntity(in_targetPlayerId, in_cbSuccess, in_cbFailure);
                // we don't currently need a state to say an update is in progress... and if we add this state we
                // need to keep track of how many updates are queued in order to set the state back to ready when *all*
                // updates have completed. So just removing the state for now... an update queued should not have any impact
                // on whether the user can transition to the delete state.
                //m_state = EntityState.Updating;
            }
        }

        /// <summary>
        /// Deletes an entity on the server. If an entity has already been deleted this method will do nothing.
        /// </param>
        /// <param name="in_cbSuccess">
        /// A callback to run when delete operation is completed successfully.
        /// </param>
        /// <param name="in_cbFailure">
        /// A callback to run when delete operation fails.
        /// </param>
        public void DeleteAsync(SuccessCallback in_cbSuccess = null, FailureCallback in_cbFailure = null)
        {
            if (m_state == EntityState.New)
            {
                // preston: caveat - if the object was created asynchronously, and we're still waiting to hear back from the server,
                // the object won't actually get deleted on the server. We can handle this later in the storeAsync/create callback
                return;
            }

            if (m_state == EntityState.Deleting || m_state == EntityState.Deleted)
            {
                // if it's already deleted or being deleted, don't delete again
                return;
            }

            DeleteEntity(in_cbSuccess, in_cbFailure);
            m_state = EntityState.Deleting;
        }

        public bool Contains(string in_key)
        {
            return m_data.ContainsKey(in_key);
        }

        public void Remove(string in_key)
        {
            if (m_data.ContainsKey(in_key))
            {
                m_data.Remove(in_key);
            }
        }


        public T Get<T>(string in_key)
        {
            return EntityUtil.GetObjectAsType<T>(m_data[in_key]);
        }

        public object this[string in_key]
        {
            get
            {
                return m_data[in_key]; // throws Exception if key doesn't exist
            }

            set
            {
                // types:
                // int      : 32 bit signed
                // long     : 64 bit signed
                // float    : 32 bit floating
                // double   : 64 bit floating
                // IList    : list of objects
                // IDictionary  : dictionary of objects

                if (value == null)
                {
                    Remove(in_key);
                    return;
                }

                if (IsBasicType(value)
                        || value is IList
                        || value is IDictionary)
                {
                    // convert DateTime to string as it's not supported by JSON directly
                    if (value is DateTime)
                    {
                        m_data[in_key] = value.ToString();
                    }
                    else
                    {
                        m_data[in_key] = value;
                    }
                }
                //else if (value is BCEntity)
                //{
                // to figure out... we should record the entity id I guess
                //}
                else
                {
                    throw new ArgumentException("Invalid object type");
                }
            }
        }

        protected void UpdateTimeStamps(JsonData in_json)
        {
            try
            {
                m_createdAt = Util.BcTimeToDateTime((long)in_json["createdAt"]);
                m_updatedAt = Util.BcTimeToDateTime((long)in_json["updatedAt"]);
            }
            catch (System.Exception)
            { }

        }

        protected void QueueUpdates()
        {
            if (m_updateWhenCreated)
            {
                StoreAsync(m_updateWhenCreatedSuccessCb, m_updateWhenCreatedFailureCb);
                m_updateWhenCreated = false;
                m_updateWhenCreatedSuccessCb = null;
                m_updateWhenCreatedFailureCb = null;
            }
        }

        private bool IsBasicType(object in_obj)
        {
            return (in_obj is int
                    || in_obj is long
                    || in_obj is float
                    || in_obj is double
                    || in_obj is string
                    || in_obj is DateTime);
        }

        private JsonData ToJsonObjectRecurse(object in_object)
        {
            if (IsBasicType(in_object))
            {
                return new JsonData(in_object);
            }
            else if (in_object is IList)
            {
                JsonData listJson = new JsonData();
                IList objectList = (IList)in_object;
                for (int i = 0; i < objectList.Count; ++i )
                {
                    listJson.Add(ToJsonObjectRecurse( (object)objectList[i]));
                }
                return listJson;
            }
            /*
            else if (in_object is IDictionary<string, object>)
            {
                JsonData dictJson = new JsonData();
                foreach (KeyValuePair<string, object> kv in (IDictionary<string, object>) in_object)
                {
                    dictJson[kv.Key] = ToJsonObjectRecurse(kv.Value);
                }
                return dictJson;
            }
             */
            else if (in_object is IDictionary)
            {
                JsonData dictJson = new JsonData();
                foreach (DictionaryEntry de in (IDictionary)in_object)
                {
                    dictJson[(string)de.Key] = ToJsonObjectRecurse(de.Value);
                }
                return dictJson;
            }
            else
            {
                throw new ArgumentException("Found unknown type, can't serialize to json!");
            }
        }

        public JsonData ToJsonObject()
        {
            JsonData root = new JsonData();

            foreach (KeyValuePair<string, object> entry in m_data)
            {
                root[entry.Key] = ToJsonObjectRecurse(entry.Value);
            }

            return root;
        }

        public string ToJsonString()
        {
            JsonData json = ToJsonObject();
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            JsonMapper.ToJson(json, writer);
            return sb.ToString();
        }

        protected static object JsonToBasicType(JsonData in_jsonObj)
        {
            if (in_jsonObj.IsString)
            {
                return (string)in_jsonObj;
            }
            else if (in_jsonObj.IsLong)
            {
                return (long)in_jsonObj;
            }
            else if (in_jsonObj.IsInt)
            {
                return (int)in_jsonObj;
            }
            else if (in_jsonObj.IsDouble)
            {
                return (double)in_jsonObj;
            }
            else if (in_jsonObj.IsBoolean)
            {
                return (bool)in_jsonObj;
            }

            throw new ArgumentException("Unexpected type");
        }

        protected static IList<object> JsonToList(JsonData in_jsonObj)
        {
            List<object> list = new List<object>();
            //foreach (JsonData child in in_jsonObj)
            JsonData child = null;
            for (int i = 0; i < in_jsonObj.Count; ++i)
            {
                child = in_jsonObj[i];
                if (child.IsObject)
                {
                    list.Add(JsonToDictionary(child));
                }
                else if (child.IsArray)
                {
                    list.Add(JsonToList(child));
                }
                else
                {
                    list.Add(JsonToBasicType(child));
                }
            }
            return list;
        }

        protected static IDictionary<string, object> JsonToDictionary(JsonData in_jsonObj)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DictionaryEntry child in in_jsonObj)
            {
                var childValue = child.Value as JsonData;
                var childKey = child.Key as string;
                
                if (childValue.IsObject)
                {
                    dict[childKey] = JsonToDictionary(childValue);
                }
                else if (childValue.IsArray)
                {
                    dict[childKey] = JsonToList(childValue);
                }
                else
                {
                    dict[childKey] = JsonToBasicType(childValue);
                }
            }

            return dict;
        }

        public void ReadFromJson(string in_json)
        {
            JsonData jsonObj = JsonMapper.ToObject(in_json);
            ReadFromJson(jsonObj);
        }

        public void ReadFromJson(JsonData in_jsonObj)
        {
            m_state = EntityState.Ready;
            m_entityType = (string)in_jsonObj["entityType"];
            m_entityId = (string)in_jsonObj["entityId"];
            m_acl = ACL.CreateFromJson(in_jsonObj["acl"]);
            UpdateTimeStamps(in_jsonObj);
            m_data = JsonToDictionary(in_jsonObj["data"]);
        }

        public override string ToString()
        {
            return ToJsonString();
        }
    }
}
