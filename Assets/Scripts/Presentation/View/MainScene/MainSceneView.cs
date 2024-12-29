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
        public IDetailedScoreRankPageView DetailedScoreRankPageView { get; private set; }
        public INextItemPanelView NextItemPanelView { get; private set; }
        public IGameOverModalView GameOverModalView { get; private set; }
        public IPauseModalView PauseModalView { get; private set; }
        public ModalBackgroundView ModalBackgroundView { get; private set; }
        public IScreenshotHandler ScreenshotHandler { get; private set; }
        public IMergeItemManager MergeItemManager { get; private set; }
        public StageView Stageview { get; private set; }


        [Inject]
        public void Construct(
            IScorePanelView scorePanelView,
            IScoreRankView scoreRankView,
            IDetailedScoreRankPageView detailedScoreRankView,
            INextItemPanelView nextItemPanelView,
            IGameOverModalView gameOverPanelView,
            IPauseModalView pausePanelView,
            ModalBackgroundView backgroundPanelView,
            IScreenshotHandler screenshotHandler,
            IInputEventProvider inputEventProvider,
            IMergeItemManager mergeItemManager,
            StageView stageview)
        {
            ScorePanelView = scorePanelView;
            ScoreRankView = scoreRankView;
            DetailedScoreRankPageView = detailedScoreRankView;
            NextItemPanelView = nextItemPanelView;
            GameOverModalView = gameOverPanelView;
            PauseModalView = pausePanelView;
            ModalBackgroundView = backgroundPanelView;
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
            DetailedScoreRankPageView = null;
            NextItemPanelView = null;
            GameOverModalView = null;
            PauseModalView = null;
            ModalBackgroundView = null;
            ScreenshotHandler = null;
            MergeItemManager = null;
            Stageview = null;

            _inputEventProvider = null;
        }

        public IObservable<Unit> RestartRequested =>
            Observable.Merge
            (
                GameOverModalView.OnRestart,
                PauseModalView.OnRestart
            );

        public IObservable<Unit> BackToTitleRequested =>
            Observable.Merge
            (
                GameOverModalView.OnBackToTitle,
                PauseModalView.OnBackToTitle
            );

        public IObservable<Unit> BackToGameRequested
            => PauseModalView.OnBackToGame;
        public IObservable<Unit> PauseRequested
            => _inputEventProvider.OnEscapeKey;
        public IObservable<Unit> DisplayScoreRequested
            => GameOverModalView.OnDisplayScore;

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
