using UseCase.DTO;
using UseCase.Interfaces;
using Presentation.DTO;
using Presentation.Interfaces;
using Presentation.State.Common;
using Presentation.State.MainScene;
using Presentation.View.MainScene;

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Presentation.Presenter
{
    public sealed class MainScenePresenter : IInitializable, ITickable, IDisposable
    {
        // UseCases
        private readonly IMergeItemUseCase _mergeItemUseCase;
        private readonly IScoreUseCase _scoreUseCase;
        private readonly ISoundUseCase _soundUseCase;
        private readonly IGameStateUseCase _gameStateUseCase;
        private readonly ISceneLoaderUseCase _sceneLoaderUseCase;
        private readonly IExceptionHandlingUseCase _exceptionHandlingUseCase;

        // View
        private readonly MainSceneView _mainSceneView;
        private readonly ReactiveProperty<PageState> _currentPageState = new(PageState.Loading);
        private readonly ReactiveProperty<ModalState> _currentModalState = new(ModalState.None);

        // Dictionary to manage the page state
        private readonly Dictionary<PageState, IPageStateHandler<MainSceneView, MainSceneViewStateData>>
            _pageStateHandlers;

        // Dictionary to manage the modal state (for overlay display)
        private readonly Dictionary<ModalState, IModalStateHandler<MainSceneView, MainSceneViewStateData>>
            _modalStateHandlers;

        // Status data (created once in Presenter and then updated)
        private MainSceneViewStateData _mainSceneViewStateData;

        private bool _isNext;
        private float _mergeItemCreateDelayTime;
        private string _previousGlobalState;

        private const string TitleSceneName = "TitleScene";
        private const int MaxRetries = 3;

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

            _pageStateHandlers = new Dictionary<PageState, IPageStateHandler<MainSceneView, MainSceneViewStateData>>
            {
                { PageState.Loading, new MainLoadingStateHandler() },
                { PageState.Playing, new PlayingStateHandler() },
                { PageState.DetailedScore, new MainDetailedScoreStateHandler() },
            };

            _modalStateHandlers = new Dictionary<ModalState, IModalStateHandler<MainSceneView, MainSceneViewStateData>>
            {
                { ModalState.None, new MainNoneStateHandler() },
                { ModalState.Paused, new PausedStateHandler() },
                { ModalState.GameOver, new GameOverStateHandler() }
            };

            _cts = new CancellationTokenSource();
            _disposables = new CompositeDisposable();
        }

        public void Initialize()
        {
            _exceptionHandlingUseCase.SafeExecuteAsync(
                () => _exceptionHandlingUseCase.RetryAsync(
                    () => InitializeAsync(_cts.Token),
                    MaxRetries,
                    _cts.Token
                ),
                _cts.Token
            ).Forget();

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
                        CreateNextItemAsync,
                        MaxRetries,
                        _cts.Token
                    ),
                    _cts.Token
                ).Forget();
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _disposables?.Dispose();
            _mainSceneViewStateData?.Dispose();

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
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                await _scoreUseCase.InitializeAsync(ct);
                _gameStateUseCase.SetGlobalGameState("Playing");

                // Initialize View state data
                _mainSceneViewStateData = new MainSceneViewStateData(
                    _scoreUseCase.CurrentScore.Value,
                    _scoreUseCase.BestScore.Value,
                    _scoreUseCase.GetScoreData(),
                    _mergeItemUseCase.NextItemIndex.Value
                );

                // After initialization is complete, switch to Playing state
                _currentPageState.Value = PageState.Playing;
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
                .Where(_ => _mainSceneViewStateData != null)
                .Subscribe(index => _mainSceneViewStateData.NextItemIndex.Value = index)
                .AddTo(_disposables);

            _scoreUseCase.CurrentScore
                .Where(_ => _mainSceneViewStateData != null)
                .Subscribe(score => _mainSceneViewStateData.CurrentScore.Value = score)
                .AddTo(_disposables);

            _scoreUseCase.BestScore
                .Where(_ => _mainSceneViewStateData != null)
                .Subscribe(score => _mainSceneViewStateData.BestScore.Value = score)
                .AddTo(_disposables);
        }

        private void SubscribeToViewEvents(CancellationToken ct)
        {
            // Update the Page when the state is changed
            _currentPageState
                .DistinctUntilChanged()
                .Subscribe(state =>
                    _exceptionHandlingUseCase.SafeExecute(() => UpdatePageStateUI(state, _cts.Token)))
                .AddTo(_disposables);

            // Update the Modal when the state is changed
            _currentModalState
                .DistinctUntilChanged()
                .Subscribe(state =>
                    _exceptionHandlingUseCase.SafeExecute(() => UpdateModalStateUI(state, _cts.Token)))
                .AddTo(_disposables);

            _mainSceneView.RestartRequested
                .Subscribe(_ =>
                    _exceptionHandlingUseCase.SafeExecuteAsync(() => HandleSceneChangeAsync(SceneManager.GetActiveScene().name, ct), ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToTitleRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecuteAsync(() => HandleSceneChangeAsync(TitleSceneName, ct), ct).Forget())
                .AddTo(_disposables);

            _mainSceneView.BackToGameRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(ResumeGame))
                .AddTo(_disposables);

            _mainSceneView.PauseRequested
                .Where(_ => _gameStateUseCase.GlobalStateString.Value != "GameOver" &&
                            _gameStateUseCase.GlobalStateString.Value != "Paused")
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(PauseGame))
                .AddTo(_disposables);

            _mainSceneView.DisplayScoreRequested
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(ShowDetailedScore))
                .AddTo(_disposables);

            _mainSceneView.DetailedScoreRankPageView.OnBack
                .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(ReturnToGameOverScreen))
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
                        .Subscribe(_ => _exceptionHandlingUseCase.SafeExecute(HandleItemDropping))
                        .AddTo(_disposables);

                    mergeItem.OnMergeRequest
                        .Subscribe(request => _exceptionHandlingUseCase.SafeExecute(() => HandleMergeRequest(request.Source, request.Target)))
                        .AddTo(_disposables);
                }).AddTo(_disposables);
        }

        private void UpdatePageStateUI(PageState state, CancellationToken ct)
        {
            if (_pageStateHandlers.TryGetValue(state, out var handler))
            {
                handler.ApplyStateAsync(_mainSceneView, _mainSceneViewStateData, ct).Forget();
            }
            else
            {
                throw new ApplicationException("The specified page state is not supported.");
            }
        }

        private void UpdateModalStateUI(ModalState state, CancellationToken ct)
        {
            if (_modalStateHandlers.TryGetValue(state, out var handler))
            {
                handler.ApplyStateAsync(_mainSceneView, _mainSceneViewStateData, ct).Forget();
            }
            else
            {
                throw new ApplicationException("The specified modal state is not supported.");
            }
        }

        private void HandleContactTimeUpdated(Guid id, float deltaTime)
        {
            _mergeItemUseCase.AddContactTime(id, deltaTime);
            if (_mergeItemUseCase.CheckGameOver(id))
            {
                HandleGameOverAsync().Forget();
            }
        }

        private void HandleContactExited(Guid id)
        {
            _mergeItemUseCase.ResetContactTime(id);
        }

        private bool CanCreateNextItem()
        {
            return _isNext && _gameStateUseCase.GlobalStateString.Value != "GameOver";
        }
        private async UniTask CreateNextItemAsync()
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _isNext = false;

                // Generate entity from use case
                var mergeItemDto = _mergeItemUseCase.CreateMergeItemDTO(_mergeItemUseCase.NextItemIndex.Value);

                await _mainSceneView.MergeItemManager.CreateItemAsync(
                    mergeItemDto.Id,
                    _mergeItemUseCase.NextItemIndex.Value,
                    _mergeItemCreateDelayTime,
                    _cts.Token);

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
            _soundUseCase.PlaySoundEffect("Drop");
            _isNext = true;
        }

        private void HandleMergeRequest(IMergeItemView sourceView, IMergeItemView targetView)
        {
            var sourceDto = _mergeItemUseCase.GetMergeItemDTOById(sourceView.Id);
            var targetDto = _mergeItemUseCase.GetMergeItemDTOById(targetView.Id);

            if (sourceDto == null
                || targetDto == null
                || !_mergeItemUseCase.CanMerge(sourceView.Id, targetView.Id)) return;
            
            Vector2 unityPosSource = sourceView.GameObject.transform.position;
            Vector2 unityPosTarget = targetView.GameObject.transform.position;

            System.Numerics.Vector2 numericsPosSource = new(unityPosSource.x, unityPosSource.y);
            System.Numerics.Vector2 numericsPosTarget = new(unityPosTarget.x, unityPosTarget.y);

            var mergeDataDto = _mergeItemUseCase.CreateMergeDataDTO(sourceView.Id, numericsPosSource, targetView.Id, numericsPosTarget);
            var newMergeItemDto = _mergeItemUseCase.CreateMergeItemDTO(mergeDataDto.ItemNo);

            HandleItemMerging(newMergeItemDto.Id, mergeDataDto, sourceView, targetView);
        }

        private void HandleItemMerging(
            Guid id,
            MergeResultDto mergeResultData,
            IMergeItemView sourceView,
            IMergeItemView targetView)
        {
            _mainSceneView.MergeItemManager.DestroyItem(sourceView.GameObject);
            _mainSceneView.MergeItemManager.DestroyItem(targetView.GameObject);

            Vector2 unityPos = new(mergeResultData.Position.X, mergeResultData.Position.Y);
            _mainSceneView.MergeItemManager.MergeItem(id, unityPos, mergeResultData.ItemNo);

            _soundUseCase.PlaySoundEffect("Merge");
            _scoreUseCase.UpdateCurrentScore(mergeResultData.ItemNo);
        }


        private async UniTask HandleGameOverAsync()
        {
            if (_cts == null || _cts.IsCancellationRequested) return;

            try
            {
                _gameStateUseCase.SetGlobalGameState("GameOver");
                await _scoreUseCase.UpdateScoreRankingAsync(_scoreUseCase.CurrentScore.Value, _cts.Token);
                _mainSceneViewStateData.ScoreContainer = _scoreUseCase.GetScoreData();
                AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);

                _currentModalState.Value = ModalState.GameOver;
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
                _currentModalState.Value = ModalState.None;
                _currentPageState.Value = PageState.Loading;
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
            _currentModalState.Value = ModalState.None;
            _currentPageState.Value = PageState.Playing;
        }

        private void PauseGame()
        {
            _previousGlobalState = _gameStateUseCase.GlobalStateString.Value;
            _gameStateUseCase.SetGlobalGameState("Paused");
            AdjustTimeScale(_gameStateUseCase.TimeScaleGameOver);
            _currentModalState.Value = ModalState.Paused;
        }

        private void ShowDetailedScore()
        {
            _currentModalState.Value = ModalState.None;
            _currentPageState.Value = PageState.DetailedScore;
        }

        private void ReturnToGameOverScreen()
        {
            _currentPageState.Value = PageState.Playing;
            _currentModalState.Value = ModalState.GameOver;
        }

        private void AdjustTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }
    }
}
