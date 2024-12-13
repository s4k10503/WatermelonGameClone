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
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        private const int _maxRetries = 3;

        private CancellationTokenSource _cts;

        [Inject]
        public ProjectPresenter(
            ISoundUseCase soundUseCase,
            IExceptionHandlingUseCase exceptionHandlingUseCase)
        {
            _soundUseCase = soundUseCase ?? throw new ArgumentNullException(nameof(soundUseCase));
            _exceptionHandlingUseCase = exceptionHandlingUseCase ?? throw new ArgumentNullException(nameof(exceptionHandlingUseCase));
            _cts = new CancellationTokenSource();
        }

        void IInitializable.Initialize()
        {
            _exceptionHandlingUseCase.SafeExecuteAsync(
                () => _exceptionHandlingUseCase.RetryAsync(() => InitializeAsync(_cts.Token), _maxRetries, _cts.Token), _cts.Token).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            await _exceptionHandlingUseCase.SafeExecuteAsync(async () =>
            {
                await _soundUseCase.InitializeAsync(ct);
                _soundUseCase.PlayBGM();
            }, _cts.Token);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _soundUseCase = null;
        }
    }
}
