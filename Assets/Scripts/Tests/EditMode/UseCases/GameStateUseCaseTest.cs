using Domain.Interfaces;
using UseCase.UseCases.Common;

using System;
using NSubstitute;
using NUnit.Framework;
using UniRx;


namespace Tests.EditMode.UseCases
{
    public sealed class GameStateUseCaseTest
    {
        private GameStateUseCase _gameStateUseCase;
        private IGameRuleSettingsRepository _mockGameRuleSettingsRepository;

        [SetUp]
        public void SetUp()
        {
            // Creating Mocks
            _mockGameRuleSettingsRepository = Substitute.For<IGameRuleSettingsRepository>();

            // Setup mock to return a default value
            _mockGameRuleSettingsRepository.GetDelayedTime().Returns(2.5f);
            _mockGameRuleSettingsRepository.GetTimeScaleGameStart().Returns(1.0f);
            _mockGameRuleSettingsRepository.GetTimeScaleGameOver().Returns(0.0f);

            // Create use cases to be tested
            _gameStateUseCase = new GameStateUseCase(_mockGameRuleSettingsRepository);
        }

        [Test]
        public void Constructor_ShouldInitializeTimeSettingsFromRepository()
        {
            // Test if time settings are initialized correctly
            Assert.AreEqual(2.5f, _gameStateUseCase.DelayedTime);
            Assert.AreEqual(1.0f, _gameStateUseCase.TimeScaleGameStart);
            Assert.AreEqual(0.0f, _gameStateUseCase.TimeScaleGameOver);

            // Check to see if the mock was called
            _mockGameRuleSettingsRepository.Received(1).GetDelayedTime();
            _mockGameRuleSettingsRepository.Received(1).GetTimeScaleGameStart();
            _mockGameRuleSettingsRepository.Received(1).GetTimeScaleGameOver();
        }

        [Test]
        public void SetGlobalGameState_ShouldUpdateGlobalState()
        {
            // Set global status
            _gameStateUseCase.SetGlobalGameState("Playing");

            // Verify that the global state has been updated as expected
            Assert.AreEqual("Playing", _gameStateUseCase.GlobalStateString.Value);
        }

        [Test]
        public void SetGlobalGameState_ShouldNotifyObservers()
        {
            // Flag to check if you received the notification
            bool notified = false;

            // Subscription
            _gameStateUseCase.GlobalStateString
                .Skip(1) // Skip the notification of the initial value
                .Subscribe(_ => notified = true);

            // Change the global status
            _gameStateUseCase.SetGlobalGameState("Playing");

            // Assert whether you received the notification
            Assert.IsTrue(notified);
        }

        [Test]
        public void SetGlobalGameState_WithInvalidValue_ShouldThrowException()
        {
            // Pass an invalid state string to make sure an exception occurs
            Assert.Throws<ArgumentException>(() =>
                _gameStateUseCase.SetGlobalGameState("InvalidState")
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
