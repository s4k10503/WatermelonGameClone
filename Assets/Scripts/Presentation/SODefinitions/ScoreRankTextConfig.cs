using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    [CreateAssetMenu(fileName = "ScoreRankTextConfig", menuName = "Configs/ScoreRankTextConfig")]
    public class ScoreRankTextConfig : ScriptableObject
    {
        public string dailyRankingTitle = "Daily Ranking";
        public string monthlyRankingTitle = "Monthly Ranking";
        public string allTimeRankingTitle = "AllTime Ranking";
    }
}
