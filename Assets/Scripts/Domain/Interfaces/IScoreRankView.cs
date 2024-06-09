using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IScoreRankView
    {
        // Methods related to UI updates
        void DisplayCurrentScore(int currentScore);
        void DisplayTopScores(ScoreContainer scoreContainer);
    }
}
