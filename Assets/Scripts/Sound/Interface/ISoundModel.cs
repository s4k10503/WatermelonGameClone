using UnityEngine;
using System.Collections.Generic;
using UniRx;

namespace WatermelonGameClone
{
    public interface ISoundModel
    {
        public void SetSoundVolume(float volume);
        public void PlaySoundEffect(SoundEffect effect, AudioSource source);

    }
}