using UnityEngine;
using System;
using UniRx;
using Zenject;

namespace WatermelonGameClone
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _inputEventProvider;
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private GameView _gameView;
        [SerializeField] private GameObject[] _spherePrefab;
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        public override void InstallBindings()
        {
            Container
                .Bind<IInputEventProvider>()
                .To<InputEventProvider>()
                .FromComponentInNewPrefab(_inputEventProvider)
                .AsCached();

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
                .FromInstance(_spherePrefab)
                .AsCached();

            Container
                .Bind<float>()
                .WithId("AudioVolume")
                .FromInstance(_audioVolume);
        }

    }
}