using Presentation.DTO;
using Presentation.State.Common;
using Presentation.View.MainScene;

using System.Threading;
using Cysharp.Threading.Tasks;

namespace Presentation.State.MainScene
{
    // Base class for MainScene PageState Handlers
    public abstract class MainScenePageStateHandlerBase
        : PageStateHandlerBase<MainSceneView, MainSceneViewStateData>
    {
        protected override async UniTask ResetAllPageAsync(
            MainSceneView view,
            CancellationToken ct)
        {
            view.HideLoadingPage();
            view.DetailedScoreRankPageView.HidePage();
            view.StageView.ShowStage();
            view.ShowMainPageMainElements();

            await UniTask.CompletedTask.AttachExternalCancellation(ct);
        }
    }
}
