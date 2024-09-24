using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public sealed class SoundUseCase : ISoundUseCase
    {
        private Dictionary<SoundEffect, AudioClip> _soundEffects;
        private ISoundEffectsRepository _soundEffectsRepository;
        private ISoundVolumeRepository _soundVolumeRepository;
        private AudioSource _audioSourceBgm;
        private AudioSource _audioSourceSe;

        // ReactiveProperties
        public IReadOnlyReactiveProperty<float> VolumeBgm
            => _audioSourceBgm
                .ObserveEveryValueChanged(x => x.volume)
                .ToReactiveProperty();
        public IReadOnlyReactiveProperty<float> VolumeSe
            => _audioSourceBgm
                .ObserveEveryValueChanged(x => x.volume)
                .ToReactiveProperty();

        [Inject]
        public SoundUseCase(
            [Inject(Id = "BGM")] AudioSource audioSourceBGM,
            [Inject(Id = "SE")] AudioSource audioSourceSE,
            ISoundVolumeRepository soundVolumeRepository,
            ISoundEffectsRepository soundEffectsRepository)
        {
            _soundEffects = new Dictionary<SoundEffect, AudioClip>();
            _audioSourceBgm = audioSourceBGM;
            _audioSourceSe = audioSourceSE;
            _soundVolumeRepository = soundVolumeRepository;
            _soundEffectsRepository = soundEffectsRepository;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            await LoadSoundSettingsAsync(ct);
        }

        private async UniTask LoadSoundSettingsAsync(CancellationToken ct)
        {
            (float volumeBgm, float volumeSE) = await _soundVolumeRepository.LoadSoundSettingsAsync(ct);
            _audioSourceBgm.volume = volumeBgm;
            _audioSourceSe.volume = volumeSE;

            _soundEffects[SoundEffect.Drop] = _soundEffectsRepository.GetClip(SoundEffect.Drop);
            _soundEffects[SoundEffect.Merge] = _soundEffectsRepository.GetClip(SoundEffect.Merge);
        }

        public void SetBGMVolume(float volumeBgm)
            => _audioSourceBgm.volume = volumeBgm;

        public void SetSEVolume(float volumeSE)
            => _audioSourceSe.volume = volumeSE;

        public void PlayBGM()
            => _audioSourceBgm.Play();

        public async UniTask SaveVolume(CancellationToken ct)
        {
            await _soundVolumeRepository
                .SaveSoundSettingsAsync(_audioSourceBgm.volume, _audioSourceSe.volume, ct);
        }

        public void PlaySoundEffect(SoundEffect effect)
        {
            if (_soundEffects.TryGetValue(effect, out AudioClip clip))
            {
                _audioSourceSe.clip = clip;
                _audioSourceSe.Play();
            }
        }
    }
}