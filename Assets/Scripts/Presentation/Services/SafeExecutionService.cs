using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Presentation.Services
{
    /// <summary>
    /// Presenter内での繰り返し例外処理パターンを共通化するサービス
    /// </summary>
    public static class SafeExecutionService
    {
        /// <summary>
        /// 同期処理を安全に実行する
        /// </summary>
        /// <param name="action">実行するアクション</param>
        /// <param name="onError">エラー時のハンドラ（オプション）</param>
        public static void SafeExecute(Action action, Action<Exception> onError = null)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                UnityEngine.Debug.LogError($"SafeExecute failed: {ex.Message}");
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 非同期処理を安全に実行する
        /// </summary>
        /// <param name="task">実行するタスク</param>
        /// <param name="ct">キャンセレーショントークン</param>
        /// <param name="onError">エラー時のハンドラ（オプション）</param>
        public static async UniTask SafeExecuteAsync(Func<UniTask> task, CancellationToken ct, Action<Exception> onError = null)
        {
            try
            {
                if (task != null)
                {
                    await task();
                }
            }
            catch (OperationCanceledException)
            {
                // キャンセレーションは正常な処理として扱う
                throw;
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                UnityEngine.Debug.LogError($"SafeExecuteAsync failed: {ex.Message}");
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// リトライ機能付きで非同期処理を安全に実行する
        /// </summary>
        /// <param name="task">実行するタスク</param>
        /// <param name="maxRetries">最大リトライ回数</param>
        /// <param name="ct">キャンセレーショントークン</param>
        /// <param name="onError">エラー時のハンドラ（オプション）</param>
        public static async UniTask SafeExecuteWithRetryAsync(
            Func<UniTask> task, 
            int maxRetries, 
            CancellationToken ct, 
            Action<Exception> onError = null)
        {
            if (task == null) return;

            int retryCount = 0;
            while (retryCount <= maxRetries)
            {
                try
                {
                    await task();
                    return; // 成功したら終了
                }
                catch (OperationCanceledException)
                {
                    // キャンセレーションは正常な処理として扱う
                    throw;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    
                    if (retryCount > maxRetries)
                    {
                        onError?.Invoke(ex);
                        UnityEngine.Debug.LogError($"SafeExecuteWithRetryAsync failed after {maxRetries} retries: {ex.Message}");
                        UnityEngine.Debug.LogException(ex);
                        return;
                    }

                    // 指数バックオフでリトライ間隔を調整
                    var delay = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 1000);
                    await UniTask.Delay(delay, cancellationToken: ct);
                }
            }
        }
    }
}