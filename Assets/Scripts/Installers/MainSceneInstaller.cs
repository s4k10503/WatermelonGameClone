using UnityEngine;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using WatermelonGameClone.Presentation;
using WatermelonGameClone.Infrastructure;


namespace WatermelonGameClone.Installers
{
    public sealed class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _inputEventProvider;
        [SerializeField] private GameObject _screenshotHandler;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private GameObject[] _nextItemImages;
        [SerializeField] private Transform _mergeItemPosition;
        [SerializeField] private GameObject[] _mergeItemPrefabs;

        public override void InstallBindings()
        {
            // Services
            Container
                .Bind<IMergeItemIndexService>()
                .To<MergeItemIndexService>()
                .FromNew()
                .AsSingle();

            Container
                .BindInterfacesTo<MergeItemUseCase>()
                .FromNew()
                .AsSingle();

            // View
            Container
                .Bind<MainSceneView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<StageView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<LoadingPanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<GameObject[]>()
                .WithId("NextItemImages")
                .FromInstance(_nextItemImages);

            Container
                .Bind<Transform>()
                .WithId("ItemPosition")
                .FromInstance(_mergeItemPosition)
                .AsCached();

            Container
                .Bind<GameObject[]>()
                .WithId("ItemPrefabs")
                .FromInstance(_mergeItemPrefabs)
                .AsCached();

            Container
                .Bind<int>()
                .WithId("MaxItemNo")
                .FromInstance(_mergeItemPrefabs.Length);

            Container
                .Bind<Camera>()
                .WithId("Main Camera")
                .FromInstance(_mainCamera);

            Container
                .Bind<Camera>()
                .WithId("UI Camera")
                .FromInstance(_uiCamera);

            Container
                .Bind<IMergeItemManager>()
                .To<MergeItemManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IScoreRankView>()
                .To<ScoreRankView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IDetailedScoreRankView>()
                .To<DetailedScoreRankView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IGameOverPanelView>()
                .To<GameOverPanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IPausePanelView>()
                .To<PausePanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IBackgroundPanelView>()
                .To<BackgroundPanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IScorePanelView>()
                .To<ScorePanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<INextItemPanelView>()
                .To<NextItemPanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IScreenshotHandler>()
                .To<ScreenshotHandler>()
                .FromComponentInNewPrefab(_screenshotHandler)
                .AsSingle();

            // Presenter
            Container
                .BindInterfacesTo<MainScenePresenter>()
                .AsSingle()
                .NonLazy();
        }
    }
}
