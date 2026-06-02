// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    public readonly struct ServiceName : System.IEquatable<ServiceName>, System.IComparable, System.IComparable<ServiceName>
    {
        #region brainCloud Service Names

        // Services
        public static readonly ServiceName AppStore              = new ServiceName("appStore");
        public static readonly ServiceName AsyncMatch            = new ServiceName("asyncMatch");
        public static readonly ServiceName Authenticate          = new ServiceName("authenticationV2");
        public static readonly ServiceName BlockChain            = new ServiceName("blockchain");
        public static readonly ServiceName Campaign              = new ServiceName("campaign");
        public static readonly ServiceName Chat                  = new ServiceName("chat");
        public static readonly ServiceName CustomEntity          = new ServiceName("customEntity");
        public static readonly ServiceName DataStream            = new ServiceName("dataStream");
        public static readonly ServiceName Entity                = new ServiceName("entity");
        public static readonly ServiceName Event                 = new ServiceName("event");
        public static readonly ServiceName File                  = new ServiceName("file");
        public static readonly ServiceName Friend                = new ServiceName("friend");
        public static readonly ServiceName Gamification          = new ServiceName("gamification");
        public static readonly ServiceName GlobalApp             = new ServiceName("globalApp");
        public static readonly ServiceName GlobalEntity          = new ServiceName("globalEntity");
        public static readonly ServiceName GlobalFile            = new ServiceName("globalFileV3");
        public static readonly ServiceName GlobalStatistics      = new ServiceName("globalGameStatistics");
        public static readonly ServiceName Group                 = new ServiceName("group");
        public static readonly ServiceName GroupFile             = new ServiceName("groupFile");
        public static readonly ServiceName HeartBeat             = new ServiceName("heartbeat");
        public static readonly ServiceName Identity              = new ServiceName("identity");
        public static readonly ServiceName ItemCatalog           = new ServiceName("itemCatalog");
        public static readonly ServiceName Leaderboard           = new ServiceName("leaderboard");
        public static readonly ServiceName Lobby                 = new ServiceName("lobby");
        public static readonly ServiceName Mail                  = new ServiceName("mail");
        public static readonly ServiceName MatchMaking           = new ServiceName("matchMaking");
        public static readonly ServiceName Messaging             = new ServiceName("messaging");
        public static readonly ServiceName OneWayMatch           = new ServiceName("onewayMatch");
        public static readonly ServiceName PlaybackStream        = new ServiceName("playbackStream");
        public static readonly ServiceName PlayerState           = new ServiceName("playerState");
        public static readonly ServiceName PlayerStatistics      = new ServiceName("playerStatistics");
        public static readonly ServiceName PlayerStatisticsEvent = new ServiceName("playerStatisticsEvent");
        public static readonly ServiceName Presence              = new ServiceName("presence");
        public static readonly ServiceName Profanity             = new ServiceName("profanity");
        public static readonly ServiceName PushNotification      = new ServiceName("pushNotification");
        public static readonly ServiceName RedemptionCode        = new ServiceName("redemptionCode");
        public static readonly ServiceName Relay                 = new ServiceName("relay");
        public static readonly ServiceName RTT                   = new ServiceName("rtt");
        public static readonly ServiceName RTTRegistration       = new ServiceName("rttRegistration");
        public static readonly ServiceName S3Handling            = new ServiceName("s3Handling");
        public static readonly ServiceName Script                = new ServiceName("script");
        public static readonly ServiceName ServerTime            = new ServiceName("time");
        public static readonly ServiceName Time                  = new ServiceName("time");
        public static readonly ServiceName Tournament            = new ServiceName("tournament");
        public static readonly ServiceName Twitter               = new ServiceName("twitter");
        public static readonly ServiceName UserItems             = new ServiceName("userItems");
        public static readonly ServiceName VirtualCurrency       = new ServiceName("virtualCurrency");

        #endregion

        private ServiceName(string value)
        {
            Value = value;
        }

        public readonly string Value;

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            return obj is ServiceName other && Equals(other);
        }

        public bool Equals(ServiceName other)
        {
            return Value == other.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is ServiceName other)
            {
                return CompareTo(other);
            }

            return 1;
        }

        public int CompareTo(ServiceName other)
        {
            return Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(ServiceName v) => v.Value;

        public static bool operator ==(ServiceName v1, ServiceName v2) => v1.Equals(v2);

        public static bool operator !=(ServiceName v1, ServiceName v2) => !(v1 == v2);

        public static bool operator >(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
