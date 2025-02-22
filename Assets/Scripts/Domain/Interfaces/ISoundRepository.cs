using System.Threading;
using Cysharp.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISoundVolumeRepository
    {
        UniTask SaveSoundSettingsAsync(float volumeBGM, float volumeSe, CancellationToken ct);
        UniTask<(float VolumeBGM, float VolumeSE)> LoadSoundSettingsAsync(CancellationToken ct);
    }
}
