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
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

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
            IGameStateUseCase gameStateUseCase,
            IExceptionHandlingUseCase exceptionHandlingUseCase)
        {
            _titleSceneView = gameView ?? throw new ArgumentNullException(nameof(gameView));
            _scoreUseCase = scoreUseCase ?? throw new ArgumentNullException(nameof(scoreUseCase));
            _soundUseCase = soundUseCase ?? throw new ArgumentNullException(nameof(soundUseCase));
            _sceneLoaderUseCase = sceneLoaderUseCase ?? throw new ArgumentNullException(nameof(sceneLoaderUseCase));
            _gameStateUseCase = gameStateUseCase ?? throw new ArgumentNullException(nameof(gameStateUseCase));
            _exceptionHandlingUseCase = exceptionHandlingUseCase ?? throw new ArgumentNullException(nameof(exceptionHandlingUseCase));

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
            _exceptionHandlingUseCase.SafeExecuteAsync(
                () => _exceptionHandlingUseCase.RetryAsync(() => InitializeAsync(_cts.Token), 3, _cts.Token)).Forget();
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

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            await _exceptionHandlingUseCase.SafeExecuteAsync(async () =>
            {
                await _scoreUseCase.InitializeAsync(ct);

                _titleSceneView.SettingsPanelView.SetBgmSliderValue(_soundUseCase.VolumeBgm.Value);
                _titleSceneView.SettingsPanelView.SetSeSliderValue(_soundUseCase.VolumeSe.Value);

                UpdateScoreDisplay();
            });
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
                .Subscribe(state => _exceptionHandlingUseCase.SafeExecute(() => UpdateViewStateUI(state)))
                .AddTo(_disposables);

            // TitlePanel
            _titleSceneView.GameStartRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecuteAsync(() => HandleGameStart()).Forget())
                .AddTo(_disposables);

            _titleSceneView.MyScoreRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleDisplayScores()))
                .AddTo(_disposables);

            _titleSceneView.TitlePanellView.OnSettings
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleDisplaySettings()))
                .AddTo(_disposables);

            // DetailedScoreRankPanel
            _titleSceneView.DetailedScoreRankView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleBackToTitlePanel()))
                .AddTo(_disposables);

            // SettingsPanel
            _titleSceneView.SettingsPanelView.ValueBgm
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => _exceptionHandlingUseCase.SafeExecute(() => HandleSetBgmVolume(value)))
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.ValueSe
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => _exceptionHandlingUseCase.SafeExecute(() => HandleSetSeVolume(value)))
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleBackToTitlePanel()))
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
            await _exceptionHandlingUseCase.SafeExecuteAsync(async () =>
            {
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
                _titleSceneView.HideTitlePage();
                _titleSceneView.ShowLoadingPage();
                await _sceneLoaderUseCase.LoadSceneAsync(MainSceneName, _cts.Token);
            });
        }

        private void HandleBackToTitlePanel()
        {
            _exceptionHandlingUseCase.SafeExecute(() =>
            {
                HandleSaveVolume();
                _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
                _currentViewState.Value = ViewState.Title;
            });
        }

        private void HandleDisplayScores()
        {
            _exceptionHandlingUseCase.SafeExecute(() =>
            {
                _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.DisplayingScores);
                _currentViewState.Value = ViewState.DetailedScore;
            });
        }

        private void HandleDisplaySettings()
        {
            _exceptionHandlingUseCase.SafeExecute(() =>
            {
                _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Settings);
                _currentViewState.Value = ViewState.Settings;
            });
        }

        private void HandleSetBgmVolume(float value)
            => _exceptionHandlingUseCase.SafeExecute(() => _soundUseCase.SetBGMVolume(value));

        private void HandleSetSeVolume(float value)
            => _exceptionHandlingUseCase.SafeExecute(() => _soundUseCase.SetSEVolume(value));

        private void HandleSaveVolume()
            => _exceptionHandlingUseCase.SafeExecuteAsync(() => _soundUseCase.SaveVolume(_cts.Token)).Forget();

        private void UpdateScoreDisplay()
        {
            _exceptionHandlingUseCase.SafeExecute(() =>
            {
                var scoreData = _scoreUseCase.GetScoreData();
                _titleSceneView.DetailedScoreRankView.DisplayTopScores(scoreData);
            });
        }
    }
}
