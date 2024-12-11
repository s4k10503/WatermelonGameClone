using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace WatermelonGameClone.UseCase
{
    public class SceneLoaderUseCase : ISceneLoaderUseCase
    {
        public async UniTask LoadSceneAsync(string sceneName, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                throw new ArgumentException("Scene name cannot be null or empty.", nameof(sceneName));
            }

            var loadOperation = SceneManager.LoadSceneAsync(sceneName);
            loadOperation.allowSceneActivation = false;

            try
            {
                while (loadOperation.progress < 0.9f)
                {
                    await UniTask.Yield(ct);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken: ct);
                loadOperation.allowSceneActivation = true;
                await loadOperation.WithCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }
    }
}
