//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{
    public static class StatusCodes
    {
        public const int OK = 200;

        /// <summary>
        /// Status code for a client side error
        /// </summary>
        public const int CLIENT_NETWORK_ERROR = 900;

        /// <summary>
        /// Status code for an internal server error
        /// </summary>
        public const int INTERNAL_SERVER_ERROR = 500;

        public const int BAD_REQUEST = 400;
        
        public const int FORBIDDEN = 403;
    }
}
