//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{
    internal sealed class OperationParam
    {
        //Push Notification Service - Register Params
        public static readonly OperationParam PushNotificationRegisterParamDeviceType = new OperationParam("deviceType");
        public static readonly OperationParam PushNotificationRegisterParamDeviceToken = new OperationParam("deviceToken");

        //Push Notification Service - Send Params
        public static readonly OperationParam PushNotificationSendParamToPlayerId = new OperationParam("toPlayerId");
        public static readonly OperationParam PushNotificationSendParamProfileId = new OperationParam("profileId");
        public static readonly OperationParam PushNotificationSendParamMessage = new OperationParam("message");
        public static readonly OperationParam PushNotificationSendParamNotificationTemplateId = new OperationParam("notificationTemplateId");
        public static readonly OperationParam PushNotificationSendParamSubstitutions = new OperationParam("substitutions");
        public static readonly OperationParam PushNotificationSendParamProfileIds = new OperationParam("profileIds");

        public static readonly OperationParam PushNotificationSendParamFcmContent = new OperationParam("fcmContent");
        public static readonly OperationParam PushNotificationSendParamIosContent = new OperationParam("iosContent");
        public static readonly OperationParam PushNotificationSendParamFacebookContent = new OperationParam("facebookContent");


        public static readonly OperationParam AlertContent = new OperationParam("alertContent");
        public static readonly OperationParam CustomData = new OperationParam("customData");

        public static readonly OperationParam StartDateUTC = new OperationParam("startDateUTC");
        public static readonly OperationParam MinutesFromNow = new OperationParam("minutesFromNow");

        // Twitter Service - Verify Params
        public static readonly OperationParam TwitterServiceVerifyToken = new OperationParam("token");
        public static readonly OperationParam TwitterServiceVerifyVerifier = new OperationParam("verifier");

        // Twitter Service - Tweet Params
        public static readonly OperationParam TwitterServiceTweetToken = new OperationParam("token");
        public static readonly OperationParam TwitterServiceTweetSecret = new OperationParam("secret");
        public static readonly OperationParam TwitterServiceTweetTweet = new OperationParam("tweet");
        public static readonly OperationParam TwitterServiceTweetPic = new OperationParam("pic");


        // Authenticate Service - Authenticate Params
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationType = new OperationParam("authenticationType");
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationToken = new OperationParam("authenticationToken");
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalId = new OperationParam("externalId");
        public static readonly OperationParam AuthenticateServiceAuthenticateGameId = new OperationParam("gameId");
        public static readonly OperationParam AuthenticateServiceAuthenticateDeviceId = new OperationParam("deviceId");
        public static readonly OperationParam AuthenticateServiceAuthenticateForceMergeFlag = new OperationParam("forceMergeFlag");
        public static readonly OperationParam AuthenticateServiceAuthenticateReleasePlatform = new OperationParam("releasePlatform");
        public static readonly OperationParam AuthenticateServiceAuthenticateGameVersion = new OperationParam("gameVersion");
        public static readonly OperationParam AuthenticateServiceAuthenticateBrainCloudVersion = new OperationParam("clientLibVersion");
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalAuthName = new OperationParam("externalAuthName");

        public static readonly OperationParam AuthenticateServiceAuthenticateLevelName = new OperationParam("levelName");
        public static readonly OperationParam AuthenticateServiceAuthenticatePeerCode = new OperationParam("peerCode");

        public static readonly OperationParam AuthenticateServiceAuthenticateCountryCode = new OperationParam("countryCode");
        public static readonly OperationParam AuthenticateServiceAuthenticateLanguageCode = new OperationParam("languageCode");
        public static readonly OperationParam AuthenticateServiceAuthenticateTimeZoneOffset = new OperationParam("timeZoneOffset");

        public static readonly OperationParam AuthenticateServiceAuthenticateAuthUpgradeID = new OperationParam("upgradeAppId");
        public static readonly OperationParam AuthenticateServiceAuthenticateAnonymousId = new OperationParam("anonymousId");
        public static readonly OperationParam AuthenticateServiceAuthenticateProfileId = new OperationParam("profileId");
        public static readonly OperationParam AuthenticateServiceAuthenticateForceCreate = new OperationParam("forceCreate");
        public static readonly OperationParam AuthenticateServicePlayerSessionExpiry = new OperationParam("playerSessionExpiry");

        // Authenticate Service - Authenticate Params
        public static readonly OperationParam IdentityServiceExternalId = new OperationParam("externalId");
        public static readonly OperationParam IdentityServiceAuthenticationType = new OperationParam("authenticationType");
        public static readonly OperationParam IdentityServiceConfirmAnonymous = new OperationParam("confirmAnonymous");
        
        public static readonly OperationParam IdentityServiceOldEmailAddress = new OperationParam("oldEmailAddress");
        public static readonly OperationParam IdentityServiceNewEmailAddress = new OperationParam("newEmailAddress");
        public static readonly OperationParam IdentityServiceUpdateContactEmail = new OperationParam("updateContactEmail");


        // Peer
        public static readonly OperationParam Peer = new OperationParam("peer");

        // Entity Service 
        public static readonly OperationParam EntityServiceEntityId = new OperationParam("entityId");
        public static readonly OperationParam EntityServiceEntityType = new OperationParam("entityType");
        public static readonly OperationParam EntityServiceEntitySubtype = new OperationParam("entitySubtype");
        public static readonly OperationParam EntityServiceData = new OperationParam("data");
        public static readonly OperationParam EntityServiceAcl = new OperationParam("acl");
        public static readonly OperationParam EntityServiceFriendData = new OperationParam("friendData");
        public static readonly OperationParam EntityServiceVersion = new OperationParam("version");
        public static readonly OperationParam EntityServiceUpdateOps = new OperationParam("updateOps");
        public static readonly OperationParam EntityServiceTargetPlayerId = new OperationParam("targetPlayerId");

        // Global Entity Service - Params
        public static readonly OperationParam GlobalEntityServiceEntityId = new OperationParam("entityId");
        public static readonly OperationParam GlobalEntityServiceEntityType = new OperationParam("entityType");
        public static readonly OperationParam GlobalEntityServiceIndexedId = new OperationParam("entityIndexedId");
        public static readonly OperationParam GlobalEntityServiceTimeToLive = new OperationParam("timeToLive");
        public static readonly OperationParam GlobalEntityServiceData = new OperationParam("data");
        public static readonly OperationParam GlobalEntityServiceAcl = new OperationParam("acl");
        public static readonly OperationParam GlobalEntityServiceVersion = new OperationParam("version");
        public static readonly OperationParam GlobalEntityServiceMaxReturn = new OperationParam("maxReturn");
        public static readonly OperationParam GlobalEntityServiceWhere = new OperationParam("where");
        public static readonly OperationParam GlobalEntityServiceOrderBy = new OperationParam("orderBy");
        public static readonly OperationParam GlobalEntityServiceContext = new OperationParam("context");
        public static readonly OperationParam GlobalEntityServicePageOffset = new OperationParam("pageOffset");
        public static readonly OperationParam OwnerId = new OperationParam("ownerId");

        // Event Service - Send Params
        public static readonly OperationParam EventServiceSendToId = new OperationParam("toId");
        public static readonly OperationParam EventServiceSendEventType = new OperationParam("eventType");
        public static readonly OperationParam EventServiceSendEventId = new OperationParam("eventId");
        public static readonly OperationParam EventServiceSendEventData = new OperationParam("eventData");
        public static readonly OperationParam EventServiceSendRecordLocally = new OperationParam("recordLocally");

        // Event Service - Update Event Data Params
        public static readonly OperationParam EventServiceUpdateEventDataFromId = new OperationParam("fromId");
        public static readonly OperationParam EventServiceUpdateEventDataEventId = new OperationParam("eventId");
        public static readonly OperationParam EventServiceUpdateEventDataData = new OperationParam("eventData");
        public static readonly OperationParam EvId = new OperationParam("evId");

        // Event Service - Delete Incoming Params
        public static readonly OperationParam EventServiceDeleteIncomingEventId = new OperationParam("eventId");
        public static readonly OperationParam EventServiceDeleteIncomingFromId = new OperationParam("fromId");

        // Event Service - Delete Sent Params
        public static readonly OperationParam EventServiceDeleteSentEventId = new OperationParam("eventId");
        public static readonly OperationParam EventServiceDeleteSentToId = new OperationParam("toId");
        public static readonly OperationParam EventServiceIncludeIncomingEvents = new OperationParam("includeIncomingEvents");
        public static readonly OperationParam EventServiceIncludeSentEvents = new OperationParam("includeSentEvents");

        // Friend Service - Params
        public static readonly OperationParam FriendServiceEntityId = new OperationParam("entityId");
        public static readonly OperationParam FriendServiceExternalId = new OperationParam("externalId");
        public static readonly OperationParam FriendServiceProfileId = new OperationParam("profileId");
        public static readonly OperationParam FriendServiceFriendId = new OperationParam("friendId");
        public static readonly OperationParam FriendServiceAuthenticationType = new OperationParam("authenticationType");
        public static readonly OperationParam ExternalAuthType = new OperationParam("externalAuthType");
        public static readonly OperationParam FriendServiceEntityType = new OperationParam("entityType");
        public static readonly OperationParam FriendServiceEntitySubtype = new OperationParam("entitySubtype");
        public static readonly OperationParam FriendServiceIncludeSummaryData = new OperationParam("includeSummaryData");
        public static readonly OperationParam FriendServiceFriendPlatform = new OperationParam("friendPlatform");
        public static readonly OperationParam FriendServiceProfileIds = new OperationParam("profileIds");

        // Friend Service operations
        //public static readonly Operation FriendServiceReadFriends = new Operation("READ_FRIENDS");

        // Friend Service - Read Player State Params
        public static readonly OperationParam FriendServiceReadPlayerStateFriendId = new OperationParam("friendId");
        public static readonly OperationParam FriendServiceSearchText = new OperationParam("searchText");
        public static readonly OperationParam FriendServiceMaxResults = new OperationParam("maxResults");


        // Friend Data Service - Read Friends Params (C++ only?)
        //public static readonly Operation FriendDataServiceReadFriends = new Operation("");
        //friendIdList;
        //friendIdCount;

        //Achievements Event Data Params
        public static readonly OperationParam GamificationServiceAchievementsName = new OperationParam("achievements");
        public static readonly OperationParam GamificationServiceAchievementsData = new OperationParam("data");
        public static readonly OperationParam GamificationServiceAchievementsGranted = new OperationParam("achievementsGranted");
        public static readonly OperationParam GamificationServiceCategory = new OperationParam("category");
        public static readonly OperationParam GamificationServiceMilestones = new OperationParam("milestones");
        public static readonly OperationParam GamificationServiceIncludeMetaData = new OperationParam("includeMetaData");

        // Player Statistic Event Params
        public static readonly OperationParam PlayerStatisticEventServiceEventName = new OperationParam("eventName");
        public static readonly OperationParam PlayerStatisticEventServiceEventMultiplier = new OperationParam("eventMultiplier");
        public static readonly OperationParam PlayerStatisticEventServiceEvents = new OperationParam("events");

        // Presence Params
        public static readonly OperationParam PresenceServicePlatform = new OperationParam("platform");
        public static readonly OperationParam PresenceServiceIncludeOffline = new OperationParam("includeOffline");
        public static readonly OperationParam PresenceServiceGroupId = new OperationParam("groupId");
        public static readonly OperationParam PresenceServiceProfileIds = new OperationParam("profileIds");
        public static readonly OperationParam PresenceServiceBidirectional = new OperationParam("bidirectional");
        public static readonly OperationParam PresenceServiceVisibile = new OperationParam("visible");
        public static readonly OperationParam PresenceServiceActivity = new OperationParam("activity");

        // Player State Service - Read Params
        public static readonly OperationParam PlayerStateServiceReadEntitySubtype = new OperationParam("entitySubType");

        // Player State Service - Update Summary Params
        public static readonly OperationParam PlayerStateServiceUpdateSummaryFriendData = new OperationParam("summaryFriendData");
        public static readonly OperationParam PlayerStateServiceUpdateNameData = new OperationParam("playerName");

        // Player State Service - Atributes
        public static readonly OperationParam PlayerStateServiceAttributes = new OperationParam("attributes");
        public static readonly OperationParam PlayerStateServiceWipeExisting = new OperationParam("wipeExisting");

        public static readonly OperationParam PlayerStateServiceIncludeSummaryData = new OperationParam("includePlayerSummaryData");

        // Player State Service - UPDATE_PICTURE_URL
        public static readonly OperationParam PlayerStateServicePlayerPictureUrl = new OperationParam("playerPictureUrl");
        public static readonly OperationParam PlayerStateServiceContactEmail = new OperationParam("contactEmail");

        // Player State Service - Reset Params
        //public static readonly Operation PlayerStateServiceReset = new Operation("");

        // Player Statistics Service - Update Increment Params
        public static readonly OperationParam PlayerStatisticsServiceStats = new OperationParam("statistics");
        public static readonly OperationParam PlayerStatisticsServiceStatNames = new OperationParam("statNames");
        public static readonly OperationParam PlayerStatisticsExperiencePoints = new OperationParam("xp_points");

        // Player Statistics Service - Read Params
        public static readonly OperationParam PlayerStatisticsServiceReadEntitySubType = new OperationParam("entitySubType");

        //public static readonly Operation PlayerStatisticsServiceDelete = new Operation("DELETE");

        // Push Notification Service operations (C++ only??)
        //public static readonly Operation PushNotificationServiceCreate = new Operation("CREATE");
        //public static readonly Operation PushNotificationServiceRegister = new Operation("REGISTER");

        // Social Leaderboard Service - general parameters
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardId = new OperationParam("leaderboardId");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardIds = new OperationParam("leaderboardIds");
        public static readonly OperationParam SocialLeaderboardServiceReplaceName = new OperationParam("replaceName");
        public static readonly OperationParam SocialLeaderboardServiceScore = new OperationParam("score");
        public static readonly OperationParam SocialLeaderboardServiceData = new OperationParam("data");
        public static readonly OperationParam SocialLeaderboardServiceEventName = new OperationParam("eventName");
        public static readonly OperationParam SocialLeaderboardServiceEventMultiplier = new OperationParam("eventMultiplier");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardType = new OperationParam("leaderboardType");
        public static readonly OperationParam SocialLeaderboardServiceRotationType = new OperationParam("rotationType");
        public static readonly OperationParam SocialLeaderboardServiceRotationReset = new OperationParam("rotationReset");
        public static readonly OperationParam SocialLeaderboardServiceRetainedCount = new OperationParam("retainedCount");
        public static readonly OperationParam NumDaysToRotate = new OperationParam("numDaysToRotate");
        public static readonly OperationParam SocialLeaderboardServiceFetchType = new OperationParam("fetchType");
        public static readonly OperationParam SocialLeaderboardServiceMaxResults = new OperationParam("maxResults");
        public static readonly OperationParam SocialLeaderboardServiceSort = new OperationParam("sort");
        public static readonly OperationParam SocialLeaderboardServiceStartIndex = new OperationParam("startIndex");
        public static readonly OperationParam SocialLeaderboardServiceEndIndex = new OperationParam("endIndex");
        public static readonly OperationParam SocialLeaderboardServiceBeforeCount = new OperationParam("beforeCount");
        public static readonly OperationParam SocialLeaderboardServiceAfterCount = new OperationParam("afterCount");
        public static readonly OperationParam SocialLeaderboardServiceIncludeLeaderboardSize = new OperationParam("includeLeaderboardSize");
        public static readonly OperationParam SocialLeaderboardServiceVersionId = new OperationParam("versionId");
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardResultCount = new OperationParam("leaderboardResultCount");
        public static readonly OperationParam SocialLeaderboardServiceGroupId = new OperationParam("groupId");
        public static readonly OperationParam SocialLeaderboardServiceProfileIds = new OperationParam("profileIds");
        public static readonly OperationParam SocialLeaderboardServiceRotationResetTime = new OperationParam("rotationResetTime");


        // Social Leaderboard Service - Reset Score Params
        //public static readonly Operation SocialLeaderboardServiceResetScore = new Operation("");

        // Product Service
        public static readonly OperationParam ProductServiceCurrencyId = new OperationParam("vc_id");
        public static readonly OperationParam ProductServiceCurrencyAmount = new OperationParam("vc_amount");

        // AppStore 
        public static readonly OperationParam AppStoreServiceStoreId = new OperationParam("storeId");
        public static readonly OperationParam AppStoreServiceReceiptData = new OperationParam("receiptData");
        public static readonly OperationParam AppStoreServicePurchaseData = new OperationParam("purchaseData");
        public static readonly OperationParam AppStoreServiceTransactionId = new OperationParam("transactionId");
        public static readonly OperationParam AppStoreServiceTransactionData = new OperationParam("transactionData");
        public static readonly OperationParam AppStoreServicePriceInfoCriteria = new OperationParam("priceInfoCriteria");

        // Virtual Currency Service
        public static readonly OperationParam VirtualCurrencyServiceCurrencyId = new OperationParam("vcId");
        public static readonly OperationParam VirtualCurrencyServiceCurrencyAmount = new OperationParam("vcAmount");

        // Product Service - Get Inventory Params
        public static readonly OperationParam ProductServiceGetInventoryPlatform = new OperationParam("platform");
        public static readonly OperationParam ProductServiceGetInventoryUserCurrency = new OperationParam("user_currency");
        public static readonly OperationParam ProductServiceGetInventoryCategory = new OperationParam("category");

        // Product Service - Op Cash In Receipt Params
        public static readonly OperationParam ProductServiceOpCashInReceiptReceipt = new OperationParam("receipt"); //C++ only
        public static readonly OperationParam ProductServiceOpCashInReceiptUrl = new OperationParam("url"); //C++ only

        // Product Service - Reset Player VC Params
        //public static readonly OperationParam ProductServiceResetPlayerVC = new OperationParam("");

        // Heartbeat Service - Params
        //public static readonly OperationParam HeartbeatService = new OperationParam("");

        // Time Service - Params
        //public static readonly OperationParam TimeService = new OperationParam("");

        // Server Time Service - Read Params
        public static readonly OperationParam ServerTimeServiceRead = new OperationParam("");

        // data creation parms
        public static readonly OperationParam ServiceMessageService = new OperationParam("service");
        public static readonly OperationParam ServiceMessageOperation = new OperationParam("operation");
        public static readonly OperationParam ServiceMessageData = new OperationParam("data");

        // data bundle creation parms
        public static readonly OperationParam ServiceMessagePacketId = new OperationParam("packetId");
        public static readonly OperationParam ServiceMessageSessionId = new OperationParam("sessionId");
        public static readonly OperationParam ServiceMessageGameId = new OperationParam("gameId");
        public static readonly OperationParam ServiceMessageMessages = new OperationParam("messages");
        public static readonly OperationParam ProfileId = new OperationParam("profileId");

        // Error Params
        public static readonly OperationParam ServiceMessageReasonCode = new OperationParam("reason_code");
        public static readonly OperationParam ServiceMessageStatusMessage = new OperationParam("status_message");

        public static readonly OperationParam DeviceRegistrationTypeIos = new OperationParam("IOS");
        public static readonly OperationParam DeviceRegistrationTypeAndroid = new OperationParam("ANG");

        public static readonly OperationParam ScriptServiceRunScriptName = new OperationParam("scriptName");
        public static readonly OperationParam ScriptServiceRunScriptData = new OperationParam("scriptData");
        public static readonly OperationParam ScriptServiceStartDateUTC = new OperationParam("startDateUTC");
        public static readonly OperationParam ScriptServiceStartMinutesFromNow = new OperationParam("minutesFromNow");
        public static readonly OperationParam ScriptServiceParentLevel = new OperationParam("parentLevel");
        public static readonly OperationParam ScriptServiceJobId = new OperationParam("jobId");

        public static readonly OperationParam MatchMakingServicePlayerRating = new OperationParam("playerRating");
        public static readonly OperationParam MatchMakingServiceMinutes = new OperationParam("minutes");
        public static readonly OperationParam MatchMakingServiceRangeDelta = new OperationParam("rangeDelta");
        public static readonly OperationParam MatchMakingServiceNumMatches = new OperationParam("numMatches");
        public static readonly OperationParam MatchMakingServiceAttributes = new OperationParam("attributes");
        public static readonly OperationParam MatchMakingServiceExtraParams = new OperationParam("extraParms");
        public static readonly OperationParam MatchMakingServicePlayerId = new OperationParam("playerId");
        public static readonly OperationParam MatchMakingServicePlaybackStreamId = new OperationParam("playbackStreamId");

        public static readonly OperationParam OfflineMatchServicePlayerId = new OperationParam("playerId");
        public static readonly OperationParam OfflineMatchServiceRangeDelta = new OperationParam("rangeDelta");
        public static readonly OperationParam OfflineMatchServicePlaybackStreamId = new OperationParam("playbackStreamId");

        public static readonly OperationParam PlaybackStreamServiceTargetPlayerId = new OperationParam("targetPlayerId");
        public static readonly OperationParam PlaybackStreamServiceInitiatingPlayerId = new OperationParam("initiatingPlayerId");
        public static readonly OperationParam PlaybackStreamServiceMaxNumberOfStreams = new OperationParam("maxNumStreams");
        public static readonly OperationParam PlaybackStreamServiceIncludeSharedData = new OperationParam("includeSharedData");
        public static readonly OperationParam PlaybackStreamServicePlaybackStreamId = new OperationParam("playbackStreamId");
        public static readonly OperationParam PlaybackStreamServiceEventData = new OperationParam("eventData");
        public static readonly OperationParam PlaybackStreamServiceSummary = new OperationParam("summary");

        public static readonly OperationParam ProductServiceTransId = new OperationParam("transId");
        public static readonly OperationParam ProductServiceOrderId = new OperationParam("orderId");
        public static readonly OperationParam ProductServiceProductId = new OperationParam("productId");
        public static readonly OperationParam ProductServiceLanguage = new OperationParam("language");
        public static readonly OperationParam ProductServiceItemId = new OperationParam("itemId");
        public static readonly OperationParam ProductServiceReceipt = new OperationParam("receipt");
        public static readonly OperationParam ProductServiceSignedRequest = new OperationParam("signed_request");
        public static readonly OperationParam ProductServiceToken = new OperationParam("token");

        //S3 Service
        public static readonly OperationParam S3HandlingServiceFileCategory = new OperationParam("category");
        public static readonly OperationParam S3HandlingServiceFileDetails = new OperationParam("fileDetails");
        public static readonly OperationParam S3HandlingServiceFileId = new OperationParam("fileId");

        //Shared Identity
        public static readonly OperationParam IdentityServiceForceSingleton = new OperationParam("forceSingleton");

        //RedemptionCode
        public static readonly OperationParam RedemptionCodeServiceScanCode = new OperationParam("scanCode");
        public static readonly OperationParam RedemptionCodeServiceCodeType = new OperationParam("codeType");
        public static readonly OperationParam RedemptionCodeServiceCustomRedemptionInfo = new OperationParam("customRedemptionInfo");

        //DataStream
        public static readonly OperationParam DataStreamEventName = new OperationParam("eventName");
        public static readonly OperationParam DataStreamEventProperties = new OperationParam("eventProperties");

        // Profanity
        public static readonly OperationParam ProfanityText = new OperationParam("text");
        public static readonly OperationParam ProfanityReplaceSymbol = new OperationParam("replaceSymbol");
        public static readonly OperationParam ProfanityFlagEmail = new OperationParam("flagEmail");
        public static readonly OperationParam ProfanityFlagPhone = new OperationParam("flagPhone");
        public static readonly OperationParam ProfanityFlagUrls = new OperationParam("flagUrls");
        public static readonly OperationParam ProfanityLanguages = new OperationParam("languages");

        //File upload
        public static readonly OperationParam UploadLocalPath = new OperationParam("localPath");
        public static readonly OperationParam UploadCloudPath = new OperationParam("cloudPath");
        public static readonly OperationParam UploadCloudFilename = new OperationParam("cloudFilename");
        public static readonly OperationParam UploadShareable = new OperationParam("shareable");
        public static readonly OperationParam UploadReplaceIfExists = new OperationParam("replaceIfExists");
        public static readonly OperationParam UploadFileSize = new OperationParam("fileSize");
        public static readonly OperationParam UploadRecurse = new OperationParam("recurse");
        public static readonly OperationParam UploadPath = new OperationParam("path");

        //group
        public static readonly OperationParam GroupId = new OperationParam("groupId");
        public static readonly OperationParam GroupProfileId = new OperationParam("profileId");
        public static readonly OperationParam GroupRole = new OperationParam("role");
        public static readonly OperationParam GroupAttributes = new OperationParam("attributes");
        public static readonly OperationParam GroupName = new OperationParam("name");
        public static readonly OperationParam GroupType = new OperationParam("groupType");
        public static readonly OperationParam GroupEntityType = new OperationParam("entityType");
        public static readonly OperationParam GroupIsOpenGroup = new OperationParam("isOpenGroup");
        public static readonly OperationParam GroupAcl = new OperationParam("acl");
        public static readonly OperationParam GroupData = new OperationParam("data");
        public static readonly OperationParam GroupOwnerAttributes = new OperationParam("ownerAttributes");
        public static readonly OperationParam GroupDefaultMemberAttributes = new OperationParam("defaultMemberAttributes");
        public static readonly OperationParam GroupIsOwnedByGroupMember = new OperationParam("isOwnedByGroupMember");
        public static readonly OperationParam GroupEntityId = new OperationParam("entityId");
        public static readonly OperationParam GroupVersion = new OperationParam("version");
        public static readonly OperationParam GroupContext = new OperationParam("context");
        public static readonly OperationParam GroupPageOffset = new OperationParam("pageOffset");
        public static readonly OperationParam GroupAutoJoinStrategy = new OperationParam("autoJoinStrategy");
        public static readonly OperationParam GroupWhere = new OperationParam("where");

        //mail
        public static readonly OperationParam Subject = new OperationParam("subject");
        public static readonly OperationParam Body = new OperationParam("body");
        public static readonly OperationParam ServiceParams = new OperationParam("serviceParams");
        public static readonly OperationParam EmailAddress = new OperationParam("emailAddress");

        public static readonly OperationParam LeaderboardId = new OperationParam("leaderboardId");
        public static readonly OperationParam DivSetId = new OperationParam("divSetId");
        public static readonly OperationParam VersionId = new OperationParam("versionId");
        public static readonly OperationParam TournamentCode = new OperationParam("tournamentCode");
        public static readonly OperationParam InitialScore = new OperationParam("initialScore");
        public static readonly OperationParam Score = new OperationParam("score");
        public static readonly OperationParam RoundStartedEpoch = new OperationParam("roundStartedEpoch");
        public static readonly OperationParam Data = new OperationParam("data");

        // chat
        public static readonly OperationParam ChatChannelId = new OperationParam("channelId");
        public static readonly OperationParam ChatMaxReturn = new OperationParam("maxReturn");
        public static readonly OperationParam ChatMessageId = new OperationParam("msgId");
        public static readonly OperationParam ChatVersion = new OperationParam("version");
        
        public static readonly OperationParam ChatChannelType = new OperationParam("channelType");
        public static readonly OperationParam ChatChannelSubId = new OperationParam("channelSubId");
        public static readonly OperationParam ChatContent = new OperationParam("content");
        public static readonly OperationParam ChatText = new OperationParam("text");

        public static readonly OperationParam ChatRich = new OperationParam("rich");
        public static readonly OperationParam ChatRecordInHistory = new OperationParam("recordInHistory");

        // TODO:: do we enumerate these ? [smrj]
        // chat channel types 
        public static readonly OperationParam AllChannelType = new OperationParam("all");
        public static readonly OperationParam GlobalChannelType = new OperationParam("gl");
        public static readonly OperationParam GroupChannelType = new OperationParam("gr");

        // messaging
        public static readonly OperationParam MessagingMessageBox = new OperationParam("msgbox");
        public static readonly OperationParam MessagingMessageIds = new OperationParam("msgIds");
        public static readonly OperationParam MessagingContext = new OperationParam("context");
        public static readonly OperationParam MessagingPageOffset = new OperationParam("pageOffset");
        public static readonly OperationParam MessagingFromName = new OperationParam("fromName");
        public static readonly OperationParam MessagingToProfileIds = new OperationParam("toProfileIds");
        public static readonly OperationParam MessagingContent = new OperationParam("contentJson");
        public static readonly OperationParam MessagingSubject = new OperationParam("subject");
        public static readonly OperationParam MessagingText = new OperationParam("text");

        public static readonly OperationParam InboxMessageType = new OperationParam("inbox");
        public static readonly OperationParam SentMessageType = new OperationParam("sent");

        // lobby
        public static readonly OperationParam LobbyRoomType = new OperationParam("lobbyType");
        public static readonly OperationParam LobbyRating = new OperationParam("rating");
        public static readonly OperationParam LobbyAlgorithm = new OperationParam("algo");
        public static readonly OperationParam LobbyMaxSteps = new OperationParam("maxSteps");
        public static readonly OperationParam LobbyStrategy = new OperationParam("strategy");
        public static readonly OperationParam LobbyAlignment = new OperationParam("alignment");
        public static readonly OperationParam LobbyRanges = new OperationParam("ranges");
        public static readonly OperationParam LobbyFilterJson = new OperationParam("filterJson");
        public static readonly OperationParam LobbySettings = new OperationParam("settings");
        public static readonly OperationParam LobbyTimeoutSeconds = new OperationParam("timeoutSecs");
        public static readonly OperationParam LobbyIsReady = new OperationParam("isReady");
        public static readonly OperationParam LobbyOtherUserCxIds = new OperationParam("otherUserCxIds");
        public static readonly OperationParam LobbyExtraJson = new OperationParam("extraJson");
        public static readonly OperationParam LobbyTeamCode = new OperationParam("teamCode");
        public static readonly OperationParam LobbyIdentifier = new OperationParam("lobbyId");
        public static readonly OperationParam LobbyToTeamName = new OperationParam("toTeamCode");
        public static readonly OperationParam LobbySignalData = new OperationParam("signalData");
        public static readonly OperationParam LobbyConnectionId = new OperationParam("cxId");

        private OperationParam(string value)
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
