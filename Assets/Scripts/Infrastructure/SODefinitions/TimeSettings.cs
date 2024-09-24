using UnityEngine;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "TimeSettings", menuName = "Configs/TimeSettings")]
    public class TimeSettings : ScriptableObject
    {
        public float DelayedTime = 1.0f;
        public float TimeScaleGameStart = 1.0f;
        public float TimeScaleGameOver = 0.0f;

    }
}