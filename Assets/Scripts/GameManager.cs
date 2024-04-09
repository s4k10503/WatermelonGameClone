using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;

namespace WatermelonGameClone
{
    public class GameManager : MonoBehaviour
    {
        [Header("View Elements")]
        [SerializeField] private GameView _gameView;
        [SerializeField] private Sphere[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private Ceiling _ceiling;

        [Header("Parameters")]
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        //Model Elements
        private GameModel _gameModel = new GameModel();

        public static GameManager Instance { get; private set; }
        public bool IsNext { get; set; }
        public int MaxSphereNo { get; private set; }

        private static readonly float s_delayedTime = 1.0f;
        private AudioSource _audioSourceEffect;

        // ReactiveProperty
        private ReactiveProperty<GameModel.GameState> _reactiveGameState;
        public IReadOnlyReactiveProperty<GameModel.GameState> GameState => _reactiveGameState;

        private ReactiveProperty<int> _nextSphereIndex = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> NextSphereIndex => _nextSphereIndex;

        private ReactiveProperty<int> _reactiveCurrentScore;

        public ReactiveCommand<GameModel.GameState> GameEvent = new ReactiveCommand<GameModel.GameState>();

        private CompositeDisposable _disposables = new CompositeDisposable();

        void Awake()
        {
            _reactiveGameState = new ReactiveProperty<GameModel.GameState>(_gameModel.CurrentState.Value);
            _reactiveCurrentScore = new ReactiveProperty<int>(_gameModel.CurrentScore.Value);

            Instance = this;
            IsNext = false;
            MaxSphereNo = _spherePrefab.Length;

            _gameView = GameObject.Find("GameView").GetComponent<GameView>();
            _gameView.Initialize();

            _ceiling = GameObject.Find("Ceiling").GetComponent<Ceiling>();

            _audioSourceEffect = gameObject.AddComponent<AudioSource>();
            _gameModel.SetSoundEffect();
            _gameModel.SetSoundVolume(_audioVolume);

            _gameModel.SetGameState(GameModel.GameState.Initializing);
        }

        void Start()
        {
            SubscribeToGameEvents();
            SubscribeToScoreChanges();
            SubscribeToRestartRequest();
            SubscribeToTitleReturnRequest();
            SubscribeToNextSphereIndex();

            SetBestScore();

            UpdateNextSphereIndex();

            DOVirtual.DelayedCall(s_delayedTime, () =>
            {
                CreateSphere();
            });
        }

        void Update()
        {
            _gameView.MoveUI();

            if (IsNext)
            {
                IsNext = false;
                DOVirtual.DelayedCall(s_delayedTime, () =>
                {
                    if (_reactiveGameState.Value != GameModel.GameState.GameOver)
                    {
                        CreateSphere();
                    }
                    UpdateNextSphereIndex();
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

        private void SubscribeToGameEvents()
        {
            GameEvent.Subscribe(state =>
            {
                _reactiveGameState.Value = state;
                _gameModel.SetGameState(state);
                switch (state)
                {
                    case GameModel.GameState.Initializing:
                        break;

                    case GameModel.GameState.SphereMoving:
                        break;

                    case GameModel.GameState.SphereDropping:
                        _gameModel.PlaySoundEffect(GameModel.SoundEffect.Drop, _audioSourceEffect);
                        break;

                    case GameModel.GameState.Merging:
                        _gameModel.PlaySoundEffect(GameModel.SoundEffect.Merge, _audioSourceEffect);
                        break;

                    case GameModel.GameState.GameOver:
                        HandleGameOver();
                        break;
                }
            }).AddTo(_disposables);
        }

        private void SubscribeToScoreChanges()
        {
            _reactiveCurrentScore.Subscribe(score =>
            {
                _gameView.UpdateCurrentScore(score);
            }).AddTo(_disposables);
        }

        private void SubscribeToRestartRequest()
        {
            _gameView.OnRestartRequested.Subscribe(_ =>
            {
                HandleRestart();
            }).AddTo(_disposables);
        }

        private void SubscribeToTitleReturnRequest()
        {
            _gameView.OnBackToTitleRequested.Subscribe(_ =>
            {
                HandleBackToTitle();
            }).AddTo(_disposables);
        }

        private void SubscribeToNextSphereIndex()
        {
            NextSphereIndex.Subscribe(index =>
            {
                _gameView.UpdateNextSphere(index);
            }).AddTo(_disposables);
        }


        public void SetCurrentScore(int SphereNo)
        {
            _gameModel.CalcScore(SphereNo);
            _reactiveCurrentScore.Value = _gameModel.CurrentScore.Value;
        }

        public void SetBestScore()
        {
            int pastBestScore = PlayerPrefs.GetInt("BestScore");
            if (_gameModel.CurrentScore.Value > pastBestScore)
            {
                _gameModel.SaveBestScore(_gameModel.CurrentScore.Value);
                _gameView.UpdateBestScore(_gameModel.BestScore.Value);
            }
        }

        private void CreateSphere()
        {
            GameEvent.Execute(GameModel.GameState.SphereMoving);

            Sphere sphereIns = Instantiate(_spherePrefab[_nextSphereIndex.Value], _spherePosition);
            sphereIns.SphereNo = _nextSphereIndex.Value;
            sphereIns.gameObject.SetActive(true);
        }

        private void UpdateNextSphereIndex()
        {
            int maxIndex = MaxSphereNo / 2 - 1;
            _nextSphereIndex.Value = Random.Range(0, maxIndex + 1);
        }

        public void MergeNext(Vector3 target, int SphereNo)
        {
            Sphere sphereIns = Instantiate(_spherePrefab[SphereNo + 1], target, Quaternion.identity, _spherePosition);
            sphereIns.SphereNo = SphereNo + 1;
            sphereIns.IsDrop = true;
            sphereIns.GetComponent<Rigidbody2D>().simulated = true;
            sphereIns.gameObject.SetActive(true);
            SetCurrentScore(SphereNo);
        }

        private void HandleGameOver()
        {
            SetBestScore();
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
