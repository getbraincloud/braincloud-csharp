using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestFriend : TestFixtureBase
    {
        [Test]
        public void TestFindPlayerByName()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.FriendService.FindPlayerByName(
                "search",
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestGetExternalIdForProfileId()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.FriendService.GetExternalIdForProfileId(
                GetUser(Users.UserA).ProfileId,
                "Facebook",
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestFindPlayerByUniversalId()
        {
            TestResult tr = new TestResult();

            BrainCloudClient.Instance.FriendService.FindPlayerByUniversalId(
                "search",
                10,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }
    }
}