//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System.Runtime.InteropServices;
#if !DOT_NET
using UnityEngine;
#endif

public class RegionLocale
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string _GetUsersCountryLocale();
#endif

    protected static string m_countryLocale = "";
    public static string UsersCountryLocale
    {
        get
        {
            if (m_countryLocale == "")
            {
                GetCountryLocale();
            }
            return m_countryLocale;
        }
    }

    protected static void GetCountryLocale()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        m_countryLocale = _GetUsersCountryLocale();
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject activityContext = jc.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject regionLocaleNative = new AndroidJavaObject("com.braincloud.unity.RegionLocaleNative");
        if (regionLocaleNative != null)
        {
            m_countryLocale = regionLocaleNative.CallStatic<string>("GetUsersCountryLocale", activityContext);
        }
#endif
    }
}
