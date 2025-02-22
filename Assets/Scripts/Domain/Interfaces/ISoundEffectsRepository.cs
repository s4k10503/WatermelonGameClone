using Domain.ValueObject;

using UnityEngine;

namespace Domain.Interfaces
{
    public interface ISoundEffectsRepository
    {
        AudioClip GetClip(SoundEffect effect);
    }
}
