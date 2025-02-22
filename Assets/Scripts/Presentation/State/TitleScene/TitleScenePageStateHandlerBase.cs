using Presentation.DTO;
using Presentation.State.Common;
using Presentation.View.TitleScene;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Presentation.State.TitleScene
{
    // Base class for TitleScene PageState Handlers
    public abstract class TitleScenePageStateHandlerBase
        : PageStateHandlerBase<TitleSceneView, TitleSceneViewStateData>
    {
        protected override async UniTask ResetAllPageAsync(
            TitleSceneView view,
            CancellationToken ct)
        {
            try
            {
                view.HideLoadingPage();
                view.TitlePageView.HidePage();
                view.DetailedScoreRankPageView.HidePage();
                view.SettingsPageView.HidePage();

                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while rest page.", ex);
            }
        }
    }
}
