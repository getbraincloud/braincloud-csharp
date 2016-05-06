//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using JsonFx.Json;
using System.Collections.Generic;

namespace BrainCloud.Common
{
    public class GroupACL : AclBase
    {
        public Access Other { get; set; }
        public Access Member { get; set; }

        public GroupACL() { }

        public GroupACL(Access otherAccess, Access memberAccess)
        {
            Other = otherAccess;
            Member = otherAccess;
        }

        public static GroupACL CreateFromJson(string in_json)
        {
            GroupACL acl = new GroupACL();
            acl.ReadFromJson(in_json);
            return acl;
        }

        public override void ReadFromJson(string in_json)
        {
            Dictionary<string, object> jsonObj = JsonReader.Deserialize<Dictionary<string, object>>(in_json);
            Other = (Access)jsonObj["other"];
            Member = (Access)jsonObj["member"];
        }

        public override string ToJsonString()
        {
            Dictionary<string, object> jsonObj = new Dictionary<string, object>
            { { "other", (int)Other }, { "member", (int)Member } };
            return JsonWriter.Serialize(jsonObj);
        }
    }
}
