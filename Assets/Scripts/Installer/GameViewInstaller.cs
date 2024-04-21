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

        [SerializeField] GameObject _textScoreRank1;
        [SerializeField] GameObject _textScoreRank2;
        [SerializeField] GameObject _textScoreRank3;
        [SerializeField] GameObject _textScoreCurrent;

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

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank1")
                .FromInstance(_textScoreRank1);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank2")
                .FromInstance(_textScoreRank2);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreRank3")
                .FromInstance(_textScoreRank3);

            Container
                .Bind<GameObject>()
                .WithId("TextScoreCurrent")
                .FromInstance(_textScoreCurrent);
        }

    }
}