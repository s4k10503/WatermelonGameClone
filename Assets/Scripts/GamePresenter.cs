using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

namespace WatermelonGameClone
{
    public class GamePresenter : MonoBehaviour
    {
        [Header("View Elements")]
        [SerializeField] private GameView _gameView;
        [SerializeField] private Sphere[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;

        [Header("Parameters")]
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        //Model Elements
        private GameModel _gameModel;

        private bool _isNext;
        private int _maxSphereNo;
        public static GamePresenter Instance { get; private set; }

        private static readonly float s_delayedTime = 1.0f;
        private AudioSource _audioSourceEffect;

        // ReactiveProperty
        private Subject<Sphere> _onSphereCreated = new Subject<Sphere>();
        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        private CompositeDisposable _disposables = new CompositeDisposable();

        void Awake()
        {
            Instance = this;
            _isNext = false;
            _maxSphereNo = _spherePrefab.Length;

            _audioSourceEffect = gameObject.AddComponent<AudioSource>();

            _gameView = GameObject.Find("GameView").GetComponent<GameView>();
            _gameView.Initialize();

            _gameModel = new GameModel();
            _gameModel.SetSoundEffect();
            _gameModel.SetSoundVolume(_audioVolume);

            _gameModel.SetGameState(GameState.Initializing);
            _gameModel.SetBestScore();
            _gameView.UpdateBestScore(_gameModel.BestScore.Value);
        }

        void Start()
        {
            SubscribeToGameView(_gameView);
            SubscribeToGameModel(_gameModel);
            SubscribeToNextSphereIndex(_nextSphereIndex);
            SubscribeToSphere(_onSphereCreated);

            UpdateNextSphereIndex(_nextSphereIndex);
            DOVirtual.DelayedCall(s_delayedTime, () =>
            {
                CreateSphere();
            });
        }

        void Update()
        {
            _gameView.MoveUI();

            if (_isNext)
            {
                _isNext = false;

                DOVirtual.DelayedCall(s_delayedTime, () =>
                {
                    if (_gameModel.CurrentState.Value != GameState.GameOver)
                    {
                        CreateSphere();
                    }
                    UpdateNextSphereIndex(_nextSphereIndex);
                });
            }
        }

        private void OnDestroy()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
            }
        }

        private void SubscribeToGameView(GameView gameView)
        {
            gameView.OnRestartRequested.Subscribe(_ =>
            {
                HandleRestart();
            })
            .AddTo(_disposables);

            gameView.OnBackToTitleRequested.Subscribe(_ =>
            {
                HandleBackToTitle();
            })
            .AddTo(_disposables);
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
                            gameModel.PlaySoundEffect(SoundEffect.Drop, _audioSourceEffect);
                            break;

                        case GameState.Merging:
                            gameModel.PlaySoundEffect(SoundEffect.Merge, _audioSourceEffect);
                            break;

                        case GameState.GameOver:
                            HandleGameOver();
                            break;
                    }
                })
                .AddTo(_disposables);

            gameModel.CurrentScore
                .Subscribe(score =>
                {
                    _gameView.UpdateCurrentScore(score);
                })
                .AddTo(_disposables);
        }

        private void SubscribeToSphere(Subject<Sphere> onSphereCreated)
        {
            onSphereCreated.Subscribe(sphere =>
            {
                sphere.OnMovingRequest
                    .Subscribe(request =>
                    {
                        _gameModel.SetGameState(GameState.SphereMoving);
                    })
                    .AddTo(_disposables);

                sphere.OnDroppingRequest
                    .Subscribe(request =>
                    {
                        _isNext = true;
                        _gameModel.SetGameState(GameState.SphereDropping);
                    })
                    .AddTo(_disposables);

                sphere.OnMergingRequest
                    .Subscribe(mergeData =>
                    {
                        //MergeSphere(mergeData.Position, mergeData.SphereNo);
                        _gameModel.SetGameState(GameState.Merging);
                    })
                    .AddTo(_disposables);

                sphere.OnGameOverRequest
                    .Subscribe(request =>
                    {
                        _gameModel.SetGameState(GameState.GameOver);
                    })
                    .AddTo(_disposables);
            })
            .AddTo(_disposables);
        }

        private void SubscribeToNextSphereIndex(ReactiveProperty<int> nextSphereIndex)
        {
            nextSphereIndex
                .Subscribe(index =>
                {
                    _gameView.UpdateNextSphereImages(index);
                })
                .AddTo(_disposables);
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

        private void CreateSphere()
        {
            _gameModel.SetGameState(GameState.SphereMoving);

            Sphere sphereIns = Instantiate(_spherePrefab[_nextSphereIndex.Value], _spherePosition);
            sphereIns.Initialize(_maxSphereNo, _nextSphereIndex.Value);
            sphereIns.gameObject.SetActive(true);

            _onSphereCreated.OnNext(sphereIns);
        }

        private void UpdateNextSphereIndex(ReactiveProperty<int> nextSphereIndex)
        {
            int maxIndex = _maxSphereNo / 2 - 1;
            nextSphereIndex.Value = Random.Range(0, maxIndex + 1);
        }

        public void MergeSphere(Vector3 position, int sphereNo)
        {
            Debug.Log("spwn");
            Sphere sphereIns = Instantiate(_spherePrefab[sphereNo + 1], position, Quaternion.identity, _spherePosition);
            sphereIns.InitializeAfterMerge(_maxSphereNo, sphereNo + 1);
            sphereIns.GetComponent<Rigidbody2D>().simulated = true;
            sphereIns.gameObject.SetActive(true);

            _gameModel.SetCurrentScore(sphereNo);

        }
    }
}