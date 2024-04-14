using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IGameModel
    {
        // Property to access the current game state in a read-only manner
        IReadOnlyReactiveProperty<GameState> CurrentState { get; }

        // Methods related to game state management
        void SetGameState(GameState newState);
        float GetDelayedTime();
    }
}
