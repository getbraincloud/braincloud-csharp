//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;

namespace BrainCloud.Internal
{
    /**
     * List of all available service operations. The values are mapped to server keys which represent that operation.
     */
    internal class ServiceOperation
    {
        public static readonly ServiceOperation Authenticate = new ServiceOperation("AUTHENTICATE");
        public static readonly ServiceOperation Attach = new ServiceOperation("ATTACH");
        public static readonly ServiceOperation Merge = new ServiceOperation("MERGE");
        public static readonly ServiceOperation Detach = new ServiceOperation("DETACH");
        public static readonly ServiceOperation ResetEmailPassword = new ServiceOperation("RESET_EMAIL_PASSWORD");
        public static readonly ServiceOperation SwitchToChildProfile = new ServiceOperation("SWITCH_TO_CHILD_PROFILE");
        public static readonly ServiceOperation SwitchToParentProfile = new ServiceOperation("SWITCH_TO_PARENT_PROFILE");
        public static readonly ServiceOperation DetachParent = new ServiceOperation("DETACH_PARENT");
        public static readonly ServiceOperation AttachParentWithIdentity = new ServiceOperation("ATTACH_PARENT_WITH_IDENTITY");

        public static readonly ServiceOperation Create = new ServiceOperation("CREATE");
        public static readonly ServiceOperation CreateWithIndexedId = new ServiceOperation("CREATE_WITH_INDEXED_ID");
        public static readonly ServiceOperation Reset = new ServiceOperation("RESET");
        public static readonly ServiceOperation Read = new ServiceOperation("READ");
        public static readonly ServiceOperation ReadSingleton = new ServiceOperation("READ_SINGLETON");
        public static readonly ServiceOperation ReadByType = new ServiceOperation("READ_BY_TYPE");
        public static readonly ServiceOperation Verify = new ServiceOperation("VERIFY");
        public static readonly ServiceOperation ReadShared = new ServiceOperation("READ_SHARED");
        public static readonly ServiceOperation ReadSharedEntity = new ServiceOperation("READ_SHARED_ENTITY");
        public static readonly ServiceOperation UpdateEntityOwnerAndAcl = new ServiceOperation("UPDATE_ENTITY_OWNER_AND_ACL");
        public static readonly ServiceOperation MakeSystemEntity = new ServiceOperation("MAKE_SYSTEM_ENTITY");

        // push notification
        public static readonly ServiceOperation DeregisterAll = new ServiceOperation("DEREGISTER_ALL");
        public static readonly ServiceOperation Deregister = new ServiceOperation("DEREGISTER");
        public static readonly ServiceOperation Register = new ServiceOperation("REGISTER");
        public static readonly ServiceOperation SendSimple = new ServiceOperation("SEND_SIMPLE");
        public static readonly ServiceOperation SendRich = new ServiceOperation("SEND_RICH");
        public static readonly ServiceOperation SendRaw = new ServiceOperation("SEND_RAW");
        public static readonly ServiceOperation SendRawBatch = new ServiceOperation("SEND_RAW_BATCH");
        public static readonly ServiceOperation SendRawToGroup = new ServiceOperation("SEND_RAW_TO_GROUP");
        public static readonly ServiceOperation SendTemplatedToGroup = new ServiceOperation("SEND_TEMPLATED_TO_GROUP");
        public static readonly ServiceOperation SendNormalizedToGroup = new ServiceOperation("SEND_NORMALIZED_TO_GROUP");
        public static readonly ServiceOperation SendNormalized = new ServiceOperation("SEND_NORMALIZED");
        public static readonly ServiceOperation SendNormalizedBatch = new ServiceOperation("SEND_NORMALIZED_BATCH");
        public static readonly ServiceOperation ScheduleRichNotification = new ServiceOperation("SCHEDULE_RICH_NOTIFICATION");
        public static readonly ServiceOperation ScheduleNormalizedNotification = new ServiceOperation("SCHEDULE_NORMALIZED_NOTIFICATION");

        public static readonly ServiceOperation ScheduleRawNotification = new ServiceOperation("SCHEDULE_RAW_NOTIFICATION");

        public static readonly ServiceOperation FullReset = new ServiceOperation("FULL_PLAYER_RESET");
        public static readonly ServiceOperation DataReset = new ServiceOperation("GAME_DATA_RESET");

