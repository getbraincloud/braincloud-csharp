using NUnit.Core;
using NUnit.Framework;
using BrainCloud;

namespace BrainCloudTests
{
    [TestFixture]
    public class TestGamification : TestFixtureBase
    {
        private readonly string _achievementId01 = "testAchievement01";
        private readonly string _achievementId02 = "testAchievement02";

        private readonly string _milestoneCategory = "Experience";
        private readonly string _milestoneId = "2";

        private readonly string _questsCategory = "Experience";

        #region Achievements

        [Test]
        public void TestAwardAchievements()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.AwardAchievements(
                new string[] {_achievementId01, _achievementId02},
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadAchievedAchievements()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadAchievedAchievements(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadAchievements()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadAchievements(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

        #region Milestones

        [Test]
        public void TestReadCompletedMilestones()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadCompletedMilestones(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadInProgressMilestones()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadInProgressMilestones(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadMilestones()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadMilestones(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadMilestonesByCategory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadMilestonesByCategory(
                _milestoneCategory,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestResetMilestones()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ResetMilestones(
                new string[] { _milestoneId },
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

        #region Quests

        [Test]
        public void TestReadCompletedQuests()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadCompletedQuests(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void ReadNotStartedQuests()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadNotStartedQuests(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadInProgressQuests()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadInProgressQuests(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadQuests()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadQuests(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadQuestsByCategory()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadQuestsByCategory(
                _questsCategory,
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadQuestsWithBasicPercentage()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadQuestsWithBasicPercentage(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadQuestsWithComplexPercentage()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadQuestsWithComplexPercentage(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadQuestsWithStatus()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadQuestsWithStatus(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        [Test]
        public void TestReadXPLevelsMetaData()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadXpLevelsMetaData(
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion

        #region Misc

        [Test]
        public void TestReadAllGamification()
        {
            TestResult tr = new TestResult(_bc);

            _bc.GamificationService.ReadAllGamification(
                true,
                tr.ApiSuccess, tr.ApiError);

            tr.Run();
        }

        #endregion
    }
}