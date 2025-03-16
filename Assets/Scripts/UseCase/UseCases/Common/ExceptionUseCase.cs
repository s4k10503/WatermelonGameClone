using Domain.Interfaces;
using UseCase.Interfaces;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.UseCases.Common
{
    public sealed class ExceptionHandlingUseCase : IExceptionHandlingUseCase
    {
        private readonly IExceptionHandler _exceptionHandler;

        public ExceptionHandlingUseCase(IExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public async UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts)
        {
            await _exceptionHandler.RetryAsync(action, maxRetries, cts);
        }

        public async UniTask SafeExecuteAsync(Func<UniTask> action, CancellationToken cts)
        {
            await _exceptionHandler.SafeExecuteAsync(action, cts);
        }

        public void SafeExecute(Action action)
        {
            _exceptionHandler.SafeExecute(action);
        }
    }
}
