using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UniRx;
using DG.Tweening;

namespace WatermelonGameClone
{
    public class GamePresenter : MonoBehaviour
    {
        [Header("View Elements")]
        [SerializeField] private GameView _gameView;

        [Header("Parameters")]
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        //Model Elements
        [SerializeField] private SphereModel _sphereModel;
        private GameModel _gameModel;
        private SoundModel _soundModel;

        private bool _isNext;
        private AudioSource _audioSourceEffect;

        // ReactiveProperty
        private Subject<SphereView> _onSphereCreated = new Subject<SphereView>();

        void Awake()
        {
            _isNext = false;

            _audioSourceEffect = gameObject.AddComponent<AudioSource>();

            _soundModel = new SoundModel();
            _soundModel.SetSoundEffect();
            _soundModel.SetSoundVolume(_audioVolume);

            _gameModel = new GameModel();
            _gameModel.SetGameState(GameState.Initializing);
            _gameModel.SetBestScore();

            _gameView.Initialize();
            _gameView.UpdateBestScore(_gameModel.BestScore.Value);

            _sphereModel.Initialise();
        }

        void Start()
        {
            SubscribeToGameView(_gameView);
            SubscribeToGameModel(_gameModel);

            SubscribeToSphere(_onSphereCreated);
            SubscribeToSphereModel(_sphereModel);

            DOVirtual.DelayedCall(_gameModel.GetDeleyedTime(), () =>
            {
                SphereView sphere = _sphereModel.CreateSphere();
                _onSphereCreated.OnNext(sphere);
            });

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    _gameView.MoveUI();
                    if (_isNext)
                    {
                        _isNext = false;
                        DOVirtual.DelayedCall(_gameModel.GetDeleyedTime(), () =>
                        {
                            if (_gameModel.CurrentState.Value != GameState.GameOver)
                            {
                                SphereView sphere = _sphereModel.CreateSphere();
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

        private void SubscribeToGameView(GameView gameView)
        {
            gameView.OnRestartRequested.Subscribe(_ =>
            {
                HandleRestart();
            })
            .AddTo(this);

            gameView.OnBackToTitleRequested.Subscribe(_ =>
            {
                HandleBackToTitle();
            })
            .AddTo(this);
        }

        private void SubscribeToGameModel(GameModel gameModel)
        {
            gameModel.CurrentState
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

            gameModel.CurrentScore
                .Subscribe(score =>
                {
                    _gameView.UpdateCurrentScore(score);
                })
                .AddTo(this);
        }

        private void SubscribeToSphereModel(SphereModel sphereModel)
        {
            sphereModel.NextSphereIndex
                .Subscribe(index =>
                {
                    _gameView.UpdateNextSphereImages(index);
                })
                .AddTo(this);
        }

        private void SubscribeToSphere(Subject<SphereView> onSphereCreated)
        {
            onSphereCreated.Subscribe(sphere =>
            {
                sphere.OnMovingRequest
                    .Subscribe(request =>
                    {
                        _gameModel.SetGameState(GameState.SphereMoving);
                    })
                    .AddTo(this);

                sphere.OnDroppingRequest
                    .Subscribe(request =>
                    {
                        _isNext = true;
                        _gameModel.SetGameState(GameState.SphereDropping);
                    })
                    .AddTo(this);

                sphere.OnMergingRequest
                    .Subscribe(mergeData =>
                    {
                        _sphereModel.MergeSphere(mergeData.Position, mergeData.SphereNo);
                        _gameModel.SetCurrentScore(mergeData.SphereNo);
                        _gameModel.SetGameState(GameState.Merging);
                    })
                    .AddTo(this);

                sphere.OnGameOverRequest
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
            _gameModel.SetBestScore();
            _gameView.UpdateBestScore(_gameModel.BestScore.Value);

            Time.timeScale = 0f;
            _gameView.ShowGameOverPopup(_gameModel.CurrentScore.Value);
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
    }
}