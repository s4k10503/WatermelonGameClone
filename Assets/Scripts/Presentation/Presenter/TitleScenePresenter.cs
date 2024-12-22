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
        private readonly ILicenseUseCase _licenseUseCase;
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        // View
        private readonly TitleSceneView _titleSceneView;
        private readonly ReactiveProperty<ViewState> _currentViewState
            = new ReactiveProperty<ViewState>(ViewState.Title);
        private readonly Dictionary<ViewState, ITitleSceneViewStateHandler> _viewStateHandlers;

        private const string MainSceneName = "MainScene";
        private const int _maxRetries = 3;
        ScoreContainer _scoreContainer;
        TitleSceneViewStateData _titleSceneViewStateData;
        IReadOnlyList<License> _licenses;

        private readonly CompositeDisposable _disposables;
        private readonly CancellationTokenSource _cts;

        [Inject]
        public TitleScenePresenter(
            TitleSceneView gameView,
            IScoreUseCase scoreUseCase,
            ISoundUseCase soundUseCase,
            ISceneLoaderUseCase sceneLoaderUseCase,
            IGameStateUseCase gameStateUseCase,
            ILicenseUseCase licenseUseCase,
            IExceptionHandlingUseCase exceptionHandlingUseCase)
        {
            _titleSceneView = gameView ?? throw new ArgumentNullException(nameof(gameView));
            _scoreUseCase = scoreUseCase ?? throw new ArgumentNullException(nameof(scoreUseCase));
            _soundUseCase = soundUseCase ?? throw new ArgumentNullException(nameof(soundUseCase));
            _sceneLoaderUseCase = sceneLoaderUseCase ?? throw new ArgumentNullException(nameof(sceneLoaderUseCase));
            _gameStateUseCase = gameStateUseCase ?? throw new ArgumentNullException(nameof(gameStateUseCase));
            _licenseUseCase = licenseUseCase ?? throw new ArgumentNullException(nameof(licenseUseCase));
            _exceptionHandlingUseCase = exceptionHandlingUseCase ?? throw new ArgumentNullException(nameof(exceptionHandlingUseCase));

            _viewStateHandlers = new Dictionary<ViewState, ITitleSceneViewStateHandler>
            {
                { ViewState.Title, new TitleViewStateHandler() },
                { ViewState.DetailedScore, new TitleSceneDetailedScoreViewStateHandler() },
                { ViewState.Settings, new SettingsViewStateHandler() },
                { ViewState.License, new LicenseViewStateHandler() },
                { ViewState.Loading, new TitleSceneLoadingViewStateHandler() },
            };

            _disposables = new CompositeDisposable();
            _cts = new CancellationTokenSource();
        }

        public void Initialize()
        {
            _exceptionHandlingUseCase
                .SafeExecuteAsync(
                    () => _exceptionHandlingUseCase.RetryAsync(
                        () => InitializeAsync(_cts.Token), _maxRetries, _cts.Token), _cts.Token).Forget();
            SetupSubscriptions();

            // Set the global state to Title and initialize the scene state
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Title);
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _disposables?.Dispose();
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            try
            {
                await _scoreUseCase.InitializeAsync(ct);
                _scoreContainer = _scoreUseCase.GetScoreData();

                _licenses = await _licenseUseCase.GetLicensesAsync(ct);

                _titleSceneViewStateData = new TitleSceneViewStateData(_scoreContainer, _licenses);

                _titleSceneView.SettingsPanelView.SetBgmSliderValue(_soundUseCase.VolumeBgm.Value);
                _titleSceneView.SettingsPanelView.SetSeSliderValue(_soundUseCase.VolumeSe.Value);

                UpdateScoreDisplay();
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

        private void SetupSubscriptions()
        {
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            // Update the UI when the state is changed
            _currentViewState
                .DistinctUntilChanged()
                .Subscribe(state => _exceptionHandlingUseCase.SafeExecute(() => UpdateViewStateUI(state, _cts.Token)))
                .AddTo(_disposables);

            // TitlePanel
            _titleSceneView.GameStartRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecuteAsync(() => HandleGameStart(), _cts.Token).Forget())
                .AddTo(_disposables);

            _titleSceneView.MyScoreRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleDisplayScores()))
                .AddTo(_disposables);

            _titleSceneView.TitlePanellView.OnSettings
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleDisplaySettings()))
                .AddTo(_disposables);

            _titleSceneView.TitlePanellView.OnLicense
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => OpenLicenseModal()))
                .AddTo(_disposables);

            // DetailedScoreRankPanel
            _titleSceneView.DetailedScoreRankView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => HandleBackToTitlePanel()))
                .AddTo(_disposables);

            // LicenseModal
            _titleSceneView.LicenseModalView.OnBack
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

        private void UpdateViewStateUI(ViewState state, CancellationToken ct)
        {
            if (_viewStateHandlers.TryGetValue(state, out var handler))
            {
                handler.ApplyAsync(_titleSceneView, _titleSceneViewStateData, ct);
            }
        }

        private async UniTask HandleGameStart()
        {
            try
            {
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
                _titleSceneView.HideTitlePage();
                _titleSceneView.ShowLoadingPage();
                await _sceneLoaderUseCase.LoadSceneAsync(MainSceneName, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while starting the game.", ex);
            }
        }

        private void HandleBackToTitlePanel()
        {
            HandleSaveVolume().Forget();
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
            _currentViewState.Value = ViewState.Title;
        }

        private void HandleDisplayScores()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.DisplayingScores);
            _currentViewState.Value = ViewState.DetailedScore;
        }

        private void HandleDisplaySettings()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Settings);
            _currentViewState.Value = ViewState.Settings;
        }

        private void HandleSetBgmVolume(float value)
        {
            _soundUseCase.SetBGMVolume(value);
        }

        private void HandleSetSeVolume(float value)
        {
            _soundUseCase.SetSEVolume(value);
        }

        private async UniTask HandleSaveVolume()
        {
            try
            {
                await _soundUseCase.SaveVolume(_cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while saving volume settings.", ex);
            }
        }

        private void UpdateScoreDisplay()
        {
            var scoreData = _scoreUseCase.GetScoreData();
            _titleSceneView.DetailedScoreRankView.DisplayTopScores(scoreData);
        }

        private void OpenLicenseModal()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.License);
            _currentViewState.Value = ViewState.License;
        }
    }
}
