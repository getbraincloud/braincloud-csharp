// Copyright 2026 bitHeads, Inc. All Rights Reserved.
//----------------------------------------------------
// brainCloud client source code
//----------------------------------------------------

using System.Collections.Generic;

#if XAMARIN
using Xamarin.Forms;
#endif

#if GODOT
using Godot;
#endif

namespace BrainCloud.Common
{
    public readonly struct Platform : System.IEquatable<Platform>, System.IComparable, System.IComparable<Platform>
    {
        #region brainCloud Platforms

        public static readonly Platform Unknown           = new Platform("UNKNOWN");
        public static readonly Platform Amazon            = new Platform("AMAZON");
        public static readonly Platform AppleTVOS         = new Platform("APPLE_TV_OS");
        public static readonly Platform BlackBerry        = new Platform("BB");
        public static readonly Platform Facebook          = new Platform("FB");
        public static readonly Platform GooglePlayAndroid = new Platform("ANG");
        public static readonly Platform iOS               = new Platform("IOS");
        public static readonly Platform Linux             = new Platform("LINUX");
        public static readonly Platform Mac               = new Platform("MAC");
        public static readonly Platform Nintendo          = new Platform("NINTENDO");
        public static readonly Platform Oculus            = new Platform("OCULUS");
        public static readonly Platform PS3               = new Platform("PS3");
        public static readonly Platform PS4               = new Platform("PS4");
        public static readonly Platform PSVita            = new Platform("PS_VITA");
        public static readonly Platform Roku              = new Platform("ROKU");
        public static readonly Platform Tizen             = new Platform("TIZEN");
        public static readonly Platform VisionOS          = new Platform("VISION_OS");
        public static readonly Platform WatchOS           = new Platform("WATCH_OS");
        public static readonly Platform Web               = new Platform("WEB");
        public static readonly Platform Wii               = new Platform("WII");
        public static readonly Platform Windows           = new Platform("WINDOWS");
        public static readonly Platform WindowsPhone      = new Platform("WINP");
        public static readonly Platform Xbox360           = new Platform("XBOX_360");
        public static readonly Platform XboxOne           = new Platform("XBOX_ONE");

        private static readonly Dictionary<string, Platform> _platformsForString = new Dictionary<string, Platform>()
        {
            { Unknown.value,           Unknown           },
            { Amazon.value,            Amazon            },
            { AppleTVOS.value,         AppleTVOS         },
            { BlackBerry.value,        BlackBerry        },
            { Facebook.value,          Facebook          },
            { GooglePlayAndroid.value, GooglePlayAndroid },
            { iOS.value,               iOS               },
            { Linux.value,             Linux             },
            { Mac.value,               Mac               },
            { Nintendo.value,          Nintendo          },
            { Oculus.value,            Oculus            },
            { PS3.value,               PS3               },
            { PS4.value,               PS4               },
            { PSVita.value,            PSVita            },
            { Roku.value,              Roku              },
            { Tizen.value,             Tizen             },
            { VisionOS.value,          VisionOS          },
            { WatchOS.value,           WatchOS           },
            { Web.value,               Web               },
            { Wii.value,               Wii               },
            { Windows.value,           Windows           },
            { WindowsPhone.value,      WindowsPhone      },
            { Xbox360.value,           Xbox360           },
            { XboxOne.value,           XboxOne           }
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
            // This kicks in if the DLL is compiled from the Visual Studio solution
#if NO_UNITY_DEFINES
            return Unknown;
#else
            // First deal with Platforms that have no defines
            // 5.0 and later
#if !UNITY_4_6 && !UNITY_2018_3_OR_NEWER
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.PSP2)
            {
                return PSVita;
            }
#endif
            // Otherwise we rely on the Unity compile flag to denote Platform
#if UNITY_STANDALONE_WIN
            return Windows;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_IOS || UNITY_IPHONE || UNITY_TVOS || UNITY_VISIONOS
            var platform = UnityEngine.Device.Application.platform;
            if (platform == UnityEngine.RuntimePlatform.IPhonePlayer)
            {
                return iOS;
            }
            else if (platform == UnityEngine.RuntimePlatform.tvOS)
            {
                return AppleTVOS;
            }
            else if (platform == UnityEngine.RuntimePlatform.VisionOS)
            {
                return VisionOS;
            }
            return Mac;
#elif UNITY_STANDALONE_LINUX
            return Linux;
#elif UNITY_WEBPLAYER || UNITY_WEBGL
            return Web;
#elif UNITY_ANDROID
            string check = UnityEngine.SystemInfo.deviceModel.ToLower();
            if (check.Contains("amazon"))
            {
                return Amazon;
            }
            else if (check.Contains("oculus") ||
                     check.Contains("quest"))
            {
                return Oculus;
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
#elif UNITY_SWITCH
            return Nintendo;
#else
            return Unknown;
#endif

#endif // NO_UNITY_DEFINES
        }
#endif

#if GODOT
        public static Platform GodotFromRuntime()
        {
            Platform platform = Unknown;
            switch(OS.GetName())
            {
                case "Windows":
                    platform = Windows;
                    break;
                case "macOS":
                    platform = Mac;
                    break;
                case "Linux":
                case "FreeBSD":
                case "NetBSD":
                case "OpenBSD":
                case "BSD":
                    platform = Linux;
                    break;
                case "Android":
                    platform = GooglePlayAndroid;
                    break;
                case "iOS":
                    platform = iOS;
                    break;
                case "Web":
                    platform = Web;
                    break;
            }

            return platform;
        }
#endif

#if XAMARIN
        public static Platform FromRuntime()
        {
            Platform platform = Unknown;
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        platform = iOS;
                        break;
                    case Device.macOS:
                        platform = Mac;
                        break;
                    case Device.Android:
                        platform = GooglePlayAndroid;
                        break;
                    case Device.WPF:
                        platform = Windows;
                        break;
                }
            }
            catch{}

            return platform;
        }
#endif

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            return obj is Platform other && Equals(other);
        }

        public bool Equals(Platform other)
        {
            return value == other.value;
        }

        public int CompareTo(object obj)
        {
            if (obj is Platform other)
            {
                return CompareTo(other);
            }

            return 1;
        }

        public int CompareTo(Platform other)
        {
            return value.CompareTo(other.value);
        }

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value;

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
