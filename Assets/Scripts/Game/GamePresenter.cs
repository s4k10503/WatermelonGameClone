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
        private IGameModel _gameModel;

        private bool _isNext;
        private AudioSource _audioSourceEffect;

        // ReactiveProperties

        // Subjects
        private Subject<SphereView> _onSphereCreated = new Subject<SphereView>();


        [Inject]
        public void Construct(
            IGameView gameView,
            DiContainer container,
            IGameModel gameModel,
            [Inject(Id = "AudioVolume")] float audioVolume,
            [Inject(Id = "SpherePosition")] Transform spherePosition,
            [Inject(Id = "SpherePrefabs")] GameObject[] spherePrefabs)
        {
            _gameView = gameView;
            _spherePosition = spherePosition;
            _spherePrefabs = spherePrefabs;
            _container = container;
            _gameModel = gameModel;
            _audioVolume = audioVolume;
        }

        void Awake()
        {
            _onSphereCreated.AddTo(this);

            _isNext = false;
            _audioSourceEffect = gameObject.AddComponent<AudioSource>();

            _gameModel.SoundModel.SetSoundVolume(_audioVolume);
            _gameModel.SetGameState(GameState.Initializing);
            _gameModel.SphereModel.UpdateNextSphereIndex();
            UpdateScoreDisplays();
        }

        void Start()
        {
            SubscribeToGameView(_gameView);
            SubscribeToSphereView(_onSphereCreated);
            SubscribeToGameModel(_gameModel);

            SphereView sphere = CreateSphere(_gameModel.SphereModel.NextSphereIndex.Value);

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (_isNext)
                    {
                        _isNext = false;
                        DOVirtual.DelayedCall(_gameModel.GetDelayedTime(), () =>
                        {
                            if (_gameModel.CurrentState.Value != GameState.GameOver)
                            {
                                SphereView sphere = CreateSphere(_gameModel.SphereModel.NextSphereIndex.Value);
                                _gameModel.SphereModel.UpdateNextSphereIndex();
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
                .RestartRequested
                .Subscribe(_ =>
                {
                    HandleRestart();
                })
                .AddTo(this);

            gameView
                .BackToTitleRequested
                .Subscribe(_ =>
                {
                    HandleBackToTitle();
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
                            _gameModel.SoundModel.PlaySoundEffect(SoundEffect.Drop, _audioSourceEffect);
                            _isNext = true;
                        })
                        .AddTo(this);

                    sphere
                        .OnMerging
                        .Subscribe(mergeData =>
                        {
                            _gameModel.SetGameState(GameState.Merging);
                            _gameModel.SoundModel.PlaySoundEffect(SoundEffect.Merge, _audioSourceEffect);
                            _gameModel.ScoreModel.UpdateCurrentScore(mergeData.SphereNo);

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

        private void SubscribeToGameModel(IGameModel gameModel)
        {
            gameModel.SphereModel
                .NextSphereIndex
                .Subscribe(index =>
                {
                    _gameView.NextSpherePanelView.UpdateNextSphereImages(index);
                })
                .AddTo(this);

            gameModel.ScoreModel
                .CurrentScore
                .Subscribe(score =>
                {
                    _gameView.ScorePanelView.UpdateCurrentScore(score);
                    _gameView.ScoreRankView.DisplayCurrentScore(score);
                })
                .AddTo(this);

            gameModel.ScoreModel
                .BestScore
                .Subscribe(score =>
                {
                    _gameView.ScorePanelView.UpdateBestScore(score);
                })
                .AddTo(this);
        }

        private void HandleGameOver()
        {
            _gameModel.ScoreModel.UpdateScoreRanking(_gameModel.ScoreModel.CurrentScore.Value);
            UpdateScoreDisplays();

            Time.timeScale = _gameModel.GetTimeScaleGameOver();
            _gameView.GameOverPanelView.ShowGameOverPopup(_gameModel.ScoreModel.CurrentScore.Value);
        }

        private void HandleRestart()
        {
            Time.timeScale = _gameModel.GetTimeScaleGameStart();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleBackToTitle()
        {
            Time.timeScale = _gameModel.GetTimeScaleGameStart();
            SceneManager.LoadScene("Title");
        }

        public SphereView CreateSphere(int sphereNo)
        {
            GameObject sphereObj = _container.InstantiatePrefab(_spherePrefabs[sphereNo], _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.Initialize(_gameModel.SphereModel.MaxSphereNo, sphereNo);
            sphere.gameObject.SetActive(true);
            _onSphereCreated.OnNext(sphere);

            return sphere;
        }

        public SphereView MergeSphere(Vector3 position, int sphereNo)
        {
            GameObject sphereObj = _container.InstantiatePrefab(_spherePrefabs[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            SphereView sphere = sphereObj.GetComponent<SphereView>();

            sphere.InitializeAfterMerge(_gameModel.SphereModel.MaxSphereNo, sphereNo + 1);
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
            _gameView.ScorePanelView.UpdateBestScore(_gameModel.ScoreModel.BestScore.Value);

            // Display of score rank for the day
            _gameView.ScoreRankView.DisplayTopScores(_gameModel.ScoreModel.TodayTopScores,
                                                    _gameModel.ScoreModel.MonthlyTopScores,
                                                    _gameModel.ScoreModel.AllTimeTopScores);
        }
    }
}