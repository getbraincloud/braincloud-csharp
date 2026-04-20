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

            // Pre-cleanup: cancel any scheduled scripts left over from a previous failed run.
            // If a prior run's assertions threw, the cleanup at the bottom was never reached and
            // those jobs remain on the server, polluting subsequent GetScheduledCloudScripts results.
            _bc.ScriptService.GetScheduledCloudScripts(long.MaxValue, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            // Capture a single "now" reference so that all scheduled times AND the query cutoff
            // are derived from the same client clock. This avoids clock-skew failures when the
            // build agent's clock differs from the brainCloud server's clock: with ScheduleRunScriptMinutes
            // the server anchors scheduled times to its own clock, but the query uses the client clock,
            // so any skew shifts Job 3 (5 min) inside the 150 s window. Using ScheduleRunScriptMillisUTC
            // sends absolute client timestamps, so the relative ordering is always consistent.
            DateTime now = DateTime.UtcNow;

            List<string> jobIds = new List<string>();
            void apiSuccess(string jsonResponse, object _)
            {
                tr.ApiSuccess(jsonResponse, _);

                // We're grabbing the scheduled jobIds from here
                string jobId = (tr.m_response["data"] as Dictionary<string, object>)["jobId"].ToString();

                jobIds.Add(jobId);
            }

            _bc.ScriptService.ScheduleRunScriptMillisUTC(
                _scriptName,
                "{}",
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(now.AddMinutes(1)),
                apiSuccess, tr.ApiError, null);

            _bc.ScriptService.ScheduleRunScriptMillisUTC(
                _scriptName,
                "{}",
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(now.AddMinutes(2)),
                apiSuccess, tr.ApiError, null);

            _bc.ScriptService.ScheduleRunScriptMillisUTC(
                _scriptName,
                "{}",
                (ulong)TimeUtil.UTCDateTimeToUTCMillis(now.AddMinutes(5)),
                apiSuccess, tr.ApiError, null);

            tr.RunExpectCount(3);

            // We need the Job IDs to be able to cancel these after the test
            Assert.That(jobIds, Is.Not.Null, "JobIDs retrieved after calls is null!");
            Assert.That(jobIds, Is.Not.Empty, "JobIDs retrieved after calls is empty!");
            Assert.That(jobIds, Has.Count.EqualTo(3), "Did not retrieve all 3 JobIDs after calls!");

            // Query for scripts scheduled before now + 150 s — captures only the 1-min and 2-min jobs,
            // not the 5-min job. Uses the same 'now' snapshot so the cutoff is clock-skew-independent.
            DateTime utcTime = now.AddSeconds(150.0);

            try
            {
                _bc.ScriptService.GetScheduledCloudScripts((ulong)TimeUtil.UTCDateTimeToUTCMillis(utcTime),
                                                           tr.ApiSuccess,
                                                           tr.ApiError,
                                                           null);

                tr.Run();

                var jobs = (tr.m_response["data"] as Dictionary<string, object>)["scheduledJobs"] as Dictionary<string, object>[];

                Assert.That(jobs, Is.Not.Null, "Scheduled jobs received is null!");

                // Verify filtering by checking our specific job IDs, not the total count.
                // Other scheduled jobs may exist in the account; what matters is that
                // the time filter correctly includes/excludes our three specific jobs.
                bool HasJob(string jobId) => jobs.Any(j =>
                    j.TryGetValue("jobId", out var v) && v is string s && s == jobId);

                Assert.That(HasJob(jobIds[0]), Is.True,  "1-min job should be returned (within 150 s cutoff)");
                Assert.That(HasJob(jobIds[1]), Is.True,  "2-min job should be returned (within 150 s cutoff)");
                Assert.That(HasJob(jobIds[2]), Is.False, "5-min job should NOT be returned (beyond 150 s cutoff)");
            }
            finally
            {
                // Always cancel the jobs we scheduled, even if an assertion above failed
                foreach (string id in jobIds)
                {
                    _bc.ScriptService.CancelScheduledScript(id);
                }
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
