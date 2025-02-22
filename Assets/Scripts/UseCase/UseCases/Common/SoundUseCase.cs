using Domain.Interfaces;
using Domain.ValueObject;
using UseCase.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace UseCase.UseCases.Common
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
            [Inject(Id = "SE")] AudioSource audioSourceSe,
            ISoundVolumeRepository soundVolumeRepository,
            ISoundEffectsRepository soundEffectsRepository)
        {
            _audioSourceBgm = audioSourceBGM ?? throw new ArgumentNullException(nameof(audioSourceBGM));
            _audioSourceSe = audioSourceSe ?? throw new ArgumentNullException(nameof(audioSourceSe));
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
                (float volumeBgm, float volumeSe) = await _soundVolumeRepository.LoadSoundSettingsAsync(ct);

                _audioSourceBgm.volume = ValidateVolume(volumeBgm, "VolumeBgm");
                _audioSourceSe.volume = ValidateVolume(volumeSe, "VolumeSe");

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

        public void SetSeVolume(float volumeSe)
            => _audioSourceSe.volume = ValidateVolume(volumeSe, nameof(volumeSe));

        public void PlayBGM()
            => _audioSourceBgm.Play();

        public async UniTask SaveVolume(CancellationToken ct)
        {
            await _soundVolumeRepository.SaveSoundSettingsAsync(_audioSourceBgm.volume, _audioSourceSe.volume, ct);
        }

        public void PlaySoundEffect(string soundName)
        {
            if (!Enum.TryParse(soundName, out SoundEffect soundEffect))
            {
                throw new ArgumentException($"Sound effect '{soundName}' is not a valid SoundEffect.", nameof(soundName));
            }

            if (!_soundEffects.TryGetValue(soundEffect, out AudioClip clip) || clip == null)
            {
                throw new ArgumentException($"Sound effect '{soundName}' not found or not initialized.", nameof(soundName));
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
