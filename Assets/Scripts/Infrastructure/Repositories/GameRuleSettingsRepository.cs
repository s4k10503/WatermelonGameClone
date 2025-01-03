using System;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public class GameRuleSettingsRepository : IGameRuleSettingsRepository
    {
        private readonly GameRuleSettings _gameRuleSettings;

        [Inject]
        public GameRuleSettingsRepository(GameRuleSettings gameRuleSettings)
        {
            _gameRuleSettings = gameRuleSettings ?? throw new ArgumentNullException(nameof(gameRuleSettings));

            // Validate TimeSettings properties
            if (_gameRuleSettings.DelayedTime < 0)
            {
                throw new InfrastructureException("DelayedTime cannot be negative in TimeSettings.");
            }
            if (_gameRuleSettings.TimeScaleGameStart <= 0)
            {
                throw new InfrastructureException("TimeScaleGameStart must be greater than zero in TimeSettings.");
            }
            if (_gameRuleSettings.TimeScaleGameOver != 0)
            {
                throw new InfrastructureException("TimesCalegameOver must be 0");
            }
            if (_gameRuleSettings.ContactTimeLimit <= 0)
            {
                throw new InfrastructureException("ContactTimeLimit must be greater than zero in TimeSettings.");
            }
        }

        public float GetDelayedTime()
        {
            return _gameRuleSettings.DelayedTime;
        }

        public float GetTimeScaleGameStart()
        {
            return _gameRuleSettings.TimeScaleGameStart;
        }

        public float GetTimeScaleGameOver()
        {
            return _gameRuleSettings.TimeScaleGameOver;
        }

        public float GetContactTimeLimit()
        {
            return _gameRuleSettings.ContactTimeLimit;
        }
    }
}
