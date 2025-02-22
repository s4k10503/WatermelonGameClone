using Domain.Interfaces;
using Infrastructure.Services;
using Infrastructure.SODefinitions;

using System;
using Zenject;

namespace Infrastructure.Repositories
{
    public sealed class GameRuleSettingsRepository : IGameRuleSettingsRepository
    {
        private readonly GameRuleSettings _gameRuleSettings;

        [Inject]
        public GameRuleSettingsRepository(GameRuleSettings gameRuleSettings)
        {
            _gameRuleSettings = gameRuleSettings ?? throw new ArgumentNullException(nameof(gameRuleSettings));

            // Validate TimeSettings properties
            if (_gameRuleSettings.delayedTime < 0)
            {
                throw new InfrastructureException("DelayedTime cannot be negative in TimeSettings.");
            }
            if (_gameRuleSettings.timeScaleGameStart <= 0)
            {
                throw new InfrastructureException("TimeScaleGameStart must be greater than zero in TimeSettings.");
            }
            if (_gameRuleSettings.timeScaleGameOver != 0)
            {
                throw new InfrastructureException("TimesScaleGameOver must be 0");
            }
            if (_gameRuleSettings.contactTimeLimit <= 0)
            {
                throw new InfrastructureException("ContactTimeLimit must be greater than zero in TimeSettings.");
            }
        }

        public float GetDelayedTime()
        {
            return _gameRuleSettings.delayedTime;
        }

        public float GetTimeScaleGameStart()
        {
            return _gameRuleSettings.timeScaleGameStart;
        }

        public float GetTimeScaleGameOver()
        {
            return _gameRuleSettings.timeScaleGameOver;
        }

        public float GetContactTimeLimit()
        {
            return _gameRuleSettings.contactTimeLimit;
        }
    }
}
