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

        public BCUserEntity(BrainCloudEntity in_braincloud)
        {
            m_braincloud = in_braincloud;
        }

        protected override void CreateEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure)
        {
            string jsonData = ToJsonString();
            string jsonAcl = m_acl == null ? null : m_acl.ToJsonString();
            m_braincloud.CreateEntity(m_entityType, jsonData, jsonAcl, CbCreateSuccess + in_cbSuccess, CbCreateFailure + in_cbFailure, this);
        }

        protected override void UpdateEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure)
        {
            string jsonData = ToJsonString();
            string jsonAcl = m_acl == null ? null : m_acl.ToJsonString();
            m_braincloud.UpdateEntity(m_entityId, m_entityType, jsonData, jsonAcl, m_version, CbUpdateSuccess + in_cbSuccess, CbUpdateFailure + in_cbFailure, this);
        }

        protected override void UpdateSharedEntity(string in_targetPlayerId, SuccessCallback in_cbSuccess, FailureCallback in_cbFailure)
        {
            string jsonData = ToJsonString();
            m_braincloud.UpdateSharedEntity(m_entityId, in_targetPlayerId, m_entityType, jsonData, m_version, CbUpdateSuccess + in_cbSuccess, CbUpdateFailure + in_cbFailure, this);
        }

        protected override void DeleteEntity(SuccessCallback in_cbSuccess, FailureCallback in_cbFailure)
        {
            m_braincloud.DeleteEntity(m_entityId, m_version, CbDeleteSuccess + in_cbSuccess, CbDeleteFailure + in_cbFailure, this);
        }



        public void CbCreateSuccess(string in_json, object in_cbObject)
        {
            JsonData json = JsonMapper.ToObject(in_json);
            UpdateTimeStamps(json["data"]);

            m_entityId = (string) json["data"]["entityId"];

            State = EntityState.Ready;

            QueueUpdates(); // important - kicks off any queued updates that happened before we retrieved an id from the server
        }

        public void CbCreateFailure(int statusCode, int reasonCode, string statusMessage, object in_cbObject)
        {

        }

        public void CbUpdateSuccess(string in_json, object in_cbObject)
        {
            JsonData json = JsonMapper.ToObject(in_json);
            UpdateTimeStamps(json["data"]);
        }

        public void CbUpdateFailure(int statusCode, int reasonCode, string statusMessage, object in_cbObject)
        {

        }

        public void CbDeleteSuccess(string in_json, object in_cbObject)
        {
            State = EntityState.Deleted;
        }

        public void CbDeleteFailure(int statusCode, int reasonCode, string statusMessage, object in_cbObject)
        {

        }
    }
}

#endif
