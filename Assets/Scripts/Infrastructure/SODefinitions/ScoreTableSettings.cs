using UnityEngine;
using Zenject;

namespace Infrastructure.SODefinitions
{
    [CreateAssetMenu(fileName = "ScoreTableSettings", menuName = "Configs/ScoreTableSettings")]
    public class ScoreTableSettings : ScriptableObjectInstaller
    {
        public int[] scores;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}
