using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class GameViewInstaller : MonoInstaller
    {
        [SerializeField] GameObject _inputEventProvider;
        [SerializeField] ScoreRankView _scoreRankView;
        [SerializeField] Transform _canvasTransform;
        [SerializeField] GameObject[] _nextSphereImages;

        public override void InstallBindings()
        {
            Container
                .Bind<IInputEventProvider>()
                .To<InputEventProvider>()
                .FromComponentInNewPrefab(_inputEventProvider)
                .AsCached();

            Container
                .Bind<IScoreRankView>()
                .To<ScoreRankView>()
                .FromComponentInNewPrefab(_scoreRankView)
                .AsCached();

            Container
                .Bind<Transform>()
                .WithId("CanvasTransform")
                .FromInstance(_canvasTransform);

            Container
                .Bind<GameObject[]>()
                .WithId("NextSphereImages")
                .FromInstance(_nextSphereImages);

            Container.Bind<IGameOverPanelView>()
                .To<GameOverPanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container.Bind<IPausePanelView>()
                .To<PausePanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container.Bind<IBackgroundPanelView>()
                .To<BackgroundPanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<IScorePanelView>()
                .To<ScorePanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<INextSpherePanelView>()
                .To<NextSpherePanelView>()
                .FromComponentInHierarchy()
                .AsCached();

            Container
                .Bind<IUIAnimator>()
                .To<UIAnimator>()
                .FromNew()
                .AsTransient();
        }
    }
}