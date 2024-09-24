using UnityEngine;

namespace WatermelonGameClone.Domain
{
    public interface ISoundEffectsRepository
    {
        AudioClip GetClip(SoundEffect effect);
    }
}
