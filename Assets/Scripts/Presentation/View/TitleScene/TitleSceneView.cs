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

        public ITitlePageView TitlePageView { get; private set; }
        public IUserNameModalView UserNameModalView;
        public IDetailedScoreRankPageView DetailedScoreRankPageView { get; private set; }
        public ISettingsModalView SettingsPageView { get; private set; }
        public ILicenseModalView LicenseModalView { get; private set; }
        public ModalBackgroundView ModalBackgroundView { get; private set; }

        public IObservable<Unit> GameStartRequested => TitlePageView.OnGameStart;
        public IObservable<Unit> MyScoreRequested => TitlePageView.OnMyScore;

        [Inject]
        public void Construct(
            ITitlePageView titlePanellView,
            IUserNameModalView userNameModalView,
            IDetailedScoreRankPageView detailedScoreRankView,
            ISettingsModalView settingsPanelView,
            ILicenseModalView licenseModalView,
            ModalBackgroundView modalBackgroundView)
        {
            TitlePageView = titlePanellView;
            UserNameModalView = userNameModalView;
            DetailedScoreRankPageView = detailedScoreRankView;
            SettingsPageView = settingsPanelView;
            LicenseModalView = licenseModalView;
            ModalBackgroundView = modalBackgroundView;
        }

        private void OnDestroy()
        {
            _titlePageRoot = null;
            _titlePageMainElements = null;
            _loadingPageRoot = null;

            TitlePageView = null;
            UserNameModalView = null;
            DetailedScoreRankPageView = null;
            SettingsPageView = null;
            LicenseModalView = null;
            ModalBackgroundView = null;
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
