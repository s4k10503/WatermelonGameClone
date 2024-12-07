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

        private readonly CompositeDisposable _disposables;
        private readonly CancellationTokenSource _cts;

        [Inject]
        public MainScenePresenter(
            MainSceneView mainSceneView,
            IMergeItemUseCase megeItemUseCase,
            IScoreUseCase scoreUseCase,
            ISoundUseCase soundUseCase,
            IGameStateUseCase gameStateUseCase,
            ISceneLoaderUseCase sceneLoaderUseCase)
        {
            _mainSceneView = mainSceneView;
            _mergeItemUseCase = megeItemUseCase;
            _scoreUseCase = scoreUseCase;
            _soundUseCase = soundUseCase;
            _gameStateUseCase = gameStateUseCase;
            _sceneLoaderUseCase = sceneLoaderUseCase;

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
            InitializeAsync(_cts.Token).Forget();
            SetupSubscriptions(_cts.Token);
        }

        public void Tick()
        {
            if (CanCreateNextItem())
            {
                CreateNextItemAsync().Forget();
            }
        }

        public void Dispose()
        {
            if (_screenshotCache != null)
            {
                RenderTexture.ReleaseTemporary(_screenshotCache);
                _screenshotCache = null;
            }

            _disposables?.Dispose();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            await _scoreUseCase.InitializeAsync(ct);
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Initializing);
            _mergeItemUseCase.UpdateNextItemIndex();
            UpdateScoreDisplays();

            _currentViewState.Value = ViewState.Playing;
        }

        private void SetupSubscriptions(CancellationToken ct)
        {
            SubscribeToModelUpdates();
            SubscribeToViewEvents(ct);
        }

        private void SubscribeToModelUpdates()
        {
            _mergeItemUseCase.NextItemIndex
                .Subscribe(index => _mainSceneView.NextItemPanelView.UpdateNextItemImages(index))
                .AddTo(_disposables);

            _scoreUseCase.CurrentScore
                .Subscribe(UpdateCurrentScoreDisplays)
                .AddTo(_disposables);

            _scoreUseCase.BestScore
                .Subscribe(score => _mainSceneView.ScorePanelView.UpdateBestScore(score))
                .AddTo(_disposables);
        }

        private void SubscribeToViewEvents(CancellationToken ct)
        {
            _currentViewState
                .DistinctUntilChanged()
                .Subscribe(UpdateViewStateUI)
                .AddTo(_disposables);

            _mainSceneView.RestartRequested
                .Subscribe(_ => HandleSceneChangeAsync(SceneManager.GetActiveScene().name, ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToTitleRequested
                .Subscribe(_ => HandleSceneChangeAsync(TitleSceneName, ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToGameRequested
                .Subscribe(ResumeGame)
                .AddTo(_disposables);

            _mainSceneView.PauseRequested
                .Where(_ => _gameStateUseCase.GlobalState.Value != GlobalGameState.GameOver &&
                            _gameStateUseCase.GlobalState.Value != GlobalGameState.Paused)
                .Subscribe(PauseGame)
                .AddTo(_disposables);

            _mainSceneView.DisplayScoreRequested
                .Subscribe(ShowDetailedScore)
                .AddTo(_disposables);

            _mainSceneView.DetailedScoreRankView.OnBack
                .Subscribe(ReturnToGameOverScreen)
                .AddTo(_disposables);

            _mainSceneView.MergeItemManager.OnItemCreated
                .Subscribe(megeItem =>
                {
                    megeItem.OnDropping
                        .Subscribe(HandleItemDropping)
                        .AddTo(_disposables);

                    megeItem.OnMerging
                        .Subscribe(HandleItemMerging)
                        .AddTo(_disposables);

                    megeItem.OnGameOver
                        .Subscribe(_ => HandleGameOverAsync().Forget())
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

        private bool CanCreateNextItem()
        {
            return _isNext && _gameStateUseCase.GlobalState.Value != GlobalGameState.GameOver;
        }

        private async UniTask CreateNextItemAsync()
        {
            _isNext = false;
            await _mainSceneView.MergeItemManager.CreateItem(
                _mergeItemUseCase.NextItemIndex.Value, _mergeItemCreateDelayTime, _cts.Token);
            _mergeItemUseCase.UpdateNextItemIndex();
            _mergeItemCreateDelayTime = 1.0f;
        }

        private void HandleItemDropping(Unit _)
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.ItemDropping);
            _soundUseCase.PlaySoundEffect(SoundEffect.Drop);
            _isNext = true;
        }

        private void HandleItemMerging(MergeData mergeData)
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Merging);
            _soundUseCase.PlaySoundEffect(SoundEffect.Merge);
            _scoreUseCase.UpdateCurrentScore(mergeData.ItemNo);
            _mainSceneView.MergeItemManager.MergeItem(mergeData.Position, mergeData.ItemNo);
            _mainSceneView.MergeItemManager.DestroyItem(mergeData.ItemA);
            _mainSceneView.MergeItemManager.DestroyItem(mergeData.ItemB);
        }

        private async UniTask HandleGameOverAsync()
        {
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.GameOver);
            await _scoreUseCase.UpdateScoreRankingAsync(_scoreUseCase.CurrentScore.Value, _cts.Token);
            UpdateScoreDisplays();
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
            _screenshotCache = await _mainSceneView.ScreenshotHandler.CaptureScreenshotAsync(_cts.Token);

            _currentViewState.Value = ViewState.GameOver;
        }

        private async UniTask HandleSceneChangeAsync(string sceneName, CancellationToken ct)
        {
            _currentViewState.Value = ViewState.Loading;
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
            await _sceneLoaderUseCase.LoadSceneAsync(sceneName, ct);
        }

        private void ResumeGame(Unit _)
        {
            _gameStateUseCase.SetGlobalGameState(_previousGlobalState);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);

            _currentViewState.Value = ViewState.Playing;
        }

        private void PauseGame(Unit _)
        {
            _previousGlobalState = _gameStateUseCase.GlobalState.Value;
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Paused);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);

            _currentViewState.Value = ViewState.Paused;
        }

        private void ShowDetailedScore(Unit _)
        {
            _currentViewState.Value = ViewState.DetailedScore;
        }

        private void ReturnToGameOverScreen(Unit _)
        {
            _currentViewState.Value = ViewState.GameOver;
        }

        private void UpdateScoreDisplays()
        {
            _mainSceneView.ScorePanelView.UpdateBestScore(_scoreUseCase.BestScore.Value);
            var scoreData = _scoreUseCase.GetScoreData();
            _mainSceneView.ScoreRankView.DisplayTopScores(scoreData);
            _mainSceneView.DetailedScoreRankView.DisplayTopScores(scoreData);
        }

        private void AdjustTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}
