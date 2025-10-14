// Copyright 2025 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Collections.Generic;
#if XAMARIN
    using Xamarin.Essentials;
#endif

namespace BrainCloud.Common
{
    public readonly struct Platform : System.IEquatable<Platform>, System.IComparable<Platform>
    {
        #region brainCloud Platforms

        public static readonly Platform AppleTVOS         = new("APPLE_TV_OS");
        public static readonly Platform BlackBerry        = new("BB");
        public static readonly Platform Facebook          = new("FB");
        public static readonly Platform Oculus            = new("Oculus");
        public static readonly Platform GooglePlayAndroid = new("ANG");
        public static readonly Platform iOS               = new("IOS");
        public static readonly Platform Linux             = new("LINUX");
        public static readonly Platform Mac               = new("MAC");
        public static readonly Platform PS3               = new("PS3");
        public static readonly Platform PS4               = new("PS4");
        public static readonly Platform PSVita            = new("PS_VITA");
        public static readonly Platform Roku              = new("ROKU");
        public static readonly Platform Tizen             = new("TIZEN");
        public static readonly Platform Unknown           = new("UNKNOWN");
        public static readonly Platform WatchOS           = new("WATCH_OS");
        public static readonly Platform Web               = new("WEB");
        public static readonly Platform Wii               = new("WII");
        public static readonly Platform WindowsPhone      = new("WINP");
        public static readonly Platform Windows           = new("WINDOWS");
        public static readonly Platform Xbox360           = new("XBOX_360");
        public static readonly Platform XboxOne           = new("XBOX_ONE");
        public static readonly Platform Amazon            = new("AMAZON");
        public static readonly Platform Nintendo          = new("NINTENDO");

        private static readonly Dictionary<string, Platform> _platformsForString = new()
        {
            { AppleTVOS.value,         AppleTVOS         },
            { Amazon.value,            Amazon            },
            { BlackBerry.value,        BlackBerry        },
            { Facebook.value,          Facebook          },
            { Oculus.value,            Oculus            },
            { GooglePlayAndroid.value, GooglePlayAndroid },
            { iOS.value,               iOS               },
            { Linux.value,             Linux             },
            { Mac.value,               Mac               },
            { PS3.value,               PS3               },
            { PS4.value,               PS4               },
            { PSVita.value,            PSVita            },
            { Roku.value,              Roku              },
            { Tizen.value,             Tizen             },
            { Unknown.value,           Unknown           },
            { WatchOS.value,           WatchOS           },
            { Web.value,               Web               },
            { Wii.value,               Wii               },
            { WindowsPhone.value,      WindowsPhone      },
            { Windows.value,           Windows           },
            { Xbox360.value,           Xbox360           },
            { XboxOne.value,           XboxOne           },
            { Nintendo.value,          Nintendo          }
        };

        #endregion

        private Platform(string value)
        {
            this.value = value;
        }

        private readonly string value;

        public static Platform FromString(string s)
        {
            return _platformsForString.TryGetValue(s, out Platform platform) ? platform : Unknown;
        }

#if !(DOT_NET || GODOT)
        public static Platform FromUnityRuntime()
        {
            // this kicks in if dll is compiled from visual studio solution
#if NO_UNITY_DEFINES
            return Unknown;
#else
            // first deal with platforms that have no define

            // 5.0 and later
#if !UNITY_4_6 && !UNITY_2018_3_OR_NEWER
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.PSP2)
            {
                return PSVita;
            }
#endif
            // otherwise we rely on the unity compile flag to denote platform

#if UNITY_STANDALONE_WIN
            return Windows;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return Mac;
#elif UNITY_STANDALONE_LINUX
            return Linux;
#elif UNITY_WEBPLAYER || UNITY_WEBGL
            return Web;
#elif UNITY_IOS || UNITY_IPHONE
            return iOS;
#elif UNITY_TVOS
            return AppleTVOS;
#elif UNITY_ANDROID
       string amazonCheck = UnityEngine.SystemInfo.deviceModel;
        if(amazonCheck.Contains("Amazon"))
        {
            return Amazon;
        }
        else
        {
            return GooglePlayAndroid;
        }
#elif UNITY_WP8 || UNITY_WP8_1
            return WindowsPhone;
#elif UNITY_WSA
            return Windows;
#elif UNITY_WII
            return Wii;
#elif UNITY_PS3
            return PS3;
#elif UNITY_PS4
            return PS4;
#elif UNITY_XBOX360
            return Xbox360;
#elif UNITY_XBOXONE
            return XboxOne;
#elif UNITY_TIZEN
            return Tizen;
#elif XAMARIN
            string checkAmazon = DeviceInfo.Manufacturer;
            if(checkAmazon.Contains("Amazon"))
            {
                return Amazon;
            }
            else
            {
                return GooglePlayAndroid;
            }
#elif UNITY_SWITCH
            return Nintendo;
#else
            return Unknown;
#endif

#endif // NO_UNITY_DEFINES
        }
#endif

        #region Overrides and Operators

        public readonly override bool Equals(object obj)
        {
            if (obj is not Platform s)
                return false;

            return Equals(s);
        }

        public readonly bool Equals(Platform other)
        {
            if (GetType() != other.GetType())
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return value == other.value;
        }

        public readonly int CompareTo(Platform other)
        {
            if (GetType() != other.GetType())
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            return value.CompareTo(other.value);
        }

        public readonly override int GetHashCode() => value.GetHashCode();

        public readonly override string ToString() => value;

        public static implicit operator string(Platform v) => v.value;

        public static bool operator ==(Platform v1, Platform v2) => v1.Equals(v2);

        public static bool operator !=(Platform v1, Platform v2) => !(v1 == v2);

        public static bool operator >(Platform v1, Platform v2) => v1.CompareTo(v2) == 1;

        public static bool operator <(Platform v1, Platform v2) => v1.CompareTo(v2) == -1;

        public static bool operator >=(Platform v1, Platform v2) => v1.CompareTo(v2) >= 0;

        public static bool operator <=(Platform v1, Platform v2) => v1.CompareTo(v2) <= 0;

        #endregion
    }
}
