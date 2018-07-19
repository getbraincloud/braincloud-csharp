//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using System;
using System.Collections.Generic;
using System.Text;
using JsonFx.Json;


namespace BrainCloud.Entity
{
    public class BCUserEntity : BCEntity
    {
        #region public 
        public BCUserEntity()
        {
        }

        public BCUserEntity(BrainCloudEntity braincloud)
        {
            m_braincloud = braincloud;
        }


        public void CbCreateSuccess(string jsonString, object cbObject)
        {
            //JsonData json = JsonMapper.ToObject(jsonString);
            Dictionary<string, object> json = JsonReader.Deserialize<Dictionary<string, object>>(jsonString);
            Dictionary<string, object> data = (Dictionary<string, object>)json["data"];
            UpdateTimeStamps(data);

            m_entityId = (string)data["entityId"];

            State = EntityState.Ready;

            QueueUpdates(); // important - kicks off any queued updates that happened before we retrieved an id from the server
        }

        public void CbCreateFailure(int statusCode, int reasonCode, string statusMessage, object cbObject)
        {

        }

        public void CbUpdateSuccess(string jsonString, object cbObject)
        {
            Dictionary<string, object> json = JsonReader.Deserialize<Dictionary<string, object>>(jsonString);
            Dictionary<string, object> data = (Dictionary<string, object>)json["data"];
            UpdateTimeStamps(data);
        }

        public void CbUpdateFailure(int statusCode, int reasonCode, string statusMessage, object cbObject)
        {

        }

        public void CbDeleteSuccess(string json, object cbObject)
        {
            State = EntityState.Deleted;
        }

        public void CbDeleteFailure(int statusCode, int reasonCode, string statusMessage, object cbObject)
        {

        }
        #endregion

        #region protected
        protected override void CreateEntity(SuccessCallback success, FailureCallback failure)
        {
            string jsonData = ToJsonString();
            string jsonAcl = m_acl == null ? null : m_acl.ToJsonString();
            m_braincloud.CreateEntity(m_entityType, jsonData, jsonAcl, CbCreateSuccess + success, CbCreateFailure + failure, this);
        }

        protected override void UpdateEntity(SuccessCallback success, FailureCallback failure)
        {
            string jsonData = ToJsonString();
            string jsonAcl = m_acl == null ? null : m_acl.ToJsonString();
            m_braincloud.UpdateEntity(m_entityId, m_entityType, jsonData, jsonAcl, m_version, CbUpdateSuccess + success, CbUpdateFailure + failure, this);
        }

        protected override void UpdateSharedEntity(string targetProfileId, SuccessCallback success, FailureCallback failure)
        {
            string jsonData = ToJsonString();
            m_braincloud.UpdateSharedEntity(m_entityId, targetProfileId, m_entityType, jsonData, m_version, CbUpdateSuccess + success, CbUpdateFailure + failure, this);
        }

        protected override void DeleteEntity(SuccessCallback success, FailureCallback failure)
        {
            m_braincloud.DeleteEntity(m_entityId, m_version, CbDeleteSuccess + success, CbDeleteFailure + failure, this);
        }
        #endregion
    }
}

#endif
