using Presentation.DTO;
using Presentation.View.MainScene;

using System;
using System.Threading;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Presentation.State.MainScene
{
    // When not over (modals are hidden), hide all necessary modals.
    public sealed class MainNoneStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the None state.", ex);
            }
        }
    }

    // When paused, the background panel is displayed and the pause modal is displayed.
    public class PausedStateHandler : MainSceneModalStateHandlerBase
    {
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                view.ModalBackgroundView.ShowPanel();
                view.PauseModalView.ShowModal();
                await UniTask.CompletedTask.AttachExternalCancellation(ct);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is considered normal behavior and the processing is terminated
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while applying the Paused state.", ex);
            }
        }
    }

    // When the game is over, display modals based on the latest scores, screenshots and ranking information.
    public class GameOverStateHandler : MainSceneModalStateHandlerBase
    {
        private RenderTexture _screenshot;
        protected override async UniTask ApplyModalAsync(
            MainSceneView view,
            MainSceneViewStateData data,
            CancellationToken ct)
        {
            try
            {
                _screenshot = await view.ScreenshotHandler.CaptureScreenshotAsync(ct);

                view.ModalBackgroundView.ShowPanel();
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
                view.GameOverModalView.ShowModal(
                    data.CurrentScore.Value,
                    _screenshot,
                    scoreContainerDto);

                view.ScorePanelView.UpdateBestScore(data.BestScore.Value);
                view.ScoreRankView.DisplayTopScores(scoreContainerDto);
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
                throw new ApplicationException("An error occurred while applying the GameOver state.", ex);
            }
        }
        public override void Dispose()
        {
            if (_screenshot == null) return;
            RenderTexture.ReleaseTemporary(_screenshot);
            _screenshot = null;
        }
    }
}
