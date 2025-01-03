namespace WatermelonGameClone.Domain
{
    public interface IGameRuleSettingsRepository
    {
        float GetDelayedTime();
        float GetTimeScaleGameStart();
        float GetTimeScaleGameOver();
        float GetContactTimeLimit();
    }
}