using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestScript : TestFixtureBase
    {
        private readonly string _scriptName = "testScript";
        private readonly string _childAppId = "10326";
        private readonly string _parentLevel = "Master";

        [Test]
        public void TestRunScript()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ScriptService.RunScript(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestScheduleScriptUTC()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ScriptService.ScheduleRunScriptUTC(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                DateTime.Now.AddDays(1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestScheduleScriptMinutesFromNow()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                60,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        //[Test]
        //public void TestRunScriptAsParent()
        //{
        //    TestResult tr = new TestResult();
        //    BrainCloudClient.Get().IdentityService.SwitchToChildProfile(
        //        null,
        //        _childAppId,
        //        true,
        //        tr.ApiSuccess, tr.ApiError);
        //    tr.Run();

        //    BrainCloudClient.Get().ScriptService.RunAsParent(
        //        _scriptName,
        //        Helpers.CreateJsonPair("testParm1", 1), _parentLevel,
        //        tr.ApiSuccess, tr.ApiError);

        //    tr.Run();
        //}
    }
}