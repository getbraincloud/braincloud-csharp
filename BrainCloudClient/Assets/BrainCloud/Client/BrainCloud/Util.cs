//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

#if !(DOT_NET)
using UnityEngine;
using SysLanguageObject = UnityEngine.SystemLanguage;
#else
using SysLanguageObject = System.String; // todo
#endif


namespace BrainCloud
{
    //[Serializable]
    public class Util
    {
        #region DateTime
        private static readonly DateTime s_unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime BcTimeToDateTime(long millis)
        {
            return s_unixEpoch.AddMilliseconds(millis);
        }

        public static double DateTimeToBcTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
        }
        #endregion

        #region Language
        protected static Dictionary<SysLanguageObject, String> s_langCodes = new Dictionary<SysLanguageObject, String>();
        protected static SysLanguageObject s_defaultLang;


        static Util()
        {
#if !(DOT_NET)
            s_defaultLang = SysLanguageObject.English;

            s_langCodes[SysLanguageObject.Afrikaans] = "af";
            s_langCodes[SysLanguageObject.Arabic] = "ar";
            s_langCodes[SysLanguageObject.Basque] = "eu";
            s_langCodes[SysLanguageObject.Belarusian] = "be";
            s_langCodes[SysLanguageObject.Bulgarian] = "bg";
            s_langCodes[SysLanguageObject.Catalan] = "ca";
            s_langCodes[SysLanguageObject.Chinese] = "zh";
            s_langCodes[SysLanguageObject.Czech] = "cs";
            s_langCodes[SysLanguageObject.Danish] = "da";
            s_langCodes[SysLanguageObject.Dutch] = "nl";
            s_langCodes[SysLanguageObject.English] = "en";
            s_langCodes[SysLanguageObject.Estonian] = "et";
            s_langCodes[SysLanguageObject.Faroese] = "fo";
            s_langCodes[SysLanguageObject.Finnish] = "fi";
            s_langCodes[SysLanguageObject.French] = "fr";
            s_langCodes[SysLanguageObject.German] = "de";
            s_langCodes[SysLanguageObject.Greek] = "el";
            s_langCodes[SysLanguageObject.Hebrew] = "he";
            s_langCodes[SysLanguageObject.Icelandic] = "is";
            s_langCodes[SysLanguageObject.Indonesian] = "id";
            s_langCodes[SysLanguageObject.Italian] = "it";
            s_langCodes[SysLanguageObject.Japanese] = "ja";
            s_langCodes[SysLanguageObject.Korean] = "ko";
            s_langCodes[SysLanguageObject.Latvian] = "lv";
            s_langCodes[SysLanguageObject.Lithuanian] = "lt";
            s_langCodes[SysLanguageObject.Norwegian] = "no";
            s_langCodes[SysLanguageObject.Polish] = "pl";
            s_langCodes[SysLanguageObject.Portuguese] = "pt";
            s_langCodes[SysLanguageObject.Romanian] = "ro";
            s_langCodes[SysLanguageObject.Russian] = "ru";
            s_langCodes[SysLanguageObject.SerboCroatian] = "hr";
            s_langCodes[SysLanguageObject.Slovak] = "sk";
            s_langCodes[SysLanguageObject.Slovenian] = "sl";
            s_langCodes[SysLanguageObject.Spanish] = "es";
            s_langCodes[SysLanguageObject.Swedish] = "sv";
            s_langCodes[SysLanguageObject.Thai] = "th";
            s_langCodes[SysLanguageObject.Turkish] = "tr";
            s_langCodes[SysLanguageObject.Ukrainian] = "uk";
            // don't add SysLanguageObject.Unknown as we have a default case in fns below...
            s_langCodes[SysLanguageObject.Vietnamese] = "vi";
            s_langCodes[SysLanguageObject.Hungarian] = "hu";
#else
            s_defaultLang = "en";
#endif
        }

        public static string GetIsoCodeForLanguage(SysLanguageObject lang)
        {
            string isoCode;
            if (!s_langCodes.TryGetValue(lang, out isoCode))
            {
                isoCode = "en";
            }
            return isoCode;
        }

        public static SysLanguageObject GetLanguageForIsoCode(string isoCode)
        {
            foreach (SysLanguageObject key in s_langCodes.Keys)
            {
                if (s_langCodes[key].Equals(isoCode))
                {
                    return key;
                }
            }

            return s_defaultLang;
        }

        public static string GetIsoCodeForCurrentLanguage()
        {
            string isoCode;
#if !(DOT_NET)
            isoCode = GetIsoCodeForLanguage(UnityEngine.Application.systemLanguage);
#else
            isoCode = s_defaultLang;
#endif

            return isoCode;
        }

        /// <summary> 
        /// Method returns the fractional UTC offset in hours of the current timezone.
        /// </summary>
        /// <returns>The fractional UTC offset in hours</returns>
        public static double GetUTCOffsetForCurrentTimeZone()
        {
            double utcOffset = 0;
            try
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;
                DateTime baseUTC = new DateTime();
                // Calculate the local time and UTC offset.
                DateTime localTime = localZone.ToLocalTime(baseUTC);
                TimeSpan localOffset = localZone.GetUtcOffset(localTime);
                utcOffset = localOffset.TotalHours;
            }
            catch (Exception)
            {
                // what to do now?
            }
            return utcOffset;
        }

        protected static string _usersLocale = "";

        /// <summary>
        /// Manually set the country code overriding the automatic value
        /// </summary>
        /// <param name="string">Two letter ISO country code</param>
        public static void SetCurrentCountryCode(string isoCode)
        {
            _usersLocale = isoCode;
        }

        /// <summary>
        /// Gets the current country code
        /// </summary>
        /// <returns>Two letter ISO country code</returns>
        public static string GetCurrentCountryCode()
        {
            return _usersLocale;
        }

        public static bool IsOptionalParameterValid(string s)
        {
            return (s != null && s.Length > 0);
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }
        #endregion
    }
}
