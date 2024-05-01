using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreModel
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }
        List<int> TodayTopScores { get; }
        List<int> MonthlyTopScores { get; }
        List<int> AllTimeTopScores { get; }

        void UpdateCurrentScore(int SphereNo);
        void UpdateScoreRanking(int newScore);
        void SaveScoresToJson();
        ScoreContainer LoadScoresFromJson();

    }
}
