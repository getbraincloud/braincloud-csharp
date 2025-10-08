// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    public readonly struct OperationParam : System.IEquatable<OperationParam>, System.IComparable<OperationParam>
    {
        #region brainCloud Operation Parameters

        //Push Notification Service - Register Params
        public static readonly OperationParam PushNotificationRegisterParamDeviceType = new("deviceType");
        public static readonly OperationParam PushNotificationRegisterParamDeviceToken = new("deviceToken");

        //Push Notification Service - Send Params
        public static readonly OperationParam PushNotificationSendParamToPlayerId = new("toPlayerId");
        public static readonly OperationParam PushNotificationSendParamProfileId = new("profileId");
        public static readonly OperationParam PushNotificationSendParamMessage = new("message");
        public static readonly OperationParam PushNotificationSendParamNotificationTemplateId = new("notificationTemplateId");
        public static readonly OperationParam PushNotificationSendParamSubstitutions = new("substitutions");
        public static readonly OperationParam PushNotificationSendParamProfileIds = new("profileIds");

        public static readonly OperationParam PushNotificationSendParamFcmContent = new("fcmContent");
        public static readonly OperationParam PushNotificationSendParamIosContent = new("iosContent");
        public static readonly OperationParam PushNotificationSendParamFacebookContent = new("facebookContent");


        public static readonly OperationParam AlertContent = new("alertContent");
        public static readonly OperationParam CustomData = new("customData");

        public static readonly OperationParam StartDateUTC = new("startDateUTC");
        public static readonly OperationParam MinutesFromNow = new("minutesFromNow");

        // Twitter Service - Verify Params
        public static readonly OperationParam TwitterServiceVerifyToken = new("token");
        public static readonly OperationParam TwitterServiceVerifyVerifier = new("verifier");

        // Twitter Service - Tweet Params
        public static readonly OperationParam TwitterServiceTweetToken = new("token");
        public static readonly OperationParam TwitterServiceTweetSecret = new("secret");
        public static readonly OperationParam TwitterServiceTweetTweet = new("tweet");
        public static readonly OperationParam TwitterServiceTweetPic = new("pic");

        public static readonly OperationParam BlockChainConfig = new("blockchainConfig");
        public static readonly OperationParam BlockChainIntegrationId = new("integrationId");
        public static readonly OperationParam BlockChainContext = new("contextJson");
        public static readonly OperationParam PublicKey = new("publicKey");

        // Authenticate Service - Authenticate Params
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationType = new("authenticationType");
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationToken = new("authenticationToken");
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalId = new("externalId");
        public static readonly OperationParam AuthenticateServiceAuthenticateUniversalId = new("universalId");
        public static readonly OperationParam AuthenticateServiceAuthenticateGameId = new("gameId");
        public static readonly OperationParam AuthenticateServiceAuthenticateDeviceId = new("deviceId");
        public static readonly OperationParam AuthenticateServiceAuthenticateForceMergeFlag = new("forceMergeFlag");
        public static readonly OperationParam AuthenticateServiceAuthenticateReleasePlatform = new("releasePlatform");
        public static readonly OperationParam AuthenticateServiceAuthenticateGameVersion = new("gameVersion");
        public static readonly OperationParam AuthenticateServiceAuthenticateBrainCloudVersion = new("clientLibVersion");
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalAuthName = new("externalAuthName");
        public static readonly OperationParam AuthenticateServiceAuthenticateEmailAddress = new("emailAddress");
        public static readonly OperationParam AuthenticateServiceAuthenticateServiceParams = new("serviceParams");
        public static readonly OperationParam AuthenticateServiceAuthenticateTokenTtlInMinutes = new("tokenTtlInMinutes");

        public static readonly OperationParam AuthenticateServiceAuthenticateLevelName = new("levelName");
        public static readonly OperationParam AuthenticateServiceAuthenticatePeerCode = new("peerCode");

        public static readonly OperationParam AuthenticateServiceAuthenticateCountryCode = new("countryCode");
        public static readonly OperationParam AuthenticateServiceAuthenticateLanguageCode = new("languageCode");
        public static readonly OperationParam AuthenticateServiceAuthenticateTimeZoneOffset = new("timeZoneOffset");

        public static readonly OperationParam AuthenticateServiceAuthenticateAuthUpgradeID = new("upgradeAppId");
        public static readonly OperationParam AuthenticateServiceAuthenticateAnonymousId = new("anonymousId");
        public static readonly OperationParam AuthenticateServiceAuthenticateProfileId = new("profileId");
        public static readonly OperationParam AuthenticateServiceAuthenticateForceCreate = new("forceCreate");
        public static readonly OperationParam AuthenticateServiceAuthenticateCompressResponses = new("compressResponses");
        public static readonly OperationParam AuthenticateServicePlayerSessionExpiry = new("playerSessionExpiry");
        public static readonly OperationParam AuthenticateServiceAuthenticateExtraJson = new("extraJson");

        // Authenticate Service - Authenticate Params
        public static readonly OperationParam IdentityServiceExternalId = new("externalId");
        public static readonly OperationParam IdentityServiceAuthenticationType = new("authenticationType");
        public static readonly OperationParam IdentityServiceConfirmAnonymous = new("confirmAnonymous");
        
        public static readonly OperationParam IdentityServiceOldEmailAddress = new("oldEmailAddress");
        public static readonly OperationParam IdentityServiceNewEmailAddress = new("newEmailAddress");
        public static readonly OperationParam IdentityServiceUpdateContactEmail = new("updateContactEmail");


        // Peer
        public static readonly OperationParam Peer = new("peer");

        // Entity Service 
        public static readonly OperationParam EntityServiceEntityId = new("entityId");
        public static readonly OperationParam EntityServiceEntityType = new("entityType");
        public static readonly OperationParam EntityServiceEntitySubtype = new("entitySubtype");
        public static readonly OperationParam EntityServiceData = new("data");
        public static readonly OperationParam EntityServiceAcl = new("acl");
        public static readonly OperationParam EntityServiceFriendData = new("friendData");
        public static readonly OperationParam EntityServiceVersion = new("version");
        public static readonly OperationParam EntityServiceUpdateOps = new("updateOps");
        public static readonly OperationParam EntityServiceTargetPlayerId = new("targetPlayerId");

        // Global Entity Service - Params
        public static readonly OperationParam GlobalEntityServiceEntityId = new("entityId");
        public static readonly OperationParam GlobalEntityServiceEntityType = new("entityType");
        public static readonly OperationParam GlobalEntityServiceIndexedId = new("entityIndexedId");
        public static readonly OperationParam GlobalEntityServiceTimeToLive = new("timeToLive");
        public static readonly OperationParam GlobalEntityServiceData = new("data");
        public static readonly OperationParam GlobalEntityServiceAcl = new("acl");
        public static readonly OperationParam GlobalEntityServiceVersion = new("version");
        public static readonly OperationParam GlobalEntityServiceMaxReturn = new("maxReturn");
        public static readonly OperationParam GlobalEntityServiceWhere = new("where");
        public static readonly OperationParam GlobalEntityServiceOrderBy = new("orderBy");
        public static readonly OperationParam GlobalEntityServiceContext = new("context");
        public static readonly OperationParam GlobalEntityServicePageOffset = new("pageOffset");
        public static readonly OperationParam OwnerId = new("ownerId");

        // Event Service - Send Params
        public static readonly OperationParam EventServiceSendToId = new("toId");
        public static readonly OperationParam EventServiceSendToIds = new("toIds");
        public static readonly OperationParam EventServiceSendEventType = new("eventType");
        public static readonly OperationParam EventServiceSendEventId = new("eventId");
        public static readonly OperationParam EventServiceSendEventData = new("eventData");
        public static readonly OperationParam EventServiceSendRecordLocally = new("recordLocally");

        // Event Service - Update Event Data Params
        public static readonly OperationParam EventServiceUpdateEventDataFromId = new("fromId");
        public static readonly OperationParam EventServiceUpdateEventDataEventId = new("eventId");
        public static readonly OperationParam EventServiceUpdateEventDataData = new("eventData");
        public static readonly OperationParam EvId = new("evId");
        public static readonly OperationParam EventServiceEvIds  = new("evIds");
        public static readonly OperationParam EventServiceDateMillis  = new("dateMillis");
        public static readonly OperationParam EventServiceEventType   = new("eventType");
        
        // Event Service - Delete Incoming Params
        public static readonly OperationParam EventServiceDeleteIncomingEventId = new("eventId");
        public static readonly OperationParam EventServiceDeleteIncomingFromId = new("fromId");

        // Event Service - Delete Sent Params
        public static readonly OperationParam EventServiceDeleteSentEventId = new("eventId");
        public static readonly OperationParam EventServiceDeleteSentToId = new("toId");
        public static readonly OperationParam EventServiceIncludeIncomingEvents = new("includeIncomingEvents");
        public static readonly OperationParam EventServiceIncludeSentEvents = new("includeSentEvents");

        // Friend Service - Params
        public static readonly OperationParam FriendServiceEntityId = new("entityId");
        public static readonly OperationParam FriendServiceExternalId = new("externalId");
        public static readonly OperationParam FriendServiceExternalIds = new("externalIds");
        public static readonly OperationParam FriendServiceProfileId = new("profileId");
        public static readonly OperationParam FriendServiceFriendId = new("friendId");
        public static readonly OperationParam FriendServiceAuthenticationType = new("authenticationType");
        public static readonly OperationParam ExternalAuthType = new("externalAuthType");
        public static readonly OperationParam FriendServiceEntityType = new("entityType");
        public static readonly OperationParam FriendServiceEntitySubtype = new("entitySubtype");
        public static readonly OperationParam FriendServiceIncludeSummaryData = new("includeSummaryData");
        public static readonly OperationParam FriendServiceFriendPlatform = new("friendPlatform");
        public static readonly OperationParam FriendServiceProfileIds = new("profileIds");
        public static readonly OperationParam FriendServiceMode = new("mode");

        // Friend Service operations
        //public static readonly Operation FriendServiceReadFriends = new Operation("READ_FRIENDS");

        // Friend Service - Read Player State Params
        public static readonly OperationParam FriendServiceReadPlayerStateFriendId = new("friendId");
        public static readonly OperationParam FriendServiceSearchText = new("searchText");
        public static readonly OperationParam FriendServiceMaxResults = new("maxResults");


        // Friend Data Service - Read Friends Params (C++ only?)
        //public static readonly Operation FriendDataServiceReadFriends = new Operation("");
        //friendIdList;
        //friendIdCount;

        //Achievements Event Data Params
        public static readonly OperationParam GamificationServiceAchievementsName = new("achievements");
        public static readonly OperationParam GamificationServiceAchievementsData = new("data");
        public static readonly OperationParam GamificationServiceAchievementsGranted = new("achievementsGranted");
        public static readonly OperationParam GamificationServiceCategory = new("category");
        public static readonly OperationParam GamificationServiceMilestones = new("milestones");
        public static readonly OperationParam GamificationServiceIncludeMetaData = new("includeMetaData");

        // Player Statistic Event Params
        public static readonly OperationParam PlayerStatisticEventServiceEventName = new("eventName");
        public static readonly OperationParam PlayerStatisticEventServiceEventMultiplier = new("eventMultiplier");
        public static readonly OperationParam PlayerStatisticEventServiceEvents = new("events");

        // Presence Params
        public static readonly OperationParam PresenceServicePlatform = new("platform");
        public static readonly OperationParam PresenceServiceIncludeOffline = new("includeOffline");
        public static readonly OperationParam PresenceServiceGroupId = new("groupId");
        public static readonly OperationParam PresenceServiceProfileIds = new("profileIds");
        public static readonly OperationParam PresenceServiceBidirectional = new("bidirectional");
        public static readonly OperationParam PresenceServiceVisibile = new("visible");
        public static readonly OperationParam PresenceServiceActivity = new("activity");

        // Player State Service - Read Params
        public static readonly OperationParam PlayerStateServiceReadEntitySubtype = new("entitySubType");

        // Player State Service - Update Summary Params
        public static readonly OperationParam PlayerStateServiceUpdateSummaryFriendData = new("summaryFriendData");
        public static readonly OperationParam PlayerStateServiceUpdateNameData = new("playerName");
        public static readonly OperationParam PlayerStateServiceTimeZoneOffset = new("timeZoneOffset");
        public static readonly OperationParam PlayerStateServiceLanguageCode = new("languageCode");

        // Player State Service - Atributes
        public static readonly OperationParam PlayerStateServiceAttributes = new("attributes");
        public static readonly OperationParam PlayerStateServiceWipeExisting = new("wipeExisting");

        public static readonly OperationParam PlayerStateServiceIncludeSummaryData = new("includePlayerSummaryData");

        // Player State Service - UPDATE_PICTURE_URL
        public static readonly OperationParam PlayerStateServicePlayerPictureUrl = new("playerPictureUrl");
        public static readonly OperationParam PlayerStateServiceContactEmail = new("contactEmail");

        // Player State Service - Reset Params
        //public static readonly Operation PlayerStateServiceReset = new Operation("");
        

        // Player Statistics Service - Update Increment Params
        public static readonly OperationParam PlayerStatisticsServiceStats = new("statistics");
        public static readonly OperationParam PlayerStatisticsServiceStatNames = new("statNames");
        public static readonly OperationParam PlayerStatisticsExperiencePoints = new("xp_points");

        // Player Statistics Service - Status Param
        public static readonly OperationParam PlayerStateServiceStatusName = new("statusName");
        
        // Player Statistics Service - Extend User Status Params
        public static readonly OperationParam PlayerStateServiceAdditionalSecs = new("additionalSecs");
        public static readonly OperationParam PlayerStateServiceDetails = new("details");

        public static readonly OperationParam PlayerStateServiceDurationSecs = new("durationSecs");

        // Player Statistics Service - Read Params
        public static readonly OperationParam PlayerStatisticsServiceReadEntitySubType = new("entitySubType");

        //public static readonly Operation PlayerStatisticsServiceDelete = new Operation("DELETE");

        // Push Notification Service operations (C++ only??)
        //public static readonly Operation PushNotificationServiceCreate = new Operation("CREATE");
        //public static readonly Operation PushNotificationServiceRegister = new Operation("REGISTER");

        // Social Leaderboard Service - general parameters
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardId = new("leaderboardId");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardIds = new("leaderboardIds");
        public static readonly OperationParam SocialLeaderboardServiceReplaceName = new("replaceName");
        public static readonly OperationParam SocialLeaderboardServiceScore = new("score");
        public static readonly OperationParam SocialLeaderboardServiceScoreData = new("scoreData");
        public static readonly OperationParam SocialLeaderboardServiceConfigJson = new("configJson");
        public static readonly OperationParam SocialLeaderboardServiceData = new("data");
        public static readonly OperationParam SocialLeaderboardServiceEventName = new("eventName");
        public static readonly OperationParam SocialLeaderboardServiceEventMultiplier = new("eventMultiplier");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardType = new("leaderboardType");
        public static readonly OperationParam SocialLeaderboardServiceRotationType = new("rotationType");
        public static readonly OperationParam SocialLeaderboardServiceRotationReset = new("rotationReset");
        public static readonly OperationParam SocialLeaderboardServiceRetainedCount = new("retainedCount");
        public static readonly OperationParam NumDaysToRotate = new("numDaysToRotate");
        public static readonly OperationParam SocialLeaderboardServiceFetchType = new("fetchType");
        public static readonly OperationParam SocialLeaderboardServiceMaxResults = new("maxResults");
        public static readonly OperationParam SocialLeaderboardServiceSort = new("sort");
        public static readonly OperationParam SocialLeaderboardServiceStartIndex = new("startIndex");
        public static readonly OperationParam SocialLeaderboardServiceEndIndex = new("endIndex");
        public static readonly OperationParam SocialLeaderboardServiceBeforeCount = new("beforeCount");
        public static readonly OperationParam SocialLeaderboardServiceAfterCount = new("afterCount");
        public static readonly OperationParam SocialLeaderboardServiceIncludeLeaderboardSize = new("includeLeaderboardSize");
        public static readonly OperationParam SocialLeaderboardServiceVersionId = new("versionId");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardResultCount = new("leaderboardResultCount");
        public static readonly OperationParam SocialLeaderboardServiceGroupId = new("groupId");
        public static readonly OperationParam SocialLeaderboardServiceProfileIds = new("profileIds");
        public static readonly OperationParam SocialLeaderboardServiceRotationResetTime = new("rotationResetTime");


        // Social Leaderboard Service - Reset Score Params
        //public static readonly Operation SocialLeaderboardServiceResetScore = new Operation("");

        // Product Service
        public static readonly OperationParam ProductServiceCurrencyId = new("vc_id");
        public static readonly OperationParam ProductServiceCurrencyAmount = new("vc_amount");

        // AppStore 
        public static readonly OperationParam AppStoreServiceStoreId = new("storeId");
        public static readonly OperationParam AppStoreServiceIAPId = new("iapId");
        public static readonly OperationParam AppStoreServiceReceiptData = new("receiptData");
        public static readonly OperationParam AppStoreServicePurchaseData = new("purchaseData");
        public static readonly OperationParam AppStoreServiceTransactionId = new("transactionId");
        public static readonly OperationParam AppStoreServiceTransactionData = new("transactionData");
        public static readonly OperationParam AppStoreServicePriceInfoCriteria = new("priceInfoCriteria");
        public static readonly OperationParam AppStoreServiceUserCurrency = new("userCurrency");
        public static readonly OperationParam AppStoreServiceCategory = new("category");
        public static readonly OperationParam AppStoreServicePayload = new("payload");

        // Virtual Currency Service
        public static readonly OperationParam VirtualCurrencyServiceCurrencyId = new("vcId");
        public static readonly OperationParam VirtualCurrencyServiceCurrencyAmount = new("vcAmount");

        // Product Service - Get Inventory Params
        public static readonly OperationParam ProductServiceGetInventoryPlatform = new("platform");
        public static readonly OperationParam ProductServiceGetInventoryUserCurrency = new("user_currency");
        public static readonly OperationParam ProductServiceGetInventoryCategory = new("category");

        // Product Service - Op Cash In Receipt Params
        public static readonly OperationParam ProductServiceOpCashInReceiptReceipt = new("receipt"); //C++ only
        public static readonly OperationParam ProductServiceOpCashInReceiptUrl = new("url"); //C++ only

        // Product Service - Reset Player VC Params
        //public static readonly OperationParam ProductServiceResetPlayerVC = new("");

        // Heartbeat Service - Params
        //public static readonly OperationParam HeartbeatService = new("");

        // Time Service - Params
        //public static readonly OperationParam TimeService = new("");

        // Server Time Service - Read Params
        public static readonly OperationParam ServerTimeServiceRead = new("");

        // data creation parms
        public static readonly OperationParam ServiceMessageService = new("service");
        public static readonly OperationParam ServiceMessageOperation = new("operation");
        public static readonly OperationParam ServiceMessageData = new("data");

        // data bundle creation parms
        public static readonly OperationParam ServiceMessagePacketId = new("packetId");
        public static readonly OperationParam ServiceMessageSessionId = new("sessionId");
        public static readonly OperationParam ServiceMessageGameId = new("gameId");
        public static readonly OperationParam ServiceMessageMessages = new("messages");
        public static readonly OperationParam ProfileId = new("profileId");

        // Error Params
        public static readonly OperationParam ServiceMessageReasonCode = new("reason_code");
        public static readonly OperationParam ServiceMessageStatusMessage = new("status_message");

        public static readonly OperationParam DeviceRegistrationTypeIos = new("IOS");
        public static readonly OperationParam DeviceRegistrationTypeAndroid = new("ANG");

        public static readonly OperationParam ScriptServiceRunScriptName = new("scriptName");
        public static readonly OperationParam ScriptServiceRunScriptData = new("scriptData");
        public static readonly OperationParam ScriptServiceStartDateUTC = new("startDateUTC");
        public static readonly OperationParam ScriptServiceStartMinutesFromNow = new("minutesFromNow");
        public static readonly OperationParam ScriptServiceParentLevel = new("parentLevel");
        public static readonly OperationParam ScriptServiceJobId = new("jobId");

        public static readonly OperationParam MatchMakingServicePlayerRating = new("playerRating");
        public static readonly OperationParam MatchMakingServiceMinutes = new("minutes");
        public static readonly OperationParam MatchMakingServiceRangeDelta = new("rangeDelta");
        public static readonly OperationParam MatchMakingServiceNumMatches = new("numMatches");
        public static readonly OperationParam MatchMakingServiceAttributes = new("attributes");
        public static readonly OperationParam MatchMakingServiceExtraParams = new("extraParms");
        public static readonly OperationParam MatchMakingServicePlayerId = new("playerId");
        public static readonly OperationParam MatchMakingServicePlaybackStreamId = new("playbackStreamId");

        public static readonly OperationParam OfflineMatchServicePlayerId = new("playerId");
        public static readonly OperationParam OfflineMatchServiceRangeDelta = new("rangeDelta");
        public static readonly OperationParam OfflineMatchServicePlaybackStreamId = new("playbackStreamId");

        public static readonly OperationParam PlaybackStreamServiceTargetPlayerId = new("targetPlayerId");
        public static readonly OperationParam PlaybackStreamServiceInitiatingPlayerId = new("initiatingPlayerId");
        public static readonly OperationParam PlaybackStreamServiceMaxNumberOfStreams = new("maxNumStreams");
        public static readonly OperationParam PlaybackStreamServiceIncludeSharedData = new("includeSharedData");
        public static readonly OperationParam PlaybackStreamServicePlaybackStreamId = new("playbackStreamId");
        public static readonly OperationParam PlaybackStreamServiceEventData = new("eventData");
        public static readonly OperationParam PlaybackStreamServiceSummary = new("summary");
        public static readonly OperationParam PlaybackStreamServiceNumDays = new("numDays");

        public static readonly OperationParam ProductServiceTransId = new("transId");
        public static readonly OperationParam ProductServiceOrderId = new("orderId");
        public static readonly OperationParam ProductServiceProductId = new("productId");
        public static readonly OperationParam ProductServiceLanguage = new("language");
        public static readonly OperationParam ProductServiceItemId = new("itemId");
        public static readonly OperationParam ProductServiceReceipt = new("receipt");
        public static readonly OperationParam ProductServiceSignedRequest = new("signed_request");
        public static readonly OperationParam ProductServiceToken = new("token");

        //S3 Service
        public static readonly OperationParam S3HandlingServiceFileCategory = new("category");
        public static readonly OperationParam S3HandlingServiceFileDetails = new("fileDetails");
        public static readonly OperationParam S3HandlingServiceFileId = new("fileId");

        //Shared Identity
        public static readonly OperationParam IdentityServiceForceSingleton = new("forceSingleton");

        //RedemptionCode
        public static readonly OperationParam RedemptionCodeServiceScanCode = new("scanCode");
        public static readonly OperationParam RedemptionCodeServiceCodeType = new("codeType");
        public static readonly OperationParam RedemptionCodeServiceCustomRedemptionInfo = new("customRedemptionInfo");

        //DataStream
        public static readonly OperationParam DataStreamEventName = new("eventName");
        public static readonly OperationParam DataStreamEventProperties = new("eventProperties");
        public static readonly OperationParam DataStreamCrashType = new("crashType");
        public static readonly OperationParam DataStreamErrorMsg = new("errorMsg");
        public static readonly OperationParam DataStreamCrashInfo = new("crashJson");
        public static readonly OperationParam DataStreamCrashLog = new("crashLog");
        public static readonly OperationParam DataStreamUserName = new("userName");
        public static readonly OperationParam DataStreamUserEmail = new("userEmail");
        public static readonly OperationParam DataStreamUserNotes = new("userNotes");
        public static readonly OperationParam DataStreamUserSubmitted = new("userSubmitted");

        // Profanity
        public static readonly OperationParam ProfanityText = new("text");
        public static readonly OperationParam ProfanityReplaceSymbol = new("replaceSymbol");
        public static readonly OperationParam ProfanityFlagEmail = new("flagEmail");
        public static readonly OperationParam ProfanityFlagPhone = new("flagPhone");
        public static readonly OperationParam ProfanityFlagUrls = new("flagUrls");
        public static readonly OperationParam ProfanityLanguages = new("languages");

        //File upload
        public static readonly OperationParam UploadLocalPath = new("localPath");
        public static readonly OperationParam UploadCloudPath = new("cloudPath");
        public static readonly OperationParam UploadCloudFilename = new("cloudFilename");
        public static readonly OperationParam UploadShareable = new("shareable");
        public static readonly OperationParam UploadReplaceIfExists = new("replaceIfExists");
        public static readonly OperationParam UploadFileSize = new("fileSize");
        public static readonly OperationParam UploadRecurse = new("recurse");
        public static readonly OperationParam UploadPath = new("path");

        //group
        public static readonly OperationParam GroupId = new("groupId");
        public static readonly OperationParam GroupProfileId = new("profileId");
        public static readonly OperationParam GroupRole = new("role");
        public static readonly OperationParam GroupAttributes = new("attributes");
        public static readonly OperationParam GroupName = new("name");
        public static readonly OperationParam GroupType = new("groupType");
        public static readonly OperationParam GroupTypes = new("groupTypes");
        public static readonly OperationParam GroupEntityType = new("entityType");
        public static readonly OperationParam GroupIsOpenGroup = new("isOpenGroup");
        public static readonly OperationParam GroupAcl = new("acl");
        public static readonly OperationParam GroupData = new("data");
        public static readonly OperationParam GroupOwnerAttributes = new("ownerAttributes");
        public static readonly OperationParam GroupDefaultMemberAttributes = new("defaultMemberAttributes");
        public static readonly OperationParam GroupIsOwnedByGroupMember = new("isOwnedByGroupMember");
        public static readonly OperationParam GroupSummaryData = new("summaryData");
        public static readonly OperationParam GroupEntityId = new("entityId");
        public static readonly OperationParam GroupVersion = new("version");
        public static readonly OperationParam GroupContext = new("context");
        public static readonly OperationParam GroupPageOffset = new("pageOffset");
        public static readonly OperationParam GroupAutoJoinStrategy = new("autoJoinStrategy");
        public static readonly OperationParam GroupWhere = new("where");
        public static readonly OperationParam GroupMaxReturn = new("maxReturn");
        
        //group file
        public static readonly OperationParam FolderPath = new("folderPath");
        public static readonly OperationParam FileName = new("filename");
        public static readonly OperationParam FullPathFilename = new("fullPathFilename");
        public static readonly OperationParam FileId = new("fileId");
        public static readonly OperationParam Version = new("version");
        public static readonly OperationParam NewTreeId = new("newTreeId");
        public static readonly OperationParam TreeVersion = new("treeVersion");
        public static readonly OperationParam NewFilename = new("newFilename");
        public static readonly OperationParam OverwriteIfPresent = new("overwriteIfPresent");
        public static readonly OperationParam Recurse = new("recurse");
        public static readonly OperationParam UserCloudPath = new("userCloudPath");
        public static readonly OperationParam UserCloudFilename = new("userCloudFilename");
        public static readonly OperationParam GroupTreeId = new("groupTreeId");
        public static readonly OperationParam GroupFilename = new("groupFilename");
        public static readonly OperationParam GroupFileACL = new("groupFileAcl");
        public static readonly OperationParam NewACL = new("newAcl");


        //GlobalFile
        public static readonly OperationParam GlobalFileServiceFileId = new("fileId");
        public static readonly OperationParam GlobalFileServiceFolderPath = new("folderPath");
        public static readonly OperationParam GlobalFileServiceFileName = new("filename");
        public static readonly OperationParam GlobalFileServiceRecurse = new("recurse");

        //mail
        public static readonly OperationParam Subject = new("subject");
        public static readonly OperationParam Body = new("body");
        public static readonly OperationParam ServiceParams = new("serviceParams");
        public static readonly OperationParam EmailAddress = new("emailAddress");
        public static readonly OperationParam EmailAddresses = new("emailAddresses");

        public static readonly OperationParam LeaderboardId = new("leaderboardId");
        public static readonly OperationParam DivSetId = new("divSetId");
        public static readonly OperationParam VersionId = new("versionId");
        public static readonly OperationParam TournamentCode = new("tournamentCode");
        public static readonly OperationParam InitialScore = new("initialScore");
        public static readonly OperationParam Score = new("score");
        public static readonly OperationParam RoundStartedEpoch = new("roundStartedEpoch");
        public static readonly OperationParam Data = new("data");

        // chat
        public static readonly OperationParam ChatChannelId = new("channelId");
        public static readonly OperationParam ChatMaxReturn = new("maxReturn");
        public static readonly OperationParam ChatMessageId = new("msgId");
        public static readonly OperationParam ChatVersion = new("version");
        
        public static readonly OperationParam ChatChannelType = new("channelType");
        public static readonly OperationParam ChatChannelSubId = new("channelSubId");
        public static readonly OperationParam ChatContent = new("content");
        public static readonly OperationParam ChatText = new("text");

        public static readonly OperationParam ChatRich = new("rich");
        public static readonly OperationParam ChatRecordInHistory = new("recordInHistory");

        // TODO:: do we enumerate these ? [smrj]
        // chat channel types 
        public static readonly OperationParam AllChannelType = new("all");
        public static readonly OperationParam GlobalChannelType = new("gl");
        public static readonly OperationParam GroupChannelType = new("gr");

        // messaging
        public static readonly OperationParam MessagingMessageBox = new("msgbox");
        public static readonly OperationParam MessagingMessageIds = new("msgIds");
        public static readonly OperationParam MessagingMarkAsRead = new("markAsRead");
        public static readonly OperationParam MessagingContext = new("context");
        public static readonly OperationParam MessagingPageOffset = new("pageOffset");
        public static readonly OperationParam MessagingFromName = new("fromName");
        public static readonly OperationParam MessagingToProfileIds = new("toProfileIds");
        public static readonly OperationParam MessagingContent = new("contentJson");
        public static readonly OperationParam MessagingSubject = new("subject");
        public static readonly OperationParam MessagingText = new("text");

        public static readonly OperationParam InboxMessageType = new("inbox");
        public static readonly OperationParam SentMessageType = new("sent");

        // lobby
        public static readonly OperationParam EntryId = new("entryId");
        public static readonly OperationParam LobbyRoomType = new("lobbyType");
        public static readonly OperationParam LobbyTypes = new("lobbyTypes");
        public static readonly OperationParam LobbyRating = new("rating");
        public static readonly OperationParam LobbyAlgorithm = new("algo");
        public static readonly OperationParam LobbyMaxSteps = new("maxSteps");
        public static readonly OperationParam LobbyStrategy = new("strategy");
        public static readonly OperationParam LobbyAlignment = new("alignment");
        public static readonly OperationParam LobbyRanges = new("ranges");
        public static readonly OperationParam LobbyFilterJson = new("filterJson");
        public static readonly OperationParam LobbySettings = new("settings");
        public static readonly OperationParam LobbyTimeoutSeconds = new("timeoutSecs");
        public static readonly OperationParam LobbyIsReady = new("isReady");
        public static readonly OperationParam LobbyOtherUserCxIds = new("otherUserCxIds");
        public static readonly OperationParam LobbyExtraJson = new("extraJson");
        public static readonly OperationParam LobbyTeamCode = new("teamCode");
        public static readonly OperationParam LobbyIdentifier = new("lobbyId");
        public static readonly OperationParam LobbyToTeamName = new("toTeamCode");
        public static readonly OperationParam LobbySignalData = new("signalData");
        public static readonly OperationParam LobbyConnectionId = new("cxId");
        public static readonly OperationParam PingData = new("pingData");
        public static readonly OperationParam LobbyMinRating = new("minRating");
        public static readonly OperationParam LobbyMaxRating = new("maxRating");

        public static readonly OperationParam CompoundAlgos = new("algos");
        public static readonly OperationParam CompoundRanges = new("compound-ranges");
        public static readonly OperationParam LobbyCritera = new("criteriaJson");
        public static readonly OperationParam Critera = new("criteria");
        public static readonly OperationParam CriteraPing = new("ping");
        public static readonly OperationParam CriteraRating = new("rating");
        public static readonly OperationParam StrategyRangedPercent = new("ranged-percent");
        public static readonly OperationParam StrategyRangedAbsolute = new("ranged-absolute");
        public static readonly OperationParam StrategyAbsolute = new("absolute");
        public static readonly OperationParam StrategyCompound = new("compound");
        public static readonly OperationParam AlignmentCenter = new("center");
        //custom entity
        public static readonly OperationParam CustomEntityServiceEntityType = new("entityType");
        public static readonly OperationParam CustomEntityServiceDeleteCriteria = new("deleteCriteria");
        public static readonly OperationParam CustomEntityServiceEntityId = new("entityId");
        public static readonly OperationParam CustomEntityServiceVersion = new("version");
        public static readonly OperationParam CustomEntityServiceFieldsJson = new("fieldsJson");
        public static readonly OperationParam CustomEntityServiceWhereJson = new("whereJson");
        public static readonly OperationParam CustomEntityServiceRowsPerPage = new("rowsPerPage");
        public static readonly OperationParam CustomEntityServiceSearchJson = new("searchJson");
        public static readonly OperationParam CustomEntityServiceSortJson = new("sortJson");
        public static readonly OperationParam CustomEntityServiceDoCount = new("doCount");
        public static readonly OperationParam CustomEntityServiceContext = new("context");
        public static readonly OperationParam CustomEntityServicePageOffset= new("pageOffset");
        public static readonly OperationParam CustomEntityServiceTimeToLive = new("timeToLive");
        public static readonly OperationParam CustomEntityServiceAcl = new("acl");
        public static readonly OperationParam CustomEntityServiceDataJson = new("dataJson");
        public static readonly OperationParam CustomEntityServiceIsOwned = new("isOwned");
        public static readonly OperationParam CustomEntityServiceMaxReturn = new("maxReturn");
        public static readonly OperationParam CustomEntityServiceShardKeyJson = new("shardKeyJson");

        //item catalog
        public static readonly OperationParam ItemCatalogServiceDefId = new("defId");
        public static readonly OperationParam ItemCatalogServiceContext = new("context");
        public static readonly OperationParam ItemCatalogServicePageOffset = new("pageOffset");

        //userInventory
        public static readonly OperationParam UserItemsServiceDefId = new("defId");
        public static readonly OperationParam UserItemsServiceQuantity = new("quantity");
        public static readonly OperationParam UserItemsServiceIncludeDef = new("includeDef");
        public static readonly OperationParam UserItemsServiceItemId = new("itemId");
        public static readonly OperationParam UserItemsServiceCriteria = new("criteria");
        public static readonly OperationParam UserItemsServiceContext = new("context");
        public static readonly OperationParam UserItemsServicePageOffset = new("pageOffset");
        public static readonly OperationParam UserItemsServiceVersion = new("version");
        public static readonly OperationParam UserItemsServiceImmediate = new("immediate");
        public static readonly OperationParam UserItemsServiceProfileId = new("profileId");
        public static readonly OperationParam UserItemsServiceShopId = new("shopId");
        public static readonly OperationParam UserItemsServiceNewItemData = new("newItemData");

        //global app
        public static readonly OperationParam GlobalAppPropertyNames = new("propertyNames");
        public static readonly OperationParam GlobalAppCategories = new("categories");

        #endregion

        private OperationParam(string value)
        {
            Value = value;
        }

        public readonly string Value;

        #region Overrides and Operators

        public readonly override bool Equals(object obj)
        {
            if (obj is not OperationParam s)
                return false;

            return Equals(s);
        }

        public readonly bool Equals(OperationParam other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public readonly int CompareTo(OperationParam other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public readonly override int GetHashCode() => Value.GetHashCode();

        public readonly override string ToString() => Value;

        public static implicit operator string(OperationParam v) => v.Value;

        public static bool operator ==(OperationParam v1, OperationParam v2) => v1.Equals(v2);

        public static bool operator !=(OperationParam v1, OperationParam v2) => !(v1 == v2);

        public static bool operator >(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
