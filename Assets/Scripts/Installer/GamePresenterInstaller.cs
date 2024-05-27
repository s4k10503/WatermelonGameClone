using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class GamePresenterInstaller : MonoInstaller
    {
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private GameView _gameView;
        [SerializeField] private GameObject[] _spherePrefabs;
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;
        [SerializeField] private GameObject _screenshotHandler;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uiCamera;

        public override void InstallBindings()
        {
            Container
                .Bind<IGameModel>()
                .To<GameModel>()
                .FromNew()
                .AsCached();

            Container
                .Bind<IScoreModel>()
                .To<ScoreModel>()
                .FromNew()
                .AsCached();

            Container
                .Bind<ISoundModel>()
                .To<SoundModel>()
                .FromNew()
                .AsCached();

            Container
                .Bind<ISphereModel>()
                .To<SphereModel>()
                .FromNew()
                .AsCached();

            Container
                .Bind<Transform>()
                .WithId("SpherePosition")
                .FromInstance(_spherePosition)
                .AsCached();

            Container
                .Bind<IGameView>()
                .To<GameView>()
                .FromComponentInNewPrefab(_gameView)
                .AsCached();

            Container
                .Bind<GameObject[]>()
                .WithId("SpherePrefabs")
                .FromInstance(_spherePrefabs)
                .AsCached();

            Container
                .Bind<float>()
                .WithId("AudioVolume")
                .FromInstance(_audioVolume);

            Container
                .Bind<int>()
                .WithId("MaxSphereNo")
                .FromInstance(_spherePrefabs.Length);

            Container
                .Bind<IScreenshotHandler>()
                .To<ScreenshotHandler>()
                .FromComponentInNewPrefab(_screenshotHandler)
                .AsCached();

            Container
                .Bind<Camera>()
                .WithId("Main Camera")
                .FromInstance(_mainCamera);

            Container
                .Bind<Camera>()
                .WithId("UI Camera")
                .FromInstance(_uiCamera);
        }

    }
}