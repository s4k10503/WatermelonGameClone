using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreRankView
    {
        // Methods related to UI updates
        void UpdateCurrentScore(int currentScore);
        void DisplayTopScores(List<int> scores);
    }
}
