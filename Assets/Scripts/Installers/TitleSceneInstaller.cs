using Domain.Interfaces;
using UseCase.Interfaces;
using UseCase.UseCases.Common;
using Infrastructure.Repositories;
using Presentation.Interfaces;
using Presentation.Presenter;
using Presentation.View.Common;
using Presentation.View.TitleScene;

using Zenject;

namespace Installers
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
                .Bind<IUserNameModalView>()
                .To<UserNameModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IDetailedScoreRankPageView>()
                .To<DetailedScoreRankPageView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ISettingsPageView>()
                .To<SettingsPageView>()
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
