using System;
using UnityEngine.Serialization;

namespace Domain.ValueObject
{
    [Serializable]
    public class VolumeSettings
    {
        [FormerlySerializedAs("VolumeBGM")] public float volumeBGM;
        [FormerlySerializedAs("VolumeSE")] public float volumeSe;
    }
}
