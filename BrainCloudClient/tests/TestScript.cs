using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;
using System;
using BrainCloud.Common;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestScript : TestFixtureBase
    {
        private readonly string _scriptName = "testScript";
        private readonly string _peerScriptName = "TestPeerScriptPublic";

        [Test]
        public void TestRunScript()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.RunScript(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestScheduleScriptUTC()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.ScheduleRunScriptUTC(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                DateTime.Now.AddDays(1),
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestScheduleScriptMinutesFromNow()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                60,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCancelJob()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                60,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = (Dictionary<string, object>)tr.m_response["data"];
            string jobId = data["jobId"] as string;

            _bc.ScriptService.CancelScheduledScript(
                jobId,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRunParentScript()
        {
            GoToChildProfile();

            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunParentScript(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1), ParentLevel,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRunPeerScript()
        {
            AttachPeer(Users.UserA, AuthenticationType.Universal);

            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunPeerScript(
                _peerScriptName,
                Helpers.CreateJsonPair("testParm1", 1), 
                PeerName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            DetachPeer();
        }

        [Test]
        public void TestRunPeerScriptAsync()
        {
            AttachPeer(Users.UserA, AuthenticationType.Universal);

            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.RunPeerScriptAsync(
                _peerScriptName,
                Helpers.CreateJsonPair("testParm1", 1), 
                PeerName,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            DetachPeer();
        }
    }
}