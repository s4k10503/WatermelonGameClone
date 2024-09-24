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
            // Presenter
            Container
                .BindInterfacesTo<TitleScenePresenter>()
                .AsSingle()
                .NonLazy();

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
                .Bind<LoadingPanelView>()
                .FromComponentInHierarchy()
                .AsCached();
        }
    }
}
