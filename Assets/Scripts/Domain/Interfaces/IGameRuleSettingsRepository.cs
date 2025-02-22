namespace Domain.Interfaces
{
    public interface IGameRuleSettingsRepository
    {
        float GetDelayedTime();
        float GetTimeScaleGameStart();
        float GetTimeScaleGameOver();
        float GetContactTimeLimit();
    }
}