using System;
using UnityEngine;
using UniRx;
using Zenject;

namespace WatermelonGameClone.Presentation
{
    public sealed class TitleSceneView : MonoBehaviour
    {
        [SerializeField] private Canvas _loadingPageRoot;

        public ITitlePageView TitlePageView { get; private set; }
        public IUserNameModalView UserNameModalView;
        public IDetailedScoreRankPageView DetailedScoreRankPageView { get; private set; }
        public ISettingsPageView SettingsPageView { get; private set; }
        public ILicenseModalView LicenseModalView { get; private set; }
        public ModalBackgroundView ModalBackgroundView { get; private set; }

        public IObservable<Unit> GameStartRequested => TitlePageView.OnGameStart;
        public IObservable<Unit> MyScoreRequested => TitlePageView.OnMyScore;

        [Inject]
        public void Construct(
            ITitlePageView titlePanellView,
            IUserNameModalView userNameModalView,
            IDetailedScoreRankPageView detailedScoreRankView,
            ISettingsPageView settingsPageView,
            ILicenseModalView licenseModalView,
            ModalBackgroundView modalBackgroundView)
        {
            TitlePageView = titlePanellView;
            UserNameModalView = userNameModalView;
            DetailedScoreRankPageView = detailedScoreRankView;
            SettingsPageView = settingsPageView;
            LicenseModalView = licenseModalView;
            ModalBackgroundView = modalBackgroundView;
        }

        private void OnDestroy()
        {
            _loadingPageRoot = null;

            TitlePageView = null;
            UserNameModalView = null;
            DetailedScoreRankPageView = null;
            SettingsPageView = null;
            LicenseModalView = null;
            ModalBackgroundView = null;
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
