using UnityEngine;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "ScoreTableSettings", menuName = "Configs/ScoreTableSettings")]
    public class ScoreTableSettings : ScriptableObject
    {
        public int[] Scores;
    }
}
