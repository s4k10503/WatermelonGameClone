using UnityEngine;
using Zenject;

namespace Infrastructure.SODefinitions
{
    [CreateAssetMenu(fileName = "TimeSettings", menuName = "Configs/TimeSettings")]
    public class GameRuleSettings : ScriptableObjectInstaller
    {
        public float delayedTime = 1.0f;
        public float timeScaleGameStart = 1.0f;
        public float timeScaleGameOver = 0.0f;
        public float contactTimeLimit = 1.0f;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}