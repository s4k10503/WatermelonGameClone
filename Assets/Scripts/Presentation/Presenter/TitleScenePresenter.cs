using UseCase.Interfaces;
using Presentation.DTO;
using Presentation.Interfaces;
using Presentation.State.Common;
using Presentation.State.TitleScene;
using Presentation.View.TitleScene;

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace Presentation.Presenter
{
    public sealed class TitleScenePresenter : IInitializable, IDisposable
    {
        // UseCases
        private readonly IScoreUseCase _scoreUseCase;
        private readonly ISoundUseCase _soundUseCase;
        private readonly ISceneLoaderUseCase _sceneLoaderUseCase;
        private readonly IGameStateUseCase _gameStateUseCase;
        private readonly ILicenseUseCase _licenseUseCase;
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        // View
        private readonly TitleSceneView _titleSceneView;
        private readonly ReactiveProperty<PageState> _currentPageState
            = new(PageState.Title);

        private readonly ReactiveProperty<ModalState> _currentModalState
            = new(ModalState.None);

        // Dictionary to manage the page state
        private readonly Dictionary<PageState, IPageStateHandler<TitleSceneView, TitleSceneViewStateData>>
            _pageStateHandlers;

        // Dictionary to manage the modal state (for overlay display)
        private readonly Dictionary<ModalState, IModalStateHandler<TitleSceneView, TitleSceneViewStateData>>
            _modalStateHandlers;

        private const string MainSceneName = "MainScene";
        private const int MaxRetries = 3;

        TitleSceneViewStateData _titleSceneViewStateData;

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

            // Page State
            _pageStateHandlers
                = new Dictionary<PageState, IPageStateHandler<TitleSceneView, TitleSceneViewStateData>>
            {
                { PageState.Title, new TitleStateHandler() },
                { PageState.DetailedScore, new TitleDetailedScoreStateHandler() },
                { PageState.Settings, new SettingsStateHandler() },
                { PageState.Loading, new TitleLoadingStateHandler() },
            };

            // Modal state
            _modalStateHandlers
                = new Dictionary<ModalState, IModalStateHandler<TitleSceneView, TitleSceneViewStateData>>
            {
                { ModalState.None, new TitleNoneStateHandler() },
                { ModalState.UserNameInput, new UserNameInputStateHandler() },
                { ModalState.License, new LicenseStateHandler() },
            };

            _disposables = new CompositeDisposable();
            _cts = new CancellationTokenSource();
        }

        public void Initialize()
        {
            _exceptionHandlingUseCase
                .SafeExecuteAsync(
                    () => _exceptionHandlingUseCase.RetryAsync(
                        () => InitializeAsync(_cts.Token),
                        MaxRetries,
                        _cts.Token
                    ),
                    _cts.Token
                ).Forget();
            SetupSubscriptions();

            // Set the global state to Title and initialize the scene state
            _gameStateUseCase.SetGlobalGameState("Title");
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _disposables?.Dispose();
            _titleSceneViewStateData?.Dispose();

            foreach (var handler in _pageStateHandlers.Values)
            {
                if (handler is IDisposable disposableHandler)
                {
                    disposableHandler.Dispose();
                }
            }

            foreach (var handler in _modalStateHandlers.Values)
            {
                if (handler is IDisposable disposableHandler)
                {
                    disposableHandler.Dispose();
                }
            }
        }

        private async UniTask InitializeAsync(CancellationToken ct)
        {
            try
            {
                await _scoreUseCase.InitializeAsync(ct);
                var licenses = await _licenseUseCase.GetLicensesAsync(ct);

                _titleSceneViewStateData = new TitleSceneViewStateData(_scoreUseCase.GetScoreData(), licenses);

                _titleSceneView.SettingsPageView.SetBgmSliderValue(_soundUseCase.VolumeBgm.Value);
                _titleSceneView.SettingsPageView.SetSeSliderValue(_soundUseCase.VolumeSe.Value);

                _titleSceneViewStateData.ScoreContainer = _scoreUseCase.GetScoreData();

                // Check the username here and display the modal if it does not exist.
                if (string.IsNullOrEmpty(_titleSceneViewStateData.UserName.Value))
                    HandleInputUserName();
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
            // Update the Page when the state is changed
            _currentPageState
                .DistinctUntilChanged()
                .Subscribe(state => _exceptionHandlingUseCase.SafeExecute(() => UpdatePageStateUI(state, _cts.Token)))
                .AddTo(_disposables);

            // Update the Modal when the state is changed
            _currentModalState
                .DistinctUntilChanged()
                .Subscribe(state => _exceptionHandlingUseCase.SafeExecute(() => UpdateModalStateUI(state, _cts.Token)))
                .AddTo(_disposables);

            // TitlePage
            _titleSceneView.GameStartRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecuteAsync(HandleGameStart, _cts.Token).Forget())
                .AddTo(_disposables);

            _titleSceneView.MyScoreRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleDisplayScores))
                .AddTo(_disposables);

            _titleSceneView.TitlePageView.OnSettings
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleDisplaySettings))
                .AddTo(_disposables);

            _titleSceneView.TitlePageView.OnLicense
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(OpenLicenseModal))
                .AddTo(_disposables);

            // DetailedScoreRankPage
            _titleSceneView.DetailedScoreRankPageView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleBackToTitlePage))
                .AddTo(_disposables);

            // SettingsPage
            _titleSceneView.SettingsPageView.ValueBgm
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => _exceptionHandlingUseCase.SafeExecute(() => HandleSetBgmVolume(value)))
                .AddTo(_disposables);

            _titleSceneView.SettingsPageView.ValueSe
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => _exceptionHandlingUseCase.SafeExecute(() => HandleSetSeVolume(value)))
                .AddTo(_disposables);

            _titleSceneView.SettingsPageView.OnUserNameChange
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(() => _currentModalState.Value = ModalState.UserNameInput))
                .AddTo(_disposables);

            _titleSceneView.SettingsPageView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleBackToTitlePage))
                .AddTo(_disposables);

            // UserNameModal
            _titleSceneView.UserNameModalView.OnUserNameSubmit
                .Subscribe(userName => _exceptionHandlingUseCase.SafeExecuteAsync(() => HandleUserNameSubmitted(userName), _cts.Token).Forget())
                .AddTo(_disposables);

            // LicenseModal
            _titleSceneView.LicenseModalView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleBackToTitlePage))
                .AddTo(_disposables);
        }

        private void UpdatePageStateUI(PageState state, CancellationToken ct)
        {
            if (_pageStateHandlers.TryGetValue(state, out var mainHandler))
            {
                mainHandler.ApplyStateAsync(_titleSceneView, _titleSceneViewStateData, ct).Forget();
            }
            else
            {
                throw new ApplicationException("The specified state is not supported.");
            }
        }

        private void UpdateModalStateUI(ModalState state, CancellationToken ct)
        {
            if (_modalStateHandlers.TryGetValue(state, out var modalHandler))
            {
                modalHandler.ApplyStateAsync(_titleSceneView, _titleSceneViewStateData, ct).Forget();
            }
            else
            {
                throw new ApplicationException("The specified state is not supported.");
            }
        }

        private async UniTask HandleGameStart()
        {
            try
            {
                _currentPageState.Value = PageState.Loading;
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

        private void HandleBackToTitlePage()
        {
            HandleSaveVolume().Forget();
            _currentPageState.Value = PageState.Title;
            _currentModalState.Value = ModalState.None;
        }

        private void HandleInputUserName()
        {
            _currentModalState.Value = ModalState.UserNameInput;
        }

        private async UniTask HandleUserNameSubmitted(string userName)
        {
            try
            {
                await _scoreUseCase.UpdateUserNameAsync(userName, _cts.Token);
                _currentModalState.Value = ModalState.None;
                _titleSceneViewStateData.UserName.Value = userName;
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while submitting the user name.", ex);
            }
        }

        private void HandleDisplayScores()
        {
            _currentPageState.Value = PageState.DetailedScore;
        }

        private void HandleDisplaySettings()
        {
            _currentPageState.Value = PageState.Settings;
        }

        private void HandleSetBgmVolume(float value)
        {
            _soundUseCase.SetBGMVolume(value);
            _titleSceneViewStateData.BgmVolume = value;
        }

        private void HandleSetSeVolume(float value)
        {
            _soundUseCase.SetSeVolume(value);
            _titleSceneViewStateData.SeVolume = value;
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

        private void OpenLicenseModal()
        {
            _currentModalState.Value = ModalState.License;
        }
    }
}
