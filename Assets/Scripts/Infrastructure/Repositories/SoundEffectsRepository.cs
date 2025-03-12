using Domain.Interfaces;
using Domain.ValueObject;
using Infrastructure.Services;
using Infrastructure.SODefinitions;

using System;
using UnityEngine;
using Zenject;

namespace Infrastructure.Repositories
{
    public class SoundEffectsRepository : ISoundEffectsRepository
    {
        private readonly SoundSettings _soundSettings;

        [Inject]
        public SoundEffectsRepository(SoundSettings soundSettings)
        {
            _soundSettings = soundSettings ?? throw new ArgumentNullException(nameof(soundSettings));

            // Validate sound settings
            if (_soundSettings.clipDrop == null)
            {
                throw new InfrastructureException("Drop sound clip is not configured in SoundSettings.");
            }
            if (_soundSettings.clipMerge == null)
            {
                throw new InfrastructureException("Merge sound clip is not configured in SoundSettings.");
            }
        }

        public AudioClip GetClip(SoundEffect effect)
        {
            return effect switch
            {
                SoundEffect.Drop => _soundSettings.clipDrop,
                SoundEffect.Merge => _soundSettings.clipMerge,
                _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, "Sound effect not found in settings.")
            };
        }
    }
}
