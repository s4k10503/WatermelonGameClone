using Domain.ValueObject;

using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace UseCase.Interfaces
{
    public interface IScoreUseCase
    {
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
        IReadOnlyReactiveProperty<int> BestScore { get; }

        UniTask InitializeAsync(CancellationToken ct);
        void UpdateCurrentScore(int itemNo);
        UniTask UpdateUserNameAsync(string userName, CancellationToken ct);
        UniTask UpdateScoreRankingAsync(int newScore, CancellationToken ct);
        ScoreContainer GetScoreData();
    }
}
