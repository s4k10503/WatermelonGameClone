using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public interface IExceptionHandlingUseCase
    {
        UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts);
        UniTask SafeExecuteAsync(Func<UniTask> action);
        void SafeExecute(Action action);
    }
}
