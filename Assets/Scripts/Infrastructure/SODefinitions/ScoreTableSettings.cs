using UnityEngine;
using Zenject;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "ScoreTableSettings", menuName = "Configs/ScoreTableSettings")]
    public class ScoreTableSettings : ScriptableObjectInstaller
    {
        public int[] Scores;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}
