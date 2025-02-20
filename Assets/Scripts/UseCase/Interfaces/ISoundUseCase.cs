using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace WatermelonGameClone.UseCase
{
    public interface ISoundUseCase
    {
        IReadOnlyReactiveProperty<float> VolumeBgm { get; }
        IReadOnlyReactiveProperty<float> VolumeSe { get; }

        UniTask InitializeAsync(CancellationToken ct);
        void SetBGMVolume(float volumeBGM);
        void SetSEVolume(float volumeSE);
        UniTask SaveVolume(CancellationToken ct);
        void PlaySoundEffect(string soundName);
        void PlayBGM();
    }
}
