using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class GameViewInstaller : MonoInstaller
    {
        [SerializeField] ScoreRankView _scoreRankView;
        [SerializeField] Transform _canvasTransform;
        [SerializeField] GameObject _gameOverPopupPanel;
        [SerializeField] GameObject[] _nextSphereImages;

        public override void InstallBindings()
        {
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
                .FromComponentInNewPrefab(_gameOverPopupPanel)
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