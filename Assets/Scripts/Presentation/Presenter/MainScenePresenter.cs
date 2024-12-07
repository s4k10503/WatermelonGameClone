using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;

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

        private bool _isNext;
        private float _mergeItemCreateDelayTime;
        private GlobalGameState _previousGlobalState;
        private RenderTexture _screenshotCache;

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

            _cts = new CancellationTokenSource();
            _disposables = new CompositeDisposable();

            _mainSceneView.HideLoadingPage();
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
            _mainSceneView.RestartRequested
                .Subscribe(_ => HandleSceneChangeAsync(SceneManager.GetActiveScene().name, ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToTitleRequested
                .Subscribe(_ => HandleSceneChangeAsync("TitleScene", ct).Forget())
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
            ShowGameOverUI();
        }

        private async UniTask HandleSceneChangeAsync(string sceneName, CancellationToken ct)
        {
            ShowLoadingScreen();
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
            await _sceneLoaderUseCase.LoadSceneAsync(sceneName, ct);
        }

        private void ResumeGame(Unit _)
        {
            HidePauseScreen();
            _gameStateUseCase.SetGlobalGameState(_previousGlobalState);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
        }

        private void PauseGame(Unit _)
        {
            _previousGlobalState = _gameStateUseCase.GlobalState.Value;
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Paused);
            ShowPauseScreen();
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
        }

        private void ShowDetailedScore(Unit _)
        {
            _mainSceneView.Stageview.HideStage();
            _mainSceneView.HideMainPageMainElements();
            _mainSceneView.GameOverPanelView.HidePanel();
            _mainSceneView.DetailedScoreRankView.ShowPanel();
        }

        private void ReturnToGameOverScreen(Unit _)
        {
            _mainSceneView.DetailedScoreRankView.HidePanel();
            _mainSceneView.Stageview.ShowStage();
            _mainSceneView.ShowMainPageMainElements();
            _mainSceneView.GameOverPanelView.ShowPanelWihtoutData();
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

        private void ShowLoadingScreen()
        {
            _mainSceneView.ShowLoadingPage();
            _mainSceneView.BackgroundPanelView.HidePanel();
            _mainSceneView.PausePanelView.HidePanel();
        }

        private void ShowPauseScreen()
        {
            _mainSceneView.BackgroundPanelView.ShowPanel();
            _mainSceneView.PausePanelView.ShowPanel();
        }

        private void HidePauseScreen()
        {
            _mainSceneView.BackgroundPanelView.HidePanel();
            _mainSceneView.PausePanelView.HidePanel();
        }

        private void ShowGameOverUI()
        {
            _mainSceneView.BackgroundPanelView.ShowPanel();
            _mainSceneView.GameOverPanelView.ShowPanel(
            _scoreUseCase.CurrentScore.Value, _screenshotCache, _scoreUseCase.GetScoreData());
        }
    }
}
