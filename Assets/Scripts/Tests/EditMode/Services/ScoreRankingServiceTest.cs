using NUnit.Framework;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class ScoreRankingServiceTest
    {
        private ScoreRankingService _scoreRankingService;

        [SetUp]
        public void SetUp()
        {
            _scoreRankingService = new ScoreRankingService();
        }

        [Test]
        public void UpdateTopScores_ShouldAddNewScoreAndReturnSortedArray()
        {
            // Initial Score
            int[] currentScores = { 100, 200, 300 };
            int newScore = 250;
            int maxEntries = 3;

            // Method call
            int[] updatedScores = _scoreRankingService.UpdateTopScores(currentScores, newScore, maxEntries);

            // Expected result: 250 new scores will be added and up to 3 entries will be sorted
            int[] expectedScores = { 300, 250, 200 };

            // Assert: Does the result match the expected score order?
            Assert.AreEqual(expectedScores, updatedScores);
        }

        [Test]
        public void UpdateTopScores_ShouldHandleNewHighScoreCorrectly()
        {
            int[] currentScores = { 150, 180, 220 };
            int newScore = 300;
            int maxEntries = 3;

            // Test to see if new high scores are included
            int[] updatedScores = _scoreRankingService.UpdateTopScores(currentScores, newScore, maxEntries);
            int[] expectedScores = { 300, 220, 180 };

            Assert.AreEqual(expectedScores, updatedScores);
        }

        [Test]
        public void UpdateTopScores_ShouldHandleNewLowScoreCorrectly()
        {
            int[] currentScores = { 400, 300, 200 };
            int newScore = 150;
            int maxEntries = 3;

            // If the new score is lower, scores that do not fit into the maximum number of entries are excluded
            int[] updatedScores = _scoreRankingService.UpdateTopScores(currentScores, newScore, maxEntries);
            int[] expectedScores = { 400, 300, 200 };  // New score not included

            Assert.AreEqual(expectedScores, updatedScores);
        }

        [Test]
        public void UpdateTopScores_ShouldLimitEntriesToMaxEntries()
        {
            int[] currentScores = { 500, 450, 400, 300, 200 };
            int newScore = 350;
            int maxEntries = 4;

            // Check if the number of entries is limited to maxEntries
            int[] updatedScores = _scoreRankingService.UpdateTopScores(currentScores, newScore, maxEntries);
            int[] expectedScores = { 500, 450, 400, 350 };

            Assert.AreEqual(expectedScores, updatedScores);
        }

        [Test]
        public void IsNewBestScore_ShouldReturnTrueForHigherNewScore()
        {
            int currentBest = 250;
            int newScore = 300;

            // See if the new score exceeds the highest current score
            bool result = _scoreRankingService.IsNewBestScore(currentBest, newScore);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsNewBestScore_ShouldReturnFalseForLowerNewScore()
        {
            int currentBest = 350;
            int newScore = 300;

            // If the new score is below the current highest score
            bool result = _scoreRankingService.IsNewBestScore(currentBest, newScore);

            Assert.IsFalse(result);
        }

        [TearDown]
        public void TearDown()
        {
            _scoreRankingService = null;
        }
    }
}
