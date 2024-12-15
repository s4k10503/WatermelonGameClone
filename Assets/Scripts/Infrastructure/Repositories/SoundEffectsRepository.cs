using System;
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
            _soundSettings = soundSettings ?? throw new ArgumentNullException(nameof(soundSettings));

            // Validate sound settings
            if (_soundSettings.ClipDrop == null)
            {
                throw new InfrastructureException("Drop sound clip is not configured in SoundSettings.");
            }
            if (_soundSettings.ClipMerge == null)
            {
                throw new InfrastructureException("Merge sound clip is not configured in SoundSettings.");
            }
        }

        public AudioClip GetClip(SoundEffect effect)
        {
            return effect switch
            {
                SoundEffect.Drop => _soundSettings.ClipDrop,
                SoundEffect.Merge => _soundSettings.ClipMerge,
                _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, "Sound effect not found in settings.")
            };
        }
    }
}
