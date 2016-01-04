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
        public static readonly Platform Web = new Platform ("WEB");
        public static readonly Platform WindowsPhone = new Platform ("WINP");
        public static readonly Platform Windows = new Platform ("WINDOWS");

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
            case "WEB":
                return Web;
            case "WINP":
                return WindowsPhone;
            case "WINDOWS":
                return Windows;
            default:
                throw new Exception("Unknown platform string: " + s);
            }
        }

#if !(DOT_NET)
        public static Platform FromUnityRuntime()
        {
            switch ( UnityEngine.Application.platform )
            {
            // web browser
            case UnityEngine.RuntimePlatform.WindowsWebPlayer:
            case UnityEngine.RuntimePlatform.OSXWebPlayer:
#if !UNITY_4_6
            case UnityEngine.RuntimePlatform.WebGLPlayer:
#endif
                return Web;

                // android
            case UnityEngine.RuntimePlatform.Android:
                return GooglePlayAndroid;
                
                // windows phone 8
            case UnityEngine.RuntimePlatform.WP8Player:
                return WindowsPhone;
                
                // mac osx
            case UnityEngine.RuntimePlatform.OSXEditor:
            case UnityEngine.RuntimePlatform.OSXPlayer:
            case UnityEngine.RuntimePlatform.OSXDashboardPlayer:
                return Mac;
                
                // windows desktop
            case UnityEngine.RuntimePlatform.WindowsEditor:
            case UnityEngine.RuntimePlatform.WindowsPlayer:
                return Windows;
                
                // linux
            case UnityEngine.RuntimePlatform.LinuxPlayer:
                return Linux;
                
                // ios and default
            case UnityEngine.RuntimePlatform.IPhonePlayer:
                return iOS;

#if !UNITY_4_6
            // appletv to add soon... but to which unity version???
            // case ???
                // return AppleTVOS;
#endif

            default:
                throw new Exception("Unknown unity runtime platform: " + UnityEngine.Application.platform);
            }
        }
#endif
    }
}

