using System.Collections;
using System.Collections.Generic;
using BrainCloud.JsonFx.Json;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    [TestFixture]
    public class TestAsyncMatch : TestFixtureBase
    {

        private string matchId;
        private readonly string _platform = "BC";

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestCreateMatch()
        {
            TestFixtureBase tr = new TestFixtureBase(_bc);

            Dictionary<string, object>[] players = new Dictionary<string, object>[1];
            players[0] = new Dictionary<string, object> { { "platform", _platform }, { "id", GetUser(Users.UserB).ProfileId } };

            _bc.AsyncMatchService.CreateMatch(
                JsonWriter.Serialize(players),
                null,
                tr.ApiSuccess, tr.ApiError);

            yield return tr.Run();
        
            matchId = (string)((Dictionary<string, object>)(tr.m_response["data"]))["matchId"];
            Debug.Log($"MatchID: {matchId}");
        }
    
        private void AbandonMatch(string matchId)
        {
            TestFixtureBase tr = new TestFixtureBase(_bc);
            _bc.AsyncMatchService.AbandonMatch(
                GetUser(Users.UserA).ProfileId,
                matchId,
                tr.ApiSuccess, tr.ApiError);
            tr.Run();
        }
    }
}
