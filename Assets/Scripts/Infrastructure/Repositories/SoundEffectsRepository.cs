using UnityEngine;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public class SoundEffectsRepository : ISoundEffectsRepository
    {
        private readonly SoundSettings _soundSettings;

        [Inject]
        public SoundEffectsRepository(SoundSettings soundSettings)
        {
            _soundSettings = soundSettings;
        }

        public AudioClip GetClip(SoundEffect effect)
        {
            switch (effect)
            {
                case SoundEffect.Drop:
                    return _soundSettings.ClipDrop;
                case SoundEffect.Merge:
                    return _soundSettings.ClipMerge;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(effect), "Sound effect not found.");
            }
        }
    }
}
