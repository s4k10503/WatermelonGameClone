using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace WatermelonGameClone.Presentation
{
    // When loading, hide the stage and main UI elements and display the loading screen.
    public class MainLoadingStateHandler : MainScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.Stageview.HideStage();
                view.HideMainPageMainElements();

                view.ShowLoadingPage();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Loading page state.", ex);
            }
        }
    }

    // While playing, the UI for normal gameplay will be displayed (automatically reflected by StateData update from Presenter)
    public class PlayingStateHandler : MainScenePageStateHandlerBase
    {
        private readonly CompositeDisposable _disposables = new();

        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.Stageview.ShowStage();
                view.ShowMainPageMainElements();

                view.HideLoadingPage();
                view.ModalBackgroundView.HidePanel();

                view.ScorePanelView.UpdateBestScore(data.BestScore.Value);
                view.ScoreRankView.DisplayTopScores(data.ScoreContainer);
                view.DetailedScoreRankPageView.DisplayTopScores(data.ScoreContainer);

                data.NextItemIndex
                    .DistinctUntilChanged()
                    .Subscribe(view.NextItemPanelView.UpdateNextItemImages)
                    .AddTo(_disposables);

                data.CurrentScore
                    .DistinctUntilChanged()
                    .Subscribe(view.ScorePanelView.UpdateCurrentScore)
                    .AddTo(_disposables);

                data.CurrentScore
                    .DistinctUntilChanged()
                    .Subscribe(view.ScoreRankView.DisplayCurrentScore)
                    .AddTo(_disposables);

                data.BestScore
                    .DistinctUntilChanged()
                    .Subscribe(view.ScorePanelView.UpdateBestScore)
                    .AddTo(_disposables);

                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Playing page state.", ex);
            }
        }

        public override void Dispose()
        {
            _disposables.Dispose();
        }
    }

    // When viewing detailed scores, hide the UI in the game and display the detailed score ranking page.
    public class MainDetailedScoreStateHandler : MainScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.Stageview.HideStage();
                view.HideMainPageMainElements();
                view.GameOverModalView.HideModal();

                view.DetailedScoreRankPageView.ShowPage();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Detailed Score page state.", ex);
            }
        }
    }
}
