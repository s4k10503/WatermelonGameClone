using Domain.Interfaces;
using Domain.ValueObject;
using UseCase.UseCases.Common;

using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests.EditMode.UseCases
{
    public sealed class ScoreUseCaseTest
    {
        private ScoreUseCase _scoreUseCase;
        private IScoreRepository _mockScoreRepository;
        private IScoreRankingService _mockScoreRankingService;
        private IScoreResetService _mockScoreResetService;
        private IScoreTableRepository _mockScoreTableRepository;
        private ScoreContainer _mockScoreContainer;

        [SetUp]
        public void SetUp()
        {
            // Mock creation
            _mockScoreRepository = Substitute.For<IScoreRepository>();
            _mockScoreRankingService = Substitute.For<IScoreRankingService>();
            _mockScoreResetService = Substitute.For<IScoreResetService>();
            _mockScoreTableRepository = Substitute.For<IScoreTableRepository>();

            // Mock data creation
            _mockScoreContainer = new ScoreContainer
            {
                data = new ScoreData
                {
                    score = new ScoreDetail { best = 100, lastPlayedDate = "2024-09-30" },
                    rankings = new Rankings
                    {
                        daily = new ScoreList { scores = new[] { 10, 20, 30 } },
                        monthly = new ScoreList { scores = new[] { 50, 60, 70 } },
                        allTime = new ScoreList { scores = new[] { 100, 150, 200 } }
                    }
                }
            };

            _mockScoreRepository.LoadScoresAsync(Arg.Any<CancellationToken>()).Returns(UniTask.FromResult(_mockScoreContainer));

            // Initialize the class to be tested
            _scoreUseCase = new ScoreUseCase(
                _mockScoreRepository,
                _mockScoreRankingService,
                _mockScoreResetService,
                _mockScoreTableRepository
            );
        }

        [UnityTest]
        public IEnumerator InitializeAsync_ShouldLoadScoresAndResetIfNecessary() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            _mockScoreResetService.ShouldResetDailyScores(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(true);
            _mockScoreResetService.ShouldResetMonthlyScores(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(false);

            // Reset ranking data
            var dailyScores = _mockScoreContainer.data.rankings.daily.scores;

            // Act
            await _scoreUseCase.InitializeAsync(CancellationToken.None);

            // Assert
            Assert.AreEqual(0, _scoreUseCase.CurrentScore.Value);
            Assert.AreEqual(100, _scoreUseCase.BestScore.Value);

            _mockScoreResetService.Received(1).ShouldResetDailyScores(Arg.Any<DateTime>(), Arg.Any<DateTime>());

            // ref Give the actual variable
            _mockScoreResetService.Received(1).ResetScores(ref dailyScores);
        });

        [Test]
        public void UpdateCurrentScore_ShouldAddScoreFromScoreTable()
        {
            // Arrange
            int[] scoreTable = { 10, 20, 30 };
            _mockScoreTableRepository.GetScoreTable().Returns(scoreTable);

            // Act
            _scoreUseCase.UpdateCurrentScore(1);

            // Assert
            Assert.AreEqual(10, _scoreUseCase.CurrentScore.Value);
        }

        [Test]
        public void UpdateCurrentScore_ShouldThrowApplicationException_WhenItemNoIsNegative()
        {
            // Arrange
            int[] scoreTable = { 10, 20, 30 };
            _mockScoreTableRepository.GetScoreTable().Returns(scoreTable);

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _scoreUseCase.UpdateCurrentScore(-1));
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex.InnerException);
            Assert.That(ex.InnerException?.Message, Does.Contain("Item number is out of range."));
        }

        [Test]
        public void UpdateCurrentScore_ShouldThrowApplicationException_WhenItemNoIsOutOfRange()
        {
            // Arrange
            int[] scoreTable = { 10, 20, 30 };
            _mockScoreTableRepository.GetScoreTable().Returns(scoreTable);

            // Act & Assert
            var ex = Assert.Throws<ApplicationException>(() => _scoreUseCase.UpdateCurrentScore(4));
            Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex.InnerException);
            Assert.That(ex.InnerException?.Message, Does.Contain("Item number is out of range."));
        }

        [UnityTest]
        public IEnumerator UpdateScoreRankingAsync_ShouldUpdateRankingsAndBestScore() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            int newScore = 120;
            _mockScoreRankingService.IsNewBestScore(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            // Call InitializeAsync to load score data
            await _scoreUseCase.InitializeAsync(CancellationToken.None);

            // Act
            await _scoreUseCase.UpdateScoreRankingAsync(newScore, CancellationToken.None);

            // Assert
            _mockScoreRankingService.Received(3).UpdateTopScores(Arg.Any<int[]>(), newScore, 7);
            Assert.AreEqual(120, _scoreUseCase.BestScore.Value);
        });

        [TearDown]
        public void TearDown()
        {
            _scoreUseCase = null;
            _mockScoreRepository = null;
            _mockScoreRankingService = null;
            _mockScoreResetService = null;
            _mockScoreTableRepository = null;
            _mockScoreContainer = null;
        }
    }
}
