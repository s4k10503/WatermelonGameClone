using UnityEngine;
using System;

namespace WatermelonGameClone
{
    [Serializable]
    public class ScoreContainer
    {
        public int CurrentScore;
        public int BestScore;
        public int[] TodayTopScores;
        public int[] MonthlyTopScores;
        public int[] AllTimeTopScores;
        public string LastPlayedDate;
    }
}