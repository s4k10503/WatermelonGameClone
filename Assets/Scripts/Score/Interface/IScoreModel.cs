using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreModel
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }

        public void SetBestScore();
        public void SaveBestScore(int currentScore);
        public void SetCurrentScore(int SphereNo);

    }
}
