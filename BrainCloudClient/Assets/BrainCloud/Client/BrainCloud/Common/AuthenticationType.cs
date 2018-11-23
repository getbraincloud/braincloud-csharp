//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;

namespace BrainCloud.Common
{
    public sealed class AuthenticationType
    {
        private readonly string value;

        public static readonly AuthenticationType Anonymous = new AuthenticationType("Anonymous");
        public static readonly AuthenticationType Universal = new AuthenticationType("Universal");
        public static readonly AuthenticationType Email = new AuthenticationType("Email");
        public static readonly AuthenticationType Facebook = new AuthenticationType("Facebook");
        public static readonly AuthenticationType GameCenter = new AuthenticationType("GameCenter");
        public static readonly AuthenticationType Steam = new AuthenticationType("Steam");
        public static readonly AuthenticationType Google = new AuthenticationType("Google");
        public static readonly AuthenticationType Twitter = new AuthenticationType("Twitter");
        public static readonly AuthenticationType Parse = new AuthenticationType("Parse");
        public static readonly AuthenticationType Handoff = new AuthenticationType("Handoff");
        public static readonly AuthenticationType External = new AuthenticationType("External");
        public static readonly AuthenticationType Unknown = new AuthenticationType("UNKNOWN");

        private static readonly Dictionary<string, AuthenticationType> _typesForString = new Dictionary<string, AuthenticationType>
        {
            { Anonymous.value, Anonymous },
            { Universal.value, Universal },
            { Email.value, Email },
            { Facebook.value, Facebook },
            { GameCenter.value, GameCenter },
            { Steam.value, Steam },
            { Google.value, Google },
            { Twitter.value, Twitter },
            { Parse.value, Parse },
            { Handoff.value, Handoff},
            { External.value, External },
            { Unknown.value, Unknown }
        };

        private AuthenticationType(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public static AuthenticationType FromString(string s)
        {
            AuthenticationType platform;
            return _typesForString.TryGetValue(s, out platform) ? platform : Unknown;
        }
    }
}

