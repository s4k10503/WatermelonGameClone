using UniRx;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface IGameStateUseCase
    {
        float DelayedTime { get; }
        float TimeScaleGameStart { get; }
        float TimeScaleGameOver { get; }
        IReadOnlyReactiveProperty<GlobalGameState> GlobalState { get; }
        IReadOnlyReactiveProperty<SceneSpecificState> SceneState { get; }

        void SetGlobalGameState(GlobalGameState newState);
        void SetSceneSpecificState(SceneSpecificState newState);
    }
}
