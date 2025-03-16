using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.Interfaces
{
    public interface IExceptionHandlingUseCase
    {
        UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts);
        UniTask SafeExecuteAsync(Func<UniTask> action, CancellationToken cts);
        void SafeExecute(Action action);
    }
}
