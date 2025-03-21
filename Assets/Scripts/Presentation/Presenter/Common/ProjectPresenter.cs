using UseCase.Interfaces;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Presentation.Presenter.Common
{
    public class ProjectPresenter : IInitializable, IDisposable
    {
        // Model
        private ISoundUseCase _soundUseCase;
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        private const int MaxRetries = 3;

        private readonly CancellationTokenSource _cts;

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
                () => _exceptionHandlingUseCase.RetryAsync(
                    () => InitializeAsync(_cts.Token), MaxRetries, _cts.Token), _cts.Token).Forget();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            try
            {
                if (_cts == null || _cts.IsCancellationRequested) return;

                await _soundUseCase.InitializeAsync(ct);
                _soundUseCase.PlayBGM();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during initialization.", ex);
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _soundUseCase = null;
        }
    }
}
