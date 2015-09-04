//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace BrainCloud
{
    public static class ReasonCodes
    {
        /// <summary>This means that you have provided a profile id
        /// but the identity lookup fails to find an identity entry.
        /// </summary>
        public const int MISSING_IDENTITY_ERROR = 40206;

        /// <summary>This means that you have provided a profile id and an
        /// identity that is matched to a different  profile id.
        /// This is where you blank out the profile to allow the switch.
        /// </summary>
        public const int SWITCHING_PROFILES = 40207;

        /// <summary>This means that you provide a blank profile id and the
        /// identity doesn’t exist and forecCreate is false.
        ///</summary>
        public const int MISSING_PROFILE_ERROR = 40208;

        /// <summary>Raised when a security error occurs
        /// </summary>
        public const int SECURITY_ERROR = 40209;

        /// <summary>This happens when you try and detach the last non-anonymous
        /// identity from an account with confirmAnonymous set to false.
        /// </summary>
        public const int DOWNGRADING_TO_ANONYMOUS_ERROR = 40210;

        /// <summary>This occurs when you try and attach an identity type that already exists for that profile.
        /// You can have only one facebook identity for a particular profile
        /// </summary>
        public const int DUPLICATE_AUTHENTICATED_IDENTITY_ERROR = 40211;

        /// <summary>Cannot validate player</summary>
        public const int CANNOT_VALIDATE_PLAYER = 40302;

        /// <summary>Session expired</summary>
        public const int SESSION_EXPIRED = 40303;

        /// <summary>SESSION NOT FOUND ERROR</summary>
        public const int SESSION_NOT_FOUND_ERROR = 40304;

        /// <summary>Player provided the wrong email and / or password</summary>
        public const int WRONG_EMAIL_AND_PASSWORD = 40307;

        /// <summary>This happens when you provide invalid auth type string in either service.</summary>
        public const int INVALID_AUTHENTICATION_TYPE = 40315;

        /// <summary>You must have an active session in order to call this api</summary>
        public const int INVALID_GAME_ID = 40316;

        /// <summary>Player is currently in a match</summary>
        public const int PLAYER_RATING_OUT_OF_RANGE = 40334;

        /// <summary>Player is currently in a match</summary>
        public const int PLAYER_CURRENTLY_IN_MATCH = 40335;

        /// <summary>Player is currently shielded</summary>
        public const int PLAYER_CURRENTLY_IS_SHIELDED = 40336;

        /// <summary>Player is currently logged in</summary>
        public const int PLAYER_CURRENTLY_LOGGED_IN = 40338;

        /// <summary>Max Concurrent Player Count Reached</summary>
        public const int MAX_CONCURRENT_PLAYER_COUNT_REACHED = 40356;

        /// <summary>Game has to parent to switch profile to</summary>
        public const int GAME_HAS_NO_PARENT = 40377;

        /// <summary>
        /// Client defined value for a timeout detected client-side.
        /// </summary>
        public const int CLIENT_NETWORK_ERROR_TIMEOUT = 90001;
    }
}
