using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestTime : TestFixtureBase
    {
        [Test]
        public void TestReadServerTime()
        {
            TestResult tr = new TestResult(_bc);

            _bc.TimeService.ReadServerTime(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTimeUtil()
        {
            DateTime _dateBefore = TimeUtil.LocalTimeToUTCTime(DateTime.Now); 
            Console.WriteLine("Date: " + _dateBefore);
            Int64 _dateTimeMillis = TimeUtil.UTCDateTimeToUTCMillis(_dateBefore);
            Console.WriteLine("Date Converted to Millis: " + _dateTimeMillis);
            DateTime _dateAfter = TimeUtil.UTCMillisToUTCDateTime(_dateTimeMillis);
            Console.WriteLine("Date Converted back: " + _dateAfter);
            Int64 _dateAfterMillis = TimeUtil.UTCDateTimeToUTCMillis(_dateAfter);
            Console.WriteLine("Date Converted to Millis: " + _dateAfterMillis);
            //in c# the z time in UTC can differ by up to 0.9 seconds when converted. So the assert will never match
            Assert.AreEqual(_dateTimeMillis, _dateAfterMillis);

            DateTime _localDate = DateTime.Now; 
            Console.WriteLine("Local Date: " + _localDate);
            DateTime _utcDate = TimeUtil.LocalTimeToUTCTime(_localDate);
            Console.WriteLine("UTC Date: " + _utcDate);
            DateTime _newDate = TimeUtil.UTCTimeToLocalTime(_utcDate);
            Console.WriteLine("Converted back to Local: " + _newDate);
            Assert.AreEqual(_localDate, _newDate);

            DateTimeOffset _dateOffset = TimeUtil.LocalTimeToUTCTime(DateTimeOffset.Now);
            Console.WriteLine("DateTime Offset: " + _dateOffset);
            Int64 _ms = TimeUtil.DateTimeOffsetToUTCMillis(_dateOffset);
            Console.WriteLine("UTC Millis of offset: " + _ms);
            DateTimeOffset _dateOffsetAfter = TimeUtil.UTCMillisToDateTimeOffset(_ms);
            Console.WriteLine("DateTime Offset After: " + _dateOffsetAfter);
            Int64 _msAfter = TimeUtil.DateTimeOffsetToUTCMillis(_dateOffsetAfter);
            Console.WriteLine("UTC Millis of offset After: " + _msAfter);
            Assert.AreEqual(_ms, _msAfter);
        }
    }
}