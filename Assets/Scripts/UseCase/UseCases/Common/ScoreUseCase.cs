using Cysharp.Threading.Tasks;
using UniRx;
using System;
using System.Threading;
using Zenject;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.UseCase
{
    public class ScoreUseCase : IScoreUseCase, IDisposable
    {
        private readonly IScoreRepository _scoreRepository;
        private readonly IScoreRankingService _scoreRankingService;
        private readonly IScoreResetService _scoreResetService;
        private readonly IScoreTableRepository _scoreTableRepository;
        private ScoreContainer _scoreData;

        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore.ToReadOnlyReactiveProperty();
        private readonly ReactiveProperty<int> _bestScore = new ReactiveProperty<int>(0);
        public IReadOnlyReactiveProperty<int> BestScore => _bestScore.ToReadOnlyReactiveProperty();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public ScoreUseCase(
            IScoreRepository scoreRepository,
            IScoreRankingService scoreRankingService,
            IScoreResetService scoreResetService,
            IScoreTableRepository scoreTableRepository)
        {
            _scoreRepository = scoreRepository;
            _scoreRankingService = scoreRankingService;
            _scoreResetService = scoreResetService;
            _scoreTableRepository = scoreTableRepository;

            _currentScore.AddTo(_disposables);
            _bestScore.AddTo(_disposables);
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            _scoreData = await _scoreRepository.LoadScoresAsync(ct);
            DateTime currentDate = DateTime.Today;
            DateTime lastPlayedDate = DateTime.Parse(_scoreData.Data.Score.LastPlayedDate);

            if (_scoreResetService.ShouldResetDailyScores(lastPlayedDate, currentDate))
                _scoreResetService.ResetScores(ref _scoreData.Data.Rankings.Daily.Scores);

            if (_scoreResetService.ShouldResetMonthlyScores(lastPlayedDate, currentDate))
                _scoreResetService.ResetScores(ref _scoreData.Data.Rankings.Monthly.Scores);

            _currentScore.Value = 0;
            _bestScore.Value = _scoreData.Data.Score.Best;
        }

        public void UpdateCurrentScore(int sphereNo)
        {
            var scores = _scoreTableRepository.GetScoreTable();

            // Retrieve scores from the score table
            if (sphereNo >= 0 && sphereNo < scores.Length)
            {
                _currentScore.Value += scores[sphereNo];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(sphereNo), "Sphere number is out of range.");
            }
        }

        public async UniTask UpdateScoreRankingAsync(int newScore, CancellationToken ct)
        {
            if (_scoreData == null || _scoreData.Data?.Rankings?.Daily?.Scores == null)
                throw new NullReferenceException("_scoreData or its nested properties are null");

            _scoreData.Data.Rankings.Daily.Scores = _scoreRankingService.UpdateTopScores(_scoreData.Data.Rankings.Daily.Scores, newScore, 7);
            _scoreData.Data.Rankings.Monthly.Scores = _scoreRankingService.UpdateTopScores(_scoreData.Data.Rankings.Monthly.Scores, newScore, 7);
            _scoreData.Data.Rankings.AllTime.Scores = _scoreRankingService.UpdateTopScores(_scoreData.Data.Rankings.AllTime.Scores, newScore, 7);

            if (_scoreRankingService.IsNewBestScore(_bestScore.Value, newScore))
            {
                _bestScore.Value = newScore;
            }

            await SaveScoresAsync(ct);
        }

        private async UniTask SaveScoresAsync(CancellationToken ct)
        {
            _scoreData.Data.Score.Best = _bestScore.Value;
            _scoreData.Data.Score.LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd");
            await _scoreRepository.SaveScoresAsync(_scoreData, ct);
        }

        public ScoreContainer GetScoreData()
            => _scoreData;

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
