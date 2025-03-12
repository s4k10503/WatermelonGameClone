using Presentation.DTO;
using Presentation.View.TitleScene;

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using System.Linq;

namespace Presentation.State.TitleScene
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
                var scoreContainerDto = new ScoreContainerDto
                {
                    data = new ScoreContainerDto.RankingData
                    {
                        rankings = new ScoreContainerDto.RankingData.Rankings
                        {
                            daily = new ScoreContainerDto.RankingData.RankingScores
                            {
                                scores = data.ScoreContainer.data.rankings.daily.scores.ToList()
                            },
                            monthly = new ScoreContainerDto.RankingData.RankingScores
                            {
                                scores = data.ScoreContainer.data.rankings.monthly.scores.ToList()
                            },
                            allTime = new ScoreContainerDto.RankingData.RankingScores
                            {
                                scores = data.ScoreContainer.data.rankings.allTime.scores.ToList()
                            }
                        }
                    }
                };
                view.DetailedScoreRankPageView.DisplayTopScores(scoreContainerDto);
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
                view.SettingsPageView.SetUserName(data.ScoreContainer.data.score.userName);
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
