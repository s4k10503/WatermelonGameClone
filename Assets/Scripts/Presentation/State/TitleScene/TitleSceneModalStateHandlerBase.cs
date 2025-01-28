using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    public abstract class TitleSceneModalStateHandlerBase
        : ModalStateHandlerBase<TitleSceneView, TitleSceneViewStateData>
    {
        protected override async UniTask ResetAllModalAsync(
            TitleSceneView view,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.HidePanel();
                view.UserNameModalView.HideModal();
                view.LicenseModalView.HideModal();

                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while resetting TitleScene modals.", ex);
            }
        }
    }
}
