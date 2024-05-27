using UnityEngine;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class SoundModel : ISoundModel
    {
        private Dictionary<SoundEffect, AudioClip> _soundEffects = new Dictionary<SoundEffect, AudioClip>();
        private readonly SoundSettings _soundSettings;


        [Inject]
        public SoundModel(SoundSettings soundSettings)
        {
            _soundSettings = soundSettings;
            LoadSoundEffects();
        }

        private void LoadSoundEffects()
        {
            _soundEffects[SoundEffect.Drop] = _soundSettings.DropClip;
            _soundEffects[SoundEffect.Merge] = _soundSettings.MergeClip;
        }

        public void SetSoundVolume(float volume)
        {
            _soundSettings.SoundVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        }

        public void PlaySoundEffect(SoundEffect effect, AudioSource source)
        {
            if (_soundEffects.TryGetValue(effect, out AudioClip clip))
            {
                source.clip = clip;
                source.volume = _soundSettings.SoundVolume;
                source.Play();
            }
        }
    }
}