using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreModel
    {
        List<int> TodayTopScores { get; }
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }

        void UpdateCurrentScore(int SphereNo);
        void UpdateTodayTopScores(int newScore);
        void SaveScoresToJson();
        ScoreContainer LoadScoresFromJson();

    }
}
