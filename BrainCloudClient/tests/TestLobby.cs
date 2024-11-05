using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;

// Lot of expect fail here. This is ok, but we make sure the failed reason 
// is not about a bad or missing argument. We validate that the API is
// written correctly, not that the output from BC is correct. This is
// verified more in depth by use case, JS unit tests and server tests.

namespace BrainCloudTests
{
    [TestFixture]
    public class TestLobby : TestFixtureBase
    {
        [Test]
        public void TestFindLobbyDeprecated()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;

            _bc.LobbyService.FindLobby("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", null, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindLobby()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;

            _bc.LobbyService.FindLobby("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", null, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestCreateLobby()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.CreateLobby("MATCH_UNRANKED", 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindOrCreateLobbyDeprecated()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;

            _bc.LobbyService.FindOrCreateLobby("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindOrCreateLobby()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;

            _bc.LobbyService.FindOrCreateLobby("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetLobbyData()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.GetLobbyData("wrongLobbyId", tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestLeaveLobby()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.LeaveLobby("wrongLobbyId", tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestJoinLobby()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.JoinLobby("wrongLobbyId", true, new Dictionary<string, object>(), "red", null, tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestRemoveMember()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.RemoveMember("wrongLobbyId", "wrongConId", tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestSendSignal()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> signal = new Dictionary<string, object>();
            signal["msg"] = "test";

            _bc.LobbyService.SendSignal("wrongLobbyId", signal, tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestSwitchTeam()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.SwitchTeam("wrongLobbyId", "all", tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestUpdateReady()
        {
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.UpdateReady("wrongLobbyId", true, new Dictionary<string, object>(), tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }

        [Test]
        public void TestUpdateSettings()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> settings = new Dictionary<string, object>();
            settings["msg"] = "test";

            _bc.LobbyService.UpdateSettings("wrongLobbyId", settings, tr.ApiSuccess, tr.ApiError);

            tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
        }
        
        [Test]
        public void TestGetLobbyInstance()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, int> ratings = new Dictionary<string, int>();
            ratings["min"] = 50;
            ratings["max"] = 70;
            
            Dictionary<string, int> ping = new Dictionary<string, int>();
            ping["max"] = 100;
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["rating"] = ratings;
            data["ping"] = ping;

            _bc.LobbyService.GetLobbyInstances(
                "MATCH_UNRANKED",
                data,
                tr.ApiSuccess,
                tr.ApiError
            );
            tr.Run();
        }
        
        [Test]
        public void TestGetLobbyInstanceWithPingData()
        {
            TestResult tr = new TestResult(_bc);
            
            Dictionary<string, int> ratings = new Dictionary<string, int>();
            ratings["min"] = 50;
            ratings["max"] = 70;
            
            Dictionary<string, int> ping = new Dictionary<string, int>();
            ping["max"] = 100;
            
            Dictionary<string, object> criteriaJson = new Dictionary<string, object>();
            criteriaJson["rating"] = ratings;
            criteriaJson["ping"] = ping;

            string[] roomTypes =
            {
                "MATCH_UNRANKED"
            };
            _bc.LobbyService.GetRegionsForLobbies(roomTypes, tr.ApiSuccess, tr.ApiError);
            tr.Run();
            
            _bc.LobbyService.PingRegions(tr.ApiSuccess,tr.ApiError);
            tr.Run();
            
            _bc.LobbyService.GetLobbyInstancesWithPingData
            (
                "MATCH_UNRANKED",
                criteriaJson,
                tr.ApiSuccess,
                tr.ApiError
            );
            tr.Run();
        }

        [Test]
        public void TestCancelFindRequest()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo["strategy"] = "ranged-absolute";
            algo["alignment"] = "center";
            List<int> ranges = new List<int>();
            ranges.Add(1000);
            algo["ranges"] = ranges;

            _bc.LobbyService.FindOrCreateLobby("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);

            tr.Run();

            var data = tr.m_response["data"] as Dictionary<string, object>;
            string in_entryId = data["entryId"] as string;

            _bc.LobbyService.CancelFindRequest("MATCH_UNRANKED", in_entryId, tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        // We include all tests regarding pings in there
        [Test]
        public void TestPingRegions()
        {
            TestResult tr = new TestResult(_bc);

            // Test trying to call a function <>withPingData without having fetched pings
            {
                Dictionary<string, object> algo = new Dictionary<string, object>();
                algo["strategy"] = "ranged-absolute";
                algo["alignment"] = "center";
                List<int> ranges = new List<int>();
                ranges.Add(1000);
                algo["ranges"] = ranges;
                _bc.LobbyService.FindOrCreateLobbyWithPingData("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.MISSING_REQUIRED_PARAMETER);
            }

            // Fetch pings
            {
                _bc.LobbyService.GetRegionsForLobbies(new string[]{"MATCH_UNRANKED"}, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            // Ping regions 2 times to make sure we see in the log there's no caching happening and that they don't all end up at 0 on the second or third time
            {
                _bc.LobbyService.PingRegions(tr.ApiSuccess, tr.ApiError);
                tr.Run();
                _bc.LobbyService.PingRegions(tr.ApiSuccess, tr.ApiError);
                tr.Run();
                Dictionary<string, long> pingData = _bc.LobbyService.PingData;
                long total = 0;
                foreach(KeyValuePair<string, long> ping in pingData)
                {
                    total += ping.Value;
                }
                total /= pingData.Count;

                // We test from east to west. So it would be physically impossible to have an average under 10ms.
                Assert.That(10 < total);
            }
            
            // Call all the <>WithPingData functions and make sure they go through braincloud
            {
                Dictionary<string, object> algo = new Dictionary<string, object>();
                algo["strategy"] = "ranged-absolute";
                algo["alignment"] = "center";
                List<int> ranges = new List<int>();
                ranges.Add(1000);
                algo["ranges"] = ranges;

                _bc.LobbyService.FindLobbyWithPingData("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", null, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            {
                Dictionary<string, object> algo = new Dictionary<string, object>();
                algo["strategy"] = "ranged-absolute";
                algo["alignment"] = "center";
                List<int> ranges = new List<int>();
                ranges.Add(1000);
                algo["ranges"] = ranges;

                _bc.LobbyService.FindLobbyWithPingData("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", null, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            {
                _bc.LobbyService.CreateLobbyWithPingData("MATCH_UNRANKED", 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }
            {
                Dictionary<string, object> algo = new Dictionary<string, object>();
                algo["strategy"] = "ranged-absolute";
                algo["alignment"] = "center";
                List<int> ranges = new List<int>();
                ranges.Add(1000);
                algo["ranges"] = ranges;

                _bc.LobbyService.FindOrCreateLobbyWithPingData("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), 0, true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            // TODO:  FindOrCreateLobbyWithPingData w/o timeoutSecs
            {
                Dictionary<string, object> algo = new Dictionary<string, object>();
                algo["strategy"] = "ranged-absolute";
                algo["alignment"] = "center";
                List<int> ranges = new List<int>();
                ranges.Add(1000);
                algo["ranges"] = ranges;

                _bc.LobbyService.FindOrCreateLobbyWithPingData("MATCH_UNRANKED", 0, 1, algo, new Dictionary<string, object>(), true, new Dictionary<string, object>(), "all", new Dictionary<string, object>(), null, tr.ApiSuccess, tr.ApiError);
                tr.Run();
            }

            {
                _bc.LobbyService.JoinLobbyWithPingData("wrongLobbyId", true, new Dictionary<string, object>(), "red", null, tr.ApiSuccess, tr.ApiError);
                tr.RunExpectFail(StatusCodes.BAD_REQUEST, ReasonCodes.LOBBY_NOT_FOUND);
            }
        }
    }
}
