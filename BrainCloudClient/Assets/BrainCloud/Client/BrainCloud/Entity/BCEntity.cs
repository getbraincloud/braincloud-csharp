//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using System;
using System.Collections.Generic;
using BrainCloud.Common;
using JsonFx.Json;
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
        protected abstract void CreateEntity(SuccessCallback success, FailureCallback failure);
        protected abstract void UpdateEntity(SuccessCallback success, FailureCallback failure);
        protected abstract void UpdateSharedEntity(string targetProfileId, SuccessCallback success, FailureCallback failure);
        protected abstract void DeleteEntity(SuccessCallback success, FailureCallback failure);
        #endregion


        public BCEntity()
        {
        }

        public BCEntity(BrainCloudEntity braincloud)
        {
            m_braincloud = braincloud;
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
        /// <param name="success">
        /// A callback to run when store operation is completed successfully.
        /// </param>
        /// <param name="failure">
        /// A callback to run when store operation fails.
        /// </param>
        public void StoreAsync(SuccessCallback success = null, FailureCallback failure = null)
        {
            if (m_state == EntityState.Deleting || m_state == EntityState.Deleted)
            {
                return;
            }

            if (m_state == EntityState.Creating)
            {
                // a store async call came in while we are waiting for the server to create the object... queue an update
                m_updateWhenCreated = true;
                m_updateWhenCreatedSuccessCb = success;
                m_updateWhenCreatedFailureCb = failure;
                return;
            }

            if (m_state == EntityState.New)
            {
                CreateEntity(success, failure);

                m_state = EntityState.Creating;
            }
            else
            {
                UpdateEntity(success, failure);
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
        /// <param name="success">
        /// A callback to run when store operation is completed successfully.
        /// </param>
        /// <param name="failure">
        /// A callback to run when store operation fails.
        /// </param>
        public void StoreAsyncShared(string targetProfileId, SuccessCallback success = null, FailureCallback failure = null)
        {
            if (m_state == EntityState.Deleting || m_state == EntityState.Deleted)
            {
                return;
            }

            if (m_state == EntityState.Creating)
            {
                // a store async call came in while we are waiting for the server to create the object... queue an update
                m_updateWhenCreated = true;
                m_updateWhenCreatedSuccessCb = success;
                m_updateWhenCreatedFailureCb = failure;
                return;
            }

            if (m_state == EntityState.New)
            {
                CreateEntity(success, failure);

                m_state = EntityState.Creating;
            }
            else
            {
                UpdateSharedEntity(targetProfileId, success, failure);
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
        /// <param name="success">
        /// A callback to run when delete operation is completed successfully.
        /// </param>
        /// <param name="failure">
        /// A callback to run when delete operation fails.
        /// </param>
        public void DeleteAsync(SuccessCallback success = null, FailureCallback failure = null)
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

            DeleteEntity(success, failure);
            m_state = EntityState.Deleting;
        }

        public bool Contains(string key)
        {
            return m_data.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (m_data.ContainsKey(key))
            {
                m_data.Remove(key);
            }
        }


        public T Get<T>(string key)
        {
            return EntityUtil.GetObjectAsType<T>(m_data[key]);
        }

        public object this[string key]
        {
            get
            {
                return m_data[key]; // throws Exception if key doesn't exist
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
                    Remove(key);
                    return;
                }

                m_data[key] = value;
            }
        }

        protected void UpdateTimeStamps(Dictionary<string, object> json)
        {
            try
            {
                m_createdAt = Util.BcTimeToDateTime((long)json["createdAt"]);
                m_updatedAt = Util.BcTimeToDateTime((long)json["updatedAt"]);
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

        public string ToJsonString()
        {
            return JsonWriter.Serialize(m_data);
        }

        public void ReadFromJson(string json)
        {
            object jsonObj = JsonReader.Deserialize(json);
            ReadFromJson(jsonObj);
        }

        public void ReadFromJson(object jsonObj)
        {
            Dictionary<string, object> jsonDict = (Dictionary<string, object>)jsonObj;
            m_state = EntityState.Ready;
            m_entityType = (string)jsonDict["entityType"];
            m_entityId = (string)jsonDict["entityId"];
            m_acl = ACL.CreateFromJson((string)jsonDict["acl"]);
            UpdateTimeStamps(jsonDict);
            m_data = jsonDict["data"] as Dictionary<string, object>;
        }

        public override string ToString()
        {
            return ToJsonString();
        }
    }
}

#endif
