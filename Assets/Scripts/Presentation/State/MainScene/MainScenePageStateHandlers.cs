using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    // MainScene Specific PageState Handlers
    public class MainLoadingStateHandler : MainScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.ShowLoadingPage();

            await UniTask.CompletedTask.AttachExternalCancellation(ct);
        }
    }

    public class PlayingStateHandler : MainScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            // Playing state is the default state set by ResetAllUI.
            await UniTask.CompletedTask.AttachExternalCancellation(ct);
        }
    }

    public class MainDetailedScoreStateHandler : MainScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.GameOverModalView.HideModal();
            view.DetailedScoreRankPageView.ShowPage();

            await UniTask.CompletedTask.AttachExternalCancellation(ct);
        }
    }
}
