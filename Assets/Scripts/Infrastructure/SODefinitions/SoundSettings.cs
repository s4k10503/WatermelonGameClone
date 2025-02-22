using UnityEngine;
using Zenject;

namespace Infrastructure.SODefinitions
{
    [CreateAssetMenu(fileName = "SoundSettings", menuName = "Audio/SoundSettings")]
    public class SoundSettings : ScriptableObjectInstaller
    {
        public AudioClip clipBGM;
        public AudioClip clipDrop;
        public AudioClip clipMerge;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}
