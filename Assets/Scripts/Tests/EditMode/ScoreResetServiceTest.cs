using NUnit.Framework;
using System;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class ScoreResetServiceTest
    {
        private ScoreResetService _scoreResetService;

        [SetUp]
        public void SetUp()
        {
            _scoreResetService = new ScoreResetService();
        }

        [Test]
        public void ShouldResetDailyScores_ShouldReturnTrueForDifferentDays()
        {
            // Date for testing
            DateTime lastPlayedDate = new DateTime(2024, 9, 30);
            DateTime currentDate = new DateTime(2024, 10, 1);

            // should return true because it is a different date
            bool result = _scoreResetService.ShouldResetDailyScores(lastPlayedDate, currentDate);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldResetDailyScores_ShouldReturnFalseForSameDay()
        {
            // Same date
            DateTime lastPlayedDate = new DateTime(2024, 10, 1);
            DateTime currentDate = new DateTime(2024, 10, 1);

            // should return false because it is the same date
            bool result = _scoreResetService.ShouldResetDailyScores(lastPlayedDate, currentDate);

            Assert.IsFalse(result);
        }

        [Test]
        public void ShouldResetMonthlyScores_ShouldReturnTrueForDifferentMonths()
        {
            // If the month is different
            DateTime lastPlayedDate = new DateTime(2024, 9, 30);
            DateTime currentDate = new DateTime(2024, 10, 1);

            // should return true because it is a different month
            bool result = _scoreResetService.ShouldResetMonthlyScores(lastPlayedDate, currentDate);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldResetMonthlyScores_ShouldReturnTrueForDifferentYears()
        {
            // If the year is different
            DateTime lastPlayedDate = new DateTime(2023, 12, 31);
            DateTime currentDate = new DateTime(2024, 1, 1);

            // should return true because it is a different year
            bool result = _scoreResetService.ShouldResetMonthlyScores(lastPlayedDate, currentDate);

            Assert.IsTrue(result);
        }

        [Test]
        public void ShouldResetMonthlyScores_ShouldReturnFalseForSameMonthAndYear()
        {
            // Same month, same year
            DateTime lastPlayedDate = new DateTime(2024, 10, 1);
            DateTime currentDate = new DateTime(2024, 10, 15);

            // Same month, same year, so false should return
            bool result = _scoreResetService.ShouldResetMonthlyScores(lastPlayedDate, currentDate);

            Assert.IsFalse(result);
        }

        [Test]
        public void ResetScores_ShouldClearAllScores()
        {
            // Initial Score
            int[] scores = { 100, 200, 300 };

            // Reset Score
            _scoreResetService.ResetScores(ref scores);

            // Make sure score is empty
            Assert.IsEmpty(scores);
        }

        [TearDown]
        public void TearDown()
        {
            _scoreResetService = null;
        }
    }
}
