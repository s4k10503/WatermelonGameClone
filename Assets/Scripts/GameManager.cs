using UnityEngine;

namespace WatermelonGameClone
{
    public class GameManager : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private Sphere[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private GameView _gameView;
        [SerializeField] private Ceiling _ceiling;

        [Header("Parameters")]
        [SerializeField, Range(0, 1.0f)] private float _audioVolume;

        private GameModel _gameModel = new GameModel();
        private static readonly float s_invokeTime = 1.0f;
        private AudioSource _audioSourceEffect;

        public static GameManager Instance { get; private set; }
        public bool IsNext { get; set; }
        public int MaxSphereNo { get; private set; }


        void Start()
        {
            _gameModel.ChangeState(GameModel.GameState.Initializing);

            Instance = this;
            IsNext = false;
            MaxSphereNo = _spherePrefab.Length;

            _gameView = GameObject.Find("GameView").GetComponent<GameView>();
            _ceiling = GameObject.Find("Ceiling").GetComponent<Ceiling>();

            _audioSourceEffect = gameObject.AddComponent<AudioSource>();
            _gameModel.SetSoundEffect();
            _gameModel.SetSoundVolume(_audioVolume);

            SetBestScore();
            CreateSphere();
        }

        void Update()
        {
            CheckGameOver();

            if (IsNext)
            {
                IsNext = false;
                Invoke("CreateSphere", s_invokeTime);
            }
        }

        public void SetCurrentScore(int SphereNo)
        {
            _gameModel.CalcScore(SphereNo);
            _gameView.UpdateCurrentScore(_gameModel.CurrentScore.Value);
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
            SetGameState(GameModel.GameState.SphereMoving);

            int maxIndex = MaxSphereNo / 2 - 1;
            int i = Random.Range(0, maxIndex + 1);

            Sphere sphereIns = Instantiate(_spherePrefab[i], _spherePosition);
            sphereIns.SphereNo = i;
            sphereIns.gameObject.SetActive(true);
        }

        public void MergeNext(Vector3 target, int SphereNo)
        {
            SetGameState(GameModel.GameState.Merging);
            _gameModel.PlaySoundEffect(GameModel.SoundEffect.Merge, _audioSourceEffect);

            Sphere sphereIns = Instantiate(_spherePrefab[SphereNo + 1], target, Quaternion.identity, _spherePosition);
            sphereIns.SphereNo = SphereNo + 1;
            sphereIns.IsDrop = true;
            sphereIns.GetComponent<Rigidbody2D>().simulated = true;
            sphereIns.gameObject.SetActive(true);
            SetCurrentScore(SphereNo);
        }

        public void CheckGameOver()
        {
            if (_gameModel.CurrentState.Value == GameModel.GameState.GameOver)
            {
                SetBestScore();
                Debug.Log("Game Over");
                Time.timeScale = 0f;
            }
        }

        public GameModel.GameState GetCurrentState()
        {
            return _gameModel.CurrentState.Value;
        }

        public void SetGameState(GameModel.GameState newState)
        {
            _gameModel.ChangeState(newState);
        }

        public void PlaySoundEffect(GameModel.SoundEffect effect)
        {
            _gameModel.PlaySoundEffect(effect, _audioSourceEffect);
        }
    }
}