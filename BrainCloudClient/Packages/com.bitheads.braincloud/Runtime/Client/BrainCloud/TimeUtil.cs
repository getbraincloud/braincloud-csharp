//----------------------------------------------------
// brainCloud client source code
// Copyright 2020 bitHeads, inc.
//----------------------------------------------------

namespace BrainCloud
{

using System;
using System.Collections.Generic;
using System.Text;
              
    public static class TimeUtil
    {
        /// <summary>
        /// Converts UTC DateTime to UTC Milliseconds 
        /// Pass in your time as a UTC time 
        /// You can use LocalTimeToUTCTime or just yourDateTime.ToUniversalTime() to achieve this.
        /// </summary>
        public static long UTCDateTimeToUTCMillis(this DateTime utcDateTime)
        {
            var ticks = utcDateTime.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var ts = ticks / TimeSpan.TicksPerMillisecond;
            return ts;
        }

        /// <summary>
        /// Converts UTC Milliseconds to UTC DateTime
        /// If you need UTC Milliseconds, you can use UTCDateTimeToUTCMillis.
        /// If you need UTC time to get the UTC milliseconds, you can use LocalTimeToUTCTime.
        /// </summary>
        public static DateTime UTCMillisToUTCDateTime(this long utcMillis)
        {
            var timeInTicks = utcMillis * TimeSpan.TicksPerMillisecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
        }

        /// <summary>
        /// Returns local time as UTC time
        /// </summary>
        public static DateTime LocalTimeToUTCTime(this DateTime localDate)
        {
            return localDate.ToUniversalTime();
        }

        /// <summary>
        /// Returns UTC time as Local time
        /// </summary>
        public static DateTime UTCTimeToLocalTime(this DateTime utcDate)
        {
            return utcDate.ToLocalTime();
        }
        
        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long DateTimeOffsetToUTCMillis(this DateTimeOffset utcDateTimeOffset)
        {
            var ticks = utcDateTimeOffset.Ticks - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).Ticks;
            var ts = ticks / TimeSpan.TicksPerMillisecond;
            return ts;
        }
        
        /// <summary>
        /// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
        /// </summary>
        public static DateTimeOffset UTCMillisToDateTimeOffset(this long utcMillis)
        {
            var timeInTicks = utcMillis * TimeSpan.TicksPerMillisecond;
            return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddTicks(timeInTicks);
        }

        /// <summary>
        /// Returns local time as UTC time
        /// </summary>
        public static DateTimeOffset LocalTimeToUTCTime(this DateTimeOffset localDate)
        {
            return localDate.ToUniversalTime();
        }

        /// <summary>
        /// Returns UTC time as Local time
        /// </summary>
        public static DateTimeOffset UTCTimeToLocalTime(this DateTimeOffset utcDate)
        {
            return utcDate.ToLocalTime();
        }
    }
}
