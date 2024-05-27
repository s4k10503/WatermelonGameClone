using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class GameView : MonoBehaviour, IGameView
    {
        [Inject] public IScorePanelView ScorePanelView { get; private set; }
        [Inject] public IScoreRankView ScoreRankView { get; private set; }
        [Inject] public INextSpherePanelView NextSpherePanelView { get; private set; }
        [Inject] public IGameOverPanelView GameOverPanelView { get; private set; }
        [Inject] public IPausePanelView PausePanelView { get; private set; }
        [Inject] public IBackgroundPanelView BackgroundPanelView { get; private set; }

        [Inject] private IInputEventProvider _inputEventProvider;

        public IObservable<Unit> RestartRequested =>
            Observable.Merge
            (
                GameOverPanelView.OnRestart,
                PausePanelView.OnRestart
            );

        public IObservable<Unit> BackToTitleRequested =>
            Observable.Merge
            (
                GameOverPanelView.OnBackToTitle,
                PausePanelView.OnBackToTitle
            );

        public IObservable<Unit> BackToGameRequested => PausePanelView.OnBackToGame;
        public IObservable<Unit> PauseRequested => _inputEventProvider.OnEscapeKey;
    }
}
