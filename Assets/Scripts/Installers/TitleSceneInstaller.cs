using UnityEngine;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using WatermelonGameClone.Presentation;
using WatermelonGameClone.Infrastructure;

namespace WatermelonGameClone.Installers
{
    public sealed class TitleSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Repository
            Container
                .Bind<ILicenseRepository>()
                .To<JsonLicenseRepository>()
                .FromNew()
                .AsSingle();

            // UseCase
            Container
                .Bind<ILicenseUseCase>()
                .To<LicenseUseCase>()
                .FromNew()
                .AsSingle();

            // View 
            Container
                .Bind<TitleSceneView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<ITitlePanelView>()
                .To<TitlePanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IDetailedScoreRankView>()
                .To<DetailedScoreRankView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ISettingsPanelView>()
                .To<SettingsPanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ILicenseModalView>()
                .To<LicenseModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<LoadingPanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            // Presenter
            Container
                .BindInterfacesTo<TitleScenePresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
