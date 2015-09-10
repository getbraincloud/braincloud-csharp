//----------------------------------------------------
// brainCloud client source code
// Copyright 2015 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using LitJson;

namespace BrainCloud.Common
{
    [Obsolete("Use the Platform class instead - removal in 90 days, 2015-12-15")]
    class Constants
    {
        public readonly static string PlatformIOS = "IOS";
        public readonly static string PlatformOSX = "MAC";
        public readonly static string PlatformBlackberry = "BB";
        public readonly static string PlatformFacebook = "FB";
        public readonly static string PlatformWindows = "WINDOWS";
        public readonly static string PlatformWindowsPhone = "WINP";
        public readonly static string PlatformGooglePlayAndroid = "ANG";
        public readonly static string PlatformLinux = "LINUX";
    }
}
