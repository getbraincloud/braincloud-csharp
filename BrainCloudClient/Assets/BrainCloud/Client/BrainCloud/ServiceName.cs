// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code

//----------------------------------------------------

namespace BrainCloud
{
    public struct ServiceName : System.IEquatable<ServiceName>, System.IComparable<ServiceName>
    {
        #region brainCloud Service Names

        // Services
        public static readonly ServiceName AsyncMatch = "asyncMatch";
        public static readonly ServiceName Authenticate = "authenticationV2";
        public static readonly ServiceName DataStream = "dataStream";
        public static readonly ServiceName Entity = "entity";
        public static readonly ServiceName Event = "event";
        public static readonly ServiceName File = "file";
        public static readonly ServiceName Friend = "friend";
        public static readonly ServiceName Gamification = "gamification";
        public static readonly ServiceName GlobalApp = "globalApp";
        public static readonly ServiceName GlobalEntity = "globalEntity";
        public static readonly ServiceName GlobalStatistics = "globalGameStatistics";
        public static readonly ServiceName Group = "group";
        public static readonly ServiceName HeartBeat = "heartbeat";
        public static readonly ServiceName Identity = "identity";
        public static readonly ServiceName ItemCatalog = "itemCatalog";
        public static readonly ServiceName UserItems = "userItems";
        public static readonly ServiceName Mail = "mail";
        public static readonly ServiceName MatchMaking = "matchMaking";
        public static readonly ServiceName OneWayMatch = "onewayMatch";
        public static readonly ServiceName PlaybackStream = "playbackStream";
        public static readonly ServiceName PlayerState = "playerState";
        public static readonly ServiceName PlayerStatistics = "playerStatistics";
        public static readonly ServiceName PlayerStatisticsEvent = "playerStatisticsEvent";
        public static readonly ServiceName Presence = "presence";
        public static readonly ServiceName Profanity = "profanity";
        public static readonly ServiceName PushNotification = "pushNotification";
        public static readonly ServiceName RedemptionCode = "redemptionCode";
        public static readonly ServiceName S3Handling = "s3Handling";
        public static readonly ServiceName Script = "script";
        public static readonly ServiceName ServerTime = "time";
        public static readonly ServiceName Leaderboard = "leaderboard";
        public static readonly ServiceName Twitter = "twitter";
        public static readonly ServiceName Time = "time";
        public static readonly ServiceName Tournament = "tournament";
        public static readonly ServiceName GlobalFile = "globalFileV3";
        public static readonly ServiceName CustomEntity = "customEntity";
        public static readonly ServiceName RTTRegistration = "rttRegistration";
        public static readonly ServiceName RTT = "rtt";
        public static readonly ServiceName Relay = "relay";
        public static readonly ServiceName Chat = "chat";
        public static readonly ServiceName Messaging = "messaging";
        public static readonly ServiceName Lobby = "lobby";
        public static readonly ServiceName VirtualCurrency = "virtualCurrency";
        public static readonly ServiceName AppStore = "appStore";
        public static readonly ServiceName BlockChain = "blockchain";
        public static readonly ServiceName GroupFile = "groupFile";

        #endregion

        private ServiceName(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            if (obj is not ServiceName c)
                return false;

            return Equals(c);
        }

        public bool Equals(ServiceName other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Value == other.Value;
        }

        public int CompareTo(ServiceName other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return Value.CompareTo(other.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;

        public static implicit operator string(ServiceName v) => v.Value;

        public static implicit operator ServiceName(string v) => new(v);

        public static bool operator ==(ServiceName v1, ServiceName v2) => v1.Equals(v2);

        public static bool operator !=(ServiceName v1, ServiceName v2) => !(v1 == v2);

        public static bool operator >(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(ServiceName v1, ServiceName v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
