using System.Threading;
using Cysharp.Threading.Tasks;

namespace UseCase.Interfaces
{
    public interface ISceneLoaderUseCase
    {
        UniTask LoadSceneAsync(string sceneName, CancellationToken ct);
    }
}
