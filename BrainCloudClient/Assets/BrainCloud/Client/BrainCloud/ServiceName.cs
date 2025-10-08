// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

namespace BrainCloud
{
    public readonly struct ServiceName : System.IEquatable<ServiceName>, System.IComparable<ServiceName>
    {
        #region brainCloud Service Names

        // Services
        public static readonly ServiceName AsyncMatch            = new("asyncMatch");
        public static readonly ServiceName Authenticate          = new("authenticationV2");
        public static readonly ServiceName DataStream            = new("dataStream");
        public static readonly ServiceName Entity                = new("entity");
        public static readonly ServiceName Event                 = new("event");
        public static readonly ServiceName File                  = new("file");
        public static readonly ServiceName Friend                = new("friend");
        public static readonly ServiceName Gamification          = new("gamification");
        public static readonly ServiceName GlobalApp             = new("globalApp");
        public static readonly ServiceName GlobalEntity          = new("globalEntity");
        public static readonly ServiceName GlobalStatistics      = new("globalGameStatistics");
        public static readonly ServiceName Group                 = new("group");
        public static readonly ServiceName HeartBeat             = new("heartbeat");
        public static readonly ServiceName Identity              = new("identity");
        public static readonly ServiceName ItemCatalog           = new("itemCatalog");
        public static readonly ServiceName UserItems             = new("userItems");
        public static readonly ServiceName Mail                  = new("mail");
        public static readonly ServiceName MatchMaking           = new("matchMaking");
        public static readonly ServiceName OneWayMatch           = new("onewayMatch");
        public static readonly ServiceName PlaybackStream        = new("playbackStream");
        public static readonly ServiceName PlayerState           = new("playerState");
        public static readonly ServiceName PlayerStatistics      = new("playerStatistics");
        public static readonly ServiceName PlayerStatisticsEvent = new("playerStatisticsEvent");
        public static readonly ServiceName Presence              = new("presence");
        public static readonly ServiceName Profanity             = new("profanity");
        public static readonly ServiceName PushNotification      = new("pushNotification");
        public static readonly ServiceName RedemptionCode        = new("redemptionCode");
        public static readonly ServiceName S3Handling            = new("s3Handling");
        public static readonly ServiceName Script                = new("script");
        public static readonly ServiceName ServerTime            = new("time");
        public static readonly ServiceName Leaderboard           = new("leaderboard");
        public static readonly ServiceName Twitter               = new("twitter");
        public static readonly ServiceName Time                  = new("time");
        public static readonly ServiceName Tournament            = new("tournament");
        public static readonly ServiceName GlobalFile            = new("globalFileV3");
        public static readonly ServiceName CustomEntity          = new("customEntity");
        public static readonly ServiceName RTTRegistration       = new("rttRegistration");
        public static readonly ServiceName RTT                   = new("rtt");
        public static readonly ServiceName Relay                 = new("relay");
        public static readonly ServiceName Chat                  = new("chat");
        public static readonly ServiceName Messaging             = new("messaging");
        public static readonly ServiceName Lobby                 = new("lobby");
        public static readonly ServiceName VirtualCurrency       = new("virtualCurrency");
        public static readonly ServiceName AppStore              = new("appStore");
        public static readonly ServiceName BlockChain            = new("blockchain");
        public static readonly ServiceName GroupFile             = new("groupFile");

        #endregion

        private ServiceName(string value)
        {
            Value = value;
        }

        public readonly string Value;

        #region Overrides and Operators

        public readonly override bool Equals(object obj)
        {
            if (obj is not ServiceName s)
                return false;

            return Equals(s);
        }

        public readonly bool Equals(ServiceName other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public readonly int CompareTo(ServiceName other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public readonly override int GetHashCode() => Value.GetHashCode();

        public readonly override string ToString() => Value;

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
