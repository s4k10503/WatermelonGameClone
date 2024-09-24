namespace WatermelonGameClone.Domain
{
    public interface ITimeSettingsRepository
    {
        float GetDelayedTime();
        float GetTimeScaleGameStart();
        float GetTimeScaleGameOver();
    }
}