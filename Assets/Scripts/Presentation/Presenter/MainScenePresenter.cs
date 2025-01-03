using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using System.Collections.Generic;

namespace WatermelonGameClone.Presentation
{
    public sealed class MainScenePresenter : IInitializable, ITickable, IDisposable
    {
        // Model
        private readonly IMergeItemUseCase _mergeItemUseCase;
        private readonly IScoreUseCase _scoreUseCase;
        private readonly ISoundUseCase _soundUseCase;
        private readonly IGameStateUseCase _gameStateUseCase;
        private readonly ISceneLoaderUseCase _sceneLoaderUseCase;
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        // View
        private readonly MainSceneView _mainSceneView;
        private readonly ReactiveProperty<ViewState> _currentViewState
            = new ReactiveProperty<ViewState>(ViewState.Loading);
        private readonly Dictionary<ViewState, IMainSceneViewStateHandler> _viewStateHandlers;

        private bool _isNext;
        private float _mergeItemCreateDelayTime;
        private GlobalGameState _previousGlobalState;
        private RenderTexture _screenshotCache;
        private const string TitleSceneName = "TitleScene";
        private const int _maxRetries = 3;

        private readonly CompositeDisposable _disposables;
        private readonly CancellationTokenSource _cts;

        [Inject]
        public MainScenePresenter(
            MainSceneView mainSceneView,
            IMergeItemUseCase mergeItemUseCase,
            IScoreUseCase scoreUseCase,
            ISoundUseCase soundUseCase,
            IGameStateUseCase gameStateUseCase,
            ISceneLoaderUseCase sceneLoaderUseCase,
            IExceptionHandlingUseCase exceptionHandlingUseCase)
        {
            _mainSceneView = mainSceneView ?? throw new ArgumentNullException(nameof(mainSceneView));
            _mergeItemUseCase = mergeItemUseCase ?? throw new ArgumentNullException(nameof(mergeItemUseCase));
            _scoreUseCase = scoreUseCase ?? throw new ArgumentNullException(nameof(scoreUseCase));
            _soundUseCase = soundUseCase ?? throw new ArgumentNullException(nameof(soundUseCase));
            _gameStateUseCase = gameStateUseCase ?? throw new ArgumentNullException(nameof(gameStateUseCase));
            _sceneLoaderUseCase = sceneLoaderUseCase ?? throw new ArgumentNullException(nameof(sceneLoaderUseCase));
            _exceptionHandlingUseCase = exceptionHandlingUseCase ?? throw new ArgumentNullException(nameof(exceptionHandlingUseCase));

            _isNext = true;
            _mergeItemCreateDelayTime = 0f;

            _viewStateHandlers = new Dictionary<ViewState, IMainSceneViewStateHandler>
            {
                { ViewState.Loading, new MainSceneLoadingViewStateHandler() },
                { ViewState.Playing, new PlayingViewStateHandler() },
                { ViewState.Paused, new PausedViewStateHandler() },
                { ViewState.GameOver, new GameOverViewStateHandler() },
                { ViewState.DetailedScore, new MainSceneDetailedScoreViewStateHandler() },
            };

            _cts = new CancellationTokenSource();
            _disposables = new CompositeDisposable();
        }

        public void Initialize()
        {
            _exceptionHandlingUseCase.SafeExecuteAsync(
                () => _exceptionHandlingUseCase.RetryAsync(
                    () => InitializeAsync(_cts.Token), _maxRetries, _cts.Token), _cts.Token).Forget();
            SetupSubscriptions(_cts.Token);
        }

        public void Tick()
        {
            if (_cts == null || _cts.IsCancellationRequested)
            {
                return;
            }

            if (CanCreateNextItem())
            {
                _exceptionHandlingUseCase.SafeExecuteAsync(
                    () => _exceptionHandlingUseCase.RetryAsync(
                        () => CreateNextItemAsync(), _maxRetries, _cts.Token), _cts.Token).Forget();
            }
        }

