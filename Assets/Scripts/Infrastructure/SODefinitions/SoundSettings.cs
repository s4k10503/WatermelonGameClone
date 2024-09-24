using UnityEngine;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Audio/SoundSettings")]
    public class SoundSettings : ScriptableObject
    {
        public AudioClip ClipBGM;
        public AudioClip ClipDrop;
        public AudioClip ClipMerge;
    }
}