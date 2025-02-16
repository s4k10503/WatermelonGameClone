using UnityEngine;
using Zenject;

namespace WatermelonGameClone.Infrastructure
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Audio/SoundSettings")]
    public class SoundSettings : ScriptableObjectInstaller
    {
        public AudioClip ClipBGM;
        public AudioClip ClipDrop;
        public AudioClip ClipMerge;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}