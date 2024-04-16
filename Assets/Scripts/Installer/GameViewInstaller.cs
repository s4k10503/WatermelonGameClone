using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class GameViewInstaller : MonoInstaller
    {
        [SerializeField] GameObject _scorePanel;
        [SerializeField] GameObject _nextSpherePanel;
        [SerializeField] GameObject _rankingPanel;
        [SerializeField] GameObject _evolutionCirclePanel;
        [SerializeField] Transform _canvasTransform;
        [SerializeField] GameObject _gameOverPopupPanel;
        [SerializeField] GameObject[] _nextSphereImages;

        public override void InstallBindings()
        {
            Container
                .Bind<GameObject>()
                .WithId("ScorePanel")
                .FromInstance(_scorePanel);

            Container
                .Bind<GameObject>()
                .WithId("NextSpherePanel")
                .FromInstance(_nextSpherePanel);

            Container
                .Bind<GameObject>()
                .WithId("RankingPanel")
                .FromInstance(_rankingPanel);

            Container
                .Bind<GameObject>()
                .WithId("EvolutionCirclePanel")
                .FromInstance(_evolutionCirclePanel);

            Container
                .Bind<Transform>()
                .WithId("CanvasTransform")
                .FromInstance(_canvasTransform);

            Container
                .Bind<GameObject>()
                .WithId("GameOverPopupPanel")
                .FromInstance(_gameOverPopupPanel);

            Container
                .Bind<GameObject[]>()
                .WithId("NextSphereImages")
                .FromInstance(_nextSphereImages);
        }

    }
}