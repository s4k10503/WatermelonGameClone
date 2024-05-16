using UnityEngine;
using Zenject;

namespace WatermelonGameClone
{
    public class TitleSceneInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _titleSceneView;
        [SerializeField] private GameObject _titlePanelView;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<TitleScenePresenter>()
                .AsCached()
                .NonLazy();

            Container
                .Bind<ITitleSceneView>()
                .To<TitleSceneView>()
                .FromComponentInNewPrefab(_titleSceneView)
                .AsCached();

            Container
                .Bind<ITitlePanelView>()
                .To<TitlePanelView>()
                .FromComponentInHierarchy(_titlePanelView)
                .AsCached();
        }
    }
}