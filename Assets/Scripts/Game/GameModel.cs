using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Zenject;


namespace WatermelonGameClone
{
    public class GameModel : IGameModel
    {
        public ISphereModel SphereModel { get; private set; }
        public IScoreModel ScoreModel { get; private set; }
        public ISoundModel SoundModel { get; private set; }

        private ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Initializing);
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState.ToReadOnlyReactiveProperty();

        public static readonly float s_delayedTime = 1.0f;
        public static readonly float s_timeScaleGameStart = 1.0f;
        public static readonly float s_timeScaleGameOver = 0.0f;

        [Inject]
        public GameModel(
            ISphereModel sphereModel,
            IScoreModel scoreModel,
            ISoundModel soundModel)
        {
            this.SphereModel = sphereModel;
            this.ScoreModel = scoreModel;
            this.SoundModel = soundModel;
        }

        private void OnDestroy()
        {
            _currentState.Dispose();
        }

        public void SetGameState(GameState newState)
        {
            _currentState.Value = newState;
        }

        public float GetDelayedTime() => s_delayedTime;
        public float GetTimeScaleGameStart() => s_timeScaleGameStart;
        public float GetTimeScaleGameOver() => s_timeScaleGameOver;
    }
}
