using System.Threading;
using Cysharp.Threading.Tasks;


namespace WatermelonGameClone.Domain
{
    public interface IScoreRepository
    {
        UniTask SaveScoresAsync(ScoreContainer scoreData, CancellationToken cancellationToken);
        UniTask<ScoreContainer> LoadScoresAsync(CancellationToken cancellationToken);
    }
}
