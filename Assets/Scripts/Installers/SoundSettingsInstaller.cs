using UnityEngine;
using Zenject;

namespace WatermelonGameClone
{
    [CreateAssetMenu(fileName = "SoundSettingsInstaller", menuName = "Installers/SoundSettingsInstaller")]
    public class SoundSettingsInstaller : ScriptableObjectInstaller<SoundSettingsInstaller>
    {
        public SoundSettings soundSettings;

        public override void InstallBindings()
        {
            Container
                .BindInstance(soundSettings)
                .AsSingle();
        }
    }
}