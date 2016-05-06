//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud.Common
{
    public abstract class AclBase
    {
        public enum Access : int
        {
            None = 0,
            ReadOnly = 1,
            ReadWrite = 2
        }

        public abstract void ReadFromJson(string in_json);

        public abstract string ToJsonString();

        public override string ToString() { return ToJsonString(); }
    }
}
