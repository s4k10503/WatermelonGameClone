using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public abstract class MainSceneModalStateHandlerBase
        : ModalStateHandlerBase<MainSceneView, MainSceneViewStateData>
    {
        protected override async UniTask ResetAllModalAsync(
            MainSceneView view,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.HidePanel();
                view.PauseModalView.HideModal();
                view.GameOverModalView.HideModal();

                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while resetting MainScene modals.", ex);
            }
        }
    }
}
