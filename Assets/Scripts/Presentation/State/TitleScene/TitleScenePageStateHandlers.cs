using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    // TitleScene Specific PageState Handlers
    public class TitleLoadingStateHandler : TitleScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ShowLoadingPage();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the loading view state.", ex);
            }
        }
    }

    public class TitleDetailedScoreStateHandler : TitleScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.DetailedScoreRankPageView.ShowPage();
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
                throw new ApplicationException("An error occurred while displaying detailed scores view state.", ex);
            }
        }
    }

    public class SettingsStateHandler : TitleScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.SettingsPageView.SetUserName(data.ScoreContainer.Data.Score.UserName);
                view.SettingsPageView.ShowPage();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the settings view state.", ex);
            }
        }
    }

    public class TitleStateHandler : TitleScenePageStateHandlerBase
    {
        protected override async UniTask ApplyPageAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.TitlePageView.ShowPage();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Title view state.", ex);
            }
        }
    }
}
