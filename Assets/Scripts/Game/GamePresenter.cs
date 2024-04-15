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
        [Inject(Id = "AudioVolume")] private float _audioVolume;

        // View Elements
        [Inject] private IGameView _gameView;
        [Inject(Id = "SpherePosition")] private Transform _spherePosition;
        [Inject(Id = "SpherePrefabs")] private GameObject[] _spherePrefab;

        // DI Container
        [Inject] DiContainer container = null;

        // Model Elements
        [Inject] private ISphereModel _sphereModel;
        [Inject] private IGameModel _gameModel;
        [Inject] private IScoreModel _scoreModel;
        [Inject] private ISoundModel _soundModel;

        private bool _isNext;
        private AudioSource _audioSourceEffect;

        // ReactiveProperty

        // Subjects
        private Subject<SphereView> _onSphereCreated = new Subject<SphereView>();

        void Awake()
        {
            _onSphereCreated.AddTo(this);

            _isNext = false;
            _audioSourceEffect = gameObject.AddComponent<AudioSource>();

            _soundModel.SetSoundEffect();
            _soundModel.SetSoundVolume(_audioVolume);

            _gameModel.SetGameState(GameState.Initializing);

            _scoreModel.SetBestScore();

            _gameView.Initialize();
            _gameView.UpdateBestScore(_scoreModel.BestScore.Value);

            _sphereModel.Initialize(_spherePrefab.Length);
            _sphereModel.UpdateNextSphereIndex();
        }

        void Start()
        {
            SubscribeToGameView(_gameView);
            SubscribeToGameModel(_gameModel);

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

        private void SubscribeToGameModel(IGameModel gameModel)
        {
            gameModel
                .CurrentState
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case GameState.Initializing:
                            break;

                        case GameState.SphereMoving:
                            break;

                        case GameState.SphereDropping:
                            _soundModel.PlaySoundEffect(SoundEffect.Drop, _audioSourceEffect);
                            break;

                        case GameState.Merging:
                            _soundModel.PlaySoundEffect(SoundEffect.Merge, _audioSourceEffect);
                            break;

                        case GameState.GameOver:
                            HandleGameOver();
                            break;
                    }
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
                        .OnMoving
                        .Subscribe(request =>
                        {
                            _gameModel.SetGameState(GameState.SphereMoving);
                        })
                        .AddTo(this);

                    sphere
                        .OnDropping
                        .Subscribe(request =>
                        {
                            _isNext = true;
                            _gameModel.SetGameState(GameState.SphereDropping);
                        })
                        .AddTo(this);

                    sphere
                        .OnMerging
                        .Subscribe(mergeData =>
                        {
                            MergeSphere(mergeData.Position, mergeData.SphereNo);
                            _scoreModel.SetCurrentScore(mergeData.SphereNo);
                            _gameModel.SetGameState(GameState.Merging);
                        })
                        .AddTo(this);

                    sphere
                        .OnGameOver
                        .Subscribe(request =>
                        {
                            _gameModel.SetGameState(GameState.GameOver);
                        })
                        .AddTo(this);
                })
                .AddTo(this);
        }

        private void HandleGameOver()
        {
            _scoreModel.SetBestScore();
            _gameView.UpdateBestScore(_scoreModel.BestScore.Value);

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
            GameObject sphereObj = container.InstantiatePrefab(_spherePrefab[sphereNo], _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.Initialize(_sphereModel.MaxSphereNo, sphereNo);
            sphere.gameObject.SetActive(true);
            return sphere;
        }

        public void MergeSphere(Vector3 position, int sphereNo)
        {
            GameObject sphereObj = container.InstantiatePrefab(_spherePrefab[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.InitializeAfterMerge(_sphereModel.MaxSphereNo, sphereNo + 1);
            sphere.GetComponent<Rigidbody2D>().simulated = true;
            sphere.gameObject.SetActive(true);
        }
    }
}