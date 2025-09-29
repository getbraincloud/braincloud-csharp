// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud.Internal
{

    using System;

    public class WrapperAuthCallbackObject
    {
        public object _cbObject;
        public SuccessCallback _successCallback;
        public FailureCallback _failureCallback;

        public WrapperAuthCallbackObject()
        {
        }
    }
}
