using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Infrastructure
{
    public class ExceptionHandler : IExceptionHandler
    {
        public async UniTask RetryAsync(Func<UniTask> action, int maxRetries, CancellationToken cts)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await action();
                    return;
                }
                catch (OperationCanceledException)
                {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Debug.LogWarning("Operation was canceled.");
#endif
                }
                catch (Exception ex)
                {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Debug.LogWarning($"Retry {i + 1}/{maxRetries} failed: {ex}");
                    if (i == maxRetries - 1)
                    {
                        Debug.LogError($"Max retries reached: {ex}");
                        throw;
                    }
#endif
                }
            }
        }

        public async UniTask SafeExecuteAsync(Func<UniTask> action)
        {
            try
            {
                await action();
            }
            catch (OperationCanceledException)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogWarning("Operation was canceled.");
#endif
            }
            catch (Exception ex)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError($"Unhandled exception: {ex}");
#endif
            }
        }

        public void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (OperationCanceledException)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogWarning("Operation was canceled.");
#endif
            }
            catch (Exception ex)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                Debug.LogError($"Unhandled exception: {ex}");
#endif
            }
        }
    }
}
