//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace BrainCloud.Common
{
    public class ACL
    {
        public enum Access : int
        {
            None = 0,
            ReadOnly = 1,
            ReadWrite = 2
        }

        private Access m_other;

        public ACL()
        {
        }

        public Access Other
        {
            get
            {
                return m_other;
            }
            set
            {
                m_other = value;
            }
        }

        public static ACL ReadOnlyOther()
        {
            ACL acl = new ACL();
            acl.m_other = Access.ReadOnly;
            return acl;
        }

        public static ACL CreateFromJson(string in_json)
        {
            JsonData jsonObj = JsonMapper.ToObject(in_json);
            return CreateFromJson(jsonObj);
        }

        public static ACL CreateFromJson(JsonData in_json)
        {
            ACL acl = new ACL();
            acl.ReadFromJson(in_json);
            return acl;
        }

        public void ReadFromJson(string in_json)
        {
            JsonData jsonObj = JsonMapper.ToObject(in_json);
            ReadFromJson(jsonObj);
        }

        public void ReadFromJson(JsonData in_json)
        {
            m_other = (Access) (int) in_json["other"];
        }

        public string ToJsonString()
        {
            JsonData acl = new JsonData();
            acl["other"] = new JsonData((int) m_other);

            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            JsonMapper.ToJson(acl, writer);
            return sb.ToString();
        }

        public override string ToString()
        {
            return ToJsonString();
        }
    }
}
