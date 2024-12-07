using UniRx;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using System.Collections.Generic;

namespace WatermelonGameClone.Presentation
{
    public sealed class TitleScenePresenter : IInitializable, IDisposable
    {
        // Model
        private readonly IScoreUseCase _scoreUseCase;
        private readonly ISoundUseCase _soundUseCase;
        private readonly ISceneLoaderUseCase _sceneLoaderUseCase;
        private readonly IGameStateUseCase _gameStateUseCase;

        // View
        private readonly TitleSceneView _titleSceneView;
        private readonly ReactiveProperty<ViewState> _currentViewState
            = new ReactiveProperty<ViewState>(ViewState.Title);
        private readonly Dictionary<ViewState, ITitleSceneViewStateHandler> _viewStateHandlers;

        private const string MainSceneName = "MainScene";

        private readonly CompositeDisposable _disposables;
        private readonly CancellationTokenSource _cts;


        [Inject]
        public TitleScenePresenter(
            TitleSceneView gameView,
            IScoreUseCase scoreUseCase,
            ISoundUseCase soundUseCase,
            ISceneLoaderUseCase sceneLoaderUseCase,
            IGameStateUseCase gameStateUseCase)
        {
            _titleSceneView = gameView;
            _scoreUseCase = scoreUseCase;
            _soundUseCase = soundUseCase;
            _sceneLoaderUseCase = sceneLoaderUseCase;
            _gameStateUseCase = gameStateUseCase;

            _viewStateHandlers = new Dictionary<ViewState, ITitleSceneViewStateHandler>
            {
                { ViewState.Title, new TitleViewStateHandler() },
                { ViewState.DetailedScore, new TitleSceneDetailedScoreViewStateHandler() },
                { ViewState.Settings, new SettingsViewStateHandler() },
                { ViewState.Loading, new TitleSceneLoadingViewStateHandler() },
            };

            _disposables = new CompositeDisposable();
            _cts = new CancellationTokenSource();
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
            SetupSubscriptions();

            // Set the global state to Title and initialize the scene state
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Title);
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private async UniTask InitializeAsync()
        {
            await _scoreUseCase
                .InitializeAsync(_cts.Token);

            _titleSceneView.SettingsPanelView
                .SetBgmSliderValue(_soundUseCase.VolumeBgm.Value);
            _titleSceneView.SettingsPanelView
                .SetSeSliderValue(_soundUseCase.VolumeSe.Value);

            UpdateScoreDisplay();
        }

        private void SetupSubscriptions()
        {
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // Update the UI when the state is changed
            _currentViewState
                .DistinctUntilChanged()
                .Subscribe(UpdateViewStateUI)
                .AddTo(_disposables);

            // TitlePanel
            _titleSceneView.GameStartRequested
                .Subscribe(_ => HandleGameStart().Forget())
                .AddTo(_disposables);

            _titleSceneView.MyScoreRequested
                .Subscribe(HandleDisplayScores)
                .AddTo(_disposables);

            _titleSceneView.TitlePanellView.OnSettings
                .Subscribe(HandleDisplaySettings)
                .AddTo(_disposables);

            // DetailedScoreRankPanel
            _titleSceneView.DetailedScoreRankView.OnBack
                .Subscribe(HandleBackToTitlePanel)
                .AddTo(_disposables);

            // SettingsPanel
            _titleSceneView.SettingsPanelView.ValueBgm
                .SkipLatestValueOnSubscribe()
                .Subscribe(HandleSetBgmVolume)
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.ValueSe
                .SkipLatestValueOnSubscribe()
                .Subscribe(HandleSetSeVolume)
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.OnBack
                .Subscribe(HandleBackToTitlePanel)
                .AddTo(_disposables);
        }

        private void UpdateViewStateUI(ViewState state)
        {
            var data = new TitleSceneViewStateData(_scoreUseCase.GetScoreData());

            if (_viewStateHandlers.TryGetValue(state, out var handler))
            {
                handler.Apply(_titleSceneView, data);
            }
        }

        private async UniTask HandleGameStart()
        {
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
            _titleSceneView.HideTitlePage();
            _titleSceneView.ShowLoadingPage();
            await _sceneLoaderUseCase.LoadSceneAsync(MainSceneName, _cts.Token);
        }

        private void HandleBackToTitlePanel(Unit _)
        {
            HandleSaveVolume();
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
            _currentViewState.Value = ViewState.Title;
        }

        private void HandleDisplayScores(Unit _)
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.DisplayingScores);
            _currentViewState.Value = ViewState.DetailedScore;
        }

        private void HandleDisplaySettings(Unit _)
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Settings);
            _currentViewState.Value = ViewState.Settings;
        }

        private void HandleSetBgmVolume(float value)
            => _soundUseCase.SetBGMVolume(value);

        private void HandleSetSeVolume(float value)
            => _soundUseCase.SetSEVolume(value);

        private void HandleSaveVolume()
            => _soundUseCase.SaveVolume(_cts.Token).Forget();

        private void UpdateScoreDisplay()
        {
            var scoreData = _scoreUseCase.GetScoreData();
            _titleSceneView.DetailedScoreRankView.DisplayTopScores(scoreData);
        }
    }
}
