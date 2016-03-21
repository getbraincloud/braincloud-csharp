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

            BrainCloudClient.Instance.MatchMakingService.GetShieldExpiry(
                null,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestRead()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.Read(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestEnableMatchMaking()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.EnableMatchMaking(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestSetPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.SetPlayerRating(
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.ResetPlayerRating(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestIncrementPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.IncrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestDecrementPlayerRating()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.DecrementPlayerRating(
                2,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOn()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.TurnShieldOn(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOff()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.TurnShieldOff(
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestTurnShieldOnFor()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.TurnShieldOnFor(
                1,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayers()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.FindPlayers(
                3,
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }


        [Test]
        public void TestFindPlayersWithAttributes()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.MatchMakingService.FindPlayersWithAttributes(
                3,
                Helpers.CreateJsonPair("name", "asdf"),
                5,
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayersUsingFilter()
        {
            TestResult tr = new TestResult();

            Dictionary<string, object> filters = new Dictionary<string, object> { { "filter1", 10 } };

            BrainCloudClient.Instance.MatchMakingService.FindPlayersUsingFilter(
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
            TestResult tr = new TestResult();

            Dictionary<string, object> filters = new Dictionary<string, object> { { "filter1", 10 } };

            BrainCloudClient.Instance.MatchMakingService.FindPlayersWithAttributesUsingFilter(
                3,
                Helpers.CreateJsonPair("name", "asdf"),
                5,
                JsonWriter.Serialize(filters),
                tr.ApiSuccess,
                tr.ApiError);

            tr.Run();
        }
    }
}