// Copyright 2026 bitHeads, Inc. All Rights Reserved.
using BrainCloud;
using BrainCloud.Common;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
        public void TestScheduleRunScriptMillisUTC()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.ScheduleRunScriptMillisUTC(
                _scriptName,
                Helpers.CreateJsonPair("testParm1", 1),
                (UInt64)((TimeZoneInfo.ConvertTimeToUtc(DateTime.UtcNow) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
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
        public void TestGetScheduledCloudScripts()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.GetScheduledCloudScripts(
                long.MaxValue,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetScheduledCloudScriptsExpectFailure()
        {
            TestResult tr = new TestResult(_bc);

            _bc.ScriptService.GetScheduledCloudScripts(
                ulong.MaxValue,
                tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.INVALID_PARAMETER_TYPE);
        }

        [Test]
        public void TestSchedule3ScriptsGet2ScheduledCloudScriptsThenCancelAll()
        {
            TestResult tr = new TestResult(_bc);

            // Lets schedule some Cloud Code scripts to run...
            List<string> jobIds = new List<string>();
            void apiSuccess(string jsonResponse, object _)
            {
                tr.ApiSuccess(jsonResponse, _);

                // We're grabbing the scheduled jobIds from here
                string jobId = (tr.m_response["data"] as Dictionary<string, object>)["jobId"].ToString();

                jobIds.Add(jobId);
            }

            _bc.ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                "{}",
                1,
                apiSuccess, tr.ApiError, null);

            _bc.ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                "{}",
                2,
                apiSuccess, tr.ApiError, null);

            _bc.ScriptService.ScheduleRunScriptMinutes(
                _scriptName,
                "{}",
                5,
                apiSuccess, tr.ApiError, null);

            tr.RunExpectCount(3);

            // We need the Job IDs to be able to cancel these after the test
            Assert.That(jobIds, Is.Not.Null, "JobIDs retrieved after calls is null!");
            Assert.That(jobIds, Is.Not.Empty, "JobIDs retrieved after calls is empty!");
            Assert.That(jobIds, Has.Count.EqualTo(3), "Did not retrieve all 3 JobIDs after calls!");

            // We're only going to try to get the first two scripts
            DateTime utcTime = DateTime.UtcNow.AddSeconds(150.0);

            _bc.ScriptService.GetScheduledCloudScripts((ulong)TimeUtil.UTCDateTimeToUTCMillis(utcTime),
                                                       tr.ApiSuccess,
                                                       tr.ApiError,
                                                       null);

            tr.Run();

            var jobs = (tr.m_response["data"] as Dictionary<string, object>)["scheduledJobs"] as Dictionary<string, object>[];

            Assert.That(jobs, Is.Not.Null, "Scheduled jobs received is null!");
            Assert.That(jobs, Is.Not.Empty, "Scheduled jobs received is empty!");
            Assert.That(jobs, Has.Length.EqualTo(2), "Scheduled jobs received should equal to 2!");

            // Now to make sure only 2 of the jobIds are actually returned
            int jobCount = 0;
            foreach (var job in jobs)
            {
                if (job.ContainsKey("jobId") && job["jobId"] is string id && jobIds.Contains(id))
                {
                    ++jobCount;
                }
            }

            Assert.That(jobCount, Is.EqualTo(2), "Found more/less than 2 jobs in scheduled jobs received!");

            // Finally lets cancel those jobs just to clean things up
            foreach (string id in jobIds)
            {
                _bc.ScriptService.CancelScheduledScript(id);
            }
        }

        [Test]
        public void TestGetRunningOrQueuedCloudScripts()
        {
            TestResult tr = new TestResult(_bc);
            _bc.ScriptService.GetRunningOrQueuedCloudScripts(
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