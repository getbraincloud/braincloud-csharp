// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    /**
     * List of all available service operations. The values are mapped to server keys which represent that operation.
     */
    public readonly struct ServiceOperation : System.IEquatable<ServiceOperation>, System.IComparable<ServiceOperation>
    {
        #region brainCloud Service Operations

        public static readonly ServiceOperation Authenticate = new("AUTHENTICATE");
        public static readonly ServiceOperation Attach = new("ATTACH");
        public static readonly ServiceOperation Merge = new("MERGE");
        public static readonly ServiceOperation Detach = new("DETACH");
        public static readonly ServiceOperation GetServerVersion = new("GET_SERVER_VERSION");
        public static readonly ServiceOperation ResetEmailPassword = new("RESET_EMAIL_PASSWORD");
        public static readonly ServiceOperation ResetEmailPasswordWithExpiry = new("RESET_EMAIL_PASSWORD_WITH_EXPIRY");
        public static readonly ServiceOperation ResetEmailPasswordAdvanced = new("RESET_EMAIL_PASSWORD_ADVANCED");
        public static readonly ServiceOperation ResetEmailPasswordAdvancedWithExpiry = new("RESET_EMAIL_PASSWORD_ADVANCED_WITH_EXPIRY");
        public static readonly ServiceOperation ResetUniversalIdPassword = new("RESET_UNIVERSAL_ID_PASSWORD");
        public static readonly ServiceOperation ResetUniversalIdPasswordWithExpiry = new("RESET_UNIVERSAL_ID_PASSWORD_WITH_EXPIRY");
        public static readonly ServiceOperation ResetUniversalIdPasswordAdvanced = new("RESET_UNIVERSAL_ID_PASSWORD_ADVANCED");
        public static readonly ServiceOperation ResetUniversalIdPasswordAdvancedWithExpiry = new("RESET_UNIVERSAL_ID_PASSWORD_ADVANCED_WITH_EXPIRY");
        public static readonly ServiceOperation SwitchToChildProfile = new("SWITCH_TO_CHILD_PROFILE");
        public static readonly ServiceOperation SwitchToParentProfile = new("SWITCH_TO_PARENT_PROFILE");
        public static readonly ServiceOperation DetachParent = new("DETACH_PARENT");
        public static readonly ServiceOperation AttachParentWithIdentity = new("ATTACH_PARENT_WITH_IDENTITY");
        public static readonly ServiceOperation AttachNonLoginUniversalId = new("ATTACH_NONLOGIN_UNIVERSAL");
        public static readonly ServiceOperation UpdateUniversalIdLogin = new("UPDATE_UNIVERSAL_LOGIN");
        public static readonly ServiceOperation GetIdentityStatus = new("GET_IDENTITY_STATUS");

        public static readonly ServiceOperation AttachBlockChain = new("ATTACH_BLOCKCHAIN_IDENTITY");
        public static readonly ServiceOperation DetachBlockChain = new("DETACH_BLOCKCHAIN_IDENTITY");
        public static readonly ServiceOperation GetBlockchainItems = new("GET_BLOCKCHAIN_ITEMS");
        public static readonly ServiceOperation GetUniqs = new("GET_UNIQS");

        public static readonly ServiceOperation Create = new("CREATE");
        public static readonly ServiceOperation CreateWithIndexedId = new("CREATE_WITH_INDEXED_ID");
        public static readonly ServiceOperation Reset = new("RESET");
        public static readonly ServiceOperation Read = new("READ");
        public static readonly ServiceOperation ReadSingleton = new("READ_SINGLETON");
        public static readonly ServiceOperation ReadByType = new("READ_BY_TYPE");
        public static readonly ServiceOperation Verify = new("VERIFY");
        public static readonly ServiceOperation ReadShared = new("READ_SHARED");
        public static readonly ServiceOperation ReadSharedEntity = new("READ_SHARED_ENTITY");
        public static readonly ServiceOperation UpdateEntityIndexedId = new("UPDATE_INDEXED_ID");
        public static readonly ServiceOperation UpdateEntityOwnerAndAcl = new("UPDATE_ENTITY_OWNER_AND_ACL");
        public static readonly ServiceOperation MakeSystemEntity = new("MAKE_SYSTEM_ENTITY");

        // push notification
        public static readonly ServiceOperation DeregisterAll = new("DEREGISTER_ALL");
        public static readonly ServiceOperation Deregister = new("DEREGISTER");
        public static readonly ServiceOperation Register = new("REGISTER");
        public static readonly ServiceOperation SendSimple = new("SEND_SIMPLE");
        public static readonly ServiceOperation SendRich = new("SEND_RICH");
        public static readonly ServiceOperation SendRaw = new("SEND_RAW");
        public static readonly ServiceOperation SendRawBatch = new("SEND_RAW_BATCH");
        public static readonly ServiceOperation SendRawToGroup = new("SEND_RAW_TO_GROUP");
        public static readonly ServiceOperation SendTemplatedToGroup = new("SEND_TEMPLATED_TO_GROUP");
        public static readonly ServiceOperation SendNormalizedToGroup = new("SEND_NORMALIZED_TO_GROUP");
        public static readonly ServiceOperation SendNormalized = new("SEND_NORMALIZED");
        public static readonly ServiceOperation SendNormalizedBatch = new("SEND_NORMALIZED_BATCH");
        public static readonly ServiceOperation ScheduleRichNotification = new("SCHEDULE_RICH_NOTIFICATION");
        public static readonly ServiceOperation ScheduleNormalizedNotification = new("SCHEDULE_NORMALIZED_NOTIFICATION");

        public static readonly ServiceOperation ScheduleRawNotification = new("SCHEDULE_RAW_NOTIFICATION");

        public static readonly ServiceOperation FullReset = new("FULL_PLAYER_RESET");
        public static readonly ServiceOperation DataReset = new("GAME_DATA_RESET");

        public static readonly ServiceOperation ProcessStatistics = new("PROCESS_STATISTICS");
        public static readonly ServiceOperation Update = new("UPDATE");
        public static readonly ServiceOperation UpdateShared = new("UPDATE_SHARED");
        public static readonly ServiceOperation UpdateAcl = new("UPDATE_ACL");
        public static readonly ServiceOperation UpdateTimeToLive = new("UPDATE_TIME_TO_LIVE");
        public static readonly ServiceOperation UpdatePartial = new("UPDATE_PARTIAL");
        public static readonly ServiceOperation UpdateSingleton = new("UPDATE_SINGLETON");
        public static readonly ServiceOperation Delete = new("DELETE");
        public static readonly ServiceOperation DeleteSingleton = new("DELETE_SINGLETON");
        public static readonly ServiceOperation UpdateSummary = new("UPDATE_SUMMARY");
        public static readonly ServiceOperation UpdateSetMinimum = new("UPDATE_SET_MINIMUM");
        public static readonly ServiceOperation UpdateIncrementToMaximum = new("UPDATE_INCREMENT_TO_MAXIMUM");
        public static readonly ServiceOperation GetFriendProfileInfoForExternalId = new("GET_FRIEND_PROFILE_INFO_FOR_EXTERNAL_ID");
        public static readonly ServiceOperation GetProfileInfoForCredential = new("GET_PROFILE_INFO_FOR_CREDENTIAL");
        public static readonly ServiceOperation GetProfileInfoForCredentialIfExists = new("GET_PROFILE_INFO_FOR_CREDENTIAL_IF_EXISTS");
        public static readonly ServiceOperation GetProfileInfoForExternalAuthId = new("GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID");
        public static readonly ServiceOperation GetProfileInfoForExternalAuthIdIfExists = new("GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID_IF_EXISTS");
        public static readonly ServiceOperation GetExternalIdForProfileId = new("GET_EXTERNAL_ID_FOR_PROFILE_ID");
        public static readonly ServiceOperation FindPlayerByUniversalId = new("FIND_PLAYER_BY_UNIVERSAL_ID");
        public static readonly ServiceOperation FindUserByExactUniversalId = new("FIND_USER_BY_EXACT_UNIVERSAL_ID");
        public static readonly ServiceOperation FindUsersByNameStartingWith = new("FIND_USERS_BY_NAME_STARTING_WITH");
        public static readonly ServiceOperation FindUsersByUniversalIdStartingWith = new("FIND_USERS_BY_UNIVERSAL_ID_STARTING_WITH");
        public static readonly ServiceOperation ReadFriends = new("READ_FRIENDS");
        public static readonly ServiceOperation ReadFriendEntity = new("READ_FRIEND_ENTITY");
        public static readonly ServiceOperation ReadFriendsEntities = new("READ_FRIENDS_ENTITIES");
        public static readonly ServiceOperation ReadFriendsWithApplication = new("READ_FRIENDS_WITH_APPLICATION");
        public static readonly ServiceOperation ReadFriendPlayerState = new("READ_FRIEND_PLAYER_STATE");
        public static readonly ServiceOperation GetSummaryDataForProfileId = new("GET_SUMMARY_DATA_FOR_PROFILE_ID");
        public static readonly ServiceOperation FindPlayerByName = new("FIND_PLAYER_BY_NAME");
        public static readonly ServiceOperation FindUsersByExactName = new("FIND_USERS_BY_EXACT_NAME");
        public static readonly ServiceOperation FindUsersBySubstrName = new("FIND_USERS_BY_SUBSTR_NAME");
        public static readonly ServiceOperation ListFriends = new("LIST_FRIENDS");
        public static readonly ServiceOperation GetMySocialInfo = new("GET_MY_SOCIAL_INFO");
        public static readonly ServiceOperation AddFriends = new("ADD_FRIENDS");
        public static readonly ServiceOperation AddFriendsFromPlatform = new("ADD_FRIENDS_FROM_PLATFORM");
        public static readonly ServiceOperation RemoveFriends = new("REMOVE_FRIENDS");
        public static readonly ServiceOperation GetUsersOnlineStatus = new("GET_USERS_ONLINE_STATUS");
        public static readonly ServiceOperation GetSocialLeaderboard = new("GET_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetSocialLeaderboardIfExists = new("GET_SOCIAL_LEADERBOARD_IF_EXISTS");
        public static readonly ServiceOperation GetSocialLeaderboardByVersion = new("GET_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation GetSocialLeaderboardByVersionIfExists = new("GET_SOCIAL_LEADERBOARD_BY_VERSION_IF_EXISTS");
        public static readonly ServiceOperation GetMultiSocialLeaderboard = new("GET_MULTI_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetGlobalLeaderboard = new("GET_GLOBAL_LEADERBOARD");
        public static readonly ServiceOperation GetGlobalLeaderboardPage = new("GET_GLOBAL_LEADERBOARD_PAGE");
        public static readonly ServiceOperation GetGlobalLeaderboardPageIfExists = new("GET_GLOBAL_LEADERBOARD_PAGE_IF_EXISTS");
        public static readonly ServiceOperation GetGlobalLeaderboardView = new("GET_GLOBAL_LEADERBOARD_VIEW");
        public static readonly ServiceOperation GetGlobalLeaderboardViewIfExists = new("GET_GLOBAL_LEADERBOARD_VIEW_IF_EXISTS");
        public static readonly ServiceOperation GetGlobalLeaderboardVersions = new("GET_GLOBAL_LEADERBOARD_VERSIONS");
        public static readonly ServiceOperation PostScore = new("POST_SCORE");
        public static readonly ServiceOperation PostScoreDynamic = new("POST_SCORE_DYNAMIC");
        public static readonly ServiceOperation PostScoreDynamicUsingConfig = new("POST_SCORE_DYNAMIC_USING_CONFIG");
        public static readonly ServiceOperation PostScoreToDynamicGroupLeaderboard = new("POST_GROUP_SCORE_DYNAMIC");
        public static readonly ServiceOperation PostScoreToDynamicGroupLeaderboardUsingConfig = new("POST_GROUP_SCORE_DYNAMIC_USING_CONFIG");
        public static readonly ServiceOperation RemovePlayerScore = new("REMOVE_PLAYER_SCORE");
        public static readonly ServiceOperation GetCompletedTournament = new("GET_COMPLETED_TOURNAMENT");
        public static readonly ServiceOperation RewardTournament = new("REWARD_TOURNAMENT");
        public static readonly ServiceOperation GetGroupSocialLeaderboard = new("GET_GROUP_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetGroupSocialLeaderboardByVersion = new("GET_GROUP_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation GetPlayersSocialLeaderboard = new("GET_PLAYERS_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetPlayersSocialLeaderboardIfExists = new("GET_PLAYERS_SOCIAL_LEADERBOARD_IF_EXISTS");
        public static readonly ServiceOperation GetPlayersSocialLeaderboardByVersion = new("GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation GetPlayersSocialLeaderboardByVersionIfExists = new("GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION_IF_EXISTS");
        public static readonly ServiceOperation ListAllLeaderboards = new("LIST_ALL_LEADERBOARDS");
        public static readonly ServiceOperation GetGlobalLeaderboardEntryCount = new("GET_GLOBAL_LEADERBOARD_ENTRY_COUNT");
        public static readonly ServiceOperation GetPlayerScore = new("GET_PLAYER_SCORE");
        public static readonly ServiceOperation GetPlayerScores = new("GET_PLAYER_SCORES");
        public static readonly ServiceOperation GetPlayerScoresFromLeaderboards = new("GET_PLAYER_SCORES_FROM_LEADERBOARDS");
        public static readonly ServiceOperation PostScoreToGroupLeaderboard = new("POST_GROUP_SCORE");
        public static readonly ServiceOperation RemoveGroupScore = new("REMOVE_GROUP_SCORE");
        public static readonly ServiceOperation GetGroupLeaderboardView = new("GET_GROUP_LEADERBOARD_VIEW");

        public static readonly ServiceOperation ReadFriendsPlayerState = new("READ_FRIEND_PLAYER_STATE");

        public static readonly ServiceOperation InitThirdParty = new("initThirdParty");
        public static readonly ServiceOperation PostThirdPartyLeaderboardScore = new("postThirdPartyLeaderboardScore");
        public static readonly ServiceOperation IncrementThirdPartyLeaderboardScore = new("incrementThirdPartyLeaderboardScore");
        public static readonly ServiceOperation LaunchAchievementUI = new("launchAchievementUI");
        public static readonly ServiceOperation PostThirdPartyLeaderboardAchievement = new("postThirdPartyLeaderboardAchievement");
        public static readonly ServiceOperation IsThirdPartyAchievementComplete = new("isThirdPartyAchievementComplete");
        public static readonly ServiceOperation ResetThirdPartyAchievements = new("resetThirdPartyAchievements");
        public static readonly ServiceOperation QueryThirdPartyAchievements = new("queryThirdPartyAchievements");

        public static readonly ServiceOperation GetInventory = new("GET_INVENTORY");
        public static readonly ServiceOperation CashInReceipt = new("OP_CASH_IN_RECEIPT");
        public static readonly ServiceOperation AwardVC = new("AWARD_VC");
        public static readonly ServiceOperation ConsumePlayerVC = new("CONSUME_VC");
        public static readonly ServiceOperation GetPlayerVC = new("GET_PLAYER_VC");
        public static readonly ServiceOperation ResetPlayerVC = new("RESET_PLAYER_VC");

        public static readonly ServiceOperation AwardParentCurrency = new("AWARD_PARENT_VC");
        public static readonly ServiceOperation ConsumeParentCurrency = new("CONSUME_PARENT_VC");
        public static readonly ServiceOperation GetParentVC = new("GET_PARENT_VC");
        public static readonly ServiceOperation ResetParentCurrency = new("RESET_PARENT_VC");

        public static readonly ServiceOperation GetPeerVC = new("GET_PEER_VC");
        public static readonly ServiceOperation StartPurchase = new("START_PURCHASE");
        public static readonly ServiceOperation FinalizePurchase = new("FINALIZE_PURCHASE");
        public static readonly ServiceOperation RefreshPromotions = new("REFRESH_PROMOTIONS");

        public static readonly ServiceOperation Send = new("SEND");
        public static readonly ServiceOperation SendToProfiles = new("SEND_EVENT_TO_PROFILES");
        public static readonly ServiceOperation UpdateEventData = new("UPDATE_EVENT_DATA");
        public static readonly ServiceOperation UpdateEventDataIfExists = new("UPDATE_EVENT_DATA_IF_EXISTS");
        public static readonly ServiceOperation DeleteSent = new("DELETE_SENT");
        public static readonly ServiceOperation DeleteIncoming = new("DELETE_INCOMING");
        public static readonly ServiceOperation DeleteIncomingEvents = new("DELETE_INCOMING_EVENTS");
        public static readonly ServiceOperation DeleteIncomingEventsOlderThan = new("DELETE_INCOMING_EVENTS_OLDER_THAN");
        public static readonly ServiceOperation DeleteIncomingEventsByTypeOlderThan = new("DELETE_INCOMING_EVENTS_BY_TYPE_OLDER_THAN");
        public static readonly ServiceOperation GetEvents = new("GET_EVENTS");

        public static readonly ServiceOperation UpdateIncrement = new("UPDATE_INCREMENT");
        public static readonly ServiceOperation ReadNextXpLevel = new("READ_NEXT_XPLEVEL");
        public static readonly ServiceOperation ReadXpLevels = new("READ_XP_LEVELS");
        public static readonly ServiceOperation SetXpPoints = new("SET_XPPOINTS");
        public static readonly ServiceOperation ReadSubset = new("READ_SUBSET");

        //GlobalFile
        public static readonly ServiceOperation GetFileInfo = new("GET_FILE_INFO");
        public static readonly ServiceOperation GetFileInfoSimple = new("GET_FILE_INFO_SIMPLE");
        public static readonly ServiceOperation GetGlobalCDNUrl = new("GET_GLOBAL_CDN_URL");
        public static readonly ServiceOperation GetGlobalFileList = new("GET_GLOBAL_FILE_LIST");

        public static readonly ServiceOperation Run = new("RUN");
        public static readonly ServiceOperation Tweet = new("TWEET");

        public static readonly ServiceOperation AwardAchievements = new("AWARD_ACHIEVEMENTS");
        public static readonly ServiceOperation ReadAchievements = new("READ_ACHIEVEMENTS");
        public static readonly ServiceOperation ReadAchievedAchievements = new("READ_ACHIEVED_ACHIEVEMENTS");

        public static readonly ServiceOperation SetPlayerRating = new("SET_PLAYER_RATING");
        public static readonly ServiceOperation ResetPlayerRating = new("RESET_PLAYER_RATING");
        public static readonly ServiceOperation IncrementPlayerRating = new("INCREMENT_PLAYER_RATING");
        public static readonly ServiceOperation DecrementPlayerRating = new("DECREMENT_PLAYER_RATING");
        public static readonly ServiceOperation ShieldOn = new("SHIELD_ON");
        public static readonly ServiceOperation ShieldOnFor = new("SHIELD_ON_FOR");
        public static readonly ServiceOperation ShieldOff = new("SHIELD_OFF");
        public static readonly ServiceOperation IncrementShieldOnFor = new("INCREMENT_SHIELD_ON_FOR");
        public static readonly ServiceOperation GetShieldExpiry = new("GET_SHIELD_EXPIRY");
        public static readonly ServiceOperation FindPlayers = new("FIND_PLAYERS");
        public static readonly ServiceOperation FindPlayersUsingFilter = new("FIND_PLAYERS_USING_FILTER");

        public static readonly ServiceOperation SubmitTurn = new("SUBMIT_TURN");
        public static readonly ServiceOperation UpdateMatchSummary = new("UPDATE_SUMMARY");
        public static readonly ServiceOperation UpdateMatchStateCurrentTurn = new("UPDATE_MATCH_STATE_CURRENT_TURN");
        public static readonly ServiceOperation Abandon = new("ABANDON");
        public static readonly ServiceOperation AbandonMatchWithSummaryData = new("ABANDON_MATCH_WITH_SUMMARY_DATA");
        public static readonly ServiceOperation CompleteMatchWithSummaryData = new("COMPLETE_MATCH_WITH_SUMMARY_DATA");
        public static readonly ServiceOperation Complete = new("COMPLETE");
        public static readonly ServiceOperation ReadMatch = new("READ_MATCH");
        public static readonly ServiceOperation ReadMatchHistory = new("READ_MATCH_HISTORY");
        public static readonly ServiceOperation FindMatches = new("FIND_MATCHES");
        public static readonly ServiceOperation FindMatchesCompleted = new("FIND_MATCHES_COMPLETED");
        public static readonly ServiceOperation DeleteMatch = new("DELETE_MATCH");

        public static readonly ServiceOperation LastUploadStatus = new("LAST_UPLOAD_STATUS");

        public static readonly ServiceOperation ReadQuests = new("READ_QUESTS");
        public static readonly ServiceOperation ReadCompletedQuests = new("READ_COMPLETED_QUESTS");
        public static readonly ServiceOperation ReadInProgressQuests = new("READ_IN_PROGRESS_QUESTS");
        public static readonly ServiceOperation ReadNotStartedQuests = new("READ_NOT_STARTED_QUESTS");
        public static readonly ServiceOperation ReadQuestsWithStatus = new("READ_QUESTS_WITH_STATUS");
        public static readonly ServiceOperation ReadQuestsWithBasicPercentage = new("READ_QUESTS_WITH_BASIC_PERCENTAGE");
        public static readonly ServiceOperation ReadQuestsWithComplexPercentage = new("READ_QUESTS_WITH_COMPLEX_PERCENTAGE");
        public static readonly ServiceOperation ReadQuestsByCategory = new("READ_QUESTS_BY_CATEGORY");
        public static readonly ServiceOperation ResetMilestones = new("RESET_MILESTONES");

        public static readonly ServiceOperation ReadForCategory = new("READ_FOR_CATEGORY");

        public static readonly ServiceOperation ReadMilestones = new("READ_MILESTONES");
        public static readonly ServiceOperation ReadMilestonesByCategory = new("READ_MILESTONES_BY_CATEGORY");

        public static readonly ServiceOperation ReadCompletedMilestones = new("READ_COMPLETED_MILESTONES");
        public static readonly ServiceOperation ReadInProgressMilestones = new("READ_IN_PROGRESS_MILESTONES");

        public static readonly ServiceOperation Trigger = new("TRIGGER");
        public static readonly ServiceOperation TriggerMultiple = new("TRIGGER_MULTIPLE");

        public static readonly ServiceOperation Logout = new("LOGOUT");

        public static readonly ServiceOperation StartMatch = new("START_MATCH");
        public static readonly ServiceOperation CancelMatch = new("CANCEL_MATCH");
        public static readonly ServiceOperation CompleteMatch = new("COMPLETE_MATCH");
        public static readonly ServiceOperation EnableMatchMaking = new("ENABLE_FOR_MATCH");
        public static readonly ServiceOperation DisableMatchMaking = new("DISABLE_FOR_MATCH");
        public static readonly ServiceOperation UpdateName = new("UPDATE_NAME");

        public static readonly ServiceOperation StartStream = new("START_STREAM");
        public static readonly ServiceOperation ReadStream = new("READ_STREAM");
        public static readonly ServiceOperation EndStream = new("END_STREAM");
        public static readonly ServiceOperation DeleteStream = new("DELETE_STREAM");
        public static readonly ServiceOperation AddEvent = new("ADD_EVENT");
        public static readonly ServiceOperation ProtectStreamUntil = new("PROTECT_STREAM_UNTIL");

        public static readonly ServiceOperation GetStreamSummariesForInitiatingPlayer = new("GET_STREAM_SUMMARIES_FOR_INITIATING_PLAYER");
        public static readonly ServiceOperation GetStreamSummariesForTargetPlayer = new("GET_STREAM_SUMMARIES_FOR_TARGET_PLAYER");
        public static readonly ServiceOperation GetRecentStreamsForInitiatingPlayer = new("GET_RECENT_STREAMS_FOR_INITIATING_PLAYER");
        public static readonly ServiceOperation GetRecentStreamsForTargetPlayer = new("GET_RECENT_STREAMS_FOR_TARGET_PLAYER");

        public static readonly ServiceOperation GetUserInfo = new("GET_USER_INFO");
        public static readonly ServiceOperation InitializeTransaction = new("INITIALIZE_TRANSACTION");
        public static readonly ServiceOperation FinalizeTransaction = new("FINALIZE_TRANSACTION");

        public static readonly ServiceOperation CachePurchasePayloadContext = new("CACHE_PURCHASE_PAYLOAD_CONTEXT");
        public static readonly ServiceOperation VerifyPurchase = new("VERIFY_PURCHASE");
        public static readonly ServiceOperation StartSteamTransaction = new("START_STEAM_TRANSACTION");
        public static readonly ServiceOperation FinalizeSteamTransaction = new("FINALIZE_STEAM_TRANSACTION");
        public static readonly ServiceOperation VerifyMicrosoftReceipt = new("VERIFY_MICROSOFT_RECEIPT");
        public static readonly ServiceOperation EligiblePromotions = new("ELIGIBLE_PROMOTIONS");

        public static readonly ServiceOperation ReadSharedEntitiesList = new("READ_SHARED_ENTITIES_LIST");
        public static readonly ServiceOperation GetList = new("GET_LIST");
        public static readonly ServiceOperation GetListByIndexedId = new("GET_LIST_BY_INDEXED_ID");
        public static readonly ServiceOperation GetListCount = new("GET_LIST_COUNT");
        public static readonly ServiceOperation GetPage = new("GET_PAGE");
        public static readonly ServiceOperation GetPageOffset = new("GET_PAGE_BY_OFFSET");
        public static readonly ServiceOperation IncrementGlobalEntityData = new("INCREMENT_GLOBAL_ENTITY_DATA");
        public static readonly ServiceOperation GetRandomEntitiesMatching = new("GET_RANDOM_ENTITIES_MATCHING");
        public static readonly ServiceOperation IncrementUserEntityData = new("INCREMENT_USER_ENTITY_DATA");
        public static readonly ServiceOperation IncrementSharedUserEntityData = new("INCREMENT_SHARED_USER_ENTITY_DATA");

        public static readonly ServiceOperation UpdatePictureUrl = new("UPDATE_PICTURE_URL");
        public static readonly ServiceOperation UpdateContactEmail = new("UPDATE_CONTACT_EMAIL");
        public static readonly ServiceOperation UpdateLanguageCode = new("UPDATE_LANGUAGE_CODE");
        public static readonly ServiceOperation UpdateTimeZoneOffset = new("UPDATE_TIMEZONE_OFFSET");
        public static readonly ServiceOperation SetUserStatus = new("SET_USER_STATUS");        
        public static readonly ServiceOperation GetUserStatus = new("GET_USER_STATUS");
        public static readonly ServiceOperation ClearUserStatus = new("CLEAR_USER_STATUS");
        public static readonly ServiceOperation ExtendUserStatus = new("EXTEND_USER_STATUS");
        public static readonly ServiceOperation GetAttributes = new("GET_ATTRIBUTES");
        public static readonly ServiceOperation UpdateAttributes = new("UPDATE_ATTRIBUTES");
        public static readonly ServiceOperation RemoveAttributes = new("REMOVE_ATTRIBUTES");
        public static readonly ServiceOperation GetChildProfiles = new("GET_CHILD_PROFILES");
        public static readonly ServiceOperation GetIdentities = new("GET_IDENTITIES");
        public static readonly ServiceOperation GetExpiredIdentities = new("GET_EXPIRED_IDENTITIES");
        public static readonly ServiceOperation RefreshIdentity = new("REFRESH_IDENTITY");
        public static readonly ServiceOperation ChangeEmailIdentity = new("CHANGE_EMAIL_IDENTITY");

        public static readonly ServiceOperation FbConfirmPurchase = new("FB_CONFIRM_PURCHASE");
        public static readonly ServiceOperation GooglePlayConfirmPurchase = new("CONFIRM_GOOGLEPLAY_PURCHASE");

        public static readonly ServiceOperation ReadProperties = new("READ_PROPERTIES");
        public static readonly ServiceOperation ReadSelectedProperties = new("READ_SELECTED_PROPERTIES");
        public static readonly ServiceOperation ReadPropertiesInCategories = new("READ_PROPERTIES_IN_CATEGORIES");

        public static readonly ServiceOperation GetUpdatedFiles = new("GET_UPDATED_FILES");
        public static readonly ServiceOperation GetFileList = new("GET_FILE_LIST");

        public static readonly ServiceOperation ScheduleCloudScript = new("SCHEDULE_CLOUD_SCRIPT");
        public static readonly ServiceOperation GetScheduledCloudScripts = new("GET_SCHEDULED_CLOUD_SCRIPTS");
        public static readonly ServiceOperation GetRunningOrQueuedCloudScripts = new("GET_RUNNING_OR_QUEUED_CLOUD_SCRIPTS");
        public static readonly ServiceOperation RunParentScript = new("RUN_PARENT_SCRIPT");
        public static readonly ServiceOperation CancelScheduledScript = new("CANCEL_SCHEDULED_SCRIPT");
        public static readonly ServiceOperation RunPeerScript = new("RUN_PEER_SCRIPT");
        public static readonly ServiceOperation RunPeerScriptAsync = new("RUN_PEER_SCRIPT_ASYNC");

        //RedemptionCode
        public static readonly ServiceOperation GetRedeemedCodes = new("GET_REDEEMED_CODES");
        public static readonly ServiceOperation RedeemCode = new("REDEEM_CODE");

        //DataStream
        public static readonly ServiceOperation CustomPageEvent = new("CUSTOM_PAGE_EVENT");
        public static readonly ServiceOperation CustomScreenEvent = new("CUSTOM_SCREEN_EVENT");
        public static readonly ServiceOperation CustomTrackEvent = new("CUSTOM_TRACK_EVENT");
        public static readonly ServiceOperation SubmitCrashReport = new("SEND_CRASH_REPORT");

        //Profanity
        public static readonly ServiceOperation ProfanityCheck = new("PROFANITY_CHECK");
        public static readonly ServiceOperation ProfanityReplaceText = new("PROFANITY_REPLACE_TEXT");
        public static readonly ServiceOperation ProfanityIdentifyBadWords = new("PROFANITY_IDENTIFY_BAD_WORDS");

        //file upload
        public static readonly ServiceOperation PrepareUserUpload = new("PREPARE_USER_UPLOAD");
        public static readonly ServiceOperation ListUserFiles = new("LIST_USER_FILES");
        public static readonly ServiceOperation DeleteUserFile = new("DELETE_USER_FILE");
        public static readonly ServiceOperation DeleteUserFiles = new("DELETE_USER_FILES");
        public static readonly ServiceOperation GetCdnUrl = new("GET_CDN_URL");

        //group
        public static readonly ServiceOperation AcceptGroupInvitation = new("ACCEPT_GROUP_INVITATION");
        public static readonly ServiceOperation AddGroupMember = new("ADD_GROUP_MEMBER");
        public static readonly ServiceOperation ApproveGroupJoinRequest = new("APPROVE_GROUP_JOIN_REQUEST");
        public static readonly ServiceOperation AutoJoinGroup = new("AUTO_JOIN_GROUP");
        public static readonly ServiceOperation AutoJoinGroupMulti = new("AUTO_JOIN_GROUP_MULTI");
        public static readonly ServiceOperation CancelGroupInvitation = new("CANCEL_GROUP_INVITATION");
        public static readonly ServiceOperation DeleteGroupJoinRequest = new("DELETE_GROUP_JOIN_REQUEST");
        public static readonly ServiceOperation CreateGroup = new("CREATE_GROUP");
        public static readonly ServiceOperation CreateGroupEntity = new("CREATE_GROUP_ENTITY");
        public static readonly ServiceOperation DeleteGroup = new("DELETE_GROUP");
        public static readonly ServiceOperation DeleteGroupEntity = new("DELETE_GROUP_ENTITY");
        public static readonly ServiceOperation DeleteGroupMemeber = new("DELETE_MEMBER_FROM_GROUP");
        public static readonly ServiceOperation GetMyGroups = new("GET_MY_GROUPS");
        public static readonly ServiceOperation IncrementGroupData = new("INCREMENT_GROUP_DATA");
        public static readonly ServiceOperation IncrementGroupEntityData = new("INCREMENT_GROUP_ENTITY_DATA");
        public static readonly ServiceOperation InviteGroupMember = new("INVITE_GROUP_MEMBER");
        public static readonly ServiceOperation JoinGroup = new("JOIN_GROUP");
        public static readonly ServiceOperation LeaveGroup = new("LEAVE_GROUP");
        public static readonly ServiceOperation ListGroupsPage = new("LIST_GROUPS_PAGE");
        public static readonly ServiceOperation ListGroupsPageByOffset = new("LIST_GROUPS_PAGE_BY_OFFSET");
        public static readonly ServiceOperation ListGroupsWithMember = new("LIST_GROUPS_WITH_MEMBER");
        public static readonly ServiceOperation ReadGroup = new("READ_GROUP");
        public static readonly ServiceOperation ReadGroupData = new("READ_GROUP_DATA");
        public static readonly ServiceOperation ReadGroupEntitiesPage = new("READ_GROUP_ENTITIES_PAGE");
        public static readonly ServiceOperation ReadGroupEntitiesPageByOffset = new("READ_GROUP_ENTITIES_PAGE_BY_OFFSET");
        public static readonly ServiceOperation ReadGroupEntity = new("READ_GROUP_ENTITY");
        public static readonly ServiceOperation ReadGroupMembers = new("READ_GROUP_MEMBERS");
        public static readonly ServiceOperation RejectGroupInvitation = new("REJECT_GROUP_INVITATION");
        public static readonly ServiceOperation RejectGroupJoinRequest = new("REJECT_GROUP_JOIN_REQUEST");
        public static readonly ServiceOperation RemoveGroupMember = new("REMOVE_GROUP_MEMBER");
        public static readonly ServiceOperation UpdateGroupData = new("UPDATE_GROUP_DATA");
        public static readonly ServiceOperation UpdateGroupEntity = new("UPDATE_GROUP_ENTITY_DATA");
        public static readonly ServiceOperation UpdateGroupMember = new("UPDATE_GROUP_MEMBER");
        public static readonly ServiceOperation UpdateGroupACL = new("UPDATE_GROUP_ACL");
        public static readonly ServiceOperation UpdateGroupEntityACL = new("UPDATE_GROUP_ENTITY_ACL");
        public static readonly ServiceOperation UpdateGroupName = new("UPDATE_GROUP_NAME");
        public static readonly ServiceOperation SetGroupOpen = new("SET_GROUP_OPEN");
        public static readonly ServiceOperation GetRandomGroupsMatching = new("GET_RANDOM_GROUPS_MATCHING");
        public static readonly ServiceOperation UpdateGroupSummaryData = new("UPDATE_GROUP_SUMMARY_DATA");

        //mail
        public static readonly ServiceOperation SendBasicEmail = new("SEND_BASIC_EMAIL");
        public static readonly ServiceOperation SendAdvancedEmail = new("SEND_ADVANCED_EMAIL");
        public static readonly ServiceOperation SendAdvancedEmailByAddress = new("SEND_ADVANCED_EMAIL_BY_ADDRESS");
        public static readonly ServiceOperation SendAdvancedEmailByAddresses = new("SEND_ADVANCED_EMAIL_BY_ADDRESSES");

        //peer
        public static readonly ServiceOperation AttachPeerProfile = new("ATTACH_PEER_PROFILE");
        public static readonly ServiceOperation DetachPeer = new("DETACH_PEER");
        public static readonly ServiceOperation GetPeerProfiles = new("GET_PEER_PROFILES");

        //presence
        public static readonly ServiceOperation ForcePush = new("FORCE_PUSH");
        public static readonly ServiceOperation GetPresenceOfFriends = new("GET_PRESENCE_OF_FRIENDS");
        public static readonly ServiceOperation GetPresenceOfGroup = new("GET_PRESENCE_OF_GROUP");
        public static readonly ServiceOperation GetPresenceOfUsers = new("GET_PRESENCE_OF_USERS");
        public static readonly ServiceOperation RegisterListenersForFriends = new("REGISTER_LISTENERS_FOR_FRIENDS");
        public static readonly ServiceOperation RegisterListenersForGroup = new("REGISTER_LISTENERS_FOR_GROUP");
        public static readonly ServiceOperation RegisterListenersForProfiles = new("REGISTER_LISTENERS_FOR_PROFILES");
        public static readonly ServiceOperation SetVisibility = new("SET_VISIBILITY");
        public static readonly ServiceOperation StopListening = new("STOP_LISTENING");
        public static readonly ServiceOperation UpdateActivity = new("UPDATE_ACTIVITY");

        //tournament
        public static readonly ServiceOperation GetTournamentStatus = new("GET_TOURNAMENT_STATUS");
        public static readonly ServiceOperation GetDivisionInfo = new("GET_DIVISION_INFO");
        public static readonly ServiceOperation GetMyDivisions = new("GET_MY_DIVISIONS");
        public static readonly ServiceOperation JoinDivision= new("JOIN_DIVISION");
        public static readonly ServiceOperation JoinTournament = new("JOIN_TOURNAMENT");
        public static readonly ServiceOperation LeaveDivisionInstance = new("LEAVE_DIVISION_INSTANCE");
        public static readonly ServiceOperation LeaveTournament = new("LEAVE_TOURNAMENT");
        public static readonly ServiceOperation PostTournamentScore = new("POST_TOURNAMENT_SCORE");
        public static readonly ServiceOperation PostTournamentScoreWithResults = new("POST_TOURNAMENT_SCORE_WITH_RESULTS");
        public static readonly ServiceOperation ViewCurrentReward = new("VIEW_CURRENT_REWARD");
        public static readonly ServiceOperation ViewReward = new("VIEW_REWARD");
        public static readonly ServiceOperation ClaimTournamentReward = new("CLAIM_TOURNAMENT_REWARD");

        // rtt
        public static readonly ServiceOperation RequestClientConnection = new("REQUEST_CLIENT_CONNECTION");

        // chat
        public static readonly ServiceOperation ChannelConnect = new("CHANNEL_CONNECT");
        public static readonly ServiceOperation ChannelDisconnect = new("CHANNEL_DISCONNECT");
        public static readonly ServiceOperation DeleteChatMessage = new("DELETE_CHAT_MESSAGE");
        public static readonly ServiceOperation GetChannelId = new("GET_CHANNEL_ID");
        public static readonly ServiceOperation GetChannelInfo = new("GET_CHANNEL_INFO");
        public static readonly ServiceOperation GetChatMessage = new("GET_CHAT_MESSAGE");
        public static readonly ServiceOperation GetRecentChatMessages = new("GET_RECENT_CHAT_MESSAGES");
        public static readonly ServiceOperation GetSubscribedChannels = new("GET_SUBSCRIBED_CHANNELS");
        public static readonly ServiceOperation PostChatMessage = new("POST_CHAT_MESSAGE");
        public static readonly ServiceOperation PostChatMessageSimple = new("POST_CHAT_MESSAGE_SIMPLE");
        public static readonly ServiceOperation UpdateChatMessage = new("UPDATE_CHAT_MESSAGE");

        // messaging 
        public static readonly ServiceOperation DeleteMessages = new("DELETE_MESSAGES");
        public static readonly ServiceOperation GetMessageBoxes = new("GET_MESSAGE_BOXES");
        public static readonly ServiceOperation GetMessageCounts = new("GET_MESSAGE_COUNTS");
        public static readonly ServiceOperation GetMessages = new("GET_MESSAGES");
        public static readonly ServiceOperation GetMessagesPage = new("GET_MESSAGES_PAGE");
        public static readonly ServiceOperation GetMessagesPageOffset = new("GET_MESSAGES_PAGE_OFFSET");
        public static readonly ServiceOperation MarkMessagesRead = new("MARK_MESSAGES_READ");
        public static readonly ServiceOperation SendMessage = new("SEND_MESSAGE");
        public static readonly ServiceOperation SendMessageSimple = new("SEND_MESSAGE_SIMPLE");

        // lobby
        public static readonly ServiceOperation FindLobby = new("FIND_LOBBY");
        public static readonly ServiceOperation FindLobbyWithPingData = new("FIND_LOBBY_WITH_PING_DATA");
        public static readonly ServiceOperation CreateLobby = new("CREATE_LOBBY");
        public static readonly ServiceOperation CreateLobbyWithPingData = new("CREATE_LOBBY_WITH_PING_DATA");
        public static readonly ServiceOperation FindOrCreateLobby = new("FIND_OR_CREATE_LOBBY");
        public static readonly ServiceOperation FindOrCreateLobbyWithPingData = new("FIND_OR_CREATE_LOBBY_WITH_PING_DATA");
        public static readonly ServiceOperation GetLobbyData = new("GET_LOBBY_DATA");
        public static readonly ServiceOperation UpdateReady = new("UPDATE_READY");
        public static readonly ServiceOperation UpdateSettings = new("UPDATE_SETTINGS");
        public static readonly ServiceOperation SwitchTeam = new("SWITCH_TEAM");
        public static readonly ServiceOperation SendSignal = new("SEND_SIGNAL");
        public static readonly ServiceOperation JoinLobby = new("JOIN_LOBBY");
        public static readonly ServiceOperation JoinLobbyWithPingData = new("JOIN_LOBBY_WITH_PING_DATA");
        public static readonly ServiceOperation LeaveLobby = new("LEAVE_LOBBY");
        public static readonly ServiceOperation RemoveMember = new("REMOVE_MEMBER");
        public static readonly ServiceOperation CancelFindRequest = new("CANCEL_FIND_REQUEST");
        public static readonly ServiceOperation GetRegionsForLobbies = new("GET_REGIONS_FOR_LOBBIES");
        public static readonly ServiceOperation GetLobbyInstances = new("GET_LOBBY_INSTANCES");
        public static readonly ServiceOperation GetLobbyInstancesWithPingData = new("GET_LOBBY_INSTANCES_WITH_PING_DATA");

        
        //ItemCatalog
        public static readonly ServiceOperation GetCatalogItemDefinition = new("GET_CATALOG_ITEM_DEFINITION");
        public static readonly ServiceOperation GetCatalogItemsPage = new("GET_CATALOG_ITEMS_PAGE");
        public static readonly ServiceOperation GetCatalogItemsPageOffset = new("GET_CATALOG_ITEMS_PAGE_OFFSET");

        //CustomEntity

        public static readonly ServiceOperation DeleteEntities = new("DELETE_ENTITIES");
        public static readonly ServiceOperation CreateCustomEntity = new("CREATE_ENTITY");
        public static readonly ServiceOperation GetCustomEntityPage = new("GET_PAGE");
        public static readonly ServiceOperation GetCustomEntityPageOffset = new("GET_ENTITY_PAGE_OFFSET");
        public static readonly ServiceOperation GetEntityPage = new("GET_ENTITY_PAGE");
        public static readonly ServiceOperation ReadCustomEntity = new("READ_ENTITY");
        public static readonly ServiceOperation IncrementData = new("INCREMENT_DATA");
        public static readonly ServiceOperation IncrementSingletonData = new("INCREMENT_SINGLETON_DATA");
        public static readonly ServiceOperation UpdateCustomEntity = new("UPDATE_ENTITY");
        public static readonly ServiceOperation UpdateCustomEntityFields = new("UPDATE_ENTITY_FIELDS");
        public static readonly ServiceOperation UpdateCustomEntityFieldsShards = new("UPDATE_ENTITY_FIELDS_SHARDED");
        public static readonly ServiceOperation DeleteCustomEntity = new("DELETE_ENTITY");
        public static readonly ServiceOperation GetCount = new("GET_COUNT");
        public static readonly ServiceOperation UpdateSingletonFields = new("UPDATE_SINGLETON_FIELDS");

        //UserItemsService

        public static readonly ServiceOperation AwardUserItem = new("AWARD_USER_ITEM");
        public static readonly ServiceOperation DropUserItem = new("DROP_USER_ITEM");
        public static readonly ServiceOperation GetUserItemsPage = new("GET_USER_ITEMS_PAGE");
        public static readonly ServiceOperation GetUserItemsPageOffset = new("GET_USER_ITEMS_PAGE_OFFSET");
        public static readonly ServiceOperation GetUserItem = new("GET_USER_ITEM");
        public static readonly ServiceOperation GiveUserItemTo = new("GIVE_USER_ITEM_TO");
        public static readonly ServiceOperation PurchaseUserItem = new("PURCHASE_USER_ITEM");
        public static readonly ServiceOperation ReceiveUserItemFrom = new("RECEIVE_USER_ITEM_FROM");
        public static readonly ServiceOperation SellUserItem = new("SELL_USER_ITEM");
        public static readonly ServiceOperation UpdateUserItemData = new("UPDATE_USER_ITEM_DATA");
        public static readonly ServiceOperation UseUserItem = new("USE_USER_ITEM");
        public static readonly ServiceOperation PublishUserItemToBlockchain = new("PUBLISH_USER_ITEM_TO_BLOCKCHAIN");
        public static readonly ServiceOperation RefreshBlockchainUserItems = new("REFRESH_BLOCKCHAIN_USER_ITEMS");
        public static readonly ServiceOperation RemoveUserItemFromBlockchain = new("REMOVE_USER_ITEM_FROM_BLOCKCHAIN");

        //Group File Services
        public static readonly ServiceOperation CheckFilenameExists = new("CHECK_FILENAME_EXISTS");
        public static readonly ServiceOperation CheckFullpathFilenameExists = new("CHECK_FULLPATH_FILENAME_EXISTS");
        public static readonly ServiceOperation CopyFile = new("COPY_FILE");
        public static readonly ServiceOperation DeleteFile = new("DELETE_FILE");
        public static readonly ServiceOperation MoveFile = new("MOVE_FILE");
        public static readonly ServiceOperation MoveUserToGroupFile = new("MOVE_USER_TO_GROUP_FILE");
        public static readonly ServiceOperation UpdateFileInfo = new("UPDATE_FILE_INFO");

        #endregion

        private ServiceOperation(string value)
        {
            Value = value;
        }

        public readonly string Value;

        #region Overrides and Operators

        public readonly override bool Equals(object obj)
        {
            if (obj is not ServiceOperation s)
                return false;

            return Equals(s);
        }

        public readonly bool Equals(ServiceOperation other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public readonly int CompareTo(ServiceOperation other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public readonly override int GetHashCode() => Value.GetHashCode();

        public readonly override string ToString() => Value;

        public static implicit operator string(ServiceOperation v) => v.Value;

        public static bool operator ==(ServiceOperation v1, ServiceOperation v2) => v1.Equals(v2);

        public static bool operator !=(ServiceOperation v1, ServiceOperation v2) => !(v1 == v2);

        public static bool operator >(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(ServiceOperation v1, ServiceOperation v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
