using System;

namespace BrainCloud.Internal
{
    public class WrapperAuthCallbackObject
    {
        public object _cbObject;
        public SuccessCallback _successCallback;
        public FailureCallback _failureCallback;

        public WrapperAuthCallbackObject ()
        {
        }
    }
}
