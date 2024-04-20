using UnityEngine;
using System;

namespace WatermelonGameClone
{
    [Serializable]
    public class ScoreContainer
    {
        public int CurrentScore;
        public int[] TodayTopScores;
        public int BestScore;
        public string LastPlayedDate;
    }
}