using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreModel
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }

        ScoreContainer ScoreData { get; }

        void UpdateCurrentScore(int SphereNo);
        void UpdateScoreRanking(int newScore);
        void SaveScoresToJson();
        void LoadScoresFromJson();
    }
}
