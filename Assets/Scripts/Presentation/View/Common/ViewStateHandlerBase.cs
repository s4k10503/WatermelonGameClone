using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    // Base class for MainScene ViewState Handlers
    public abstract class MainSceneViewStateHandlerBase : IMainSceneViewStateHandler
    {
        public virtual void Apply(MainSceneView view, MainSceneViewStateData data)
        {
            ResetAllUI(view);
            ApplyCustomState(view, data);
        }

        protected virtual void ResetAllUI(MainSceneView view)
        {
            view.HideLoadingPage();
            view.ModalBackgroundView.HidePanel();
            view.PauseModalView.HidePanel();
            view.GameOverModalView.HidePanel();
            view.DetailedScoreRankPageView.HidePanel();
            view.Stageview.ShowStage();
            view.ShowMainPageMainElements();
        }

        protected abstract void ApplyCustomState(MainSceneView view, MainSceneViewStateData data);
    }

    // Base class for TitleScene ViewState Handlers
    public abstract class TitleSceneViewStateHandlerBase : ITitleSceneViewStateHandler
    {
        public virtual async UniTask ApplyAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
        {
            await ResetAllUIAsync(view, ct);
            await ApplyCustomStateAsync(view, data, ct);
        }

        protected virtual async UniTask ResetAllUIAsync(
            TitleSceneView view, CancellationToken ct)
        {
            try
            {
                view.HideLoadingPage();
                view.ModalBackgroundView.HidePanel();
                view.HideTitlePageMainElements();
                view.UserNameModalView.HideModal();
                view.DetailedScoreRankPageView.HidePanel();
                view.SettingsPageView.HidePanel();
                view.LicenseModalView.HideModal();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while rest UI.", ex);
            }
        }

        protected abstract UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct);
    }
}
