using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class GameStateUseCase : IGameStateUseCase, IDisposable
    {
        // State of the entire game (global state)
        private readonly ReactiveProperty<GlobalGameState> _globalState
            = new ReactiveProperty<GlobalGameState>(GlobalGameState.Title);
        public IReadOnlyReactiveProperty<GlobalGameState> GlobalState
            => _globalState;

        // scene specific state
        private readonly ReactiveProperty<SceneSpecificState> _sceneState
            = new ReactiveProperty<SceneSpecificState>(SceneSpecificState.Initializing);
        public IReadOnlyReactiveProperty<SceneSpecificState> SceneState
            => _sceneState;

        // Time related settings
        public float DelayedTime { get; private set; }
        public float TimeScaleGameStart { get; private set; }
        public float TimeScaleGameOver { get; private set; }

        private readonly ITimeSettingsRepository _timeSettingsRepository;

        [Inject]
        public GameStateUseCase(ITimeSettingsRepository timeSettingsRepository)
        {
            _timeSettingsRepository = timeSettingsRepository;

            // Get time settings
            DelayedTime = _timeSettingsRepository.GetDelayedTime();
            TimeScaleGameStart = _timeSettingsRepository.GetTimeScaleGameStart();
            TimeScaleGameOver = _timeSettingsRepository.GetTimeScaleGameOver();
        }

        public void Dispose()
        {
            _globalState?.Dispose();
            _sceneState?.Dispose();
        }

        // set global game state
        public void SetGlobalGameState(GlobalGameState newState)
        {
            if (!Enum.IsDefined(typeof(GlobalGameState), newState))
            {
                throw new ArgumentException("The invalid globalgamestate value is specified", nameof(newState));
            }

            _globalState.Value = newState;
        }

        //Set scene-specific state
        public void SetSceneSpecificState(SceneSpecificState newState)
            => _sceneState.Value = newState;
    }
}
