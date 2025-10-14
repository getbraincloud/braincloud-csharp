// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Collections.Generic;

namespace BrainCloud.Common
{
    public readonly struct AuthenticationType : System.IEquatable<AuthenticationType>, System.IComparable<AuthenticationType>
    {
        #region brainCloud Authentication Types

        public static readonly AuthenticationType Anonymous           = new("Anonymous");
        public static readonly AuthenticationType Universal           = new("Universal");
        public static readonly AuthenticationType Email               = new("Email");
        public static readonly AuthenticationType Facebook            = new("Facebook");
        public static readonly AuthenticationType FacebookLimited     = new("FacebookLimited");
        public static readonly AuthenticationType Oculus              = new("Oculus");
        public static readonly AuthenticationType PlaystationNetwork  = new("PlaystationNetwork");
        public static readonly AuthenticationType PlaystationNetwork5 = new("PlaystationNetwork5");
        public static readonly AuthenticationType GameCenter          = new("GameCenter");
        public static readonly AuthenticationType Steam               = new("Steam");
        public static readonly AuthenticationType Apple               = new("Apple");
        public static readonly AuthenticationType Google              = new("Google");
        public static readonly AuthenticationType GoogleOpenId        = new("GoogleOpenId");
        public static readonly AuthenticationType Twitter             = new("Twitter");
        public static readonly AuthenticationType Parse               = new("Parse");
        public static readonly AuthenticationType External            = new("External");
        public static readonly AuthenticationType Handoff             = new("Handoff");
        public static readonly AuthenticationType SettopHandoff       = new("SettopHandoff");
        public static readonly AuthenticationType Ultra               = new("Ultra");
        public static readonly AuthenticationType Nintendo            = new("Nintendo");
        public static readonly AuthenticationType Unknown             = new("UNKNOWN");

        private static readonly Dictionary<string, AuthenticationType> _typesForString = new()
        {
            { Anonymous.value,          Anonymous          },
            { Universal.value,          Universal          },
            { Email.value,              Email              },
            { Facebook.value,           Facebook           },
            { FacebookLimited.value,    FacebookLimited    },
            { Oculus.value,             Oculus             },
            { PlaystationNetwork.value, PlaystationNetwork },
            { GameCenter.value,         GameCenter         },
            { Steam.value,              Steam              },
            { Apple.value,              Apple              },
            { Google.value,             Google             },
            { GoogleOpenId.value,       GoogleOpenId       },
            { Twitter.value,            Twitter            },
            { Parse.value,              Parse              },
            { Handoff.value,            Handoff            },
            { External.value,           External           },
            { SettopHandoff.value,      SettopHandoff      },
            { Unknown.value,            Unknown            }
        };

        #endregion

        private AuthenticationType(string value)
        {
            this.value = value;
        }

        private readonly string value;

        public static AuthenticationType FromString(string s)
        {
            return _typesForString.TryGetValue(s, out AuthenticationType platform) ? platform : Unknown;
        }

        #region Overrides and Operators

        public readonly override bool Equals(object obj)
        {
            if (obj is not AuthenticationType s)
                return false;

            return Equals(s);
        }

        public readonly bool Equals(AuthenticationType other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return value == other.value;
        }

        public readonly int CompareTo(AuthenticationType other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return value.CompareTo(other.value);
        }

        public readonly override int GetHashCode() => value.GetHashCode();

        public readonly override string ToString() => value;

        public static implicit operator string(AuthenticationType v) => v.value;

        public static bool operator ==(AuthenticationType v1, AuthenticationType v2) => v1.Equals(v2);

        public static bool operator !=(AuthenticationType v1, AuthenticationType v2) => !(v1 == v2);

        public static bool operator >(AuthenticationType v1, AuthenticationType v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(AuthenticationType v1, AuthenticationType v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(AuthenticationType v1, AuthenticationType v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(AuthenticationType v1, AuthenticationType v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
