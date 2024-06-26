using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace WatermelonGameClone
{
    public interface IGameView
    {
        IScorePanelView ScorePanelView { get; }
        IScoreRankView ScoreRankView { get; }
        INextSpherePanelView NextSpherePanelView { get; }
        IGameOverPanelView GameOverPanelView { get; }
        IPausePanelView PausePanelView { get; }
        IBackgroundPanelView BackgroundPanelView { get; }

        IObservable<Unit> RestartRequested { get; }
        IObservable<Unit> BackToTitleRequested { get; }
        IObservable<Unit> BackToGameRequested { get; }
        IObservable<Unit> PauseRequested { get; }
    }
}
