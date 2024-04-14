using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public class SoundModel : ISoundModel
    {
        private Dictionary<SoundEffect, AudioClip> soundEffects = new Dictionary<SoundEffect, AudioClip>();

        private float _soundVolume = 1.0f;
        private float _audioVolume;

        public void SetSoundEffect()
        {
            LoadSoundEffect(SoundEffect.Drop, "Drop");
            LoadSoundEffect(SoundEffect.Merge, "Merge");
        }

        public void SetSoundVolume(float volume)
        {
            _soundVolume = Mathf.Clamp(volume, 0.0f, 1.0f);
        }

        public void PlaySoundEffect(SoundEffect effect, AudioSource source)
        {
            if (soundEffects.TryGetValue(effect, out AudioClip clip))
            {
                source.clip = clip;
                source.volume = _soundVolume;
                source.Play();
            }
        }

        private void LoadSoundEffect(SoundEffect effect, string resourcePath)
        {
            AudioClip clip = Resources.Load<AudioClip>(resourcePath);
            if (clip != null)
            {
                soundEffects[effect] = clip;
            }
            else
            {
                Debug.LogError("Sound effect not found at path: " + resourcePath);
            }
        }
    }
}