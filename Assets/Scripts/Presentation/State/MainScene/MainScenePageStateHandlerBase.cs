using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
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
            view.Stageview.ShowStage();
            view.ShowMainPageMainElements();

            await UniTask.CompletedTask.AttachExternalCancellation(ct);
        }
    }
}
