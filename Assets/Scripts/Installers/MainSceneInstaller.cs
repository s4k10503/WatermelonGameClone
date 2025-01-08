using UnityEngine;
using Zenject;
using WatermelonGameClone.Domain;
using WatermelonGameClone.UseCase;
using WatermelonGameClone.Presentation;

namespace WatermelonGameClone.Installers
{
    public sealed class MainSceneInstaller : MonoInstaller
    {
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
                .Bind<IMergeService>()
                .To<MergeService>()
                .FromNew()
                .AsSingle();

            // UseCases
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
                .Bind<LoadingPageView>()
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
                .To<ScoreRankPanelView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IDetailedScoreRankPageView>()
                .To<DetailedScoreRankPageView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IGameOverModalView>()
                .To<GameOverModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<IPauseModalView>()
                .To<PauseModalView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container
                .Bind<ModalBackgroundView>()
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
