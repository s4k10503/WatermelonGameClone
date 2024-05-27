using UnityEngine;

namespace WatermelonGameClone
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Audio/SoundSettings")]
    public class SoundSettings : ScriptableObject
    {
        public AudioClip DropClip;
        public AudioClip MergeClip;
        public float SoundVolume;
    }
}