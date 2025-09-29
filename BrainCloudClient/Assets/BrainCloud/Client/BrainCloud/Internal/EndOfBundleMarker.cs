// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//
// Class serves as an internal end of message bundle marker
//----------------------------------------------------

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