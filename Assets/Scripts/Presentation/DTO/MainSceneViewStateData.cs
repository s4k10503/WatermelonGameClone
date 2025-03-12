using Domain.ValueObject;

using UniRx;

namespace Presentation.DTO
{
    public class MainSceneViewStateData
    {
        public ReactiveProperty<int> NextItemIndex { get; }
        public ReactiveProperty<int> CurrentScore { get; set; }
        public ReactiveProperty<int> BestScore { get; set; }
        public ScoreContainer ScoreContainer { get; set; }
        private readonly CompositeDisposable _disposables = new();

        public MainSceneViewStateData(
            int currentScore,
            int bestScore,
            ScoreContainer scoreContainer,
            int nextItemIndex)
        {
            CurrentScore = new ReactiveProperty<int>(currentScore);
            BestScore = new ReactiveProperty<int>(bestScore);
            ScoreContainer = scoreContainer;

            NextItemIndex = new ReactiveProperty<int>(nextItemIndex);

            CurrentScore.AddTo(_disposables);
            BestScore.AddTo(_disposables);
            NextItemIndex.AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}
