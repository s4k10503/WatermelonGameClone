using UnityEngine;
using UniRx;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class GameModel
    {
        public enum GameState
        {
            Initializing,
            SphereMoving,
            SphereDropping,
            CheckingCollision,
            Merging,
            GameOver
        }

        private ReactiveProperty<GameState> _currentState = new ReactiveProperty<GameState>(GameState.Initializing);
        private ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private readonly int _scoreCoefficient = 10;

        public ReactiveProperty<GameState> CurrentState
        {
            get { return _currentState; }
            private set { _currentState = value; }
        }

        public ReactiveProperty<int> CurrentScore
        {
            get { return _currentScore; }
            private set { _currentScore = value; }
        }

        public void AddScore(int sphereNo)
        {
            int scoreToAdd = (sphereNo + 1) * _scoreCoefficient;
            CurrentScore.Value += scoreToAdd;
        }

        public void ChangeState(GameState newState)
        {
            CurrentState.Value = newState;
        }
    }
}
