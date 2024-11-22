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

        [Inject]
        public MainScenePresenter(
            MainSceneView mainSceneView,
            IMergeItemUseCase mergeItemUseCase,
            IScoreUseCase scoreUseCase,
            ISoundUseCase soundUseCase,
            IGameStateUseCase gameStateUseCase,
            ISceneLoaderUseCase sceneLoaderUseCase)
        {
            _mainSceneView = mainSceneView;
            _mergeItemUseCase = mergeItemUseCase;
            _scoreUseCase = scoreUseCase;
            _soundUseCase = soundUseCase;
            _gameStateUseCase = gameStateUseCase;
            _sceneLoaderUseCase = sceneLoaderUseCase;

            _isNext = true;
            _mergeItemCreateDelayTime = 0f;

            _disposables = new CompositeDisposable();

            _mainSceneView.HideLoadingPage();
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
            SetupSubscriptions();
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

            _disposables.Dispose();
        }

        private async UniTask InitializeAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                await _scoreUseCase.InitializeAsync(cts.Token);
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
                _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Initializing);
                _mergeItemUseCase.UpdateNextItemIndex();
                UpdateScoreDisplays();
                cts.Cancel();
            }
        }

        private void SetupSubscriptions()
        {
            SubscribeToModelUpdates();
            SubscribeToViewEvents();
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

        private void SubscribeToViewEvents()
        {
            _mainSceneView.RestartRequested
                .Subscribe(_ => HandleSceneChangeAsync(SceneManager.GetActiveScene().name).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToTitleRequested
                .Subscribe(_ => HandleSceneChangeAsync("TitleScene").Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToGameRequested
                .Subscribe(_ => ResumeGame())
                .AddTo(_disposables);

            _mainSceneView.PauseRequested
                .Where(_ => _gameStateUseCase.GlobalState.Value != GlobalGameState.GameOver &&
                            _gameStateUseCase.GlobalState.Value != GlobalGameState.Paused)
                .Subscribe(_ => PauseGame())
                .AddTo(_disposables);

            _mainSceneView.DisplayScoreRequested
                .Subscribe(_ => ShowDetailedScore())
                .AddTo(_disposables);

            _mainSceneView.DetailedScoreRankView.OnBack
                .Subscribe(_ => ReturnToGameOverScreen())
                .AddTo(_disposables);

            _mainSceneView.MergeItemManager.OnItemCreated
                .Subscribe(mergeItem =>
                {
                    mergeItem.OnDropping
                        .Subscribe(_ => HandleItemDropping())
                        .AddTo(_disposables);

                    mergeItem.OnMerging
                        .Subscribe(HandleItemMerging)
                        .AddTo(_disposables);

                    mergeItem.OnGameOver
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
            using (var cts = new CancellationTokenSource())
            {
                _isNext = false;
                await _mainSceneView.MergeItemManager.CreateItem(
                    _mergeItemUseCase.NextItemIndex.Value, _mergeItemCreateDelayTime, cts.Token);
                _mergeItemUseCase.UpdateNextItemIndex();
                _mergeItemCreateDelayTime = 1.0f;
                cts.Cancel();
            }
        }

        private void HandleItemDropping()
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
            using (var cts = new CancellationTokenSource())
            {
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.GameOver);
                await _scoreUseCase.UpdateScoreRankingAsync(_scoreUseCase.CurrentScore.Value, cts.Token);
                UpdateScoreDisplays();
                AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
                _screenshotCache = await _mainSceneView.ScreenshotHandler.CaptureScreenshotAsync(cts.Token);
                ShowGameOverUI();
                cts.Cancel();
            }
        }

        private async UniTask HandleSceneChangeAsync(string sceneName)
        {
            using (var cts = new CancellationTokenSource())
            {
                ShowLoadingScreen();
                AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
                await _sceneLoaderUseCase.LoadSceneAsync(sceneName, cts.Token);
                cts.Cancel();
            }
        }

        private void ResumeGame()
        {
            HidePauseScreen();
            _gameStateUseCase.SetGlobalGameState(_previousGlobalState);
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameStart);
        }

        private void PauseGame()
        {
            _previousGlobalState = _gameStateUseCase.GlobalState.Value;
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Paused);
            ShowPauseScreen();
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
        }

        private void ShowDetailedScore()
        {
            _mainSceneView.Stageview.HideStage();
            _mainSceneView.HideMainPageMainElements();
            _mainSceneView.GameOverPanelView.HidePanel();
            _mainSceneView.DetailedScoreRankView.ShowPanel();
        }

        private void ReturnToGameOverScreen()
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
