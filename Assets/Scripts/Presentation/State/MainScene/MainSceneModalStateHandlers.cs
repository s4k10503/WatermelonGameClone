using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WatermelonGameClone.Presentation
{
    // When not over (modals are hidden), hide all necessary modals.
    public class MainNoneStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.HidePanel();
                view.GameOverModalView.HideModal();
                view.PauseModalView.HideModal();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the None state.", ex);
            }
        }
    }

    // When pause, the background panel is displayed and the pause modal is displayed.
    public class PausedStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                view.PauseModalView.ShowModal();

                view.GameOverModalView.HideModal();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Paused state.", ex);
            }
        }
    }

    // When the game is over, display modals based on the latest scores, screenshots and ranking information.
    public class GameOverStateHandler : MainSceneModalStateHandlerBase
    {
        private RenderTexture _screenshot;
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                _screenshot = await view.ScreenshotHandler.CaptureScreenshotAsync(ct);

                view.ModalBackgroundView.ShowPanel();
                view.GameOverModalView.ShowModal(
                    data.CurrentScore.Value,
                    _screenshot,
                    data.ScoreContainer);

                view.PauseModalView.HideModal();

                view.ScorePanelView.UpdateBestScore(data.BestScore.Value);
                view.ScoreRankView.DisplayTopScores(data.ScoreContainer);
                view.DetailedScoreRankPageView.DisplayTopScores(data.ScoreContainer);

                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the GameOver state.", ex);
            }
        }
        public override void Dispose()
        {
            if (_screenshot != null)
            {
                RenderTexture.ReleaseTemporary(_screenshot);
                _screenshot = null;
            }
        }
    }
}
