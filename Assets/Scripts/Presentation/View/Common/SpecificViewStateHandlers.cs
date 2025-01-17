using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace WatermelonGameClone.Presentation
{
    // MainScene Specific Handlers
    public class MainSceneLoadingViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.ShowLoadingPage();
        }
    }

    public class PlayingViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            // Playing state is the default state set by ResetAllUI.
        }
    }

    public class PausedViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.ModalBackgroundView.ShowPanel();
            view.PauseModalView.ShowPanel();
        }
    }

    public class GameOverViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.ModalBackgroundView.ShowPanel();
            view.GameOverModalView.ShowPanel(data.CurrentScore, data.Screenshot, data.ScoreContainer);
        }
    }

    public class MainSceneDetailedScoreViewStateHandler : MainSceneViewStateHandlerBase
    {
        protected override void ApplyCustomState(MainSceneView view, MainSceneViewStateData data)
        {
            view.Stageview.HideStage();
            view.HideMainPageMainElements();
            view.GameOverModalView.HidePanel();
            view.DetailedScoreRankPageView.ShowPanel();
        }
    }

    // TitleScene Specific Handlers
    public class TitleSceneLoadingViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override async UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
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

    public class TitleSceneDetailedScoreViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override async UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
        {
            try
            {
                view.DetailedScoreRankPageView.ShowPanel();
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

    public class SettingsViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override async UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                view.SettingsPageView.ShowPanel();
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

    public class LicenseViewStateHandler : TitleSceneViewStateHandlerBase
    {
        private bool _isLicenseTextSet = false;

        protected override async UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                var licenseModalView = view.LicenseModalView;
                licenseModalView.ShowModal();

                // Update license information text
                if (!_isLicenseTextSet)
                {
                    await licenseModalView.SetLicensesAsync(data.Licenses, ct);
                    _isLicenseTextSet = true;
                }

                // Forcibly update the mesh to stabilize the layout
                licenseModalView.ForceMeshUpdateText();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the license view state.", ex);
            }
        }
    }

    public class TitleViewStateHandler : TitleSceneViewStateHandlerBase
    {
        protected override async UniTask ApplyCustomStateAsync(
            TitleSceneView view, TitleSceneViewStateData data, CancellationToken ct)
        {
            try
            {
                view.ShowTitlePageMainElements();
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