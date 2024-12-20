using System;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class SoundUseCase : ISoundUseCase, IDisposable
    {
        private readonly ISoundEffectsRepository _soundEffectsRepository;
        private readonly ISoundVolumeRepository _soundVolumeRepository;
        private readonly Dictionary<SoundEffect, AudioClip> _soundEffects = new();
        private readonly AudioSource _audioSourceBgm;
        private readonly AudioSource _audioSourceSe;

        public IReadOnlyReactiveProperty<float> VolumeBgm => _audioSourceBgm.ObserveEveryValueChanged(x => x.volume).ToReactiveProperty();
        public IReadOnlyReactiveProperty<float> VolumeSe => _audioSourceSe.ObserveEveryValueChanged(x => x.volume).ToReactiveProperty();

        [Inject]
        public SoundUseCase(
            [Inject(Id = "BGM")] AudioSource audioSourceBGM,
            [Inject(Id = "SE")] AudioSource audioSourceSE,
            ISoundVolumeRepository soundVolumeRepository,
            ISoundEffectsRepository soundEffectsRepository)
        {
            _audioSourceBgm = audioSourceBGM ?? throw new ArgumentNullException(nameof(audioSourceBGM));
            _audioSourceSe = audioSourceSE ?? throw new ArgumentNullException(nameof(audioSourceSE));
            _soundVolumeRepository = soundVolumeRepository ?? throw new ArgumentNullException(nameof(soundVolumeRepository));
            _soundEffectsRepository = soundEffectsRepository ?? throw new ArgumentNullException(nameof(soundEffectsRepository));
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            await LoadSoundSettingsAsync(ct);
        }

        public void Dispose()
        {
            _soundEffects.Clear();
        }

        private async UniTask LoadSoundSettingsAsync(CancellationToken ct)
        {
            try
            {
                (float volumeBgm, float volumeSE) = await _soundVolumeRepository.LoadSoundSettingsAsync(ct);

                _audioSourceBgm.volume = ValidateVolume(volumeBgm, "VolumeBgm");
                _audioSourceSe.volume = ValidateVolume(volumeSE, "VolumeSe");

                _soundEffects[SoundEffect.Drop] = _soundEffectsRepository.GetClip(SoundEffect.Drop);
                _soundEffects[SoundEffect.Merge] = _soundEffectsRepository.GetClip(SoundEffect.Merge);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
        }

        public void SetBGMVolume(float volumeBgm)
            => _audioSourceBgm.volume = ValidateVolume(volumeBgm, nameof(volumeBgm));

        public void SetSEVolume(float volumeSE)
            => _audioSourceSe.volume = ValidateVolume(volumeSE, nameof(volumeSE));

        public void PlayBGM()
            => _audioSourceBgm.Play();

        public async UniTask SaveVolume(CancellationToken ct)
        {
            await _soundVolumeRepository.SaveSoundSettingsAsync(_audioSourceBgm.volume, _audioSourceSe.volume, ct);
        }

        public void PlaySoundEffect(SoundEffect effect)
        {
            if (!_soundEffects.TryGetValue(effect, out AudioClip clip) || clip == null)
            {
                throw new ArgumentException($"Sound effect '{effect}' not found or not initialized.", nameof(effect));
            }

            _audioSourceSe.clip = clip;
            _audioSourceSe.Play();
        }

        private static float ValidateVolume(float value, string parameterName)
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentException($"{parameterName} must be between 0 and 1.", parameterName);
            }
            return value;
        }
    }
}
