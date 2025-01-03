using NUnit.Framework;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Tests
{
    public class GameOverJudgmentServiceTest
    {
        private GameOverJudgmentService _gameOverJudgmentService;

        [SetUp]
        public void SetUp()
        {
            _gameOverJudgmentService = new GameOverJudgmentService();
        }

        [Test]
        public void CheckGameOver_WhenContactTimeExceedsTimeLimit_ShouldReturnTrue()
        {
            // Arrange
            float contactTime = 1.5f;
            float timeLimit = 1.0f;

            // Act
            bool result = _gameOverJudgmentService.CheckGameOver(contactTime, timeLimit);

            // Assert
            Assert.IsTrue(result, "GameOver should be true when contact time exceeds time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsEqualToTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            float contactTime = 1.0f;
            float timeLimit = 1.0f;

            // Act
            bool result = _gameOverJudgmentService.CheckGameOver(contactTime, timeLimit);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is equal to time limit.");
        }

        [Test]
        public void CheckGameOver_WhenContactTimeIsLessThanTimeLimit_ShouldReturnFalse()
        {
            // Arrange
            float contactTime = 0.5f;
            float timeLimit = 1.0f;

            // Act
            bool result = _gameOverJudgmentService.CheckGameOver(contactTime, timeLimit);

            // Assert
            Assert.IsFalse(result, "GameOver should be false when contact time is less than time limit.");
        }

        [TearDown]
        public void TearDown()
        {
            _gameOverJudgmentService = null;
        }
    }
}
