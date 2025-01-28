using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    // MainScene Specific ModalState Handlers
    public class MainNoneStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
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
                throw new ApplicationException("An error occurred while applying the None state.", ex);
            }
        }
    }

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

    public class GameOverStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                view.GameOverModalView.ShowModal(data.CurrentScore, data.Screenshot, data.ScoreContainer);
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
    }
}
