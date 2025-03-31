//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System;
#if UNITY_STANDALONE_WIN
    using System.Text;
#endif
#if (!(DOT_NET || GODOT))
    using UnityEngine;
#endif
#if ENABLE_WINMD_SUPPORT
    using Windows.Globalization;
#endif

    public class RegionLocale
    {
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string _GetUsersCountryLocale();
#endif

#if UNITY_STANDALONE_WIN
    [DllImport("kernel32.dll", ExactSpelling = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern int GetUserGeoID(GeoClass geoClass);

    [DllImport("kernel32.dll")]
    private static extern int GetUserDefaultLCID();

    [DllImport("kernel32.dll")]
    private static extern int GetGeoInfo(int geoid, int geoType, StringBuilder lpGeoData, int cchData, int langid);

    private enum GeoClass : int
    {
        Nation = 16,
        Region = 14,
    };
#endif

        protected static string m_countryLocale = "";

        public static string UsersCountryLocale
        {
            get
            {
                if (string.IsNullOrEmpty(m_countryLocale))
                {
                    GetCountryLocale();
                }

                return m_countryLocale;
            }
        }

        protected static void GetCountryLocale()
        {
#if !ENABLE_WINMD_SUPPORT
            //By default get it from region info
            RegionInfo regionInfo = RegionInfo.CurrentRegion;
            m_countryLocale = regionInfo.TwoLetterISORegionName;
#else
            try
            {
                // Create an instance of the GeographicRegion class from Windows.Globalization
                GeographicRegion region = new GeographicRegion();
                string regionCode = region.CodeTwoLetter;
                string displayName = region.DisplayName;
                m_countryLocale = regionCode;

                // Output the results to the Unity console
                Debug.Log($"Region Code: {regionCode}, Display Name: {displayName}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error accessing Windows.Globalization: " + ex.Message);
            }
#endif

            //For these specific cases, try to get region more accurately rather than from the system language selection
#if UNITY_IPHONE && !UNITY_EDITOR
            m_countryLocale = _GetUsersCountryLocale();
#elif UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass javaClass = new AndroidJavaClass("com.Plugins.AndroidNative.DeviceInfo");
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
            AndroidJavaObject activityContext = jc.GetStatic<AndroidJavaObject>("currentActivity");
            if(javaClass != null && activityContext != null)
            {
                m_countryLocale = javaClass.CallStatic<string>("GetCountryCode", activityContext);    
            }
#elif UNITY_STANDALONE_WIN
            GeoClass geoClass = GeoClass.Nation;
            int geoId = GetUserGeoID(geoClass);
            int lcid = GetUserDefaultLCID();
            var locationBuffer = new StringBuilder(3);
            GetGeoInfo(geoId, 4, locationBuffer, 3, lcid);
            m_countryLocale = locationBuffer.ToString();
#elif UNITY_STANDALONE_OSX
            m_countryLocale = System.Globalization.RegionInfo.CurrentRegion.ToString();
#endif

            if (m_countryLocale == "419")
            {
                m_countryLocale = "_LA_";
            }
            if (m_countryLocale == "Hans" || m_countryLocale == "Hant")
            {
                m_countryLocale = "CN";
            }
        }
    }

}