using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class GameModel : IGameModel
    {
        private ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Initializing);

        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState.ToReadOnlyReactiveProperty();
        private static readonly float s_delayedTime = 1.0f;


        private void OnDestroy()
        {
            _currentState.Dispose();
        }

        public void SetGameState(GameState newState)
        {
            _currentState.Value = newState;
        }

        public float GetDelayedTime()
        {
            return s_delayedTime;
        }
    }
}
