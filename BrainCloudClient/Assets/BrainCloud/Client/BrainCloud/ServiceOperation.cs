// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{
    /**
     * List of all available service operations. The values are mapped to server keys which represent that operation.
     */
    public struct ServiceOperation : System.IEquatable<ServiceOperation>, System.IComparable<ServiceOperation>
    {
        #region brainCloud Service Operations

        public static readonly ServiceOperation Authenticate = "AUTHENTICATE";
        public static readonly ServiceOperation Attach = "ATTACH";
        public static readonly ServiceOperation Merge = "MERGE";
        public static readonly ServiceOperation Detach = "DETACH";
        public static readonly ServiceOperation GetServerVersion = "GET_SERVER_VERSION";
        public static readonly ServiceOperation ResetEmailPassword = "RESET_EMAIL_PASSWORD";
        public static readonly ServiceOperation ResetEmailPasswordWithExpiry = "RESET_EMAIL_PASSWORD_WITH_EXPIRY";
        public static readonly ServiceOperation ResetEmailPasswordAdvanced = "RESET_EMAIL_PASSWORD_ADVANCED";
        public static readonly ServiceOperation ResetEmailPasswordAdvancedWithExpiry = "RESET_EMAIL_PASSWORD_ADVANCED_WITH_EXPIRY";
        public static readonly ServiceOperation ResetUniversalIdPassword = "RESET_UNIVERSAL_ID_PASSWORD";
        public static readonly ServiceOperation ResetUniversalIdPasswordWithExpiry = "RESET_UNIVERSAL_ID_PASSWORD_WITH_EXPIRY";
        public static readonly ServiceOperation ResetUniversalIdPasswordAdvanced = "RESET_UNIVERSAL_ID_PASSWORD_ADVANCED";
        public static readonly ServiceOperation ResetUniversalIdPasswordAdvancedWithExpiry = "RESET_UNIVERSAL_ID_PASSWORD_ADVANCED_WITH_EXPIRY";
        public static readonly ServiceOperation SwitchToChildProfile = "SWITCH_TO_CHILD_PROFILE";
        public static readonly ServiceOperation SwitchToParentProfile = "SWITCH_TO_PARENT_PROFILE";
        public static readonly ServiceOperation DetachParent = "DETACH_PARENT";
        public static readonly ServiceOperation AttachParentWithIdentity = "ATTACH_PARENT_WITH_IDENTITY";
        public static readonly ServiceOperation AttachNonLoginUniversalId = "ATTACH_NONLOGIN_UNIVERSAL";
        public static readonly ServiceOperation UpdateUniversalIdLogin = "UPDATE_UNIVERSAL_LOGIN";
        public static readonly ServiceOperation GetIdentityStatus = "GET_IDENTITY_STATUS";

        public static readonly ServiceOperation AttachBlockChain = "ATTACH_BLOCKCHAIN_IDENTITY";
        public static readonly ServiceOperation DetachBlockChain = "DETACH_BLOCKCHAIN_IDENTITY";
        public static readonly ServiceOperation GetBlockchainItems = "GET_BLOCKCHAIN_ITEMS";
        public static readonly ServiceOperation GetUniqs = "GET_UNIQS";

        public static readonly ServiceOperation Create = "CREATE";
        public static readonly ServiceOperation CreateWithIndexedId = "CREATE_WITH_INDEXED_ID";
        public static readonly ServiceOperation Reset = "RESET";
        public static readonly ServiceOperation Read = "READ";
        public static readonly ServiceOperation ReadSingleton = "READ_SINGLETON";
        public static readonly ServiceOperation ReadByType = "READ_BY_TYPE";
        public static readonly ServiceOperation Verify = "VERIFY";
        public static readonly ServiceOperation ReadShared = "READ_SHARED";
        public static readonly ServiceOperation ReadSharedEntity = "READ_SHARED_ENTITY";
        public static readonly ServiceOperation UpdateEntityIndexedId = "UPDATE_INDEXED_ID";
        public static readonly ServiceOperation UpdateEntityOwnerAndAcl = "UPDATE_ENTITY_OWNER_AND_ACL";
        public static readonly ServiceOperation MakeSystemEntity = "MAKE_SYSTEM_ENTITY";

        // push notification
        public static readonly ServiceOperation DeregisterAll = "DEREGISTER_ALL";
        public static readonly ServiceOperation Deregister = "DEREGISTER";
        public static readonly ServiceOperation Register = "REGISTER";
        public static readonly ServiceOperation SendSimple = "SEND_SIMPLE";
        public static readonly ServiceOperation SendRich = "SEND_RICH";
        public static readonly ServiceOperation SendRaw = "SEND_RAW";
        public static readonly ServiceOperation SendRawBatch = "SEND_RAW_BATCH";
        public static readonly ServiceOperation SendRawToGroup = "SEND_RAW_TO_GROUP";
        public static readonly ServiceOperation SendTemplatedToGroup = "SEND_TEMPLATED_TO_GROUP";
        public static readonly ServiceOperation SendNormalizedToGroup = "SEND_NORMALIZED_TO_GROUP";
        public static readonly ServiceOperation SendNormalized = "SEND_NORMALIZED";
        public static readonly ServiceOperation SendNormalizedBatch = "SEND_NORMALIZED_BATCH";
        public static readonly ServiceOperation ScheduleRichNotification = "SCHEDULE_RICH_NOTIFICATION";
        public static readonly ServiceOperation ScheduleNormalizedNotification = "SCHEDULE_NORMALIZED_NOTIFICATION";

        public static readonly ServiceOperation ScheduleRawNotification = "SCHEDULE_RAW_NOTIFICATION";

        public static readonly ServiceOperation FullReset = "FULL_PLAYER_RESET";
        public static readonly ServiceOperation DataReset = "GAME_DATA_RESET";

        public static readonly ServiceOperation ProcessStatistics = "PROCESS_STATISTICS";
        public static readonly ServiceOperation Update = "UPDATE";
        public static readonly ServiceOperation UpdateShared = "UPDATE_SHARED";
        public static readonly ServiceOperation UpdateAcl = "UPDATE_ACL";
        public static readonly ServiceOperation UpdateTimeToLive = "UPDATE_TIME_TO_LIVE";
        public static readonly ServiceOperation UpdatePartial = "UPDATE_PARTIAL";
        public static readonly ServiceOperation UpdateSingleton = "UPDATE_SINGLETON";
        public static readonly ServiceOperation Delete = "DELETE";
        public static readonly ServiceOperation DeleteSingleton = "DELETE_SINGLETON";
        public static readonly ServiceOperation UpdateSummary = "UPDATE_SUMMARY";
        public static readonly ServiceOperation UpdateSetMinimum = "UPDATE_SET_MINIMUM";
        public static readonly ServiceOperation UpdateIncrementToMaximum = "UPDATE_INCREMENT_TO_MAXIMUM";
        public static readonly ServiceOperation GetFriendProfileInfoForExternalId = "GET_FRIEND_PROFILE_INFO_FOR_EXTERNAL_ID";
        public static readonly ServiceOperation GetProfileInfoForCredential = "GET_PROFILE_INFO_FOR_CREDENTIAL";
        public static readonly ServiceOperation GetProfileInfoForCredentialIfExists = "GET_PROFILE_INFO_FOR_CREDENTIAL_IF_EXISTS";
        public static readonly ServiceOperation GetProfileInfoForExternalAuthId = "GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID";
        public static readonly ServiceOperation GetProfileInfoForExternalAuthIdIfExists = "GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID_IF_EXISTS";
        public static readonly ServiceOperation GetExternalIdForProfileId = "GET_EXTERNAL_ID_FOR_PROFILE_ID";
        public static readonly ServiceOperation FindPlayerByUniversalId = "FIND_PLAYER_BY_UNIVERSAL_ID";
        public static readonly ServiceOperation FindUserByExactUniversalId = "FIND_USER_BY_EXACT_UNIVERSAL_ID";
        public static readonly ServiceOperation FindUsersByNameStartingWith = "FIND_USERS_BY_NAME_STARTING_WITH";
        public static readonly ServiceOperation FindUsersByUniversalIdStartingWith = "FIND_USERS_BY_UNIVERSAL_ID_STARTING_WITH";
        public static readonly ServiceOperation ReadFriends = "READ_FRIENDS";
        public static readonly ServiceOperation ReadFriendEntity = "READ_FRIEND_ENTITY";
        public static readonly ServiceOperation ReadFriendsEntities = "READ_FRIENDS_ENTITIES";
        public static readonly ServiceOperation ReadFriendsWithApplication = "READ_FRIENDS_WITH_APPLICATION";
        public static readonly ServiceOperation ReadFriendPlayerState = "READ_FRIEND_PLAYER_STATE";
        public static readonly ServiceOperation GetSummaryDataForProfileId = "GET_SUMMARY_DATA_FOR_PROFILE_ID";
        public static readonly ServiceOperation FindPlayerByName = "FIND_PLAYER_BY_NAME";
        public static readonly ServiceOperation FindUsersByExactName = "FIND_USERS_BY_EXACT_NAME";
        public static readonly ServiceOperation FindUsersBySubstrName = "FIND_USERS_BY_SUBSTR_NAME";
        public static readonly ServiceOperation ListFriends = "LIST_FRIENDS";
        public static readonly ServiceOperation GetMySocialInfo = "GET_MY_SOCIAL_INFO";
        public static readonly ServiceOperation AddFriends = "ADD_FRIENDS";
        public static readonly ServiceOperation AddFriendsFromPlatform = "ADD_FRIENDS_FROM_PLATFORM";
        public static readonly ServiceOperation RemoveFriends = "REMOVE_FRIENDS";
        public static readonly ServiceOperation GetUsersOnlineStatus = "GET_USERS_ONLINE_STATUS";
        public static readonly ServiceOperation GetSocialLeaderboard = "GET_SOCIAL_LEADERBOARD";
        public static readonly ServiceOperation GetSocialLeaderboardIfExists = "GET_SOCIAL_LEADERBOARD_IF_EXISTS";
        public static readonly ServiceOperation GetSocialLeaderboardByVersion = "GET_SOCIAL_LEADERBOARD_BY_VERSION";
        public static readonly ServiceOperation GetSocialLeaderboardByVersionIfExists = "GET_SOCIAL_LEADERBOARD_BY_VERSION_IF_EXISTS";
        public static readonly ServiceOperation GetMultiSocialLeaderboard = "GET_MULTI_SOCIAL_LEADERBOARD";
        public static readonly ServiceOperation GetGlobalLeaderboard = "GET_GLOBAL_LEADERBOARD";
        public static readonly ServiceOperation GetGlobalLeaderboardPage = "GET_GLOBAL_LEADERBOARD_PAGE";
        public static readonly ServiceOperation GetGlobalLeaderboardPageIfExists = "GET_GLOBAL_LEADERBOARD_PAGE_IF_EXISTS";
        public static readonly ServiceOperation GetGlobalLeaderboardView = "GET_GLOBAL_LEADERBOARD_VIEW";
        public static readonly ServiceOperation GetGlobalLeaderboardViewIfExists = "GET_GLOBAL_LEADERBOARD_VIEW_IF_EXISTS";
        public static readonly ServiceOperation GetGlobalLeaderboardVersions = "GET_GLOBAL_LEADERBOARD_VERSIONS";
        public static readonly ServiceOperation PostScore = "POST_SCORE";
        public static readonly ServiceOperation PostScoreDynamic = "POST_SCORE_DYNAMIC";
        public static readonly ServiceOperation PostScoreDynamicUsingConfig = "POST_SCORE_DYNAMIC_USING_CONFIG";
        public static readonly ServiceOperation PostScoreToDynamicGroupLeaderboard = "POST_GROUP_SCORE_DYNAMIC";
        public static readonly ServiceOperation PostScoreToDynamicGroupLeaderboardUsingConfig = "POST_GROUP_SCORE_DYNAMIC_USING_CONFIG";
        public static readonly ServiceOperation RemovePlayerScore = "REMOVE_PLAYER_SCORE";
        public static readonly ServiceOperation GetCompletedTournament = "GET_COMPLETED_TOURNAMENT";
        public static readonly ServiceOperation RewardTournament = "REWARD_TOURNAMENT";
        public static readonly ServiceOperation GetGroupSocialLeaderboard = "GET_GROUP_SOCIAL_LEADERBOARD";
        public static readonly ServiceOperation GetGroupSocialLeaderboardByVersion = "GET_GROUP_SOCIAL_LEADERBOARD_BY_VERSION";
        public static readonly ServiceOperation GetPlayersSocialLeaderboard = "GET_PLAYERS_SOCIAL_LEADERBOARD";
        public static readonly ServiceOperation GetPlayersSocialLeaderboardIfExists = "GET_PLAYERS_SOCIAL_LEADERBOARD_IF_EXISTS";
        public static readonly ServiceOperation GetPlayersSocialLeaderboardByVersion = "GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION";
        public static readonly ServiceOperation GetPlayersSocialLeaderboardByVersionIfExists = "GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION_IF_EXISTS";
        public static readonly ServiceOperation ListAllLeaderboards = "LIST_ALL_LEADERBOARDS";
        public static readonly ServiceOperation GetGlobalLeaderboardEntryCount = "GET_GLOBAL_LEADERBOARD_ENTRY_COUNT";
        public static readonly ServiceOperation GetPlayerScore = "GET_PLAYER_SCORE";
        public static readonly ServiceOperation GetPlayerScores = "GET_PLAYER_SCORES";
        public static readonly ServiceOperation GetPlayerScoresFromLeaderboards = "GET_PLAYER_SCORES_FROM_LEADERBOARDS";
        public static readonly ServiceOperation PostScoreToGroupLeaderboard = "POST_GROUP_SCORE";
        public static readonly ServiceOperation RemoveGroupScore = "REMOVE_GROUP_SCORE";
        public static readonly ServiceOperation GetGroupLeaderboardView = "GET_GROUP_LEADERBOARD_VIEW";

        public static readonly ServiceOperation ReadFriendsPlayerState = "READ_FRIEND_PLAYER_STATE";

        public static readonly ServiceOperation InitThirdParty = "initThirdParty";
        public static readonly ServiceOperation PostThirdPartyLeaderboardScore = "postThirdPartyLeaderboardScore";
        public static readonly ServiceOperation IncrementThirdPartyLeaderboardScore = "incrementThirdPartyLeaderboardScore";
        public static readonly ServiceOperation LaunchAchievementUI = "launchAchievementUI";
        public static readonly ServiceOperation PostThirdPartyLeaderboardAchievement = "postThirdPartyLeaderboardAchievement";
        public static readonly ServiceOperation IsThirdPartyAchievementComplete = "isThirdPartyAchievementComplete";
        public static readonly ServiceOperation ResetThirdPartyAchievements = "resetThirdPartyAchievements";
        public static readonly ServiceOperation QueryThirdPartyAchievements = "queryThirdPartyAchievements";

        public static readonly ServiceOperation GetInventory = "GET_INVENTORY";
        public static readonly ServiceOperation CashInReceipt = "OP_CASH_IN_RECEIPT";
        public static readonly ServiceOperation AwardVC = "AWARD_VC";
        public static readonly ServiceOperation ConsumePlayerVC = "CONSUME_VC";
        public static readonly ServiceOperation GetPlayerVC = "GET_PLAYER_VC";
        public static readonly ServiceOperation ResetPlayerVC = "RESET_PLAYER_VC";

        public static readonly ServiceOperation AwardParentCurrency = "AWARD_PARENT_VC";
        public static readonly ServiceOperation ConsumeParentCurrency = "CONSUME_PARENT_VC";
        public static readonly ServiceOperation GetParentVC = "GET_PARENT_VC";
        public static readonly ServiceOperation ResetParentCurrency = "RESET_PARENT_VC";

        public static readonly ServiceOperation GetPeerVC = "GET_PEER_VC";
        public static readonly ServiceOperation StartPurchase = "START_PURCHASE";
        public static readonly ServiceOperation FinalizePurchase = "FINALIZE_PURCHASE";
        public static readonly ServiceOperation RefreshPromotions = "REFRESH_PROMOTIONS";

        public static readonly ServiceOperation Send = "SEND";
        public static readonly ServiceOperation SendToProfiles = "SEND_EVENT_TO_PROFILES";
        public static readonly ServiceOperation UpdateEventData = "UPDATE_EVENT_DATA";
        public static readonly ServiceOperation UpdateEventDataIfExists = "UPDATE_EVENT_DATA_IF_EXISTS";
        public static readonly ServiceOperation DeleteSent = "DELETE_SENT";
        public static readonly ServiceOperation DeleteIncoming = "DELETE_INCOMING";
        public static readonly ServiceOperation DeleteIncomingEvents = "DELETE_INCOMING_EVENTS";
        public static readonly ServiceOperation DeleteIncomingEventsOlderThan = "DELETE_INCOMING_EVENTS_OLDER_THAN";
        public static readonly ServiceOperation DeleteIncomingEventsByTypeOlderThan = "DELETE_INCOMING_EVENTS_BY_TYPE_OLDER_THAN";
        public static readonly ServiceOperation GetEvents = "GET_EVENTS";

        public static readonly ServiceOperation UpdateIncrement = "UPDATE_INCREMENT";
        public static readonly ServiceOperation ReadNextXpLevel = "READ_NEXT_XPLEVEL";
        public static readonly ServiceOperation ReadXpLevels = "READ_XP_LEVELS";
        public static readonly ServiceOperation SetXpPoints = "SET_XPPOINTS";
        public static readonly ServiceOperation ReadSubset = "READ_SUBSET";

        //GlobalFile
        public static readonly ServiceOperation GetFileInfo = "GET_FILE_INFO";
        public static readonly ServiceOperation GetFileInfoSimple = "GET_FILE_INFO_SIMPLE";
        public static readonly ServiceOperation GetGlobalCDNUrl = "GET_GLOBAL_CDN_URL";
        public static readonly ServiceOperation GetGlobalFileList = "GET_GLOBAL_FILE_LIST";

        public static readonly ServiceOperation Run = "RUN";
        public static readonly ServiceOperation Tweet = "TWEET";

        public static readonly ServiceOperation AwardAchievements = "AWARD_ACHIEVEMENTS";
        public static readonly ServiceOperation ReadAchievements = "READ_ACHIEVEMENTS";
        public static readonly ServiceOperation ReadAchievedAchievements = "READ_ACHIEVED_ACHIEVEMENTS";

        public static readonly ServiceOperation SetPlayerRating = "SET_PLAYER_RATING";
        public static readonly ServiceOperation ResetPlayerRating = "RESET_PLAYER_RATING";
        public static readonly ServiceOperation IncrementPlayerRating = "INCREMENT_PLAYER_RATING";
        public static readonly ServiceOperation DecrementPlayerRating = "DECREMENT_PLAYER_RATING";
        public static readonly ServiceOperation ShieldOn = "SHIELD_ON";
        public static readonly ServiceOperation ShieldOnFor = "SHIELD_ON_FOR";
        public static readonly ServiceOperation ShieldOff = "SHIELD_OFF";
        public static readonly ServiceOperation IncrementShieldOnFor = "INCREMENT_SHIELD_ON_FOR";
        public static readonly ServiceOperation GetShieldExpiry = "GET_SHIELD_EXPIRY";
        public static readonly ServiceOperation FindPlayers = "FIND_PLAYERS";
        public static readonly ServiceOperation FindPlayersUsingFilter = "FIND_PLAYERS_USING_FILTER";

        public static readonly ServiceOperation SubmitTurn = "SUBMIT_TURN";
        public static readonly ServiceOperation UpdateMatchSummary = "UPDATE_SUMMARY";
        public static readonly ServiceOperation UpdateMatchStateCurrentTurn = "UPDATE_MATCH_STATE_CURRENT_TURN";
        public static readonly ServiceOperation Abandon = "ABANDON";
        public static readonly ServiceOperation AbandonMatchWithSummaryData = "ABANDON_MATCH_WITH_SUMMARY_DATA";
        public static readonly ServiceOperation CompleteMatchWithSummaryData = "COMPLETE_MATCH_WITH_SUMMARY_DATA";
        public static readonly ServiceOperation Complete = "COMPLETE";
        public static readonly ServiceOperation ReadMatch = "READ_MATCH";
        public static readonly ServiceOperation ReadMatchHistory = "READ_MATCH_HISTORY";
        public static readonly ServiceOperation FindMatches = "FIND_MATCHES";
        public static readonly ServiceOperation FindMatchesCompleted = "FIND_MATCHES_COMPLETED";
        public static readonly ServiceOperation DeleteMatch = "DELETE_MATCH";

        public static readonly ServiceOperation LastUploadStatus = "LAST_UPLOAD_STATUS";

        public static readonly ServiceOperation ReadQuests = "READ_QUESTS";
        public static readonly ServiceOperation ReadCompletedQuests = "READ_COMPLETED_QUESTS";
        public static readonly ServiceOperation ReadInProgressQuests = "READ_IN_PROGRESS_QUESTS";
        public static readonly ServiceOperation ReadNotStartedQuests = "READ_NOT_STARTED_QUESTS";
        public static readonly ServiceOperation ReadQuestsWithStatus = "READ_QUESTS_WITH_STATUS";
        public static readonly ServiceOperation ReadQuestsWithBasicPercentage = "READ_QUESTS_WITH_BASIC_PERCENTAGE";
        public static readonly ServiceOperation ReadQuestsWithComplexPercentage = "READ_QUESTS_WITH_COMPLEX_PERCENTAGE";
        public static readonly ServiceOperation ReadQuestsByCategory = "READ_QUESTS_BY_CATEGORY";
        public static readonly ServiceOperation ResetMilestones = "RESET_MILESTONES";

        public static readonly ServiceOperation ReadForCategory = "READ_FOR_CATEGORY";

        public static readonly ServiceOperation ReadMilestones = "READ_MILESTONES";
        public static readonly ServiceOperation ReadMilestonesByCategory = "READ_MILESTONES_BY_CATEGORY";

        public static readonly ServiceOperation ReadCompletedMilestones = "READ_COMPLETED_MILESTONES";
        public static readonly ServiceOperation ReadInProgressMilestones = "READ_IN_PROGRESS_MILESTONES";

        public static readonly ServiceOperation Trigger = "TRIGGER";
        public static readonly ServiceOperation TriggerMultiple = "TRIGGER_MULTIPLE";

        public static readonly ServiceOperation Logout = "LOGOUT";

        public static readonly ServiceOperation StartMatch = "START_MATCH";
        public static readonly ServiceOperation CancelMatch = "CANCEL_MATCH";
        public static readonly ServiceOperation CompleteMatch = "COMPLETE_MATCH";
        public static readonly ServiceOperation EnableMatchMaking = "ENABLE_FOR_MATCH";
        public static readonly ServiceOperation DisableMatchMaking = "DISABLE_FOR_MATCH";
        public static readonly ServiceOperation UpdateName = "UPDATE_NAME";

        public static readonly ServiceOperation StartStream = "START_STREAM";
        public static readonly ServiceOperation ReadStream = "READ_STREAM";
        public static readonly ServiceOperation EndStream = "END_STREAM";
        public static readonly ServiceOperation DeleteStream = "DELETE_STREAM";
        public static readonly ServiceOperation AddEvent = "ADD_EVENT";
        public static readonly ServiceOperation ProtectStreamUntil = "PROTECT_STREAM_UNTIL";

        public static readonly ServiceOperation GetStreamSummariesForInitiatingPlayer = "GET_STREAM_SUMMARIES_FOR_INITIATING_PLAYER";
        public static readonly ServiceOperation GetStreamSummariesForTargetPlayer = "GET_STREAM_SUMMARIES_FOR_TARGET_PLAYER";
        public static readonly ServiceOperation GetRecentStreamsForInitiatingPlayer = "GET_RECENT_STREAMS_FOR_INITIATING_PLAYER";
        public static readonly ServiceOperation GetRecentStreamsForTargetPlayer = "GET_RECENT_STREAMS_FOR_TARGET_PLAYER";

        public static readonly ServiceOperation GetUserInfo = "GET_USER_INFO";
        public static readonly ServiceOperation InitializeTransaction = "INITIALIZE_TRANSACTION";
        public static readonly ServiceOperation FinalizeTransaction = "FINALIZE_TRANSACTION";

        public static readonly ServiceOperation CachePurchasePayloadContext = "CACHE_PURCHASE_PAYLOAD_CONTEXT";
        public static readonly ServiceOperation VerifyPurchase = "VERIFY_PURCHASE";
        public static readonly ServiceOperation StartSteamTransaction = "START_STEAM_TRANSACTION";
        public static readonly ServiceOperation FinalizeSteamTransaction = "FINALIZE_STEAM_TRANSACTION";
        public static readonly ServiceOperation VerifyMicrosoftReceipt = "VERIFY_MICROSOFT_RECEIPT";
        public static readonly ServiceOperation EligiblePromotions = "ELIGIBLE_PROMOTIONS";

        public static readonly ServiceOperation ReadSharedEntitiesList = "READ_SHARED_ENTITIES_LIST";
        public static readonly ServiceOperation GetList = "GET_LIST";
        public static readonly ServiceOperation GetListByIndexedId = "GET_LIST_BY_INDEXED_ID";
        public static readonly ServiceOperation GetListCount = "GET_LIST_COUNT";
        public static readonly ServiceOperation GetPage = "GET_PAGE";
        public static readonly ServiceOperation GetPageOffset = "GET_PAGE_BY_OFFSET";
        public static readonly ServiceOperation IncrementGlobalEntityData = "INCREMENT_GLOBAL_ENTITY_DATA";
        public static readonly ServiceOperation GetRandomEntitiesMatching = "GET_RANDOM_ENTITIES_MATCHING";
        public static readonly ServiceOperation IncrementUserEntityData = "INCREMENT_USER_ENTITY_DATA";
        public static readonly ServiceOperation IncrementSharedUserEntityData = "INCREMENT_SHARED_USER_ENTITY_DATA";

        public static readonly ServiceOperation UpdatePictureUrl = "UPDATE_PICTURE_URL";
        public static readonly ServiceOperation UpdateContactEmail = "UPDATE_CONTACT_EMAIL";
        public static readonly ServiceOperation UpdateLanguageCode = "UPDATE_LANGUAGE_CODE";
        public static readonly ServiceOperation UpdateTimeZoneOffset = "UPDATE_TIMEZONE_OFFSET";
        public static readonly ServiceOperation SetUserStatus = "SET_USER_STATUS";        
        public static readonly ServiceOperation GetUserStatus = "GET_USER_STATUS";
        public static readonly ServiceOperation ClearUserStatus = "CLEAR_USER_STATUS";
        public static readonly ServiceOperation ExtendUserStatus = "EXTEND_USER_STATUS";
        public static readonly ServiceOperation GetAttributes = "GET_ATTRIBUTES";
        public static readonly ServiceOperation UpdateAttributes = "UPDATE_ATTRIBUTES";
        public static readonly ServiceOperation RemoveAttributes = "REMOVE_ATTRIBUTES";
        public static readonly ServiceOperation GetChildProfiles = "GET_CHILD_PROFILES";
        public static readonly ServiceOperation GetIdentities = "GET_IDENTITIES";
        public static readonly ServiceOperation GetExpiredIdentities = "GET_EXPIRED_IDENTITIES";
        public static readonly ServiceOperation RefreshIdentity = "REFRESH_IDENTITY";
        public static readonly ServiceOperation ChangeEmailIdentity = "CHANGE_EMAIL_IDENTITY";

        public static readonly ServiceOperation FbConfirmPurchase = "FB_CONFIRM_PURCHASE";
        public static readonly ServiceOperation GooglePlayConfirmPurchase = "CONFIRM_GOOGLEPLAY_PURCHASE";

        public static readonly ServiceOperation ReadProperties = "READ_PROPERTIES";
        public static readonly ServiceOperation ReadSelectedProperties = "READ_SELECTED_PROPERTIES";
        public static readonly ServiceOperation ReadPropertiesInCategories = "READ_PROPERTIES_IN_CATEGORIES";

        public static readonly ServiceOperation GetUpdatedFiles = "GET_UPDATED_FILES";
        public static readonly ServiceOperation GetFileList = "GET_FILE_LIST";

        public static readonly ServiceOperation ScheduleCloudScript = "SCHEDULE_CLOUD_SCRIPT";
        public static readonly ServiceOperation GetScheduledCloudScripts = "GET_SCHEDULED_CLOUD_SCRIPTS";
        public static readonly ServiceOperation GetRunningOrQueuedCloudScripts = "GET_RUNNING_OR_QUEUED_CLOUD_SCRIPTS";
        public static readonly ServiceOperation RunParentScript = "RUN_PARENT_SCRIPT";
        public static readonly ServiceOperation CancelScheduledScript = "CANCEL_SCHEDULED_SCRIPT";
        public static readonly ServiceOperation RunPeerScript = "RUN_PEER_SCRIPT";
        public static readonly ServiceOperation RunPeerScriptAsync = "RUN_PEER_SCRIPT_ASYNC";

        //RedemptionCode
        public static readonly ServiceOperation GetRedeemedCodes = "GET_REDEEMED_CODES";
        public static readonly ServiceOperation RedeemCode = "REDEEM_CODE";

        //DataStream
        public static readonly ServiceOperation CustomPageEvent = "CUSTOM_PAGE_EVENT";
        public static readonly ServiceOperation CustomScreenEvent = "CUSTOM_SCREEN_EVENT";
        public static readonly ServiceOperation CustomTrackEvent = "CUSTOM_TRACK_EVENT";
        public static readonly ServiceOperation SubmitCrashReport = "SEND_CRASH_REPORT";

        //Profanity
        public static readonly ServiceOperation ProfanityCheck = "PROFANITY_CHECK";
        public static readonly ServiceOperation ProfanityReplaceText = "PROFANITY_REPLACE_TEXT";
        public static readonly ServiceOperation ProfanityIdentifyBadWords = "PROFANITY_IDENTIFY_BAD_WORDS";

        //file upload
        public static readonly ServiceOperation PrepareUserUpload = "PREPARE_USER_UPLOAD";
        public static readonly ServiceOperation ListUserFiles = "LIST_USER_FILES";
        public static readonly ServiceOperation DeleteUserFile = "DELETE_USER_FILE";
        public static readonly ServiceOperation DeleteUserFiles = "DELETE_USER_FILES";
        public static readonly ServiceOperation GetCdnUrl = "GET_CDN_URL";

        //group
        public static readonly ServiceOperation AcceptGroupInvitation = "ACCEPT_GROUP_INVITATION";
        public static readonly ServiceOperation AddGroupMember = "ADD_GROUP_MEMBER";
        public static readonly ServiceOperation ApproveGroupJoinRequest = "APPROVE_GROUP_JOIN_REQUEST";
        public static readonly ServiceOperation AutoJoinGroup = "AUTO_JOIN_GROUP";
        public static readonly ServiceOperation AutoJoinGroupMulti = "AUTO_JOIN_GROUP_MULTI";
        public static readonly ServiceOperation CancelGroupInvitation = "CANCEL_GROUP_INVITATION";
        public static readonly ServiceOperation DeleteGroupJoinRequest = "DELETE_GROUP_JOIN_REQUEST";
        public static readonly ServiceOperation CreateGroup = "CREATE_GROUP";
        public static readonly ServiceOperation CreateGroupEntity = "CREATE_GROUP_ENTITY";
        public static readonly ServiceOperation DeleteGroup = "DELETE_GROUP";
        public static readonly ServiceOperation DeleteGroupEntity = "DELETE_GROUP_ENTITY";
        public static readonly ServiceOperation DeleteGroupMemeber = "DELETE_MEMBER_FROM_GROUP";
        public static readonly ServiceOperation GetMyGroups = "GET_MY_GROUPS";
        public static readonly ServiceOperation IncrementGroupData = "INCREMENT_GROUP_DATA";
        public static readonly ServiceOperation IncrementGroupEntityData = "INCREMENT_GROUP_ENTITY_DATA";
        public static readonly ServiceOperation InviteGroupMember = "INVITE_GROUP_MEMBER";
        public static readonly ServiceOperation JoinGroup = "JOIN_GROUP";
        public static readonly ServiceOperation LeaveGroup = "LEAVE_GROUP";
        public static readonly ServiceOperation ListGroupsPage = "LIST_GROUPS_PAGE";
        public static readonly ServiceOperation ListGroupsPageByOffset = "LIST_GROUPS_PAGE_BY_OFFSET";
        public static readonly ServiceOperation ListGroupsWithMember = "LIST_GROUPS_WITH_MEMBER";
        public static readonly ServiceOperation ReadGroup = "READ_GROUP";
        public static readonly ServiceOperation ReadGroupData = "READ_GROUP_DATA";
        public static readonly ServiceOperation ReadGroupEntitiesPage = "READ_GROUP_ENTITIES_PAGE";
        public static readonly ServiceOperation ReadGroupEntitiesPageByOffset = "READ_GROUP_ENTITIES_PAGE_BY_OFFSET";
        public static readonly ServiceOperation ReadGroupEntity = "READ_GROUP_ENTITY";
        public static readonly ServiceOperation ReadGroupMembers = "READ_GROUP_MEMBERS";
        public static readonly ServiceOperation RejectGroupInvitation = "REJECT_GROUP_INVITATION";
        public static readonly ServiceOperation RejectGroupJoinRequest = "REJECT_GROUP_JOIN_REQUEST";
        public static readonly ServiceOperation RemoveGroupMember = "REMOVE_GROUP_MEMBER";
        public static readonly ServiceOperation UpdateGroupData = "UPDATE_GROUP_DATA";
        public static readonly ServiceOperation UpdateGroupEntity = "UPDATE_GROUP_ENTITY_DATA";
        public static readonly ServiceOperation UpdateGroupMember = "UPDATE_GROUP_MEMBER";
        public static readonly ServiceOperation UpdateGroupACL = "UPDATE_GROUP_ACL";
        public static readonly ServiceOperation UpdateGroupEntityACL = "UPDATE_GROUP_ENTITY_ACL";
        public static readonly ServiceOperation UpdateGroupName = "UPDATE_GROUP_NAME";
        public static readonly ServiceOperation SetGroupOpen = "SET_GROUP_OPEN";
        public static readonly ServiceOperation GetRandomGroupsMatching = "GET_RANDOM_GROUPS_MATCHING";
        public static readonly ServiceOperation UpdateGroupSummaryData = "UPDATE_GROUP_SUMMARY_DATA";

        //mail
        public static readonly ServiceOperation SendBasicEmail = "SEND_BASIC_EMAIL";
        public static readonly ServiceOperation SendAdvancedEmail = "SEND_ADVANCED_EMAIL";
        public static readonly ServiceOperation SendAdvancedEmailByAddress = "SEND_ADVANCED_EMAIL_BY_ADDRESS";
        public static readonly ServiceOperation SendAdvancedEmailByAddresses = "SEND_ADVANCED_EMAIL_BY_ADDRESSES";

        //peer
        public static readonly ServiceOperation AttachPeerProfile = "ATTACH_PEER_PROFILE";
        public static readonly ServiceOperation DetachPeer = "DETACH_PEER";
        public static readonly ServiceOperation GetPeerProfiles = "GET_PEER_PROFILES";

        //presence
        public static readonly ServiceOperation ForcePush = "FORCE_PUSH";
        public static readonly ServiceOperation GetPresenceOfFriends = "GET_PRESENCE_OF_FRIENDS";
        public static readonly ServiceOperation GetPresenceOfGroup = "GET_PRESENCE_OF_GROUP";
        public static readonly ServiceOperation GetPresenceOfUsers = "GET_PRESENCE_OF_USERS";
        public static readonly ServiceOperation RegisterListenersForFriends = "REGISTER_LISTENERS_FOR_FRIENDS";
        public static readonly ServiceOperation RegisterListenersForGroup = "REGISTER_LISTENERS_FOR_GROUP";
        public static readonly ServiceOperation RegisterListenersForProfiles = "REGISTER_LISTENERS_FOR_PROFILES";
        public static readonly ServiceOperation SetVisibility = "SET_VISIBILITY";
        public static readonly ServiceOperation StopListening = "STOP_LISTENING";
        public static readonly ServiceOperation UpdateActivity = "UPDATE_ACTIVITY";

        //tournament
        public static readonly ServiceOperation GetTournamentStatus = "GET_TOURNAMENT_STATUS";
        public static readonly ServiceOperation GetDivisionInfo = "GET_DIVISION_INFO";
        public static readonly ServiceOperation GetMyDivisions = "GET_MY_DIVISIONS";
        public static readonly ServiceOperation JoinDivision= "JOIN_DIVISION";
        public static readonly ServiceOperation JoinTournament = "JOIN_TOURNAMENT";
        public static readonly ServiceOperation LeaveDivisionInstance = "LEAVE_DIVISION_INSTANCE";
        public static readonly ServiceOperation LeaveTournament = "LEAVE_TOURNAMENT";
        public static readonly ServiceOperation PostTournamentScore = "POST_TOURNAMENT_SCORE";
        public static readonly ServiceOperation PostTournamentScoreWithResults = "POST_TOURNAMENT_SCORE_WITH_RESULTS";
        public static readonly ServiceOperation ViewCurrentReward = "VIEW_CURRENT_REWARD";
        public static readonly ServiceOperation ViewReward = "VIEW_REWARD";
        public static readonly ServiceOperation ClaimTournamentReward = "CLAIM_TOURNAMENT_REWARD";

        // rtt
        public static readonly ServiceOperation RequestClientConnection = "REQUEST_CLIENT_CONNECTION";

        // chat
        public static readonly ServiceOperation ChannelConnect = "CHANNEL_CONNECT";
        public static readonly ServiceOperation ChannelDisconnect = "CHANNEL_DISCONNECT";
        public static readonly ServiceOperation DeleteChatMessage = "DELETE_CHAT_MESSAGE";
        public static readonly ServiceOperation GetChannelId = "GET_CHANNEL_ID";
        public static readonly ServiceOperation GetChannelInfo = "GET_CHANNEL_INFO";
        public static readonly ServiceOperation GetChatMessage = "GET_CHAT_MESSAGE";
        public static readonly ServiceOperation GetRecentChatMessages = "GET_RECENT_CHAT_MESSAGES";
        public static readonly ServiceOperation GetSubscribedChannels = "GET_SUBSCRIBED_CHANNELS";
        public static readonly ServiceOperation PostChatMessage = "POST_CHAT_MESSAGE";
        public static readonly ServiceOperation PostChatMessageSimple = "POST_CHAT_MESSAGE_SIMPLE";
        public static readonly ServiceOperation UpdateChatMessage = "UPDATE_CHAT_MESSAGE";

        // messaging 
        public static readonly ServiceOperation DeleteMessages = "DELETE_MESSAGES";
        public static readonly ServiceOperation GetMessageBoxes = "GET_MESSAGE_BOXES";
        public static readonly ServiceOperation GetMessageCounts = "GET_MESSAGE_COUNTS";
        public static readonly ServiceOperation GetMessages = "GET_MESSAGES";
        public static readonly ServiceOperation GetMessagesPage = "GET_MESSAGES_PAGE";
        public static readonly ServiceOperation GetMessagesPageOffset = "GET_MESSAGES_PAGE_OFFSET";
        public static readonly ServiceOperation MarkMessagesRead = "MARK_MESSAGES_READ";
        public static readonly ServiceOperation SendMessage = "SEND_MESSAGE";
        public static readonly ServiceOperation SendMessageSimple = "SEND_MESSAGE_SIMPLE";

        // lobby
        public static readonly ServiceOperation FindLobby = "FIND_LOBBY";
        public static readonly ServiceOperation FindLobbyWithPingData = "FIND_LOBBY_WITH_PING_DATA";
        public static readonly ServiceOperation CreateLobby = "CREATE_LOBBY";
        public static readonly ServiceOperation CreateLobbyWithPingData = "CREATE_LOBBY_WITH_PING_DATA";
        public static readonly ServiceOperation FindOrCreateLobby = "FIND_OR_CREATE_LOBBY";
        public static readonly ServiceOperation FindOrCreateLobbyWithPingData = "FIND_OR_CREATE_LOBBY_WITH_PING_DATA";
        public static readonly ServiceOperation GetLobbyData = "GET_LOBBY_DATA";
        public static readonly ServiceOperation UpdateReady = "UPDATE_READY";
        public static readonly ServiceOperation UpdateSettings = "UPDATE_SETTINGS";
        public static readonly ServiceOperation SwitchTeam = "SWITCH_TEAM";
        public static readonly ServiceOperation SendSignal = "SEND_SIGNAL";
        public static readonly ServiceOperation JoinLobby = "JOIN_LOBBY";
        public static readonly ServiceOperation JoinLobbyWithPingData = "JOIN_LOBBY_WITH_PING_DATA";
        public static readonly ServiceOperation LeaveLobby = "LEAVE_LOBBY";
        public static readonly ServiceOperation RemoveMember = "REMOVE_MEMBER";
        public static readonly ServiceOperation CancelFindRequest = "CANCEL_FIND_REQUEST";
        public static readonly ServiceOperation GetRegionsForLobbies = "GET_REGIONS_FOR_LOBBIES";
        public static readonly ServiceOperation GetLobbyInstances = "GET_LOBBY_INSTANCES";
        public static readonly ServiceOperation GetLobbyInstancesWithPingData = "GET_LOBBY_INSTANCES_WITH_PING_DATA";

        
        //ItemCatalog
        public static readonly ServiceOperation GetCatalogItemDefinition = "GET_CATALOG_ITEM_DEFINITION";
        public static readonly ServiceOperation GetCatalogItemsPage = "GET_CATALOG_ITEMS_PAGE";
        public static readonly ServiceOperation GetCatalogItemsPageOffset = "GET_CATALOG_ITEMS_PAGE_OFFSET";

        //CustomEntity

        public static readonly ServiceOperation DeleteEntities = "DELETE_ENTITIES";
        public static readonly ServiceOperation CreateCustomEntity = "CREATE_ENTITY";
        public static readonly ServiceOperation GetCustomEntityPage = "GET_PAGE";
        public static readonly ServiceOperation GetCustomEntityPageOffset = "GET_ENTITY_PAGE_OFFSET";
        public static readonly ServiceOperation GetEntityPage = "GET_ENTITY_PAGE";
        public static readonly ServiceOperation ReadCustomEntity = "READ_ENTITY";
        public static readonly ServiceOperation IncrementData = "INCREMENT_DATA";
        public static readonly ServiceOperation IncrementSingletonData = "INCREMENT_SINGLETON_DATA";
        public static readonly ServiceOperation UpdateCustomEntity = "UPDATE_ENTITY";
        public static readonly ServiceOperation UpdateCustomEntityFields = "UPDATE_ENTITY_FIELDS";
        public static readonly ServiceOperation UpdateCustomEntityFieldsShards = "UPDATE_ENTITY_FIELDS_SHARDED";
        public static readonly ServiceOperation DeleteCustomEntity = "DELETE_ENTITY";
        public static readonly ServiceOperation GetCount = "GET_COUNT";
        public static readonly ServiceOperation UpdateSingletonFields = "UPDATE_SINGLETON_FIELDS";

        //UserItemsService

        public static readonly ServiceOperation AwardUserItem = "AWARD_USER_ITEM";
        public static readonly ServiceOperation DropUserItem = "DROP_USER_ITEM";
        public static readonly ServiceOperation GetUserItemsPage = "GET_USER_ITEMS_PAGE";
        public static readonly ServiceOperation GetUserItemsPageOffset = "GET_USER_ITEMS_PAGE_OFFSET";
        public static readonly ServiceOperation GetUserItem = "GET_USER_ITEM";
        public static readonly ServiceOperation GiveUserItemTo = "GIVE_USER_ITEM_TO";
        public static readonly ServiceOperation PurchaseUserItem = "PURCHASE_USER_ITEM";
        public static readonly ServiceOperation ReceiveUserItemFrom = "RECEIVE_USER_ITEM_FROM";
        public static readonly ServiceOperation SellUserItem = "SELL_USER_ITEM";
        public static readonly ServiceOperation UpdateUserItemData = "UPDATE_USER_ITEM_DATA";
        public static readonly ServiceOperation UseUserItem = "USE_USER_ITEM";
        public static readonly ServiceOperation PublishUserItemToBlockchain = "PUBLISH_USER_ITEM_TO_BLOCKCHAIN";
        public static readonly ServiceOperation RefreshBlockchainUserItems = "REFRESH_BLOCKCHAIN_USER_ITEMS";
        public static readonly ServiceOperation RemoveUserItemFromBlockchain = "REMOVE_USER_ITEM_FROM_BLOCKCHAIN";

        //Group File Services
        public static readonly ServiceOperation CheckFilenameExists = "CHECK_FILENAME_EXISTS";
        public static readonly ServiceOperation CheckFullpathFilenameExists = "CHECK_FULLPATH_FILENAME_EXISTS";
        public static readonly ServiceOperation CopyFile = "COPY_FILE";
        public static readonly ServiceOperation DeleteFile = "DELETE_FILE";
        public static readonly ServiceOperation MoveFile = "MOVE_FILE";
        public static readonly ServiceOperation MoveUserToGroupFile = "MOVE_USER_TO_GROUP_FILE";
        public static readonly ServiceOperation UpdateFileInfo = "UPDATE_FILE_INFO";

        #endregion

        private ServiceOperation(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            if (obj is not ServiceOperation c)
                return false;

            return Equals(c);
        }

        public bool Equals(ServiceOperation other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public int CompareTo(ServiceOperation other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(ServiceOperation v) => v.Value;

        public static implicit operator ServiceOperation(string v) => new(v);

        public static bool operator ==(ServiceOperation v1, ServiceOperation v2) => v1.Equals(v2);

        public static bool operator !=(ServiceOperation v1, ServiceOperation v2) => !(v1 == v2);

        public static bool operator >(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
