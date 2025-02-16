using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

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
                view.TitlePageView.HidePage();
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
        private readonly CompositeDisposable _disposables = new();

        protected override async UniTask ApplyPageAsync(
            TitleSceneView view,
            TitleSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.SettingsPageView.SetUserName(data.ScoreContainer.Data.Score.UserName);
                data.UserName
                    .DistinctUntilChanged()
                    .Subscribe(newName => view.SettingsPageView.SetUserName(newName))
                    .AddTo(_disposables);

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
        public override void Dispose()
        {
            _disposables.Dispose();
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
