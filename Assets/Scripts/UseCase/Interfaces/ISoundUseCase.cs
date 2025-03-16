using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace UseCase.Interfaces
{
    public interface ISoundUseCase
    {
        IReadOnlyReactiveProperty<float> VolumeBgm { get; }
        IReadOnlyReactiveProperty<float> VolumeSe { get; }

        UniTask InitializeAsync(CancellationToken ct);
        void SetBGMVolume(float volumeBGM);
        void SetSeVolume(float volumeSe);
        UniTask SaveVolume(CancellationToken ct);
        void PlaySoundEffect(string soundName);
        void PlayBGM();
    }
}
