using System;
using UnityEngine;
using UniRx;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class MainSceneView : MonoBehaviour
    {
        [SerializeField] private Canvas _mainPageRoot;
        [SerializeField] private Canvas _mainPageMainElements;
        [SerializeField] private Canvas _loadingPageRoot;

        private IInputEventProvider _inputEventProvider;

        public IScorePanelView ScorePanelView { get; private set; }
        public IScoreRankView ScoreRankView { get; private set; }
        public IDetailedScoreRankView DetailedScoreRankView { get; private set; }
        public INextItemPanelView NextItemPanelView { get; private set; }
        public IGameOverPanelView GameOverPanelView { get; private set; }
        public IPausePanelView PausePanelView { get; private set; }
        public IBackgroundPanelView BackgroundPanelView { get; private set; }
        public IScreenshotHandler ScreenshotHandler { get; private set; }
        public IMergeItemManager MergeItemManager { get; private set; }
        public StageView Stageview { get; private set; }


        [Inject]
        public void Construct(
            IScorePanelView scorePanelView,
            IScoreRankView scoreRankView,
            IDetailedScoreRankView detailedScoreRankView,
            INextItemPanelView nextItemPanelView,
            IGameOverPanelView gameOverPanelView,
            IPausePanelView pausePanelView,
            IBackgroundPanelView backgroundPanelView,
            IScreenshotHandler screenshotHandler,
            IInputEventProvider inputEventProvider,
            IMergeItemManager mergeItemManager,
            StageView stageview)
        {
            ScorePanelView = scorePanelView;
            ScoreRankView = scoreRankView;
            DetailedScoreRankView = detailedScoreRankView;
            NextItemPanelView = nextItemPanelView;
            GameOverPanelView = gameOverPanelView;
            PausePanelView = pausePanelView;
            BackgroundPanelView = backgroundPanelView;
            ScreenshotHandler = screenshotHandler;
            MergeItemManager = mergeItemManager;
            _inputEventProvider = inputEventProvider;
            Stageview = stageview;
        }

        private void OnDestroy()
        {
            _mainPageRoot = null;
            _mainPageMainElements = null;
            _loadingPageRoot = null;

            ScorePanelView = null;
            ScoreRankView = null;
            DetailedScoreRankView = null;
            NextItemPanelView = null;
            GameOverPanelView = null;
            PausePanelView = null;
            BackgroundPanelView = null;
            ScreenshotHandler = null;
            MergeItemManager = null;
            Stageview = null;

            _inputEventProvider = null;
        }

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

        public IObservable<Unit> BackToGameRequested
            => PausePanelView.OnBackToGame;
        public IObservable<Unit> PauseRequested
            => _inputEventProvider.OnEscapeKey;
        public IObservable<Unit> DisplayScoreRequested
            => GameOverPanelView.OnDisplayScore;

        public void ShowMainPage()
        {
            _mainPageRoot.enabled = true;
        }

        public void HideMainPage()
        {
            _mainPageRoot.enabled = false;
        }

        public void ShowMainPageMainElements()
        {
            _mainPageMainElements.enabled = true;
        }

        public void HideMainPageMainElements()
        {
            _mainPageMainElements.enabled = false;
        }

        public void ShowLoadingPage()
        {
            _loadingPageRoot.enabled = true;
        }

        public void HideLoadingPage()
        {
            _loadingPageRoot.enabled = false;
        }
    }
}
