//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud.Internal
{
    internal class ServiceName
    {
        // Services
        public static readonly ServiceName AsyncMatch = new ServiceName("asyncMatch");
        public static readonly ServiceName Authenticate = new ServiceName("authenticationV2");
        public static readonly ServiceName DataStream = new ServiceName("dataStream");
        public static readonly ServiceName Entity = new ServiceName("entity");
        public static readonly ServiceName Event = new ServiceName("event");
        public static readonly ServiceName File = new ServiceName("file");
        public static readonly ServiceName Friend = new ServiceName("friend");
        public static readonly ServiceName Gamification = new ServiceName("gamification");
        public static readonly ServiceName GlobalApp = new ServiceName("globalApp");
        public static readonly ServiceName GlobalEntity = new ServiceName("globalEntity");
        public static readonly ServiceName GlobalStatistics = new ServiceName("globalGameStatistics");
        public static readonly ServiceName Group = new ServiceName("group");
        public static readonly ServiceName HeartBeat = new ServiceName("heartbeat");
        public static readonly ServiceName Identity = new ServiceName("identity");
        public static readonly ServiceName Mail = new ServiceName("mail");
        public static readonly ServiceName MatchMaking = new ServiceName("matchMaking");
        public static readonly ServiceName OneWayMatch = new ServiceName("onewayMatch");
        public static readonly ServiceName PlaybackStream = new ServiceName("playbackStream");
        public static readonly ServiceName PlayerState = new ServiceName("playerState");
        public static readonly ServiceName PlayerStatistics = new ServiceName("playerStatistics");
        public static readonly ServiceName PlayerStatisticsEvent = new ServiceName("playerStatisticsEvent");
        public static readonly ServiceName Presence = new ServiceName("presence");
        public static readonly ServiceName Product = new ServiceName("product");
        public static readonly ServiceName Profanity = new ServiceName("profanity");
        public static readonly ServiceName PushNotification = new ServiceName("pushNotification");
        public static readonly ServiceName RedemptionCode = new ServiceName("redemptionCode");
        public static readonly ServiceName S3Handling = new ServiceName("s3Handling");
        public static readonly ServiceName Script = new ServiceName("script");
        public static readonly ServiceName ServerTime = new ServiceName("time");
        public static readonly ServiceName Leaderboard = new ServiceName("leaderboard");
        public static readonly ServiceName Twitter = new ServiceName("twitter");
        public static readonly ServiceName Time = new ServiceName("time");
        public static readonly ServiceName Tournament = new ServiceName("tournament");
        public static readonly ServiceName RTTRegistration = new ServiceName("rttRegistration");
        public static readonly ServiceName RTT = new ServiceName("rtt");
        public static readonly ServiceName Chat = new ServiceName("chat");
        public static readonly ServiceName Messaging = new ServiceName("messaging");
        public static readonly ServiceName Lobby = new ServiceName("lobby");
        public static readonly ServiceName VirtualCurrency = new ServiceName("virtualCurrency");
        public static readonly ServiceName AppStore = new ServiceName("appStore");

        private ServiceName(string value)
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
