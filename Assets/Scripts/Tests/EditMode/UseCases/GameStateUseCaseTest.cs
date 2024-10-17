using NUnit.Framework;
using UniRx;
using WatermelonGameClone.UseCase;
using WatermelonGameClone.Domain;
using NSubstitute;
using System;

namespace WatermelonGameClone.Tests
{
    public class GameStateUseCaseTest
    {
        private GameStateUseCase _gameStateUseCase;
        private ITimeSettingsRepository _mockTimeSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            // Creating Mocks
            _mockTimeSettingsRepository = Substitute.For<ITimeSettingsRepository>();

            // Creating Mocks
            _mockTimeSettingsRepository.GetDelayedTime().Returns(2.5f);
            _mockTimeSettingsRepository.GetTimeScaleGameStart().Returns(1.0f);
            _mockTimeSettingsRepository.GetTimeScaleGameOver().Returns(0.0f);

            // Create use cases to be tested
            _gameStateUseCase = new GameStateUseCase(_mockTimeSettingsRepository);
        }

        [Test]
        public void Constructor_ShouldInitializeTimeSettingsFromRepository()
        {
            // Test if time settings are initialized correctly
            Assert.AreEqual(2.5f, _gameStateUseCase.DelayedTime);
            Assert.AreEqual(1.0f, _gameStateUseCase.TimeScaleGameStart);
            Assert.AreEqual(0.0f, _gameStateUseCase.TimeScaleGameOver);

            // Check to see if the mock was called
            _mockTimeSettingsRepository.Received(1).GetDelayedTime();
            _mockTimeSettingsRepository.Received(1).GetTimeScaleGameStart();
            _mockTimeSettingsRepository.Received(1).GetTimeScaleGameOver();
        }

        [Test]
        public void SetGlobalGameState_ShouldUpdateGlobalState()
        {
            // Set global status
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);

            // Verify that the global state has been updated as expected
            Assert.AreEqual(GlobalGameState.Playing, _gameStateUseCase.GlobalState.Value);
        }

        [Test]
        public void SetSceneSpecificState_ShouldUpdateSceneState()
        {
            // Set scene-specific status
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.ItemDropping);

            // Verify that scene-specific conditions have been updated as expected
            Assert.AreEqual(SceneSpecificState.ItemDropping, _gameStateUseCase.SceneState.Value);
        }

        [Test]
        public void SetSceneSpecificState_ShouldHandleMergingState()
        {
            // Set scene-specific status (Merging)
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Merging);

            // Verify that the status is correctly set to Merging
            Assert.AreEqual(SceneSpecificState.Merging, _gameStateUseCase.SceneState.Value);
        }

        [Test]
        public void SetGlobalGameState_ShouldNotifyObservers()
        {
            // Flag to check if you received the notification
            bool notified = false;

            // Subscription
            _gameStateUseCase.GlobalState
                .Skip(1) // Skip the notification of the initial value
                .Subscribe(state => notified = true);

            // Change the global status
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);

            // Assert whether you received the notification
            Assert.IsTrue(notified);
        }

        [Test]
        public void SetSceneSpecificState_ShouldNotifyObservers()
        {
            bool notified = false;

            _gameStateUseCase.SceneState
                .Skip(1)
                .Subscribe(state => notified = true);

            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.ItemDropping);

            Assert.IsTrue(notified);
        }

        [Test]
        public void SetGlobalGameState_WithInvalidValue_ShouldThrowException()
        {
            // Check the behavior when you set an invalid value
            Assert.Throws<ArgumentException>(() =>
                _gameStateUseCase.SetGlobalGameState((GlobalGameState)(-1))
            );
        }

        [TearDown]
        public void TearDown()
        {
            _gameStateUseCase.Dispose();
            _gameStateUseCase = null;
        }
    }
}
