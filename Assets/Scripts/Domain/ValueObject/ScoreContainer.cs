using System;

namespace WatermelonGameClone.Domain
{
    [Serializable]
    public class ScoreContainer
    {
        public ScoreData Data;
    }

    [Serializable]
    public class ScoreData
    {
        public ScoreDetail Score;
        public Rankings Rankings;
    }

    [Serializable]
    public class ScoreDetail
    {
        public int Best;
        public string LastPlayedDate;
    }

    [Serializable]
    public class Rankings
    {
        public ScoreList Daily;
        public ScoreList Monthly;
        public ScoreList AllTime;
    }

    [Serializable]
    public class ScoreList
    {
        public int[] Scores;
    }

}