using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using System;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestLobby : TestFixtureBase
    {
        [Test]
        public void TestLobbyNoPing()
        {
            Dictionary<string, object> extraJson = new Dictionary<string, object>();
            Dictionary<string, object> settings = new Dictionary<string, object>();
            Dictionary<string, object> pingData = new Dictionary<string, object>();
            //"{ \"ca-central-1\": \"98\", \"us-west-1\": \"123\"}",
            string[] othercxids = new string[1];
            othercxids[0] = "$gameId:aaa-bbb-ccc-ddd:asdfjkl";

            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo.Add("strategy", "ranged-absolute");
            algo.Add("alignment", "center");
            double[] arr = new double[3];
            arr[0] = 5;
            arr[1] = 7.5;
            arr[2] = 10;
            algo.Add("ranges", arr);
            Dictionary<string, object> filterJson = new Dictionary<string, object>();
            filterJson.Add("cheater", "false");

            TestResult tr = new TestResult(_bc);
            _bc.LobbyService.CreateLobby(
                "4v4",
                76,
                false,
                extraJson,
                "blue",
                settings,
                othercxids,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();

            TestResult tr2 = new TestResult(_bc);
            _bc.LobbyService.FindLobby(
                "4v4",
                76,
                2,
                algo,
                filterJson,
                1,
                false,
                extraJson,
                "blue",
                othercxids,
                tr2.ApiSuccess, tr2.ApiError);
            tr2.Run();
        }

        [Test]
        public void TestLobbyWithPingData()
        {
            Dictionary<string, object> extraJson = new Dictionary<string, object>();
            Dictionary<string, object> settings = new Dictionary<string, object>();
            Dictionary<string, object> pingData = new Dictionary<string, object>();
            //"{ \"ca-central-1\": \"98\", \"us-west-1\": \"123\"}",
            string[] othercxids = new string[1];
            othercxids[0] = "$gameId:aaa-bbb-ccc-ddd:asdfjkl";
            Dictionary<string, object> algo = new Dictionary<string, object>();
            algo.Add("strategy", "ranged-absolute");
            algo.Add("alignment", "center");
            algo.Add("ranges", "[5,7.5,10]");
            Dictionary<string, object> filterJson = new Dictionary<string, object>();
            filterJson.Add("cheater", "false");
            string[] roomTypes = new string[2];
            roomTypes[0] = "4v4";
            roomTypes[1] = "MATCH_UNRANKED";

            TestResult tra = new TestResult(_bc);
            _bc.LobbyService.GetRegionsForLobbies(roomTypes, tra.ApiSuccess, tra.ApiError);
            tra.Run();

            TestResult trb = new TestResult(_bc);
            _bc.LobbyService.PingRegions(tra.ApiSuccess, tra.ApiError);
            trb.Run();

            //create the lobby with ping data
            TestResult tr = new TestResult(_bc);

            _bc.LobbyService.CreateLobbyWithPingData(
                "4v4",
                76,
                false,
                extraJson,
                "blue",
                settings,
                othercxids,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();

            //find a lobby with ping data
            TestResult tr2 = new TestResult(_bc);

            _bc.LobbyService.FindLobbyWithPingData(
                "4v4",
                76,
                3,
                algo,
                filterJson,
                1,
                false,
                extraJson,
                "blue",
                othercxids,
                tr2.ApiSuccess, tr2.ApiError);

            tr2.Run();

            TestResult tr3 = new TestResult(_bc);

            _bc.LobbyService.JoinLobbyWithPingData(
                "$gameId:4v4:3",
                false,
                extraJson,
                "blue",
                othercxids,
                tr3.ApiSuccess, tr3.ApiError);

              tr3.Run();
        }
    }
}
