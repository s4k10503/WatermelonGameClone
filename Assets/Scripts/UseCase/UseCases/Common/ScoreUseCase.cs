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
            _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
            _scoreRankingService = scoreRankingService ?? throw new ArgumentNullException(nameof(scoreRankingService));
            _scoreResetService = scoreResetService ?? throw new ArgumentNullException(nameof(scoreResetService));
            _scoreTableRepository = scoreTableRepository ?? throw new ArgumentNullException(nameof(scoreTableRepository));

            _currentScore.AddTo(_disposables);
            _bestScore.AddTo(_disposables);
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            try
            {
                _scoreData = await _scoreRepository.LoadScoresAsync(ct)
                    ?? throw new ApplicationException("Failed to load score data. The returned data is null.");

                DateTime currentDate = DateTime.Today;
                DateTime lastPlayedDate = DateTime.Parse(_scoreData.Data.Score.LastPlayedDate);

                if (_scoreResetService.ShouldResetDailyScores(lastPlayedDate, currentDate))
                {
                    _scoreResetService.ResetScores(ref _scoreData.Data.Rankings.Daily.Scores);
                }

                if (_scoreResetService.ShouldResetMonthlyScores(lastPlayedDate, currentDate))
                {
                    _scoreResetService.ResetScores(ref _scoreData.Data.Rankings.Monthly.Scores);
                }

                _currentScore.Value = 0;
                _bestScore.Value = _scoreData.Data.Score.Best;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during score initialization.", ex);
            }
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _scoreData = null;
        }

        public void UpdateCurrentScore(int itemNo)
        {
            try
            {
                var scores = _scoreTableRepository.GetScoreTable();

                if (scores == null || scores.Length == 0)
                {
                    throw new ApplicationException("Score table is invalid or empty.");
                }

                if (itemNo > scores.Length || 0 > itemNo)
                {
                    throw new ArgumentOutOfRangeException(nameof(itemNo), "Item number is out of range.");
                }

                // The score table has a value corresponding to each item
                // itemNo is the number of the item after merge, so subtract 1 to get the score
                _currentScore.Value += scores[itemNo - 1];
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the current score.", ex);
            }
        }

        public async UniTask UpdateScoreRankingAsync(int newScore, CancellationToken ct)
        {
            if (_scoreData == null)
            {
                throw new NullReferenceException("_scoreData is null. Ensure InitializeAsync was called before updating scores.");
            }

            try
            {
                _scoreData.Data.Rankings.Daily.Scores = _scoreRankingService.UpdateTopScores(
                    _scoreData.Data.Rankings.Daily.Scores, newScore, 7);
                _scoreData.Data.Rankings.Monthly.Scores = _scoreRankingService.UpdateTopScores(
                    _scoreData.Data.Rankings.Monthly.Scores, newScore, 7);
                _scoreData.Data.Rankings.AllTime.Scores = _scoreRankingService.UpdateTopScores(
                    _scoreData.Data.Rankings.AllTime.Scores, newScore, 7);

                if (_scoreRankingService.IsNewBestScore(_bestScore.Value, newScore))
                {
                    _bestScore.Value = newScore;
                }

                await SaveScoresAsync(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating score rankings.", ex);
            }
        }

        public async UniTask UpdateUserNameAsync(string userName, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentException("User name cannot be null or empty.", nameof(userName));

                if (_scoreData == null)
                    throw new NullReferenceException("_scoreData is null. Ensure InitializeAsync was called before updating user name.");

                _scoreData.Data.Score.UserName = userName;
                await SaveScoresAsync(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while updating the user name.", ex);
            }
        }

        private async UniTask SaveScoresAsync(CancellationToken ct)
        {
            try
            {
                if (_scoreData == null)
                {
                    throw new ApplicationException("Cannot save null score data.");
                }

                _scoreData.Data.Score.Best = _bestScore.Value;
                _scoreData.Data.Score.LastPlayedDate = DateTime.Today.ToString("yyyy-MM-dd");

                await _scoreRepository.SaveScoresAsync(_scoreData, ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while saving scores.", ex);
            }
        }

        public ScoreContainer GetScoreData()
            => _scoreData;
    }
}
