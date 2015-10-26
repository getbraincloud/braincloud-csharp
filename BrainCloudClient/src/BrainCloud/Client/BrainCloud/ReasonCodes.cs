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
        public const int NO_REASON_CODE = 0;
        
        public const int INVALID_NOTIFICATION = 20200;
        
        public const int INVALID_REQUEST = 40001;

        public const int SWITCHING_FACEBOOK_MEMORY = 40201;
        public const int MERGING_MEMORY = 40202;
        public const int RECREATING_ANONYMOUS_MEMORY = 40203;
        public const int MOVING_ANONYMOUS_MEMORY = 40204;
        public const int LOGIN_SECURITY_ERROR = 40205;

        /// <summary>This means that you have provided a profile id
        /// but the identity lookup fails to find an identity entry.
        /// </summary>
        public const int MISSING_IDENTITY_ERROR = 40206;

        /// <summary>This means that you have provided a profile id and an
        /// identity that is matched to a different profile id.
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
        public const int DUPLICATE_IDENTITY_TYPE = 40211;
        [Obsolete("use DUPLICATE_IDENTITY_TYPE instead")]
        public const int DUPLICATE_AUTHENTICATED_IDENTITY_ERROR = DUPLICATE_IDENTITY_TYPE;


        public const int MERGE_PROFILES = 40212;
        public const int INVALID_PROPERTY_NAME = 40213;
        public const int EMAIL_NOT_VALIDATED = 40214;

        public const int UNABLE_TO_GET_FRIENDS_FROM_FACEBOOK = 40300;
        public const int BAD_SIGNATURE = 40301;

        /// <summary>Cannot validate player
        /// </summary>
        public const int UNABLE_TO_VALIDATE_PLAYER = 40302;
        [Obsolete("use UNABLE_TO_VALIDATE_PLAYER instead")]
        public const int CANNOT_VALIDATE_PLAYER = UNABLE_TO_VALIDATE_PLAYER;

        /// <summary>Session expired</summary>
        public const int PLAYER_SESSION_EXPIRED = 40303;
        [Obsolete("use PLAYER_SESSION_EXPIRED instead")]
        public const int SESSION_EXPIRED = PLAYER_SESSION_EXPIRED;


        /// <summary>SESSION NOT FOUND ERROR</summary>
        public const int NO_SESSION = 40304;
        [Obsolete("use NO_SESSION instead")]
        public const int SESSION_NOT_FOUND_ERROR = NO_SESSION;

        public const int PLAYER_SESSION_MISMATCH = 40305;
        public const int OPERATION_REQUIRES_A_SESSION = 40306;

        /// <summary>Player provided the wrong email and / or password</summary>
        public const int TOKEN_DOES_NOT_MATCH_USER = 40307;
        [Obsolete("use TOKEN_DOES_NOT_MATCH_USER instead")]
        public const int WRONG_EMAIL_AND_PASSWORD = TOKEN_DOES_NOT_MATCH_USER;

        public const int EVENT_CAN_ONLY_SEND_TO_FRIEND_OR_SELF = 40309;
        public const int NOT_FRIENDS = 40310;
        public const int VC_BALANCE_CANNOT_BE_SPECIFIED = 40311;
        public const int VC_LIMIT_EXCEEDED = 40312;
        public const int UNABLE_TO_GET_MY_DATA_FROM_FACEBOOK = 40313;

        /// <summary>This happens when you provide invalid auth type string in either service.</summary>
        public const int INVALID_AUTHENTICATION_TYPE = 40315;

        /// <summary>You must have an active session in order to call this api</summary>
        public const int INVALID_GAME_ID = 40316;

        /// <summary>This product and receipt have already been claimed</summary>
        public const int APPLE_TRANS_ID_ALREADY_CLAIMED = 40317;
        [Obsolete("use APPLE_TRANS_ID_ALREADY_CLAIMED instead")]
        public const int ITUNES_PURCHASE_ALREADY_CLAIMED = APPLE_TRANS_ID_ALREADY_CLAIMED;

        public const int CLIENT_VERSION_NOT_SUPPORTED = 40318;
        public const int BRAINCLOUD_VERSION_NOT_SUPPORTED = 40319;
        public const int PLATFORM_NOT_SUPPORTED = 40320;
        public const int INVALID_PLAYER_STATISTICS_EVENT_NAME = 40321;
       
        /// <summary>App Version No Longer Supported</summary>
        public const int GAME_VERSION_NOT_SUPPORTED = 40322;
        [Obsolete("use GAME_VERSION_NOT_SUPPORTED instead")]
        public const int APP_VERSION_NO_LONGER_SUPPORTED = GAME_VERSION_NOT_SUPPORTED;

        public const int BAD_REFERENCE_DATA = 40324;
        public const int MISSING_OAUTH_TOKEN = 40325;
        public const int MISSING_OAUTH_VERIFIER = 40326;
        public const int MISSING_OAUTH_TOKEN_SECRET = 40327;
        public const int MISSING_TWEET = 40328;
        public const int FACEBOOK_PAYMENT_ID_ALREADY_PROCESSED = 40329;
        public const int DISABLED_GAME = 40330;
        public const int MATCH_MAKING_DISABLED = 40331;
        public const int UPDATE_FAILED = 40332;
        public const int INVALID_OPERATION = 40333;  // invalid operation for API call

        /// <summary>Player is currently in a match</summary>
        public const int MATCH_RANGE_ERROR = 40334;
        [Obsolete("use MATCH_RANGE_ERROR instead")]
        public const int PLAYER_RATING_OUT_OF_RANGE = MATCH_RANGE_ERROR;

        /// <summary>Player is currently in a match</summary>
        public const int PLAYER_IN_MATCH = 40335;
        [Obsolete("use PLAYER_IN_MATCH instead")]
        public const int PLAYER_CURRENTLY_IN_MATCH = PLAYER_IN_MATCH;

        /// <summary>Player is currently shielded</summary>
        public const int MATCH_PLAYER_SHIELDED = 40336;
        [Obsolete("use MATCH_PLAYER_SHIELDED instead")]
        public const int PLAYER_CURRENTLY_IS_SHIELDED = MATCH_PLAYER_SHIELDED;

        public const int MATCH_PLAYER_MISSING = 40337;

        /// <summary>Player is currently logged in</summary>
        public const int MATCH_PLAYER_LOGGED_IN = 40338;
        [Obsolete("use MATCH_PLAYER_LOGGED_IN instead")]
        public const int PLAYER_CURRENTLY_LOGGED_IN = MATCH_PLAYER_LOGGED_IN;

        public const int INVALID_ITEM_ID = 40339;
        public const int MISSING_PRICE = 40340;
        public const int MISSING_USER_INFO = 40341;
        public const int MISSING_STEAM_RESPONSE = 40342;
        public const int MISSING_STEAM_TRANSACTION = 40343;
        public const int ENTITY_VERSION_MISMATCH = 40344;
        public const int MISSING_RECORD = 40345;
        public const int INSUFFICIENT_PERMISSIONS = 40346;
        public const int MISSING_IN_QUERY = 40347;
        public const int RECORD_EXPIRED = 40348;
        public const int INVALID_WHERE = 40349;
        public const int S3_ERROR = 40350;
        public const int INVALID_ATTRIBUTES = 40351;
        public const int IMPORT_MISSING_GAME_DATA = 40352;
        public const int IMPORT_SCHEMA_VERSION_TOO_OLD = 40353;
        public const int IMPORT_SCHEMA_VERSION_INVALID = 40355;

        /// <summary>Max Concurrent Player Count Reached</summary>
        public const int PLAYER_SESSION_LOGGED_OUT = 40356;
        [Obsolete("use PLAYER_SESSION_LOGGED_OUT instead")]
        public const int MAX_CONCURRENT_PLAYER_COUNT_REACHED = PLAYER_SESSION_LOGGED_OUT;

        public const int API_HOOK_SCRIPT_ERROR = 40357;
        public const int MISSING_REQUIRED_PARAMETER = 40358;
        public const int INVALID_PARAMETER_TYPE = 40359;
        public const int INVALID_IDENTITY_TYPE = 40360;
        public const int EMAIL_SEND_ERROR = 40361;
        public const int CHILD_ENTITY_PARTIAL_UPDATE_INVALID_DATA = 40362;
        public const int MISSING_SCRIPT = 40363;
        public const int SCRIPT_SECURITY_ERROR = 40364;  
        public const int SERVER_SESSION_EXPIRED = 40365;
        public const int STREAM_DOES_NOT_EXIT = 40366;
        public const int STREAM_ACCESS_ERROR = 40367;
        public const int STREAM_COMPLETE = 40368;
        public const int INVALID_STATISTIC_NAME = 40369;
        public const int INVALID_HTTP_REQUEST = 40370;
        public const int GAME_LIMIT_REACHED = 40371;
        public const int GAME_RUNSTATE_DISABLED = 40372;
        public const int INVALID_COMPANY_ID = 40373;
        public const int INVALID_PLAYER_ID = 40374;
        public const int INVALID_TEMPLATE_ID = 40375;
        public const int MINIMUM_SEARCH_INPUT = 40376;

        /// <summary>Game has to parent to switch profile to</summary>
        public const int MISSING_GAME_PARENT = 40377;
        [Obsolete("use MISSING_GAME_PARENT instead")]
        public const int GAME_HAS_NO_PARENT = MISSING_GAME_PARENT;

        public const int GAME_PARENT_MISMATCH = 40378;
        public const int CHILD_PLAYER_MISSING = 40379;
        public const int MISSING_PLAYER_PARENT = 40380;
        public const int PLAYER_PARENT_MISMATCH = 40381;
        public const int MISSING_PLAYER_ID = 40382;
        public const int DECODE_CONTEXT = 40383;
        public const int INVALID_QUERY_CONTEXT = 40384;
        public const int GROUP_MEMBER_NOT_FOUND = 40385;
        public const int INVALID_SORT = 40386;
        public const int GAME_NOT_FOUND = 40387;
        public const int GAMES_NOT_IN_SAME_COMPANY = 40388;
        public const int IMPORT_NO_PARENT_ASSIGNED = 40389;
        public const int IMPORT_PARENT_CURRENCIES_MISMATCH = 40390;
        public const int INVALID_SUBSTITUION_ENTRY = 40391;
        public const int INVALID_TEMPLATE_STRING = 40392;
        public const int TEMPLATE_SUBSTITUTION_ERROR = 40393;
        public const int INVALID_OPPONENTS = 40394;
        public const int REDEMPTION_CODE_NOT_FOUND = 40395;
        public const int REDEMPTION_CODE_VERSION_MISMATCH = 40396;
        public const int REDEMPTION_CODE_ACTIVE = 40397;
        public const int REDEMPTION_CODE_NOT_ACTIVE = 40398;
        public const int REDEMPTION_CODE_INVALID = 40399;
        public const int REDEMPTION_CODE_REDEEMED = 40400;
        public const int REDEMPTION_CODE_REDEEMED_BY_SELF = 40401;
        public const int REDEMPTION_CODE_REDEEMED_BY_OTHER = 40402;
        
        public const int NO_TWITTER_CONSUMER_KEY = 500001;
        public const int NO_TWITTER_CONSUMER_SECRET = 500002;
        public const int INVALID_CONFIGURATION = 500003;
        public const int ERROR_GETTING_REQUEST_TOKEN = 500004;
        public const int ERROR_GETTING_ACCESS_TOKEN = 500005;
        
        public const int FACEBOOK_ERROR = 500010;
        public const int FACEBOOK_SECRET_MISMATCH = 500011;
        public const int FACEBOOK_AUTHENTICATION_ERROR = 500012;
        public const int FACEBOOK_APPLICATION_TOKEN_REQUEST_ERROR = 500013;
        public const int FACEBOOK_BAD_APPLICATION_TOKEN_SIGNATURE = 500014;

        /// <summary>
        /// Client defined value for a timeout detected client-side.
        /// </summary>
        public const int CLIENT_NETWORK_ERROR_TIMEOUT = 90001;
    }
}
