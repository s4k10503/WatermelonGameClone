using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Domain
{
    public interface ISoundVolumeRepository
    {
        UniTask SaveSoundSettingsAsync(float volumeBGM, float volumeSE, CancellationToken ct);
        UniTask<(float VolumeBGM, float VolumeSE)> LoadSoundSettingsAsync(CancellationToken ct);
    }
}
