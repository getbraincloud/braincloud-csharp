//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//
// Class serves as an internal end of message bundle marker
//----------------------------------------------------

using System.Collections;

namespace BrainCloud.Internal
{
    internal class EndOfBundleMarker : ServerCall
    {
        public EndOfBundleMarker()
            : base(ServiceName.HeartBeat, ServiceOperation.Send, null, null)
        {
        }
    }
}