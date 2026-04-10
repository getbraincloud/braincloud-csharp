// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Collections.Generic;

namespace BrainCloud.Common
{
    public readonly struct AuthenticationType : System.IEquatable<AuthenticationType>, System.IComparable, System.IComparable<AuthenticationType>
    {
        #region brainCloud Authentication Types

        public static readonly AuthenticationType Anonymous           = new AuthenticationType("Anonymous");
        public static readonly AuthenticationType Universal           = new AuthenticationType("Universal");
        public static readonly AuthenticationType Email               = new AuthenticationType("Email");
        public static readonly AuthenticationType Facebook            = new AuthenticationType("Facebook");
        public static readonly AuthenticationType FacebookLimited     = new AuthenticationType("FacebookLimited");
        public static readonly AuthenticationType Oculus              = new AuthenticationType("Oculus");
        public static readonly AuthenticationType PlaystationNetwork  = new AuthenticationType("PlaystationNetwork");
        public static readonly AuthenticationType PlaystationNetwork5 = new AuthenticationType("PlaystationNetwork5");
        public static readonly AuthenticationType GameCenter          = new AuthenticationType("GameCenter");
        public static readonly AuthenticationType Steam               = new AuthenticationType("Steam");
        public static readonly AuthenticationType Apple               = new AuthenticationType("Apple");
        public static readonly AuthenticationType Google              = new AuthenticationType("Google");
        public static readonly AuthenticationType GoogleOpenId        = new AuthenticationType("GoogleOpenId");
        public static readonly AuthenticationType Twitter             = new AuthenticationType("Twitter");
        public static readonly AuthenticationType Parse               = new AuthenticationType("Parse");
        public static readonly AuthenticationType External            = new AuthenticationType("External");
        public static readonly AuthenticationType Handoff             = new AuthenticationType("Handoff");
        public static readonly AuthenticationType SettopHandoff       = new AuthenticationType("SettopHandoff");
        public static readonly AuthenticationType Ultra               = new AuthenticationType("Ultra");
        public static readonly AuthenticationType Nintendo            = new AuthenticationType("Nintendo");
        public static readonly AuthenticationType Unknown             = new AuthenticationType("UNKNOWN");

        private static readonly Dictionary<string, AuthenticationType> _typesForString = new Dictionary<string, AuthenticationType>()
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

        public override bool Equals(object obj)
        {
            return obj is AuthenticationType other && Equals(other);
        }

        public bool Equals(AuthenticationType other)
        {
            return value == other.value;
        }

        public int CompareTo(object obj)
        {
            if (obj is AuthenticationType other)
            {
                return CompareTo(other);
            }

            return 1;
        }

        public int CompareTo(AuthenticationType other)
        {
            return value.CompareTo(other.value);
        }

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value;

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
