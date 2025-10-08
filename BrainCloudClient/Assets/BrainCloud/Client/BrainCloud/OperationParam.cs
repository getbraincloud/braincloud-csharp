// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{
    public struct OperationParam : System.IEquatable<OperationParam>, System.IComparable<OperationParam>
    {
        #region brainCloud Operation Parameters

        //Push Notification Service - Register Params
        public static readonly OperationParam PushNotificationRegisterParamDeviceType = "deviceType";
        public static readonly OperationParam PushNotificationRegisterParamDeviceToken = "deviceToken";

        //Push Notification Service - Send Params
        public static readonly OperationParam PushNotificationSendParamToPlayerId = "toPlayerId";
        public static readonly OperationParam PushNotificationSendParamProfileId = "profileId";
        public static readonly OperationParam PushNotificationSendParamMessage = "message";
        public static readonly OperationParam PushNotificationSendParamNotificationTemplateId = "notificationTemplateId";
        public static readonly OperationParam PushNotificationSendParamSubstitutions = "substitutions";
        public static readonly OperationParam PushNotificationSendParamProfileIds = "profileIds";

        public static readonly OperationParam PushNotificationSendParamFcmContent = "fcmContent";
        public static readonly OperationParam PushNotificationSendParamIosContent = "iosContent";
        public static readonly OperationParam PushNotificationSendParamFacebookContent = "facebookContent";


        public static readonly OperationParam AlertContent = "alertContent";
        public static readonly OperationParam CustomData = "customData";

        public static readonly OperationParam StartDateUTC = "startDateUTC";
        public static readonly OperationParam MinutesFromNow = "minutesFromNow";

        // Twitter Service - Verify Params
        public static readonly OperationParam TwitterServiceVerifyToken = "token";
        public static readonly OperationParam TwitterServiceVerifyVerifier = "verifier";

        // Twitter Service - Tweet Params
        public static readonly OperationParam TwitterServiceTweetToken = "token";
        public static readonly OperationParam TwitterServiceTweetSecret = "secret";
        public static readonly OperationParam TwitterServiceTweetTweet = "tweet";
        public static readonly OperationParam TwitterServiceTweetPic = "pic";

        public static readonly OperationParam BlockChainConfig = "blockchainConfig";
        public static readonly OperationParam BlockChainIntegrationId = "integrationId";
        public static readonly OperationParam BlockChainContext = "contextJson";
        public static readonly OperationParam PublicKey = "publicKey";

        // Authenticate Service - Authenticate Params
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationType = "authenticationType";
        public static readonly OperationParam AuthenticateServiceAuthenticateAuthenticationToken = "authenticationToken";
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalId = "externalId";
        public static readonly OperationParam AuthenticateServiceAuthenticateUniversalId = "universalId";
        public static readonly OperationParam AuthenticateServiceAuthenticateGameId = "gameId";
        public static readonly OperationParam AuthenticateServiceAuthenticateDeviceId = "deviceId";
        public static readonly OperationParam AuthenticateServiceAuthenticateForceMergeFlag = "forceMergeFlag";
        public static readonly OperationParam AuthenticateServiceAuthenticateReleasePlatform = "releasePlatform";
        public static readonly OperationParam AuthenticateServiceAuthenticateGameVersion = "gameVersion";
        public static readonly OperationParam AuthenticateServiceAuthenticateBrainCloudVersion = "clientLibVersion";
        public static readonly OperationParam AuthenticateServiceAuthenticateExternalAuthName = "externalAuthName";
        public static readonly OperationParam AuthenticateServiceAuthenticateEmailAddress = "emailAddress";
        public static readonly OperationParam AuthenticateServiceAuthenticateServiceParams = "serviceParams";
        public static readonly OperationParam AuthenticateServiceAuthenticateTokenTtlInMinutes = "tokenTtlInMinutes";

        public static readonly OperationParam AuthenticateServiceAuthenticateLevelName = "levelName";
        public static readonly OperationParam AuthenticateServiceAuthenticatePeerCode = "peerCode";

        public static readonly OperationParam AuthenticateServiceAuthenticateCountryCode = "countryCode";
        public static readonly OperationParam AuthenticateServiceAuthenticateLanguageCode = "languageCode";
        public static readonly OperationParam AuthenticateServiceAuthenticateTimeZoneOffset = "timeZoneOffset";

        public static readonly OperationParam AuthenticateServiceAuthenticateAuthUpgradeID = "upgradeAppId";
        public static readonly OperationParam AuthenticateServiceAuthenticateAnonymousId = "anonymousId";
        public static readonly OperationParam AuthenticateServiceAuthenticateProfileId = "profileId";
        public static readonly OperationParam AuthenticateServiceAuthenticateForceCreate = "forceCreate";
        public static readonly OperationParam AuthenticateServiceAuthenticateCompressResponses = "compressResponses";
        public static readonly OperationParam AuthenticateServicePlayerSessionExpiry = "playerSessionExpiry";
        public static readonly OperationParam AuthenticateServiceAuthenticateExtraJson = "extraJson";

        // Authenticate Service - Authenticate Params
        public static readonly OperationParam IdentityServiceExternalId = "externalId";
        public static readonly OperationParam IdentityServiceAuthenticationType = "authenticationType";
        public static readonly OperationParam IdentityServiceConfirmAnonymous = "confirmAnonymous";
        
        public static readonly OperationParam IdentityServiceOldEmailAddress = "oldEmailAddress";
        public static readonly OperationParam IdentityServiceNewEmailAddress = "newEmailAddress";
        public static readonly OperationParam IdentityServiceUpdateContactEmail = "updateContactEmail";


        // Peer
        public static readonly OperationParam Peer = "peer";

        // Entity Service 
        public static readonly OperationParam EntityServiceEntityId = "entityId";
        public static readonly OperationParam EntityServiceEntityType = "entityType";
        public static readonly OperationParam EntityServiceEntitySubtype = "entitySubtype";
        public static readonly OperationParam EntityServiceData = "data";
        public static readonly OperationParam EntityServiceAcl = "acl";
        public static readonly OperationParam EntityServiceFriendData = "friendData";
        public static readonly OperationParam EntityServiceVersion = "version";
        public static readonly OperationParam EntityServiceUpdateOps = "updateOps";
        public static readonly OperationParam EntityServiceTargetPlayerId = "targetPlayerId";

        // Global Entity Service - Params
        public static readonly OperationParam GlobalEntityServiceEntityId = "entityId";
        public static readonly OperationParam GlobalEntityServiceEntityType = "entityType";
        public static readonly OperationParam GlobalEntityServiceIndexedId = "entityIndexedId";
        public static readonly OperationParam GlobalEntityServiceTimeToLive = "timeToLive";
        public static readonly OperationParam GlobalEntityServiceData = "data";
        public static readonly OperationParam GlobalEntityServiceAcl = "acl";
        public static readonly OperationParam GlobalEntityServiceVersion = "version";
        public static readonly OperationParam GlobalEntityServiceMaxReturn = "maxReturn";
        public static readonly OperationParam GlobalEntityServiceWhere = "where";
        public static readonly OperationParam GlobalEntityServiceOrderBy = "orderBy";
        public static readonly OperationParam GlobalEntityServiceContext = "context";
        public static readonly OperationParam GlobalEntityServicePageOffset = "pageOffset";
        public static readonly OperationParam OwnerId = "ownerId";

        // Event Service - Send Params
        public static readonly OperationParam EventServiceSendToId = "toId";
        public static readonly OperationParam EventServiceSendToIds = "toIds";
        public static readonly OperationParam EventServiceSendEventType = "eventType";
        public static readonly OperationParam EventServiceSendEventId = "eventId";
        public static readonly OperationParam EventServiceSendEventData = "eventData";
        public static readonly OperationParam EventServiceSendRecordLocally = "recordLocally";

        // Event Service - Update Event Data Params
        public static readonly OperationParam EventServiceUpdateEventDataFromId = "fromId";
        public static readonly OperationParam EventServiceUpdateEventDataEventId = "eventId";
        public static readonly OperationParam EventServiceUpdateEventDataData = "eventData";
        public static readonly OperationParam EvId = "evId";
        public static readonly OperationParam EventServiceEvIds  = "evIds";
        public static readonly OperationParam EventServiceDateMillis  = "dateMillis";
        public static readonly OperationParam EventServiceEventType   = "eventType";
        
        // Event Service - Delete Incoming Params
        public static readonly OperationParam EventServiceDeleteIncomingEventId = "eventId";
        public static readonly OperationParam EventServiceDeleteIncomingFromId = "fromId";

        // Event Service - Delete Sent Params
        public static readonly OperationParam EventServiceDeleteSentEventId = "eventId";
        public static readonly OperationParam EventServiceDeleteSentToId = "toId";
        public static readonly OperationParam EventServiceIncludeIncomingEvents = "includeIncomingEvents";
        public static readonly OperationParam EventServiceIncludeSentEvents = "includeSentEvents";

        // Friend Service - Params
        public static readonly OperationParam FriendServiceEntityId = "entityId";
        public static readonly OperationParam FriendServiceExternalId = "externalId";
        public static readonly OperationParam FriendServiceExternalIds = "externalIds";
        public static readonly OperationParam FriendServiceProfileId = "profileId";
        public static readonly OperationParam FriendServiceFriendId = "friendId";
        public static readonly OperationParam FriendServiceAuthenticationType = "authenticationType";
        public static readonly OperationParam ExternalAuthType = "externalAuthType";
        public static readonly OperationParam FriendServiceEntityType = "entityType";
        public static readonly OperationParam FriendServiceEntitySubtype = "entitySubtype";
        public static readonly OperationParam FriendServiceIncludeSummaryData = "includeSummaryData";
        public static readonly OperationParam FriendServiceFriendPlatform = "friendPlatform";
        public static readonly OperationParam FriendServiceProfileIds = "profileIds";
        public static readonly OperationParam FriendServiceMode = "mode";

        // Friend Service operations
        //public static readonly Operation FriendServiceReadFriends = new Operation("READ_FRIENDS";

        // Friend Service - Read Player State Params
        public static readonly OperationParam FriendServiceReadPlayerStateFriendId = "friendId";
        public static readonly OperationParam FriendServiceSearchText = "searchText";
        public static readonly OperationParam FriendServiceMaxResults = "maxResults";


        // Friend Data Service - Read Friends Params (C++ only?)
        //public static readonly Operation FriendDataServiceReadFriends = new Operation("";
        //friendIdList;
        //friendIdCount;

        //Achievements Event Data Params
        public static readonly OperationParam GamificationServiceAchievementsName = "achievements";
        public static readonly OperationParam GamificationServiceAchievementsData = "data";
        public static readonly OperationParam GamificationServiceAchievementsGranted = "achievementsGranted";
        public static readonly OperationParam GamificationServiceCategory = "category";
        public static readonly OperationParam GamificationServiceMilestones = "milestones";
        public static readonly OperationParam GamificationServiceIncludeMetaData = "includeMetaData";

        // Player Statistic Event Params
        public static readonly OperationParam PlayerStatisticEventServiceEventName = "eventName";
        public static readonly OperationParam PlayerStatisticEventServiceEventMultiplier = "eventMultiplier";
        public static readonly OperationParam PlayerStatisticEventServiceEvents = "events";

        // Presence Params
        public static readonly OperationParam PresenceServicePlatform = "platform";
        public static readonly OperationParam PresenceServiceIncludeOffline = "includeOffline";
        public static readonly OperationParam PresenceServiceGroupId = "groupId";
        public static readonly OperationParam PresenceServiceProfileIds = "profileIds";
        public static readonly OperationParam PresenceServiceBidirectional = "bidirectional";
        public static readonly OperationParam PresenceServiceVisibile = "visible";
        public static readonly OperationParam PresenceServiceActivity = "activity";

        // Player State Service - Read Params
        public static readonly OperationParam PlayerStateServiceReadEntitySubtype = "entitySubType";

        // Player State Service - Update Summary Params
        public static readonly OperationParam PlayerStateServiceUpdateSummaryFriendData = "summaryFriendData";
        public static readonly OperationParam PlayerStateServiceUpdateNameData = "playerName";
        public static readonly OperationParam PlayerStateServiceTimeZoneOffset = "timeZoneOffset";
        public static readonly OperationParam PlayerStateServiceLanguageCode = "languageCode";

        // Player State Service - Atributes
        public static readonly OperationParam PlayerStateServiceAttributes = "attributes";
        public static readonly OperationParam PlayerStateServiceWipeExisting = "wipeExisting";

        public static readonly OperationParam PlayerStateServiceIncludeSummaryData = "includePlayerSummaryData";

        // Player State Service - UPDATE_PICTURE_URL
        public static readonly OperationParam PlayerStateServicePlayerPictureUrl = "playerPictureUrl";
        public static readonly OperationParam PlayerStateServiceContactEmail = "contactEmail";

        // Player State Service - Reset Params
        //public static readonly Operation PlayerStateServiceReset = new Operation("";
        

        // Player Statistics Service - Update Increment Params
        public static readonly OperationParam PlayerStatisticsServiceStats = "statistics";
        public static readonly OperationParam PlayerStatisticsServiceStatNames = "statNames";
        public static readonly OperationParam PlayerStatisticsExperiencePoints = "xp_points";

        // Player Statistics Service - Status Param
        public static readonly OperationParam PlayerStateServiceStatusName = "statusName";
        
        // Player Statistics Service - Extend User Status Params
        public static readonly OperationParam PlayerStateServiceAdditionalSecs = "additionalSecs";
        public static readonly OperationParam PlayerStateServiceDetails = "details";

        public static readonly OperationParam PlayerStateServiceDurationSecs = "durationSecs";

        // Player Statistics Service - Read Params
        public static readonly OperationParam PlayerStatisticsServiceReadEntitySubType = "entitySubType";

        //public static readonly Operation PlayerStatisticsServiceDelete = new Operation("DELETE";

        // Push Notification Service operations (C++ only??)
        //public static readonly Operation PushNotificationServiceCreate = new Operation("CREATE";
        //public static readonly Operation PushNotificationServiceRegister = new Operation("REGISTER";

        // Social Leaderboard Service - general parameters
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardId = "leaderboardId";
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardIds = "leaderboardIds";
        public static readonly OperationParam SocialLeaderboardServiceReplaceName = "replaceName";
        public static readonly OperationParam SocialLeaderboardServiceScore = "score";
        public static readonly OperationParam SocialLeaderboardServiceScoreData = "scoreData";
        public static readonly OperationParam SocialLeaderboardServiceConfigJson = "configJson";
        public static readonly OperationParam SocialLeaderboardServiceData = "data";
        public static readonly OperationParam SocialLeaderboardServiceEventName = "eventName";
        public static readonly OperationParam SocialLeaderboardServiceEventMultiplier = "eventMultiplier";
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardType = "leaderboardType";
        public static readonly OperationParam SocialLeaderboardServiceRotationType = "rotationType";
        public static readonly OperationParam SocialLeaderboardServiceRotationReset = "rotationReset";
        public static readonly OperationParam SocialLeaderboardServiceRetainedCount = "retainedCount";
        public static readonly OperationParam NumDaysToRotate = "numDaysToRotate";
        public static readonly OperationParam SocialLeaderboardServiceFetchType = "fetchType";
        public static readonly OperationParam SocialLeaderboardServiceMaxResults = "maxResults";
        public static readonly OperationParam SocialLeaderboardServiceSort = "sort";
        public static readonly OperationParam SocialLeaderboardServiceStartIndex = "startIndex";
        public static readonly OperationParam SocialLeaderboardServiceEndIndex = "endIndex";
        public static readonly OperationParam SocialLeaderboardServiceBeforeCount = "beforeCount";
        public static readonly OperationParam SocialLeaderboardServiceAfterCount = "afterCount";
        public static readonly OperationParam SocialLeaderboardServiceIncludeLeaderboardSize = "includeLeaderboardSize";
        public static readonly OperationParam SocialLeaderboardServiceVersionId = "versionId";
        public static readonly OperationParam SocialLeaderboardServiceLeaderboardResultCount = "leaderboardResultCount";
        public static readonly OperationParam SocialLeaderboardServiceGroupId = "groupId";
        public static readonly OperationParam SocialLeaderboardServiceProfileIds = "profileIds";
        public static readonly OperationParam SocialLeaderboardServiceRotationResetTime = "rotationResetTime";


        // Social Leaderboard Service - Reset Score Params
        //public static readonly Operation SocialLeaderboardServiceResetScore = new Operation("";

        // Product Service
        public static readonly OperationParam ProductServiceCurrencyId = "vc_id";
        public static readonly OperationParam ProductServiceCurrencyAmount = "vc_amount";

        // AppStore 
        public static readonly OperationParam AppStoreServiceStoreId = "storeId";
        public static readonly OperationParam AppStoreServiceIAPId = "iapId";
        public static readonly OperationParam AppStoreServiceReceiptData = "receiptData";
        public static readonly OperationParam AppStoreServicePurchaseData = "purchaseData";
        public static readonly OperationParam AppStoreServiceTransactionId = "transactionId";
        public static readonly OperationParam AppStoreServiceTransactionData = "transactionData";
        public static readonly OperationParam AppStoreServicePriceInfoCriteria = "priceInfoCriteria";
        public static readonly OperationParam AppStoreServiceUserCurrency = "userCurrency";
        public static readonly OperationParam AppStoreServiceCategory = "category";
        public static readonly OperationParam AppStoreServicePayload = "payload";

        // Virtual Currency Service
        public static readonly OperationParam VirtualCurrencyServiceCurrencyId = "vcId";
        public static readonly OperationParam VirtualCurrencyServiceCurrencyAmount = "vcAmount";

        // Product Service - Get Inventory Params
        public static readonly OperationParam ProductServiceGetInventoryPlatform = "platform";
        public static readonly OperationParam ProductServiceGetInventoryUserCurrency = "user_currency";
        public static readonly OperationParam ProductServiceGetInventoryCategory = "category";

        // Product Service - Op Cash In Receipt Params
        public static readonly OperationParam ProductServiceOpCashInReceiptReceipt = "receipt"; //C++ only
        public static readonly OperationParam ProductServiceOpCashInReceiptUrl = "url"; //C++ only

        // Product Service - Reset Player VC Params
        //public static readonly OperationParam ProductServiceResetPlayerVC = "";

        // Heartbeat Service - Params
        //public static readonly OperationParam HeartbeatService = "";

        // Time Service - Params
        //public static readonly OperationParam TimeService = "";

        // Server Time Service - Read Params
        public static readonly OperationParam ServerTimeServiceRead = "";

        // data creation parms
        public static readonly OperationParam ServiceMessageService = "service";
        public static readonly OperationParam ServiceMessageOperation = "operation";
        public static readonly OperationParam ServiceMessageData = "data";

        // data bundle creation parms
        public static readonly OperationParam ServiceMessagePacketId = "packetId";
        public static readonly OperationParam ServiceMessageSessionId = "sessionId";
        public static readonly OperationParam ServiceMessageGameId = "gameId";
        public static readonly OperationParam ServiceMessageMessages = "messages";
        public static readonly OperationParam ProfileId = "profileId";

        // Error Params
        public static readonly OperationParam ServiceMessageReasonCode = "reason_code";
        public static readonly OperationParam ServiceMessageStatusMessage = "status_message";

        public static readonly OperationParam DeviceRegistrationTypeIos = "IOS";
        public static readonly OperationParam DeviceRegistrationTypeAndroid = "ANG";

        public static readonly OperationParam ScriptServiceRunScriptName = "scriptName";
        public static readonly OperationParam ScriptServiceRunScriptData = "scriptData";
        public static readonly OperationParam ScriptServiceStartDateUTC = "startDateUTC";
        public static readonly OperationParam ScriptServiceStartMinutesFromNow = "minutesFromNow";
        public static readonly OperationParam ScriptServiceParentLevel = "parentLevel";
        public static readonly OperationParam ScriptServiceJobId = "jobId";

        public static readonly OperationParam MatchMakingServicePlayerRating = "playerRating";
        public static readonly OperationParam MatchMakingServiceMinutes = "minutes";
        public static readonly OperationParam MatchMakingServiceRangeDelta = "rangeDelta";
        public static readonly OperationParam MatchMakingServiceNumMatches = "numMatches";
        public static readonly OperationParam MatchMakingServiceAttributes = "attributes";
        public static readonly OperationParam MatchMakingServiceExtraParams = "extraParms";
        public static readonly OperationParam MatchMakingServicePlayerId = "playerId";
        public static readonly OperationParam MatchMakingServicePlaybackStreamId = "playbackStreamId";

        public static readonly OperationParam OfflineMatchServicePlayerId = "playerId";
        public static readonly OperationParam OfflineMatchServiceRangeDelta = "rangeDelta";
        public static readonly OperationParam OfflineMatchServicePlaybackStreamId = "playbackStreamId";

        public static readonly OperationParam PlaybackStreamServiceTargetPlayerId = "targetPlayerId";
        public static readonly OperationParam PlaybackStreamServiceInitiatingPlayerId = "initiatingPlayerId";
        public static readonly OperationParam PlaybackStreamServiceMaxNumberOfStreams = "maxNumStreams";
        public static readonly OperationParam PlaybackStreamServiceIncludeSharedData = "includeSharedData";
        public static readonly OperationParam PlaybackStreamServicePlaybackStreamId = "playbackStreamId";
        public static readonly OperationParam PlaybackStreamServiceEventData = "eventData";
        public static readonly OperationParam PlaybackStreamServiceSummary = "summary";
        public static readonly OperationParam PlaybackStreamServiceNumDays = "numDays";

        public static readonly OperationParam ProductServiceTransId = "transId";
        public static readonly OperationParam ProductServiceOrderId = "orderId";
        public static readonly OperationParam ProductServiceProductId = "productId";
        public static readonly OperationParam ProductServiceLanguage = "language";
        public static readonly OperationParam ProductServiceItemId = "itemId";
        public static readonly OperationParam ProductServiceReceipt = "receipt";
        public static readonly OperationParam ProductServiceSignedRequest = "signed_request";
        public static readonly OperationParam ProductServiceToken = "token";

        //S3 Service
        public static readonly OperationParam S3HandlingServiceFileCategory = "category";
        public static readonly OperationParam S3HandlingServiceFileDetails = "fileDetails";
        public static readonly OperationParam S3HandlingServiceFileId = "fileId";

        //Shared Identity
        public static readonly OperationParam IdentityServiceForceSingleton = "forceSingleton";

        //RedemptionCode
        public static readonly OperationParam RedemptionCodeServiceScanCode = "scanCode";
        public static readonly OperationParam RedemptionCodeServiceCodeType = "codeType";
        public static readonly OperationParam RedemptionCodeServiceCustomRedemptionInfo = "customRedemptionInfo";

        //DataStream
        public static readonly OperationParam DataStreamEventName = "eventName";
        public static readonly OperationParam DataStreamEventProperties = "eventProperties";
        public static readonly OperationParam DataStreamCrashType = "crashType";
        public static readonly OperationParam DataStreamErrorMsg = "errorMsg";
        public static readonly OperationParam DataStreamCrashInfo = "crashJson";
        public static readonly OperationParam DataStreamCrashLog = "crashLog";
        public static readonly OperationParam DataStreamUserName = "userName";
        public static readonly OperationParam DataStreamUserEmail = "userEmail";
        public static readonly OperationParam DataStreamUserNotes = "userNotes";
        public static readonly OperationParam DataStreamUserSubmitted = "userSubmitted";

        // Profanity
        public static readonly OperationParam ProfanityText = "text";
        public static readonly OperationParam ProfanityReplaceSymbol = "replaceSymbol";
        public static readonly OperationParam ProfanityFlagEmail = "flagEmail";
        public static readonly OperationParam ProfanityFlagPhone = "flagPhone";
        public static readonly OperationParam ProfanityFlagUrls = "flagUrls";
        public static readonly OperationParam ProfanityLanguages = "languages";

        //File upload
        public static readonly OperationParam UploadLocalPath = "localPath";
        public static readonly OperationParam UploadCloudPath = "cloudPath";
        public static readonly OperationParam UploadCloudFilename = "cloudFilename";
        public static readonly OperationParam UploadShareable = "shareable";
        public static readonly OperationParam UploadReplaceIfExists = "replaceIfExists";
        public static readonly OperationParam UploadFileSize = "fileSize";
        public static readonly OperationParam UploadRecurse = "recurse";
        public static readonly OperationParam UploadPath = "path";

        //group
        public static readonly OperationParam GroupId = "groupId";
        public static readonly OperationParam GroupProfileId = "profileId";
        public static readonly OperationParam GroupRole = "role";
        public static readonly OperationParam GroupAttributes = "attributes";
        public static readonly OperationParam GroupName = "name";
        public static readonly OperationParam GroupType = "groupType";
        public static readonly OperationParam GroupTypes = "groupTypes";
        public static readonly OperationParam GroupEntityType = "entityType";
        public static readonly OperationParam GroupIsOpenGroup = "isOpenGroup";
        public static readonly OperationParam GroupAcl = "acl";
        public static readonly OperationParam GroupData = "data";
        public static readonly OperationParam GroupOwnerAttributes = "ownerAttributes";
        public static readonly OperationParam GroupDefaultMemberAttributes = "defaultMemberAttributes";
        public static readonly OperationParam GroupIsOwnedByGroupMember = "isOwnedByGroupMember";
        public static readonly OperationParam GroupSummaryData = "summaryData";
        public static readonly OperationParam GroupEntityId = "entityId";
        public static readonly OperationParam GroupVersion = "version";
        public static readonly OperationParam GroupContext = "context";
        public static readonly OperationParam GroupPageOffset = "pageOffset";
        public static readonly OperationParam GroupAutoJoinStrategy = "autoJoinStrategy";
        public static readonly OperationParam GroupWhere = "where";
        public static readonly OperationParam GroupMaxReturn = "maxReturn";
        
        //group file
        public static readonly OperationParam FolderPath = "folderPath";
        public static readonly OperationParam FileName = "filename";
        public static readonly OperationParam FullPathFilename = "fullPathFilename";
        public static readonly OperationParam FileId = "fileId";
        public static readonly OperationParam Version = "version";
        public static readonly OperationParam NewTreeId = "newTreeId";
        public static readonly OperationParam TreeVersion = "treeVersion";
        public static readonly OperationParam NewFilename = "newFilename";
        public static readonly OperationParam OverwriteIfPresent = "overwriteIfPresent";
        public static readonly OperationParam Recurse = "recurse";
        public static readonly OperationParam UserCloudPath = "userCloudPath";
        public static readonly OperationParam UserCloudFilename = "userCloudFilename";
        public static readonly OperationParam GroupTreeId = "groupTreeId";
        public static readonly OperationParam GroupFilename = "groupFilename";
        public static readonly OperationParam GroupFileACL = "groupFileAcl";
        public static readonly OperationParam NewACL = "newAcl";


        //GlobalFile
        public static readonly OperationParam GlobalFileServiceFileId = "fileId";
        public static readonly OperationParam GlobalFileServiceFolderPath = "folderPath";
        public static readonly OperationParam GlobalFileServiceFileName = "filename";
        public static readonly OperationParam GlobalFileServiceRecurse = "recurse";

        //mail
        public static readonly OperationParam Subject = "subject";
        public static readonly OperationParam Body = "body";
        public static readonly OperationParam ServiceParams = "serviceParams";
        public static readonly OperationParam EmailAddress = "emailAddress";
        public static readonly OperationParam EmailAddresses = "emailAddresses";

        public static readonly OperationParam LeaderboardId = "leaderboardId";
        public static readonly OperationParam DivSetId = "divSetId";
        public static readonly OperationParam VersionId = "versionId";
        public static readonly OperationParam TournamentCode = "tournamentCode";
        public static readonly OperationParam InitialScore = "initialScore";
        public static readonly OperationParam Score = "score";
        public static readonly OperationParam RoundStartedEpoch = "roundStartedEpoch";
        public static readonly OperationParam Data = "data";

        // chat
        public static readonly OperationParam ChatChannelId = "channelId";
        public static readonly OperationParam ChatMaxReturn = "maxReturn";
        public static readonly OperationParam ChatMessageId = "msgId";
        public static readonly OperationParam ChatVersion = "version";
        
        public static readonly OperationParam ChatChannelType = "channelType";
        public static readonly OperationParam ChatChannelSubId = "channelSubId";
        public static readonly OperationParam ChatContent = "content";
        public static readonly OperationParam ChatText = "text";

        public static readonly OperationParam ChatRich = "rich";
        public static readonly OperationParam ChatRecordInHistory = "recordInHistory";

        // TODO:: do we enumerate these ? [smrj]
        // chat channel types 
        public static readonly OperationParam AllChannelType = "all";
        public static readonly OperationParam GlobalChannelType = "gl";
        public static readonly OperationParam GroupChannelType = "gr";

        // messaging
        public static readonly OperationParam MessagingMessageBox = "msgbox";
        public static readonly OperationParam MessagingMessageIds = "msgIds";
        public static readonly OperationParam MessagingMarkAsRead = "markAsRead";
        public static readonly OperationParam MessagingContext = "context";
        public static readonly OperationParam MessagingPageOffset = "pageOffset";
        public static readonly OperationParam MessagingFromName = "fromName";
        public static readonly OperationParam MessagingToProfileIds = "toProfileIds";
        public static readonly OperationParam MessagingContent = "contentJson";
        public static readonly OperationParam MessagingSubject = "subject";
        public static readonly OperationParam MessagingText = "text";

        public static readonly OperationParam InboxMessageType = "inbox";
        public static readonly OperationParam SentMessageType = "sent";

        // lobby
        public static readonly OperationParam EntryId = "entryId";
        public static readonly OperationParam LobbyRoomType = "lobbyType";
        public static readonly OperationParam LobbyTypes = "lobbyTypes";
        public static readonly OperationParam LobbyRating = "rating";
        public static readonly OperationParam LobbyAlgorithm = "algo";
        public static readonly OperationParam LobbyMaxSteps = "maxSteps";
        public static readonly OperationParam LobbyStrategy = "strategy";
        public static readonly OperationParam LobbyAlignment = "alignment";
        public static readonly OperationParam LobbyRanges = "ranges";
        public static readonly OperationParam LobbyFilterJson = "filterJson";
        public static readonly OperationParam LobbySettings = "settings";
        public static readonly OperationParam LobbyTimeoutSeconds = "timeoutSecs";
        public static readonly OperationParam LobbyIsReady = "isReady";
        public static readonly OperationParam LobbyOtherUserCxIds = "otherUserCxIds";
        public static readonly OperationParam LobbyExtraJson = "extraJson";
        public static readonly OperationParam LobbyTeamCode = "teamCode";
        public static readonly OperationParam LobbyIdentifier = "lobbyId";
        public static readonly OperationParam LobbyToTeamName = "toTeamCode";
        public static readonly OperationParam LobbySignalData = "signalData";
        public static readonly OperationParam LobbyConnectionId = "cxId";
        public static readonly OperationParam PingData = "pingData";
        public static readonly OperationParam LobbyMinRating = "minRating";
        public static readonly OperationParam LobbyMaxRating = "maxRating";

        public static readonly OperationParam CompoundAlgos = "algos";
        public static readonly OperationParam CompoundRanges = "compound-ranges";
        public static readonly OperationParam LobbyCritera = "criteriaJson";
        public static readonly OperationParam Critera = "criteria";
        public static readonly OperationParam CriteraPing = "ping";
        public static readonly OperationParam CriteraRating = "rating";
        public static readonly OperationParam StrategyRangedPercent = "ranged-percent";
        public static readonly OperationParam StrategyRangedAbsolute = "ranged-absolute";
        public static readonly OperationParam StrategyAbsolute = "absolute";
        public static readonly OperationParam StrategyCompound = "compound";
        public static readonly OperationParam AlignmentCenter = "center";
        //custom entity
        public static readonly OperationParam CustomEntityServiceEntityType = "entityType";
        public static readonly OperationParam CustomEntityServiceDeleteCriteria = "deleteCriteria";
        public static readonly OperationParam CustomEntityServiceEntityId = "entityId";
        public static readonly OperationParam CustomEntityServiceVersion = "version";
        public static readonly OperationParam CustomEntityServiceFieldsJson = "fieldsJson";
        public static readonly OperationParam CustomEntityServiceWhereJson = "whereJson";
        public static readonly OperationParam CustomEntityServiceRowsPerPage = "rowsPerPage";
        public static readonly OperationParam CustomEntityServiceSearchJson = "searchJson";
        public static readonly OperationParam CustomEntityServiceSortJson = "sortJson";
        public static readonly OperationParam CustomEntityServiceDoCount = "doCount";
        public static readonly OperationParam CustomEntityServiceContext = "context";
        public static readonly OperationParam CustomEntityServicePageOffset= "pageOffset";
        public static readonly OperationParam CustomEntityServiceTimeToLive = "timeToLive";
        public static readonly OperationParam CustomEntityServiceAcl = "acl";
        public static readonly OperationParam CustomEntityServiceDataJson = "dataJson";
        public static readonly OperationParam CustomEntityServiceIsOwned = "isOwned";
        public static readonly OperationParam CustomEntityServiceMaxReturn = "maxReturn";
        public static readonly OperationParam CustomEntityServiceShardKeyJson = "shardKeyJson";

        //item catalog
        public static readonly OperationParam ItemCatalogServiceDefId = "defId";
        public static readonly OperationParam ItemCatalogServiceContext = "context";
        public static readonly OperationParam ItemCatalogServicePageOffset = "pageOffset";

        //userInventory
        public static readonly OperationParam UserItemsServiceDefId = "defId";
        public static readonly OperationParam UserItemsServiceQuantity = "quantity";
        public static readonly OperationParam UserItemsServiceIncludeDef = "includeDef";
        public static readonly OperationParam UserItemsServiceItemId = "itemId";
        public static readonly OperationParam UserItemsServiceCriteria = "criteria";
        public static readonly OperationParam UserItemsServiceContext = "context";
        public static readonly OperationParam UserItemsServicePageOffset = "pageOffset";
        public static readonly OperationParam UserItemsServiceVersion = "version";
        public static readonly OperationParam UserItemsServiceImmediate = "immediate";
        public static readonly OperationParam UserItemsServiceProfileId = "profileId";
        public static readonly OperationParam UserItemsServiceShopId = "shopId";
        public static readonly OperationParam UserItemsServiceNewItemData = "newItemData";

        //global app
        public static readonly OperationParam GlobalAppPropertyNames = "propertyNames";
        public static readonly OperationParam GlobalAppCategories = "categories";

        #endregion

        private OperationParam(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            if (obj is not OperationParam c)
                return false;

            return Equals(c);
        }

        public bool Equals(OperationParam other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public int CompareTo(OperationParam other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(OperationParam v) => v.Value;

        public static implicit operator OperationParam(string v) => new(v);

        public static bool operator ==(OperationParam v1, OperationParam v2) => v1.Equals(v2);

        public static bool operator !=(OperationParam v1, OperationParam v2) => !(v1 == v2);

        public static bool operator >(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(OperationParam v1, OperationParam v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
