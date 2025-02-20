using System;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class GameStateUseCase : IGameStateUseCase, IDisposable
    {
        // State of the entire game (global state)
        private readonly ReactiveProperty<GlobalGameState> _globalState = new(GlobalGameState.Title);
        private readonly ReactiveProperty<string> _globalStateString;
        public IReadOnlyReactiveProperty<string> GlobalStateString => _globalStateString;

        // Time related settings
        public float DelayedTime { get; private set; }
        public float TimeScaleGameStart { get; private set; }
        public float TimeScaleGameOver { get; private set; }

        private readonly IGameRuleSettingsRepository _gameRuleSettingsRepository;

        [Inject]
        public GameStateUseCase(IGameRuleSettingsRepository gameRuleSettingsRepository)
        {
            _gameRuleSettingsRepository = gameRuleSettingsRepository ??
                throw new ArgumentNullException(nameof(gameRuleSettingsRepository));

            // Get time settings
            DelayedTime = ValidateTimeValue(_gameRuleSettingsRepository.GetDelayedTime(), "DelayedTime");
            TimeScaleGameStart = ValidateTimeValue(_gameRuleSettingsRepository.GetTimeScaleGameStart(), "TimeScaleGameStart");
            TimeScaleGameOver = ValidateTimeValue(_gameRuleSettingsRepository.GetTimeScaleGameOver(), "TimeScaleGameOver");

            _globalStateString = new ReactiveProperty<string>(_globalState.Value.ToString());

            // Every time the value of _globalState changes, the _globalStateString reflects an update
            _globalState.Subscribe(state => _globalStateString.Value = state.ToString());
        }

        public void Dispose()
        {
            _globalState?.Dispose();
            _globalStateString?.Dispose();
        }

        // Set global game state
        public void SetGlobalGameState(string newState)
        {
            if (Enum.TryParse<GlobalGameState>(newState, out var parsedState))
            {
                _globalState.Value = parsedState;
            }
            else
            {
                throw new ArgumentException("Invalid global game state string", nameof(newState));
            }
        }

        private static float ValidateTimeValue(float value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentException($"{parameterName} cannot be negative.", parameterName);
            }
            return value;
        }
    }
}
