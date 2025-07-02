using Domain.ValueObject;
using UseCase.DTO;

using NUnit.Framework;

namespace Tests.EditMode.UseCases
{
    public sealed class ScoreDataMapperTest
    {
        private ScoreContainer _validScoreContainer;

        [SetUp]
        public void SetUp()
        {
            // Create valid test data
            _validScoreContainer = new ScoreContainer
            {
                data = new ScoreData
                {
                    score = new ScoreDetail 
                    { 
                        userName = "TestUser",
                        best = 150, 
                        lastPlayedDate = "2024-12-01" 
                    },
                    rankings = new Rankings
                    {
                        daily = new ScoreList { scores = new[] { 10, 20, 30, 40, 50 } },
                        monthly = new ScoreList { scores = new[] { 60, 70, 80, 90, 100 } },
                        allTime = new ScoreList { scores = new[] { 110, 120, 130, 140, 150 } }
                    }
                }
            };
        }

        [Test]
        public void ToScoreDataDto_ShouldReturnValidDto_WhenValidScoreContainerProvided()
        {
            // Act
            var result = ScoreDataMapper.ToScoreDataDto(_validScoreContainer);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasValue);
            
            var scoreDataDto = result.Value;
            
            // Check Score data
            Assert.AreEqual("TestUser", scoreDataDto.Score.UserName);
            Assert.AreEqual(150, scoreDataDto.Score.Best);
            Assert.AreEqual("2024-12-01", scoreDataDto.Score.LastPlayedDate);
            
            // Check Rankings data
            CollectionAssert.AreEqual(new[] { 10, 20, 30, 40, 50 }, scoreDataDto.Rankings.Daily.Scores);
            CollectionAssert.AreEqual(new[] { 60, 70, 80, 90, 100 }, scoreDataDto.Rankings.Monthly.Scores);
            CollectionAssert.AreEqual(new[] { 110, 120, 130, 140, 150 }, scoreDataDto.Rankings.AllTime.Scores);
        }

        [Test]
        public void ToScoreDataDto_ShouldReturnNull_WhenScoreContainerIsNull()
        {
            // Act
            var result = ScoreDataMapper.ToScoreDataDto(null);

            // Assert
            Assert.IsNull(result);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ToScoreDataDto_ShouldReturnNull_WhenScoreContainerDataIsNull()
        {
            // Arrange
            var scoreContainerWithNullData = new ScoreContainer
            {
                data = null
            };

            // Act
            var result = ScoreDataMapper.ToScoreDataDto(scoreContainerWithNullData);

            // Assert
            Assert.IsNull(result);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ToScoreDataDto_ShouldHandleEmptyScores()
        {
            // Arrange
            var scoreContainerWithEmptyScores = new ScoreContainer
            {
                data = new ScoreData
                {
                    score = new ScoreDetail 
                    { 
                        userName = "TestUser",
                        best = 0, 
                        lastPlayedDate = "2024-12-01" 
                    },
                    rankings = new Rankings
                    {
                        daily = new ScoreList { scores = new int[0] },
                        monthly = new ScoreList { scores = new int[0] },
                        allTime = new ScoreList { scores = new int[0] }
                    }
                }
            };

            // Act
            var result = ScoreDataMapper.ToScoreDataDto(scoreContainerWithEmptyScores);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.HasValue);
            
            var scoreDataDto = result.Value;
            Assert.AreEqual(0, scoreDataDto.Score.Best);
            Assert.AreEqual(0, scoreDataDto.Rankings.Daily.Scores.Length);
            Assert.AreEqual(0, scoreDataDto.Rankings.Monthly.Scores.Length);
            Assert.AreEqual(0, scoreDataDto.Rankings.AllTime.Scores.Length);
        }

        [Test]
        public void ToScoreContainer_ShouldThrowNotImplementedException()
        {
            // Arrange
            var scoreDataDto = new ScoreDataDto(
                new ScoreDto("TestUser", 100, "2024-12-01"),
                new RankingsDto(
                    new RankingCategoryDto(new[] { 10, 20 }),
                    new RankingCategoryDto(new[] { 30, 40 }),
                    new RankingCategoryDto(new[] { 50, 60 })
                )
            );

            // Act & Assert
            Assert.Throws<System.NotImplementedException>(() => 
                ScoreDataMapper.ToScoreContainer(scoreDataDto));
        }

        [TearDown]
        public void TearDown()
        {
            _validScoreContainer = null;
        }
    }
}