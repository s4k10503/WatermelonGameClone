using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExceptionHandler
    {
        UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts);
        UniTask SafeExecuteAsync(Func<UniTask> action, CancellationToken cts);
        void SafeExecute(Action action);
    }
}