        public static readonly ServiceOperation ProcessStatistics = new ServiceOperation("PROCESS_STATISTICS");
        public static readonly ServiceOperation Update = new ServiceOperation("UPDATE");
        public static readonly ServiceOperation UpdateShared = new ServiceOperation("UPDATE_SHARED");
        public static readonly ServiceOperation UpdateAcl = new ServiceOperation("UPDATE_ACL");
        public static readonly ServiceOperation UpdateTimeToLive = new ServiceOperation("UPDATE_TIME_TO_LIVE");
        public static readonly ServiceOperation UpdatePartial = new ServiceOperation("UPDATE_PARTIAL");
        public static readonly ServiceOperation UpdateSingleton = new ServiceOperation("UPDATE_SINGLETON");
        public static readonly ServiceOperation Delete = new ServiceOperation("DELETE");
        public static readonly ServiceOperation DeleteSingleton = new ServiceOperation("DELETE_SINGLETON");
        public static readonly ServiceOperation UpdateSummary = new ServiceOperation("UPDATE_SUMMARY");
        public static readonly ServiceOperation UpdateSetMinimum = new ServiceOperation("UPDATE_SET_MINIMUM");
        public static readonly ServiceOperation UpdateIncrementToMaximum = new ServiceOperation("UPDATE_INCREMENT_TO_MAXIMUM");
        public static readonly ServiceOperation GetFriendProfileInfoForExternalId = new ServiceOperation("GET_FRIEND_PROFILE_INFO_FOR_EXTERNAL_ID");
        public static readonly ServiceOperation GetProfileInfoForCredential = new ServiceOperation("GET_PROFILE_INFO_FOR_CREDENTIAL");
        public static readonly ServiceOperation GetProfileInfoForExternalAuthId = new ServiceOperation("GET_PROFILE_INFO_FOR_EXTERNAL_AUTH_ID");
        public static readonly ServiceOperation GetExternalIdForProfileId = new ServiceOperation("GET_EXTERNAL_ID_FOR_PROFILE_ID");
        public static readonly ServiceOperation FindPlayerByUniversalId = new ServiceOperation("FIND_PLAYER_BY_UNIVERSAL_ID");
        public static readonly ServiceOperation ReadFriends = new ServiceOperation("READ_FRIENDS");
        public static readonly ServiceOperation ReadFriendEntity = new ServiceOperation("READ_FRIEND_ENTITY");
        public static readonly ServiceOperation ReadFriendsEntities = new ServiceOperation("READ_FRIENDS_ENTITIES");
        public static readonly ServiceOperation ReadFriendsWithApplication = new ServiceOperation("READ_FRIENDS_WITH_APPLICATION");
        public static readonly ServiceOperation ReadFriendPlayerState = new ServiceOperation("READ_FRIEND_PLAYER_STATE");
        public static readonly ServiceOperation GetSummaryDataForProfileId = new ServiceOperation("GET_SUMMARY_DATA_FOR_PROFILE_ID");
        public static readonly ServiceOperation FindPlayerByName = new ServiceOperation("FIND_PLAYER_BY_NAME");
        public static readonly ServiceOperation FindUsersByExactName = new ServiceOperation("FIND_USERS_BY_EXACT_NAME");
        public static readonly ServiceOperation FindUsersBySubstrName = new ServiceOperation("FIND_USERS_BY_SUBSTR_NAME");
        public static readonly ServiceOperation ListFriends = new ServiceOperation("LIST_FRIENDS");
        public static readonly ServiceOperation AddFriends = new ServiceOperation("ADD_FRIENDS");
        public static readonly ServiceOperation RemoveFriends = new ServiceOperation("REMOVE_FRIENDS");
        public static readonly ServiceOperation GetUsersOnlineStatus = new ServiceOperation("GET_USERS_ONLINE_STATUS");
        public static readonly ServiceOperation GetSocialLeaderboard = new ServiceOperation("GET_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetSocialLeaderboardByVersion = new ServiceOperation("GET_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation GetMultiSocialLeaderboard = new ServiceOperation("GET_MULTI_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetGlobalLeaderboard = new ServiceOperation("GET_GLOBAL_LEADERBOARD");
        public static readonly ServiceOperation GetGlobalLeaderboardPage = new ServiceOperation("GET_GLOBAL_LEADERBOARD_PAGE");
        public static readonly ServiceOperation GetGlobalLeaderboardView = new ServiceOperation("GET_GLOBAL_LEADERBOARD_VIEW");
        public static readonly ServiceOperation GetGlobalLeaderboardVersions = new ServiceOperation("GET_GLOBAL_LEADERBOARD_VERSIONS");
        public static readonly ServiceOperation PostScore = new ServiceOperation("POST_SCORE");
        public static readonly ServiceOperation PostScoreDynamic = new ServiceOperation("POST_SCORE_DYNAMIC");
        public static readonly ServiceOperation RemovePlayerScore = new ServiceOperation("REMOVE_PLAYER_SCORE");
        public static readonly ServiceOperation GetCompletedTournament = new ServiceOperation("GET_COMPLETED_TOURNAMENT");
        public static readonly ServiceOperation RewardTournament = new ServiceOperation("REWARD_TOURNAMENT");
        public static readonly ServiceOperation GetGroupSocialLeaderboard = new ServiceOperation("GET_GROUP_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetGroupSocialLeaderboardByVersion = new ServiceOperation("GET_GROUP_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation GetPlayersSocialLeaderboard = new ServiceOperation("GET_PLAYERS_SOCIAL_LEADERBOARD");
        public static readonly ServiceOperation GetPlayersSocialLeaderboardByVersion = new ServiceOperation("GET_PLAYERS_SOCIAL_LEADERBOARD_BY_VERSION");
        public static readonly ServiceOperation ListAllLeaderboards = new ServiceOperation("LIST_ALL_LEADERBOARDS");
        public static readonly ServiceOperation GetGlobalLeaderboardEntryCount = new ServiceOperation("GET_GLOBAL_LEADERBOARD_ENTRY_COUNT");
        public static readonly ServiceOperation GetPlayerScore = new ServiceOperation("GET_PLAYER_SCORE");
        public static readonly ServiceOperation GetPlayerScoresFromLeaderboards = new ServiceOperation("GET_PLAYER_SCORES_FROM_LEADERBOARDS");

        public static readonly ServiceOperation ReadFriendsPlayerState = new ServiceOperation("READ_FRIEND_PLAYER_STATE");

        public static readonly ServiceOperation InitThirdParty = new ServiceOperation("initThirdParty");
        public static readonly ServiceOperation PostThirdPartyLeaderboardScore = new ServiceOperation("postThirdPartyLeaderboardScore");
        public static readonly ServiceOperation IncrementThirdPartyLeaderboardScore = new ServiceOperation("incrementThirdPartyLeaderboardScore");
        public static readonly ServiceOperation LaunchAchievementUI = new ServiceOperation("launchAchievementUI");
        public static readonly ServiceOperation PostThirdPartyLeaderboardAchievement = new ServiceOperation("postThirdPartyLeaderboardAchievement");
        public static readonly ServiceOperation IsThirdPartyAchievementComplete = new ServiceOperation("isThirdPartyAchievementComplete");
        public static readonly ServiceOperation ResetThirdPartyAchievements = new ServiceOperation("resetThirdPartyAchievements");
        public static readonly ServiceOperation QueryThirdPartyAchievements = new ServiceOperation("queryThirdPartyAchievements");

        public static readonly ServiceOperation GetInventory = new ServiceOperation("GET_INVENTORY");
        public static readonly ServiceOperation CashInReceipt = new ServiceOperation("OP_CASH_IN_RECEIPT");
        public static readonly ServiceOperation AwardVC = new ServiceOperation("AWARD_VC");
        public static readonly ServiceOperation ConsumePlayerVC = new ServiceOperation("CONSUME_VC");
        public static readonly ServiceOperation GetPlayerVC = new ServiceOperation("GET_PLAYER_VC");
        public static readonly ServiceOperation ResetPlayerVC = new ServiceOperation("RESET_PLAYER_VC");

        public static readonly ServiceOperation AwardParentCurrency = new ServiceOperation("AWARD_PARENT_VC");
        public static readonly ServiceOperation ConsumeParentCurrency = new ServiceOperation("CONSUME_PARENT_VC");
        public static readonly ServiceOperation GetParentVC = new ServiceOperation("GET_PARENT_VC");
        public static readonly ServiceOperation ResetParentCurrency = new ServiceOperation("RESET_PARENT_VC");

        public static readonly ServiceOperation GetPeerVC = new ServiceOperation("GET_PEER_VC");
        public static readonly ServiceOperation StartPurchase = new ServiceOperation("START_PURCHASE");
        public static readonly ServiceOperation FinalizePurchase = new ServiceOperation("FINALIZE_PURCHASE");

        public static readonly ServiceOperation Send = new ServiceOperation("SEND");
        public static readonly ServiceOperation UpdateEventData = new ServiceOperation("UPDATE_EVENT_DATA");
        public static readonly ServiceOperation DeleteSent = new ServiceOperation("DELETE_SENT");
        public static readonly ServiceOperation DeleteIncoming = new ServiceOperation("DELETE_INCOMING");
        public static readonly ServiceOperation GetEvents = new ServiceOperation("GET_EVENTS");

        public static readonly ServiceOperation UpdateIncrement = new ServiceOperation("UPDATE_INCREMENT");
        public static readonly ServiceOperation ReadNextXpLevel = new ServiceOperation("READ_NEXT_XPLEVEL");
        public static readonly ServiceOperation ReadXpLevels = new ServiceOperation("READ_XP_LEVELS");
        public static readonly ServiceOperation SetXpPoints = new ServiceOperation("SET_XPPOINTS");
        public static readonly ServiceOperation ReadSubset = new ServiceOperation("READ_SUBSET");

        public static readonly ServiceOperation Run = new ServiceOperation("RUN");
        public static readonly ServiceOperation Tweet = new ServiceOperation("TWEET");

        public static readonly ServiceOperation AwardAchievements = new ServiceOperation("AWARD_ACHIEVEMENTS");
        public static readonly ServiceOperation ReadAchievements = new ServiceOperation("READ_ACHIEVEMENTS");
        public static readonly ServiceOperation ReadAchievedAchievements = new ServiceOperation("READ_ACHIEVED_ACHIEVEMENTS");

        public static readonly ServiceOperation SetPlayerRating = new ServiceOperation("SET_PLAYER_RATING");
        public static readonly ServiceOperation ResetPlayerRating = new ServiceOperation("RESET_PLAYER_RATING");
        public static readonly ServiceOperation IncrementPlayerRating = new ServiceOperation("INCREMENT_PLAYER_RATING");
        public static readonly ServiceOperation DecrementPlayerRating = new ServiceOperation("DECREMENT_PLAYER_RATING");
        public static readonly ServiceOperation ShieldOn = new ServiceOperation("SHIELD_ON");
        public static readonly ServiceOperation ShieldOnFor = new ServiceOperation("SHIELD_ON_FOR");
        public static readonly ServiceOperation ShieldOff = new ServiceOperation("SHIELD_OFF");
        public static readonly ServiceOperation IncrementShieldOnFor = new ServiceOperation("INCREMENT_SHIELD_ON_FOR");
        public static readonly ServiceOperation GetShieldExpiry = new ServiceOperation("GET_SHIELD_EXPIRY");
        public static readonly ServiceOperation FindPlayers = new ServiceOperation("FIND_PLAYERS");
        public static readonly ServiceOperation FindPlayersUsingFilter = new ServiceOperation("FIND_PLAYERS_USING_FILTER");

        public static readonly ServiceOperation SubmitTurn = new ServiceOperation("SUBMIT_TURN");
        public static readonly ServiceOperation UpdateMatchSummary = new ServiceOperation("UPDATE_SUMMARY");
        public static readonly ServiceOperation Abandon = new ServiceOperation("ABANDON");
        public static readonly ServiceOperation Complete = new ServiceOperation("COMPLETE");
        public static readonly ServiceOperation ReadMatch = new ServiceOperation("READ_MATCH");
        public static readonly ServiceOperation ReadMatchHistory = new ServiceOperation("READ_MATCH_HISTORY");
        public static readonly ServiceOperation FindMatches = new ServiceOperation("FIND_MATCHES");
        public static readonly ServiceOperation FindMatchesCompleted = new ServiceOperation("FIND_MATCHES_COMPLETED");
        public static readonly ServiceOperation DeleteMatch = new ServiceOperation("DELETE_MATCH");

        public static readonly ServiceOperation LastUploadStatus = new ServiceOperation("LAST_UPLOAD_STATUS");

        public static readonly ServiceOperation ReadQuests = new ServiceOperation("READ_QUESTS");
        public static readonly ServiceOperation ReadCompletedQuests = new ServiceOperation("READ_COMPLETED_QUESTS");
        public static readonly ServiceOperation ReadInProgressQuests = new ServiceOperation("READ_IN_PROGRESS_QUESTS");
        public static readonly ServiceOperation ReadNotStartedQuests = new ServiceOperation("READ_NOT_STARTED_QUESTS");
        public static readonly ServiceOperation ReadQuestsWithStatus = new ServiceOperation("READ_QUESTS_WITH_STATUS");
        public static readonly ServiceOperation ReadQuestsWithBasicPercentage = new ServiceOperation("READ_QUESTS_WITH_BASIC_PERCENTAGE");
        public static readonly ServiceOperation ReadQuestsWithComplexPercentage = new ServiceOperation("READ_QUESTS_WITH_COMPLEX_PERCENTAGE");
        public static readonly ServiceOperation ReadQuestsByCategory = new ServiceOperation("READ_QUESTS_BY_CATEGORY");
        public static readonly ServiceOperation ResetMilestones = new ServiceOperation("RESET_MILESTONES");

        public static readonly ServiceOperation ReadForCategory = new ServiceOperation("READ_FOR_CATEGORY");

        public static readonly ServiceOperation ReadMilestones = new ServiceOperation("READ_MILESTONES");
        public static readonly ServiceOperation ReadMilestonesByCategory = new ServiceOperation("READ_MILESTONES_BY_CATEGORY");

        public static readonly ServiceOperation ReadCompletedMilestones = new ServiceOperation("READ_COMPLETED_MILESTONES");
        public static readonly ServiceOperation ReadInProgressMilestones = new ServiceOperation("READ_IN_PROGRESS_MILESTONES");

        public static readonly ServiceOperation Trigger = new ServiceOperation("TRIGGER");
        public static readonly ServiceOperation TriggerMultiple = new ServiceOperation("TRIGGER_MULTIPLE");

        public static readonly ServiceOperation Logout = new ServiceOperation("LOGOUT");

        public static readonly ServiceOperation StartMatch = new ServiceOperation("START_MATCH");
        public static readonly ServiceOperation CancelMatch = new ServiceOperation("CANCEL_MATCH");
        public static readonly ServiceOperation CompleteMatch = new ServiceOperation("COMPLETE_MATCH");
        public static readonly ServiceOperation EnableMatchMaking = new ServiceOperation("ENABLE_FOR_MATCH");
        public static readonly ServiceOperation DisableMatchMaking = new ServiceOperation("DISABLE_FOR_MATCH");
        public static readonly ServiceOperation UpdateName = new ServiceOperation("UPDATE_NAME");

        public static readonly ServiceOperation StartStream = new ServiceOperation("START_STREAM");
        public static readonly ServiceOperation ReadStream = new ServiceOperation("READ_STREAM");
        public static readonly ServiceOperation EndStream = new ServiceOperation("END_STREAM");
        public static readonly ServiceOperation DeleteStream = new ServiceOperation("DELETE_STREAM");
        public static readonly ServiceOperation AddEvent = new ServiceOperation("ADD_EVENT");
        public static readonly ServiceOperation GetStreamSummariesForInitiatingPlayer = new ServiceOperation("GET_STREAM_SUMMARIES_FOR_INITIATING_PLAYER");
        public static readonly ServiceOperation GetStreamSummariesForTargetPlayer = new ServiceOperation("GET_STREAM_SUMMARIES_FOR_TARGET_PLAYER");
        public static readonly ServiceOperation GetRecentStreamsForInitiatingPlayer = new ServiceOperation("GET_RECENT_STREAMS_FOR_INITIATING_PLAYER");
        public static readonly ServiceOperation GetRecentStreamsForTargetPlayer = new ServiceOperation("GET_RECENT_STREAMS_FOR_TARGET_PLAYER");

        public static readonly ServiceOperation GetUserInfo = new ServiceOperation("GET_USER_INFO");
        public static readonly ServiceOperation InitializeTransaction = new ServiceOperation("INITIALIZE_TRANSACTION");
        public static readonly ServiceOperation FinalizeTransaction = new ServiceOperation("FINALIZE_TRANSACTION");

        public static readonly ServiceOperation VerifyPurchase = new ServiceOperation("VERIFY_PURCHASE");
        public static readonly ServiceOperation StartSteamTransaction = new ServiceOperation("START_STEAM_TRANSACTION");
        public static readonly ServiceOperation FinalizeSteamTransaction = new ServiceOperation("FINALIZE_STEAM_TRANSACTION");
        public static readonly ServiceOperation VerifyMicrosoftReceipt = new ServiceOperation("VERIFY_MICROSOFT_RECEIPT");
        public static readonly ServiceOperation EligiblePromotions = new ServiceOperation("ELIGIBLE_PROMOTIONS");

        public static readonly ServiceOperation ReadSharedEntitiesList = new ServiceOperation("READ_SHARED_ENTITIES_LIST");
        public static readonly ServiceOperation GetList = new ServiceOperation("GET_LIST");
        public static readonly ServiceOperation GetListByIndexedId = new ServiceOperation("GET_LIST_BY_INDEXED_ID");
        public static readonly ServiceOperation GetListCount = new ServiceOperation("GET_LIST_COUNT");
        public static readonly ServiceOperation GetPage = new ServiceOperation("GET_PAGE");
        public static readonly ServiceOperation GetPageOffset = new ServiceOperation("GET_PAGE_BY_OFFSET");
        public static readonly ServiceOperation IncrementGlobalEntityData = new ServiceOperation("INCREMENT_GLOBAL_ENTITY_DATA");
        public static readonly ServiceOperation GetRandomEntitiesMatching = new ServiceOperation("GET_RANDOM_ENTITIES_MATCHING");
        public static readonly ServiceOperation IncrementUserEntityData = new ServiceOperation("INCREMENT_USER_ENTITY_DATA");
        public static readonly ServiceOperation IncrementSharedUserEntityData = new ServiceOperation("INCREMENT_SHARED_USER_ENTITY_DATA");

        public static readonly ServiceOperation UpdatePictureUrl = new ServiceOperation("UPDATE_PICTURE_URL");
        public static readonly ServiceOperation UpdateContactEmail = new ServiceOperation("UPDATE_CONTACT_EMAIL");

        public static readonly ServiceOperation GetAttributes = new ServiceOperation("GET_ATTRIBUTES");
        public static readonly ServiceOperation UpdateAttributes = new ServiceOperation("UPDATE_ATTRIBUTES");
        public static readonly ServiceOperation RemoveAttributes = new ServiceOperation("REMOVE_ATTRIBUTES");
        public static readonly ServiceOperation GetChildProfiles = new ServiceOperation("GET_CHILD_PROFILES");
        public static readonly ServiceOperation GetIdentities = new ServiceOperation("GET_IDENTITIES");
        public static readonly ServiceOperation GetExpiredIdentities = new ServiceOperation("GET_EXPIRED_IDENTITIES");
        public static readonly ServiceOperation RefreshIdentity = new ServiceOperation("REFRESH_IDENTITY");
        public static readonly ServiceOperation ChangeEmailIdentity = new ServiceOperation("CHANGE_EMAIL_IDENTITY");

        public static readonly ServiceOperation FbConfirmPurchase = new ServiceOperation("FB_CONFIRM_PURCHASE");
        public static readonly ServiceOperation GooglePlayConfirmPurchase = new ServiceOperation("CONFIRM_GOOGLEPLAY_PURCHASE");

        public static readonly ServiceOperation ReadProperties = new ServiceOperation("READ_PROPERTIES");

        public static readonly ServiceOperation GetUpdatedFiles = new ServiceOperation("GET_UPDATED_FILES");
        public static readonly ServiceOperation GetFileList = new ServiceOperation("GET_FILE_LIST");

        public static readonly ServiceOperation ScheduleCloudScript = new ServiceOperation("SCHEDULE_CLOUD_SCRIPT");
        public static readonly ServiceOperation RunParentScript = new ServiceOperation("RUN_PARENT_SCRIPT");
        public static readonly ServiceOperation CancelScheduledScript = new ServiceOperation("CANCEL_SCHEDULED_SCRIPT");
        public static readonly ServiceOperation RunPeerScript = new ServiceOperation("RUN_PEER_SCRIPT");
        public static readonly ServiceOperation RunPeerScriptAsync = new ServiceOperation("RUN_PEER_SCRIPT_ASYNC");

        //RedemptionCode
        public static readonly ServiceOperation GetRedeemedCodes = new ServiceOperation("GET_REDEEMED_CODES");
        public static readonly ServiceOperation RedeemCode = new ServiceOperation("REDEEM_CODE");

        //DataStream
        public static readonly ServiceOperation CustomPageEvent = new ServiceOperation("CUSTOM_PAGE_EVENT");
        public static readonly ServiceOperation CustomScreenEvent = new ServiceOperation("CUSTOM_SCREEN_EVENT");
        public static readonly ServiceOperation CustomTrackEvent = new ServiceOperation("CUSTOM_TRACK_EVENT");

        //Profanity
        public static readonly ServiceOperation ProfanityCheck = new ServiceOperation("PROFANITY_CHECK");
        public static readonly ServiceOperation ProfanityReplaceText = new ServiceOperation("PROFANITY_REPLACE_TEXT");
        public static readonly ServiceOperation ProfanityIdentifyBadWords = new ServiceOperation("PROFANITY_IDENTIFY_BAD_WORDS");

        //file upload
        public static readonly ServiceOperation PrepareUserUpload = new ServiceOperation("PREPARE_USER_UPLOAD");
        public static readonly ServiceOperation ListUserFiles = new ServiceOperation("LIST_USER_FILES");
        public static readonly ServiceOperation DeleteUserFile = new ServiceOperation("DELETE_USER_FILE");
        public static readonly ServiceOperation DeleteUserFiles = new ServiceOperation("DELETE_USER_FILES");
        public static readonly ServiceOperation GetCdnUrl = new ServiceOperation("GET_CDN_URL");

        //group
        public static readonly ServiceOperation AcceptGroupInvitation = new ServiceOperation("ACCEPT_GROUP_INVITATION");
        public static readonly ServiceOperation AddGroupMember = new ServiceOperation("ADD_GROUP_MEMBER");
        public static readonly ServiceOperation ApproveGroupJoinRequest = new ServiceOperation("APPROVE_GROUP_JOIN_REQUEST");
        public static readonly ServiceOperation AutoJoinGroup = new ServiceOperation("AUTO_JOIN_GROUP");
        public static readonly ServiceOperation CancelGroupInvitation = new ServiceOperation("CANCEL_GROUP_INVITATION");
        public static readonly ServiceOperation CreateGroup = new ServiceOperation("CREATE_GROUP");
        public static readonly ServiceOperation CreateGroupEntity = new ServiceOperation("CREATE_GROUP_ENTITY");
        public static readonly ServiceOperation DeleteGroup = new ServiceOperation("DELETE_GROUP");
        public static readonly ServiceOperation DeleteGroupEntity = new ServiceOperation("DELETE_GROUP_ENTITY");
        public static readonly ServiceOperation DeleteGroupMemeber = new ServiceOperation("DELETE_MEMBER_FROM_GROUP");
        public static readonly ServiceOperation GetMyGroups = new ServiceOperation("GET_MY_GROUPS");
        public static readonly ServiceOperation IncrementGroupData = new ServiceOperation("INCREMENT_GROUP_DATA");
        public static readonly ServiceOperation IncrementGroupEntityData = new ServiceOperation("INCREMENT_GROUP_ENTITY_DATA");
        public static readonly ServiceOperation InviteGroupMember = new ServiceOperation("INVITE_GROUP_MEMBER");
        public static readonly ServiceOperation JoinGroup = new ServiceOperation("JOIN_GROUP");
        public static readonly ServiceOperation LeaveGroup = new ServiceOperation("LEAVE_GROUP");
        public static readonly ServiceOperation ListGroupsPage = new ServiceOperation("LIST_GROUPS_PAGE");
        public static readonly ServiceOperation ListGroupsPageByOffset = new ServiceOperation("LIST_GROUPS_PAGE_BY_OFFSET");
        public static readonly ServiceOperation ListGroupsWithMember = new ServiceOperation("LIST_GROUPS_WITH_MEMBER");
        public static readonly ServiceOperation ReadGroup = new ServiceOperation("READ_GROUP");
        public static readonly ServiceOperation ReadGroupData = new ServiceOperation("READ_GROUP_DATA");
        public static readonly ServiceOperation ReadGroupEntitiesPage = new ServiceOperation("READ_GROUP_ENTITIES_PAGE");
        public static readonly ServiceOperation ReadGroupEntitiesPageByOffset = new ServiceOperation("READ_GROUP_ENTITIES_PAGE_BY_OFFSET");
        public static readonly ServiceOperation ReadGroupEntity = new ServiceOperation("READ_GROUP_ENTITY");
        public static readonly ServiceOperation ReadGroupMembers = new ServiceOperation("READ_GROUP_MEMBERS");
        public static readonly ServiceOperation RejectGroupInvitation = new ServiceOperation("REJECT_GROUP_INVITATION");
        public static readonly ServiceOperation RejectGroupJoinRequest = new ServiceOperation("REJECT_GROUP_JOIN_REQUEST");
        public static readonly ServiceOperation RemoveGroupMember = new ServiceOperation("REMOVE_GROUP_MEMBER");
        public static readonly ServiceOperation UpdateGroupData = new ServiceOperation("UPDATE_GROUP_DATA");
        public static readonly ServiceOperation UpdateGroupEntity = new ServiceOperation("UPDATE_GROUP_ENTITY_DATA");
        public static readonly ServiceOperation UpdateGroupMember = new ServiceOperation("UPDATE_GROUP_MEMBER");
        public static readonly ServiceOperation UpdateGroupName = new ServiceOperation("UPDATE_GROUP_NAME");
        public static readonly ServiceOperation SetGroupOpen = new ServiceOperation("SET_GROUP_OPEN");

        //mail
        public static readonly ServiceOperation SendBasicEmail = new ServiceOperation("SEND_BASIC_EMAIL");
        public static readonly ServiceOperation SendAdvancedEmail = new ServiceOperation("SEND_ADVANCED_EMAIL");
        public static readonly ServiceOperation SendAdvancedEmailByAddress = new ServiceOperation("SEND_ADVANCED_EMAIL_BY_ADDRESS");

        //peer
        public static readonly ServiceOperation AttachPeerProfile = new ServiceOperation("ATTACH_PEER_PROFILE");
        public static readonly ServiceOperation DetachPeer = new ServiceOperation("DETACH_PEER");
        public static readonly ServiceOperation GetPeerProfiles = new ServiceOperation("GET_PEER_PROFILES");

        //presence
        public static readonly ServiceOperation ForcePush = new ServiceOperation("FORCE_PUSH");
        public static readonly ServiceOperation GetPresenceOfFriends = new ServiceOperation("GET_PRESENCE_OF_FRIENDS");
        public static readonly ServiceOperation GetPresenceOfGroup = new ServiceOperation("GET_PRESENCE_OF_GROUP");
        public static readonly ServiceOperation GetPresenceOfUsers = new ServiceOperation("GET_PRESENCE_OF_USERS");
        public static readonly ServiceOperation RegisterListenersForFriends = new ServiceOperation("REGISTER_LISTENERS_FOR_FRIENDS");
        public static readonly ServiceOperation RegisterListenersForGroup = new ServiceOperation("REGISTER_LISTENERS_FOR_GROUP");
        public static readonly ServiceOperation RegisterListenersForProfiles = new ServiceOperation("REGISTER_LISTENERS_FOR_PROFILES");
        public static readonly ServiceOperation SetVisibility = new ServiceOperation("SET_VISIBILITY");
        public static readonly ServiceOperation StopListening = new ServiceOperation("STOP_LISTENING");
        public static readonly ServiceOperation UpdateActivity = new ServiceOperation("UPDATE_ACTIVITY");

        //tournament
        public static readonly ServiceOperation GetTournamentStatus = new ServiceOperation("GET_TOURNAMENT_STATUS");
        public static readonly ServiceOperation GetDivisionInfo = new ServiceOperation("GET_DIVISION_INFO");
        public static readonly ServiceOperation GetMyDivisions = new ServiceOperation("GET_MY_DIVISIONS");
        public static readonly ServiceOperation JoinDivision= new ServiceOperation("JOIN_DIVISION");
        public static readonly ServiceOperation JoinTournament = new ServiceOperation("JOIN_TOURNAMENT");
        public static readonly ServiceOperation LeaveDivisionInstance = new ServiceOperation("LEAVE_DIVISION_INSTANCE");
        public static readonly ServiceOperation LeaveTournament = new ServiceOperation("LEAVE_TOURNAMENT");
        public static readonly ServiceOperation PostTournamentScore = new ServiceOperation("POST_TOURNAMENT_SCORE");
        public static readonly ServiceOperation PostTournamentScoreWithResults = new ServiceOperation("POST_TOURNAMENT_SCORE_WITH_RESULTS");
        public static readonly ServiceOperation ViewCurrentReward = new ServiceOperation("VIEW_CURRENT_REWARD");
        public static readonly ServiceOperation ViewReward = new ServiceOperation("VIEW_REWARD");
        public static readonly ServiceOperation ClaimTournamentReward = new ServiceOperation("CLAIM_TOURNAMENT_REWARD");

        // rtt
        public static readonly ServiceOperation RequestClientConnection = new ServiceOperation("REQUEST_CLIENT_CONNECTION");

        // chat
        public static readonly ServiceOperation ChannelConnect = new ServiceOperation("CHANNEL_CONNECT");
        public static readonly ServiceOperation ChannelDisconnect = new ServiceOperation("CHANNEL_DISCONNECT");
        public static readonly ServiceOperation DeleteChatMessage = new ServiceOperation("DELETE_CHAT_MESSAGE");
        public static readonly ServiceOperation GetChannelId = new ServiceOperation("GET_CHANNEL_ID");
        public static readonly ServiceOperation GetChannelInfo = new ServiceOperation("GET_CHANNEL_INFO");
        public static readonly ServiceOperation GetChatMessage = new ServiceOperation("GET_CHAT_MESSAGE");
        public static readonly ServiceOperation GetRecentChatMessages = new ServiceOperation("GET_RECENT_CHAT_MESSAGES");
        public static readonly ServiceOperation GetSubscribedChannels = new ServiceOperation("GET_SUBSCRIBED_CHANNELS");
        public static readonly ServiceOperation PostChatMessage = new ServiceOperation("POST_CHAT_MESSAGE");
        public static readonly ServiceOperation PostChatMessageSimple = new ServiceOperation("POST_CHAT_MESSAGE_SIMPLE");
        public static readonly ServiceOperation UpdateChatMessage = new ServiceOperation("UPDATE_CHAT_MESSAGE");

        // messaging 
        public static readonly ServiceOperation DeleteMessages = new ServiceOperation("DELETE_MESSAGES");
        public static readonly ServiceOperation GetMessageBoxes = new ServiceOperation("GET_MESSAGE_BOXES");
        public static readonly ServiceOperation GetMessageCounts = new ServiceOperation("GET_MESSAGE_COUNTS");
        public static readonly ServiceOperation GetMessages = new ServiceOperation("GET_MESSAGES");
        public static readonly ServiceOperation GetMessagesPage = new ServiceOperation("GET_MESSAGES_PAGE");
        public static readonly ServiceOperation GetMessagesPageOffset = new ServiceOperation("GET_MESSAGES_PAGE_OFFSET");
        public static readonly ServiceOperation MarkMessagesRead = new ServiceOperation("MARK_MESSAGES_READ");
        public static readonly ServiceOperation SendMessage = new ServiceOperation("SEND_MESSAGE");
        public static readonly ServiceOperation SendMessageSimple = new ServiceOperation("SEND_MESSAGE_SIMPLE");

        // lobby
        public static readonly ServiceOperation FindLobby = new ServiceOperation("FIND_LOBBY");
        public static readonly ServiceOperation CreateLobby = new ServiceOperation("CREATE_LOBBY");
        public static readonly ServiceOperation FindOrCreateLobby = new ServiceOperation("FIND_OR_CREATE_LOBBY");
        public static readonly ServiceOperation GetLobbyData = new ServiceOperation("GET_LOBBY_DATA");
        public static readonly ServiceOperation UpdateReady = new ServiceOperation("UPDATE_READY");
        public static readonly ServiceOperation UpdateLobbyConfig = new ServiceOperation("UPDATE_SETTINGS");
        public static readonly ServiceOperation SwitchTeam = new ServiceOperation("SWITCH_TEAM");
        public static readonly ServiceOperation SendSignal = new ServiceOperation("SEND_SIGNAL");
        public static readonly ServiceOperation JoinLobby = new ServiceOperation("JOIN_LOBBY");
        public static readonly ServiceOperation LeaveLobby = new ServiceOperation("LEAVE_LOBBY");
        public static readonly ServiceOperation RemoveMember = new ServiceOperation("REMOVE_MEMBER");

        private ServiceOperation(string value)
        {
            Value = value;
        }

        public string Value
        {
            get;
            private set;
        }
    }
}