        public void Dispose()
        {
            if (_screenshotCache != null)
            {
                RenderTexture.ReleaseTemporary(_screenshotCache);
                _screenshotCache = null;
            }

            _cts?.Cancel();
            _cts?.Dispose();
            _disposables?.Dispose();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                await _scoreUseCase.InitializeAsync(ct);
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
                _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Initializing);
                UpdateScoreDisplays();
                _currentViewState.Value = ViewState.Playing;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during initialization.", ex);
            }
        }

        private void SetupSubscriptions(CancellationToken ct)
        {
            SubscribeToModelUpdates();
            SubscribeToViewEvents(ct);
        }

        private void SubscribeToModelUpdates()
        {
            _mergeItemUseCase.NextItemIndex
                .Subscribe(
                    index => _exceptionHandlingUseCase
                        .SafeExecute(() => _mainSceneView.NextItemPanelView.UpdateNextItemImages(index)))
                .AddTo(_disposables);

            _scoreUseCase.CurrentScore
                .Subscribe(
                    score => _exceptionHandlingUseCase
                        .SafeExecute(() => UpdateCurrentScoreDisplays(score)))
                .AddTo(_disposables);

            _scoreUseCase.BestScore
                .Subscribe(
                    score => _exceptionHandlingUseCase
                        .SafeExecute(() => _mainSceneView.ScorePanelView.UpdateBestScore(score)))
                .AddTo(_disposables);
        }

        private void SubscribeToViewEvents(CancellationToken ct)
        {
            _currentViewState
                .DistinctUntilChanged()
                .Subscribe(state => _exceptionHandlingUseCase
                    .SafeExecute(() => UpdateViewStateUI(state)))
                .AddTo(_disposables);

            _mainSceneView.RestartRequested
                .Subscribe(_ => _exceptionHandlingUseCase
                    .SafeExecuteAsync(() => HandleSceneChangeAsync(SceneManager.GetActiveScene().name, ct), ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToTitleRequested
                .Subscribe(_ => _exceptionHandlingUseCase
                    .SafeExecuteAsync(() => HandleSceneChangeAsync(TitleSceneName, ct), ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToGameRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => ResumeGame()))
                .AddTo(_disposables);

            _mainSceneView.PauseRequested
                .Where(_ => _gameStateUseCase.GlobalState.Value != GlobalGameState.GameOver &&
                            _gameStateUseCase.GlobalState.Value != GlobalGameState.Paused)
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => PauseGame()))
                .AddTo(_disposables);

            _mainSceneView.DisplayScoreRequested
                .Subscribe(_ => ShowDetailedScore())
                .AddTo(_disposables);

            _mainSceneView.DetailedScoreRankPageView.OnBack
                .Subscribe(_ => ReturnToGameOverScreen())
                .AddTo(_disposables);

            _mainSceneView.MergeItemManager.OnItemCreated
                .Subscribe(mergeItem =>
                {
                    mergeItem.OnContactTimeUpdated
                        .Subscribe(data => _exceptionHandlingUseCase.SafeExecute(() => HandleContactTimeUpdated(data.id, data.deltaTime)))
                        .AddTo(_disposables);

                    mergeItem.OnContactExited
                        .Subscribe(data => _exceptionHandlingUseCase.SafeExecute(() => HandleContactExited(data)))
                        .AddTo(_disposables);

                    mergeItem.OnDropping
                        .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleItemDropping()))
                        .AddTo(_disposables);

                    mergeItem.OnMergeRequest
                        .Subscribe(request => _exceptionHandlingUseCase.SafeExecute(() => HandleMergeRequest(request.Source, request.Target)))
                        .AddTo(_disposables);
                }).AddTo(_disposables);
        }

        private void UpdateViewStateUI(ViewState state)
        {
            var data = new MainSceneViewStateData(
                _scoreUseCase.CurrentScore.Value,
                _scoreUseCase.BestScore.Value,
                _scoreUseCase.GetScoreData(),
                _screenshotCache
            );

            if (_viewStateHandlers.TryGetValue(state, out var handler))
            {
                handler.Apply(_mainSceneView, data);
            }
        }

        private void UpdateCurrentScoreDisplays(int score)
        {
            _mainSceneView.ScorePanelView.UpdateCurrentScore(score);
            _mainSceneView.ScoreRankView.DisplayCurrentScore(score);
        }

        private void HandleContactTimeUpdated(Guid id, float deltaTime)
        {
            var entity = _mergeItemUseCase.GetEntityById(id);
            if (entity != null)
            {
                _mergeItemUseCase.AddContactTime(id, deltaTime);

                if (_mergeItemUseCase.CheckGameOver(entity.ContactTime))
                {
                    HandleGameOverAsync(entity.ContactTime).Forget();
                }
            }
        }

        private void HandleContactExited(Guid id)
        {
            _mergeItemUseCase.ResetContactTime(id);
        }

        private bool CanCreateNextItem()
        {
            return _isNext && _gameStateUseCase.GlobalState.Value != GlobalGameState.GameOver;
        }

        private async UniTask CreateNextItemAsync()
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _isNext = false;

                // Generate entity from use case
                var entity = _mergeItemUseCase.CreateMergeItemEntity(_mergeItemUseCase.NextItemIndex.Value);
                var id = entity.Id;

                await _mainSceneView.MergeItemManager.CreateItemAsync(
                    id,
                    _mergeItemUseCase.NextItemIndex.Value,
                    _mergeItemCreateDelayTime, _cts.Token);

                _mergeItemUseCase.UpdateNextItemIndex();
                _mergeItemCreateDelayTime = 1.0f;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the next item.", ex);
            }
        }

        private void HandleItemDropping()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.ItemDropping);
            _soundUseCase.PlaySoundEffect(SoundEffect.Drop);
            _isNext = true;
        }

        private void HandleMergeRequest(IMergeItemView source, IMergeItemView target)
        {
            if (_mergeItemUseCase.CanMerge(source.ItemNo, target.ItemNo))
            {
                var mergeData = _mergeItemUseCase.CreateMergeData(
                    source.GameObject.transform.position,
                    target.GameObject.transform.position,
                    source.ItemNo);

                // Generate a new entity in the use case
                var newEntity = _mergeItemUseCase.CreateMergeItemEntity(mergeData.ItemNo);
                var newId = newEntity.Id;

                HandleItemMerging(newId, mergeData, source, target);
            }
        }

        private void HandleItemMerging(
            Guid id,
            MergeData mergeData,
            IMergeItemView itemA,
            IMergeItemView itemB)
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Merging);

            _mainSceneView.MergeItemManager.DestroyItem(itemA.GameObject);
            _mainSceneView.MergeItemManager.DestroyItem(itemB.GameObject);

            _mainSceneView.MergeItemManager.MergeItem(id, mergeData.Position, mergeData.ItemNo);

            _soundUseCase.PlaySoundEffect(SoundEffect.Merge);
            _scoreUseCase.UpdateCurrentScore(mergeData.ItemNo);
        }

        private async UniTask HandleGameOverAsync(float contactTime)
        {
            if (!_mergeItemUseCase.CheckGameOver(contactTime)) return;

            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.GameOver);
                await _scoreUseCase.UpdateScoreRankingAsync(_scoreUseCase.CurrentScore.Value, _cts.Token);
                UpdateScoreDisplays();
                AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
                _screenshotCache = await _mainSceneView.ScreenshotHandler.CaptureScreenshotAsync(_cts.Token);

                _currentViewState.Value = ViewState.GameOver;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred during game over handling.", ex);
            }
        }

        private async UniTask HandleSceneChangeAsync(string sceneName, CancellationToken ct)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _currentViewState.Value = ViewState.Loading;
                AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
                await _sceneLoaderUseCase.LoadSceneAsync(sceneName, ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while changing the scene to {sceneName}.", ex);
            }
        }

        private void ResumeGame()
        {
            _gameStateUseCase.SetGlobalGameState(_previousGlobalState);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
            _currentViewState.Value = ViewState.Playing;
        }

        private void PauseGame()
        {
            _previousGlobalState = _gameStateUseCase.GlobalState.Value;
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Paused);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
            _currentViewState.Value = ViewState.Paused;
        }

        private void ShowDetailedScore()
        {
            _currentViewState.Value = ViewState.DetailedScore;
        }

        private void ReturnToGameOverScreen()
        {
            _currentViewState.Value = ViewState.GameOver;
        }

        private void UpdateScoreDisplays()
        {
            _mainSceneView.ScorePanelView.UpdateBestScore(_scoreUseCase.BestScore.Value);
            var scoreData = _scoreUseCase.GetScoreData();
            _mainSceneView.ScoreRankView.DisplayTopScores(scoreData);
            _mainSceneView.DetailedScoreRankPageView.DisplayTopScores(scoreData);
        }

        private void AdjustTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}
