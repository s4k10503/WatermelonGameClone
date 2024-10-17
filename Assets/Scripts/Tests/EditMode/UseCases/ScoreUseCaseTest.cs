using NUnit.Framework;
using NSubstitute;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections;
using UniRx;
using UnityEngine.TestTools;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;

namespace WatermelonGameClone.Tests
{
    public class ScoreUseCaseTest
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
                Data = new ScoreData
                {
                    Score = new ScoreDetail { Best = 100, LastPlayedDate = "2024-09-30" },
                    Rankings = new Rankings
                    {
                        Daily = new ScoreList { Scores = new int[] { 10, 20, 30 } },
                        Monthly = new ScoreList { Scores = new int[] { 50, 60, 70 } },
                        AllTime = new ScoreList { Scores = new int[] { 100, 150, 200 } }
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
            var dailyScores = _mockScoreContainer.Data.Rankings.Daily.Scores;
            
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
            Assert.AreEqual(20, _scoreUseCase.CurrentScore.Value);
        }

        [Test]
        public void UpdateCurrentScore_ShouldThrowExceptionForInvalidSphereNo()
        {
            // Arrange
            int[] scoreTable = { 10, 20, 30 };
            _mockScoreTableRepository.GetScoreTable().Returns(scoreTable);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _scoreUseCase.UpdateCurrentScore(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => _scoreUseCase.UpdateCurrentScore(3));
        }

        [UnityTest]
        public IEnumerator UpdateScoreRankingAsync_ShouldUpdateRankingsAndBestScore() => UniTask.ToCoroutine(async () =>
        {
            // Arrange
            int newScore = 120;
            _mockScoreRankingService.IsNewBestScore(Arg.Any<int>(), Arg.Any<int>()).Returns(true);

            // Call InitializEasync to load score data
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
