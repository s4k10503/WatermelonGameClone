using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Domain
{
    public interface IExceptionHandler
    {
        UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts);
        UniTask SafeExecuteAsync(Func<UniTask> action);
        void SafeExecute(Action action);
    }
}
