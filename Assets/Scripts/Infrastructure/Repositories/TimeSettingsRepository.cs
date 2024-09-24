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
            _timeSettings = timeSettings;
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
