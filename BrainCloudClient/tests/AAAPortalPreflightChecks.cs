// Copyright 2026 bitHeads, Inc. All Rights Reserved.

using BrainCloud.JsonFx.Json;
using NUnit.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace BrainCloudTests
{
    // Runs first (alphabetically before all TestXxx fixtures) to verify that all
    // required portal configurations exist. A single failure here means the environment
    // is missing portal setup — fix those before investigating other test failures.
    [TestFixture]
    public class AAAPortalPreflightChecks : TestFixtureBase
    {
        [Test]
        public void PortalPreflightCheck()
        {
            var missing = new List<string>();

            // ---------------------------------------------------------------
            // Leaderboards
            // ---------------------------------------------------------------
            foreach (string lbId in new[] { "testLeaderboard", "testSocialLeaderboard", "testTournamentLeaderboard", "groupLeaderboardConfig" })
            {
                TestResult tr = new TestResult(_bc);
                _bc.LeaderboardService.GetGlobalLeaderboardEntryCount(lbId, tr.ApiSuccess, tr.ApiError);
                if (!RunSilent(tr)) missing.Add("leaderboard: " + lbId);
            }

            // ---------------------------------------------------------------
            // Item catalog
            // ---------------------------------------------------------------
            foreach (string itemId in new[] { "sword001", "equipmentBundle" })
            {
                TestResult tr = new TestResult(_bc);
                _bc.ItemCatalogService.GetCatalogItemDefinition(itemId, tr.ApiSuccess, tr.ApiError);
                if (!RunSilent(tr)) missing.Add("catalog item: " + itemId);
            }

            // ---------------------------------------------------------------
            // Global properties
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.GlobalAppService.ReadSelectedProperties(
                    new string[] { "prop1", "prop2", "prop3" },
                    tr.ApiSuccess, tr.ApiError);
                if (RunSilent(tr))
                {
                    var props = tr.m_response["data"] as Dictionary<string, object>;
                    foreach (string name in new[] { "prop1", "prop2", "prop3" })
                    {
                        if (props == null || !props.ContainsKey(name))
                            missing.Add("global property: " + name);
                    }
                }
                else
                {
                    missing.Add("global properties: prop1, prop2, prop3");
                }
            }

            // ---------------------------------------------------------------
            // Achievements
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.GamificationService.ReadAchievements(false, tr.ApiSuccess, tr.ApiError);
                if (RunSilent(tr))
                {
                    var data = tr.m_response["data"] as Dictionary<string, object>;
                    var achs = data != null && data.ContainsKey("achievements")
                        ? data["achievements"] as object[]
                        : null;
                    bool found01 = false, found02 = false;
                    if (achs != null)
                    {
                        foreach (var a in achs)
                        {
                            var ach = a as Dictionary<string, object>;
                            if (ach == null) continue;
                            string id = ach.ContainsKey("id") ? ach["id"] as string : null;
                            if (id == "testAchievement01") found01 = true;
                            if (id == "testAchievement02") found02 = true;
                        }
                    }
                    if (!found01) missing.Add("achievement: testAchievement01");
                    if (!found02) missing.Add("achievement: testAchievement02");
                }
                else
                {
                    missing.Add("achievement: testAchievement01");
                    missing.Add("achievement: testAchievement02");
                }
            }

            // ---------------------------------------------------------------
            // Milestone and quest category: Experience
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.GamificationService.ReadMilestonesByCategory("Experience", false, tr.ApiSuccess, tr.ApiError);
                if (RunSilent(tr))
                {
                    var data = tr.m_response["data"] as Dictionary<string, object>;
                    var milestones = data != null && data.ContainsKey("milestones") ? data["milestones"] as object[] : null;
                    if (milestones == null || milestones.Length == 0)
                        missing.Add("milestone category: Experience (no milestones defined)");
                }
                else
                {
                    missing.Add("milestone category: Experience");
                }
            }
            {
                TestResult tr = new TestResult(_bc);
                _bc.GamificationService.ReadQuestsByCategory("Experience", false, tr.ApiSuccess, tr.ApiError);
                if (RunSilent(tr))
                {
                    var data = tr.m_response["data"] as Dictionary<string, object>;
                    var quests = data != null && data.ContainsKey("quests") ? data["quests"] as object[] : null;
                    if (quests == null || quests.Length == 0)
                        missing.Add("quest category: Experience (no quests defined)");
                }
                else
                {
                    missing.Add("quest category: Experience");
                }
            }

            // ---------------------------------------------------------------
            // Virtual currency type: credits
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.VirtualCurrencyService.GetCurrency(null, tr.ApiSuccess, tr.ApiError);
                if (RunSilent(tr))
                {
                    var data = tr.m_response["data"] as Dictionary<string, object>;
                    var currency = data != null && data.ContainsKey("currencyMap")
                        ? data["currencyMap"] as Dictionary<string, object>
                        : null;
                    if (currency == null || !currency.ContainsKey("credits"))
                        missing.Add("virtual currency type: credits");
                }
                else
                {
                    missing.Add("virtual currency type: credits");
                }
            }

            // ---------------------------------------------------------------
            // Custom entity type: athletes
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.CustomEntityService.GetEntityPage(
                    "athletes",
                    "{\"pagination\":{\"rowsPerPage\":1,\"pageNumber\":1},\"searchCriteria\":{}}",
                    tr.ApiSuccess, tr.ApiError);
                if (!RunSilent(tr)) missing.Add("custom entity type: athletes");
            }

            // ---------------------------------------------------------------
            // Tournament division set: testDivSetId
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.TournamentService.GetDivisionInfo("testDivSetId", tr.ApiSuccess, tr.ApiError);
                if (!RunSilent(tr)) missing.Add("tournament division set: testDivSetId");
            }

            // ---------------------------------------------------------------
            // Lobby type: MATCH_UNRANKED
            // ---------------------------------------------------------------
            {
                TestResult tr = new TestResult(_bc);
                _bc.LobbyService.GetRegionsForLobbies(new string[] { "MATCH_UNRANKED" }, tr.ApiSuccess, tr.ApiError);
                if (!RunSilent(tr)) missing.Add("lobby type: MATCH_UNRANKED");
            }

            // ---------------------------------------------------------------
            // Report
            // ---------------------------------------------------------------
            if (missing.Count > 0)
            {
                string message = "\nPORTAL PREFLIGHT CHECK FAILED - the following items are not configured on the portal:\n";
                foreach (string item in missing)
                    message += "  - " + item + "\n";
                message += "\nSet these up in the portal before running the full test suite.\n";
                Assert.Fail(message);
            }
        }

        // Spins the update loop and returns the result without throwing on failure.
        private bool RunSilent(TestResult tr)
        {
            try
            {
                tr.Run();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
