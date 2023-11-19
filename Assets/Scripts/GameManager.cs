using UnityEngine;
using SuikaGameClone;

namespace SuikaGameClone
{
    public class GameManager : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] private Sphere[] _spherePrefab;
        [SerializeField] private Transform _spherePosition;
        [SerializeField] private GameView _gameView;
        [SerializeField] private Ceiling _ceiling;


        private GameModel _gameModel = new GameModel();
        private readonly float _invokeTime = 1.0f;

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

            SetBestScore();
            CreateSphere();
        }

        void Update()
        {
            CheckGameOver();

            if (IsNext)
            {
                IsNext = false;
                Invoke("CreateSphere", _invokeTime);
            }
        }

        public void SetCurrentScore(int _sphereNo)
        {
            _gameModel.CalcScore(_sphereNo);
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
            sphereIns._sphereNo = i;
            sphereIns.gameObject.SetActive(true);
        }

        public void MergeNext(Vector3 target, int _sphereNo)
        {
            SetGameState(GameModel.GameState.Merging);

            Sphere sphereIns = Instantiate(_spherePrefab[_sphereNo + 1], target, Quaternion.identity, _spherePosition);
            sphereIns._sphereNo = _sphereNo + 1;
            sphereIns._isDrop = true;
            sphereIns.GetComponent<Rigidbody2D>().simulated = true;
            sphereIns.gameObject.SetActive(true);
            SetCurrentScore(_sphereNo);
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
    }
}