using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.UseCase
{
    public interface ISceneLoaderUseCase
    {
        UniTask LoadSceneAsync(string sceneName, CancellationToken ct);
    }
}
