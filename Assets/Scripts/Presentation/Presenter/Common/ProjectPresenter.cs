using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;
using WatermelonGameClone.UseCase;

namespace WatermelonGameClone.Presentation
{
    public class ProjectPresenter : IInitializable, IDisposable
    {
        // Model
        private ISoundUseCase _soundUseCase;

        private CancellationTokenSource _cts;


        [Inject]
        public ProjectPresenter(ISoundUseCase soundUseCase)
        {
            _soundUseCase = soundUseCase;
            _cts = new CancellationTokenSource();
        }

        void IInitializable.Initialize()
        {
            InitializeAsync(_cts.Token).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            await _soundUseCase.InitializeAsync(ct);
            _soundUseCase.PlayBGM();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
