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
        private ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

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

        public ReactiveProperty<int> BestScore
        {
            get { return _bestScore; }
            private set { _bestScore = value; }
        }

        public void ChangeState(GameState newState)
        {
            CurrentState.Value = newState;
        }

        public void CalcScore(int sphereNo)
        {
            int scoreToAdd = (sphereNo + 1) * _scoreCoefficient;
            CurrentScore.Value += scoreToAdd;
        }

        public void SaveBestScore(int currentScore)
        {
            BestScore.Value = currentScore;
            PlayerPrefs.SetInt("key1", BestScore.Value);
        }
    }
}
