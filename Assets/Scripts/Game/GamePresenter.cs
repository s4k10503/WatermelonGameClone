using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniRx;
using DG.Tweening;
using Zenject;

namespace WatermelonGameClone
{
    public class GamePresenter : MonoBehaviour
    {
        // Parameters
        private float _audioVolume;

        // View Elements
        private IGameView _gameView;
        private Transform _spherePosition;
        private GameObject[] _spherePrefabs;

        // DI Container
        private DiContainer _container = null;

        // Model Elements
        private ISphereModel _sphereModel;
        private IGameModel _gameModel;
        private IScoreModel _scoreModel;
        private ISoundModel _soundModel;

        private bool _isNext;
        private AudioSource _audioSourceEffect;

        // ReactiveProperties

        // Subjects
        private Subject<SphereView> _onSphereCreated = new Subject<SphereView>();

        [Inject]
        public void Construct(
            IGameView gameView,
            DiContainer container,
            ISphereModel sphereModel,
            IGameModel gameModel,
            IScoreModel scoreModel,
            ISoundModel soundModel,
            [Inject(Id = "AudioVolume")] float audioVolume,
            [Inject(Id = "SpherePosition")] Transform spherePosition,
            [Inject(Id = "SpherePrefabs")] GameObject[] spherePrefabs)
        {
            _gameView = gameView;
            _spherePosition = spherePosition;
            _spherePrefabs = spherePrefabs;
            _container = container;
            _sphereModel = sphereModel;
            _gameModel = gameModel;
            _scoreModel = scoreModel;
            _soundModel = soundModel;
            _audioVolume = audioVolume;
        }

        void Awake()
        {
            _onSphereCreated.AddTo(this);

            _isNext = false;
            _audioSourceEffect = gameObject.AddComponent<AudioSource>();

            _soundModel.SetSoundVolume(_audioVolume);
            _gameModel.SetGameState(GameState.Initializing);
            _sphereModel.UpdateNextSphereIndex();
            UpdateScoreDisplays();
        }

        void Start()
        {
            SubscribeToGameView(_gameView);
            SubscribeToScoreModel(_scoreModel);
            SubscribeToSphereView(_onSphereCreated);
            SubscribeToSphereModel(_sphereModel);

            SphereView sphere = CreateSphere(_sphereModel.NextSphereIndex.Value);
            _onSphereCreated.OnNext(sphere);

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    _gameView.MoveUI();

                    if (_isNext)
                    {
                        _isNext = false;
                        DOVirtual.DelayedCall(_gameModel.GetDelayedTime(), () =>
                        {
                            if (_gameModel.CurrentState.Value != GameState.GameOver)
                            {
                                SphereView sphere = CreateSphere(_sphereModel.NextSphereIndex.Value);
                                _onSphereCreated.OnNext(sphere);

                                _sphereModel.UpdateNextSphereIndex();
                            }
                        });
                    }
                })
                .AddTo(this);
        }

        private void OnDestroy()
        {

        }

        private void SubscribeToGameView(IGameView gameView)
        {
            gameView
                .OnRestart
                .Subscribe(_ =>
                {
                    HandleRestart();
                })
                .AddTo(this);

            gameView
                .OnBackToTitle
                .Subscribe(_ =>
                {
                    HandleBackToTitle();
                })
                .AddTo(this);
        }

        private void SubscribeToScoreModel(IScoreModel scoreModel)
        {
            scoreModel
                .CurrentScore
                .Subscribe(score =>
                {
                    _gameView.UpdateCurrentScore(score);
                })
                .AddTo(this);

            scoreModel
                .BestScore
                .Subscribe(score =>
                {
                    _gameView.UpdateBestScore(score);
                })
                .AddTo(this);
        }

        private void SubscribeToSphereModel(ISphereModel sphereModel)
        {
            sphereModel
                .NextSphereIndex
                .Subscribe(index =>
                {
                    _gameView.UpdateNextSphereImages(index);
                })
                .AddTo(this);
        }

        private void SubscribeToSphereView(Subject<SphereView> onSphereCreated)
        {
            onSphereCreated
                .Subscribe(sphere =>
                {
                    sphere
                        .OnDropping
                        .Subscribe(request =>
                        {
                            _gameModel.SetGameState(GameState.SphereDropping);
                            _soundModel.PlaySoundEffect(SoundEffect.Drop, _audioSourceEffect);
                            _isNext = true;
                        })
                        .AddTo(this);

                    sphere
                        .OnMerging
                        .Subscribe(mergeData =>
                        {
                            _gameModel.SetGameState(GameState.Merging);
                            _soundModel.PlaySoundEffect(SoundEffect.Merge, _audioSourceEffect);
                            _scoreModel.UpdateCurrentScore(mergeData.SphereNo);

                            SphereView sphere = MergeSphere(mergeData.Position, mergeData.SphereNo);
                            _onSphereCreated.OnNext(sphere);

                            DestroySphere(mergeData.SphereA);
                            DestroySphere(mergeData.SphereB);
                        })
                        .AddTo(this);

                    sphere
                        .OnGameOver
                        .Subscribe(request =>
                        {
                            _gameModel.SetGameState(GameState.GameOver);
                            HandleGameOver();
                        })
                        .AddTo(this);
                })
                .AddTo(this);
        }

        private void HandleGameOver()
        {
            _scoreModel.UpdateTodayTopScores(_scoreModel.CurrentScore.Value);
            _scoreModel.SaveScoresToJson();
            UpdateScoreDisplays();

            Time.timeScale = 0f;
            _gameView.ShowGameOverPopup(_scoreModel.CurrentScore.Value);
        }

        private void HandleRestart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleBackToTitle()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Title");
        }

        public SphereView CreateSphere(int sphereNo)
        {
            GameObject sphereObj = _container.InstantiatePrefab(_spherePrefabs[sphereNo], _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.Initialize(_sphereModel.MaxSphereNo, sphereNo);
            sphere.gameObject.SetActive(true);
            return sphere;
        }

        public SphereView MergeSphere(Vector3 position, int sphereNo)
        {
            GameObject sphereObj = _container.InstantiatePrefab(_spherePrefabs[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.InitializeAfterMerge(_sphereModel.MaxSphereNo, sphereNo + 1);
            sphere.GetComponent<Rigidbody2D>().simulated = true;
            sphere.gameObject.SetActive(true);
            return sphere;
        }

        public void DestroySphere(GameObject sphere)
        {
            Destroy(sphere.gameObject);
        }

        private void UpdateScoreDisplays()
        {
            _gameView.UpdateBestScore(_scoreModel.BestScore.Value);
            _gameView.DisplayTodayTopScores(_scoreModel.TodayTopScores);
        }
    }
}