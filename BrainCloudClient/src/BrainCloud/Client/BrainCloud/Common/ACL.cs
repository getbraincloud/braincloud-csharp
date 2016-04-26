//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

#if !XAMARIN
using LitJson;
#endif
using System.Collections.Generic;

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

        public ACL() { }

        public ACL(Access access)
        {
            m_other = access;
        }

        public Access Other
        {
            get { return m_other; }
            set { m_other = value; }
        }

        public static ACL ReadOnlyOther()
        {
            ACL acl = new ACL();
            acl.m_other = Access.ReadOnly;
            return acl;
        }

        public static ACL CreateFromJson(string in_json)
        {
            ACL acl = new ACL();
            acl.ReadFromJson(in_json);
            return acl;
        }

        public void ReadFromJson(string in_json)
        {
            Dictionary<string, object> jsonObj = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(in_json);
            m_other = (Access)(int)jsonObj["other"];
        }

        public string ToJsonString()
        {
            Dictionary<string, object> jsonObj = new Dictionary<string, object> { { "other", (int)m_other } };
            return JsonFx.Json.JsonWriter.Serialize(jsonObj);
        }

        public override string ToString()
        {
            return ToJsonString();
        }

#if !XAMARIN
        public static ACL CreateFromJson(JsonData in_json)
        {
            ACL acl = new ACL();
            acl.ReadFromJson(in_json);
            return acl;
        }

        public void ReadFromJson(JsonData in_json)
        {
            m_other = (Access)(int)in_json["other"];
        }
#endif
    }
}
