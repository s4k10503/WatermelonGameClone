using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Zenject;
using System;


namespace WatermelonGameClone
{
    public class GameModel : IGameModel, IDisposable
    {
        public ISphereModel SphereModel { get; private set; }
        public IScoreModel ScoreModel { get; private set; }
        public ISoundModel SoundModel { get; private set; }

        private ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Initializing);
        public IReadOnlyReactiveProperty<GameState> CurrentState => _currentState;

        private CompositeDisposable _disposables;

        public static readonly float s_delayedTime = 1.0f;
        public static readonly float s_timeScaleGameStart = 1.0f;
        public static readonly float s_timeScaleGameOver = 0.0f;

        [Inject]
        public GameModel(
            ISphereModel sphereModel,
            IScoreModel scoreModel,
            ISoundModel soundModel)
        {
            SphereModel = sphereModel;
            ScoreModel = scoreModel;
            SoundModel = soundModel;

            _disposables = new CompositeDisposable();

            // Adding ReactiveProperties to _disposables ensures it gets disposed
            // when ScoreModel is disposed, preventing memory leaks.
            _currentState.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
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
