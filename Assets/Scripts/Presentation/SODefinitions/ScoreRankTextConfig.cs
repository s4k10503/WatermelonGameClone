using UnityEngine;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    [CreateAssetMenu(fileName = "ScoreRankTextConfig", menuName = "Configs/ScoreRankTextConfig")]
    public class ScoreRankTextConfig : ScriptableObjectInstaller
    {
        public string dailyRankingTitle = "Daily Ranking";
        public string monthlyRankingTitle = "Monthly Ranking";
        public string allTimeRankingTitle = "AllTime Ranking";

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}
