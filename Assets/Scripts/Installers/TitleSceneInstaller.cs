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
                .Bind<ITitlePageView>()
                .To<TitlePageView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IDetailedScoreRankPageView>()
                .To<DetailedScoreRankPageView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ISettingsModalView>()
                .To<SettingsModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ILicenseModalView>()
                .To<LicenseModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<LoadingPageView>()
                .FromComponentInHierarchy()
                .AsCached();
            
            Container
                .Bind<ModalBackgroundView>()
                .FromComponentInHierarchy()
                .AsSingle();

            // Presenter
            Container
                .BindInterfacesTo<TitleScenePresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
