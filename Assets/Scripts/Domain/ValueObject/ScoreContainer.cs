using System;
using UnityEngine.Serialization;

namespace Domain.ValueObject
{
    [Serializable]
    public class ScoreContainer
    {
        [FormerlySerializedAs("Data")] public ScoreData data;
    }

    [Serializable]
    public class ScoreData
    {
        [FormerlySerializedAs("Score")] public ScoreDetail score;
        [FormerlySerializedAs("Rankings")] public Rankings rankings;
    }

    [Serializable]
    public class ScoreDetail
    {
        [FormerlySerializedAs("UserName")] public string userName;
        [FormerlySerializedAs("Best")] public int best;
        [FormerlySerializedAs("LastPlayedDate")] public string lastPlayedDate;
    }

    [Serializable]
    public class Rankings
    {
        [FormerlySerializedAs("Daily")] public ScoreList daily;
        [FormerlySerializedAs("Monthly")] public ScoreList monthly;
        [FormerlySerializedAs("AllTime")] public ScoreList allTime;
    }

    [Serializable]
    public class ScoreList
    {
        [FormerlySerializedAs("Scores")] public int[] scores;
    }
}
