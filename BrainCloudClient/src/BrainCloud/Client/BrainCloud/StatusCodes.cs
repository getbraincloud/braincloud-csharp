//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace BrainCloud
{
    public static class StatusCodes
    {

        /// <summary>
        /// Status code for a client side error
        /// </summary>
        public const int CLIENT_NETWORK_ERROR = 900;

        /// <summary>
        /// Status code for an internal server error
        /// </summary>
        public const int INTERNAL_SERVER_ERROR = 500;
    }
}
