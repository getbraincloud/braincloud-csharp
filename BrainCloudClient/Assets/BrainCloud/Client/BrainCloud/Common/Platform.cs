//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Collections.Generic;

namespace BrainCloud.Common
{
    public sealed class Platform
    {
        private readonly string value;

        public static readonly Platform AppleTVOS = new Platform("APPLE_TV_OS");
        public static readonly Platform BlackBerry = new Platform("BB");
        public static readonly Platform Facebook = new Platform("FB");
        public static readonly Platform GooglePlayAndroid = new Platform("ANG");
        public static readonly Platform iOS = new Platform("IOS");
        public static readonly Platform Linux = new Platform("LINUX");
        public static readonly Platform Mac = new Platform("MAC");
        public static readonly Platform PS3 = new Platform("PS3");
        public static readonly Platform PS4 = new Platform("PS4");
        public static readonly Platform PSVita = new Platform("PS_VITA");
        public static readonly Platform Roku = new Platform("ROKU");
        public static readonly Platform Tizen = new Platform("TIZEN");
        public static readonly Platform Unknown = new Platform("UNKNOWN");
        public static readonly Platform WatchOS = new Platform("WATCH_OS");
        public static readonly Platform Web = new Platform("WEB");
        public static readonly Platform Wii = new Platform("WII");
        public static readonly Platform WindowsPhone = new Platform("WINP");
        public static readonly Platform Windows = new Platform("WINDOWS");
        public static readonly Platform Xbox360 = new Platform("XBOX_360");
        public static readonly Platform XboxOne = new Platform("XBOX_ONE");

        private static readonly Dictionary<string, Platform> _platformsForString = new Dictionary<string, Platform>
        {
            { AppleTVOS.value, AppleTVOS },
            { BlackBerry.value, BlackBerry },
            { Facebook.value, Facebook },
            { GooglePlayAndroid.value, GooglePlayAndroid },
            { iOS.value, iOS },
            { Linux.value, Linux },
            { Mac.value, Mac },
            { PS3.value, PS3 },
            { PS4.value, PS4 },
            { PSVita.value, PSVita },
            { Roku.value, Roku },
            { Tizen.value, Tizen },
            { Unknown.value, Unknown },
            { WatchOS.value, WatchOS },
            { Web.value, Web },
            { Wii.value, Wii },
            { WindowsPhone.value, WindowsPhone },
            { Windows.value, Windows },
            { Xbox360.value, Xbox360 },
            { XboxOne.value, XboxOne }
        };

        private Platform(string value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value;
        }

        public static Platform FromString(string s)
        {
            Platform platform;
            return _platformsForString.TryGetValue(s, out platform) ? platform : Unknown;
        }

#if !(DOT_NET)
        public static Platform FromUnityRuntime()
        {
            // this kicks in if dll is compiled from visual studio solution
#if NO_UNITY_DEFINES
            return Unknown;
#else
            
            // first deal with platforms that have no define

            // 5.0 and later
#if !UNITY_4_6
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.PSP2)
            {
                return PSVita;
            }
#endif

            // otherwise we rely on the unity compile flag to denote platform

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
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
            return GooglePlayAndroid;
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
#else
            return Unknown;
#endif

#endif // NO_UNITY_DEFINES
        }
#endif
    }
}

