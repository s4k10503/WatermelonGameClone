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
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private SphereView[] _spherePrefab;

        [Header("Parameters")]
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        // Model Elements
        private SphereModel _sphereModel;
        private GameModel _gameModel;
        private ScoreModel _scoreModel;
        private SoundModel _soundModel;

        private bool _isNext;
        private AudioSource _audioSourceEffect;

        // ReactiveProperty

        // Observables
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

            _scoreModel = new ScoreModel();
            _scoreModel.SetBestScore();

            _gameView.Initialize();
            _gameView.UpdateBestScore(_scoreModel.BestScore.Value);

            _sphereModel = new SphereModel();
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

            DOVirtual.DelayedCall(_gameModel.GetDelayedTime(), () =>
            {
                SphereView sphere = CreateSphere();
                _onSphereCreated.OnNext(sphere);
            });

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
                                SphereView sphere = CreateSphere();
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
        }

        private void SubscribeToScoreModel(ScoreModel scoreModel)
        {
            scoreModel.CurrentScore
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

        private void SubscribeToSphereView(Subject<SphereView> onSphereCreated)
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
                        MergeSphere(mergeData.Position, mergeData.SphereNo);
                        _scoreModel.SetCurrentScore(mergeData.SphereNo);
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

        public SphereView CreateSphere()
        {
            int index = _sphereModel.NextSphereIndex.Value;
            SphereView sphere = Instantiate(_spherePrefab[index], _spherePosition);
            sphere.Initialize(_sphereModel.MaxSphereNo, index);
            sphere.gameObject.SetActive(true);
            return sphere;
        }

        public void MergeSphere(Vector3 position, int sphereNo)
        {
            SphereView sphere = Instantiate(_spherePrefab[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            sphere.InitializeAfterMerge(_sphereModel.MaxSphereNo, sphereNo + 1);
            sphere.GetComponent<Rigidbody2D>().simulated = true;
            sphere.gameObject.SetActive(true);
        }
    }
}