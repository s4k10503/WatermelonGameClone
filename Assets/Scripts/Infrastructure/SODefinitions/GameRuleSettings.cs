using UnityEngine;
using Zenject;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "TimeSettings", menuName = "Configs/TimeSettings")]
    public class GameRuleSettings : ScriptableObjectInstaller
    {
        public float DelayedTime = 1.0f;
        public float TimeScaleGameStart = 1.0f;
        public float TimeScaleGameOver = 0.0f;
        public float ContactTimeLimit = 1.0f;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}