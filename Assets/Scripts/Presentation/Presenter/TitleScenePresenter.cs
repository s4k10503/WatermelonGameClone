using UniRx;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;

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

        private readonly CompositeDisposable _disposables;

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

            _disposables = new CompositeDisposable();

            _titleSceneView.HideLoadingPage();
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
            SubscribeToGameView();

            // Set the global state to Title and initialize the scene state
            _gameStateUseCase.SetGlobalGameState(GlobalGameState.Title);
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
        }

        private async UniTask InitializeAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                await _scoreUseCase.InitializeAsync(cts.Token);

                _titleSceneView.SettingsPanelView
                    .SetBgmSliderValue(_soundUseCase.VolumeBgm.Value);
                _titleSceneView.SettingsPanelView
                    .SetSeSliderValue(_soundUseCase.VolumeSe.Value);

                UpdateScoreDisplay();
                cts.Cancel();
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void SubscribeToGameView()
        {
            // TitlePanel
            _titleSceneView.GameStartRequested
                .Subscribe(_ => HandleGameStart().Forget())
                .AddTo(_disposables);

            _titleSceneView.MyScoreRequested
                .Subscribe(_ => HandleDisplayScores())
                .AddTo(_disposables);

            _titleSceneView.TitlePanellView.OnSettings
                .Subscribe(_ => HandleDisplaySettings())
                .AddTo(_disposables);

            // DetailedScoreRankPanel
            _titleSceneView.DetailedScoreRankView.OnBack
                .Subscribe(_ => HandleBackToTitlePanel())
                .AddTo(_disposables);

            // SettingsPanel
            _titleSceneView.SettingsPanelView.ValueBgm
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => HandleSetBgmVolume(value))
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.ValueSe
                .SkipLatestValueOnSubscribe()
                .Subscribe(value => HandleSetSeVolume(value))
                .AddTo(_disposables);

            _titleSceneView.SettingsPanelView.OnBack
                .Subscribe(_ => HandleBackToTitlePanel())
                .AddTo(_disposables);
        }

        private async UniTask HandleGameStart()
        {
            using (var cts = new CancellationTokenSource())
            {
                _gameStateUseCase.SetGlobalGameState(GlobalGameState.Playing);
                _titleSceneView.HideTitlePage();
                _titleSceneView.ShowLoadingPage();
                await _sceneLoaderUseCase.LoadSceneAsync("MainScene", cts.Token);
                cts.Cancel();
            }
        }

        private void HandleBackToTitlePanel()
        {
            HandleSaveVolume().Forget();
            _titleSceneView.DetailedScoreRankView.HidePanel();
            _titleSceneView.SettingsPanelView.HidePanel();
            _titleSceneView.ShowTitlePageMainElements();
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Idle);
        }

        private void HandleDisplayScores()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.DisplayingScores);
            _titleSceneView.HideTitlePageMainElements();
            _titleSceneView.DetailedScoreRankView.ShowPanel();
        }

        private void HandleDisplaySettings()
        {
            _gameStateUseCase.SetSceneSpecificState(SceneSpecificState.Settings);
            _titleSceneView.SettingsPanelView.ShowPanel();
        }

        private void HandleSetBgmVolume(float value)
            => _soundUseCase.SetBGMVolume(value);

        private void HandleSetSeVolume(float value)
            => _soundUseCase.SetSEVolume(value);

        private async UniTask HandleSaveVolume()
        {
            using (var cts = new CancellationTokenSource())
            {
                await _soundUseCase.SaveVolume(cts.Token);
                cts.Cancel();
            }
        }

        private void UpdateScoreDisplay()
        {
            var scoreData = _scoreUseCase.GetScoreData();
            _titleSceneView.DetailedScoreRankView.DisplayTopScores(scoreData);
        }
    }
}
