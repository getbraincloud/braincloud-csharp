//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
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

        public Access Other { get; set; }

        public ACL() { }

        public ACL(Access access)
        {
            Other = access;
        }

        public static ACL ReadOnlyOther()
        {
            ACL acl = new ACL();
            acl.Other = Access.ReadOnly;
            return acl;
        }

        public static ACL CreateFromJson(string json)
        {
            ACL acl = new ACL();
            acl.ReadFromJson(json);
            return acl;
        }

        public void ReadFromJson(string json)
        {
            Dictionary<string, object> jsonObj = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(json);
            Other = (Access)(int)jsonObj["other"];
        }

        public string ToJsonString()
        {
            Dictionary<string, object> jsonObj = new Dictionary<string, object> { { "other", (int)Other } };
            return JsonFx.Json.JsonWriter.Serialize(jsonObj);
        }

#if !XAMARIN
        public static ACL CreateFromJson(JsonData json)
        {
            ACL acl = new ACL();
            acl.ReadFromJson(json);
            return acl;
        }

        public void ReadFromJson(JsonData json)
        {
            Other = (Access)(int)json["other"];
        }
#endif
    }
}
