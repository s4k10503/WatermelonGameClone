using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using WatermelonGameClone.Domain;


namespace WatermelonGameClone.UseCase
{
    public interface IScoreUseCase
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }

        UniTask InitializeAsync(CancellationToken ct);
        void UpdateCurrentScore(int itemNo);
        UniTask UpdateScoreRankingAsync(int newScore, CancellationToken ct);
        ScoreContainer GetScoreData();
    }
}
