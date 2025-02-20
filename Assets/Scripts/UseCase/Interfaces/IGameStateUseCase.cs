using UniRx;

namespace WatermelonGameClone.UseCase
{
    public interface IGameStateUseCase
    {
        IReadOnlyReactiveProperty<string> GlobalStateString { get; }

        float DelayedTime { get; }
        float TimeScaleGameStart { get; }
        float TimeScaleGameOver { get; }

        void SetGlobalGameState(string newState);
    }
}
