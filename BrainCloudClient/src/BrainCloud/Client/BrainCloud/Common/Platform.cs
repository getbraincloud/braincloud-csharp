using System;
namespace BrainCloud.Common
{
    public sealed class Platform
    {
        private readonly String value;

        public static readonly Platform AppleTVOS = new Platform ("APPLE_TV_OS");
        public static readonly Platform BlackBerry = new Platform ("BB");
        public static readonly Platform Facebook = new Platform ("FB");
        public static readonly Platform GooglePlayAndroid = new Platform ("ANG");
        public static readonly Platform iOS = new Platform ("IOS");
        public static readonly Platform Linux = new Platform ("LINUX");
        public static readonly Platform Mac = new Platform ("MAC");
        public static readonly Platform PS3 = new Platform ("PS3");
        public static readonly Platform PS4 = new Platform ("PS4");
        public static readonly Platform PSVita = new Platform ("PS_VITA");
        public static readonly Platform Tizen = new Platform ("TIZEN");
        public static readonly Platform Unknown = new Platform ("UNKNOWN");
        public static readonly Platform Web = new Platform ("WEB");
        public static readonly Platform Wii = new Platform ("WII");
        public static readonly Platform WindowsPhone = new Platform ("WINP");
        public static readonly Platform Windows = new Platform ("WINDOWS");
        public static readonly Platform Xbox360 = new Platform ("XBOX_360");
        public static readonly Platform XboxOne = new Platform ("XBOX_ONE");

        private Platform(String value)
        {
            this.value = value;
        }
        
        public override String ToString()
        {
            return value;
        }

        public static Platform FromString(string s)
        {
            switch(s)
            {
            case "APPLE_TV_OS":
                return AppleTVOS;
            case "BB":
                return BlackBerry;
            case "FB":
                return Facebook;
            case "ANG":
                return GooglePlayAndroid;
            case "IOS":
                return iOS;
            case "LINUX":
                return Linux;
            case "MAC":
                return Mac;
            case "PS3":
                return PS3;
            case "PS4":
                return PS4;
            case "PS_VITA":
                return PSVita;
            case "TIZEN":
                return Tizen;
            case "WEB":
                return Web;
            case "WII":
                return Wii;
            case "WINP":
                return WindowsPhone;
            case "WINDOWS":
                return Windows;
            case "XBOX_360":
                return Xbox360;
            case "XBOX_ONE":
                return XboxOne;
            default:
                return Unknown;
            }
        }

#if !(DOT_NET)
        public static Platform FromUnityRuntime()
        {
            // first deal with platforms that have no define

            // newer than 5.3
        #if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.tvOS)
            {
                return AppleTVOS;
            }
        #endif

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
        #elif UNITY_IOS
            return iOS;
        #elif UNITY_ANDROID
            return Android;
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
        }
#endif
    }
}

