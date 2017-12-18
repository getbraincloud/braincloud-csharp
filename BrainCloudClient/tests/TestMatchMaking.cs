using NUnit.Core;
using NUnit.Framework;
using BrainCloud;
using System.Collections.Generic;
using JsonFx.Json;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestMatchMaking : TestFixtureBase
    {
        [Test]
        public void TestGetShieldExpiry()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.GetShieldExpiry(
                null,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRead()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.Read(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestEnableMatchMaking()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.EnableMatchMaking(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDisableMatchMaking()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.DisableMatchMaking(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetPlayerRating()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.SetPlayerRating(
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetPlayerRating()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.ResetPlayerRating(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestIncrementPlayerRating()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.IncrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDecrementPlayerRating()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.DecrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOn()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.TurnShieldOn(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOff()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.TurnShieldOff(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOnFor()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.TurnShieldOnFor(
                1,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayers()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.FindPlayers(
                3,
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }


        [Test]
        public void TestFindPlayersWithAttributes()
        {
            TestResult tr = new TestResult(_bc);

            _bc.MatchMakingService.FindPlayersWithAttributes(
                3,
                5,
                Helpers.CreateJsonPair("name", "asdf"),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayersUsingFilter()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> filters = new Dictionary<string, object> { { "filter1", 10 } };

            _bc.MatchMakingService.FindPlayersUsingFilter(
                3,
                5,
                JsonWriter.Serialize(filters),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayersUsingFilterWithAttributes()
        {
            TestResult tr = new TestResult(_bc);

            Dictionary<string, object> filters = new Dictionary<string, object> { { "filter1", 10 } };

            _bc.MatchMakingService.FindPlayersWithAttributesUsingFilter(
                3,
                5,
                Helpers.CreateJsonPair("name", "asdf"),
                JsonWriter.Serialize(filters),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }
    }
}