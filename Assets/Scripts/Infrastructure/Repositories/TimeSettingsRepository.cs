using System;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public class TimeSettingsRepository : ITimeSettingsRepository
    {
        private readonly TimeSettings _timeSettings;

        [Inject]
        public TimeSettingsRepository(TimeSettings timeSettings)
        {
            _timeSettings = timeSettings ?? throw new ArgumentNullException(nameof(timeSettings));

            // Validate TimeSettings properties
            if (_timeSettings.DelayedTime < 0)
            {
                throw new InfrastructureException("DelayedTime cannot be negative in TimeSettings.");
            }
            if (_timeSettings.TimeScaleGameStart <= 0)
            {
                throw new InfrastructureException("TimeScaleGameStart must be greater than zero in TimeSettings.");
            }
            if (_timeSettings.TimeScaleGameOver != 0)
            {
                throw new InfrastructureException("TimesCalegameOver must be 0");
            }
        }

        public float GetDelayedTime()
        {
            return _timeSettings.DelayedTime;
        }

        public float GetTimeScaleGameStart()
        {
            return _timeSettings.TimeScaleGameStart;
        }

        public float GetTimeScaleGameOver()
        {
            return _timeSettings.TimeScaleGameOver;
        }
    }
}
