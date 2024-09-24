using System;
using UnityEngine;
using UniRx;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class TitleSceneView : MonoBehaviour
    {
        [SerializeField] private Canvas _titlePageRoot;
        [SerializeField] private Canvas _titlePageMainElements;
        [SerializeField] private Canvas _loadingPageRoot;

        public ITitlePanelView TitlePanellView { get; private set; }
        public IDetailedScoreRankView DetailedScoreRankView { get; private set; }
        public ISettingsPanelView SettingsPanelView { get; private set; }

        public IObservable<Unit> GameStartRequested => TitlePanellView.OnGameStart;
        public IObservable<Unit> MyScoreRequested => TitlePanellView.OnMyScore;

        [Inject]
        public void Construct(
            ITitlePanelView titlePanellView,
            IDetailedScoreRankView detailedScoreRankView,
            ISettingsPanelView settingsPanelView)
        {
            TitlePanellView = titlePanellView;
            DetailedScoreRankView = detailedScoreRankView;
            SettingsPanelView = settingsPanelView;
        }

        public void ShowTitlePage()
        {
            _titlePageRoot.enabled = true;
        }

        public void HideTitlePage()
        {
            _titlePageRoot.enabled = false;
        }

        public void ShowTitlePageMainElements()
        {
            _titlePageMainElements.enabled = true;
        }

        public void HideTitlePageMainElements()
        {
            _titlePageMainElements.enabled = false;
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
