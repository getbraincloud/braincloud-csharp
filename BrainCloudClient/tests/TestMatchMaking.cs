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
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.GetShieldExpiry(
                null,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRead()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.Read(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestEnableMatchMaking()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.EnableMatchMaking(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.SetPlayerRating(
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.ResetPlayerRating(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestIncrementPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.IncrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDecrementPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.DecrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOn()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.TurnShieldOn(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOff()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.TurnShieldOff(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOnFor()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.TurnShieldOnFor(
                1,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetOneWayPlayers()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Get().MatchMakingService.FindPlayers(
                3,
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetOneWayPlayersWithFilter()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> filters = new Dictionary<string, object> { { "filter1", 10 } };

            BrainCloudClient.Get().MatchMakingService.FindPlayersWithFilter(
                3,
                5,
                JsonWriter.Serialize(filters),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }
    }
}