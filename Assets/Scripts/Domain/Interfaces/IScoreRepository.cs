using Domain.ValueObject;

using System.Threading;
using Cysharp.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IScoreRepository
    {
        UniTask SaveScoresAsync(ScoreContainer scoreData, CancellationToken cancellationToken);
        UniTask<ScoreContainer> LoadScoresAsync(CancellationToken cancellationToken);
    }
}
