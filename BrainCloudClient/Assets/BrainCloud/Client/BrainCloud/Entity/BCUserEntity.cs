//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using System;
using System.Collections.Generic;
using System.Text;
using LitJson;


namespace BrainCloud.Entity
{
    public class BCUserEntity : BCEntity
    {
        public BCUserEntity()
        {
        }

        public BCUserEntity(BrainCloudEntity braincloud)
        {
            m_braincloud = braincloud;
        }

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



        public void CbCreateSuccess(string jsonString, object cbObject)
        {
            JsonData json = JsonMapper.ToObject(jsonString);
            UpdateTimeStamps(json["data"]);

            m_entityId = (string) json["data"]["entityId"];

            State = EntityState.Ready;

            QueueUpdates(); // important - kicks off any queued updates that happened before we retrieved an id from the server
        }

        public void CbCreateFailure(int statusCode, int reasonCode, string statusMessage, object cbObject)
        {

        }

        public void CbUpdateSuccess(string jsonString, object cbObject)
        {
            JsonData json = JsonMapper.ToObject(jsonString);
            UpdateTimeStamps(json["data"]);
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
    }
}

#endif
