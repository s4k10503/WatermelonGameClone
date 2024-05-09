using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

        public IObservable<Unit> RestartRequested => GameOverPanelView.OnRestart;
        public IObservable<Unit> BackToTitleRequested => GameOverPanelView.OnBackToTitle;
    }
}
