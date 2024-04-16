using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class ScoreModel : IScoreModel
    {
        private ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        private ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore.ToReadOnlyReactiveProperty();
        public IReadOnlyReactiveProperty<int> BestScore => _bestScore.ToReadOnlyReactiveProperty();

        private static readonly int s_scoreCoefficient = 10;


        private void OnDestroy()
        {
            _currentScore.Dispose();
            _bestScore.Dispose();
        }

        public void SetBestScore()
        {
            int pastBestScore = PlayerPrefs.GetInt("BestScore");
            if (_currentScore.Value > pastBestScore)
            {
                SaveBestScore(_currentScore.Value);
            }
        }

        public void SaveBestScore(int currentScore)
        {
            _bestScore.Value = currentScore;
            PlayerPrefs.SetInt("BestScore", _bestScore.Value);
        }

        public void SetCurrentScore(int SphereNo)
        {
            int scoreToAdd = (SphereNo + 1) * s_scoreCoefficient;
            _currentScore.Value += scoreToAdd;
        }

    }
}
