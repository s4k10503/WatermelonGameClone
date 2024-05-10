using UnityEngine;
using UniRx;


namespace WatermelonGameClone
{
    public interface IGameModel
    {
        ISphereModel SphereModel { get; }
        IScoreModel ScoreModel { get; }
        ISoundModel SoundModel { get; }

        IReadOnlyReactiveProperty<GameState> CurrentState { get; }

        // Methods related to game state management
        void SetGameState(GameState newState);
        float GetDelayedTime();
        float GetTimeScaleGameStart();
        float GetTimeScaleGameOver();
    }
}
